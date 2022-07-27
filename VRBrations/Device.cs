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
            removedSensors = new HashSet<string>();
            id = toy.id;
            features = toy.totalFeatureCount;
            /*for (int i = 0; i < toy.totalFeatureCount; i++)
            {
                BehaviourData newBehaviour = new BehaviourData();
                newBehaviour.feature = i;
                behaviours.Add(newBehaviour);
            }*/
        }

        public string id;
        public int features = 0;
        public List<BehaviourData> behaviours;
        public HashSet<string> removedSensors;

        public BehaviourData AddBehaviour(Toy t, string sensorName)
        {
            BehaviourData newBehaviour = new BehaviourData();
            newBehaviour.SetSensorName(sensorName, false);
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
        public CalculationType type = CalculationType.VOLUME;
        public int feature = 0;
        public float volume_width = 0;
        public float volume_depth = 1;

        public float thrusting_acceleration = 0.1f;
        public float thrusting_speed_scale = 1;
        public float thrusting_depth_scale = 0;

        public float rubbing_acceleration = 0.1f;
        public float rubbing_scale = 1;

        public AudioLinkChannel audioLink_channel;
        public float audioLink_scale = 1;

        public float max = 1;

        public string name;

        private bool _active;
        /** <summary>Sets toy active and shows ui.</summary> **/
        public void SetActive(Toy toy)
        {
            GetBehaviourUI(toy).ShowUI();
            _active = true;
        }

        /** <summary>Sets toy inactive and hides ui.</summary> **/
        public void SetInactive(Toy toy)
        {
            GetBehaviourUI(toy).HideUI();
            _active = false;
        }

        public bool IsActive
        {
            get
            {
                return _active;
            }
        }

        private BehaviourUI ui;
        /** <summary>Gets UI for behaviour. If doesnt exist creates one</summary> **/
        public BehaviourUI GetBehaviourUI(Toy toy)
        {
            if(ui == null) ui = toy.GetDeviceUI().GetBehaviourUI(this);
            return ui;
        }

        /** <summary>Sets sensor name. Changes type if nessesary. Saves if not otherwise specified.</summary> **/
        public void SetSensorName(string name, bool save = true)
        {
            this.name = name;
            ChangeCalculatioTypeIfSensorSpecifiesIt();
            if(save) Config.Singleton.Save();
        }

        /** <summary>Changes calculation type if required by sensor.</summary> **/
        private void ChangeCalculatioTypeIfSensorSpecifiesIt()
        {
            if (name == Config.SENSORNAME_AUDIOLINK) type = CalculationType.AUDIOLINK;
            else if (type == CalculationType.AUDIOLINK) type = CalculationType.VOLUME;
        }

        public void SetFeature(int max)
        {
            this.feature = Math.Max(0, max);
            Config.Singleton.Save();
        }

        public void SetCalculationType(CalculationType type)
        {
            this.type = type;
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

        private float currentStrength;
        public float CalculateStrength(float[] audioLinkData)
        {
            if (type == CalculationType.VOLUME)
            {
                currentStrength = runtimeData.CalculateVolume(volume_width, volume_depth);
            }
            else if (type == CalculationType.THRUSTING)
            {
                currentStrength = runtimeData.CalculateThrusting(thrusting_acceleration, thrusting_speed_scale, thrusting_depth_scale);
            }
            else if (type == CalculationType.RUBBING)
            {
                currentStrength = runtimeData.CalculateRubbing(thrusting_acceleration, thrusting_speed_scale);
            }else if( type == CalculationType.AUDIOLINK)
            {
                currentStrength = runtimeData.CalculateAudioLink(audioLink_scale, audioLink_channel, audioLinkData);
            }
            return currentStrength;
        }

        public float GetCurrentStrength()
        {
            return currentStrength;
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

    public enum CalculationType { VOLUME, THRUSTING, RUBBING, AUDIOLINK }

    public struct CalculationTypeData
    {
        public readonly bool showInDropdown;
        public readonly int drowDownIndex;
        public readonly string displayName;

        private readonly static CalculationTypeData VOLUME = new CalculationTypeData(true, 0, "Volume");
        private readonly static CalculationTypeData THRUSTING = new CalculationTypeData(true, 1, "Thrusting");
        private readonly static CalculationTypeData RUBBING = new CalculationTypeData(true, 2, "Rubbing");
        private readonly static CalculationTypeData AUDIOLINK = new CalculationTypeData(false, -1, "Audio Link");

        public readonly static CalculationTypeData[] COLLECTION = new CalculationTypeData[] { VOLUME, THRUSTING, RUBBING, AUDIOLINK };
        private CalculationTypeData(bool s, int drowDownIndex, string displayName)
        {
            this.showInDropdown = s;
            this.drowDownIndex = drowDownIndex;
            this.displayName = displayName;
        }

        public static CalculationTypeData Get(CalculationType type)
        {
            return COLLECTION[(int)type];
        }
    }
}
