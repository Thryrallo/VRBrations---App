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
        public DeviceParamsUI(string[] motorValues)
        {
            InitializeComponent();

            foreach(object o in Enum.GetValues(typeof(CalulcationType)))
                this.typeSelector.Items.Add(o);
            typeSelector.SelectedIndex = 0;
            EnagleSelectedGroup();

            motor.Items.AddRange(motorValues);
            motor.SelectedIndex = 0;
        }

        public void Populate(DeviceParams param)
        {
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
        }

        private string CleanConfigNumber(float f)
        {
            string s = "" + f;
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
        
        private void BoxFloatBetween0And1(object sender, KeyEventArgs e)
        {
            if (sender.GetType() != typeof(TextBox))
                return;
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
            this.Parent.Controls.Remove(this);
        }
    }
}
