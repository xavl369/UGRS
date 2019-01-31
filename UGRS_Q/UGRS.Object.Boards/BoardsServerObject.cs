using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.Boards.Enums;
using UGRS.Object.Boards.Services;

namespace UGRS.Object.Boards
{
    public class BoardsServerObject : MarshalByRefObject
    {
        #region Attributes

        private ISerialPortService mObjDisplayOneService;
        private ISerialPortService mObjDisplayTwoService;
        private IList<Guid> mLstObjConnections;
        private LocationEnum mEnmLocation;

        #endregion

        #region Properties

        private ISerialPortService DisplayOne
        {
            get
            {
                return mObjDisplayOneService;
            }
        }
        private ISerialPortService DisplayTwo
        {
            get
            {
                return mObjDisplayTwoService;
            }
        }

        private IList<Guid> Connections
        {
            get
            {
                return mLstObjConnections;
            }
        }

        private LocationEnum Location
        {
            get
            {
                return mEnmLocation;
            }
        }

        #endregion

        #region Constructor

        public BoardsServerObject()
        {
            LogService.WriteInfo("Initializando constructor...");
            GetLocation();
            InitializeDisplays();
            InitializeConnections();
        }

        #endregion

        #region Methods

        #region Connection

        public Guid Connect()
        {
            LogService.WriteInfo("Iniciando conexion..." + Location.ToString());

            if (Location == LocationEnum.HERMOSILLO)
            {
                ConnectDisplay(DisplayOne);
            }
            else if (Location == LocationEnum.SONORA_SUR)
            {
                ConnectDisplay(DisplayOne);
                ConnectDisplay(DisplayTwo);
            }
            else
            {
                throw new Exception("Favor de implementar la conexión con las pantallas de otras localizaciones.");
            }

            return AddConnection();
        }

        public void Disconnect(Guid pObjConnection)
        {
            LogService.WriteInfo(string.Format("Deteniendo conexion '{0}'...", pObjConnection.ToString()));

            RemoveConnection(pObjConnection);

            if (Location == LocationEnum.HERMOSILLO)
            {
                DisconnectDisplay(DisplayOne);
            }
            else if (Location == LocationEnum.SONORA_SUR)
            {
                DisconnectDisplay(DisplayOne);
                DisconnectDisplay(DisplayTwo);
            }
            else
            {
                throw new Exception("Favor de implementar la conexión con las pantallas de otras localizaciones.");
            }

            LogService.WriteInfo("Conexion detenida.");
        }

        public void DisconnectAll()
        {
            LogService.WriteInfo("Deteniendo todas las conexiones...");

            int lIntConnectionsCount = Connections.Count;
            for (int i = 0; i < lIntConnectionsCount; i++)
            {
                Disconnect(Connections[i]);
            }

            LogService.WriteInfo("Se han detenido todas las conexiones.");
        }

        #endregion

        #region Write

        public void WriteDisplayOne(int pStrHeadsNumber, float pStrTotalWeight, float pStrAverageWeight)
        {
            if (Location == LocationEnum.HERMOSILLO || Location == LocationEnum.SONORA_SUR)
            {
                WriteDisplayOne(DisplayOne, pStrHeadsNumber, pStrTotalWeight, pStrAverageWeight);
            }
            else
            {
                throw new Exception("Favor de implementar la escritura en las pantallas de otras localizaciones.");
            }
        }

        public void WriteDisplayTwo(string pStrBatchNumber, int pIntHeadsNumber, float pFlTotalWeight, float pFlAverageWeight, string pStrBuyerNumber, decimal pDecPrice)
        {

            LogService.WriteInfo("batchnum:" + pStrBatchNumber + " heads: " + pIntHeadsNumber.ToString() + " weight: " + pFlTotalWeight + " avgWeight: " + pFlAverageWeight.ToString() + " buyer: " + pStrBuyerNumber + " price: " + pDecPrice.ToString());

            if (Location == LocationEnum.HERMOSILLO)
            {
                WriteDisplayTwo(DisplayOne, pStrBatchNumber, pIntHeadsNumber, pFlTotalWeight, pFlAverageWeight, pStrBuyerNumber, pDecPrice);
            }
            else if(Location == LocationEnum.SONORA_SUR)
            {
                WriteDisplayTwo(DisplayTwo, pStrBatchNumber, pIntHeadsNumber, pFlTotalWeight, pFlAverageWeight, pStrBuyerNumber, pDecPrice);
            }
            else
            {
                throw new Exception("Favor de implementar la escritura en las pantallas de otras localizaciones.");
            }
        }

        #endregion

        #region Initialize

        private void GetLocation()
        {
            mEnmLocation = ConfigurationUtility.GetValue<LocationEnum>("Location");
        }

        private void InitializeDisplays()
        {
            if (Location == LocationEnum.HERMOSILLO)
            {
                mObjDisplayOneService = new SerialPortService();
            }
            else if (Location == LocationEnum.SONORA_SUR)
            {
                mObjDisplayOneService = new SerialPortService("Display1");
                mObjDisplayTwoService = new SerialPortService("Display2");
            }
            else
            {
                throw new Exception("Favor de configurar la localización de las pantallas.");
            }
        }

