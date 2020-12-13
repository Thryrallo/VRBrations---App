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
        const string PATH = @"./config.json";

        public string window_name = "VRChat";
        public float window_scale = 0.02f;
        public float update_rate = 100;
        public float threshold = 0.05f;
        public Device[] devices;


        private static Config p_config_in_file;
        public static Config config_in_file
        {
            get
            {
                if (p_config_in_file == null)
                    Load();
                return p_config_in_file;
            }
        }

        private static Config p_config;
        public static Config config
        {
            get
            {
                if (p_config == null)
                {
                    Load();
                    p_config = config_in_file;
                }
                return p_config;
            }
        }

        private static void Load()
        {
            if (!File.Exists(PATH))
            {
                p_config_in_file = new Config();
                p_config_in_file.Save();
            }

            string data = File.ReadAllText(PATH);
            p_config_in_file = JsonConvert.DeserializeObject<Config>(data);
        }

        public void Save()
        {
            File.WriteAllText(PATH, JsonConvert.SerializeObject(this), Encoding.UTF8);
        }
    }
}
