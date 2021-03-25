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
    public partial class DeviceParamsUI : UserControl
    {
        private DeviceUI deviceUI;
        private DeviceParams param;

        private bool blockApply = false;

        public DeviceParamsUI(string[] motorValues, bool loadedFromConfig, DeviceUI deviceUI)
        {
            InitializeComponent();

            this.deviceUI = deviceUI;

            foreach (object o in Enum.GetValues(typeof(CalulcationType)))
                this.typeSelector.Items.Add(o);
            
            motor.Items.AddRange(motorValues);

            if (loadedFromConfig)
            {
                typeSelector.SelectedIndex = 0;
                EnagleSelectedGroup();
                motor.SelectedIndex = 0;
                Apply_All();
            }

            deviceUI.paramsList.Controls.Add(this);
            this.LoadCorrelatingDeviceParams();
        }

        private void Apply_All()
        {
            Apply_InputPos(null, EventArgs.Empty);
            Apply_Max(null, new KeyEventArgs(Keys.None));
            Apply_Type(null, EventArgs.Empty);
            Apply_Feature(null, EventArgs.Empty);
            Apply_Volume(null, new KeyEventArgs(Keys.None));
            Apply_Thrusting(null, new KeyEventArgs(Keys.None));
            Apply_Rubbing(null, new KeyEventArgs(Keys.None));
        }

        private void Apply_InputPos(object sender, EventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.input_pos = new int[] { (int)this.x.Value, (int)this.y.Value };
        }

        private void Apply_Max(object sender, KeyEventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.max = Math.Min(1.0f, Math.Max(0.0f, ParseFloat(this.max.Text)));
        }

        private void Apply_Type(object sender, EventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.type = (CalulcationType)this.typeSelector.SelectedIndex;
        }

        private void Apply_Feature(object sender, EventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.motor = this.motor.SelectedIndex;
        }

        private void Apply_Volume(object sender, KeyEventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.volume_width = ParseFloat(this.volume_width);
            param.volume_depth = ParseFloat(this.volume_depth);
        }

        private void Apply_Thrusting(object sender, KeyEventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.thrusting_acceleration = Math.Min(1, ParseFloat(this.thrust_acceleration));
            param.thrusting_speed_scale = ParseFloat(this.thrust_speed_scale);
            param.thrusting_depth_scale = ParseFloat(this.thrust_depth_scale);
        }

        private void Apply_Rubbing(object sender, KeyEventArgs e)
        {
            if (blockApply) return;
            LoadCorrelatingDeviceParams();
            param.rubbing_acceleration = Math.Min(1, ParseFloat(this.rub_acceleration));
            param.rubbing_scale = ParseFloat(this.rub_scale);
        }

        private void LoadCorrelatingDeviceParams()
        {
            if (param != null) return;

            Device device = GetCorrelatingDevice();

            int index = deviceUI.paramsList.Controls.IndexOf(this);
            if (index < device.device_params.Length && index > -1)
            {
                param = device.device_params[index];
                param.SetDeviceParamsUI(this);
            }
            else
            {
                param = new DeviceParams();
                param.SetDeviceParamsUI(this);
                List<DeviceParams> list = new List<DeviceParams>(device.device_params);
                list.Add(param);
                device.device_params = list.ToArray();
            }
        }

        private Device GetCorrelatingDevice()
        {
            Device device = Config.Singleton.devices.FirstOrDefault(d => d.device_name == deviceUI.Name);
            if (device == null)
            {
                device = new Device();
                device.device_name = deviceUI.Name;
                device.motors = deviceUI.motorsValues.Length;
                List<Device> list = new List<Device>(Config.Singleton.devices);
                list.Add(device);
                Config.Singleton.devices = list.ToArray();
            }
            return device;
        }

        private float ParseFloat(TextBox box)
        {
            float f;
            if (float.TryParse(box.Text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out f))
            {
                return f;
            }
            box.Text = "1.0";
            return 1.0f;
        }
        private float ParseFloat(string s)
        {
            float f;
            if(float.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out f)){
                return f;
            }
            return 1.0f;
        }

        public void Populate(DeviceParams param)
        {
            blockApply = true;

            this.x.Value = param.input_pos[0];
            this.y.Value = param.input_pos[1];

            this.motor.SelectedIndex = param.motor;

            this.max.Text = CleanConfigNumber(param.max);

            this.volume_width.Text = CleanConfigNumber(param.volume_width);
            this.volume_depth.Text = CleanConfigNumber(param.volume_depth);

            this.thrust_acceleration.Text = CleanConfigNumber(param.thrusting_acceleration);
            this.thrust_speed_scale.Text = CleanConfigNumber(param.thrusting_speed_scale);
            this.thrust_depth_scale.Text = CleanConfigNumber(param.thrusting_depth_scale);

            this.rub_acceleration.Text = CleanConfigNumber(param.rubbing_acceleration);
            this.rub_scale.Text = CleanConfigNumber(param.rubbing_scale);

            typeSelector.SelectedIndex =  (int)param.type;

            blockApply = false;
        }

        private string CleanConfigNumber(float f)
        {
            string s = "" + f;
            s = s.Replace(",", ".");
            if (s == "")
                s = "1.0";
            return s;
        }

        private void KeyPressIsNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void KeyPressIsInt(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
        
        private void KeyPressIsFloatBetween0And1(object sender, KeyPressEventArgs e)
        {
            if (sender.GetType() != typeof(TextBox))
                return;

            KeyPressIsNumber(sender, e);
            if (e.Handled) return;

            TextBox box = (TextBox)sender;
            float f;
            bool isNotFloat = float.TryParse(box.Text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out f) == false || f < 0 || f > 1;
            //Console.WriteLine(f+","+isNotFloat + "," + box.Text);
            if (isNotFloat && box.Text != "")
            {
                box.Text = "1.0";
                e.Handled = true;
            }
        }

        private void typeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnagleSelectedGroup();
            Apply_Type(null, EventArgs.Empty);
        }

        private void EnagleSelectedGroup()
        {
            CalulcationType selected = (CalulcationType)typeSelector.SelectedIndex;
            groupVolume.Visible = selected == CalulcationType.VOLUME;
            groupThrusting.Visible = selected == CalulcationType.THRUSTING;
            groupRubbing.Visible = selected == CalulcationType.RUBBING;
        }

        private void rem_button_Click(object sender, EventArgs e)
        {
            //remove from config
            Device device = Config.Singleton.devices.FirstOrDefault(d => d.device_name == deviceUI.Name);
            List<DeviceParams> dparams = new List<DeviceParams>(device.device_params);
            dparams.Remove(param);
            device.device_params = dparams.ToArray();
            Config.Singleton.Save();
            //remove from ui
            deviceUI.paramsList.Controls.Remove(this);
        }
    }
}