        private void InitializeConnections()
        {
            if (mLstObjConnections == null)
            {
                mLstObjConnections = new List<Guid>();
            }
        }

        #endregion

        #region Connection handler

        private Guid AddConnection()
        {
            Guid lObjConnection;
            lock (Connections)
            {
                LogService.WriteInfo("Connectando...");
                lObjConnection = Guid.NewGuid();
                Connections.Add(lObjConnection);
                LogService.WriteInfo("Conectado correctamente.");
            }
            return lObjConnection;
        }

        private void RemoveConnection(Guid pObjConnection)
        {
            lock (Connections)
            {
                if (Connections.Contains(pObjConnection))
                {
                    Connections.Remove(pObjConnection);
                }
            }
        }

        private void ConnectDisplay(ISerialPortService pObjSerialPort)
        {
            if (pObjSerialPort != null)
            {
                LogService.WriteInfo("Puerto serial valido.");
                lock (pObjSerialPort)
                {
                    if (!pObjSerialPort.IsOpen())
                    {
                        LogService.WriteInfo(string.Format("Conectando puerto serial {0}...", pObjSerialPort.GetName()));
                        pObjSerialPort.Open();
                        LogService.WriteInfo(string.Format("Puerto serial {0} conectado.", pObjSerialPort.GetName()));
                    }
                }
            }
        }

        private void DisconnectDisplay(ISerialPortService pObjSerialPort)
        {
            if (pObjSerialPort != null)
            {
                lock (pObjSerialPort)
                {
                    if (Connections.Count == 0 && pObjSerialPort.IsOpen())
                    {
                        LogService.WriteInfo(string.Format("Desconectando puerto serial {0}...", pObjSerialPort.GetName()));
                        pObjSerialPort.Close();
                        LogService.WriteInfo(string.Format("Puerto serial {0} desconectado.", pObjSerialPort.GetName()));
                    }
                }
            }
        }

        #endregion

        #region Write handler

        private void WriteDisplayOne(ISerialPortService pObjDisplay, int pIntHeadsNumber, float pFlTotalWeight, float pFlAverageWeight)
        {

            string lStrHeads = Location == LocationEnum.HERMOSILLO ? pIntHeadsNumber.ToString() : pIntHeadsNumber.ToString("00");
            string lStrTotalWeight = Location == LocationEnum.HERMOSILLO ? pFlTotalWeight.ToString("###0") : pFlTotalWeight.ToString("00000");
            string lStrAverageWeight = Location == LocationEnum.HERMOSILLO ? pFlAverageWeight.ToString("###0.0") : pFlAverageWeight.ToString("000.0");

            LogService.WriteInfo(lStrHeads + ' ' + lStrTotalWeight + ' ' + lStrAverageWeight);
            LogService.WriteInfo(GetFormattedMessage(lStrHeads, GetPositionCode(PositionEnum.HEADS_NUMBER)));
            LogService.WriteInfo(GetFormattedMessage(lStrTotalWeight, GetPositionCode(PositionEnum.TOTAL_WEIGHT)));
            LogService.WriteInfo(GetFormattedMessage(lStrAverageWeight, GetPositionCode(PositionEnum.AVERAGE_WEIGHT)));

            pObjDisplay.Write(GetFormattedMessage(lStrHeads, GetPositionCode(PositionEnum.HEADS_NUMBER)));
            if (Location == LocationEnum.SONORA_SUR)
            {
                Thread.Sleep(800);
            }
            pObjDisplay.Write(GetFormattedMessage(lStrTotalWeight, GetPositionCode(PositionEnum.TOTAL_WEIGHT)));
            if (Location == LocationEnum.SONORA_SUR)
            {
                Thread.Sleep(800);
            }
            pObjDisplay.Write(GetFormattedMessage(lStrAverageWeight, GetPositionCode(PositionEnum.AVERAGE_WEIGHT)));
        }

