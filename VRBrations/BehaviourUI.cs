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
    public partial class BehaviourUI : UserControl
    {
        private Toy toy;

        private bool blockApply = false;

        private BehaviourData behaviourData;

        private bool isShown = false;

        public BehaviourUI(Toy toy, BehaviourData behaviourData, string[] featureNames)
        {
            InitializeComponent();

            this.toy = toy;
            this.behaviourData = behaviourData;

            foreach (object o in Enum.GetValues(typeof(AudioLinkChannel)))
                this.audioLinkChannel.Items.Add(o);

            motor.Items.AddRange(featureNames);

            Populate(behaviourData);
            ShowUI();
        }

        public BehaviourData GetBehaviourData()
        {
            return behaviourData;
        }

        public void ShowUI()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((Action)delegate ()
                {
                    CopySensorNamesFromMediator();
                    Show();
                    isShown = true;
                });
            }
            else
            {
                CopySensorNamesFromMediator();
                Show();
                isShown = true;
            }
        }

        private void CopySensorNamesFromMediator()
        {
            blockApply = true;
            this.sensorSelection.Items.Clear();
            this.sensorSelection.Items.AddRange(Mediator.activeSensorPositions.Keys.ToArray());
            if (behaviourData.name == null) this.sensorSelection.SelectedIndex = -1;
            else this.sensorSelection.SelectedIndex = this.sensorSelection.Items.IndexOf(behaviourData.name);
            blockApply = false;
        }

        public void HideUI()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((Action)delegate () { 
                    Hide();
                    isShown = false;
                });
            }
            else
            {
                Hide();
                isShown = false;
            }
            
        }

        public void AddSensor(string sname)
        {
            if (this.IsHandleCreated) this.Invoke((Action)delegate () { this.sensorSelection.Items.Add(sname); });
            else this.sensorSelection.Items.Add(sname);
        }

        public void RemoveSensor(string sname)
        {
            if (this.IsHandleCreated) this.Invoke((Action)delegate () { this.sensorSelection.Items.Remove(sname); });
            else this.sensorSelection.Items.Remove(sname);
        }

        private void Apply_All()
        {
            if (!isShown) return;
            Apply_SensorName(null, new KeyEventArgs(Keys.None));
            Apply_Max(null, new KeyEventArgs(Keys.None));
            Apply_Type(null, EventArgs.Empty);
            Apply_Feature(null, EventArgs.Empty);
            Apply_Volume(null, new KeyEventArgs(Keys.None));
            Apply_Thrusting(null, new KeyEventArgs(Keys.None));
            Apply_Rubbing(null, new KeyEventArgs(Keys.None));
            Apply_AudioLink(null, new KeyEventArgs(Keys.None));
        }

        private void Apply_SensorName(object sender, EventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetSensorName(sensorSelection.SelectedItem as string);
            UpdateTypeSelector();
        }

        

        private void Apply_Max(object sender, KeyEventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetMax(ParseFloat(this.max.Text));
        }

        private void Apply_Type(object sender, EventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetCalculationType((CalculationType)this.typeSelector.SelectedIndex);
        }

        private void Apply_Feature(object sender, EventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetFeature(this.motor.SelectedIndex);
        }

        private void Apply_Volume(object sender, KeyEventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetVolume(ParseFloat(this.volume_width), ParseFloat(this.volume_depth));
        }

        private void Apply_Thrusting(object sender, KeyEventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetThrusting(Math.Min(1, ParseFloat(this.thrust_acceleration)), ParseFloat(this.thrust_speed_scale), ParseFloat(this.thrust_depth_scale));
        }

        private void Apply_Rubbing(object sender, KeyEventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetRubbing(Math.Min(1, ParseFloat(this.rub_acceleration)), ParseFloat(this.rub_scale));
        }

        private void Apply_AudioLink(object sender, EventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetAudioLink((AudioLinkChannel)audioLinkChannel.SelectedIndex, ParseFloat(this.audioLinkStrength));
        }

        private void Apply_AudioLink(object sender, KeyEventArgs e)
        {
            if (!isShown) return;
            if (blockApply) return;
            behaviourData.SetAudioLink((AudioLinkChannel)audioLinkChannel.SelectedIndex, ParseFloat(this.audioLinkStrength));
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

        private void Populate(BehaviourData param)
        {
            blockApply = true;

            this.motor.SelectedIndex = param.feature;

            this.max.Text = CleanConfigNumber(param.max);

            this.volume_width.Text = CleanConfigNumber(param.volume_width);
            this.volume_depth.Text = CleanConfigNumber(param.volume_depth);

            this.thrust_acceleration.Text = CleanConfigNumber(param.thrusting_acceleration);
            this.thrust_speed_scale.Text = CleanConfigNumber(param.thrusting_speed_scale);
            this.thrust_depth_scale.Text = CleanConfigNumber(param.thrusting_depth_scale);

            this.rub_acceleration.Text = CleanConfigNumber(param.rubbing_acceleration);
            this.rub_scale.Text = CleanConfigNumber(param.rubbing_scale);

            this.audioLinkStrength.Text = CleanConfigNumber(param.audioLink_scale);
            this.audioLinkChannel.SelectedIndex = (int)param.audioLink_channel;

            UpdateTypeSelector();
            EnagleSelectedGroup();

            blockApply = false;
        }

        private void UpdateTypeSelector()
        {
            blockApply = true;
            if (CalculationTypeData.Get(behaviourData.type).showInDropdown)
            {
                CopyTypesFromStruct();
                typeSelector.Enabled = true;
            }
            else
            {
                typeSelector.Items.Clear();
                typeSelector.Items.Add(CalculationTypeData.Get(behaviourData.type).displayName);
                typeSelector.SelectedIndex = 0;
                typeSelector.Enabled = false;
            }
            blockApply = false;
        }

        private void CopyTypesFromStruct()
        {
            typeSelector.Items.Clear();
            foreach (CalculationTypeData t in CalculationTypeData.COLLECTION)
                if (t.showInDropdown)
                    this.typeSelector.Items.Add(t.displayName);
            typeSelector.SelectedIndex = CalculationTypeData.Get(behaviourData.type).drowDownIndex;
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
            Apply_Type(null, EventArgs.Empty);
            EnagleSelectedGroup();
        }

        private void EnagleSelectedGroup()
        {
            groupVolume.Visible = behaviourData.type == CalculationType.VOLUME;
            groupThrusting.Visible = behaviourData.type == CalculationType.THRUSTING;
            groupRubbing.Visible = behaviourData.type == CalculationType.RUBBING;
            audioLinkSettings.Visible = behaviourData.type == CalculationType.AUDIOLINK;
        }

        public void UpdateStrengthIndicatorValue()
        {
            if (strengthIndicator.IsHandleCreated == false) return;
            strengthIndicator.Invoke((Action)delegate
            {
               Size size = strengthIndicator.Parent.Size;
               size.Width = (int)(size.Width * behaviourData.GetCurrentStrength());
               strengthIndicator.Size = size;
           });
            
        }

        public void UpdateStrengthIndicatorValue(float value)
        {
            if (strengthIndicator.IsHandleCreated == false) return;
            strengthIndicator.Invoke((Action)delegate
            {
                Size size = strengthIndicator.Parent.Size;
                size.Width = (int)(size.Width * value);
                strengthIndicator.Size = size;
            });

        }

        private void rem_button_Click(object sender, EventArgs e)
        {
            toy.RemoveBehaviour(this);
        }
    }
}
