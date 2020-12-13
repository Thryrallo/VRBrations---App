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

        private Dictionary<string, LovenseConnectDomain> domains;

        private LovenseConnectAPI()
        {
            GetToys();
        }

        public void GetToys()
        {
            string data = Get("https://api.lovense.com/api/lan/getToys");
            Console.WriteLine("-----------Lovense Connect---------");
            domains = JsonConvert.DeserializeObject<Dictionary<string, LovenseConnectDomain>>(data);
            foreach (KeyValuePair<string, LovenseConnectDomain> d in domains)
            {
                Console.WriteLine("Domain: " + d.Key);
                foreach (LovenseConnectToy t in d.Value.toys.Values)
                {
                    Console.WriteLine("Toy: " + t.name + "," + t.id);
                    if (Enum.TryParse<LovenseConnectToyType>(t.name, out t.type) == false)
                        t.type = LovenseConnectToyType.none;
                    Console.WriteLine("Type: " + t.type);
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
                    t.domain = d.Value;
                    Mediator.AddToy(t);
                }
            }
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
            nora,max,lush,hush,ambi,edge,domi,osci,none
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
            //throw new NotImplementedException();
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
            string fullurl = "http://" + t.domain.domain + ":" + t.domain.httpPort + "/" + url + "?v=" + (speed * 20) + "&t=" + t.id;
            //Console.WriteLine("GET:: " + fullurl);
            Get(fullurl);
        }

    }
}
