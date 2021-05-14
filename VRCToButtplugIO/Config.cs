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
        public const float PIXEL_SCALE = 0.02f;

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
}
