using Microsoft.VisualBasic;
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

using MaterialSkin;
using MaterialSkin.Controls;

namespace VRCToyController
{
    public partial class MainUI : MaterialForm
    {
        public MainUI()
        {
            InitializeComponent();

            this.Icon = new System.Drawing.Icon("./lib/icon.ico");
            scan.Click += new EventHandler(Scan);

            scan.Text = "Stop Scanning";

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Pink300, Primary.Pink300,
                Primary.Pink300, Accent.Pink100,
                TextShade.WHITE
            );

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

        private void b_Save_Click(object sender, EventArgs e)
        {
            Config.Singleton.Save();
        }

        private void footer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://thryrallo.de");
        }

        private async void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Config.Singleton.Save();
            await KeyManager.FreeKeyAsync();
            await KeyManager.FreeKeyAsync();
            this.FormClosing -= MainUI_FormClosing;
            this.Invoke((Action)delegate ()
            {
                this.Close();
            });
        }

        private void button_AddLovenseConnectURL_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                LovenseConnectAPI api = (Mediator.toyAPIs.Where(a => a is LovenseConnectAPI).First() as LovenseConnectAPI);
                Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                string input = "";
                while (api.AddCustomURL(input) == false)
                {
                    input = Interaction.InputBox("Please input the Local IP from your Lovense Connect App on your phone.", "Lovense Connect Local IP", "https://192-168-0-1.lovense.club:34568/GetToys", rect.Width / 2 - 200, rect.Height / 2 - 200);
                    input = input.Trim();
                    if (input.Length == 0)
                    {
                        break;
                    }
                }
            });
        }
    }
}
