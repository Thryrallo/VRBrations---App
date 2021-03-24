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

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [Flags]
        private enum DwmWindowAttribute : uint
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        }

        static float dpi = 1;
        static int titleBarHeight = 0;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


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
            config = Config.config;
            dpi = getScalingFactor();
            Console.WriteLine("DPI:" + dpi);

            string justHereToForceUIInit = Mediator.ui.Name;

            //this is correct if the title bar is default, but not with material design
            //Rectangle screenRectangle = Mediator.ui.RectangleToScreen(Mediator.ui.ClientRectangle);
            //titleBarHeight = (int)((screenRectangle.Top - Mediator.ui.Top) * dpi);

            //Console.WriteLine(screenRectangle.Top - Mediator.ui.Top);

            //dont ask why +8, idk, but tests seemed to have shown it as correct
            titleBarHeight = (int)((SystemInformation.CaptionHeight + 8) * dpi);
            DebugToFile("TitleBar Height: " + titleBarHeight);
        }

        static Config config;

        static float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

        static void CreateLogicThread()
        {
            var thread = new Thread(Logic);
            thread.IsBackground = true;
            thread.Start();
        }

        static long lastSlowUpdate = 0;
        const float slowUpdateRate = 10000;

        static long lastUpdate = 0;
        static float lastVerifiedKey = 0;
        static void Logic()
        {
            while (true)
            {
                if (KeyManager.VerifyKey())
                {
                    string activeWindow = GetActiveWindow();
                    if (activeWindow == config.window_name)
                    {
                        try
                        {
                            Rectangle windowBounds = GetWindowBounds();
                            if (windowBounds.Height > 0 && windowBounds.Width > 0)
                            {
                                Bitmap capture = Capture(windowBounds);
                                CheckCaptureForInput(capture);
                                capture.Dispose();

                                SetUIMessage(Mediator.ui.label_vrc_focus, "VRC is in focus", Color.Green);
                            }
                            else
                            {
                                SetUIMessage(Mediator.ui.label_vrc_focus, "VRC bounds wrong", Color.Red);
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
                        TurnAllToysOff();
                        SetUIMessage(Mediator.ui.label_vrc_focus, "VRC not in focus\nCurrent window:\n" + activeWindow, Color.Red);
                    }
                    //do slow update on apis
                    if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastSlowUpdate > slowUpdateRate)
                    {
                        Thread slowUpdate = new Thread(SlowUpdate);
                        slowUpdate.Name = "SlowUpdate";
                        slowUpdate.Start();
                    } 
                    
                    lastVerifiedKey = lastUpdate;
                }
                else if (lastUpdate != 0 && (lastUpdate-lastUpdate) > 10000)
                {
                    Task.Delay(1000).ContinueWith(t => { System.Windows.Forms.Application.Exit(); });
                }
                int timeout = (int)(config.update_rate - (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdate));
                if(timeout>0)
                    Thread.Sleep(timeout);
                lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        private static void SlowUpdate()
        {
            foreach (ToyAPI api in Mediator.toyAPIs)
            {
                api.SlowUpdate();
            }
            foreach (Toy toy in Mediator.activeToys.Values)
            {
                toy.toyAPI.UpdateBatteryIndicator(toy);
            }
            lastSlowUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private static void SetUIMessage(Label l, string message, Color color)
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
                }
            }
            catch (Exception e) { };
        }

        private static void TurnAllToysOff()
        {
            if (config.devices == null)
                return;
            foreach (Device device in config.devices)
            {
                if (!Mediator.activeToys.ContainsKey(device.device_name))
                    continue;
                Mediator.activeToys[device.device_name].Vibrate(new double[] { 0 });
            }
        }

        class MotorData
        {
            public float lastStength = 0;
            public float nextStrength = 0;
        }
        class BehaviourData
        {
            public float[] lastAvgPos = new float[] { 0, 0 };
            public float lastDepth = 0;
            public float lastWidth = 0;
            public float lastRubAcceleration = 0;
            public float lastThrustAcceleration = 0;
        }
        private static Dictionary<Device, MotorData[]> toyMotorsDictionary = new Dictionary<Device, MotorData[]>();
        private static Dictionary<DeviceParams, BehaviourData> behaviourDataDictionary = new Dictionary<DeviceParams, BehaviourData>();
        static void CheckCaptureForInput(Bitmap capture)
        {
            if (capture == null || config.devices == null)
                return;
            foreach(Device device in config.devices)
            {
                if (!Mediator.activeToys.ContainsKey(device.device_name))
                    continue;
               if (!toyMotorsDictionary.ContainsKey(device))
                {
                    MotorData[] ar = new MotorData[device.motors];
                    toyMotorsDictionary.Add(device, ar);
                    for (int i = 0; i < device.motors; i++) ar[i] = new MotorData();
                }
                MotorData[] toyMotors = toyMotorsDictionary[device];
                for (int i=0;i< device.device_params.Length; i++)
                {
                    DeviceParams param = device.device_params[i];

                    if (!behaviourDataDictionary.ContainsKey(param))
                    {
                        behaviourDataDictionary.Add(param, new BehaviourData());
                    }
                    BehaviourData behaviourData = behaviourDataDictionary[param];

                    int[] pos = param.input_pos;
                    float x1 = pos[0] * config.window_scale + config.window_scale * 0.25f;
                    float y1 = pos[1] * config.window_scale + config.window_scale * 0.5f;
                    float x2 = pos[0] * config.window_scale + config.window_scale * 0.75f;
                    float y2 = pos[1] * config.window_scale + config.window_scale * 0.5f;
                    Color col1 = capture.GetPixel((int)(x1*capture.Width), (int)(y1*capture.Height));
                    Color col2 = capture.GetPixel((int)(x2*capture.Width), (int)(y2*capture.Height));
                    //Console.WriteLine(i + ": " + col1 + " at "+ (int)(x1 * capture.Width) +","+ (int)(y1 * capture.Height));

                    float depth = col1.R * 0.0039f;
                    float width = col1.G * 0.0039f;
                    float[] avgPos = new float[] { col2.G * 0.0039f, col2.B * 0.0039f };

                    //Console.WriteLine("------" + param.type+"------");
                    //Console.WriteLine(col1 + "," + col2);
                    if (col1.B != 0 || col2.R != 0)
                    {
                        SetUIMessage(param.GetDeviceParamsUI().label_pixel_found, "not found", Color.Red);
                        continue;
                    }
                    else
                    {
                        SetUIMessage(param.GetDeviceParamsUI().label_pixel_found, "found", Color.Green);
                    }

                    
                    float add = 0;
                    if(param.type == CalulcationType.VOLUME)
                    {
                         add = param.volume_width * width + param.volume_depth * depth;
                    }else if(param.type == CalulcationType.THRUSTING)
                    {
                        float deltaDepth = Math.Abs(depth - behaviourData.lastDepth);
                        float thrustAcceleration = Math.Min(1, behaviourData.lastThrustAcceleration + param.thrusting_acceleration);
                        if (deltaDepth == 0)
                            thrustAcceleration = Math.Max(0, thrustAcceleration-param.thrusting_acceleration);

                        //Console.WriteLine("Thrusting: "+depth+","+deltaDepth);
                        add = (depth * param.thrusting_depth_scale + deltaDepth * param.thrusting_speed_scale) * thrustAcceleration;

                        behaviourData.lastThrustAcceleration = thrustAcceleration;
                    }
                    else if(param.type == CalulcationType.RUBBING)
                    {
                        float distance = (float)(Math.Pow(behaviourData.lastAvgPos[0] - avgPos[0], 2) + Math.Pow(behaviourData.lastAvgPos[1] - avgPos[1], 2))*80;
                        float rubAcceleration = Math.Min(1, behaviourData.lastRubAcceleration + param.rubbing_acceleration);

                        if (distance == 0)
                            rubAcceleration = Math.Max(0,rubAcceleration-param.rubbing_acceleration*2);

                        add = param.rubbing_scale * Math.Min(1,distance) * rubAcceleration;
                        //Console.WriteLine("(" + avgPos[0] + "," + avgPos[1] + ") - (" + behaviourData.lastAvgPos[0] + "," + behaviourData.lastAvgPos[1] + ") = " + distance + " * " + param.rubbing_scale + " * " + rubAcceleration + " = "+(param.rubbing_scale * Math.Min(1, distance) * rubAcceleration));


                        behaviourData.lastRubAcceleration = rubAcceleration;
                    }
                    toyMotors[param.motor].nextStrength += Math.Min(param.max,add);

                    behaviourData.lastDepth = depth;
                    behaviourData.lastWidth = width;
                    behaviourData.lastAvgPos = avgPos;
                }

                double[] vals = new double[device.motors];
                for (int i = 0; i < toyMotors.Length; i++)
                {
                    if (toyMotors[i].nextStrength < config.threshold)
                        vals[i] = 0;
                    else
                        vals[i] = Math.Min(1.0f, toyMotors[i].nextStrength);
                    toyMotors[i].lastStength = toyMotors[i].nextStrength;
                    toyMotors[i].nextStrength = 0;
                }
                Mediator.activeToys[device.device_name].ExecuteFeatures(vals);
            }
        }

        static Bitmap Capture(Rectangle bounds)
        {
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            //bitmap.Save("./test.jpg", ImageFormat.Jpeg);
            return bitmap;
        }

        public static bool IsBoundsFullScreen(Rectangle bounds, Screen screen)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            return bounds.Contains(screen.Bounds);
        }

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

        private static Rectangle GetWindowBounds()
        {
            Rectangle myRect = new Rectangle();
            IntPtr handle = GetForegroundWindow();
            RECT rct = GetWindowRectangle(handle);
            //MessageBox.Show(rct.ToString());

            myRect.X = (int)(rct.Left );
            myRect.Y = (int)(rct.Top );
            myRect.Width = (int)((rct.Right - rct.Left + 1) );
            myRect.Height = (int)((rct.Bottom - rct.Top + 1) );
            if (IsBoundsFullScreen(myRect, null))
                return myRect;
            return CleanBounds(myRect);
        }

        public static RECT GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect;

            int size = Marshal.SizeOf(typeof(RECT));
            DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

            return rect;
        }

        static Rectangle CleanBounds(Rectangle bounds)
        {
            bounds.Y += titleBarHeight;
            bounds.Height -= titleBarHeight;
            return bounds;
        }

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
