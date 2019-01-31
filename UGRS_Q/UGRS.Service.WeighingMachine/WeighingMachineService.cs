using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine;

namespace UGRS.Service.WeighingMachine
{
    partial class WeighingMachineService : ServiceBase
    {
        public WeighingMachineService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //LogService.WriteInfo("Iniciando servicio de báscula...");
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
                //LogService.WriteInfo("Deteniendo servicio de báscula...");
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
            //RemotingConfiguration.Configure(PathUtilities.GetCurrent("UGRS.Service.WeighingMachine.exe.config"), false);
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
        }

        private void Disconnect()
        {
            WeighingMachineServerObject lObjWeighingMachine = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
            lObjWeighingMachine.DisconnectAll();
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
