using Buttplug.Client;
using Buttplug.Core.Logging;
using Buttplug.Core.Messages;
using Buttplug.Test;
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
            var connector = new ButtplugEmbeddedConnector("Example Server");
            var client = new ButtplugClient("Example Client",
                new ButtplugEmbeddedConnector("Example Server"));
            buttplugIOInterface.client = client;

            await client.ConnectAsync();
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
            Mediator.ui.scan.Text = "Stop scanning";

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
            if (strength.Length == toy.device.GetMessageAttributes<VibrateCmd>().FeatureCount)
            {
                toy.device.SendVibrateCmd(strength);
            }
            else if (strength.Length > 0)
            {
                toy.device.SendVibrateCmd(strength[0]);
            }
            if (toy.lovenseType == LovenseToyType.max && strength.Length > 1)
            {
                toy.device.SendLovenseCmd("Air:Level:" + (strength[1] * 3) + ";");
            }
        }

        public void TestLovense(string deviceName)
        {
            foreach (var device in client.Devices)
            {
                if (device.Name == deviceName)
                {
                    device.SendLovenseCmd("Vibrate:10;");
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
                    s += ($"- {msgInfo.Key.Name}");
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
            this.motorCount = (int)(device.GetMessageAttributes<VibrateCmd>().FeatureCount) + (lovenseType == LovenseToyType.max ? 1 : 0);
            foreach (ToyAPI api in Mediator.toyAPIs)
            {
                if (api is ButtplugIOAPI)
                    toyAPI = api;
            }
        }

        public static LovenseToyType GetLovenseType(ButtplugClientDevice device)
        {
            if (device.AllowedMessages.ContainsKey(typeof(LovenseCmd)))
            {
                if (device.Name.Contains("Max"))
                    return LovenseToyType.max;
                return LovenseToyType.any;
                //string result = await device.SendLovenseCmd("DeviceType;");
            }
            return LovenseToyType.none;
        }
    }
}
