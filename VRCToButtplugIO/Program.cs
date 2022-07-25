using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCToyController
{
    static class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //load key from args
            if (args.Length > 0 && args[0] != null && args[0].Length == 64)
            {
                KeyManager.LoadKey(args[0]);
            }

            if (await KeyManager.VerifyWorld() != WorldStatus.ACTIVE)
            {

                while (KeyManager.LoadKey() == false || await KeyManager.VerifyKeyAsync() == KeyStatus.INVALID)
                {
                    Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    string input = Interaction.InputBox("Please input your 'VRbrations' Key", "Key", "", rect.Width / 2 - 200, rect.Height / 2 - 200);
                    input = input.Trim();
                    if (input != null && input.Length == 64)
                        KeyManager.LoadKey(input);
                    if (input == null || input.Length == 0)
                    {
                        if (await KeyManager.VerifyWorld() == WorldStatus.ACTIVE)
                        {
                            break;
                        }
                        else
                        {
                            DebugToFile("[Key] no key was entered, or cancel was pressed. => Status: "+ KeyManager.status);
                            return;
                        }
                    }
                }

            }
            DebugToFile("[Key] Key has been verified.");

            DebugToFile("[API] Starting Inizilization.");
            Init();
            DebugToFile("[API] Starting Logic Thread.");
            CreateLogicThread();
            DebugToFile("[API] Starting Application.");

            DebugToFile("[API] Starting ButtplugIO.");
            InitButtplugIO();
            DebugToFile("[API] Starting LovenseConnect.");
            //Mediator.toyAPIs.Add(await LovenseConnectAPI.GetClient());
            InitLovenseConnect();
            DebugToFile("[API] All APIs have been started.");

            Application.Run(Mediator.ui);
        }

        static  void InitButtplugIO()
        {
            var thread = new Thread(async delegate() { Mediator.toyAPIs.Add(await ButtplugIOAPI.GetClient()); });
            thread.IsBackground = true;
            thread.Start();
        }

        static void InitLovenseConnect()
        {
            var thread = new Thread(async delegate () { Mediator.toyAPIs.Add(await LovenseConnectAPI.GetClient()); });
            thread.IsBackground = true;
            thread.Start();
        }

        static async void Init()
        {
            config = Config.Singleton;

            string justHereToForceUIInit = Mediator.ui.Name;
        }

        static Config config;

        public static bool isRunning = true;
        public static Thread trd_slowUpdate;
        static void CreateLogicThread()             
        {
            Thread thread = new Thread(Logic);
            thread.Name = "Logic";
            thread.IsBackground = true;
            thread.Start();

            trd_slowUpdate = new Thread(SlowUpdate);
            trd_slowUpdate.Name = "SlowUpdate";
            trd_slowUpdate.Start();
        }

        static long lastUpdate = 0;
        static long lastVerifiedKey = 0;

        static long vrchatNotInFocusStart = 0;
        static bool wasVRChatInFocus = true;
        static void Logic()
        {
            while (isRunning)
            {
                if (KeyManager.VerifyKey())
                {
                    string activeWindow = GetActiveWindow();
                    bool isAppInFocus = Config.WINDOW_NAMES.Contains(activeWindow);
                    if (isAppInFocus)
                    {
                        try 
                        {
                            if (GameWindowReader.Singleton.UpdateCapture())
                            {
                                SearchForSensors();
                                CheckCaptureForInput();
                            }
                        }
                        catch (Exception e)
                        {
                            DebugToFile(e.ToString(),2);
                            SetUIMessage(Mediator.ui.label_vrc_focus, "Error. Check log.", Color.Orange);
                        }
                    }
                    else
                    {
                        //if game screenshot was taken before, with vrbrations, use this to search for more sensors
                        if (vrbrationsFound && GameWindowReader.Singleton.HasCapture())
                        {
                            SearchForSensors();
                        }
                        if (wasVRChatInFocus)
                        {
                            TurnAllToysOff();
                            vrchatNotInFocusStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        }
                        SetUIMessage(Mediator.ui.label_vrc_focus, $"VRC not in focus\nCurrent window:\n{activeWindow}", Color.Red);
                    }
                    wasVRChatInFocus = isAppInFocus;
                    lastVerifiedKey = lastUpdate;
                }
                else if (lastUpdate != 0 && (lastUpdate - lastVerifiedKey) > 30000)
                {
                    DebugToFile("[KEY] Key has not been verified recently. Closing Application.");
                    Mediator.ui.Invoke((Action)delegate () { Mediator.ui.Close(); });
                    isRunning = false;
                }
                int timeout = (int)(config.update_rate - (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdate));
                if(timeout>0)
                    Thread.Sleep(timeout);
                lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        //Also runs when key not verified
        static bool hasSentNotInFocusNotification = false;
        private static void SlowUpdate()
        {
            while (isRunning)
            {
                foreach (ToyAPI api in Mediator.toyAPIs)
                {
                    api.SlowUpdate();
                }
                foreach (Toy toy in Mediator.activeToys.Values)
                {
                    toy.UpdateBatteryIndicator();
                }
                //Not in focus notification
                if (wasVRChatInFocus == false && hasSentNotInFocusNotification == false && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - vrchatNotInFocusStart > 20000)
                {
                    //Mediator.SendXSNotification("VRChat is not in focus.", "Make sure the vrchat window is the active window.");
                    hasSentNotInFocusNotification = true;
                }
                else if (wasVRChatInFocus)
                {
                    hasSentNotInFocusNotification = false;
                }
                for(int i=0;i<Config.SLOW_UPDATE_RATE;i += 500)
                {
                    if (isRunning == false) continue;
                    Thread.Sleep(500);
                }
            }
        }

        private static Dictionary<Label, long> lastXSMessageTime = new Dictionary<Label, long>();
        public static void SetUIMessage(Label l, string message, Color color, string xsTitle = null, string xsMessage = null, float xsCooldownTime = 0, bool xsCooldownBeforeFirstPopup = false)
        {
            if (l == null) return;
            //try chach, because this throws sometimes an error when application is closing. should probabnly be fixed correctly at some point
            try
            {
                if (l.ForeColor != color || l.Text != message)
                {
                    l.Invoke((Action)delegate ()
                    {
                        l.Text = message;
                        l.ForeColor = color;
                    });
                    if(xsTitle != null)
                    {
                        long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        if(lastXSMessageTime.ContainsKey(l))
                        {
                            if(time - lastXSMessageTime[l] > xsCooldownTime)
                            {
                                Mediator.SendXSNotification(xsTitle, xsMessage);
                                lastXSMessageTime[l] = time;
                            }
                        }
                        else
                        {
                            if(xsCooldownBeforeFirstPopup == false) Mediator.SendXSNotification(xsTitle, xsMessage);
                            lastXSMessageTime[l] = time;
                        }
                    }
                }
            }
            catch (Exception e) { };
        }

        #region ToysLogic
        private static void TurnAllToysOff()
        {
            foreach(Toy t in Mediator.activeToys.Values)
            {
                t.TurnOff();
            }
        }

        static bool UpdateBehaviourPixelData(BehaviourData behaviourData, Toy toy)
        {
            (int,int) coords = Mediator.GetSensorPosition(behaviourData.name);
            bool exisits = GameWindowReader.Singleton.GetShort(coords, Config.COORDS_CHECK_VALUE) == Config.CHECK_VALUE_SHORT;
            if (exisits)
            {
                behaviourData.GetRuntimeData().depth = GameWindowReader.Singleton.GetFloat(coords ,Config.COORDS_SENSOR_DATA_DEPTH);
                behaviourData.GetRuntimeData().width = GameWindowReader.Singleton.GetFloat(coords, Config.COORDS_SENSOR_DATA_WIDTH);
                behaviourData.GetRuntimeData().avgX = GameWindowReader.Singleton.GetFloat(coords, Config.COORDS_SENSOR_DATA_X);
                behaviourData.GetRuntimeData().avgY = GameWindowReader.Singleton.GetFloat(coords, Config.COORDS_SENSOR_DATA_Y);
            }

            return exisits;
        }

        static bool vrbrationsFound = false;
        static bool audioLinkFound;
        static float[] audioLinkData = new float[] { 0, 0, 0, 0 };
        static float[] overrideStrengthControls = new float[] { 0, 0 };

        static bool LoadGlobalData()
        {
            vrbrationsFound = GameWindowReader.Singleton.GetShort((0,0), Config.COORDS_CHECK_VALUE) == Config.CHECK_VALUE_SHORT;
            if (vrbrationsFound)
            {
                SetUIMessage(Mediator.ui.messageLabel2, "VRbrations active", Color.Green);

                overrideStrengthControls[0] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_WORLD_STRENGTH_1);
                overrideStrengthControls[1] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_WORLD_STRENGTH_2);

                audioLinkFound = GameWindowReader.Singleton.GetBool((0, 0), Config.COORDS_AUDIOLINK_EXISITS);
                if (audioLinkFound)
                {
                    audioLinkData[0] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_AUDIOLINK.Add(0, 0));
                    audioLinkData[1] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_AUDIOLINK.Add(1, 0));
                    audioLinkData[2] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_AUDIOLINK.Add(2, 0));
                    audioLinkData[3] = GameWindowReader.Singleton.GetFloat((0, 0), Config.COORDS_AUDIOLINK.Add(3, 0));
                    Mediator.SetSensorActive(Config.SENSORNAME_AUDIOLINK, -1, -1);
                }
                return true;
            }
            else
            {
                SetUIMessage(Mediator.ui.messageLabel2, "You are not in a VRbrations enabled World or Avatar.", Color.Orange);
                audioLinkFound = false;
            }

            if (audioLinkFound == false)
            {
                Mediator.SetSensorInactive(Config.SENSORNAME_AUDIOLINK);
                audioLinkData[0] = 0;
                audioLinkData[1] = 0;
                audioLinkData[2] = 0;
                audioLinkData[3] = 0;
            }

            return vrbrationsFound;
        }

        static int searchX = 1;
        static int searchY = 0;
        static void SearchForSensors()
        {
            (int,int) sensorCoordinates = (searchX, searchY);
            bool found = GameWindowReader.Singleton.GetShort(sensorCoordinates, Config.COORDS_CHECK_VALUE) == Config.CHECK_VALUE_SHORT;
            //add if found
            if (found)
            {
                string name = GameWindowReader.Singleton.GetString(sensorCoordinates, Config.COORDS_SENSOR_NAME, Config.SENSOR_PIXELS_X, 2);
                Mediator.SetSensorActive(name, searchX, searchY);
            }
            else
            {
                Mediator.SetSensorInactive(searchX, searchY);
            }
            //Increment search
            searchX++;
            if(searchX >= Config.SENSOR_COUNT_X)
            {
                searchX = 1;
                searchY++;
                if(searchY >= Config.SENSOR_COUNT_Y)
                {
                    searchY = 0;
                }
            }
        }

        static void CheckCaptureForInput()
        {
            LoadGlobalData();

            if (vrbrationsFound)
            {
                foreach (Toy t in Mediator.activeToys.Values)
                {
                    //Reset feature strengths
                    for (int i = 0; i < t.featureStrengths.Length; i++)
                    {
                        if (i < overrideStrengthControls.Length)
                            t.featureStrengths[i] = overrideStrengthControls[i];
                        else
                            t.featureStrengths[i] = 0;
                    }

                    //add up behaviour values
                    foreach (BehaviourData behaviour in t.GetBehaviours())
                    {
                        if (behaviour.IsActive == false) continue;

                        bool behaviourFound;
                        if (behaviour.type == CalculationType.AUDIOLINK) behaviourFound = audioLinkFound;
                        else behaviourFound = UpdateBehaviourPixelData(behaviour, t);

                        if (!behaviourFound) continue;
                        t.featureStrengths[behaviour.feature] += behaviour.CalculateStrength(audioLinkData);
                        behaviour.GetBehaviourUI(t).UpdateStrengthIndicatorValue();
                    }

                    //Set the strengths on the device
                    for (int i = 0; i < t.totalFeatureCount; i++)
                    {
                        t.ExecuteFeatures(t.featureStrengths);
                    }
                }
            }
            else
            {
                TurnAllToysOff();
            }
        }
        #endregion

        #region Capturing

        private static string GetActiveWindow()
        {
            const int nChars = 256;
            IntPtr handle;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return "";
        }

        #endregion

        public static void DebugToFileOld(string s, int type = 0)
        {
            if (type == 1) s += "[Warning]";
            else if (type == 2) s += "[Error]";
            Console.WriteLine(s);
            /*using (StreamWriter sw = File.AppendText("./debug"))
            {
                sw.WriteLine(s);
            }*/
        }

        static ReaderWriterLock locker = new ReaderWriterLock();
        public static void DebugToFile(string s, int type = 0)
        {
            if (type == 1) s += "[Warning]";
            else if (type == 2) s += "[Error]";
            try
            {
                locker.AcquireWriterLock(int.MaxValue); //You might wanna change timeout value 
                Console.WriteLine(s);
                System.IO.File.AppendAllLines("./debug", new[] { s });
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }
    }
}
