// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------
namespace GUI.SpecificationView
{
    partial class Window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.nextErrortoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextWarningToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nextInfoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.specBrowserTreeView = new GUI.SpecificationView.SpecificationTreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid = new GUI.MyPropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.specTabPage = new System.Windows.Forms.TabPage();
            this.specBrowserTextView = new GUI.EditorTextBox();
            this.implementationTabPage = new System.Windows.Forms.TabPage();
            this.specBrowserRuleView = new GUI.SpecificationView.SpecificationTreeView();
            this.messagesGroupBox = new System.Windows.Forms.GroupBox();
            this.messagesRichTextBox = new GUI.EditorTextBox();
            this.functionalBlocksTabPage = new System.Windows.Forms.TabPage();
            this.functionalBlocksTreeView = new GUI.SpecificationView.FunctionalBlocksTreeView();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.specTabPage.SuspendLayout();
            this.implementationTabPage.SuspendLayout();
            this.messagesGroupBox.SuspendLayout();
            this.functionalBlocksTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripButton2,
            this.nextErrortoolStripButton,
            this.nextWarningToolStripButton,
            this.nextInfoToolStripButton,
            this.toolStripSeparator2});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(352, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel1.Text = "Specifications";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Select previous marking";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "Select next marking";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // nextErrortoolStripButton
            // 
            this.nextErrortoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextErrortoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextErrortoolStripButton.Image")));
            this.nextErrortoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextErrortoolStripButton.Name = "nextErrortoolStripButton";
            this.nextErrortoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextErrortoolStripButton.Text = "Next error";
            this.nextErrortoolStripButton.Click += new System.EventHandler(this.nextErrortoolStripButton_Click);
            // 
            // nextWarningToolStripButton
            // 
            this.nextWarningToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextWarningToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextWarningToolStripButton.Image")));
            this.nextWarningToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextWarningToolStripButton.Name = "nextWarningToolStripButton";
            this.nextWarningToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextWarningToolStripButton.Text = "Next warning";
            this.nextWarningToolStripButton.Click += new System.EventHandler(this.nextWarningToolStripButton_Click);
            // 
            // nextInfoToolStripButton
            // 
            this.nextInfoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextInfoToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("nextInfoToolStripButton.Image")));
            this.nextInfoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextInfoToolStripButton.Name = "nextInfoToolStripButton";
            this.nextInfoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nextInfoToolStripButton.Text = "Next info";
            this.nextInfoToolStripButton.Click += new System.EventHandler(this.nextInfoToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.specBrowserTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(352, 733);
            this.splitContainer1.SplitterDistance = 365;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // specBrowserTreeView
            // 
            this.specBrowserTreeView.AllowDrop = true;
            this.specBrowserTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specBrowserTreeView.HideSelection = false;
            this.specBrowserTreeView.ImageIndex = 0;
            this.specBrowserTreeView.LabelEdit = true;
            this.specBrowserTreeView.Location = new System.Drawing.Point(0, 0);
            this.specBrowserTreeView.Name = "specBrowserTreeView";
            this.specBrowserTreeView.Root = null;
            this.specBrowserTreeView.Selected = null;
            this.specBrowserTreeView.SelectedImageIndex = 0;
            this.specBrowserTreeView.Size = new System.Drawing.Size(352, 365);
            this.specBrowserTreeView.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.messagesGroupBox);
            this.splitContainer3.Size = new System.Drawing.Size(352, 365);
            this.splitContainer3.SplitterDistance = 279;
            this.splitContainer3.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(352, 279);
            this.splitContainer2.SplitterDistance = 195;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.AllowDrop = true;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(352, 195);
            this.propertyGrid.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.specTabPage);
            this.tabControl1.Controls.Add(this.implementationTabPage);
            this.tabControl1.Controls.Add(this.functionalBlocksTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(352, 81);
            this.tabControl1.TabIndex = 5;
            // 
            // specTabPage
            // 
            this.specTabPage.Controls.Add(this.specBrowserTextView);
            this.specTabPage.Location = new System.Drawing.Point(4, 22);
            this.specTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.specTabPage.Name = "specTabPage";
            this.specTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.specTabPage.Size = new System.Drawing.Size(344, 55);
            this.specTabPage.TabIndex = 0;
            this.specTabPage.Text = "Description";
            this.specTabPage.UseVisualStyleBackColor = true;
            // 
            // specBrowserTextView
            // 
            this.specBrowserTextView.AllowDrop = true;
            this.specBrowserTextView.AutoComplete = true;
            this.specBrowserTextView.ConsiderOnlyTypes = false;
            this.specBrowserTextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specBrowserTextView.Instance = null;
            this.specBrowserTextView.Lines = new string[] { "" };
            this.specBrowserTextView.Location = new System.Drawing.Point(2, 2);
            this.specBrowserTextView.Name = "specBrowserTextView";
            this.specBrowserTextView.ReadOnly = false;
            this.specBrowserTextView.Rtf = resources.GetString("specBrowserTextView.Rtf");
            this.specBrowserTextView.Size = new System.Drawing.Size(340, 51);
            this.specBrowserTextView.TabIndex = 3;
            // 
            // implementationTabPage
            // 
            this.implementationTabPage.Controls.Add(this.specBrowserRuleView);
            this.implementationTabPage.Location = new System.Drawing.Point(4, 22);
            this.implementationTabPage.Margin = new System.Windows.Forms.Padding(2);
            this.implementationTabPage.Name = "implementationTabPage";
            this.implementationTabPage.Padding = new System.Windows.Forms.Padding(2);
            this.implementationTabPage.Size = new System.Drawing.Size(344, 55);
            this.implementationTabPage.TabIndex = 1;
            this.implementationTabPage.Text = "Implementation";
            this.implementationTabPage.UseVisualStyleBackColor = true;
            // 
            // specBrowserRuleView
            // 
            this.specBrowserRuleView.AllowDrop = true;
            this.specBrowserRuleView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specBrowserRuleView.HideSelection = false;
            this.specBrowserRuleView.ImageIndex = 0;
            this.specBrowserRuleView.LabelEdit = true;
            this.specBrowserRuleView.Location = new System.Drawing.Point(2, 2);
            this.specBrowserRuleView.Name = "specBrowserRuleView";
            this.specBrowserRuleView.Root = null;
            this.specBrowserRuleView.Selected = null;
            this.specBrowserRuleView.SelectedImageIndex = 0;
            this.specBrowserRuleView.Size = new System.Drawing.Size(340, 51);
            this.specBrowserRuleView.TabIndex = 4;
            // 
            // messagesGroupBox
            // 
            this.messagesGroupBox.Controls.Add(this.messagesRichTextBox);
            this.messagesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.messagesGroupBox.Name = "messagesGroupBox";
            this.messagesGroupBox.Size = new System.Drawing.Size(352, 82);
            this.messagesGroupBox.TabIndex = 0;
            this.messagesGroupBox.TabStop = false;
            this.messagesGroupBox.Text = "Messages";
            // 
            // messagesRichTextBox
            // 
            this.messagesRichTextBox.AllowDrop = true;
            this.messagesRichTextBox.AutoComplete = true;
            this.messagesRichTextBox.ConsiderOnlyTypes = false;
            this.messagesRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesRichTextBox.Instance = null;
            this.messagesRichTextBox.Lines = new string[] { "" };
            this.messagesRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.messagesRichTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.messagesRichTextBox.Name = "messagesRichTextBox";
            this.messagesRichTextBox.ReadOnly = false;
            this.messagesRichTextBox.Rtf = resources.GetString("messagesRichTextBox.Rtf");
            this.messagesRichTextBox.Size = new System.Drawing.Size(346, 63);
            this.messagesRichTextBox.TabIndex = 0;
            // 
            // functionalBlocksTabPage
            // 
            this.functionalBlocksTabPage.Controls.Add(this.functionalBlocksTreeView);
            this.functionalBlocksTabPage.Location = new System.Drawing.Point(4, 22);
            this.functionalBlocksTabPage.Name = "functionalBlocksTabPage";
            this.functionalBlocksTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.functionalBlocksTabPage.Size = new System.Drawing.Size(344, 55);
            this.functionalBlocksTabPage.TabIndex = 2;
            this.functionalBlocksTabPage.Text = "Functional blocks";
            this.functionalBlocksTabPage.UseVisualStyleBackColor = true;
            // 
            // functionalBlocksTreeView
            // 
            this.functionalBlocksTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionalBlocksTreeView.Location = new System.Drawing.Point(3, 3);
            this.functionalBlocksTreeView.Name = "functionalBlocksTreeView";
            this.functionalBlocksTreeView.Size = new System.Drawing.Size(338, 49);
            this.functionalBlocksTreeView.TabIndex = 0;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 758);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.ShowInTaskbar = false;
            this.Text = "Specifications";
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.specTabPage.ResumeLayout(false);
            this.implementationTabPage.ResumeLayout(false);
            this.messagesGroupBox.ResumeLayout(false);
            this.functionalBlocksTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip3;
        private SpecificationTreeView specBrowserTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private MyPropertyGrid propertyGrid;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage specTabPage;
        public EditorTextBox specBrowserTextView;
        private System.Windows.Forms.TabPage implementationTabPage;
        public GUI.SpecificationView.SpecificationTreeView specBrowserRuleView;
        private EditorTextBox messagesRichTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton nextErrortoolStripButton;
        private System.Windows.Forms.ToolStripButton nextWarningToolStripButton;
        private System.Windows.Forms.ToolStripButton nextInfoToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox messagesGroupBox;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.TabPage functionalBlocksTabPage;
        public GUI.SpecificationView.FunctionalBlocksTreeView functionalBlocksTreeView;

    }
}