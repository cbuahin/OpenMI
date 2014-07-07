namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    partial class ConnectionDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionDlg));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBoxTargets = new System.Windows.Forms.GroupBox();
            this.treeViewSources = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.groupBoxAvailable = new System.Windows.Forms.GroupBox();
            this.listBoxAdaptedOutputs = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonAddAdaptedOutput = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRemoveLink = new System.Windows.Forms.Button();
            this.listBoxConnections = new System.Windows.Forms.ListBox();
            this.buttonAddConnection = new System.Windows.Forms.Button();
            this.groupBoxSources = new System.Windows.Forms.GroupBox();
            this.treeViewTargets = new System.Windows.Forms.TreeView();
            this.groupBoxSelectedObjectsProperties = new System.Windows.Forms.GroupBox();
            this.propertyGridConnection = new System.Windows.Forms.PropertyGrid();
            this.buttonPlotElementSet = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.timerError = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxTargets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBoxAvailable.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxSources.SuspendLayout();
            this.groupBoxSelectedObjectsProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxSelectedObjectsProperties);
            this.splitContainer1.Size = new System.Drawing.Size(1097, 549);
            this.splitContainer1.SplitterDistance = 875;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxSources);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(875, 549);
            this.splitContainer2.SplitterDistance = 234;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBoxTargets
            // 
            this.groupBoxTargets.Controls.Add(this.treeViewTargets);
            this.groupBoxTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTargets.Location = new System.Drawing.Point(0, 0);
            this.groupBoxTargets.Name = "groupBoxTargets";
            this.groupBoxTargets.Size = new System.Drawing.Size(240, 549);
            this.groupBoxTargets.TabIndex = 2;
            this.groupBoxTargets.TabStop = false;
            this.groupBoxTargets.Text = "Targets";
            // 
            // treeViewSources
            // 
            this.treeViewSources.CheckBoxes = true;
            this.treeViewSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSources.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.treeViewSources.ImageIndex = 0;
            this.treeViewSources.ImageList = this.imageList;
            this.treeViewSources.Location = new System.Drawing.Point(3, 16);
            this.treeViewSources.Name = "treeViewSources";
            this.treeViewSources.SelectedImageIndex = 0;
            this.treeViewSources.Size = new System.Drawing.Size(228, 530);
            this.treeViewSources.TabIndex = 0;
            this.treeViewSources.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSources_AfterCheck);
            this.treeViewSources.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewSources_NodeMouseClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "valueDef");
            this.imageList.Images.SetKeyName(1, "velocityVector");
            this.imageList.Images.SetKeyName(2, "id");
            this.imageList.Images.SetKeyName(3, "point2d");
            this.imageList.Images.SetKeyName(4, "line2d");
            this.imageList.Images.SetKeyName(5, "mline2d");
            this.imageList.Images.SetKeyName(6, "polygon2d");
            this.imageList.Images.SetKeyName(7, "point3d");
            this.imageList.Images.SetKeyName(8, "line3d");
            this.imageList.Images.SetKeyName(9, "mline3d");
            this.imageList.Images.SetKeyName(10, "polygon3d");
            this.imageList.Images.SetKeyName(11, "polyhedra");
            this.imageList.Images.SetKeyName(12, "adaptedOutput1");
            this.imageList.Images.SetKeyName(13, "RESOURCE.BMP");
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBoxTargets);
            this.splitContainer3.Size = new System.Drawing.Size(637, 549);
            this.splitContainer3.SplitterDistance = 393;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.groupBoxAvailable);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer4.Size = new System.Drawing.Size(393, 549);
            this.splitContainer4.SplitterDistance = 355;
            this.splitContainer4.TabIndex = 1;
            // 
            // groupBoxAvailable
            // 
            this.groupBoxAvailable.Controls.Add(this.listBoxAdaptedOutputs);
            this.groupBoxAvailable.Controls.Add(this.button1);
            this.groupBoxAvailable.Controls.Add(this.buttonAddAdaptedOutput);
            this.groupBoxAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAvailable.Location = new System.Drawing.Point(0, 0);
            this.groupBoxAvailable.Name = "groupBoxAvailable";
            this.groupBoxAvailable.Size = new System.Drawing.Size(393, 355);
            this.groupBoxAvailable.TabIndex = 0;
            this.groupBoxAvailable.TabStop = false;
            this.groupBoxAvailable.Text = "Available Adaptors For Current Selection";
            // 
            // listBoxAdaptedOutputs
            // 
            this.listBoxAdaptedOutputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAdaptedOutputs.FormattingEnabled = true;
            this.listBoxAdaptedOutputs.HorizontalScrollbar = true;
            this.listBoxAdaptedOutputs.Location = new System.Drawing.Point(6, 48);
            this.listBoxAdaptedOutputs.Name = "listBoxAdaptedOutputs";
            this.listBoxAdaptedOutputs.Size = new System.Drawing.Size(381, 290);
            this.listBoxAdaptedOutputs.TabIndex = 4;
            this.listBoxAdaptedOutputs.SelectedValueChanged += new System.EventHandler(this.listBoxAdaptedOutputs_SelectedValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Add Sources";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // buttonAddAdaptedOutput
            // 
            this.buttonAddAdaptedOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddAdaptedOutput.Location = new System.Drawing.Point(272, 19);
            this.buttonAddAdaptedOutput.Name = "buttonAddAdaptedOutput";
            this.buttonAddAdaptedOutput.Size = new System.Drawing.Size(115, 23);
            this.buttonAddAdaptedOutput.TabIndex = 2;
            this.buttonAddAdaptedOutput.Text = "Add Adapted Output";
            this.buttonAddAdaptedOutput.UseVisualStyleBackColor = true;
            this.buttonAddAdaptedOutput.Click += new System.EventHandler(this.buttonAddAdaptedOutput_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRemoveLink);
            this.groupBox2.Controls.Add(this.listBoxConnections);
            this.groupBox2.Controls.Add(this.buttonAddConnection);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 190);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connections";
            // 
            // buttonRemoveLink
            // 
            this.buttonRemoveLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveLink.Location = new System.Drawing.Point(272, 18);
            this.buttonRemoveLink.Name = "buttonRemoveLink";
            this.buttonRemoveLink.Size = new System.Drawing.Size(115, 23);
            this.buttonRemoveLink.TabIndex = 6;
            this.buttonRemoveLink.Text = "Remove Connection";
            this.buttonRemoveLink.UseVisualStyleBackColor = true;
            this.buttonRemoveLink.Click += new System.EventHandler(this.buttonRemoveLink_Click);
            // 
            // listBoxConnections
            // 
            this.listBoxConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxConnections.FormattingEnabled = true;
            this.listBoxConnections.HorizontalScrollbar = true;
            this.listBoxConnections.Location = new System.Drawing.Point(6, 47);
            this.listBoxConnections.Name = "listBoxConnections";
            this.listBoxConnections.Size = new System.Drawing.Size(381, 134);
            this.listBoxConnections.TabIndex = 5;
            this.listBoxConnections.SelectedValueChanged += new System.EventHandler(this.listBoxConnections_SelectedValueChanged);
            // 
            // buttonAddConnection
            // 
            this.buttonAddConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddConnection.Location = new System.Drawing.Point(151, 18);
            this.buttonAddConnection.Name = "buttonAddConnection";
            this.buttonAddConnection.Size = new System.Drawing.Size(115, 23);
            this.buttonAddConnection.TabIndex = 1;
            this.buttonAddConnection.Text = "Add Connection";
            this.buttonAddConnection.UseVisualStyleBackColor = true;
            this.buttonAddConnection.Click += new System.EventHandler(this.buttonAddConnection_Click);
            // 
            // groupBoxSources
            // 
            this.groupBoxSources.Controls.Add(this.treeViewSources);
            this.groupBoxSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSources.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSources.Name = "groupBoxSources";
            this.groupBoxSources.Size = new System.Drawing.Size(234, 549);
            this.groupBoxSources.TabIndex = 1;
            this.groupBoxSources.TabStop = false;
            this.groupBoxSources.Text = "Sources";
            // 
            // treeViewTargets
            // 
            this.treeViewTargets.CheckBoxes = true;
            this.treeViewTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTargets.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.treeViewTargets.ImageIndex = 0;
            this.treeViewTargets.ImageList = this.imageList;
            this.treeViewTargets.Location = new System.Drawing.Point(3, 16);
            this.treeViewTargets.Name = "treeViewTargets";
            this.treeViewTargets.SelectedImageIndex = 0;
            this.treeViewTargets.Size = new System.Drawing.Size(234, 530);
            this.treeViewTargets.TabIndex = 0;
            this.treeViewTargets.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTargets_AfterCheck);
            this.treeViewTargets.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTargets_NodeMouseClick);
            // 
            // groupBoxSelectedObjectsProperties
            // 
            this.groupBoxSelectedObjectsProperties.Controls.Add(this.propertyGridConnection);
            this.groupBoxSelectedObjectsProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSelectedObjectsProperties.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSelectedObjectsProperties.Name = "groupBoxSelectedObjectsProperties";
            this.groupBoxSelectedObjectsProperties.Size = new System.Drawing.Size(218, 549);
            this.groupBoxSelectedObjectsProperties.TabIndex = 1;
            this.groupBoxSelectedObjectsProperties.TabStop = false;
            this.groupBoxSelectedObjectsProperties.Text = "Selected Object\'s Properties";
            // 
            // propertyGridConnection
            // 
            this.propertyGridConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridConnection.Location = new System.Drawing.Point(3, 16);
            this.propertyGridConnection.Name = "propertyGridConnection";
            this.propertyGridConnection.Size = new System.Drawing.Size(212, 530);
            this.propertyGridConnection.TabIndex = 0;
            // 
            // buttonPlotElementSet
            // 
            this.buttonPlotElementSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlotElementSet.Location = new System.Drawing.Point(12, 567);
            this.buttonPlotElementSet.Name = "buttonPlotElementSet";
            this.buttonPlotElementSet.Size = new System.Drawing.Size(134, 23);
            this.buttonPlotElementSet.TabIndex = 0;
            this.buttonPlotElementSet.Text = "Plot Selected Elements";
            this.buttonPlotElementSet.UseVisualStyleBackColor = true;
            this.buttonPlotElementSet.Click += new System.EventHandler(this.buttonPlotElementSet_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(1034, 567);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelError
            // 
            this.labelError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelError.AutoSize = true;
            this.labelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(152, 572);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(43, 13);
            this.labelError.TabIndex = 2;
            this.labelError.Text = "Ready";
            // 
            // timerError
            // 
            this.timerError.Interval = 1000;
            this.timerError.Tick += new System.EventHandler(this.timerError_Tick);
            // 
            // ConnectionDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1121, 602);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.buttonPlotElementSet);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConnectionDlg";
            this.Text = "ConnectionDlg";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxTargets.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBoxAvailable.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxSources.ResumeLayout(false);
            this.groupBoxSelectedObjectsProperties.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView treeViewSources;
        private System.Windows.Forms.GroupBox groupBoxSources;
        private System.Windows.Forms.GroupBox groupBoxTargets;
        private System.Windows.Forms.TreeView treeViewTargets;
        private System.Windows.Forms.PropertyGrid propertyGridConnection;
        private System.Windows.Forms.GroupBox groupBoxSelectedObjectsProperties;
        private System.Windows.Forms.Button buttonPlotElementSet;
        private System.Windows.Forms.GroupBox groupBoxAvailable;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonAddConnection;
        private System.Windows.Forms.Button buttonAddAdaptedOutput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxAdaptedOutputs;
        private System.Windows.Forms.Button buttonRemoveLink;
        private System.Windows.Forms.ListBox listBoxConnections;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Timer timerError;
    }
}