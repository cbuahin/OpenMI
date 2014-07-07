using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.Backbone;
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
        ElementSetViewer elementSetViewer;
        UIConnection connection;
        static List<UIAdaptedFactory> externalFactories;
        Link currentLink;
        Dictionary<UIOutputItem, TreeNode> fastLookUpOutputs;
        Dictionary<UIInputItem, TreeNode> fastLookUpInputs;

        static ConnectionDlg()
        {
            externalFactories = new List<UIAdaptedFactory>();
        }

        public ConnectionDlg(UIConnection connection)
        {
            InitializeComponent();

            currentLink = new Link();

            this.connection = connection;

            elementSetViewer = new ElementSetViewer();

            updateTargetsTreeView();
            updateSourcesTreeView();

            listBoxConnections.DataSource = connection.Links;

        }

        void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void updateSourcesTreeView()
        {

            treeViewSources.SuspendLayout();
            treeViewSources.Nodes.Clear();

            fastLookUpOutputs = new Dictionary<UIOutputItem, TreeNode>();

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

                    UIOutputItem uitem = (from n in connection.Links
                                          where n.FindInChain(tspaceOutput) != null
                                          select n.FindInChain(tspaceOutput)).FirstOrDefault();



                    if (uitem != null)
                    { 
                        item.Tag = uitem;
                        fastLookUpOutputs.Add(uitem, item);
                        expandOutputItem(item, uitem);
                    }
                    else
                    {
                        uitem = new UIOutputItem(tspaceOutput);
                        item.Tag = uitem;
                        fastLookUpOutputs.Add(uitem, item);
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
            treeViewTargets.Nodes.Clear();

            fastLookUpInputs = new Dictionary<UIInputItem, TreeNode>();

            UIModel targetModel = connection.TargetModel;
            Dictionary<string, TreeNode> quantities = new Dictionary<string, TreeNode>();

            foreach (IBaseInput input in targetModel.LinkableComponent.Inputs)
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

                    UIInputItem uitem = (from n in connection.Links
                                         where n.Target != null && n.Target.ExchangeItem == tspaceInput
                                         select n.Target).FirstOrDefault();
                    if (uitem != null)
                    {
                        
                    }
                    else
                    {
                        uitem = new UIInputItem(tspaceInput);
                       
                    }

                    item.Tag = uitem;
                    fastLookUpInputs.Add(uitem, item);

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

        void expandOutputItem(TreeNode node, UIOutputItem item)
        {
            List<UIOutputItem> children = currentLink.FindNextChildrenInChain(item);

            for (int i = 0; i < children.Count; i++)
            {
                UIOutputItem child = children[0];
                TreeNode n1 = null;

                if(!TreeNodeContains(child, node,out n1))
                {
                    n1 = new TreeNode(child.Caption);
                    n1.Tag = child;
                    n1.ImageKey = "RESOURCE.BMP";
                    n1.SelectedImageKey = "RESOURCE.BMP";
                    node.Nodes.Add(n1);
                    if (!fastLookUpOutputs.ContainsKey(child))
                        fastLookUpOutputs.Add(child, n1);
                }

                if(n1 != null)
                {
                    expandOutputItem(n1, child);
                }
            }

            foreach(Link link in connection.Links)
            {
                children = link.FindNextChildrenInChain(item);

                for (int i = 0; i < children.Count; i++)
                {
                    UIOutputItem child = children[0];
                    TreeNode n1 = null;

                    if (!TreeNodeContains(child, node, out n1))
                    {
                        n1 = new TreeNode(child.Caption);
                        n1.Tag = child;
                        n1.ImageKey = "RESOURCE.BMP";
                        n1.SelectedImageKey = "RESOURCE.BMP";
                        node.Nodes.Add(n1);

                        if(!fastLookUpOutputs.ContainsKey(child))
                        fastLookUpOutputs.Add(child, n1);
                    }

                    if (n1 != null)
                    {
                        expandOutputItem(n1, child);
                    }
                }
            }
        }

        bool TreeNodeContains( IBaseExchangeItem item , TreeNode node , out TreeNode outnode) 
        {
            outnode = null;

            foreach(TreeNode n  in node.Nodes)
            {
                if (n.Tag != null && n.Tag is IBaseExchangeItem && ((item == (IBaseExchangeItem)n.Tag) || item.Id == ((IBaseExchangeItem)node.Tag).Id))
                {
                    outnode = n;
                    return true;
                }
            }

            return false;
        }
        

        void uncheckAllNodes(bool Target = true , bool withBlocking = true)
        {
            if (!Target)
            {
                treeViewSources.SuspendLayout();

                if (withBlocking)
                {
                    treeViewSources.AfterCheck-=treeViewSources_AfterCheck;
                }

                foreach(TreeNode node in treeViewSources.Nodes)
                {
                    if(node.Checked)
                    {
                        node.Checked = false;
                    }

                    CheckDescendants(node, false);

                }

                if (withBlocking)
                {
                    treeViewSources.AfterCheck += treeViewSources_AfterCheck;
                }

                treeViewSources.ResumeLayout();
            }
            else
            {
                treeViewTargets.SuspendLayout();

                if (withBlocking)
                {
                    treeViewTargets.AfterCheck-=treeViewTargets_AfterCheck;
                }

                foreach (TreeNode node in treeViewTargets.Nodes)
                {
                    if (node.Checked)
                    {
                        node.Checked = false;
                    }

                    CheckDescendants(node, false);

                }
                
                if (withBlocking)
                {
                    treeViewTargets.AfterCheck += treeViewTargets_AfterCheck;
                }

                treeViewTargets.ResumeLayout();
            }
        }

        void treeViewSources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                propertyGridConnection.SelectedObject = e.Node.Tag;

                if (e.Node.Checked)
                    SetValidAdatedOutputs(e.Node);
                else
                    listBoxAdaptedOutputs.DataSource = null;
            }
            else
                propertyGridConnection.SelectedObject = null;
        }

        void treeViewTargets_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                propertyGridConnection.SelectedObject = e.Node.Tag;
            }
            else
                propertyGridConnection.SelectedObject = null;
        }

        void buttonPlotElementSet_Click(object sender, EventArgs e)
        {
            ArrayList elementSets = new ArrayList();
            List<UIOutputItem> outputs = GetCheckedItems<UIOutputItem>(treeViewSources.Nodes);
            List<UIInputItem> inputs = GetCheckedItems<UIInputItem>(treeViewTargets.Nodes);

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
                elementSetViewer.PopulateDialog(elementSets);
                elementSetViewer.ShowDialog();
            }
        }

        void treeViewSources_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeViewSources.SuspendLayout();
                      
            treeViewSources.AfterCheck -= treeViewSources_AfterCheck;

            if (e.Node.Tag != null)
            {
                if (e.Node.Checked)
                {
                    CheckDescendants(e.Node, true);
                    SetValidAdatedOutputs(e.Node);
                }
                else
                {
                    if(e.Node.Level >= 2)
                    {
                        CheckDescendants(e.Node, false);
                    }

                   
                }
            }
           
            treeViewSources.AfterCheck += treeViewSources_AfterCheck;
            treeViewSources.ResumeLayout();
            UpdateCurrentLink();
        }

        void treeViewTargets_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treeViewTargets.SuspendLayout();
            bool checkedd = e.Node.Checked;
 
            uncheckAllNodes(true, true);

            treeViewTargets.AfterCheck -= treeViewTargets_AfterCheck;

            e.Node.Checked = checkedd;

            if (e.Node.Tag != null && e.Node.Checked)
            {
                CheckDescendants(e.Node, true);
            }
       

            treeViewTargets.AfterCheck += treeViewTargets_AfterCheck;
            treeViewTargets.ResumeLayout();

          
            UpdateCurrentLink();
        }

        List<T> GetDownStreamCheckedItems<T>(TreeNodeCollection nodes)
        {
            List<T> outputs = new List<T>();

            List<TreeNode> outNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                outNodes.AddRange(GetDownStreamCheckedNodes(node));
            }

            foreach (TreeNode node in outNodes)
            {
                if (node.Tag is T)
                {
                    outputs.Add((T)node.Tag);
                }
            }

            return outputs;
        }

        List<TreeNode> GetDownStreamCheckedNodes(TreeNode node)
        {
            List<TreeNode> outputs = new List<TreeNode>();

            if (node.Checked)
            {
                List<TreeNode> checkedNodes = GetCheckedChildNodes(node);

                if (checkedNodes.Count == 0)
                {
                    outputs.Add(node);
                }
                else
                {
                    foreach (TreeNode node1 in checkedNodes)
                    {
                        outputs.AddRange(GetDownStreamCheckedNodes(node1));
                    }
                }
            }


            return outputs;
        }

        List<TreeNode> GetCheckedChildNodes(TreeNode node)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Checked)
                {
                    nodes.Add(n);
                }
            }

            return nodes;
        }

        List<T> GetCheckedItems<T>(TreeNodeCollection nodes)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();
            List<T> outputs = new List<T>();

            foreach (TreeNode node in nodes)
            {
                checkedNodes.AddRange(GetCheckedNodes(node));
            }

            foreach (TreeNode node in checkedNodes)
            {
                if (node.Tag is T)
                {
                    outputs.Add((T)node.Tag);
                }
            }

            return outputs;
        }

        List<TreeNode> GetCheckedNodes(TreeNode node)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Checked)
                {
                    nodes.Add(n);
                    nodes.AddRange(GetCheckedNodes(n));
                }
            }

            return nodes;
        }

        void CheckDescendants(TreeNode node, bool upward)
        {
            if (upward)
            {
                if (node.Parent != null)
                {
                    node.Parent.Checked = node.Checked;
                    CheckDescendants(node.Parent, upward);
                }
            }
            else
            {
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = node.Checked;
                    CheckDescendants(child, upward);
                }
            }
        }

        void SetValidAdatedOutputs(TreeNode sender = null)
        {
            UIInputItem input = null;
            UIOutputItem output  = null;

            if(sender != null && sender.Tag is UIOutputItem)
            {
                output = sender.Tag as UIOutputItem;
            }
            else
            {
                SetErrorText("No output item has been selected");
            }

            List<UIInputItem> checkedInputs = GetDownStreamCheckedItems<UIInputItem>(treeViewTargets.Nodes);

            if(checkedInputs.Count != 1)
            {
                SetErrorText("More than one input item has been selected");
            }
            else
            {
                input = checkedInputs[0];
            }

            if(input == null)
            {
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            if(output == null)
            {
                listBoxAdaptedOutputs.DataSource = null;
                return;
            }

            List<UIAdaptedOutputItem> ids = new List<UIAdaptedOutputItem>();


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
                    UIAdaptedFactory tempFac = new UIAdaptedFactory();
                    tempFac.InitialiseAsNative(factory.Id , connection.SourceModel.LinkableComponent);

                   ITimeSpaceAdaptedOutput adpout =   (ITimeSpaceAdaptedOutput)factory.CreateAdaptedOutput(f, output, input);
                   UIAdaptedOutputItem uiadpout = new UIAdaptedOutputItem(tempFac, f,  adpout , output);
                   ids.Add(uiadpout);
                }
            }

            foreach(TreeNode node in sender.Nodes)
            {
                if(node.Tag != null && node.Tag is UIAdaptedOutputItem)
                {
                    UIAdaptedOutputItem item = (UIAdaptedOutputItem)node.Tag;

                    for(int i = 0 ; i < ids.Count ; i++)
                    {
                        if(item.Id == ids[i].Id)
                        {
                            ids.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            listBoxAdaptedOutputs.DataSource = ids;
            listBoxAdaptedOutputs.DisplayMember = "Caption";
        }

        void UpdateCurrentLink()
        {
            List<UIOutputItem> outputs = GetDownStreamCheckedItems<UIOutputItem>(treeViewSources.Nodes);
            List<UIInputItem> inputs = GetDownStreamCheckedItems<UIInputItem>(treeViewTargets.Nodes);

            if(inputs.Count == 1)
            {
                if(outputs.Count != 1 && !(inputs[0] is IBaseMultiInput))
                {
                    SetErrorText("Selected input item requires one output item");
                }
                else
                {
                    currentLink.Target = inputs[0];
                    currentLink.Sources = outputs;
                }
            }
            else
            {
                SetErrorText("Unable to update link because current input item does not support the number of output items selected");
            }
        }


        private void buttonAddConnection_Click(object sender, EventArgs e)
        {
            UpdateCurrentLink();

            if(ValidateLink(currentLink))
            {
                connection.Links.Add(currentLink);
                currentLink = new Link(currentLink.Sources, currentLink.Target);
                listBoxConnections.DataSource = null;
                listBoxConnections.DataSource = connection.Links;
            }
        }

        bool ValidateLink(Link link)
        {
            if(link.Target != null && link.Sources.Count> 0
                )
            {
                for (int i = 0; i < connection.Links.Count; i++)
                {
                    if (currentLink.Equals(connection.Links[i]))
                    {
                        MessageBox.Show("Link already exists");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("Please select valid target and sources");
                return false;
            }

        }

        private void buttonAddAdaptedOutput_Click(object sender, EventArgs e)
        {
            UIOutputItem output = null;

            if (treeViewSources.SelectedNode != null && treeViewSources.SelectedNode.Checked && treeViewSources.SelectedNode.Tag is UIOutputItem)
            {
                output = (UIOutputItem)treeViewSources.SelectedNode.Tag;

                if(listBoxAdaptedOutputs.SelectedItem is UIAdaptedOutputItem)
                {
                    UIAdaptedOutputItem adaptedOutput = (UIAdaptedOutputItem)listBoxAdaptedOutputs.SelectedItem;

                    if(adaptedOutput.Adaptee == output)
                    {
                        if (fastLookUpOutputs.ContainsKey(output))
                        {
                            TreeNode parent = fastLookUpOutputs[output];
                            TreeNode child = new TreeNode(adaptedOutput.Caption);
                            child.Tag = adaptedOutput;
                            child.ImageKey = "RESOURCE.BMP";
                            child.SelectedImageKey = "RESOURCE.BMP";
                            child.Checked = true;
                            parent.Nodes.Add(child);

                            fastLookUpOutputs.Add(adaptedOutput, child);

                            parent.ExpandAll();
                            UpdateCurrentLink();

                        }
                    }
                    else
                    {
                        SetErrorText("Selected adaptor is not valid for selected output");
                    }
                }
            }
        }

        private void listBoxAdaptedOutputs_SelectedValueChanged(object sender, EventArgs e)
        {
            propertyGridConnection.SelectedObject = listBoxAdaptedOutputs.SelectedItem;
        }

        private void SetErrorText(string error)
        {
            labelError.Text = error;
            timerError.Start();
        }
        
        private void timerError_Tick(object sender, EventArgs e)
        {
            labelError.Text = "";
            timerError.Stop();
        }

        private void listBoxConnections_SelectedValueChanged(object sender, EventArgs e)
        {
            if(listBoxConnections.SelectedItem != null)
            {
                Link selected = listBoxConnections.SelectedItem as Link;

                if (selected != null)
                {

                    uncheckAllNodes(true, true);
                    uncheckAllNodes(false, true);

                    UIInputItem item = selected.Target;

                    if (fastLookUpInputs.ContainsKey(item))
                    {
                        TreeNode node = fastLookUpInputs[item];
                        node.Checked = true;
                    }

                    foreach (UIOutputItem output in selected.Sources)
                    {
                        if (fastLookUpOutputs.ContainsKey(output))
                        {
                            TreeNode node = fastLookUpOutputs[output];
                            node.Checked = true;
                        }
                    }
                    
                    currentLink = new Link(selected.Sources, selected.Target);

                }
            }
        }

        private void buttonRemoveLink_Click(object sender, EventArgs e)
        {
            if (listBoxConnections.SelectedItem != null)
            {
                Link selected = listBoxConnections.SelectedItem as Link;

                if (selected != null)
                {
                    connection.Links.Remove(selected);
                    uncheckAllNodes(true, true);
                    uncheckAllNodes(false, true);

                    currentLink = new Link();

                    updateSourcesTreeView();

                    listBoxConnections.DataSource = null;
                    listBoxConnections.DataSource = connection.Links;
                }
            }
        }
    }

}
