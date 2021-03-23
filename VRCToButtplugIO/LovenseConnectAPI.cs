using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRCToyController
{
    class LovenseConnectAPI : ToyAPI
    {

        public static new async Task<LovenseConnectAPI> GetClient()
        {
            return new LovenseConnectAPI();
        }

        private Dictionary<string, LovenseConnectDomain> domains = new Dictionary<string, LovenseConnectDomain>();

        private List<CustomLovenseConnectDomain> customDomains = new List<CustomLovenseConnectDomain>();

        private LovenseConnectAPI()
        {
            GetToys();
        }

        public void GetToys()
        {
            Task.Factory.StartNew(() =>
            {
                string data = Get("https://api.lovense.com/api/lan/getToys");
                Dictionary<string, LovenseConnectDomain> newDomains = JsonConvert.DeserializeObject<Dictionary<string, LovenseConnectDomain>>(data);

                IEnumerable<LovenseConnectToy> previouslyExistingToys = Mediator.activeToys.Values.Where(aT => aT is LovenseConnectToy).Select(t => t as LovenseConnectToy);
                foreach (KeyValuePair<string, LovenseConnectDomain> d in newDomains)
                {
                    HandleDomainToys(d.Key, d.Value, previouslyExistingToys);
                }
                foreach (CustomLovenseConnectDomain cD_Url in customDomains)
                {
                    data = Get(cD_Url.toysUrl);
                    LovenseConnectDomain domain = JsonConvert.DeserializeObject<LovenseConnectDomain>(data);
                    domain.toys = domain.data;
                    domain.domain = cD_Url.domain;
                    domain.httpPort = cD_Url.httpPort;
                    domain.isHttps = true;

                    HandleDomainToys(cD_Url.domain, domain, previouslyExistingToys);
                }
            });
        }

        private void HandleDomainToys(string domainId, LovenseConnectDomain domain, IEnumerable<LovenseConnectToy> previouslyExistingToys)
        {
            //if domains already exists check for new toys
            if (domains.ContainsKey(domainId))
            {
                foreach (LovenseConnectToy t in domain.toys.Values)
                {
                    bool isNewToy = Mediator.activeToys.Values.Where(aT => aT is LovenseConnectToy && (aT as LovenseConnectToy).id == t.id && (aT as LovenseConnectToy).domain.domain == domainId).Count() == 0;
                    if (isNewToy)
                    {
                        AddToy(t, domain);
                    }
                }
            }
            //else add all toys
            else
            {
                Program.DebugToFile("[LovenseConnect] New Domain: " + domainId);
                domains.Add(domainId, domain);
                foreach (LovenseConnectToy t in domain.toys.Values)
                {
                    AddToy(t, domain);
                }
            }
            //Remove old devices
            //Where domains is same and domain does not contain toy
            IEnumerable<LovenseConnectToy> oldToys = previouslyExistingToys.Where(t => t.domain.domain == domainId && domain.toys.ContainsKey(t.id) == false);
            foreach (LovenseConnectToy t in oldToys) Mediator.RemoveToy(domainId + "_" + t.id);
        }

        private void AddToy(LovenseConnectToy t, LovenseConnectDomain d)
        {
            
            if (Enum.TryParse<LovenseConnectToyType>(t.name.ToLower(), out t.type) == false)
                t.type = LovenseConnectToyType.none;
            Program.DebugToFile("[LovenseConnect] Add Toy: " + t.name + "," + t.id+ ", type: "+t.type);
            switch (t.type)
            {
                case LovenseConnectToyType.max:
                case LovenseConnectToyType.edge:
                    t.motorCount = 2;
                    break;
                default:
                    t.motorCount = 1;
                    break;
            }
            t.toyAPI = this;
            t.domain = d;
            t.vrcToys_id = d.domain + "_" + t.id;
            Mediator.AddToy(t);
        }

        public void ClearToys()
        {
            List<Toy> toRemove = new List<Toy>();
            foreach(Toy t in Mediator.activeToys.Values)
            {
                if (t is LovenseConnectToy)
                    toRemove.Add(t);
            }
            foreach (Toy t in toRemove)
                Mediator.RemoveToy(t.name);
        }

        private class LovenseConnectDomain
        {
            public string domain;
            public int httpPort;
            public bool isHttps;
            public Dictionary<string,LovenseConnectToy> toys;
            //in case of custom domain
            public Dictionary<string,LovenseConnectToy> data;

            private string p_url;
            public string url
            {
                get
                {
                    if(p_url == null)
                    {
                        p_url = (isHttps ? "https://" : "http://") + domain + ":" + httpPort;
                    }
                    return p_url;
                }
            }
        }
        private class LovenseConnectToy : Toy
        {
            public string id;
            public string nickName;
            public LovenseConnectToyType type;
            public LovenseConnectDomain domain;
        }

        private enum LovenseConnectToyType
        {
            none,nora, max,lush,hush,ambi,edge,domi,osci,diamo
        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private const string vibrateURL = "Vibrate";
        private const string vibrate1URL = "Vibrate1";
        private const string vibrate2URL = "Vibrate2";
        private const string rotateURL = "Rotate";
        private const string airURL = "AirAuto";
        private const string airInURL = "AirIn";
        private const string airOutURL = "AirOut";
        public override void Vibrate(Toy itoy, double[] strength)
        {
            foreach (double s in strength)
                if (s > 1.0f)
                    return;

            LovenseConnectToy toy = (LovenseConnectToy)itoy;
            if(toy.type == LovenseConnectToyType.max && strength.Length > 1)
            {
                LovenseGet(toy, vibrate1URL ,strength[0]);
                if (strength[1] > 0.5f)
                    LovenseGet(toy, airInURL ,0);
                else
                    LovenseGet(toy, airOutURL ,0);
            }
            else if(toy.type == LovenseConnectToyType.edge && strength.Length > 1)
            {
                LovenseGet(toy, vibrate1URL ,strength[0]);
                LovenseGet(toy, vibrate2URL ,strength[1]);
            }else if(toy.type == LovenseConnectToyType.nora && strength.Length > 1)
            {
                LovenseGet(toy, vibrateURL ,strength[0]);
                LovenseGet(toy, rotateURL ,strength[1]);
            }
            else
            {
                LovenseGet(toy, vibrateURL, strength[0]);
            }
        }

        private void LovenseGet(LovenseConnectToy t, string url, double speed)
        {
            string fullurl = t.domain.url + "/" + url + "?v=" + (int)(speed * 20 + 0.4f) + "&t=" + t.id;
            //Console.WriteLine("GET:: " + fullurl);
            try
            {
                Get(fullurl);
            }catch(Exception e)
            {
                System.IO.File.AppendAllText("./error.txt", "System.Net.WebException: The remote name could not be resolved: " + fullurl + "\n");
            }
        }

        public bool AddCustomURL(string url)
        {
            string regex = @"https:\/\/\d+-\d+-\d+-\d+\.lovense.club:\d+\/GetToys";
            if(Regex.IsMatch(url, regex))
            {
                CustomLovenseConnectDomain domain = new CustomLovenseConnectDomain();
                domain.toysUrl = url;
                string pattern = @"\d+-\d+-\d+-\d+.lovense\.club";
                domain.domain = Regex.Match(url, pattern).Value;
                pattern = @":\d+";
                domain.httpPort = int.Parse(Regex.Match(url, pattern).Value.Trim(':'));
                customDomains.Add(domain);
                GetToys();
                return true;
            }
            return false;
        }

        public override void SlowUpdate()
        {
            GetToys();
        }

        public override void UpdateBatteryIndicator(Toy iToy)
        {
            UpdateBatteryIndicatorAsync(iToy);
        }

        private struct LovenseBattery
        {
            public int data;
        }

        private struct CustomLovenseConnectDomain
        {
            public string domain;
            public int httpPort;
            public string toysUrl;
        }

        private void UpdateBatteryIndicatorAsync(Toy iToy)
        {
            LovenseConnectToy t = (LovenseConnectToy)iToy;
            string fullurl = t.domain.url + "/Battery?t="+t.id;
            string data = Get(fullurl);
            LovenseBattery battery = JsonConvert.DeserializeObject<LovenseBattery>(data);
            int level = battery.data;
            t.UpdateBatterUI(level);
        }
    }
}
