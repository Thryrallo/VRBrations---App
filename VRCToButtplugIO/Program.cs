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
            if(args.Length > 0 && args[0] != null && args[0].Length == 64)
            {
                KeyManager.LoadKey(args[0]);
            }

            while (KeyManager.LoadKey() == false || await KeyManager.VerifyKeyAsync() == KeyStatus.INVALID)
            {
                Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                string input = Interaction.InputBox("Please input your 'VRC Toy Controller' Key", "Key", "", rect.Width / 2 - 200, rect.Height / 2 - 200);
                input = input.Trim();
                if (input != null && input.Length == 64)
                    KeyManager.LoadKey(input);
                if(input == null || input.Length == 0)
                {
                    DebugToFile("[Key] no key was entered, or cancel was pressed.");
                    return;
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
        static void CreateLogicThread()             
        {
            Thread thread = new Thread(Logic);
            thread.IsBackground = true;
            thread.Start();

            Thread slowUpdate = new Thread(SlowUpdate);
            slowUpdate.Name = "SlowUpdate";
            slowUpdate.Start();
        }

        static long lastUpdate = 0;
        static float lastVerifiedKey = 0;

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
            //GameWindowReader gameWindowReader = new GameWindowReader();
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
                /*if (gameWindowReader.UpdateCapture())
                {
                    TestPixels(gameWindowReader);
                }*/
                GameWindowReader.Singleton.SaveCurrentCapture("test");
                Console.WriteLine("Saved");
                Thread.Sleep(Config.SLOW_UPDATE_RATE);
            }
        }

        private static void TestPixels(GameWindowReader gameWindowReader)
        {
            if(gameWindowReader.Capture != null)
            {
                Console.WriteLine(gameWindowReader.GetBounds());
                //float f = gameWindowReader.GetFloat(1, 0 , true);
                int i = gameWindowReader.GetInt(1, 0 , true);
                /*Pixel pixel0 = gameWindowReader.GetPixel(gameWindowReader.Capture.Width - 1, 0);
                Color pixel1 = gameWindowReader.Capture.GetPixel(gameWindowReader.Capture.Width - 1, 0);
                Pixel pixel2 = new Pixel(pixel1.R, pixel1.G, pixel1.B);
                Pixel pixel3 = gameWindowReader.GetPixel(0, 3);
                Color pixel4 = gameWindowReader.Capture.GetPixel(0, 3);
                Console.WriteLine("Pixel 0: "+ pixel0);
                Console.WriteLine("Pixel 1: "+pixel1);
                Console.WriteLine("Pixel 2: "+pixel2);
                Console.WriteLine("Pixel 3: "+pixel3);
                Console.WriteLine("Pixel 4: "+ pixel4);*/
                //Console.WriteLine("Float: "+ f);
                Console.WriteLine("Pixeint: "+ i);
                Console.WriteLine();
                gameWindowReader.SaveCurrentCapture("pixelTesting");
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

        static bool UpdateBehaviourPixelData(BehaviourData behaviourData)
        {
            int baseX = behaviourData.input_pos[0] * Config.SENSOR_WIDTH;
            int baseY = behaviourData.input_pos[1] * Config.SENSOR_HEIGHT;
            GameWindowReader.Singleton.CailbrateColors(baseX + Config.COORDS_REFERENCE_COLORS.x, baseY + Config.COORDS_REFERENCE_COLORS.y);
            bool exisits = GameWindowReader.Singleton.GetShort(baseX + 0, baseY + 0) == Config.CHECK_VALUE_SHORT;

            if (exisits)
            {
                behaviourData.GetRuntimeData().depth = GameWindowReader.Singleton.GetFloat(baseX + Config.COORDS_SENSOR_DATA_DEPTH.x, baseY + Config.COORDS_SENSOR_DATA_DEPTH.y);
                behaviourData.GetRuntimeData().width = GameWindowReader.Singleton.GetFloat(baseX + Config.COORDS_SENSOR_DATA_WIDTH.x, baseY + Config.COORDS_SENSOR_DATA_WIDTH.y);
                behaviourData.GetRuntimeData().avgX = GameWindowReader.Singleton.GetFloat(baseX + Config.COORDS_SENSOR_DATA_X.x, baseY + Config.COORDS_SENSOR_DATA_X.y);
                behaviourData.GetRuntimeData().avgY = GameWindowReader.Singleton.GetFloat(baseX + Config.COORDS_SENSOR_DATA_Y.x, baseY + Config.COORDS_SENSOR_DATA_Y.y);
            }

            return exisits;
        }

        static bool audioLinkFound;
        static float[] audioLinkData = new float[] { 0, 0, 0, 0 };

        static bool LoadGlobalData()
        {
            int resolutionWriterOffset = (int)(Config.RESOLUTION_SCREEN_WIDTH / 100.0f * GameWindowReader.Singleton.Capture.Width + 1);
            GameWindowReader.Singleton.CailbrateColors(resolutionWriterOffset + Config.COORDS_REFERENCE_COLORS.x, Config.COORDS_REFERENCE_COLORS.y, true);
            if (GameWindowReader.Singleton.GetShort(resolutionWriterOffset + Config.COORDS_CHECK_VALUE.x, Config.COORDS_CHECK_VALUE.y, true) == Config.CHECK_VALUE_SHORT)
            {
                SetUIMessage(Mediator.ui.messageLabel2, "", Color.Green);

                audioLinkFound = GameWindowReader.Singleton.GetBool(Config.COORDS_AUDIOLINK_EXISITS.x, Config.COORDS_AUDIOLINK_EXISITS.y, fromRight: true);
                audioLinkData[0] = GameWindowReader.Singleton.GetFloat(Config.COORDS_AUDIOLINK.x + 1, Config.COORDS_AUDIOLINK.y, true);
                audioLinkData[1] = GameWindowReader.Singleton.GetFloat(Config.COORDS_AUDIOLINK.x + 2, Config.COORDS_AUDIOLINK.y, true);
                audioLinkData[2] = GameWindowReader.Singleton.GetFloat(Config.COORDS_AUDIOLINK.x + 3, Config.COORDS_AUDIOLINK.y, true);
                audioLinkData[3] = GameWindowReader.Singleton.GetFloat(Config.COORDS_AUDIOLINK.x + 4, Config.COORDS_AUDIOLINK.y, true);
                return true;
            }
            else
            {
                SetUIMessage(Mediator.ui.messageLabel2, "VRbrations indicator not found", Color.Orange);
                audioLinkData[0] = 0;
                audioLinkData[1] = 0;
                audioLinkData[2] = 0;
                audioLinkData[3] = 0;
                audioLinkFound = false;
                return false;
            }
        }

        static void CheckCaptureForInput()
        {
            LoadGlobalData();

            foreach (Toy t in Mediator.activeToys.Values)
            {
                for (int i = 0; i < t.featureStrengths.Length; i++) t.featureStrengths[i] = 0;

                foreach (BehaviourData behaviour in t.GetBehaviours())
                {
                    bool behaviourFound;
                    if (behaviour.type == CalulcationType.AudioLink) behaviourFound = audioLinkFound;
                    else behaviourFound = UpdateBehaviourPixelData(behaviour);

                    //Get The pixel data and set label accordingly
                    if (behaviourFound)
                    {
                        if (behaviour.GetRuntimeData().wasFound == false && t.GetDeviceUI() != null)
                        {
                            SetUIMessage(t.GetDeviceUI().GetBehaviourUI(behaviour).label_pixel_found, "found", Color.Green);
                            behaviour.GetRuntimeData().wasFound = true;
                        }
                    }
                    else
                    {
                        if (behaviour.GetRuntimeData().wasFound && t.GetDeviceUI() != null)
                        {
                            SetUIMessage(t.GetDeviceUI().GetBehaviourUI(behaviour).label_pixel_found, "not found", Color.Red);
                            behaviour.GetRuntimeData().wasFound = false;
                        }
                        continue;
                    }
                    t.featureStrengths[behaviour.feature] += behaviour.CalculateStrength(audioLinkData);
                    behaviour.GetBehaviourUI(t).UpdateStrengthIndicatorValue();
                }

                for(int i = 0; i < t.totalFeatureCount; i++)
                {
                    t.ExecuteFeatures(t.featureStrengths);
                }
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

        public static void DebugToFile2(string s, int type = 0)
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
