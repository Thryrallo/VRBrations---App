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
        const string DEFAULT_CONFIG_NAME = "default";
        const string FILE_POSTFIX = @".config";
        private string fileName;
        private static string[] CONFIG_FILENAMES; 

        public string window_name = "VRChat";
        public float window_scale = 0.02f;
        public float update_rate = 100;
        public float threshold = 0.05f;
        public Device[] devices;

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
                Mediator.RemoveToy(keyValue.Value.vrcToys_id);

            Load(name);

            foreach (KeyValuePair<string, Toy> keyValue in toys) //readd all toys with new cofig
                Mediator.AddToy(keyValue.Value);
        }

        public void DeleteFile()
        {
            //string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + fileName + FILE_POSTFIX;
            string path = "./" + fileName + FILE_POSTFIX;
            Program.DebugToFile("Delete file: " + path);
            if (File.Exists(path))
            {
                File.Delete(path);
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
            p_config = JsonConvert.DeserializeObject<Config>(data,settings);
            p_config.fileName = fileName;

            if (p_config.devices == null)
                p_config.devices = new Device[0];
        }

        public void Save()
        {
            Program.DebugToFile("Save Config: " + "./" + this.fileName + FILE_POSTFIX);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Culture = System.Globalization.CultureInfo.InvariantCulture;
            File.WriteAllText("./" + this.fileName + FILE_POSTFIX, JsonConvert.SerializeObject(this,settings), Encoding.UTF8);
        }
    }
}
