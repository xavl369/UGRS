using System;
using System.Runtime.InteropServices;

namespace UGRS.Object.WeighingMachine.Events
{
    [Serializable]
    [ComVisible(true)]
    public delegate void SerialPortEventHandler(SerialPortEventArgs pObjVirtualEventsArgs);

    public class SerialPortEventArgs : EventArgs
    {
        private readonly string mStrValue = "";

        public SerialPortEventArgs(string pStrValue)
        {
            mStrValue = pStrValue;
        }

        public string Value
        {
            get { return mStrValue; }
        }
    }
}
