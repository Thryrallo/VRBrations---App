﻿namespace VRCToyController
{
    partial class MainUI
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
            this.scan = new System.Windows.Forms.Button();
            this.deviceList = new System.Windows.Forms.FlowLayoutPanel();
            this.b_Save = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.footer = new System.Windows.Forms.LinkLabel();
            this.warning = new System.Windows.Forms.Label();
            this.deviceList.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scan
            // 
            this.scan.Location = new System.Drawing.Point(12, 12);
            this.scan.Name = "scan";
            this.scan.Size = new System.Drawing.Size(130, 42);
            this.scan.TabIndex = 0;
            this.scan.Text = "scan";
            this.scan.UseVisualStyleBackColor = true;
            // 
            // deviceList
            // 
            this.deviceList.AutoScroll = true;
            this.deviceList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deviceList.Controls.Add(this.warning);
            this.deviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.deviceList.Location = new System.Drawing.Point(155, 0);
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(743, 836);
            this.deviceList.TabIndex = 1;
            this.deviceList.WrapContents = false;
            // 
            // b_Save
            // 
            this.b_Save.Location = new System.Drawing.Point(12, 171);
            this.b_Save.Name = "b_Save";
            this.b_Save.Size = new System.Drawing.Size(130, 56);
            this.b_Save.TabIndex = 4;
            this.b_Save.Text = "Save";
            this.b_Save.UseVisualStyleBackColor = true;
            this.b_Save.Click += new System.EventHandler(this.b_Save_Click);
            // 
            // apply
            // 
            this.apply.Location = new System.Drawing.Point(12, 109);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(130, 56);
            this.apply.TabIndex = 5;
            this.apply.Text = "Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.footer);
            this.panel1.Controls.Add(this.b_Save);
            this.panel1.Controls.Add(this.apply);
            this.panel1.Controls.Add(this.scan);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 836);
            this.panel1.TabIndex = 6;
            // 
            // footer
            // 
            this.footer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.footer.AutoSize = true;
            this.footer.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.footer.LinkColor = System.Drawing.Color.Fuchsia;
            this.footer.Location = new System.Drawing.Point(34, 807);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(88, 20);
            this.footer.TabIndex = 6;
            this.footer.TabStop = true;
            this.footer.Text = "by Thryrallo";
            this.footer.VisitedLinkColor = System.Drawing.Color.Fuchsia;
            this.footer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.footer_LinkClicked);
            // 
            // warning
            // 
            this.warning.AutoSize = true;
            this.warning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.warning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.warning.Location = new System.Drawing.Point(3, 5);
            this.warning.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(53, 22);
            this.warning.TabIndex = 1;
            this.warning.Text = "label1";
            this.warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.warning.Visible = false;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(898, 836);
            this.Controls.Add(this.deviceList);
            this.Controls.Add(this.panel1);
            this.Name = "MainUI";
            this.Text = "VRC Toy Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.deviceList.ResumeLayout(false);
            this.deviceList.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button scan;
        public System.Windows.Forms.FlowLayoutPanel deviceList;
        private System.Windows.Forms.Button b_Save;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel footer;
        public System.Windows.Forms.Label warning;
    }
}
