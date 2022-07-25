using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCToyController
{
    class BoundsCalculator
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

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

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        const long MIN_UPDATE_RATE = 20000; //recheck bounds every 20 seconds

        private Rectangle currentBounds;
        private Rectangle fullVRChatBounds;

        public BoundsCalculator()
        {
        }

        /**
         * <returns>Returns true if bounds are new</returns>
         * */
        public bool Update()
        {
            GetWindowBounds();
            if(isNewBounds) UpdateUI();
            return isNewBounds;
        }

        public bool IsValidBounds()
        {
            return currentBounds.Height > 0 && currentBounds.Width > 0;
        }

        public Rectangle bounds
        {
            get
            {
                return currentBounds;
            }
        }

        private void UpdateUI()
        {
            if (windowType == WindowType.windowed && IsOutsideBounds(fullVRChatBounds))
            {
                Program.SetUIMessage(Mediator.ui.label_vrc_focus, "VRChat is not fully on screen.", Color.Red);
            }
            else if (IsValidBounds())
            {
                Program.SetUIMessage(Mediator.ui.label_vrc_focus, $"VRC is in focus ( {windowType} )", Color.Green);
            } 
            else
            {
                Program.SetUIMessage(Mediator.ui.label_vrc_focus, "VRC bounds wrong", Color.Red);
            }
        }

        private Bitmap Capture(Rectangle bounds)
        {
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }
            return bitmap;
        }

        struct BoundsCalculationData
        {
            public Rectangle bounds;
            public int leftOffset;
            public int topOffset;
            public int rightOffset;
            public int bottomOffset;
            public BoundsCalculationData(Rectangle bounds)
            {
                this.bounds = bounds;
                this.leftOffset = 0;
                this.topOffset = 0;
                this.rightOffset = 0;
                this.bottomOffset = 0;
            }

            public void Offset(int left, int right, int top, int bottom)
            {
                bounds.X += left;
                bounds.Width -= left + right;
                bounds.Y += top;
                bounds.Height -= top + bottom;
                leftOffset += left;
                rightOffset += right;
                topOffset += top;
                bottomOffset += bottom;
            }
        }

        private bool TEST = false;

        bool isNewBounds; //new cleaned bounds
        bool isNewFullBounds; //new full window bounds

        long lastUpdate = 0;
        WindowType _windowType;
        Rectangle lastFullBounds;
        int vrcBlackBarsHorizontal = 0;
        int vrcBlackBarsVertical = 0;
        private void GetWindowBounds()
        {
            Rectangle forgroundWindowBounds = GetForegroundWindowBounds();
            //bounds have changed
            isNewFullBounds = forgroundWindowBounds != lastFullBounds;
            //periodic reload in case of wrongly calculated bounds
            bool periodicReload = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdate > MIN_UPDATE_RATE;
            if ((isNewFullBounds || periodicReload) && forgroundWindowBounds.Width > 0 && forgroundWindowBounds.Height > 0)
            {
                Rectangle prevBounds = currentBounds;
                fullVRChatBounds = forgroundWindowBounds;
                //Console.WriteLine(isNewFullBounds + " , " + periodicReload);

                if (isNewFullBounds) Thread.Sleep(500);
                
                Bitmap capture = Capture(forgroundWindowBounds);

                WindowType newWindowType = DetermineWindowType(forgroundWindowBounds);

                if(newWindowType == WindowType.windowed)
                {
                    if (windowType != WindowType.windowed) Program.DebugToFile("[Game Window] Swapped to windowed");
                    _windowType = WindowType.windowed;
                    currentBounds = CleanBoundsWindowed(forgroundWindowBounds, capture);
                    currentBounds = VRCBoundsToVRbrationsBounds(currentBounds);
                    lastFullBounds = forgroundWindowBounds;
                }else if(newWindowType == WindowType.maximized)
                {
                    if (windowType != WindowType.maximized) Program.DebugToFile("[Game Window] Swapped to maximized");
                    _windowType = WindowType.maximized;
                    currentBounds = CleanBoundsMaximied(forgroundWindowBounds, capture);
                    currentBounds = VRCBoundsToVRbrationsBounds(currentBounds);
                    lastFullBounds = forgroundWindowBounds;
                }
                else if(newWindowType == WindowType.fullscreen)
                {
                    if (windowType != WindowType.fullscreen) Program.DebugToFile("[Game Window] Swapped to fullscreen");
                    //Is Fullscreen
                    //if whole screen black try again next time
                    if (OnVRCEnteredFullscreen(capture, forgroundWindowBounds))
                    {
                        _windowType = WindowType.fullscreen;
                        currentBounds = BoundsRemoveVRCBlackBars(forgroundWindowBounds);
                        currentBounds = VRCBoundsToVRbrationsBounds(currentBounds);
                        lastFullBounds = forgroundWindowBounds;
                    }
                }
                if (TEST)
                {
                    Bitmap debugcapture = Capture(lastFullBounds);
                    debugcapture.Save("./fullBounds.png", ImageFormat.Png);
                    debugcapture.Dispose();
                    debugcapture = Capture(currentBounds);
                    debugcapture.Save("./currentBounds.png", ImageFormat.Png);
                    debugcapture.Dispose();
                    debugcapture = Capture(forgroundWindowBounds);
                    debugcapture.Save("./forgroundWindowBounds.png", ImageFormat.Png);
                    debugcapture.Dispose();
                }
                if (prevBounds != currentBounds) isNewBounds = true;
                lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                capture.Dispose();
            }
        }

        public WindowType windowType
        {
            get
            {
                return _windowType;
            }
        }

        static float scaleForgroundRelativeToSelf = 1;
        public static WindowType DetermineWindowType(Rectangle bounds)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(GetForegroundWindow(), ref placement);
            if(placement.showCmd == 1)
            {
                return IsBoundsFullScreen(bounds) ? WindowType.fullscreen : WindowType.windowed;
            }
            else
            {
                //maximized
                return WindowType.maximized;
            }
        }

        public static bool IsBoundsFullScreen(Rectangle bounds)
        {
            foreach (Screen s in Screen.AllScreens)
                if (bounds.Contains(s.Bounds))
                    return true;
            return false;
        }

        public static bool IsOutsideBounds(Rectangle bounds)
        {
            bool isInside = false;
            foreach (Screen s in Screen.AllScreens)
                if (s.Bounds.Contains(bounds))
                    isInside = true;
            return !isInside;
        }

        private static Rectangle GetForegroundWindowBounds()
        {
            Rectangle fullBounds = new Rectangle();
            IntPtr handle = GetForegroundWindow();
            RECT rct = GetWindowRectangle(handle);
            //MessageBox.Show(rct.ToString());

            fullBounds.X = (int)(rct.Left);
            fullBounds.Y = (int)(rct.Top);
            fullBounds.Width = (int)((rct.Right - rct.Left));
            fullBounds.Height = (int)((rct.Bottom - rct.Top));

            return fullBounds;
        }

        public enum WindowType { windowed, maximized, fullscreen };

        public static RECT GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect;

            int size = Marshal.SizeOf(typeof(RECT));
            DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);

            return rect;
        }

        static Rectangle VRCBoundsToVRbrationsBounds(Rectangle bounds)
        {
            bounds.Width = (int)(bounds.Width * Config.VRBATIONS_WIDTH + 0.99f);
            bounds.Height = (int)(bounds.Height * Config.VRBATIONS_HEIGHT + 0.99f);
            return bounds;
        }

        static Rectangle CleanBoundsWindowed(Rectangle bounds, Bitmap capture)
        {   
            //Windows 11 has 2 full ass pixel border on all sides
            BoundsCalculationData data = new BoundsCalculationData(bounds);
            data = RemoveTransparency(data, capture);
            //data.Offset(1, 1, 0, 1);
            data.Offset(2, 2, 2, 2);
            data = RemoveTitlebar(data, capture);
            return data.bounds;
        }

        static Rectangle CleanBoundsMaximied(Rectangle bounds, Bitmap capture)
        {
            BoundsCalculationData data = new BoundsCalculationData(bounds);
            data = RemoveTransparency(data, capture);
            data = RemoveTitlebar(data, capture);
            return data.bounds;
        }

        static BoundsCalculationData RemoveTransparency(BoundsCalculationData data, Bitmap capture)
        {
            int top = 0;
            int left = 0;
            int bottom = 0;
            int right = 0;
            while (capture.GetPixel(data.bounds.Width / 2, top).A < 1) top++;
            while (capture.GetPixel(data.bounds.Width / 2, data.bounds.Height - 1 - bottom).A < 1) bottom++;
            while (capture.GetPixel(left, data.bounds.Height / 2).A < 1) left++;
            while (capture.GetPixel(data.bounds.Width - 1 - right, data.bounds.Height / 2).A < 1) right++;
            data.Offset(left, right, top, bottom);
            return data;
        }

        static BoundsCalculationData RemoveTitlebar(BoundsCalculationData data, Bitmap capture)
        {
            Color titleBarColor = capture.GetPixel((int)(data.bounds.Width / 2), data.topOffset + 1 );
            int y = data.topOffset + 1;
            int lastTitleBarRow = 0;
            while(capture.GetPixel((int)(data.bounds.Width / 2), y) == titleBarColor)
            {
                bool wasCompleteRow = true;
                for (int x = 2; x < capture.Width - 2; x++)
                {
                    if(capture.GetPixel(x,y) != titleBarColor)
                    {
                        wasCompleteRow = false;
                        break;
                    }
                }
                if (wasCompleteRow) lastTitleBarRow = y;
                y++;
            }
            data.Offset(0, 0, 1 + lastTitleBarRow - data.topOffset, 0);
            return data;
        }

        private bool OnVRCEnteredFullscreen(Bitmap capture, Rectangle fullBounds)
        {
            vrcBlackBarsHorizontal = 0;
            bool collumIsBlack = true;
            while (vrcBlackBarsHorizontal < capture.Width && collumIsBlack)
            {
                for (int y = 0; y < capture.Height; y++)
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

        private Rectangle BoundsRemoveVRCBlackBars(Rectangle bounds)
        {
            bounds.X += vrcBlackBarsHorizontal;
            bounds.Width -= vrcBlackBarsHorizontal * 2;
            bounds.Y += vrcBlackBarsVertical;
            bounds.Height -= vrcBlackBarsVertical * 2;
            return bounds;
        }

    }
}
