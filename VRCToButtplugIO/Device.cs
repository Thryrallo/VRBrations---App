using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public class Device
    {

        public Device()
        {
            device_params = new DeviceParams[0];
        }

        public string device_name;
        public int motors = 0;
        public DeviceParams[] device_params;
    }

    public class DeviceParams
    {
        public CalulcationType type = CalulcationType.VOLUME;
        public int motor = 0;
        public int[] input_pos = new int[] { 1, 1 };
        public float volume_width = 0.5f;
        public float volume_depth = 0.5f;

        public float thrusting_acceleration = 0.1f;
        public float thrusting_speed_scale = 1;
        public float thrusting_depth_scale = 1;

        public float rubbing_acceleration = 0.1f;
        public float rubbing_scale = 1;

        public float max = 1;

        public DeviceParamsUI ui;
    }

    public enum CalulcationType
    {
        VOLUME, THRUSTING, RUBBING
    }
}
