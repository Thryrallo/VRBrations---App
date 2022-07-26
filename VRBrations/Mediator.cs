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

        public static Dictionary<string, (int, int)> activeSensorPositions = new Dictionary<string, (int, int)>();
        public static Dictionary<(int, int), string> activeSensorPositionNameMap = new Dictionary<(int, int), string>();

        private static MainUI p_ui;
        private static uint _displayDPIX;
        private static uint _displayDPIY;
        public static MainUI ui
        {
            get
            {
                if (p_ui == null)
                {
                    p_ui = new MainUI();
                    System.Drawing.Graphics graphics = p_ui.CreateGraphics();
                    _displayDPIX = (uint)graphics.DpiX;
                    _displayDPIY = (uint)graphics.DpiY;
                    graphics.Dispose();
                }
                return p_ui;
            }
        }

        public static bool IsUICreated()
        {
            return p_ui != null && p_ui.Created;
        }

        public static uint DisplayDpiX
        {
            get { return _displayDPIX;  }
        }

        public static uint DisplayDpiY
        {
            get { return _displayDPIY;  }
        }

        public static bool SetSensorActive(string name, int x, int y)
        {
            if (activeSensorPositions.ContainsKey(name) == false && activeSensorPositionNameMap.ContainsKey((x,y)) == false)
            {
                activeSensorPositions.Add(name, (x, y));
                activeSensorPositionNameMap.Add((x, y), name);

                foreach (Toy t in Mediator.activeToys.Values)
                {
                    bool added = false;
                    foreach (BehaviourData behaviour in t.GetBehaviours())
                    {
                        if (behaviour.IsActive)
                        {
                            behaviour.GetBehaviourUI(t).AddSensor(name);
                        }
                        if (behaviour.name == name)
                        {
                            behaviour.SetActive(t);
                            added = true;
                        }
                        
                    }
                    //create new behaviour
                    if (!added && t.GetDeviceData().removedSensors.Contains(name) == false)
                    {
                        BehaviourData b = t.AddBehaviour(name);
                    }
                }

                return true;
            }
            return false;
        }

        public static bool SetSensorInactive(string name)
        {
            if (activeSensorPositions.ContainsKey(name))
            {
                activeSensorPositionNameMap.Remove(activeSensorPositions[name]);
                activeSensorPositions.Remove(name);

                foreach (Toy t in Mediator.activeToys.Values)
                {
                    foreach (BehaviourData behaviour in t.GetBehaviours())
                    {
                        if (behaviour.IsActive)
                        {
                            behaviour.GetBehaviourUI(t).RemoveSensor(name);

                            if (behaviour.name == name)
                            {
                                behaviour.SetInactive(t);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetSensorInactive(int x, int y)
        {
            if (activeSensorPositionNameMap.ContainsKey((x,y)))
            {
                return SetSensorInactive(activeSensorPositionNameMap[(x,y)]);
            }
            return false;
        }

        public static (int,int) GetSensorPosition(string name)
        {
            if (activeSensorPositions.ContainsKey(name) == false) return (0, 0);
            return activeSensorPositions[name];
        }

        public static bool IsSensorActive(string name)
        {
            return activeSensorPositions.ContainsKey(name);
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
                ActivateBehavioursForActiveSensors();
                return true;
            }
        }

        /** <summary> Enable ui and function for behaviours of already found sensors when toy is added </summary> **/
        public static void ActivateBehavioursForActiveSensors()
        {
            foreach (Toy t in Mediator.activeToys.Values)
            {
                bool added = false;
                foreach (BehaviourData behaviour in t.GetBehaviours())
                {
                    if (activeSensorPositions.ContainsKey(behaviour.name))
                    {
                        behaviour.SetActive(t);
                    }
                }
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
