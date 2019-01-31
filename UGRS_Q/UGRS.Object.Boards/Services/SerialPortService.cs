using System.IO.Ports;
using UGRS.Core.Utility;

namespace UGRS.Object.Boards.Services
{
    public class SerialPortService : ISerialPortService
    {
        private SerialPort mObjSerialPort;
        private string mStrConfigurationKey;

        public SerialPortService()
        {
            mStrConfigurationKey = "";
            mObjSerialPort = GetSerialPort();
        }

        public SerialPortService(string pStrConfigurationKey)
        {
            mStrConfigurationKey = pStrConfigurationKey;
            mObjSerialPort = GetSerialPort();
        }

        public string GetName()
        {
            return mObjSerialPort != null ? mObjSerialPort.PortName : "Unknown";
        }

        public void Open()
        {
            if (mObjSerialPort != null && !mObjSerialPort.IsOpen)
            {
                mObjSerialPort.Open();
            }
        }

        public void Close()
        {
            if (mObjSerialPort != null && mObjSerialPort.IsOpen)
            {
                mObjSerialPort.Close();
            }
        }

        public bool IsOpen()
        {
            return mObjSerialPort.IsOpen;
        }

        public void Write(string pStrValue)
        {
            if (mObjSerialPort != null && mObjSerialPort.IsOpen)
            {
                mObjSerialPort.Write(pStrValue);
            }
        }

        private SerialPort GetSerialPort()
        {
            SerialPort lObjResult = new SerialPort();
            string lStrConfigurationKey = !string.IsNullOrEmpty(mStrConfigurationKey) ? string.Concat(mStrConfigurationKey, "_") : "";

            //Get and set properties
            lObjResult.PortName = GetPortName(lStrConfigurationKey);
            lObjResult.BaudRate = GetBaudRate(lStrConfigurationKey);
            lObjResult.DataBits = GetDataBits(lStrConfigurationKey);
            lObjResult.Parity = GetParity(lStrConfigurationKey);
            lObjResult.StopBits = GetStopBits(lStrConfigurationKey);
            lObjResult.ReadTimeout = GetReadTimeout(lStrConfigurationKey);

            return lObjResult;
        }

        #region Configuration

        private string GetPortName(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<string>(string.Concat(pStrConfigurationKey ,"PortName"));
        }

        private int GetBaudRate(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<int>(string.Concat(pStrConfigurationKey ,"BaudRate"));
        }

        private int GetDataBits(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<int>(string.Concat(pStrConfigurationKey ,"DataBits"));
        }

        private Parity GetParity(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<Parity>(string.Concat(pStrConfigurationKey ,"Parity"));
        }

        private StopBits GetStopBits(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<StopBits>(string.Concat(pStrConfigurationKey ,"StopBits"));
        }

        private int GetReadTimeout(string pStrConfigurationKey)
        {
            return ConfigurationUtility.GetValue<int>(string.Concat(pStrConfigurationKey ,"ReadTimeout"));
        }

        #endregion
    }
}
