﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static VRCToyController.Toy;

namespace VRCToyController
{
    public partial class DeviceUI : UserControl
    {
        public Toy toy;

        private bool is_testing;
        public DeviceUI()
        {
            InitializeComponent();
        }

        public string[] featureNames;
        public DeviceUI(Toy toy)
        {
            InitializeComponent();

            //Name + ID
            this.name.Text = toy.name;
            this.toy = toy;

            //Add feature names
            featureNames = new string[toy.totalFeatureCount];
            int j = 0;
            foreach(KeyValuePair<ToyFeatureType,int> featureC in toy.featureCount)
            {
                for(int f = 0; f < featureC.Value; f++)
                {
                    featureNames[j++] = featureC.Key + (f > 0 ? " " + f : "");
                }
            }

            //foreach (BehaviourData behaviour in toy.GetDeviceData().behaviours)
            //    AddBehaviourUI(behaviour);
        }

        public BehaviourUI AddBehaviourUI(BehaviourData behaviour)
        {
            BehaviourUI newui = new BehaviourUI(toy, behaviour, featureNames);
            if (paramsList.IsHandleCreated)
            {
                paramsList.Invoke((Action)delegate ()
                {
                    paramsList.Controls.Add(newui);
                });
            }
            else
            {
                paramsList.Controls.Add(newui);
            }
            return newui;
        }

        public void RemoveBehaviourUI(DeviceData deviceData, BehaviourUI behaviourUI)
        {
            if (paramsList.IsHandleCreated)
            {
                paramsList.Invoke((Action)delegate ()
                {
                    paramsList.Controls.Remove(behaviourUI);
                });
            }
            else
            {
                paramsList.Controls.Remove(behaviourUI);
            }
            deviceData.removedSensors.Add(behaviourUI.GetBehaviourData().name);
        }

        public BehaviourUI GetBehaviourUI(BehaviourData behaviour)
        {
            foreach(Control c in paramsList.Controls)
            {
                if(c is BehaviourUI && (c as BehaviourUI).GetBehaviourData() == behaviour)
                {
                    return c as BehaviourUI;
                }
            }
            return AddBehaviourUI(behaviour);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            if(Mediator.activeSensorPositions.Count > 0)
            {
                toy.AddBehaviour(Mediator.activeSensorPositions.Keys.First());
            }
        }

        private void b_test_Click(object sender, EventArgs e)
        {
            if(is_testing == false)
            {
                Task.Factory.StartNew(() => {
                    is_testing = true;
                    toy.TestBehaviours();
                    is_testing = false;
                });
            }
        }

        private void DeviceUI_Load(object sender, EventArgs e)
        {

        }
    }
}
