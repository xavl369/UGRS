using System;

namespace UGRS.Object.WeighingMachine
{
    public class WrapperObject : MarshalByRefObject
    {
        public event WeighingMachineEventHandler WrapperDataReceived;

        public void WrapperOnDataReceived(string pStrValue)
        {
            if (WrapperDataReceived != null)
                WrapperDataReceived(pStrValue);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
