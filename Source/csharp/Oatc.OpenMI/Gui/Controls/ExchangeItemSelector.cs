#region Copyright

/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using log4net;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Controls
{
    /// <summary>
    /// This control represents tree of exchange items. First level nodes represent
    /// <see cref="IQuantity">IQuantity</see>, second level nodes represent 
    /// <see cref="IElementSet">IElementSet</see> and 
    /// third level nodes represent <see cref="IOutputItemDecorator">IDataOperation</see>s in case of 
    /// <see cref="ITimeSpaceExchangeItem">IExchangeItem</see>s.
    /// </summary>	
    public class ExchangeItemSelector : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExchangeItemSelector));

        #region Form controls

        private TreeView treeView;

        #endregion

        #region Public events

        /// <summary>
        /// Occurs when the selection of tree node changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the check-state of checkboxes in the tree changes.
        /// </summary>
        public event EventHandler ExchangeItemChanged;

        #endregion

        #region Member variables

        List<UIExchangeItem> _items
            = new List<UIExchangeItem>();
        Dictionary<ITimeSpaceExchangeItem, TreeNode> _nodes
            = new Dictionary<ITimeSpaceExchangeItem, TreeNode>();

        private ImageList imageList;

        private IContainer components;

        private ITimeSpaceComponent _linkableComponent = null;

        TreeOptions _treeOptions;

        private bool _ignoreCheckEvents = false;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="ExchangeItemSelector">ExchangeItemSelector</see> control.
        /// </summary>
        public ExchangeItemSelector()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        // ADH: ToDo need to validate that a LC exchenge item identifiers are all unique, also for factories?

        public struct TreeOptions
        {
            public bool ShowCheckboxs;
            public bool FilterTargetByElementType;
            public bool FilterTargetByDimension;
            public bool IsSource;
            public bool IsTarget;
        }

        public void TreePopulate(TreeOptions options)
        {
            _linkableComponent = null;
            _treeOptions = options;

            _items.Clear();

            TreeCreate(null);
        }

        public void TreePopulate(ITimeSpaceComponent linkableComponent, TreeOptions options)
        {
            _linkableComponent = linkableComponent;
            _treeOptions = options;

            _items = _treeOptions.IsSource
                ? Sources(linkableComponent)
                : Targets(linkableComponent);

            TreeCreate(null);
        }

        public void TreePopulate(ITimeSpaceComponent source, ITimeSpaceComponent target, TreeOptions options)
        {
            TreePopulate(source, target, null, options);
        }

        public void TreePopulate(UIConnection connection, TreeOptions options)
        {
            ITimeSpaceComponent source = connection.SourceModel.LinkableComponent;
            ITimeSpaceComponent target = connection.TargetModel.LinkableComponent;

            List<UIOutputItem> extraSources = new List<UIOutputItem>();

            UIOutputItem src;

            foreach (Link pair in connection.Links)
            {
                src = pair.Source;

                //while (src.Parent != null) // decorator
                //{
                //    extraSources.Add(src);
                //    src = src.Parent;
                //}
            }

            TreePopulate(source, target, extraSources, options);
        }

        public void TreeAdd(List<UIOutputItem> extraSources)
        {
            Debug.Assert(_treeOptions.IsSource);

            if (extraSources != null && extraSources.Count > 0)
            {
                _items.AddRange(extraSources.ToArray());

                TreeCreate(extraSources[extraSources.Count - 1]);
            }
        }

        public void TreePopulate(ITimeSpaceComponent source, ITimeSpaceComponent target, List<UIOutputItem> extraSources, TreeOptions options)
        {
            _treeOptions = options;

            List<UIExchangeItem> sources = Sources(source);
            List<UIExchangeItem> targets = Targets(target);

            UIExchangeItem toCheck = null;

            if (extraSources != null)
            {
                sources.AddRange(extraSources.ToArray());

                if (extraSources.Count > 0)
                    toCheck = extraSources[0];
            }

            Filter(sources, targets);

            if (_treeOptions.IsSource)
            {
                _linkableComponent = source;
                _items = sources;
            }
            else if (_treeOptions.IsTarget)
            {
                _linkableComponent = target;
                _items = targets;
            }
            else
            {
                _linkableComponent = null;
                _items = new List<UIExchangeItem>();
            }

            TreeCreate(toCheck);
        }

        static List<UIExchangeItem> Sources(ITimeSpaceComponent source)
        {
            if (source == null)
                return new List<UIExchangeItem>();

            List<UIExchangeItem> sources
                = new List<UIExchangeItem>(source.Outputs.Count);

            foreach (ITimeSpaceOutput item in source.Outputs)
                sources.Add(new UIOutputItem(item));

            return sources;
        }

        static List<UIExchangeItem> Targets(ITimeSpaceComponent target)
        {
            if (target == null)
                return new List<UIExchangeItem>();

            List<UIExchangeItem> targets
                = new List<UIExchangeItem>(target.Inputs.Count);

            foreach (ITimeSpaceInput item in target.Inputs)
                targets.Add(new UIInputItem(item));

            return targets;
        }

        void Filter(List<UIExchangeItem> sources, List<UIExchangeItem> targets)
        {
            // nasty n squared stuff

            if (_treeOptions.FilterTargetByElementType)
            {
                // ADH: Filters expensive need caching 

                FilterOnElementSetTypes filter = new FilterOnElementSetTypes(sources);

                List<UIExchangeItem> filtered = targets.FindAll(filter.Predicate);

                targets = filtered;
            }

            if (_treeOptions.FilterTargetByDimension)
            {
                FilterOnDimension filter = new FilterOnDimension(sources);

                List<UIExchangeItem> filtered = targets.FindAll(filter.Predicate);

                targets = filtered;
            }
        }

        class FilterOnElementSetTypes
        {
            List<ElementType> _types = new List<ElementType>();

            public FilterOnElementSetTypes(List<UIExchangeItem> items)
            {
                _types = new List<ElementType>();

                foreach (UIExchangeItem item in items)
                {
                    if (!_types.Contains(item.ElementSet.ElementType))
                        _types.Add(item.ElementSet.ElementType);

                    if (_types.Count == 4)
                        break;
                }
            }

            public bool Predicate(UIExchangeItem item)
            {
                return _types.Contains(item.ElementSet.ElementType);
            }
        }

        class FilterOnDimension
        {
            List<IDimension> _dimensions = new List<IDimension>();

            public FilterOnDimension(List<UIExchangeItem> items)
            {
                if (items.Count > 0)
                {
                    CompareDimensions compare = new CompareDimensions();

                    items.Sort(compare);

                    for (int n = 0; n < items.Count; ++n)
                        if (items[n].ValueDefinition is IQuantity)
                            if (_dimensions.Count == 0
                                || !Dimension.DescribesSameAs(_dimensions[_dimensions.Count - 1],
                                ((IQuantity)items[n].ValueDefinition).Unit.Dimension))
                                _dimensions.Add(((IQuantity)items[n].ValueDefinition).Unit.Dimension);
                }
            }

            static bool DimensionsAreEqual(UIExchangeItem x, UIExchangeItem y)
            {
                return x.ValueDefinition is IQuantity
                        && y.ValueDefinition is IQuantity
                        && Dimension.DescribesSameAs(((IQuantity)x.ValueDefinition).Unit.Dimension,
                            ((IQuantity)y.ValueDefinition).Unit.Dimension);
            }

            class CompareDimensions : IComparer<UIExchangeItem>
            {
                #region IComparer<UIExchangeItem> Members

                public int Compare(UIExchangeItem x, UIExchangeItem y)
                {
                    if (DimensionsAreEqual(x, y))
                        return 0;

                    return -1;
                }

                #endregion
            }

            public bool Predicate(UIExchangeItem item)
            {
                if (!(item.ValueDefinition is IQuantity))
                    return false;

                foreach (IDimension d in _dimensions)
                    if (Dimension.DescribesSameAs(d, ((IQuantity)item.ValueDefinition).Unit.Dimension))
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Creates the tree based on element sets and quantities in exchange items
        /// passed with <see cref="PopulateExchangeItemTree">PopulateExchangeItemTree</see> method.
        /// </summary>
        public void TreeCreate(UIExchangeItem toCheck)
        {
            _nodes.Clear();

            if (_items == null)
            {
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                treeView.EndUpdate();
                return;
            }

            ExchangeItemsComparer comparer = new ExchangeItemsComparer();
            _items.Sort(comparer);

            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.CheckBoxes = _treeOptions.ShowCheckboxs;

            // Top Level

            Dictionary<string, TreeNode> quantities = new Dictionary<string, TreeNode>();
            TreeNode node;

            foreach (UIExchangeItem item in _items)
            {
                if (item.ValueDefinition == null)
                    throw new InvalidOperationException("IValueDefinition == null");

                if (!quantities.ContainsKey(item.ValueDefinition.Caption))
                {
                    node = treeView.Nodes.Add(item.ValueDefinition.Caption);
                    node.Tag = null;
                    quantities.Add(item.ValueDefinition.Caption, node);
                }
            }

            // Second level

            Stack<TreeNode> kids = new Stack<TreeNode>(_items.Count);

            foreach (UIExchangeItem item in _items)
            {
                node = new TreeNode(item.ToString());

                node.ImageIndex
                    = node.SelectedImageIndex
                    = ImageIndex(item.ElementSet.ElementType);

                node.Tag = item;

                if (item is UIOutputItem && ((UIOutputItem)item).Parent != null)
                {
                    kids.Push(node);
                }
                else
                {
                    quantities[item.ValueDefinition.Caption].Nodes.Add(node);

                    _nodes.Add(item.ExchangeItem, node);
                }
            }

            // Lower levels

            if (kids.Count > 0)
            {
                Stack<TreeNode> unassigned;
                UIExchangeItem iParent, iNode;
                int added;

                do
                {
                    added = 0;
                    unassigned = new Stack<TreeNode>(kids.Count);

                    while (kids.Count > 0)
                    {
                        node = kids.Pop();
                        iNode = (UIExchangeItem)node.Tag;
                        iParent = ((UIOutputItem)iNode).Parent;

                        if (_nodes.ContainsKey(iParent.ExchangeItem))
                        {
                            _nodes[iParent.ExchangeItem].Nodes.Add(node);
                            _nodes.Add(iNode.ExchangeItem, node);
                            ++added;
                        }
                        else
                        {
                            unassigned.Push(node);
                        }
                    }

                    if (added == 0 && unassigned.Count > 0)
                    {
                        Debug.Assert(false); // cant assign kids
                        break;
                    }

                    kids = unassigned;
                }
                while (unassigned.Count > 0);
            }

            treeView.ExpandAll();
            treeView.EndUpdate();

            if (toCheck != null && _nodes.ContainsKey(toCheck.ExchangeItem))
                CheckNode(_nodes[toCheck.ExchangeItem], true);

            treeView.Invalidate();
        }

        int ImageIndex(ElementType type)
        {
            switch (type)
            {
                case ElementType.IdBased:
                    return 2;
                case ElementType.Point:
                    return 3;
                case ElementType.PolyLine:
                    return 5;
                case ElementType.Polygon:
                    return 6;
                /*
            case ElementType.Polyhedron:
                return 11;
                 */
                default:
                    break;
            }

            Debug.Assert(false);
            return 11;
        }

        public void CheckLink(Link link)
        {
            if (_treeOptions.IsSource)
            {
                CheckNode(_nodes[link.Source.ExchangeItem], true);
            }
            else if (_treeOptions.IsTarget)
            {
                CheckNode(_nodes[link.Target.ExchangeItem], true);
            }
        }

        public UIExchangeItem GetSelectedObject()
        {
            return treeView.SelectedNode != null ? (UIExchangeItem)treeView.SelectedNode.Tag : null;
        }

        private class ExchangeItemsComparer : IComparer<UIExchangeItem>
        {
            #region IComparer<UIExchangeItem> Members

            public int Compare(UIExchangeItem x, UIExchangeItem y)
            {
                int result = string.Compare(x.ValueDefinition.Caption, y.ValueDefinition.Caption, false);

                if (result == 0)
                    result = string.Compare(x.ElementSet.Caption, y.ElementSet.Caption, false);

                return (result);
            }

            #endregion
        }

        #endregion

        void SetNodeAndChildrenCheck(TreeNode node, bool state)
        {
            node.Checked = state;

            if (node.Nodes != null)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    SetNodeAndChildrenCheck(n, state); // RECURSION
                }
            }
        }

        void SetNodeAndParentsCheck(TreeNode node, bool state)
        {
            node.Checked = state;

            if (node.Parent != null)
            {
                SetNodeAndParentsCheck(node.Parent, state); // RECURSION
            }
        }

        void SetAllNodesCheck(TreeNode node, bool state)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
            }

            SetNodeAndChildrenCheck(node, state);
        }

        /// <summary>
        /// Get any checked exchange items
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public List<UIExchangeItem> GetCheckedExchangeItems()
        {
            List<UIExchangeItem> items = new List<UIExchangeItem>();

            foreach (TreeNode node in treeView.Nodes)
            {
                AddCheckedExchangeChildren(node, items);
            }

            return items;
        }

        void AddCheckedExchangeChildren(TreeNode node, List<UIExchangeItem> items)
        {
            UIExchangeItem item = (UIExchangeItem)node.Tag;

            if (node.Checked && item != null)
            {
                items.Add(item);
            }

            foreach (TreeNode n in node.Nodes)
            {
                AddCheckedExchangeChildren(n, items); // RECURSION
            }
        }

        #region Event handlers

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNode(e.Node, e.Node.Checked);
        }

        void CheckNode(TreeNode node, bool check)
        {
            if (_ignoreCheckEvents)
                return;

            try
            {
                _ignoreCheckEvents = true;

                // For check tree we are only interested in exchange items (whether decorated or not)
                // so if user checks a node, all parents should be checked as root parent is exchange item
                // If user unchecked, then all kids should be unchecked too

                if (!check)
                {
                    SetNodeAndChildrenCheck(node, false);
                }
                else
                {
                    SetAllNodesCheck(node, false);
                    SetNodeAndParentsCheck(node, check);
                }

                ExchangeItemChanged(this, new EventArgs());

                treeView.EndUpdate();
            }
            finally
            {
                _ignoreCheckEvents = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectionChanged(this, new EventArgs());
        }


        private void treeView1_Enter(object sender, EventArgs e)
        {
            // control was activated, so behave like some node was selected
            treeView1_AfterSelect(null, null);
        }

        #endregion

        #region .NET generated members

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(ExchangeItemSelector));
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.CheckBoxes = true;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView1";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(256, 328);
            this.treeView.TabIndex = 0;
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            this.treeView.Enter += new System.EventHandler(this.treeView1_Enter);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList.ImageStream =
                ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            this.imageList.Images.SetKeyName(4, "");
            this.imageList.Images.SetKeyName(5, "");
            this.imageList.Images.SetKeyName(6, "");
            this.imageList.Images.SetKeyName(7, "");
            this.imageList.Images.SetKeyName(8, "");
            this.imageList.Images.SetKeyName(9, "");
            this.imageList.Images.SetKeyName(10, "");
            this.imageList.Images.SetKeyName(11, "");
            this.imageList.Images.SetKeyName(12, "");
            this.imageList.Images.SetKeyName(13, "RESOURCE.BMP");
            // 
            // ExchangeItemSelector
            // 
            this.Controls.Add(this.treeView);
            this.Name = "ExchangeItemSelector";
            this.Size = new System.Drawing.Size(256, 328);
            this.ResumeLayout(false);
        }

        #endregion

        #endregion
    }
}