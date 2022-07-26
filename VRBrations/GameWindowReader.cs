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

        public bool HasCapture()
        {
            return _capture != null;
        }

        public int GetInt((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes)
        {
            //Read colors from screen
            Pixel[] colors = GetPixels(sensorCoords, inSensorCoordiantes, 2);
            //Decode colors
            return ((colors[0].r > 0.5f) ? -1 : 1) * ((colors[0].bBinary << 12) + (colors[1].rBinary << 8) + (colors[1].gBinary << 4) + colors[1].bBinary);
        }

        public int GetShort((int,int) sensorCoords, InSensorCoordiantes inSensorCoordiantes)
        {
            //Read colors from screen
            Pixel[] colors = GetPixels(sensorCoords, inSensorCoordiantes, 1);
            //Decode colors
            return ((colors[0].r > 0.5f) ? -1 : 1) * ((colors[0].gBinary << 4) + colors[0].bBinary);
        }

        public float GetFloat((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes)
        {
            return GetShort(sensorCoords, inSensorCoordiantes) / 255.0f;
        }

        public bool GetBool((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes)
        {
            return GetPixels(sensorCoords, inSensorCoordiantes, 1)[0].r > 0.5f;
        }

        /**
         * <param name="width">In pixel * 2 (char size)</param>
         * */
        public string GetString((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes, int width, int height)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Pixel pixel = GetPixel(sensorCoords, inSensorCoordiantes.Add(x, y), 1, false);
                    char[] c = PixelToChars(pixel);
                    if (c[0] != (char)0) sb.Append(c[0]);
                    if (c[1] != (char)0) sb.Append(c[1]);
                }
            }
            return sb.ToString();
        }

        public static char[] PixelToChars(Pixel c)
        {
            int binaryOne = 0;
            int binaryTwo = 0;

            int r = (int)((c.r * 255 + 8) / 17);
            binaryOne += (r & 12) << 2;
            binaryTwo += (r & 3) << 4;

            binaryOne += (int)((c.g * 255 + 8) / 17);
            binaryTwo += (int)((c.b * 255 + 8) / 17);

            return new char[] { CompressedIntToChar(binaryOne), CompressedIntToChar(binaryTwo) };
        }

        public Pixel[] GetPixels((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes, int amount)
        {
            Pixel[] ar = new Pixel[amount];
            int y = (int)(inSensorCoordiantes.GetYWithSensor(sensorCoords.Item2) * _capture.Height);
            float baseX = inSensorCoordiantes.GetXWithSensor(sensorCoords.Item1);
            for (int i = 0; i < amount; i++)
            {
                int x = (int)((baseX + i * Config.PIXEL_WIDTH) * _capture.Width);
                ar[i] = new Pixel(x, y, _capture, true);
            }
            return ar;
        }

        public Pixel GetPixel((int, int) sensorCoords, InSensorCoordiantes inSensorCoordiantes, int amount, bool doGammaCorrction)
        {
            int y = (int)(inSensorCoordiantes.GetYWithSensor(sensorCoords.Item2) * _capture.Height);
            int x = (int)(inSensorCoordiantes.GetXWithSensor(sensorCoords.Item1) * _capture.Width);
            return new Pixel(x, y, _capture, doGammaCorrction);
        }

        public static char CompressedIntToChar(int i)
        {
            if (i == 0) return (char)0;
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

        public void SaveCurrentCapture(string filename)
        {
            this.saveName = filename;
            this.save = true;
        }

        public void SaveCurrentCaptureDirectly(string filename)
        {
            this.saveName = filename;
            if (_capture != null)
            {
                _capture.Save($"./{saveName}.png", ImageFormat.Png);
            }
        }
    }

    public struct Pixel
    {
        public float r;
        public float g;
        public float b;

        public Color c;
        public int captureX;
        public int captureY;

        public Pixel(int captureX, int captureY, Bitmap capture, bool doGammaCorrection)
        {
            this.captureX = captureX;
            this.captureY = captureY;
            this.c = capture.GetPixel(captureX, captureY);
            if (doGammaCorrection)
            {
                this.r = GammaToLinear((float)c.R / 255.0f);
                this.g = GammaToLinear((float)c.G / 255.0f);
                this.b = GammaToLinear((float)c.B / 255.0f);
            }
            else
            {
                this.r = ((float)c.R / 255.0f);
                this.g = ((float)c.G / 255.0f);
                this.b = ((float)c.B / 255.0f);
            }
        }

        public int rBinary
        {
            get{ return (int)( (r * 255 + 8) / 17 ); }
        }

        public int gBinary
        {
            get { return (int)((g * 255 + 8) / 17); }
        }

        public int bBinary
        {
            get { return (int)((b * 255 + 8) / 17); }
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
