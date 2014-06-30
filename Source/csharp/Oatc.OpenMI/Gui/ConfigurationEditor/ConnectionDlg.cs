using Oatc.OpenMI.Gui.Core;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public partial class ConnectionDlg : Form
    {
        private ElementSetViewer _elementSetViewer;
        UIConnection connection;
        Dictionary<UIInputItem, List<Link>> links;
        static List<UIAdaptedFactory> externalFactories;
        Link currentLink;
        List<Link> workingLinks;
        static ConnectionDlg()
        {
            externalFactories = new List<UIAdaptedFactory>();

        }

        public ConnectionDlg(UIConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            workingLinks = new List<Link>();
            currentLink = new Link();
            workingLinks.Add(currentLink);
            workingLinks.AddRange(connection.Links);
            listBoxConnections.DataSource = links;
            _elementSetViewer = new ElementSetViewer();
            updateSourcesTreeView();
            updateTargetsTreeView();
        }

        void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void updateSourcesTreeView()
        {

            treeViewSources.SuspendLayout();
            treeViewSources.Nodes.Clear();

            UIModel sourceModel = connection.SourceModel;
            Dictionary<string, TreeNode> quantities = new Dictionary<string, TreeNode>();

            foreach (IBaseOutput output in sourceModel.LinkableComponent.Outputs)
            {
                IValueDefinition value = output.ValueDefinition;
                TreeNode node = null;

                if (!quantities.ContainsKey(value.Caption))
                {
                    node = new TreeNode(value.Caption);
                    node.ImageKey = "valueDef";
                    quantities.Add(value.Caption, node);
                    treeViewSources.Nodes.Add(node);
                }
                else
                {
                    node = quantities[value.Caption];
                }

                TreeNode item = new TreeNode(output.Id);
                item.ImageKey = "id";
                item.Tag = output;
                node.Nodes.Add(item);

                if (output is ITimeSpaceOutput)
                {

                    ITimeSpaceOutput tspaceOutput = (ITimeSpaceOutput)output;

                    UIOutputItem uitem = (from n in workingLinks
                                                 where n.Source != null && upperMostParent(n.Source).ExchangeItem == tspaceOutput
                                                 select upperMostParent(n.Source)).FirstOrDefault();



                    if (uitem != null)
                    {

                        item.Tag = uitem;

                        updateUIOutputItem(item, uitem);

                    }
                    else
                    {
                        item.Tag = new UIOutputItem(tspaceOutput);
                    }

                    if (tspaceOutput.SpatialDefinition is IElementSet)
                    {

                        IElementSet eSet = (IElementSet)tspaceOutput.SpatialDefinition;

                        switch (eSet.ElementType)
                        {
                            case ElementType.IdBased:
                                item.ImageKey = "id";
                                break;
                            case ElementType.Point:

                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "point2d";
                                    item.SelectedImageKey = "point2d";
                                }
                                else
                                {
                                    item.ImageKey = "point3d";
                                    item.SelectedImageKey = "point3d";
                                }

                                break;
                            case ElementType.Polygon:
                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "polygon2d";
                                    item.SelectedImageKey = "polygon2d";
                                }
                                else
                                {
                                    item.ImageKey = "polygon3d";
                                    item.SelectedImageKey = "polygon3d";
                                }
                                break;
                            case ElementType.Polyhedron:
                                item.ImageKey = "polyhedra";
                                item.SelectedImageKey = "polyhedra";
                                break;
                            case ElementType.PolyLine:
                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "mline2d";
                                    item.SelectedImageKey = "mline2d";
                                }
                                else
                                {
                                    item.ImageKey = "mline3d";
                                    item.SelectedImageKey = "mline3d";
                                }
                                break;
                        }
                    }
                }
            }

            treeViewSources.ExpandAll();
            treeViewSources.ResumeLayout();
        }

        void updateTargetsTreeView()
        {
            treeViewTargets.SuspendLayout();

            UIModel sourceModel = connection.SourceModel;
            Dictionary<string, TreeNode> quantities = new Dictionary<string, TreeNode>();

            foreach (IBaseInput input in sourceModel.LinkableComponent.Inputs)
            {
                IValueDefinition value = input.ValueDefinition;
                TreeNode node = null;

                if (!quantities.ContainsKey(value.Caption))
                {
                    node = new TreeNode(value.Caption);
                    node.ImageKey = "valueDef";
                    quantities.Add(value.Caption, node);
                    treeViewTargets.Nodes.Add(node);
                }
                else
                {
                    node = quantities[value.Caption];
                }

                TreeNode item = new TreeNode(input.Id);
                item.ImageKey = "id";
                item.Tag = input;
                node.Nodes.Add(item);

                if (input is ITimeSpaceInput)
                {

                    ITimeSpaceInput tspaceInput = (ITimeSpaceInput)input;

                    UIInputItem uitem = (from n in workingLinks
                                         where n.Target != null && n.Target.ExchangeItem == tspaceInput
                                         select n.Target).FirstOrDefault();
                    if (uitem != null)
                    {
                        item.Tag = uitem;
                    }
                    else
                    {
                        item.Tag = new UIInputItem(tspaceInput);
                    }


                    item.Tag = new UIInputItem(tspaceInput);

                    if (tspaceInput.SpatialDefinition is IElementSet)
                    {

                        IElementSet eSet = (IElementSet)tspaceInput.SpatialDefinition;

                        switch (eSet.ElementType)
                        {
                            case ElementType.IdBased:
                                item.ImageKey = "id";
                                break;
                            case ElementType.Point:

                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "point2d";
                                    item.SelectedImageKey = "point2d";
                                }
                                else
                                {
                                    item.ImageKey = "point3d";
                                    item.SelectedImageKey = "point3d";
                                }

                                break;
                            case ElementType.Polygon:
                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "polygon2d";
                                    item.SelectedImageKey = "polygon2d";
                                }
                                else
                                {
                                    item.ImageKey = "polygon3d";
                                    item.SelectedImageKey = "polygon3d";
                                }
                                break;
                            case ElementType.Polyhedron:
                                item.ImageKey = "polyhedra";
                                item.SelectedImageKey = "polyhedra";
                                break;
                            case ElementType.PolyLine:
                                if (!eSet.HasZ)
                                {
                                    item.ImageKey = "mline2d";
                                    item.SelectedImageKey = "mline2d";
                                }
                                else
                                {
                                    item.ImageKey = "mline3d";
                                    item.SelectedImageKey = "mline3d";
                                }
                                break;
                        }
                    }

                }
            }

            treeViewTargets.ExpandAll();
            treeViewTargets.ResumeLayout();
        }


        UIOutputItem upperMostParent(UIOutputItem item)
        {
            UIOutputItem parent = item;

            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }

            return parent;
        }
        void treeViewSources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
                propertyGridConnection.SelectedObject = e.Node.Tag;
            else
                propertyGridConnection.SelectedObject = null;
        }

        void treeViewTargets_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
                propertyGridConnection.SelectedObject = e.Node.Tag;
            else
                propertyGridConnection.SelectedObject = null;
        }

        void buttonPlotElementSet_Click(object sender, EventArgs e)
        {
            ArrayList elementSets = new ArrayList();
            List<UIOutputItem> outputs = getSelectedSources(treeViewSources.Nodes);
            List<UIInputItem> inputs = getSelectedTargets(treeViewTargets.Nodes);

            List<IElementSet> elements = (from n in outputs
                                          where n is ITimeSpaceOutput && ((ITimeSpaceOutput)n).SpatialDefinition is IElementSet
                                          select (IElementSet)(((ITimeSpaceOutput)n).SpatialDefinition)).ToList<IElementSet>();

            elementSets.AddRange(elements);

            elements = (from n in outputs
                        where n is ITimeSpaceInput && ((ITimeSpaceInput)n).SpatialDefinition is IElementSet
                        select (IElementSet)(((ITimeSpaceInput)n).SpatialDefinition)).ToList<IElementSet>();

            elementSets.AddRange(elements);

            if (elementSets.Count > 0)
            {
                _elementSetViewer.PopulateDialog(elementSets);
                _elementSetViewer.ShowDialog();
            }
        }

        List<UIOutputItem> getSelectedSources(TreeNodeCollection nodes)
        {
            List<UIOutputItem> outputs = new List<UIOutputItem>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node.Tag != null)
                {
                    outputs.Add((UIOutputItem)node.Tag);
                }

                outputs.AddRange(getSelectedSources(node.Nodes));
            }


            return outputs;
        }

        List<TreeNode> getSelectedNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> outputs = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node.Tag != null)
                {
                    outputs.Add(node);
                }

                outputs.AddRange(getSelectedNodes(node.Nodes));
            }


            return outputs;
        }

        void getSelectedNodesWithLevels(TreeNodeCollection nodes, ref Dictionary<int, List<TreeNode>> outputs)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Level > 0 && node.Checked)
                {
                    if (!outputs.ContainsKey(node.Level))
                    {
                        List<TreeNode> tempNodes = new List<TreeNode>();
                        tempNodes.Add(node);
                        outputs.Add(node.Level, tempNodes);
                    }
                    else
                    {
                        outputs[node.Level].Add(node);
                    }
                }

                if (node.Nodes.Count > 0)
                {
                    getSelectedNodesWithLevels(node.Nodes, ref outputs);
                }

            }
        }

        List<UIInputItem> getSelectedTargets(TreeNodeCollection nodes)
        {
            List<UIInputItem> outputs = new List<UIInputItem>();

            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node.Tag != null)
                {
                    outputs.Add((UIInputItem)node.Tag);
                }

                outputs.AddRange(getSelectedTargets(node.Nodes));
            }

            return outputs;
        }

        void treeViewSources_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeViewSources.SuspendLayout();
            treeViewSources.AfterCheck -= treeViewSources_AfterCheck;

            if (e.Node.Tag == null)
            {
                checkDescendants(e.Node, false);
            }
            else
            {
                checkDescendants(e.Node, true);
            }
            
           
            treeViewSources.AfterCheck += treeViewSources_AfterCheck;
            treeViewSources.ResumeLayout();

            getAdaptedOutputs();
        }

        void treeViewTargets_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeViewTargets.SuspendLayout();
            treeViewTargets.AfterCheck -= treeViewTargets_AfterCheck;

            if (e.Node.Tag == null)
            {
                checkDescendants(e.Node, false);
            }
            else
            {
                checkDescendants(e.Node, true);
            }

            treeViewTargets.AfterCheck += treeViewTargets_AfterCheck;
            treeViewTargets.ResumeLayout();

            getAdaptedOutputs();
        }


        void getAdaptedOutputs()
        {
            UIOutputItem output;
            UIInputItem input;

            getCurrentLinkOutputs(out output, out input);

            if(input == null)
            {
                //MessageBox.Show("Either no input item has been selected or more than on input item selected");
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            if(output == null)
            {
                //MessageBox.Show("Either no output item has been selected or more than on output item selected");
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            List<IBaseAdaptedOutput> ids = new List<IBaseAdaptedOutput>();


            foreach(UIAdaptedFactory factory in externalFactories)
            {
                IIdentifiable[] idf = factory.GetAvailableAdaptedOutputIds(output, input);

                foreach(IIdentifiable f in idf)
                {
                    ITimeSpaceAdaptedOutput adpout = (ITimeSpaceAdaptedOutput)factory.CreateAdaptedOutput(f, output, input);
                    UIAdaptedOutputItem uiadpout = new UIAdaptedOutputItem(factory, f, adpout,output);
                    ids.Add( uiadpout);
                }
            }

            foreach(IAdaptedOutputFactory factory in connection.SourceModel.LinkableComponent.AdaptedOutputFactories)
            {
          UIAdaptedFactory fac = new UIAdaptedFactory();
                fac.InitialiseAsNative(factory.Id,connection.SourceModel.LinkableComponent);

                IIdentifiable[] idf = factory.GetAvailableAdaptedOutputIds(output, input);

                foreach (IIdentifiable f in idf)
                {
                UIAdaptedOutputItem dt = new UIAdaptedOutputItem(fac,)
                    ids.Add(factory.CreateAdaptedOutput(f, output, input));
                }
            }

            listBoxAdaptedOutputs.DataSource = ids;
            listBoxAdaptedOutputs.DisplayMember = "Caption";
        }

        void checkDescendants(TreeNode node, bool upward)
        {
            if (upward)
            {
                if (node.Parent != null)
                {
                    node.Parent.Checked = node.Checked;
                    checkDescendants(node.Parent, upward);
                }
            }
            else
            {
                //foreach (TreeNode child in node.Nodes)
                //{
                //    child.Checked = node.Checked;
                //    checkDescendants(child, upward);
                //}
            }
        }

        void updateUIOutputItem(TreeNode node, UIOutputItem output)
        {

            List<UIOutputItem> uitems = (from n in workingLinks
                                         where upperMostParent(n.Source).ExchangeItem == output.ExchangeItem
                                         select n.Source).ToList();

            for (int i = 0; i < uitems.Count; i++)
            {
                List<UIOutputItem> steps = stepsToParent(uitems[0], output);

                if(steps.Count > 0 )
                {
                    UIOutputItem last = steps[steps.Count - 1];
                    TreeNode n1 = getChildNode(node, last);

                    if (n1 != null)
                    {

                    }
                    else
                    {
                        n1 = new TreeNode(last.Id);
                        n1.Tag = last;

                        if(last is ITimeSpaceAdaptedOutput)
                        {
                            n1.ImageKey = "RESOURCE.BMP";
                        }

                        node.Nodes.Add(n1);
                    }

                    updateUIOutputItem(n1, last);
                }
            }
        }

        void getCurrentLinkOutputs(out UIOutputItem output, out UIInputItem input)
        {
            output = null;
            input = null;

            List<UIInputItem> inputs = getSelectedTargets(treeViewTargets.Nodes);
           
            if(inputs.Count == 1)
            {
                input = inputs[0];

           }

            Dictionary<int, List<TreeNode>> nodes = new Dictionary<int, List<TreeNode>>();
            getSelectedNodesWithLevels(treeViewSources.Nodes, ref nodes);

            if (nodes.Count > 0)
            {
                List<int> levels = nodes.Keys.ToList<int>();
                levels.Sort();


                bool valid = true;
                int current = levels.Count - 1;

                for (int i = current; i >= 0; i--)
                {
                    List<TreeNode> nodesAtLevel = nodes[levels[current]];

                    if (nodesAtLevel.Count != 1 && valid)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid && nodes[levels[current]][0].Tag != null)
                {
                    output = (UIOutputItem)nodes[levels[current]][0].Tag;
                }
            }
        }

        private void buttonAddConnection_Click(object sender, EventArgs e)
        {

        }

        List<UIOutputItem> stepsToParent(UIOutputItem item, UIOutputItem parent)
        {
            UIOutputItem check = item;

            List<UIOutputItem> items = new List<UIOutputItem>();
            
            while (check != null && check != parent)
            {
                check = check.Parent;
                items.Add(check);
            }


            return items;
        }

        TreeNode getChildNode(TreeNode node , UIOutputItem outputItem)
        {
            TreeNode nod = null;

            foreach(TreeNode no in node.Nodes)
            {
                if(no.Tag == outputItem)
                {
                    return no;
                }

                return getChildNode(no, outputItem);
            }
           
            return nod;
        }

        private void buttonAddAdaptedOutput_Click(object sender, EventArgs e)
        {
            UIOutputItem output;
            UIInputItem input;

            getCurrentLinkOutputs(out output, out input);

            if (input == null)
            {
                //MessageBox.Show("Either no input item has been selected or more than on input item selected");
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            if (output == null)
            {
                //MessageBox.Show("Either no output item has been selected or more than on output item selected");
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            if(listBoxAdaptedOutputs.SelectedItem != null)
            {
                if(currentLink.Source != null && currentLink.Target != null )
                {
                    currentLink.Source = output;
                    currentLink.Target = input;
                }

                IBaseAdaptedOutput selected = (IBaseAdaptedOutput)listBoxAdaptedOutputs.SelectedItem;

                UIAdaptedOutputItem output = new UIAdaptedOutputItem()
            }

        }
    }

    public class LinkChain
    {

        public List<Link> Links
        {
            get;
            set;
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
