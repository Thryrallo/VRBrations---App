using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    class Mediator
    {

        public static List<ToyAPI> toyAPIs = new List<ToyAPI>();

        public static Dictionary<string, Toy> activeToys = new Dictionary<string, Toy>();

        private static MainUI p_ui;
        public static MainUI ui
        {
            get
            {
                if (p_ui == null)
                    p_ui = new MainUI();
                return p_ui;
            }
        }

        public static void RemoveToy(string name)
        {
            Mediator.ui.Invoke((Action)delegate ()
            {
                Mediator.activeToys.Remove(name);
                for (int i = Mediator.ui.deviceList.Controls.Count - 1; i >= 0; i--)
                {
                    if (Mediator.ui.deviceList.Controls[i].Name == name)
                        Mediator.ui.deviceList.Controls.RemoveAt(i);

                }
            });
        }

        public static void AddToy(Toy toy)
        {
            Mediator.ui.Invoke((Action)delegate ()
            {
                DeviceUI deviceControl = new DeviceUI(toy);
                deviceControl.Populate(Config.config);
                toy.ui = deviceControl;
                Mediator.activeToys.Add(toy.name, toy);

                Mediator.ui.deviceList.Controls.Add(deviceControl);

                toy.UpdateBatterUI(0);

                toy.toyAPI.UpdateBatteryIndicator(toy);
            });
        }
    }
}
