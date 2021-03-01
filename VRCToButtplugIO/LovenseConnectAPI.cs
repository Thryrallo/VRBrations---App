using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        private LovenseConnectAPI()
        {
            GetToys();
        }

        public void GetToys()
        {
            string data = Get("https://api.lovense.com/api/lan/getToys");
            Dictionary<string, LovenseConnectDomain>  newDomains = JsonConvert.DeserializeObject<Dictionary<string, LovenseConnectDomain>>(data);
            foreach (KeyValuePair<string, LovenseConnectDomain> d in newDomains)
            {
                //if domains already exists check for new toys
                if (domains.ContainsKey(d.Key))
                {
                    foreach (LovenseConnectToy t in d.Value.toys.Values)
                    {
                        bool isNewToy = Mediator.activeToys.Values.Where(aT => aT is LovenseConnectToy && (aT as LovenseConnectToy).id == t.id).Count() == 0;
                        if (isNewToy)
                        {
                            AddToy(t, d.Value);
                        }
                    }
                }
                //else add all toys
                else
                {
                    Program.DebugToFile("[LovenseConnect] New Domain: "+d.Key);
                    domains.Add(d.Key, d.Value);
                    foreach (LovenseConnectToy t in d.Value.toys.Values)
                    {
                        AddToy(t, d.Value);
                    }
                }
            }
        }

        private void AddToy(LovenseConnectToy t, LovenseConnectDomain d)
        {
            
            if (Enum.TryParse<LovenseConnectToyType>(t.name, out t.type) == false)
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
            public Dictionary<string,LovenseConnectToy> toys;
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
            string fullurl = "http://" + t.domain.domain + ":" + t.domain.httpPort + "/" + url + "?v=" + (int)(speed * 20 + 0.4f) + "&t=" + t.id;
            //Console.WriteLine("GET:: " + fullurl);
            try
            {
                Get(fullurl);
            }catch(Exception e)
            {
                System.IO.File.AppendAllText("./error.txt", "System.Net.WebException: The remote name could not be resolved: " + fullurl + "\n");
            }
        }

        public override void SlowUpdate()
        {
            GetToys();
        }
    }
}
