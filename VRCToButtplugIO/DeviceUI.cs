using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static VRCToyController.Toy;

namespace VRCToyController
{
    public partial class DeviceUI : UserControl
    {

        private bool is_testing;
        public DeviceUI()
        {
            InitializeComponent();

            /*b_test.Click += delegate (object o, EventArgs a)
            {
                if (Mediator.activeToys.ContainsKey(this.Name))
                    Mediator.activeToys[this.Name].Test();
            };*/
        }

        public string[] motorsValues;
        public DeviceUI(Toy toy)
        {
            InitializeComponent();

            this.name.Text = toy.name;
            this.Name = toy.vrcToys_id;
            b_test.Click += delegate (object o, EventArgs a)
            {
                if (Mediator.activeToys.ContainsKey(toy.vrcToys_id))
                    Mediator.activeToys[toy.vrcToys_id].Test();
            };

            motorsValues = new string[toy.totalFeatureCount];
            int j = 0;
            foreach(KeyValuePair<ToyFeatureType,int> featureC in toy.featureCount)
            {
                for(int f = 0; f < featureC.Value; f++)
                {
                    motorsValues[j++] = featureC.Key + (f > 0 ? " " + f : "");
                }
            }

            Device savedDevice = Config.Singleton.devices.FirstOrDefault(d => d.device_name == toy.vrcToys_id);
            if (savedDevice == null)
            {
                Program.DebugToFile("[UI] Creating fresh UI for " + toy.name);
                for (int i = 0; i < motorsValues.Length; i++)
                {
                    DeviceParamsUI paramControl = new DeviceParamsUI(motorsValues, true, this);
                    paramControl.motor.SelectedIndex = i;
                    paramControl.x.Value = i;
                }
            }
            else
            {
                Program.DebugToFile("[UI] Creating UI for " + toy.name +" from save file");
                Populate(savedDevice);
            }
        }

        private void Populate(Device device)
        {
            for (int i = 0; i < device.device_params.Length; i++)
            {
                if (i >= paramsList.Controls.Count)
                {
                    new DeviceParamsUI(motorsValues, false, this);
                }
                ((DeviceParamsUI)paramsList.Controls[i]).Populate(device.device_params[i]);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            DeviceParamsUI paramControl = new DeviceParamsUI(motorsValues,true,this);
            paramsList.Controls.Add(paramControl);
        }

        private void b_test_Click(object sender, EventArgs e)
        {
            if(is_testing == false)
            {
                Task.Factory.StartNew(() =>
                {
                    Toy toy = Mediator.activeToys[this.Name];
                    if (toy != null) {
                        is_testing = true;
                        foreach (Control c in paramsList.Controls)
                        {
                            DeviceParams deviceParam = (c as DeviceParamsUI).GetDeviceParam();
                            double[] strengths = new double[toy.totalFeatureCount];
                            for (int i = 0; i < strengths.Length; i++) strengths[i] = 0;
                            strengths[deviceParam.motor] = deviceParam.max;
                            //Console.WriteLine("Testing " + deviceParam.motor + " at " + deviceParam.max);
                            toy.ExecuteFeatures(strengths);
                            System.Threading.Thread.Sleep(2000);
                        }
                        is_testing = false;
                    }
                });
            }
        }

        private void DeviceUI_Load(object sender, EventArgs e)
        {

        }
    }
}
