namespace VRCToyController
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
            try
            {
                base.Dispose(disposing);
            }
            catch (System.Exception e)
            {
                
            }
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
            this.warning = new System.Windows.Forms.Label();
            this.b_Save = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_vrc_focus = new System.Windows.Forms.Label();
            this.footer = new System.Windows.Forms.LinkLabel();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.configSelector = new System.Windows.Forms.ComboBox();
            this.materialFlatButton1 = new MaterialSkin.Controls.MaterialFlatButton();
            this.looseFocus = new System.Windows.Forms.Label();
            this.deviceList.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // scan
            // 
            this.scan.Location = new System.Drawing.Point(6, 368);
            this.scan.Name = "scan";
            this.scan.Size = new System.Drawing.Size(130, 42);
            this.scan.TabIndex = 0;
            this.scan.Text = "scan";
            this.scan.UseVisualStyleBackColor = true;
            this.scan.Visible = false;
            // 
            // deviceList
            // 
            this.deviceList.AutoSize = true;
            this.deviceList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deviceList.BackColor = System.Drawing.Color.White;
            this.deviceList.Controls.Add(this.warning);
            this.deviceList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.deviceList.Location = new System.Drawing.Point(29, 139);
            this.deviceList.Margin = new System.Windows.Forms.Padding(3, 3, 10, 100);
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(59, 27);
            this.deviceList.TabIndex = 1;
            this.deviceList.WrapContents = false;
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
            // b_Save
            // 
            this.b_Save.Location = new System.Drawing.Point(3, 432);
            this.b_Save.Name = "b_Save";
            this.b_Save.Size = new System.Drawing.Size(130, 56);
            this.b_Save.TabIndex = 4;
            this.b_Save.Text = "Save";
            this.b_Save.UseVisualStyleBackColor = true;
            this.b_Save.Visible = false;
            this.b_Save.Click += new System.EventHandler(this.b_Save_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.b_Save);
            this.panel1.Controls.Add(this.scan);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(23, 736);
            this.panel1.TabIndex = 6;
            this.panel1.Visible = false;
            // 
            // label_vrc_focus
            // 
            this.label_vrc_focus.BackColor = System.Drawing.Color.White;
            this.label_vrc_focus.Location = new System.Drawing.Point(26, 107);
            this.label_vrc_focus.Name = "label_vrc_focus";
            this.label_vrc_focus.Size = new System.Drawing.Size(592, 20);
            this.label_vrc_focus.TabIndex = 7;
            this.label_vrc_focus.Text = "Window Indicator";
            // 
            // footer
            // 
            this.footer.AutoSize = true;
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.footer.LinkColor = System.Drawing.Color.Fuchsia;
            this.footer.Location = new System.Drawing.Point(0, 810);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(88, 20);
            this.footer.TabIndex = 6;
            this.footer.TabStop = true;
            this.footer.Text = "by Thryrallo";
            this.footer.VisitedLinkColor = System.Drawing.Color.Fuchsia;
            this.footer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.footer_LinkClicked);
            // 
            // panel_bottom
            // 
            this.panel_bottom.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel_bottom.Controls.Add(this.label1);
            this.panel_bottom.Controls.Add(this.configSelector);
            this.panel_bottom.Controls.Add(this.materialFlatButton1);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.Location = new System.Drawing.Point(0, 741);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Size = new System.Drawing.Size(898, 69);
            this.panel_bottom.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 25);
            this.label1.TabIndex = 11;
            this.label1.Text = "Config File:";
            // 
            // configSelector
            // 
            this.configSelector.FormattingEnabled = true;
            this.configSelector.Location = new System.Drawing.Point(142, 11);
            this.configSelector.Name = "configSelector";
            this.configSelector.Size = new System.Drawing.Size(170, 28);
            this.configSelector.TabIndex = 10;
            this.configSelector.SelectedIndexChanged += new System.EventHandler(this.configSelector_SelectedIndexChanged);
            this.configSelector.TextChanged += new System.EventHandler(this.configSelector_TextChanged);
            this.configSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.configSelector_KeyDown);
            // 
            // materialFlatButton1
            // 
            this.materialFlatButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.materialFlatButton1.AutoSize = true;
            this.materialFlatButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialFlatButton1.Depth = 0;
            this.materialFlatButton1.Icon = null;
            this.materialFlatButton1.Location = new System.Drawing.Point(684, 6);
            this.materialFlatButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialFlatButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialFlatButton1.Name = "materialFlatButton1";
            this.materialFlatButton1.Primary = false;
            this.materialFlatButton1.Size = new System.Drawing.Size(201, 36);
            this.materialFlatButton1.TabIndex = 9;
            this.materialFlatButton1.Text = "Add Lovense URL";
            this.materialFlatButton1.UseVisualStyleBackColor = true;
            this.materialFlatButton1.Click += new System.EventHandler(this.button_AddLovenseConnectURL_Click);
            // 
            // looseFocus
            // 
            this.looseFocus.Location = new System.Drawing.Point(490, 22);
            this.looseFocus.Name = "looseFocus";
            this.looseFocus.Size = new System.Drawing.Size(0, 0);
            this.looseFocus.TabIndex = 10;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(898, 830);
            this.Controls.Add(this.looseFocus);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.footer);
            this.Controls.Add(this.label_vrc_focus);
            this.Controls.Add(this.deviceList);
            this.Controls.Add(this.panel1);
            this.Name = "MainUI";
            this.Text = "VRbrations 3.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.deviceList.ResumeLayout(false);
            this.deviceList.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel_bottom.ResumeLayout(false);
            this.panel_bottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button scan;
        public System.Windows.Forms.FlowLayoutPanel deviceList;
        private System.Windows.Forms.Button b_Save;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel footer;
        public System.Windows.Forms.Label warning;
        public System.Windows.Forms.Label label_vrc_focus;
        private System.Windows.Forms.Panel panel_bottom;
        private MaterialSkin.Controls.MaterialFlatButton materialFlatButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox configSelector;
        private System.Windows.Forms.Label looseFocus;
    }
}

