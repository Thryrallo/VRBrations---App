using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static VRCToyController.Toy;

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
            //move id to lovenseId
            foreach (LovenseConnectToy t in domain.toys.Values) t.lovenseId = t.id;
            //if domains already exists check for new toys
            if (domains.ContainsKey(domainId))
            {
                foreach (LovenseConnectToy t in domain.toys.Values)
                {
                    bool isNewToy = Mediator.activeToys.Values.Where(aT => aT is LovenseConnectToy && (aT as LovenseConnectToy).lovenseId == t.lovenseId && (aT as LovenseConnectToy).domain.domain == domainId).Count() == 0;
                    if (isNewToy && t.status == 1)
                    {
                        AddToy(t, domain);
                    }
                }
            }
            //else add all toys
            else
            {
                domains.Add(domainId, domain);
                Program.DebugToFile("[LovenseConnect] New Domain: " + domainId);
                foreach (LovenseConnectToy t in domain.toys.Values)
                {

                    if(t.status == 1)
                        AddToy(t, domain);
                }
            }
            //Remove old devices
            //Where domains is same and domain does not contain toy
            IEnumerable<LovenseConnectToy> oldToys = previouslyExistingToys.Where(t => t.domain.domain == domainId && domain.toys.ContainsKey(t.lovenseId) == false);
            foreach (LovenseConnectToy t in oldToys) Mediator.ToyDisconnected(t.id);
        }

        private void AddToy(LovenseConnectToy t, LovenseConnectDomain d)
        {
            Program.DebugToFile("[LovenseConnect] Add Toy: " + t.name + "," + t.lovenseId + ", type: " + t.type);
            t.Constructor(d, this);
            Mediator.ToyConnected(t);
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
                Mediator.ToyDisconnected(t.name);
        }

        protected class LovenseConnectDomain
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

        protected enum LovenseConnectToyType
        {
            none,nora, max,lush,hush,ambi,edge,domi,osci,diamo
        }

        public static string Get(string uri)
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

        protected static void LovenseGet(LovenseConnectToy t, string url, double speed)
        {
            string fullurl = t.domain.url + "/" + url + "?v=" + (int)(speed * 20 + 0.4f) + "&t=" + t.lovenseId;
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
            public string data;
            public int code;
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
            string fullurl = t.domain.url + "/Battery?t="+t.lovenseId;
            string data = Get(fullurl);
            LovenseBattery battery = JsonConvert.DeserializeObject<LovenseBattery>(data);
            if(battery.code == 200)
            {
                t.SetBatterLevel(int.Parse(battery.data));
            }
        }

        protected class LovenseConnectToy : Toy
        {
            public string lovenseId;
            public string nickName;
            public int status;
            public LovenseConnectToyType type;
            public LovenseConnectDomain domain;

            public LovenseConnectToy() : base()
            {
                
            }

            public void Constructor(LovenseConnectDomain d, LovenseConnectAPI api)
            {
                if (Enum.TryParse<LovenseConnectToyType>(name.ToLower(), out type) == false)
                    type = LovenseConnectToyType.none;
                featureCount[ToyFeatureType.Vibrate] = 1;
                switch (type)
                {
                    case LovenseConnectToyType.max:
                        featureCount[ToyFeatureType.Air] = 1;
                        break;
                    case LovenseConnectToyType.nora:
                        featureCount[ToyFeatureType.Rotate] = 1;
                        break;
                    case LovenseConnectToyType.edge:
                        featureCount[ToyFeatureType.Vibrate] = 2;
                        break;
                }
                UpdateTotalFeatureCount();
                toyAPI = api;
                domain = d;
                id = d.domain + "_" + id;
                AtEndOfConstructor();
            }

            private const string vibrateURL = "Vibrate";
            private const string vibrate1URL = "Vibrate1";
            private const string vibrate2URL = "Vibrate2";
            private const string rotateURL = "Rotate";
            private const string airURL = "AirAuto";
            private const string airInURL = "AirIn";
            private const string airOutURL = "AirOut";

            public override void Vibrate(IEnumerable<double> strength)
            {
                if (strength.Count() > 1)
                {
                    LovenseGet(this, vibrate1URL, strength.First());
                    LovenseGet(this, vibrate2URL, strength.Skip(1).First());

                }
                else if (strength.Count() > 0)
                {
                    LovenseGet(this, vibrateURL, strength.First());
                }
            }

            public override void Rotate(IEnumerable<double> strength)
            {
                if (strength.Count() > 0)
                {
                    LovenseGet(this, rotateURL, strength.First());
                }
            }

            public override void Air(IEnumerable<double> strength)
            {
                if (strength.Count() > 0)
                {
                    double s = strength.First();
                    if (s > 0.5f)
                        LovenseGet(this, airInURL, 0);
                    else
                        LovenseGet(this, airOutURL, 0);
                }
            }
        }
    }
}
