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
        public DeviceUI ui;
        public ToyAPI toyAPI;
        public string name;
        public string vrcToys_id;
        public int motorCount;

        public void Vibrate(double[] strength)
        {
            toyAPI.Vibrate(this, strength);
        }

        public void Test()
        {
            Vibrate(new double[] { 1, 0 });
            System.Threading.Thread.Sleep(2000);
            Vibrate(new double[] { 0, 1 });
            System.Threading.Thread.Sleep(2000);
            Vibrate(new double[] { 0, 0 });
        }

        public void UpdateBatterUI(int level)
        {
            ui.battery_bar.Invoke((Action)delegate ()
            {
                var size = ui.battery_bar.Size;
                size.Width = (int)((level / 100.0f) * ui.battery_bar.Parent.Size.Width);
                ui.battery_bar.Size = size;
                if (level > 50) ui.battery_bar.BackColor = Color.LimeGreen;
                else if (level > 20) ui.battery_bar.BackColor = Color.Orange;
                else ui.battery_bar.BackColor = Color.Red;
            });
        }
    }
}
