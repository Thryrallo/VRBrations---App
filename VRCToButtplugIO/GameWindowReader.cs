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

        public float RECTANGLE_ABSOLUTE_WIDTH = 0;
        public float RECTANGLE_ABSOLUTE_HEIGHT = 0;

        public float SENSOR_ABSOLUTE_WIDTH = 0;
        public float SENSOR_ABSOLUTE_HEIGHT = 0;

        public float RECTANGLE_ABSOLUTE_TO_MID_OFFSET_X = 0;
        public float RECTANGLE_ABSOLUTE_TO_MID_OFFSET_Y = 0;

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
                RECTANGLE_ABSOLUTE_WIDTH = Config.DATA_RECTANGLE_WIDTH * _capture.Width;
                RECTANGLE_ABSOLUTE_HEIGHT = Config.DATA_RECTANGLE_HEIGHT * _capture.Height;
                SENSOR_ABSOLUTE_WIDTH = RECTANGLE_ABSOLUTE_WIDTH * Config.SENSOR_RECTANGLES_X;
                SENSOR_ABSOLUTE_HEIGHT = RECTANGLE_ABSOLUTE_HEIGHT * Config.SENSOR_RECTANGLES_Y;
                RECTANGLE_ABSOLUTE_TO_MID_OFFSET_X = Config.DATA_RECTANGLE_WIDTH * 0.5f * _capture.Width;
                RECTANGLE_ABSOLUTE_TO_MID_OFFSET_Y = Config.DATA_RECTANGLE_WIDTH * 0.5f * _capture.Width;
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

        Color c_zero;
        Color c_one;
        Color c_two;
        Color c_three;
        Color c_four;
        Color c_five;
        Color c_six;
        Color c_seven;
        public void CailbrateColors(SensorCoordinates sensorCoordinates, bool fromRight = false)
        {
            int x = Config.COORDS_REFERENCE_COLORS.GetXWithSensor(sensorCoordinates);
            int y = Config.COORDS_REFERENCE_COLORS.GetYWithSensor(sensorCoordinates);
            float offset = Config.DATA_RECTANGLE_WIDTH * _capture.Width;
            if (fromRight)
            {
                x = x + 1;
                c_zero =  _capture.GetPixel((int)(_capture.Width - x - 0 * offset), y);
                c_one =   _capture.GetPixel((int)(_capture.Width - x - 1 * offset), y);
                c_two =   _capture.GetPixel((int)(_capture.Width - x - 2 * offset), y);
                c_three = _capture.GetPixel((int)(_capture.Width - x - 3 * offset), y);
                c_four =  _capture.GetPixel((int)(_capture.Width - x - 4 * offset), y);
                c_five =  _capture.GetPixel((int)(_capture.Width - x - 5 * offset), y);
                c_six =   _capture.GetPixel((int)(_capture.Width - x - 6 * offset), y);
                c_seven = _capture.GetPixel((int)(_capture.Width - x - 7 * offset), y);
            }
            else
            {
                c_zero =  _capture.GetPixel((int)(x + 0 * offset), y);
                c_one =   _capture.GetPixel((int)(x + 1 * offset), y);
                c_two =   _capture.GetPixel((int)(x + 2 * offset), y);
                c_three = _capture.GetPixel((int)(x + 3 * offset), y);
                c_four =  _capture.GetPixel((int)(x + 4 * offset), y);
                c_five =  _capture.GetPixel((int)(x + 5 * offset), y);
                c_six =   _capture.GetPixel((int)(x + 6 * offset), y);
                c_seven = _capture.GetPixel((int)(x + 7 * offset), y);
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

        private Color[] GetColors(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, int amount, bool fromRight = false)
        {
            Color[] ar = new Color[amount];
            int y = sensorCoordinates.pixel_y + inSensorCoordiantes.pixel_offset_y;
            int baseX = sensorCoordinates.pixel_x + inSensorCoordiantes.pixel_offset_x;
            if (fromRight) baseX = _capture.Width - 1 - sensorCoordinates.pixel_x - inSensorCoordiantes.pixel_offset_x;
            float offset = Config.DATA_RECTANGLE_WIDTH * _capture.Width;
            for (int i = 0; i < amount; i++)
            {
                if (fromRight)
                {
                    ar[i] = _capture.GetPixel((int)(baseX - i * offset), y);
                }
                else
                {
                    ar[i] = _capture.GetPixel((int)(baseX + i * offset), y);
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
            integer += (firstColAsBinary & ~4) << ((colors.Length - 1) * 3); //get rid of first bit, which indicators negative
            if ((firstColAsBinary & 4) == 4) integer = -integer; //if first bit is 1, int is negative
            return integer;
        }

        public int GetInt(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, bool fromRight = false)
        {
            //Read colors from screen
            Color[] colors = GetColors(sensorCoordinates, inSensorCoordiantes, 6, fromRight);
            //Decode colors
            return DecodeColorsAsInt(colors);
        }

        public int GetShort(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, bool fromRight = false)
        {
            //Read colors from screen
            Color[] colors = GetColors(sensorCoordinates, inSensorCoordiantes, 3, fromRight);
            string s = "";
            foreach(Color c in colors)
            {
                s += c;
            }
            //Decode colors
            return DecodeColorsAsInt(colors);
        }

        public float GetFloat(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, bool fromRight = false)
        {
            return GetShort(sensorCoordinates, inSensorCoordiantes, fromRight) / 255.0f;
        }

        public bool GetBool(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, bool fromRight = false)
        {
            if(fromRight) return GetColorAsBinary(_capture.GetPixel(_capture.Width - inSensorCoordiantes.GetXWithSensor(sensorCoordinates), inSensorCoordiantes.GetYWithSensor(sensorCoordinates))) == 7;
            return GetColorAsBinary(_capture.GetPixel(inSensorCoordiantes.GetXWithSensor(sensorCoordinates), inSensorCoordiantes.GetYWithSensor(sensorCoordinates))) == 7;
        }

        /**
         * <param name="width">In pixel * 2 (char size)</param>
         * */
        public string GetString(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, int width, int height, bool fromRight = false)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x = x + 2)
                {
                    sb.Append(GetChar(sensorCoordinates, inSensorCoordiantes.Add(x,y), fromRight));
                }
            }
            return sb.ToString();
        }

        public char GetChar(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, bool fromRight = false)
        {
            Color c1 = GetPixel(sensorCoordinates, inSensorCoordiantes, 0, 0, fromRight);
            Color c2 = GetPixel(sensorCoordinates, inSensorCoordiantes, 1, 0, fromRight);
            int i = (GetColorAsBinary(c1) << 3) + GetColorAsBinary(c2);
            return CompressedIntToChar(i);
        }

        private Color GetPixel(SensorCoordinates sensorCoordinates, InSensorCoordiantes inSensorCoordiantes, int offsetX = 0, int offsetY = 0, bool fromRight = false)
        {
            inSensorCoordiantes = inSensorCoordiantes.Add(offsetX, offsetY);
            if (fromRight)
            {
                return _capture.GetPixel(_capture.Width - 1 -inSensorCoordiantes.GetXWithSensor(sensorCoordinates), inSensorCoordiantes.GetYWithSensor(sensorCoordinates));
            }
            else
            {
                return _capture.GetPixel(inSensorCoordiantes.GetXWithSensor(sensorCoordinates), inSensorCoordiantes.GetYWithSensor(sensorCoordinates));
            }
            
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

        public void SaveCurrentCaptureDirectly(string filename)
        {
            this.saveName = filename;
            _capture.Save($"./{saveName}.png", ImageFormat.Png);
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
