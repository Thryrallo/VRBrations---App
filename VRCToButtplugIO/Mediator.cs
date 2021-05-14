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

        public static void RemoveToy(Toy toy)
        {
            Program.DebugToFile("[Mediator] Remove Toy " + toy.id);
            Mediator.activeToys.Remove(toy.id);
            Mediator.ui.Invoke((Action)delegate ()
            {
                Mediator.ui.deviceList.Controls.Remove(toy.GetDeviceUI());
            });
        }

        public static void AddToy(Toy toy)
        {
            Program.DebugToFile("[Mediator] Add Toy " + toy.id);
            Mediator.activeToys.Add(toy.id, toy);
            Mediator.ui.Invoke((Action)delegate ()
            {
                toy.CreateUI();
            });
        }

        public static void ToyDisconnected(string id)
        {
            Toy toy = Mediator.activeToys[id];
            SendXSNotification("Toy Disconnected", toy.name);
            RemoveToy(toy);
        }

        public static void ToyConnected(Toy toy)
        {
            SendXSNotification("Toy Connected", toy.name);
            AddToy(toy);
        }
    }
}
