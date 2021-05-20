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
            if (boundsCalculator.Update())
            {
                if (_capture != null)
                {
                    _capture.Dispose();
                }
                _capture = new Bitmap(boundsCalculator.bounds.Width, boundsCalculator.bounds.Height, PixelFormat.Format32bppRgb);
            }
            if (save && _capture != null)
            {
                _capture.Save($"./{saveName}.png", ImageFormat.Png);
                save = false;
            }
            if (boundsCalculator.IsValidBounds())
            {
                using (Graphics g = Graphics.FromImage(_capture))
                {
                    g.CopyFromScreen(new Point(boundsCalculator.bounds.Left, boundsCalculator.bounds.Top), Point.Empty, boundsCalculator.bounds.Size);
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

        public Pixel GetPixel(int x, int y)
        {
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
