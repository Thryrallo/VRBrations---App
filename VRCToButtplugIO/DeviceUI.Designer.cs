namespace VRCToyController
{
    partial class DeviceUI
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.name = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.battery_bar = new System.Windows.Forms.Label();
            this.b_test = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.paramsList = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Controls.Add(this.paramsList);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(542, 52);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.name);
            this.flowLayoutPanel2.Controls.Add(this.panel1);
            this.flowLayoutPanel2.Controls.Add(this.b_test);
            this.flowLayoutPanel2.Controls.Add(this.button_add);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(534, 38);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // name
            // 
            this.name.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.Location = new System.Drawing.Point(3, 9);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(53, 20);
            this.name.TabIndex = 8;
            this.name.Text = "name";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.battery_bar);
            this.panel1.Location = new System.Drawing.Point(62, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 32);
            this.panel1.TabIndex = 9;
            // 
            // battery_bar
            // 
            this.battery_bar.BackColor = System.Drawing.Color.Red;
            this.battery_bar.Location = new System.Drawing.Point(1, 1);
            this.battery_bar.Margin = new System.Windows.Forms.Padding(0);
            this.battery_bar.Name = "battery_bar";
            this.battery_bar.Size = new System.Drawing.Size(0, 31);
            this.battery_bar.TabIndex = 0;
            // 
            // b_test
            // 
            this.b_test.Location = new System.Drawing.Point(268, 3);
            this.b_test.Name = "b_test";
            this.b_test.Size = new System.Drawing.Size(75, 32);
            this.b_test.TabIndex = 4;
            this.b_test.Text = "Test";
            this.b_test.UseVisualStyleBackColor = true;
            this.b_test.Click += new System.EventHandler(this.b_test_Click);
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(349, 3);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(182, 32);
            this.button_add.TabIndex = 5;
            this.button_add.Text = "Add new behaviour";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // paramsList
            // 
            this.paramsList.AutoSize = true;
            this.paramsList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.paramsList.Location = new System.Drawing.Point(3, 47);
            this.paramsList.Name = "paramsList";
            this.paramsList.Size = new System.Drawing.Size(0, 0);
            this.paramsList.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // DeviceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DeviceUI";
            this.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.Size = new System.Drawing.Size(548, 65);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.FlowLayoutPanel paramsList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        public System.Windows.Forms.Label name;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label battery_bar;
        private System.Windows.Forms.Button b_test;
        private System.Windows.Forms.Button button_add;
    }
}
