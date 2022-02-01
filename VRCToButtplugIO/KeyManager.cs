using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace VRCToyController
{
    class KeyManager
    {
        const string KEY_FILE_PATH = "./key.txt";
        private static string key;
        private static string eventKey = "";

        static VRCLogReader logReader = new VRCLogReader();

        private static KeyStatus p_status = KeyStatus.NONE;
        private static WorldStatus p_status_world = WorldStatus.NONE;
        private static string p_world_name = "";
        public static KeyStatus status{
            get
            {
                return p_status;
            }
        }

        static WorldStatus status_world
        {
            get
            {
                return p_status_world;
            }
            set
            {
                p_status_world = value;
            }
        }

        private static long lastVerification = 0;
        private static long lastWorldVerification = 0;
        const long UPDATE_RATE = 10*60*1000;
        const long WORLD_UPDATE_RATE = 60*1000;

        private static readonly HttpClient client = new HttpClient();

        public static bool LoadKey()
        {
            if (!File.Exists(KEY_FILE_PATH))
            {
                return false;
            }
            key = File.ReadAllText(KEY_FILE_PATH);
            return true;
        }

        public static bool LoadKey(string key)
        {
            KeyManager.key = key;
            File.WriteAllText(KEY_FILE_PATH, key, Encoding.UTF8);
            return true;
        }

        private static bool closing = false;
        public static async Task FreeKeyAsync()
        {
            closing = true;
            if(status == KeyStatus.VALID)
                await SendFreeRequest();
        }

        private static bool popup = false;
        public static bool VerifyKey()
        {
            HandleKeyVerification();
            return status == KeyStatus.VALID || status_world == WorldStatus.ACTIVE;
        }

        private static void Popup(string s)
        {
            if (popup)
                return;
            Console.WriteLine("popup: " + s);
            System.Windows.Forms.MessageBox.Show(s);
            KeyUIMessage(s, false);
            popup = true;
        }

        private static void KeyUIMessage(string s, bool good)
        {
            Mediator.ui.Invoke((Action)delegate ()
            {
                Mediator.ui.warning.Text = s;
                Mediator.ui.warning.Visible = true;
                Mediator.ui.warning.BackColor = good ? System.Drawing.Color.White : System.Drawing.Color.Red;
                Mediator.ui.warning.ForeColor = good ? System.Drawing.Color.Green : System.Drawing.Color.White;
            });
        }

        private static void KeyUIMessageHide()
        {
            Mediator.ui.Invoke((Action)delegate ()
            {
                Mediator.ui.warning.Visible = false;
            });
        }

        static long lastStringUpdate = 0;
        public static void HandleKeyVerification()
        {
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if(status_world != WorldStatus.ACTIVE && time - lastVerification > UPDATE_RATE && !closing)
            {
                lastVerification = time;
                Task.Factory.StartNew(VerifyKeyAsync);
            }
            if (time - lastWorldVerification > WORLD_UPDATE_RATE && !closing)
            {
                lastWorldVerification = time;
                Task.Factory.StartNew(VerifyWorld);
            }

            if(time - lastStringUpdate > 5000 && Mediator.IsUICreated())
            {
                lastStringUpdate = time;

                if (status_world == WorldStatus.ACTIVE)
                {
                    KeyUIMessage("VRbrations Activation via: " + p_world_name, true);
                }
                else
                {
                    if (status == KeyStatus.IN_USE)
                        Popup("Key is already in use.");
                    else if (status == KeyStatus.TRIAL_OVER)
                        Popup("Your trial has expired. Please upgrade your key to a full version.");
                    else if (status == KeyStatus.INVALID)
                        Popup("Your key is invalid.");
                    else if (status == KeyStatus.NONE)
                        KeyUIMessage("World Activation lost. Closing App soon.", false);
                    else if(status == KeyStatus.VALID)
                        KeyUIMessageHide();

                }
            }
        }

        public static async Task<KeyStatus> VerifyKeyAsync()
        {
            if (string.IsNullOrEmpty(key)) return KeyStatus.NONE;

            var values = new Dictionary<string, string>
            {
                { "key", key },
                { "eventkey", eventKey },
                { "version", Config.VERSION }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.vrbrations.com/app/veryfyKey.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            ValidationResult result = JsonConvert.DeserializeObject<ValidationResult>(responseString);
            p_status = result.status;
            return status;
        }

        public static async Task<WorldStatus> VerifyWorld()
        {
            string worldId = logReader.GetRecentWorld();
            if (worldId == null) return WorldStatus.NONE;

            var values = new Dictionary<string, string>
            {
                { "worldId", worldId }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.vrbrations.com/app/veryfyWorld.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            WorldResult result = JsonConvert.DeserializeObject<WorldResult>(responseString);
            status_world = result.status;
            p_world_name = result.name;
            return status_world;
        }

        private static async Task SendFreeRequest()
        {
            var values = new Dictionary<string, string>
            {
                { "key", key },
                { "eventkey", eventKey },
                { "version", Config.VERSION }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.vrbrations.com/app/free.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

    }

    public class ValidationResult
    {
        public KeyStatus status;
    }

    public enum KeyStatus
    {
        NONE = 0, VALID = 1, IN_USE=2, TRIAL_OVER= 3, INVALID = 4
    }

    public class WorldResult
    {
        public WorldStatus status;
        public string name;
    }

    public enum WorldStatus
    {
        NONE = 0, ACTIVE = 1, INACTIVE = 2
    }
}
