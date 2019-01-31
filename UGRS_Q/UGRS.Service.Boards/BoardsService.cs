using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.Boards;

namespace UGRS.Service.Boards
{
    partial class BoardsService : ServiceBase
    {
        public BoardsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //LogService.WriteInfo("Iniciando servicio de pantallas...");
                RegisterObject();
                LogService.WriteInfo("Servicio Iniciando.");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
        }

        protected override void OnStop()
        {
            try
            {
                //LogService.WriteInfo("Deteniendo servicio de pantallas...");
                Disconnect();
                UnRegisterObject();
                //base.Stop();
                LogService.WriteInfo("Servicio detenido.");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
        }

        private void RegisterObject()
        {
            //RemotingConfiguration.Configure(PathUtilities.GetCurrent("UGRS.Service.Boards.exe.config"), false);
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
        }

        private void Disconnect()
        {
            BoardsServerObject lObjBoards = (BoardsServerObject)Activator.GetObject(typeof(BoardsServerObject), "http://localhost:8820/Boards");
            lObjBoards.DisconnectAll();
        }

        private void UnRegisterObject()
        {
            //IChannel[] lArrObjRegistedChannels = ChannelServices.RegisteredChannels;
            //IChannel lObjChannel = (IChannel)ChannelServices.GetChannel(lArrObjRegistedChannels[0].ChannelName);
            IChannel lObjChannel = (IChannel)ChannelServices.GetChannel(GetChannelName());
            ChannelServices.UnregisterChannel(lObjChannel);
        }

        private string GetChannelName()
        {
            return ConfigurationUtility.GetValue<string>("ChannelName");
        }

        private int GetPort()
        {
            return ConfigurationUtility.GetValue<int>("Port");
        }
    }
}
