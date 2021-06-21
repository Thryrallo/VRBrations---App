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

        private static KeyStatus p_status = KeyStatus.NONE;
        public static KeyStatus status{
            get
            {
                return p_status;
            }
        }
        private static long lastVerification = 0;
        const long UPDATE_RATE = 10*60*1000;

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
            if (status == KeyStatus.IN_USE)
                Popup("Key is already in use.");
            else if(status == KeyStatus.TRIAL_OVER)
                Popup("Your trial has expired. Please upgrade your key to a full version.");
            else if(status == KeyStatus.INVALID)
                Popup("Your key is invalid.");
            return status == KeyStatus.VALID;
        }

        private static void Popup(string s)
        {
            if (popup)
                return;
            Console.WriteLine("popup: " + s);
            System.Windows.Forms.MessageBox.Show(s);
            Mediator.ui.Invoke((Action)delegate ()
            {
                Mediator.ui.warning.Text = s;
                Mediator.ui.warning.Visible = true;
            });
            popup = true;
        }

        public static void HandleKeyVerification()
        {
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if(time-lastVerification > UPDATE_RATE && !closing)
            {
                Task.Factory.StartNew(VerifyKeyAsync);
            }
        }

        public static async Task<KeyStatus> VerifyKeyAsync()
        {
            var values = new Dictionary<string, string>
            {
                { "key", key }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.vrbrations.com/app/veryfyKey.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            ValidationResult result = JsonConvert.DeserializeObject<ValidationResult>(responseString);
            p_status = result.status;
            lastVerification = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return status;
        }

        private static async Task SendFreeRequest()
        {
            var values = new Dictionary<string, string>
            {
                { "key", key }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.vrbrations.com/app/free.php", content);

            var responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
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
}
