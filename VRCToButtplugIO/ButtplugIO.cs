using Buttplug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCToyController
{
    class ButtplugIOAPI : ToyAPI
    {

        private ButtplugClient client;

        public static new async Task<ButtplugIOAPI> GetClient()
        {
            ButtplugIOAPI buttplugIOInterface = new ButtplugIOAPI();
            var connector = new ButtplugEmbeddedConnectorOptions();
            var client = new ButtplugClient("Example Client");
            buttplugIOInterface.client = client;

            await client.ConnectAsync(connector);
            Console.WriteLine("Connected!");

            void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
            {
                //add to ui
                var device = aArgs.Device;
                ButtplugToy toy = new ButtplugToy(device);
                Mediator.AddToy(toy);
            }
            client.DeviceAdded += HandleDeviceAdded;

            void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
            {
                //remove from ui
                Mediator.RemoveToy(aArgs.Device.Name);
            }
            client.DeviceRemoved += HandleDeviceRemoved;

            buttplugIOInterface.StartScanning();
            return buttplugIOInterface;
        }

        public bool is_scanning = false;
        public void StartScanning()
        {
            if (!is_scanning)
            {
                client.StartScanningAsync();
                is_scanning = true;
            }
        }

        public void StopScanning()
        {
            client.StopScanningAsync();
            is_scanning = false;


            Console.WriteLine(connectedDevices());
        }

        public override void Vibrate(Toy itoy, double[] strength)
        {
            foreach (double s in strength)
                if (s > 1.0f)
                    return;
            ButtplugToy toy = (ButtplugToy)itoy;
            if (toy.device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.VibrateCmd) == false) return;
            if (strength.Length == toy.device.AllowedMessages[ServerMessage.Types.MessageAttributeType.VibrateCmd].FeatureCount)
            {
                toy.device.SendVibrateCmd(strength);
            }
            else if (strength.Length > 0)
            {
                toy.device.SendVibrateCmd(strength[0]);
            }
            if (toy.lovenseType == LovenseToyType.max && strength.Length > 1)
            {
                toy.device.SendRawWriteCmd(Endpoint.Command, Encoding.ASCII.GetBytes("Air:Level:" + (strength[1] * 3) + ";"), false);
            }
        }

        public void TestLovense(string deviceName)
        {
            foreach (var device in client.Devices)
            {
                if (device.Name == deviceName)
                {
                    device.SendVibrateCmd(10);
                }
            }
        }

        public string connectedDevices()
        {
            string s = "";
            foreach (var device in client.Devices)
            {
                s += ($"{device.Name} supports these messages:");
                foreach (var msgInfo in device.AllowedMessages)
                {
                    s += ($"- {msgInfo.Key.ToString()}");
                    if (msgInfo.Value.FeatureCount != null)
                    {
                        s += ($" - Features: {msgInfo.Value.FeatureCount}");
                    }
                }
            }
            return s;
        }

        public async void Disconnect()
        {
            await client.DisconnectAsync();
        }

    }

    public enum LovenseToyType
    {
        none,any,max
    }

    public class ButtplugToy : Toy
    {
        public ButtplugClientDevice device;
        public LovenseToyType lovenseType;

        public ButtplugToy(ButtplugClientDevice device)
        {
            this.device = device;
            this.lovenseType = GetLovenseType(device);
            this.name = device.Name;
            if (device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.VibrateCmd))
                this.motorCount = (int)(device.AllowedMessages[ServerMessage.Types.MessageAttributeType.VibrateCmd].FeatureCount) + (lovenseType == LovenseToyType.max ? 1 : 0);
            else
                this.motorCount = 0;
            foreach (ToyAPI api in Mediator.toyAPIs)
            {
                if (api is ButtplugIOAPI)
                    toyAPI = api;
            }
        }

        public static LovenseToyType GetLovenseType(ButtplugClientDevice device)
        {
            if (device.Name.StartsWith("Lovense"))
            {
                if (device.Name.Contains("Lovense Max"))
                    return LovenseToyType.max;
                return LovenseToyType.any;
                //string result = await device.SendLovenseCmd("DeviceType;");
            }
            return LovenseToyType.none;
        }
    }
}
