﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public abstract class ToyAPI
    {
        public static Task<ToyAPI> GetClient;
        //public abstract void ExecuteFeatures(Toy toy, double[] strength);

        public abstract void UpdateBatteryIndicator(Toy itoy);
        public abstract void SlowUpdate();
    }
}
