﻿using Buttplug.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCToyController
{
    public partial class MainUI : Form
    {
        public MainUI()
        {
            InitializeComponent();

            this.Icon = new System.Drawing.Icon("./lib/icon.ico");
            scan.Click += new EventHandler(Scan);
        }

        private void Scan(object sender, EventArgs e)
        {
            if(Mediator.ui.scan.Text== "Start scanning")
            {
                Mediator.ui.scan.Text = "Stop scanning";
                foreach (ToyAPI api in Mediator.toyAPIs)
                {
                    if (api is ButtplugIOAPI)
                        ((ButtplugIOAPI)api).StartScanning();
                    else if (api is LovenseConnectAPI)
                        ((LovenseConnectAPI)api).GetToys();
                }
            }
            else
            {
                Mediator.ui.scan.Text = "Start scanning";
                foreach (ToyAPI api in Mediator.toyAPIs)
                {
                    if (api is ButtplugIOAPI)
                        ((ButtplugIOAPI)api).StartScanning();
                    else
                        ((LovenseConnectAPI)api).ClearToys();

                }
            }
            //Console.WriteLine("connected devieces: "+Mediator.buttplugIOInterface.connectedDevices());
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void apply_Click(object sender, EventArgs e)
        {
            Config config = Config.config;
            if (config.devices == null)
                config.devices = new Device[0];
            foreach (Control deviceControl in deviceList.Controls)
            {
                if (deviceControl.GetType() != typeof(DeviceUI) )
                    continue;
                DeviceUI deviceUI = (DeviceUI)deviceControl;
                Device device = null;
                foreach (Device d in config.devices)
                {
                    if (d.device_name == deviceUI.Name)
                        device = d;
                }
                if (device == null)
                {
                    device = new Device();
                    List<Device> list = new List<Device>(config.devices);
                    list.Add(device);
                    config.devices = list.ToArray();
                }
                device.device_name = deviceUI.Name;
                device.motors = deviceUI.motorsValues.Length;
                for (int i = 0; i < deviceUI.paramsList.Controls.Count; i++)
                {
                    DeviceParamsUI deviceParamsUI = (DeviceParamsUI)deviceUI.paramsList.Controls[i];
                    DeviceParams param = null;
                    if (i < device.device_params.Length)
                        param = device.device_params[i];
                    else
                    {
                        param = new DeviceParams();
                        List<DeviceParams> list = new List<DeviceParams>(device.device_params);
                        list.Add(param);
                        device.device_params = list.ToArray();
                    }
                    param.input_pos = new int[] { (int)deviceParamsUI.x.Value, (int)deviceParamsUI.y.Value };
                    param.max = Math.Min(1.0f, Math.Max(0.0f, float.Parse(deviceParamsUI.max.Text.Replace('.', ',')) ));
                    param.type = (CalulcationType)deviceParamsUI.typeSelector.SelectedIndex;
                    param.motor = deviceParamsUI.motor.SelectedIndex;
                    param.volume_width = float.Parse(deviceParamsUI.volume_width.Text.Replace('.', ','));
                    param.volume_depth = float.Parse(deviceParamsUI.volume_depth.Text.Replace('.', ','));
                    param.thrusting_acceleration = Math.Min(1,float.Parse(deviceParamsUI.thrust_acceleration.Text.Replace('.', ',')));
                    param.thrusting_speed_scale = float.Parse(deviceParamsUI.thrust_speed_scale.Text.Replace('.', ','));
                    param.thrusting_depth_scale = float.Parse(deviceParamsUI.thrust_depth_scale.Text.Replace('.', ','));
                    param.rubbing_acceleration = Math.Min(1, float.Parse(deviceParamsUI.rub_acceleration.Text.Replace('.', ',')));
                    param.rubbing_scale = float.Parse(deviceParamsUI.rub_scale.Text.Replace('.', ','));
                }
                if(device.device_params.Length> deviceUI.paramsList.Controls.Count)
                {
                    List<DeviceParams> list = new List<DeviceParams>(device.device_params);
                    list = list.GetRange(0, deviceUI.paramsList.Controls.Count);
                    device.device_params = list.ToArray();
                }
            }
        }

        private void b_Save_Click(object sender, EventArgs e)
        {
            apply_Click(sender, e);
            Config.config.Save();
        }

        private void footer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://thryrallo.de");
        }

        private async void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            await KeyManager.FreeKeyAsync();
            await KeyManager.FreeKeyAsync();
            this.FormClosing -= MainUI_FormClosing;
            this.Invoke((Action)delegate ()
            {
                this.Close();
            });
        }
    }
}