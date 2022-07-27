using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRCToyController;

namespace VRbrations
{
    internal class OSCInterface
    {
        public static Dictionary<string, float> ContactValues = new Dictionary<string, float>();
        public static Dictionary<string, (float rootRoot, float rootFwd, float backRoot)> PenetratorValues = new Dictionary<string, (float rootRoot, float rootFwd, float backRoot)>();
        public static Dictionary<string, (float depth, float width1, float width2)> OrificeValues = new Dictionary<string, (float depth, float width1, float width2)>();
        private static int s_ContactIndex = 0;
        
        //Not a cool osc implementation, but man the other libary (CoreOSC) didnt even recieve anything
        public static void ListenTask()
        {
            HandleOscPacket cb = delegate (OscPacket packet)
            {
                if (packet is OscMessage) HandleOSCMessage(packet as OscMessage);
                else if(packet is OscBundle)
                {
                    foreach (OscMessage message in ((OscBundle)packet).Messages)
                    {
                        HandleOSCMessage(message);
                    }
                }
                
            };

            var listener = new UDPListener(9001, cb);
        }

        static void HandleOSCMessage(OscMessage message)
        {
            if (message != null)
            {
                if (message.Address == "/avatar/change")
                {
                    RemoveContactsFromLists();
                    s_ContactIndex = 0;
                    ContactValues.Clear();
                    PenetratorValues.Clear();
                }
                else if (message.Arguments.Count > 0 && message.Arguments.First() is float)
                {
                    if (message.Address.StartsWith("/avatar/parameters/TPS_Internal/Pen/"))
                    {
                        string[] parts = message.Address.Substring(36).Split('/');
                        parts[0] = "Penetrator: " + parts[0];
                        (float rootRoot, float rootFwd, float backRoot) values = (0, 0, 0);
                        if (PenetratorValues.ContainsKey(parts[0])) values = PenetratorValues[parts[0]];
                        else Mediator.SetSensorActive(parts[0], -2 - (s_ContactIndex++), 0, Mediator.SensorType.OSC);
                        if (parts[1] == "RootRoot") values.rootRoot = (float)message.Arguments.First();
                        if (parts[1] == "RootForw") values.rootFwd = (float)message.Arguments.First();
                        if (parts[1] == "BackRoot") values.backRoot = (float)message.Arguments.First();
                        PenetratorValues[parts[0]] = values;
                    }
                    else if (message.Address.StartsWith("/avatar/parameters/TPS_Internal/Orf/"))
                    {
                        string[] parts = message.Address.Substring(36).Split('/');
                        parts[0] = "Orifice: " + parts[0];
                        (float depth, float width1, float width2) values = (0, 0, 0);
                        if (OrificeValues.ContainsKey(parts[0])) values = OrificeValues[parts[0]];
                        else Mediator.SetSensorActive(parts[0], -2 - (s_ContactIndex++), 0, Mediator.SensorType.OSC);
                        if (parts[1] == "Depth_In") values.depth = (float)message.Arguments.First();
                        if (parts[1] == "Width1_In") values.width1 = (float)message.Arguments.First();
                        if (parts[1] == "Width2_In") values.width2 = (float)message.Arguments.First();
                        OrificeValues[parts[0]] = values;
                    }
                }
            }
        }

        static void RemoveContactsFromLists()
        {
            foreach(var name in PenetratorValues.Keys)
            {
                Mediator.SetSensorInactive(name);
            }
            foreach (var name in OrificeValues.Keys)
            {
                Mediator.SetSensorInactive(name);
            }
        }
    }
}
