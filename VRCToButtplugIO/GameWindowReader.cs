using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    class GameWindowReader
    {
        public static GameWindowReader Singleton { get; private set; } = new GameWindowReader();

        private Bitmap _capture;
        private BoundsCalculator boundsCalculator;

        private bool save;
        private string saveName;

        public GameWindowReader(){
            boundsCalculator = new BoundsCalculator();
        }

        /**
         * <return>Returns true if successfull</return>
         * */
        public bool UpdateCapture()
        {
            if (save && _capture != null)
            {
                _capture.Save($"./{saveName}.png", ImageFormat.Png);
                save = false;
            }
            if (boundsCalculator.Update())
            {
                if (_capture != null)
                {
                    _capture.Dispose();
                }
                _capture = new Bitmap(boundsCalculator.bounds.Width, boundsCalculator.bounds.Height, PixelFormat.Format32bppRgb);
            }
            if (boundsCalculator.IsValidBounds())
            {
                using (Graphics g = Graphics.FromImage(_capture))
                {
                    g.CopyFromScreen(new Point(boundsCalculator.bounds.Left, boundsCalculator.bounds.Top), Point.Empty, boundsCalculator.bounds.Size);
                }
                if(boundsCalculator.windowType == BoundsCalculator.WindowType.fullscreen)
                {
                    GetRenderSize();
                }
                return true;
            }
            return false;
        }

        public Bitmap Capture
        {
            get
            {
                return _capture;
            }
        }

        private void GetRenderSize()
        {
            //TODO
        }

        Color c_zero;
        Color c_one;
        Color c_two;
        Color c_three;
        Color c_four;
        Color c_five;
        Color c_six;
        Color c_seven;
        public void CailbrateColors(int x, int y, bool fromRight = false)
        {
            if (fromRight)
            {
                c_zero =  _capture.GetPixel(_capture.Width - x - 1, y);
                c_one =   _capture.GetPixel(_capture.Width - x - 2, y);
                c_two =   _capture.GetPixel(_capture.Width - x - 3, y);
                c_three = _capture.GetPixel(_capture.Width - x - 4, y);
                c_four =  _capture.GetPixel(_capture.Width - x - 5, y);
                c_five =  _capture.GetPixel(_capture.Width - x - 6, y);
                c_six =   _capture.GetPixel(_capture.Width - x - 7, y);
                c_seven = _capture.GetPixel(_capture.Width - x - 8, y);
            }
            else
            {
                c_zero =  _capture.GetPixel(x + 0, y);
                c_one =   _capture.GetPixel(x + 1, y);
                c_two =   _capture.GetPixel(x + 2, y);
                c_three = _capture.GetPixel(x + 3, y);
                c_four =  _capture.GetPixel(x + 4, y);
                c_five =  _capture.GetPixel(x + 5, y);
                c_six =   _capture.GetPixel(x + 6, y);
                c_seven = _capture.GetPixel(x + 7, y);
            }
        }

        private Color AddMargin(Color c)
        {
            return Color.FromArgb(Math.Max(0, c.R - 40), Math.Max(0, c.G - 40), Math.Max(0, c.B - 40));
        }

        private int GetColorAsBinary(Color c)
        {
            int closestMatchDistance = 255;
            int closestMatch = 0;
            DistanceCheck(c, c_seven, 7, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_six  , 6, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_five,  5, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_four,  4, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_three, 3, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_two,   2, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_one,   1, ref closestMatch, ref closestMatchDistance);
            DistanceCheck(c, c_zero,  0, ref closestMatch, ref closestMatchDistance);
            return closestMatch;
        }

        private void DistanceCheck(Color c, Color target, int matchInt, ref int closestMatch, ref int closestMatchDistance)
        {
            int d = Math.Abs(c.R - target.R) + Math.Abs(c.G - target.G) + Math.Abs(c.B - target.B);
            if (d < closestMatchDistance)
            {
                closestMatch = matchInt;
                closestMatchDistance = d;
            }
        }

        private Color[] GetColors(int x, int y, int amount, bool fromRight = false)
        {
            Color[] ar = new Color[amount];
            int baseX = x;
            if (fromRight) baseX = _capture.Width - 1 - x;
            for (int i = 0; i < amount; i++)
            {
                if (fromRight)
                {
                    Console.WriteLine((baseX - i) + " , " + y);
                    ar[i] = _capture.GetPixel(baseX - i, y);
                }
                else
                {
                    ar[i] = _capture.GetPixel(baseX + i, y);
                }
            }
            return ar;
        }

        private int DecodeColorsAsInt(Color[] colors)
        {
            int integer = 0;
            for(int i = 1; i < colors.Length; i++)
            {
                integer += GetColorAsBinary(colors[i]) << ((colors.Length - i - 1) * 3);
            }
            int firstColAsBinary = GetColorAsBinary(colors[0]); // extra handling cuase first color has negative indicator
            integer += firstColAsBinary & ~4; //get rid of first bit, which indicators negative
            if ((firstColAsBinary & 4) == 4) integer = -integer; //if first bit is 1, int is negative
            return integer;
        }

        public int GetInt(int x, int y, bool fromRight = false)
        {
            //Read colors from screen
            Color[] colors = GetColors(x, y, 6, fromRight);
            //Decode colors
            return DecodeColorsAsInt(colors);
        }

        public int GetShort(int x, int y, bool fromRight = false)
        {
            //Read colors from screen
            Color[] colors = GetColors(x, y, 3, fromRight);
            string s = "";
            foreach(Color c in colors)
            {
                s += c;
            }
            Console.WriteLine(s + " : " + DecodeColorsAsInt(colors));
            //Decode colors
            return DecodeColorsAsInt(colors);
        }

        public float GetFloat(int x, int y, bool fromRight = false)
        {
            return GetShort(x, y, fromRight) / 255.0f;
        }

        public bool GetBool(int x, int y, int subX = 0, bool fromRight = false)
        {
            if(fromRight) return GetColorAsBinary(_capture.GetPixel(_capture.Width - x * 3 - subX - 1, y)) == 7;
            return GetColorAsBinary(_capture.GetPixel(x * 3 + subX, y)) == 7;
        }

        /**
         * <param name="width">In pixel * 2 (char size)</param>
         * */
        public string GetString(int startX, int startY, int width, int height, int subX = 0, bool fromRight = false)
        {
            string s = "";
            startX = startX * 3 + subX;
            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width * 2; x = x + 2)
                {
                    s += GetChar(x, y, pixelCoordinates: true, fromRight: fromRight);
                }
            }
            return s;
        }

        public char GetChar(int x, int y, int subX = 0, bool pixelCoordinates = false, bool fromRight = false)
        {
            Color c1;
            Color c2;
            int baseX = x;
            if (pixelCoordinates == false)
            {
                baseX = x * 3 + subX;
            }
            if (fromRight)
            {
                c1 = _capture.GetPixel(_capture.Width - baseX - 1, y);
                c2 = _capture.GetPixel(_capture.Width - baseX - 2, y);
            }
            else
            {
                c1 = _capture.GetPixel(baseX + 0, y);
                c2 = _capture.GetPixel(baseX + 1, y);
            }
            int i = (GetColorAsBinary(c1) << 3) + GetColorAsBinary(c2);
            return CompressedIntToChar(i);
        }

        public static char CompressedIntToChar(int i)
        {
            if (i == 0) return (char)32;
            if (i == 1) return (char)32;
            if (i >= 2 && i <= 11) return (char)(i + 48 - 2);
            if (i >= 12 && i <= 37) return (char)(i + 65 - 12);
            if (i >= 38 && i <= 63) return (char)(i + 97 - 38);
            return (char)0;
        }

        public Rectangle GetBounds()
        {
            return boundsCalculator.bounds;
        }

        public Pixel GetPixel(int x, int y, bool fromRight = false)
        {
            if(fromRight) return new Pixel(_capture.GetPixel(_capture.Width - x, y));
            return new Pixel(_capture.GetPixel(x, y));
        }

        public void SaveCurrentCapture(string filename)
        {
            this.saveName = filename;
            this.save = true;
        }
    }

    public struct Pixel
    {
        public float r;
        public float g;
        public float b;
        public Pixel(float x, float y, float z)
        {
            this.r = x;
            this.g = y;
            this.b = z;
        }
        public Pixel(byte x, byte y, byte z)
        {
            this.r = (float)x / 255.0f;
            this.g = (float)y / 255.0f;
            this.b = (float)z / 255.0f;
        }
        public Pixel(Color c)
        {
            this.r = GammaToLinear((float)c.R / 255.0f);
            this.g = GammaToLinear((float)c.G / 255.0f);
            this.b = GammaToLinear((float)c.B / 255.0f);
        }
        public static bool ValueEquals(Pixel p1, Pixel p2)
        {
            return ValueEquals(p1.r, p2.r) && ValueEquals(p1.g, p2.g) && ValueEquals(p1.b, p2.b);
        }
        public static bool ValueEquals(float f1 , float f2)
        {
            return Math.Abs(f1 - f2) < 0.008f;
        }
        public static float GammaToLinear(float gamma)
        {
            return (float)Math.Pow(gamma, 2.2f);
        }
        public override string ToString()
        {
            return $"({r},{g},{b})";
        }
    }
}
