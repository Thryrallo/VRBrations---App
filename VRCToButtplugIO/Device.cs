using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public class DeviceData
    {

        public DeviceData()
        {

        }

        public DeviceData(Toy toy)
        {
            behaviours = new List<BehaviourData>();
            id = toy.id;
            features = toy.totalFeatureCount;
            for (int i = 0; i < toy.totalFeatureCount; i++)
            {
                BehaviourData newBehaviour = new BehaviourData();
                newBehaviour.input_pos = new int[] { i, 0 };
                newBehaviour.feature = i;
                behaviours.Add(newBehaviour);
            }
        }

        public string id;
        public int features = 0;
        public List<BehaviourData> behaviours;

        public BehaviourData AddBehaviour(Toy t)
        {
            BehaviourData newBehaviour = new BehaviourData();
            behaviours.Add(newBehaviour);
            Config.Singleton.Save();
            return newBehaviour;
        }

        public void RemoveBehaviour(BehaviourData behaviour)
        {
            behaviours.Remove(behaviour);
            Config.Singleton.Save();
        }
    }

    public class BehaviourData
    {
        public CalulcationType type = CalulcationType.VOLUME;
        public int feature = 0;
        public int[] input_pos = new int[] { 0, 0 };
        public float volume_width = 0.5f;
        public float volume_depth = 0.5f;

        public float thrusting_acceleration = 0.1f;
        public float thrusting_speed_scale = 1;
        public float thrusting_depth_scale = 1;

        public float rubbing_acceleration = 0.1f;
        public float rubbing_scale = 1;

        public AudioLinkChannel audioLink_channel;
        public float audioLink_scale = 1;

        public float max = 1;

        public void SetFeature(int max)
        {
            this.feature = Math.Max(0, max);
            Config.Singleton.Save();
        }

        public void SetInputPos(int[] pos)
        {
            if (pos.Length < 2) return;
            input_pos[0] = Math.Min(10, Math.Max(0, pos[0]));
            input_pos[1] = Math.Min(10, Math.Max(0, pos[1]));
            Config.Singleton.Save();
        }

        public void SetCalculationType(CalulcationType calulcationType)
        {
            this.type = calulcationType;
            Config.Singleton.Save();
        }

        public void SetMax(float max)
        {
            this.max = Math.Min(1, Math.Max(0, max));
            Config.Singleton.Save();
        }

        public void SetVolume(float w, float d)
        {
            volume_width = w;
            volume_depth = d;
            Config.Singleton.Save();
        }

        public void SetThrusting(float a, float s, float d)
        {
            thrusting_acceleration = a;
            thrusting_speed_scale = s;
            thrusting_depth_scale = d;
            Config.Singleton.Save();
        }

        public void SetRubbing(float a , float s)
        {
            rubbing_acceleration = a;
            rubbing_scale = s;
            Config.Singleton.Save();
        }

        public void SetAudioLink(AudioLinkChannel c, float s)
        {
            audioLink_channel = c;
            audioLink_scale = s;
            Config.Singleton.Save();
        }

        //used during runtime only
        private BehaviourStrengthData runtimeData = new BehaviourStrengthData();
        public BehaviourStrengthData GetRuntimeData()
        {
            return runtimeData;
        }

        public float CalculateStrength(float[] audioLinkData)
        {
            if (type == CalulcationType.VOLUME)
            {
                return runtimeData.CalculateVolume(volume_width, volume_depth);
            }
            else if (type == CalulcationType.THRUSTING)
            {
                return runtimeData.CalculateThrusting(thrusting_acceleration, thrusting_speed_scale, thrusting_depth_scale);
            }
            else if (type == CalulcationType.RUBBING)
            {
                return runtimeData.CalculateRubbing(thrusting_acceleration, thrusting_speed_scale);
            }else if( type == CalulcationType.AudioLink)
            {
                return runtimeData.CalculateAudioLink(audioLink_scale, audioLink_channel, audioLinkData);
            }
            return 0;
        }
    }

    public class BehaviourStrengthData
    {
        public bool wasFound = true;

        public float avgX = 0;
        public float avgY = 0;
        public float depth = 0;
        public float width = 0;

        public float rubSpeed = 0;
        public float thrustSpeed = 0;

        public float lastAvgX = 0;
        public float lastAvgY = 0;
        public float lastDepth = 0;
        public float lastWidth = 0;

        public float CalculateVolume(float widthScale, float depthScale)
        {
            return widthScale * width + depthScale * depth;
        }

        public float CalculateThrusting(float accScale, float speedScale, float depthScale)
        {
            float deltaDepth = Math.Abs(depth - lastDepth);
            thrustSpeed = Math.Min(1, thrustSpeed + accScale);
            if (deltaDepth == 0)
                thrustSpeed = Math.Max(0, thrustSpeed - accScale);

            lastDepth = depth;
            return (depth * depthScale + deltaDepth * speedScale) * thrustSpeed;
        }

        public float CalculateRubbing(float accScale, float speedScale)
        {
            float distance = (float)(Math.Pow(lastAvgX - avgX, 2) + Math.Pow(lastAvgY - avgY, 2)) * 80;
            rubSpeed = Math.Min(1, rubSpeed + accScale);

            if (distance == 0)
                rubSpeed = Math.Max(0, rubSpeed - accScale * 2);

            lastAvgX = avgX;
            lastAvgY = avgY;
            return speedScale * Math.Min(1, distance) * rubSpeed;
        }

        public float CalculateAudioLink(float scale, AudioLinkChannel channel, float[] audioLinkData)
        {
            if ((int)channel < audioLinkData.Length) return audioLinkData[(int)channel] * scale;
            return 0;
        }
    }

    public enum AudioLinkChannel
    {
        Low,LowMid,Mid,Trebble
    }

    public enum CalulcationType
    {
        VOLUME, THRUSTING, RUBBING, AudioLink
    }
}
