using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToyController
{
    public abstract class Toy
    {
        public ToyAPI toyAPI;
        public string name;
        public int motorCount;

        public void Vibrate(double[] strength)
        {
            toyAPI.Vibrate(this, strength);
        }

        public void Test()
        {
            Vibrate(new double[] { 1, 0 });
            System.Threading.Thread.Sleep(2000);
            Vibrate(new double[] { 0, 1 });
            System.Threading.Thread.Sleep(2000);
            Vibrate(new double[] { 0, 0 });
        }
    }
}
