
namespace Jacdac.Explorer
{
    partial class JacdacExplorer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.deviceTree = new System.Windows.Forms.TreeView();
            this.registerListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.usbDeviceSelectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.packetTracerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.eventListView = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).BeginInit();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 24);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.deviceTree);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.splitContainerRight);
            this.mainSplit.Size = new System.Drawing.Size(800, 426);
            this.mainSplit.SplitterDistance = 266;
            this.mainSplit.TabIndex = 0;
            // 
            // deviceTree
            // 
            this.deviceTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceTree.Location = new System.Drawing.Point(0, 0);
            this.deviceTree.Name = "deviceTree";
            this.deviceTree.Size = new System.Drawing.Size(266, 426);
            this.deviceTree.TabIndex = 0;
            // 
            // registerListView
            // 
            this.registerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.registerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerListView.FullRowSelect = true;
            this.registerListView.HideSelection = false;
            this.registerListView.Location = new System.Drawing.Point(0, 0);
            this.registerListView.MultiSelect = false;
            this.registerListView.Name = "registerListView";
            this.registerListView.Size = new System.Drawing.Size(530, 229);
            this.registerListView.TabIndex = 0;
            this.registerListView.UseCompatibleStateImageBehavior = false;
            this.registerListView.View = System.Windows.Forms.View.Details;
            this.registerListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Value";
            this.columnHeader3.Width = 160;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usbDeviceSelectMenu,
            this.packetTracerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // usbDeviceSelectMenu
            // 
            this.usbDeviceSelectMenu.Name = "usbDeviceSelectMenu";
            this.usbDeviceSelectMenu.Size = new System.Drawing.Size(78, 20);
            this.usbDeviceSelectMenu.Text = "USB Device";
            // 
            // packetTracerToolStripMenuItem
            // 
            this.packetTracerToolStripMenuItem.Name = "packetTracerToolStripMenuItem";
            this.packetTracerToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.packetTracerToolStripMenuItem.Text = "Packet Tracer";
            this.packetTracerToolStripMenuItem.Click += new System.EventHandler(this.packetTracerToolStripMenuItem_Click);
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            this.splitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.registerListView);
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.groupBox1);
            this.splitContainerRight.Size = new System.Drawing.Size(530, 426);
            this.splitContainerRight.SplitterDistance = 229;
            this.splitContainerRight.TabIndex = 1;
            // 
            // eventListView
            // 
            this.eventListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.eventListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventListView.FullRowSelect = true;
            this.eventListView.HideSelection = false;
            this.eventListView.Location = new System.Drawing.Point(3, 19);
            this.eventListView.MultiSelect = false;
            this.eventListView.Name = "eventListView";
            this.eventListView.Size = new System.Drawing.Size(524, 171);
            this.eventListView.TabIndex = 0;
            this.eventListView.UseCompatibleStateImageBehavior = false;
            this.eventListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Payload";
            this.columnHeader6.Width = 180;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.eventListView);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 193);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Events";
            // 
            // JacdacExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "JacdacExplorer";
            this.Text = "Jacdac Explorer";
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).EndInit();
            this.splitContainerRight.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.TreeView deviceTree;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem usbDeviceSelectMenu;
        private System.Windows.Forms.ListView registerListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripMenuItem packetTracerToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView eventListView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}

