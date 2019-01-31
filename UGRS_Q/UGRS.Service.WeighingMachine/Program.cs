using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using UGRS.Core.Utility;
using UGRS.Object.WeighingMachine;

namespace UGRS.Service.WeighingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            {
                new WeighingMachineService()
            };
            ServiceBase.Run(ServicesToRun);

            ////RemotingConfiguration.Configure(PathUtilities.GetCurrent("UGRS.Service.WeighingMachine.exe.config"), false);
            //RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

            //Console.WriteLine("Server is running...");
            //Console.ReadLine();

            //WeighingMachineServerObject lObjWeighingMachine = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
            //lObjWeighingMachine.DisconnectAll();

            ////IChannel[] regChannels = ChannelServices.RegisteredChannels;
            ////IChannel channel = (IChannel)ChannelServices.GetChannel(regChannels[0].ChannelName);
            //IChannel channel = (IChannel)ChannelServices.GetChannel(ConfigurationUtility.GetValue<string>("ChannelName"));

            //ChannelServices.UnregisterChannel(channel);
        }
    }
}
