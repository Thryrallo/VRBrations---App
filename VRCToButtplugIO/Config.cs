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

        //This is also in vrbrations.cginc
        public const float PIXEL_WIDTH_SCREEN_RELATIVE = 0.005f;
        public const float PIXEL_HEIGHT_SCREEN_RELATIVE = 0.005f;

        public const int SENSOR_PIXELS_X = 4;
        public const int SENSOR_PIXELS_Y = 5;

        //================================

        public const int SENSOR_COUNT_X = 40;
        public const int SENSOR_COUNT_Y = 1;

        public const float SENSOR_WIDTH = PIXEL_WIDTH_SCREEN_RELATIVE * SENSOR_PIXELS_X;
        public const float SENSOR_HEIGHT = PIXEL_HEIGHT_SCREEN_RELATIVE * SENSOR_PIXELS_Y;

        public const float VRBATIONS_WIDTH = SENSOR_COUNT_X * SENSOR_WIDTH;
        public const float VRBATIONS_HEIGHT = SENSOR_COUNT_Y * SENSOR_HEIGHT;

        public const float PIXEL_WIDTH = PIXEL_WIDTH_SCREEN_RELATIVE / VRBATIONS_WIDTH;
        public const float PIXEL_HEIGHT = PIXEL_HEIGHT_SCREEN_RELATIVE / VRBATIONS_HEIGHT;

        public const int CHECK_VALUE_SHORT = 175;

        //Coords for exist check
        public static readonly InSensorCoordiantes COORDS_CHECK_VALUE = new InSensorCoordiantes(0, 0);
        public static readonly InSensorCoordiantes COORDS_CHECK_VALUE_2 = new InSensorCoordiantes(1, 0);

        //Coords for audiolink
        public static readonly InSensorCoordiantes COORDS_AUDIOLINK = new InSensorCoordiantes(0, 1);
        public static readonly InSensorCoordiantes COORDS_AUDIOLINK_EXISITS = new InSensorCoordiantes(2, 0);

        //Coords for sensor data
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_DEPTH = new InSensorCoordiantes(0, 1);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_WIDTH = new InSensorCoordiantes(1, 1);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_X =     new InSensorCoordiantes(2, 1);
        public static readonly InSensorCoordiantes COORDS_SENSOR_DATA_Y =     new InSensorCoordiantes(3, 1);

        public static readonly InSensorCoordiantes COORDS_SENSOR_NAME = new InSensorCoordiantes(0, 3);

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

        public float GetXWithSensor(int sensorX)
        {
            return (sensorX + (x + 0.5f) / Config.SENSOR_PIXELS_X) / Config.SENSOR_COUNT_X;
        }

        public float GetYWithSensor(int sensorY)
        {
            return (sensorY + (y + 0.5f) / Config.SENSOR_PIXELS_Y) / Config.SENSOR_COUNT_Y;
        }
    }
}
