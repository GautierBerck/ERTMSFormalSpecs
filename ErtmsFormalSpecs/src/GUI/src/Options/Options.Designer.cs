﻿namespace GUI.Options
{
    partial class Options
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.displayEnclosingMessagesCheckBox = new System.Windows.Forms.CheckBox();
            this.displayRequirementsAsListCheckBox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(472, 212);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.displayRequirementsAsListCheckBox);
            this.tabPage1.Controls.Add(this.displayEnclosingMessagesCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(464, 186);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Interface";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // displayEnclosingMessagesCheckBox
            // 
            this.displayEnclosingMessagesCheckBox.AutoSize = true;
            this.displayEnclosingMessagesCheckBox.Location = new System.Drawing.Point(8, 6);
            this.displayEnclosingMessagesCheckBox.Name = "displayEnclosingMessagesCheckBox";
            this.displayEnclosingMessagesCheckBox.Size = new System.Drawing.Size(158, 17);
            this.displayEnclosingMessagesCheckBox.TabIndex = 1;
            this.displayEnclosingMessagesCheckBox.Text = "Display enclosing messages";
            this.displayEnclosingMessagesCheckBox.UseVisualStyleBackColor = true;
            // 
            // displayRequirementsAsListCheckBox
            // 
            this.displayRequirementsAsListCheckBox.AutoSize = true;
            this.displayRequirementsAsListCheckBox.Location = new System.Drawing.Point(8, 29);
            this.displayRequirementsAsListCheckBox.Name = "displayRequirementsAsListCheckBox";
            this.displayRequirementsAsListCheckBox.Size = new System.Drawing.Size(152, 17);
            this.displayRequirementsAsListCheckBox.TabIndex = 2;
            this.displayRequirementsAsListCheckBox.Text = "Display requirements as list";
            this.displayRequirementsAsListCheckBox.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 212);
            this.Controls.Add(this.tabControl1);
            this.Name = "Options";
            this.Text = "Options";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox displayEnclosingMessagesCheckBox;
        private System.Windows.Forms.CheckBox displayRequirementsAsListCheckBox;
    }
}