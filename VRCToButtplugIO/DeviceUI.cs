using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCToyController
{
    public partial class DeviceUI : UserControl
    {
        public DeviceUI()
        {
            InitializeComponent();

            b_test.Click += delegate (object o, EventArgs a)
            {
                if (Mediator.activeToys.ContainsKey(this.name.Text))
                    Mediator.activeToys[this.name.Text].Test();
            };
        }

        public string[] motorsValues;
        public DeviceUI(Toy toy)
        {
            InitializeComponent();

            this.name.Text = toy.name;
            this.Name = toy.name;
            b_test.Click += delegate (object o, EventArgs a)
            {
                if (Mediator.activeToys.ContainsKey(this.name.Text))
                    Mediator.activeToys[this.name.Text].Test();
            };

            motorsValues = new string[toy.motorCount];
            for (int i = 0; i < motorsValues.Length; i++)
            {
                motorsValues[i] = "" + i;
            }

            for(int i = 0; i < motorsValues.Length; i++)
            {
                DeviceParamsUI paramControl = new DeviceParamsUI(motorsValues);
                paramControl.motor.SelectedIndex = i;
                paramsList.Controls.Add(paramControl);
            }
        }

        public void Populate(Config config)
        {
            if (config.devices == null)
                return;
            foreach(Device d in config.devices)
            {
                if(d.device_name == this.name.Text)
                {
                    Populate(d);
                    return;
                }
            }
        }

        public void Populate(Device device)
        {
            for (int i = 0; i < device.device_params.Length; i++)
            {
                if(i>=paramsList.Controls.Count)
                    paramsList.Controls.Add(new DeviceParamsUI(motorsValues));
                ((DeviceParamsUI)paramsList.Controls[i]).Populate(device.device_params[i]);
            }
            if (device.device_params.Length < paramsList.Controls.Count)
            {
                for (int i = 0; i < paramsList.Controls.Count - device.device_params.Length; i++)
                    paramsList.Controls.Add(new DeviceParamsUI(motorsValues));
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            DeviceParamsUI paramControl = new DeviceParamsUI(motorsValues);
            paramsList.Controls.Add(paramControl);
        }
    }
}
