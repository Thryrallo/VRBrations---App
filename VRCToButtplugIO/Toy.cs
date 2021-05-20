using Buttplug;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public abstract class Toy
    {
        public ToyAPI toyAPI;
        public string name;
        public string id;
        public Dictionary<ToyFeatureType, int> featureCount;
        public int totalFeatureCount;

        private int batteryLevel = 100;
        private bool hasSentBatteryWarning = false;

        private DeviceData deviceData;
        private DeviceUI deviceUI;

        public double[] featureStrengths;

        public Toy()
        {
            featureCount = new Dictionary<ToyFeatureType, int>();
            foreach(ToyFeatureType f in Enum.GetValues(typeof(ToyFeatureType))){
                featureCount[f] = 0;
            }
        }

        public void AtEndOfConstructor()
        {
            featureStrengths = new double[totalFeatureCount];
            deviceData = Config.Singleton.GetDeviceData(this);
        }

        public void CreateUI()
        {
            deviceUI = new DeviceUI(this);
            Mediator.ui.deviceList.Controls.Add(deviceUI);
            toyAPI.UpdateBatteryIndicator(this);
        }

        public DeviceUI GetDeviceUI()
        {
            return deviceUI;
        }

        public DeviceData GetDeviceData()
        {
            return deviceData;
        }

        public void AddBehaviour()
        {
            deviceUI.AddBehaviourUI(deviceData.AddBehaviour(this));
        }

        public void RemoveBehaviour(BehaviourUI behaviourUI)
        {
            deviceUI.RemoveBehaviourUI(behaviourUI);
            deviceData.RemoveBehaviour(behaviourUI.GetBehaviourData());
        }

        public List<BehaviourData> GetBehaviours()
        {
            return deviceData.behaviours;
        }

        public void UpdateTotalFeatureCount()
        {
            totalFeatureCount = this.featureCount.Sum(f => f.Value);
        }

        public enum ToyFeatureType
        {
            Vibrate,Rotate,Air
        }

        public void ExecuteFeatures(double[] strength)
        {
            for (int i = 0; i < strength.Length; i++) strength[i] = Math.Min(strength[i], 1);
            if (strength.Length == this.totalFeatureCount)
            {
                //split into different featres
                IEnumerable<double> vibrate = strength.Take(featureCount[ToyFeatureType.Vibrate]).ToArray();
                IEnumerable<double> rotate = strength.Skip(featureCount[ToyFeatureType.Vibrate]).Take(featureCount[ToyFeatureType.Rotate]);
                IEnumerable<double> air = strength.Skip(featureCount[ToyFeatureType.Vibrate] + featureCount[ToyFeatureType.Rotate]).Take(featureCount[ToyFeatureType.Air]);
                Vibrate(vibrate);
                Rotate(rotate);
                Air(air);
            }
            else if (strength.Length > 0 && featureCount[ToyFeatureType.Vibrate] > 0)
            {
                Vibrate(strength.Take(1));
            }
        }

        public void TurnOff()
        {
            Vibrate(new double[] { 0 });
            Rotate(new double[] { 0 });
            Air(new double[] { 0 });
        }

        public abstract void Vibrate(IEnumerable<double> strength);
        public abstract void Rotate(IEnumerable<double> strength);
        public abstract void Air(IEnumerable<double> strength);

        public void Test()
        {
            ExecuteFeatures(new double[] { 1, 0 });
            System.Threading.Thread.Sleep(2000);
            ExecuteFeatures(new double[] { 0, 1 });
            System.Threading.Thread.Sleep(2000);
            ExecuteFeatures(new double[] { 0, 0 });
        }

        public void TestBehaviours()
        {
            foreach (BehaviourData behaviour in GetDeviceData().behaviours)
            {
                double[] strengths = new double[totalFeatureCount];
                for (int i = 0; i < strengths.Length; i++) strengths[i] = 0;
                strengths[behaviour.feature] = behaviour.max;
                //Console.WriteLine("Testing " + behaviour.feature + " at " + behaviour.max);
                ExecuteFeatures(strengths);
                System.Threading.Thread.Sleep(2000);
            }
            TurnOff();
        }

        public void UpdateBatteryIndicator()
        {
            toyAPI.UpdateBatteryIndicator(this);
        }

        public void SetBatterLevel(int level)
        {
            this.batteryLevel = level;
            UpdateBatteryUI();
            if(this.batteryLevel < 20 && !hasSentBatteryWarning)
            {
                Mediator.SendXSNotification("Toy Battery Low", "The Battery of " + this.name + " is low.");
                hasSentBatteryWarning = true;
            }
        }

        private void UpdateBatteryUI()
        {
            if (deviceUI.IsHandleCreated == false) return; //Prevents an error being thrown when closing application
            deviceUI.battery_bar.Invoke((Action)delegate ()
            {
                var size = deviceUI.battery_bar.Size;
                size.Width = (int)((this.batteryLevel / 100.0f) * deviceUI.battery_bar.Parent.Size.Width);
                deviceUI.battery_bar.Size = size;
                if (this.batteryLevel > 50) deviceUI.battery_bar.BackColor = Color.LimeGreen;
                else if (this.batteryLevel > 20) deviceUI.battery_bar.BackColor = Color.Orange;
                else deviceUI.battery_bar.BackColor = Color.Red;
            });
        }
    }
}
