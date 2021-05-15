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
        public static int titleBarHeight = 0;
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
            dpi = getScalingFactor();

            string justHereToForceUIInit = Mediator.ui.Name;

            //this is correct if the title bar is default, but not with material design
            //Rectangle screenRectangle = Mediator.ui.RectangleToScreen(Mediator.ui.ClientRectangle);
            //titleBarHeight = (int)((screenRectangle.Top - Mediator.ui.Top) * dpi);

            //Console.WriteLine(screenRectangle.Top - Mediator.ui.Top);

            //dont ask why +8, idk, but tests seemed to have shown it as correct
            titleBarHeight = (int)((SystemInformation.CaptionHeight + 8) * dpi);
            DebugToFile("TitleBar Height: " + titleBarHeight+", DPI: "+dpi);
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

        public static double LinarToGamma(double linear)
        {
            return Math.Pow(linear, 1 / 2.2);
        }

        public static double GammaToLinar(double gamma)
        {
            
            return Math.Pow(gamma, 2.2);
        }

        public static Color GetColor(double r, double g, double b)
        {
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        public static Color GetGammaColor(double r, double g, double b)
        {
            return Color.FromArgb((int)(LinarToGamma(r) * 255), (int)(LinarToGamma(g) * 255), (int)(LinarToGamma(b) * 255));
        }

        public static bool IsSameColor(Color a, Color b)
        {
            return IsSameColor(a.R , b.R) && IsSameColor(a.G , b.G) && IsSameColor(a.B , b.B);
        }

        public static bool IsSameColor(int f1, int f2)
        {
            double f = f1 / 255.0f;
            double maxDelta = (LinarToGamma(f + 0.018f) - LinarToGamma(f)) * 255;
            return Math.Abs(f1 - f2) < maxDelta;
        }

        static readonly Color GLOBAL_INDICATOR_COLOR = GetGammaColor(0.69,0.01,0.69);

        static long lastSlowUpdateStart = 0;
        static long lastSlowUpdateEnd = 0;
        const float slowUpdateRate = 10000;

        static long lastUpdate = 0;
        static float lastVerifiedKey = 0;

        static long vrchatNotInFocusStart = 0;
        static bool wasVRChatInFocus = true;
        static void Logic()
        {
            while (true)
            {
                long now;
                if (KeyManager.VerifyKey())
                {
                    string activeWindow = GetActiveWindow();
                    bool isAppInFocus = Config.WINDOW_NAMES.Contains(activeWindow);
                    if (isAppInFocus)
                    {
                        try
                        {
                            Rectangle windowBounds = GetWindowBounds();
                            if (windowBounds.Height > 0 && windowBounds.Width > 0)
                            {
                                Bitmap capture = Capture(windowBounds);
                                CheckCaptureForInput(capture);
                                capture.Dispose();

                                SetUIMessage(Mediator.ui.label_vrc_focus, $"VRC is in focus ( {windowType} )", Color.Green);
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
                        if (wasVRChatInFocus)
                        {
                            TurnAllToysOff();
                            vrchatNotInFocusStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        }
                        SetUIMessage(Mediator.ui.label_vrc_focus, $"VRC not in focus\nCurrent window:\n{activeWindow}", Color.Red);
                    }
                    wasVRChatInFocus = isAppInFocus;
                    //do slow update on apis
                    now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if (now - lastSlowUpdateStart > slowUpdateRate && now - lastSlowUpdateEnd > slowUpdateRate)
                    {
                        Thread slowUpdate = new Thread(SlowUpdate);
                        slowUpdate.Name = "SlowUpdate";
                        slowUpdate.Start();
                        lastSlowUpdateStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    } 
                    
                    lastVerifiedKey = lastUpdate;
                }
                else if (lastUpdate != 0 && (lastVerifiedKey - lastUpdate) > 30000)
                {
                    DebugToFile("[KEY] Key has not been verified recently. Closing Application.");
                    Task.Delay(1000).ContinueWith(t => { System.Windows.Forms.Application.Exit(); });
                    now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
                int timeout = (int)(config.update_rate - (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdate));
                if(timeout>0)
                    Thread.Sleep(timeout);
                lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        static bool hasSentNotInFocusNotification = false;
        private static void SlowUpdate()
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
            if(wasVRChatInFocus == false && hasSentNotInFocusNotification==false && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - vrchatNotInFocusStart > 20000)
            {
                //Mediator.SendXSNotification("VRChat is not in focus.", "Make sure the vrchat window is the active window.");
                hasSentNotInFocusNotification = true;
            }
            else if(wasVRChatInFocus)
            {
                hasSentNotInFocusNotification = false;
            }
            lastSlowUpdateEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private static Dictionary<Label, long> lastXSMessageTime = new Dictionary<Label, long>();
        private static void SetUIMessage(Label l, string message, Color color, string xsTitle = null, string xsMessage = null, float xsCooldownTime = 0, bool xsCooldownBeforeFirstPopup = false)
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

        static bool UpdateBehaviourPixelData(Bitmap capture, BehaviourData behaviourData)
        {
            int[] pos = behaviourData.input_pos;
            float x1 = pos[0] * Config.PIXEL_SCALE + Config.PIXEL_SCALE * 0.25f;
            float y1 = pos[1] * Config.PIXEL_SCALE + Config.PIXEL_SCALE * 0.25f;
            float x2 = pos[0] * Config.PIXEL_SCALE + Config.PIXEL_SCALE * 0.75f;
            float y2 = pos[1] * Config.PIXEL_SCALE + Config.PIXEL_SCALE * 0.25f;
            Color col1 = capture.GetPixel((int)(x1 * capture.Width), (int)(y1 * capture.Height));
            Color col2 = capture.GetPixel((int)(x2 * capture.Width), (int)(y2 * capture.Height));
            //Console.WriteLine(i + ": " + col1 + " at "+ (int)(x1 * capture.Width) +","+ (int)(y1 * capture.Height));

            behaviourData.GetRuntimeData().depth = col1.R * 0.0039f;
            behaviourData.GetRuntimeData().width = col1.G * 0.0039f;
            behaviourData.GetRuntimeData().avgX = col2.G * 0.0039f;
            behaviourData.GetRuntimeData().avgY = col2.B * 0.0039f;

            return col1.B == 0 && col2.R == 0;
        }

        static bool audioLinkFound;
        static float[] audioLinkData = new float[] { 0, 0, 0, 0 };

        static bool LoadGlobalData(Bitmap capture)
        {
            float insidePixelOffset = Config.PIXEL_SCALE * 0.5f;
            Color indicatorColor = capture.GetPixel((int)((1 - insidePixelOffset) * capture.Width), (int)(insidePixelOffset * capture.Height));
            if(IsSameColor(indicatorColor,GLOBAL_INDICATOR_COLOR))
            {
                Color audioLinkColor0 = capture.GetPixel((int)((1 - insidePixelOffset - Config.PIXEL_SCALE * 1) * capture.Width), (int)(insidePixelOffset * capture.Height));
                Color audioLinkColor1 = capture.GetPixel((int)((1 - insidePixelOffset - Config.PIXEL_SCALE * 2) * capture.Width), (int)(insidePixelOffset * capture.Height));
                audioLinkData[0] = (float)GammaToLinar(audioLinkColor0.R / 255.0);
                audioLinkData[1] = (float)GammaToLinar(audioLinkColor0.G / 255.0);
                audioLinkData[2] = (float)GammaToLinar(audioLinkColor1.R / 255.0);
                audioLinkData[3] = (float)GammaToLinar(audioLinkColor1.G / 255.0);
                audioLinkFound = audioLinkColor0.B > 0;
                return true;
            }
            else
            {
                audioLinkData[0] = 0;
                audioLinkData[1] = 0;
                audioLinkData[2] = 0;
                audioLinkData[3] = 0;
                audioLinkFound = false;
                return false;
            }
        }

        static void CheckCaptureForInput(Bitmap capture)
        {
            if (capture == null)
                return;

            LoadGlobalData(capture);

            foreach (Toy t in Mediator.activeToys.Values)
            {
                for (int i = 0; i < t.featureStrengths.Length; i++) t.featureStrengths[i] = 0;

                foreach (BehaviourData behaviour in t.GetBehaviours())
                {
                    bool behaviourFound;
                    if (behaviour.type == CalulcationType.AudioLink) behaviourFound = audioLinkFound;
                    else behaviourFound = UpdateBehaviourPixelData(capture, behaviour);

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

        static WindowType windowType;
        static Rectangle lastBounds;
        static int vrcBlackBarsHorizontal = 0;
        static int vrcBlackBarsVertical = 0;
        private static Rectangle GetWindowBounds()
        {
            Rectangle forgroundWindowBounds = GetForegroundWindowBounds();
            if(forgroundWindowBounds != lastBounds)
            {
                //Recalculate windowtype and
                if(IsBoundsFullScreen(forgroundWindowBounds,null) == false)
                {
                    if (windowType != WindowType.windowed) DebugToFile("[Game Window] Swapped to windowed");
                    windowType = WindowType.windowed;
                    lastBounds = forgroundWindowBounds;
                }
                else
                {
                    Thread.Sleep(500);
                    Bitmap capture = Capture(forgroundWindowBounds);
                    Color topLeft = capture.GetPixel(1, 1);
                    Color topRight = capture.GetPixel(capture.Width - 2, 1);

                    if (topLeft == topRight && topLeft == Color.FromArgb(255, 255, 255))
                    {
                        if (windowType != WindowType.maximized) DebugToFile("[Game Window] Swapped to maximized");
                        windowType = WindowType.maximized;
                        lastBounds = forgroundWindowBounds;
                    }
                    else
                    {
                        if (windowType != WindowType.fullscreen) DebugToFile("[Game Window] Swapped to fullscreen");
                        //Is Fullscreen
                        //if whole screen black try again next time
                        if (OnVRCEnteredFullscreen(capture, forgroundWindowBounds))
                        {
                            //Bitmap capture = Capture(BoundsRemoveVRCBlackBars(fullBounds));
                            //capture.Save("./fullscreen.jpg", ImageFormat.Jpeg);
                            //capture.Dispose();
                            windowType = WindowType.fullscreen;
                            lastBounds = forgroundWindowBounds;
                        }
                    }
                    capture.Dispose();
                }
            }

            switch (windowType)
            {
                case WindowType.maximized:
                case WindowType.windowed:
                    return CleanBounds(forgroundWindowBounds);
                case WindowType.fullscreen:
                    return BoundsRemoveVRCBlackBars(forgroundWindowBounds);
            }
            return forgroundWindowBounds;
        }

        private static Rectangle GetForegroundWindowBounds()
        {
            Rectangle fullBounds = new Rectangle();
            IntPtr handle = GetForegroundWindow();
            RECT rct = GetWindowRectangle(handle);
            //MessageBox.Show(rct.ToString());

            fullBounds.X = (int)(rct.Left);
            fullBounds.Y = (int)(rct.Top);
            fullBounds.Width = (int)((rct.Right - rct.Left + 1));
            fullBounds.Height = (int)((rct.Bottom - rct.Top + 1));

            return fullBounds;
        }

        enum WindowType { windowed, maximized, fullscreen};

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

        private static bool OnVRCEnteredFullscreen(Bitmap capture, Rectangle fullBounds)
        {
            

            vrcBlackBarsHorizontal = 0;
            bool collumIsBlack = true;
            while(vrcBlackBarsHorizontal < capture.Width && collumIsBlack)
            {
                for(int y = 0; y < capture.Height; y++)
                {
                    Color c = capture.GetPixel(vrcBlackBarsHorizontal, y);
                    if (c.R > 0 || c.G > 0 || c.B > 0)
                    {
                        collumIsBlack = false;
                    }
                }
                if (collumIsBlack) vrcBlackBarsHorizontal++;
            }

            vrcBlackBarsVertical = 0;
            bool isRowBlack = true;
            while (vrcBlackBarsVertical < capture.Height && isRowBlack)
            {
                for (int x = 0; x < capture.Width; x++)
                {
                    Color c = capture.GetPixel(x, vrcBlackBarsVertical);
                    if (c.R > 0 || c.G > 0 || c.B > 0)
                    {
                        isRowBlack = false;
                    }
                }
                if (isRowBlack) vrcBlackBarsVertical++;
            }

            int width = capture.Width;
            int height = capture.Height;
            if (vrcBlackBarsHorizontal == width || vrcBlackBarsVertical == height)
            {
                vrcBlackBarsHorizontal = 0;
                vrcBlackBarsVertical = 0;
                return false;
            }
            return true;
        }

        private static Rectangle BoundsRemoveVRCBlackBars(Rectangle bounds)
        {
            bounds.X += vrcBlackBarsHorizontal;
            bounds.Width -= vrcBlackBarsHorizontal * 2;
            bounds.Y += vrcBlackBarsVertical;
            bounds.Height -= vrcBlackBarsVertical * 2;
            return bounds;
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
