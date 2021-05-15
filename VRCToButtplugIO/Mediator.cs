using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSNotifications;
using XSNotifications.Enum;

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

        public static XSNotifier xSNotifier { get; private set; } = new XSNotifier();
        public static string appName { get; private set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static void SendXSNotification(string title, string content)
        {
            xSNotifier.SendNotification(new XSNotification() { Title = title, Content = content, SourceApp = appName, Timeout = 7  });
        }

        public static bool RemoveToy(Toy toy)
        {
            if (activeToys.ContainsKey(toy.id))
            {
                activeToys.Remove(toy.id);
                Program.DebugToFile($"[Mediator] Remove Toy {toy.id}");
                ui.Invoke((Action)delegate () { ui.deviceList.Controls.Remove(toy.GetDeviceUI()); });
                return true;
            }
            else
            {
                Program.DebugToFile($"[Mediator] Tried to remove Toy {toy.id} that is not dictionery.");
                return false;
            }
        }

        public static bool AddToy(Toy toy)
        {
            if (activeToys.ContainsKey(toy.id))
            {
                Program.DebugToFile($"[Mediator] Tried to add Toy {toy.id} but key already exisits.");
                return false;
            }
            else
            {
                activeToys.Add(toy.id, toy);
                Program.DebugToFile($"[Mediator] Add Toy {toy.id}");
                ui.Invoke((Action)delegate (){ toy.CreateUI(); });
                return true;
            }
        }

        public static void ToyDisconnected(string id)
        {
            if (activeToys.ContainsKey(id) == false) return;
            Toy toy = Mediator.activeToys[id];
            if(RemoveToy(toy)) SendXSNotification("Toy Disconnected", toy.name);
        }

        public static void ToyConnected(Toy toy)
        {
            if (toy == null) return;
            if(AddToy(toy)) SendXSNotification("Toy Connected", toy.name);
        }
    }
}