        private void WriteDisplayTwo(ISerialPortService pObjDisplay, string pStrBatchNumber, int pIntHeadsNumber, float pFlTotalWeight, float pFlAverageWeight, string pStrBuyerNumber, decimal pDbecPrice)
        {
            string lStrBatch = Location == LocationEnum.HERMOSILLO ? pStrBatchNumber : Convert.ToInt32(pStrBatchNumber).ToString("000");
            string lStrHeads = Location == LocationEnum.HERMOSILLO ? pIntHeadsNumber.ToString() : pIntHeadsNumber.ToString("00");
            string lStrTotalWeight = Location == LocationEnum.HERMOSILLO ? pFlTotalWeight.ToString("###0") : pFlTotalWeight.ToString("00000");
            string lStrAverageWeight = Location == LocationEnum.HERMOSILLO ? pFlAverageWeight.ToString("###0.0") : pFlAverageWeight.ToString("000.0");
            string lStrPrice = Location == LocationEnum.HERMOSILLO ? (pDbecPrice > 999 ? pDbecPrice / 10 : pDbecPrice).ToString("N") : pDbecPrice.ToString("00000.00");
            LogService.WriteInfo("buyer: " + pStrBuyerNumber);

            string lStrBuyer = Location == LocationEnum.HERMOSILLO ? pStrBuyerNumber : !string.IsNullOrEmpty(pStrBuyerNumber) ? pStrBuyerNumber.Substring(0, 3) : "000";

            LogService.WriteInfo(lStrBuyer);

            if (Location == LocationEnum.HERMOSILLO)
            {
                pObjDisplay.Write(GetFormattedMessage(lStrBatch, GetPositionCode(PositionEnum.BATCH_NUMBER)));
                pObjDisplay.Write(GetFormattedMessage(lStrHeads, GetPositionCode(PositionEnum.SALE_HEADS_NUMBER)));
                pObjDisplay.Write(GetFormattedMessage(lStrTotalWeight, GetPositionCode(PositionEnum.SALE_TOTAL_WEIGHT)));
                pObjDisplay.Write(GetFormattedMessage(lStrAverageWeight, GetPositionCode(PositionEnum.SALE_AVERAGE_WEIGHT)));
                pObjDisplay.Write(GetFormattedMessage(lStrBuyer, GetPositionCode(PositionEnum.BUYER_NUMBER)));
                pObjDisplay.Write(GetFormattedMessage(lStrPrice, GetPositionCode(PositionEnum.PRICE)));
            }
            else
            {
                string lStrMessage = "Z" + GetFormattedMessage(lStrBatch, GetPositionCode(PositionEnum.BATCH_NUMBER)) +
                    GetFormattedMessage(lStrHeads, GetPositionCode(PositionEnum.SALE_HEADS_NUMBER)) +
                    GetFormattedMessage(lStrTotalWeight, GetPositionCode(PositionEnum.SALE_TOTAL_WEIGHT)) +
                    GetFormattedMessage(lStrAverageWeight, GetPositionCode(PositionEnum.SALE_AVERAGE_WEIGHT)) +
                    GetFormattedMessage(lStrBuyer, GetPositionCode(PositionEnum.BUYER_NUMBER)) +
                    GetFormattedMessage(lStrPrice, GetPositionCode(PositionEnum.PRICE));

                LogService.WriteInfo(lStrMessage);

                pObjDisplay.Write(lStrMessage);
            }
        }

        private string GetFormattedMessage(string pStrMessage, string pStrPosition)
        {
            if (Location == LocationEnum.HERMOSILLO)
            {
                return string.Concat((char)2, pStrPosition, pStrMessage, (char)3);
            }
            else
            {
                LogService.WriteInfo(string.Concat(pStrPosition, pStrMessage));
                return string.Concat(pStrPosition, pStrMessage);
            }
        }

        private string GetPositionCode(PositionEnum pEnmPosition)
        {
            switch (pEnmPosition)
            {
                case PositionEnum.BATCH_NUMBER:
                    return Location == LocationEnum.HERMOSILLO ? "01" :
                           Location == LocationEnum.SONORA_SUR ? "L" : "";
                case PositionEnum.SALE_HEADS_NUMBER:
                    return Location == LocationEnum.HERMOSILLO ? "02" :
                           Location == LocationEnum.SONORA_SUR ? "C" : "";
                case PositionEnum.SALE_TOTAL_WEIGHT:
                    return Location == LocationEnum.HERMOSILLO ? "03" :
                           Location == LocationEnum.SONORA_SUR ? "P" : "";
                case PositionEnum.SALE_AVERAGE_WEIGHT:
                    return Location == LocationEnum.HERMOSILLO ? "04" :
                           Location == LocationEnum.SONORA_SUR ? "p" : "";
                case PositionEnum.BUYER_NUMBER:
                    return Location == LocationEnum.HERMOSILLO ? "05" :
                           Location == LocationEnum.SONORA_SUR ? "B" : "";
                case PositionEnum.PRICE:
                    return Location == LocationEnum.HERMOSILLO ? "06" :
                           Location == LocationEnum.SONORA_SUR ? "M" : "";
                case PositionEnum.HEADS_NUMBER:
                    return Location == LocationEnum.HERMOSILLO ? "07" : 
                           Location == LocationEnum.SONORA_SUR ? "C" : "";
                case PositionEnum.TOTAL_WEIGHT:
                    return Location == LocationEnum.HERMOSILLO ? "08" :
                           Location == LocationEnum.SONORA_SUR ? "P" : "";
                case PositionEnum.AVERAGE_WEIGHT:
                    return Location == LocationEnum.HERMOSILLO ? "09" :
                           Location == LocationEnum.SONORA_SUR ? "p" : "";
                default:
                    throw new Exception("Posición invalida.");
            }
        }

        #endregion

        #endregion

        #region Other

        public override object InitializeLifetimeService()
        {
            return null;
        }

        ~BoardsServerObject()
        {
            RemotingServices.Disconnect(this);
        }

        #endregion
    }
}
