﻿namespace VRCToyController
{
    partial class BehaviourUI
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
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.sensorSelection = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.motor = new System.Windows.Forms.ComboBox();
            this.label_pixel_found = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.typeSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.max = new System.Windows.Forms.TextBox();
            this.groupVolume = new System.Windows.Forms.TableLayoutPanel();
            this.volume_depth = new System.Windows.Forms.TextBox();
            this.volume_width = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupThrusting = new System.Windows.Forms.TableLayoutPanel();
            this.thrust_speed_scale = new System.Windows.Forms.TextBox();
            this.thrust_acceleration = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.thrust_depth_scale = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupRubbing = new System.Windows.Forms.TableLayoutPanel();
            this.rub_scale = new System.Windows.Forms.TextBox();
            this.rub_acceleration = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.audioLinkSettings = new System.Windows.Forms.TableLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.audioLinkStrength = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.audioLinkChannel = new System.Windows.Forms.ComboBox();
            this.rem_button = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.strengthIndicator = new System.Windows.Forms.Label();
            this.battery_bar = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupVolume.SuspendLayout();
            this.groupThrusting.SuspendLayout();
            this.groupRubbing.SuspendLayout();
            this.audioLinkSettings.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel3);
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Controls.Add(this.groupVolume);
            this.flowLayoutPanel1.Controls.Add(this.groupThrusting);
            this.flowLayoutPanel1.Controls.Add(this.groupRubbing);
            this.flowLayoutPanel1.Controls.Add(this.audioLinkSettings);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(452, 253);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label1);
            this.flowLayoutPanel3.Controls.Add(this.sensorSelection);
            this.flowLayoutPanel3.Controls.Add(this.label11);
            this.flowLayoutPanel3.Controls.Add(this.motor);
            this.flowLayoutPanel3.Controls.Add(this.label_pixel_found);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(448, 37);
            this.flowLayoutPanel3.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sensor:";
            // 
            // sensorSelection
            // 
            this.sensorSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensorSelection.FormattingEnabled = true;
            this.sensorSelection.Location = new System.Drawing.Point(73, 3);
            this.sensorSelection.Name = "sensorSelection";
            this.sensorSelection.Size = new System.Drawing.Size(180, 28);
            this.sensorSelection.TabIndex = 12;
            this.sensorSelection.SelectedIndexChanged += new System.EventHandler(this.Apply_SensorName);
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(259, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "Feature:";
            // 
            // motor
            // 
            this.motor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.motor.FormattingEnabled = true;
            this.motor.Location = new System.Drawing.Point(334, 3);
            this.motor.Name = "motor";
            this.motor.Size = new System.Drawing.Size(97, 28);
            this.motor.TabIndex = 4;
            this.motor.SelectedIndexChanged += new System.EventHandler(this.Apply_Feature);
            this.motor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsInt);
            // 
            // label_pixel_found
            // 
            this.label_pixel_found.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_pixel_found.AutoSize = true;
            this.label_pixel_found.Location = new System.Drawing.Point(437, 7);
            this.label_pixel_found.Name = "label_pixel_found";
            this.label_pixel_found.Size = new System.Drawing.Size(0, 20);
            this.label_pixel_found.TabIndex = 5;
            this.label_pixel_found.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 46);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(448, 30);
            this.panel3.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel2.Location = new System.Drawing.Point(3, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(445, 5);
            this.panel2.TabIndex = 10;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label10);
            this.flowLayoutPanel2.Controls.Add(this.typeSelector);
            this.flowLayoutPanel2.Controls.Add(this.label4);
            this.flowLayoutPanel2.Controls.Add(this.max);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 82);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(430, 38);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 20);
            this.label10.TabIndex = 6;
            this.label10.Text = "Calculation:";
            // 
            // typeSelector
            // 
            this.typeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeSelector.FormattingEnabled = true;
            this.typeSelector.Location = new System.Drawing.Point(100, 3);
            this.typeSelector.Name = "typeSelector";
            this.typeSelector.Size = new System.Drawing.Size(147, 28);
            this.typeSelector.TabIndex = 3;
            this.typeSelector.SelectedIndexChanged += new System.EventHandler(this.typeSelector_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.Location = new System.Drawing.Point(253, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 29);
            this.label4.TabIndex = 4;
            this.label4.Text = "Max:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // max
            // 
            this.max.Location = new System.Drawing.Point(301, 3);
            this.max.Name = "max";
            this.max.Size = new System.Drawing.Size(100, 26);
            this.max.TabIndex = 5;
            this.max.Text = "1.0";
            this.max.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsFloatBetween0And1);
            this.max.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Max);
            // 
            // groupVolume
            // 
            this.groupVolume.ColumnCount = 2;
            this.groupVolume.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupVolume.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupVolume.Controls.Add(this.volume_depth, 1, 1);
            this.groupVolume.Controls.Add(this.volume_width, 1, 0);
            this.groupVolume.Controls.Add(this.label3, 0, 0);
            this.groupVolume.Controls.Add(this.label2, 0, 1);
            this.groupVolume.Location = new System.Drawing.Point(3, 126);
            this.groupVolume.Name = "groupVolume";
            this.groupVolume.RowCount = 2;
            this.groupVolume.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupVolume.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupVolume.Size = new System.Drawing.Size(246, 70);
            this.groupVolume.TabIndex = 3;
            this.toolTip1.SetToolTip(this.groupVolume, "(strength = widthScale * width + depthScale * depth)");
            // 
            // volume_depth
            // 
            this.volume_depth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.volume_depth.Location = new System.Drawing.Point(126, 39);
            this.volume_depth.Name = "volume_depth";
            this.volume_depth.Size = new System.Drawing.Size(100, 26);
            this.volume_depth.TabIndex = 1;
            this.volume_depth.Text = "0.5";
            this.volume_depth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.volume_depth.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Volume);
            // 
            // volume_width
            // 
            this.volume_width.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.volume_width.Location = new System.Drawing.Point(126, 4);
            this.volume_width.Name = "volume_width";
            this.volume_width.Size = new System.Drawing.Size(100, 26);
            this.volume_width.TabIndex = 0;
            this.volume_width.Text = "0.5";
            this.volume_width.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.volume_width.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Volume);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Width scale:";
            this.toolTip1.SetToolTip(this.label3, "(strength = widthScale * width + depthScale * depth)");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Depth scale:";
            this.toolTip1.SetToolTip(this.label2, "(strength = widthScale * width + depthScale * depth)");
            // 
            // groupThrusting
            // 
            this.groupThrusting.ColumnCount = 2;
            this.groupThrusting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupThrusting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupThrusting.Controls.Add(this.thrust_speed_scale, 1, 1);
            this.groupThrusting.Controls.Add(this.thrust_acceleration, 1, 0);
            this.groupThrusting.Controls.Add(this.label5, 0, 2);
            this.groupThrusting.Controls.Add(this.thrust_depth_scale, 1, 2);
            this.groupThrusting.Controls.Add(this.label7, 0, 0);
            this.groupThrusting.Controls.Add(this.label6, 0, 1);
            this.groupThrusting.Location = new System.Drawing.Point(457, 3);
            this.groupThrusting.Name = "groupThrusting";
            this.groupThrusting.RowCount = 3;
            this.groupThrusting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupThrusting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupThrusting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.groupThrusting.Size = new System.Drawing.Size(246, 98);
            this.groupThrusting.TabIndex = 4;
            this.toolTip1.SetToolTip(this.groupThrusting, "(strength = (depth*depthScale+thustSpeed*speedScale) *(acceleration*time))");
            // 
            // thrust_speed_scale
            // 
            this.thrust_speed_scale.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.thrust_speed_scale.Location = new System.Drawing.Point(126, 35);
            this.thrust_speed_scale.Name = "thrust_speed_scale";
            this.thrust_speed_scale.Size = new System.Drawing.Size(100, 26);
            this.thrust_speed_scale.TabIndex = 1;
            this.thrust_speed_scale.Text = "1.0";
            this.thrust_speed_scale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.thrust_speed_scale.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Thrusting);
            // 
            // thrust_acceleration
            // 
            this.thrust_acceleration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.thrust_acceleration.Location = new System.Drawing.Point(126, 3);
            this.thrust_acceleration.Name = "thrust_acceleration";
            this.thrust_acceleration.Size = new System.Drawing.Size(100, 26);
            this.thrust_acceleration.TabIndex = 0;
            this.thrust_acceleration.Text = "1.0";
            this.thrust_acceleration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.thrust_acceleration.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Thrusting);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Depth scale:";
            this.toolTip1.SetToolTip(this.label5, "(strength = (depth*depthScale+thustSpeed*speedScale) *(acceleration*time))");
            // 
            // thrust_depth_scale
            // 
            this.thrust_depth_scale.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.thrust_depth_scale.Location = new System.Drawing.Point(126, 68);
            this.thrust_depth_scale.Name = "thrust_depth_scale";
            this.thrust_depth_scale.Size = new System.Drawing.Size(100, 26);
            this.thrust_depth_scale.TabIndex = 5;
            this.thrust_depth_scale.Text = "1.0";
            this.thrust_depth_scale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.thrust_depth_scale.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Thrusting);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 20);
            this.label7.TabIndex = 2;
            this.label7.Text = "Speed scale:";
            this.toolTip1.SetToolTip(this.label7, "(strength = (depth*depthScale+thustSpeed*speedScale) *(acceleration*time))");
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Acceleration:";
            this.toolTip1.SetToolTip(this.label6, "(strength = (depth*depthScale+thustSpeed*speedScale) *(acceleration*time))");
            // 
            // groupRubbing
            // 
            this.groupRubbing.ColumnCount = 2;
            this.groupRubbing.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupRubbing.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupRubbing.Controls.Add(this.rub_scale, 1, 1);
            this.groupRubbing.Controls.Add(this.rub_acceleration, 1, 0);
            this.groupRubbing.Controls.Add(this.label8, 0, 0);
            this.groupRubbing.Controls.Add(this.label9, 0, 1);
            this.groupRubbing.Location = new System.Drawing.Point(457, 107);
            this.groupRubbing.Name = "groupRubbing";
            this.groupRubbing.RowCount = 2;
            this.groupRubbing.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupRubbing.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.groupRubbing.Size = new System.Drawing.Size(246, 70);
            this.groupRubbing.TabIndex = 5;
            this.toolTip1.SetToolTip(this.groupRubbing, "(strength = (rubSpeed*scale)*(acceleration*time))");
            // 
            // rub_scale
            // 
            this.rub_scale.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rub_scale.Location = new System.Drawing.Point(126, 39);
            this.rub_scale.Name = "rub_scale";
            this.rub_scale.Size = new System.Drawing.Size(100, 26);
            this.rub_scale.TabIndex = 1;
            this.rub_scale.Text = "1.0";
            this.rub_scale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.rub_scale.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Rubbing);
            // 
            // rub_acceleration
            // 
            this.rub_acceleration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rub_acceleration.Location = new System.Drawing.Point(126, 4);
            this.rub_acceleration.Name = "rub_acceleration";
            this.rub_acceleration.Size = new System.Drawing.Size(100, 26);
            this.rub_acceleration.TabIndex = 0;
            this.rub_acceleration.Text = "1.0";
            this.rub_acceleration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressIsNumber);
            this.rub_acceleration.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Apply_Rubbing);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "Acceleration:";
            this.toolTip1.SetToolTip(this.label8, "(strength = (rubSpeed*scale)*(acceleration*time))");
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(67, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 20);
            this.label9.TabIndex = 2;
            this.label9.Text = "Scale:";
            this.toolTip1.SetToolTip(this.label9, "(strength = (rubSpeed*scale)*(acceleration*time))");
            // 
            // audioLinkSettings
            // 
            this.audioLinkSettings.ColumnCount = 2;
            this.audioLinkSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.audioLinkSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.audioLinkSettings.Controls.Add(this.label13, 0, 0);
            this.audioLinkSettings.Controls.Add(this.audioLinkStrength, 1, 1);
            this.audioLinkSettings.Controls.Add(this.label12, 0, 1);
            this.audioLinkSettings.Controls.Add(this.audioLinkChannel, 1, 0);
            this.audioLinkSettings.Location = new System.Drawing.Point(709, 3);
            this.audioLinkSettings.Name = "audioLinkSettings";
            this.audioLinkSettings.RowCount = 2;
            this.audioLinkSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.audioLinkSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.audioLinkSettings.Size = new System.Drawing.Size(246, 75);
            this.audioLinkSettings.TabIndex = 9;
            this.toolTip1.SetToolTip(this.audioLinkSettings, "(strength = (rubSpeed*scale)*(acceleration*time))");
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(48, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 20);
            this.label13.TabIndex = 11;
            this.label13.Text = "Channel:";
            // 
            // audioLinkStrength
            // 
            this.audioLinkStrength.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.audioLinkStrength.Location = new System.Drawing.Point(126, 44);
            this.audioLinkStrength.Name = "audioLinkStrength";
            this.audioLinkStrength.Size = new System.Drawing.Size(100, 26);
            this.audioLinkStrength.TabIndex = 0;
            this.audioLinkStrength.Text = "1.0";
            this.audioLinkStrength.TextChanged += new System.EventHandler(this.Apply_AudioLink);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(45, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 20);
            this.label12.TabIndex = 3;
            this.label12.Text = "Strength:";
            this.toolTip1.SetToolTip(this.label12, "(strength = (rubSpeed*scale)*(acceleration*time))");
            // 
            // audioLinkChannel
            // 
            this.audioLinkChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audioLinkChannel.FormattingEnabled = true;
            this.audioLinkChannel.Location = new System.Drawing.Point(126, 3);
            this.audioLinkChannel.Name = "audioLinkChannel";
            this.audioLinkChannel.Size = new System.Drawing.Size(117, 28);
            this.audioLinkChannel.TabIndex = 10;
            this.audioLinkChannel.SelectedIndexChanged += new System.EventHandler(this.Apply_AudioLink);
            // 
            // rem_button
            // 
            this.rem_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rem_button.AutoSize = true;
            this.rem_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rem_button.Location = new System.Drawing.Point(374, 221);
            this.rem_button.Name = "rem_button";
            this.rem_button.Size = new System.Drawing.Size(78, 30);
            this.rem_button.TabIndex = 5;
            this.rem_button.Text = "Remove";
            this.rem_button.UseVisualStyleBackColor = true;
            this.rem_button.Click += new System.EventHandler(this.rem_button_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.strengthIndicator);
            this.panel1.Controls.Add(this.battery_bar);
            this.panel1.Location = new System.Drawing.Point(3, 247);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(452, 27);
            this.panel1.TabIndex = 5;
            // 
            // strengthIndicator
            // 
            this.strengthIndicator.BackColor = System.Drawing.Color.Cyan;
            this.strengthIndicator.Location = new System.Drawing.Point(0, 0);
            this.strengthIndicator.Name = "strengthIndicator";
            this.strengthIndicator.Size = new System.Drawing.Size(0, 20);
            this.strengthIndicator.TabIndex = 1;
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
            // BehaviourUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.rem_button);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "BehaviourUI";
            this.Size = new System.Drawing.Size(460, 277);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupVolume.ResumeLayout(false);
            this.groupVolume.PerformLayout();
            this.groupThrusting.ResumeLayout(false);
            this.groupThrusting.PerformLayout();
            this.groupRubbing.ResumeLayout(false);
            this.groupRubbing.PerformLayout();
            this.audioLinkSettings.ResumeLayout(false);
            this.audioLinkSettings.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        public System.Windows.Forms.ComboBox typeSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel groupVolume;
        private System.Windows.Forms.TableLayoutPanel groupThrusting;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel groupRubbing;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox thrust_acceleration;
        public System.Windows.Forms.TextBox thrust_speed_scale;
        public System.Windows.Forms.TextBox rub_acceleration;
        public System.Windows.Forms.TextBox rub_scale;
        public System.Windows.Forms.TextBox volume_width;
        public System.Windows.Forms.TextBox volume_depth;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox thrust_depth_scale;
        public System.Windows.Forms.TextBox max;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.ComboBox motor;
        private System.Windows.Forms.Button rem_button;
        public System.Windows.Forms.Label label_pixel_found;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel audioLinkSettings;
        public System.Windows.Forms.TextBox audioLinkStrength;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.ComboBox audioLinkChannel;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label battery_bar;
        public System.Windows.Forms.Label strengthIndicator;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox sensorSelection;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
    }
}
