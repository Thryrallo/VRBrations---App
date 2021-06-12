using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public class Config
    {
        //Constant private valies
        private const string DEFAULT_CONFIG_NAME = "default";
        private const string FILE_POSTFIX = @".config";
        private static string[] CONFIG_FILENAMES;

        //Constant Values
        public static readonly string[] WINDOW_NAMES = new string[] { "VRChat" };
        public const int SLOW_UPDATE_RATE = 10000;
        public const int SENSOR_RECTANGLES_X = 12;
        public const int SENSOR_RECTANGLES_Y = 7;

        public const int SENSOR_MAX_X = 5;
        public const int SENSOR_MAX_Y = 2;

        public const float DATA_RECTANGLE_WIDTH_IN_PERCENT = 0.5f;
        public const float DATA_RECTANGLE_HEIGHT_IN_PERCENT = 0.5f;

        public const float DATA_RECTANGLE_WIDTH = DATA_RECTANGLE_WIDTH_IN_PERCENT / 100f;
        public const float DATA_RECTANGLE_HEIGHT = DATA_RECTANGLE_HEIGHT_IN_PERCENT / 100f;

        public const float SENSOR_RELATIVE_WIDTH = SENSOR_RECTANGLES_X * DATA_RECTANGLE_WIDTH;
        public const float SENSOR_RELATIVE_HEIGHT = SENSOR_RECTANGLES_Y * DATA_RECTANGLE_HEIGHT;

        public const int CHECK_VALUE_SHORT = 175;

        public static readonly SensorCoordinates COORDS_MAIN_POSITION = new SensorCoordinates(0, 0);

        //Coords for exist check
        public static readonly InSensorCoordiantes COORDS_REFERENCE_COLORS = new InSensorCoordiantes(0, 3);
        public static readonly InSensorCoordiantes COORDS_CHECK_VALUE = new InSensorCoordiantes(9, 3);

        //Coords for audiolink
        public static readonly InSensorCoordiantes COORDS_AUDIOLINK = new InSensorCoordiantes(0, 1);
        public static readonly InSensorCoordiantes COORDS_AUDIOLINK_EXISITS = new InSensorCoordiantes(0, 2);

        //Coords for sensor data
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_DEPTH = new InSensorCoordiantes(0, 2);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_WIDTH = new InSensorCoordiantes(3, 2);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_X = new InSensorCoordiantes(6, 2);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_Y = new InSensorCoordiantes(9, 2);

        public static readonly InSensorCoordiantes COORDS_SENSOR_NAME = new InSensorCoordiantes(0, 5);

        public const string SENSORNAME_AUDIOLINK = "AudioLink";

        //Serialized Values
        public float update_rate = 100;
        public float threshold = 0.05f;
        public List<DeviceData> devices;

        //Private Non Serialized values
        private string fileName;
        private Dictionary<string, DeviceData> deviceDataIdDictionary = new Dictionary<string, DeviceData>();

        public Config()
        {
            devices = new List<DeviceData>();
        }

        public DeviceData GetDeviceData(Toy toy)
        {
            if (deviceDataIdDictionary.ContainsKey(toy.id) == false)
            {
                DeviceData newDeviceData = new DeviceData(toy);
                devices.Add(newDeviceData);
                deviceDataIdDictionary[toy.id] = newDeviceData;
            }
            return deviceDataIdDictionary[toy.id];
        }

        public DeviceData GetDeviceData(string deviceName)
        {
            if (deviceDataIdDictionary.ContainsKey(deviceName)) return deviceDataIdDictionary[deviceName];
            return null;
        }

        private void InitDevices()
        {
            foreach(DeviceData d in devices)
            {
                deviceDataIdDictionary.Add(d.id, d);
                List<BehaviourData> invalidBehaviours = new List<BehaviourData>();
                foreach(BehaviourData b in d.behaviours)
                {
                    if (string.IsNullOrEmpty(b.name))
                    {
                        invalidBehaviours.Add(b);
                    }
                }
                foreach(BehaviourData invalid in invalidBehaviours)
                {
                    d.behaviours.Remove(invalid);
                }
            }
        }

        public void DeleteFile()
        {
            string path = "./" + fileName + FILE_POSTFIX;
            Program.DebugToFile("Delete file: " + path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void Save()
        {
            Program.DebugToFile("Save Config: " + "./" + this.fileName + FILE_POSTFIX + " | device count: "+devices.Count);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Culture = System.Globalization.CultureInfo.InvariantCulture;
            settings.Formatting = Formatting.Indented;
            File.WriteAllText("./" + this.fileName + FILE_POSTFIX, JsonConvert.SerializeObject(this, settings), Encoding.UTF8);
        }

        #region STATIC

        private static Config p_config;
        public static Config Singleton
        {
            get
            {
                if (p_config == null)
                {
                    Load(DEFAULT_CONFIG_NAME);
                    FindAllConfigs();
                }
                return p_config;
            }
        }

        private static void Load(string fileName)
        {
            string path = "./" + fileName + FILE_POSTFIX;
            if (!File.Exists(path))
            {
                p_config = new Config();
                p_config.fileName = fileName;
                p_config.Save();
            }

            string data = File.ReadAllText(path);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Culture = System.Globalization.CultureInfo.InvariantCulture;
            p_config = JsonConvert.DeserializeObject<Config>(data, settings);
            p_config.fileName = fileName;

            p_config.InitDevices();
        }

        public static string[] GetConfigNames()
        {
            if (p_config == null)
            {
                Load(DEFAULT_CONFIG_NAME);
                FindAllConfigs();
            }
            return CONFIG_FILENAMES;
        }

        public static void AddConfigName(string s)
        {
            List<string> list = new List<string>(CONFIG_FILENAMES);
            list.Add(s);
            CONFIG_FILENAMES = list.ToArray();
        }

        public static void LoadNewConfig(string name)
        {
            Dictionary<string, Toy> toys = new Dictionary<string, Toy>(Mediator.activeToys);
            foreach (KeyValuePair<string, Toy> keyValue in toys) //remove all toys
                Mediator.RemoveToy(keyValue.Value);

            Load(name);

            foreach (KeyValuePair<string, Toy> keyValue in toys)//readd all toys with new cofig
            { 
                keyValue.Value.AtEndOfConstructor(); //forces values to be read from config again
                Mediator.AddToy(keyValue.Value);
            }
        }

        private static void FindAllConfigs()
        {
            List<string> files = new List<string>();
            if (Directory.Exists("./"))
            {
                foreach(string p in Directory.GetFiles("./"))
                {
                    if (p.EndsWith(FILE_POSTFIX) && p.EndsWith(".exe"+FILE_POSTFIX) == false)
                    {
                        files.Add(Path.GetFileNameWithoutExtension(p));
                    }
                }
            }
            CONFIG_FILENAMES = files.ToArray();
            for(int i = 0; i < CONFIG_FILENAMES.Length; i++)
            {
                if(CONFIG_FILENAMES[i] == DEFAULT_CONFIG_NAME)
                {
                    CONFIG_FILENAMES[i] = CONFIG_FILENAMES[0];
                    CONFIG_FILENAMES[0] = DEFAULT_CONFIG_NAME;
                    break;
                }
            }
        }

        #endregion
    }

    /**
     * Coordinates a rectangle in the sensor
     * */
    public struct InSensorCoordiantes
    {
        public int x;
        public int y;
        public InSensorCoordiantes(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public InSensorCoordiantes Add(int x, int y)
        {
            InSensorCoordiantes sens = new InSensorCoordiantes();
            sens.x = this.x + x;
            sens.y = this.y + y;
            return sens;
        }

        public int pixel_offset_x
        {
            get
            {
                return (int)(x * GameWindowReader.Singleton.RECTANGLE_ABSOLUTE_WIDTH);
            }
        }

        public int pixel_offset_y
        {
            get
            {
                return (int)(y * GameWindowReader.Singleton.RECTANGLE_ABSOLUTE_HEIGHT);
            }
        }

        public int GetXWithSensor(SensorCoordinates sensor)
        {
            return sensor.pixel_x + this.pixel_offset_x;
        }

        public int GetYWithSensor(SensorCoordinates sensor)
        {
            return sensor.pixel_y + this.pixel_offset_y;
        }
    }

    /**
     * Coordinates for a sensor positoon. with respect to sensor size
     * */
    public struct SensorCoordinates
    {
        public int x;
        public int y;
        public SensorCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public SensorCoordinates((int,int) pos)
        {
            this.x = pos.Item1;
            this.y = pos.Item2;
        }

        public int pixel_x
        {
            get
            {
                return (int)(x * GameWindowReader.Singleton.SENSOR_ABSOLUTE_WIDTH + GameWindowReader.Singleton.RECTANGLE_ABSOLUTE_TO_MID_OFFSET_X);
                //return (int)((float)x * GameWindowReader.Singleton.Capture.Width / Config.SENSOR_WIDTH + Config.DATA_RECTANGLE_WIDTH / 100f * GameWindowReader.Singleton.Capture.Width);
            }
        }

        public int pixel_y
        {
            get
            {
                return (int)(y * GameWindowReader.Singleton.SENSOR_ABSOLUTE_HEIGHT + GameWindowReader.Singleton.RECTANGLE_ABSOLUTE_TO_MID_OFFSET_Y);
            }
        }
    }
}
