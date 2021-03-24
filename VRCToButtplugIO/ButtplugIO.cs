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
using static VRCToyController.Toy;

namespace VRCToyController
{
    class ButtplugIOAPI : ToyAPI
    {

        private ButtplugClient client;

        public static new async Task<ButtplugIOAPI> GetClient()
        {
            ButtplugIOAPI buttplugIOInterface = new ButtplugIOAPI();
            var connector = new ButtplugEmbeddedConnectorOptions();
            connector.AllowRawMessages = true;
            var client = new ButtplugClient("Client");
            buttplugIOInterface.client = client;

            await client.ConnectAsync(connector);
            Program.DebugToFile("[Bluetooth] Connected to Buttplug.IO Client");

            void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
            {
                //add to ui
                var device = aArgs.Device;
                Program.DebugToFile("[Bluetooth] New device: "+device.Name);
                ButtplugToy toy = new ButtplugToy(device);
                toy.vrcToys_id = GetId(device);
                Program.DebugToFile("[Bluetooth] Add device to ui: " + device.Name);
                Mediator.AddToy(toy);
                //Program.DebugToFile(buttplugIOInterface.connectedDevices());
            }
            client.DeviceAdded += HandleDeviceAdded;

            void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
            {
                //remove from ui
                Mediator.RemoveToy(GetId(aArgs.Device));
                Program.DebugToFile("[Bluetooth] Removed device: "+ aArgs.Device.Name);
            }
            client.DeviceRemoved += HandleDeviceRemoved;

            void HandleError(object aObj, ButtplugExceptionEventArgs aArgs)
            {
                Program.DebugToFile("[Bluetooth Error] " + aArgs.Exception.ToString());
            }
            client.ErrorReceived += HandleError;

            buttplugIOInterface.StartScanning();
            return buttplugIOInterface;
        }

        private static string GetId(ButtplugClientDevice device)
        {
            return "BP_"+ device.Name + "_" + device.Index;
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

        public override void SlowUpdate()
        {
            
        }

        public override void UpdateBatteryIndicator(Toy iToy)
        {
            _ = UpdateBatteryIndicatorAsync(iToy);
        }

        private async Task UpdateBatteryIndicatorAsync(Toy iToy)
        {
            ButtplugToy toy = (ButtplugToy)iToy;
            //this causes problems if the toy is disconnected
            double level = await toy.device.SendBatteryLevelCmd();
            toy.UpdateBatterUI((int)(level * 100));
        }
    }

    public enum LovenseToyType
    {
        none,any,max,nora
    }

    public class ButtplugToy : Toy
    {
        public ButtplugClientDevice device;
        public LovenseToyType lovenseType;

        public ButtplugToy(ButtplugClientDevice device) : base()
        {
            this.device = device;
            this.lovenseType = GetLovenseType(device);
            this.name = device.Name;
            if (device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.VibrateCmd))
                this.featureCount[ToyFeatureType.Vibrate] = (int)device.AllowedMessages[ServerMessage.Types.MessageAttributeType.VibrateCmd].FeatureCount;
            if (device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.RotateCmd))
                this.featureCount[ToyFeatureType.Rotate] = (int)device.AllowedMessages[ServerMessage.Types.MessageAttributeType.RotateCmd].FeatureCount;
            if (lovenseType == LovenseToyType.max)
                this.featureCount[ToyFeatureType.Air] = 1;
            this.UpdateTotalFeatureCount();
            foreach (ToyAPI api in Mediator.toyAPIs)
            {
                if (api is ButtplugIOAPI)
                    toyAPI = api;
            }
        }

        public static LovenseToyType GetLovenseType(ButtplugClientDevice device)
        {
            string name = device.Name.ToLower();
            if (name.StartsWith("lovense"))
            {
                if (name.Contains("lovense max"))
                    return LovenseToyType.max;
                else if (name.Contains("lovense nora"))
                    return LovenseToyType.nora;
                return LovenseToyType.any;
                //string result = await device.SendLovenseCmd("DeviceType;");
            }
            return LovenseToyType.none;
        }

        public override void Vibrate(IEnumerable<double> strength)
        {
            if(strength.Count() == this.featureCount[ToyFeatureType.Vibrate])
            {
                this.device.SendVibrateCmd(strength);
            }
            else if(strength.Count() > 0)
            {
                this.device.SendVibrateCmd(strength.First());
            }
        }

        public override void Rotate(IEnumerable<double> strength)
        {
            if (strength.Count() == this.featureCount[ToyFeatureType.Rotate])
            {
                this.device.SendRotateCmd(strength.Select(v => (v,true)));
            }
            else if (strength.Count() > 0)
            {
                this.device.SendRotateCmd(strength.First(),true);
            }
        }

        public override void Air(IEnumerable<double> strength)
        {
            if (strength.Count() == 1 && this.lovenseType != LovenseToyType.none)
            {
                Console.WriteLine("Air:Level:" + (strength.First() * 3) + ";");
                this.device.SendRawWriteCmd(Endpoint.Tx, Encoding.ASCII.GetBytes("Air:Level:" + (strength.First() * 3) + ";"), false);
            }
        }
    }
}
