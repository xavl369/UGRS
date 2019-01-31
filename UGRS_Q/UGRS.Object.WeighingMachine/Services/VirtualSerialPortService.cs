using System;
using System.Timers;
using UGRS.Object.WeighingMachine.Events;

namespace UGRS.Object.WeighingMachine.Services
{
    public class VirtualSerialPortService : ISerialPortService
    {
        private Timer mObjTimer;
        private Random mObjRandom;
        private bool mBolIsOpened;
        private bool mBolFirtValue;

        public VirtualSerialPortService()
        {
            mObjRandom = new Random();

            mObjTimer = new Timer
            {
                AutoReset = true,
                Interval = 500
            };
        }

        public event SerialPortEventHandler DataReceived;

        protected virtual void OnDataReceived(SerialPortEventArgs pObjEventArgs)
        {
            if (DataReceived != null)
            {
                DataReceived(pObjEventArgs);
            }
        }

        public string GetName()
        {
            return "Virtual";
        }

        public void Open()
        {
            mObjTimer.Elapsed += GetNextValue;
            mObjTimer.Start();
            mBolIsOpened = true;
        }

        public void Close()
        {
            mObjTimer.Elapsed -= GetNextValue;
            mObjTimer.Stop();
            mBolIsOpened = false;
        }

        public bool IsOpen()
        {
            return mBolIsOpened;
        }

        public void Write(string pStrText)
        {

        }

        private void GetNextValue(object pObjSender, ElapsedEventArgs pObjEventArgs)
        {
            string lStrValue;
            if (mBolFirtValue)
            {
                lStrValue = string.Format("N       {0}", mObjRandom.Next(5, 9));
                mBolFirtValue = false;
            }
            else
            {
                lStrValue = string.Format("{0} kg\r ", mObjRandom.Next(0, 100));
                mBolFirtValue = true;
            }

            OnDataReceived(new SerialPortEventArgs(lStrValue));
        }
    }
}
