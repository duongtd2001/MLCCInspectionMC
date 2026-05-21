using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLCCInspectionMC
{
    public abstract class ILightting : MSystem
    {
        public abstract void Connect(string port, int baurate);
        public abstract bool IsConnected();
        public abstract void Disconnect();
        public abstract void LightON(int channel, int val);
        public abstract void LightOFF(int channel);
        public abstract void MutilChannelON(int val);
        public abstract void MutilChannelOFF();
        public abstract List<(string channel, string value)> GetLights();
    }
}
