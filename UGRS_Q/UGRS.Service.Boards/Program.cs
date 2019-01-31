using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using UGRS.Core.Utility;
using UGRS.Object.Boards;

namespace UGRS.Service.Boards
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            {
                new BoardsService()
            };
            ServiceBase.Run(ServicesToRun);

            //RemotingConfiguration.Configure(PathUtilities.GetCurrent("UGRS.Service.Boards.exe.config"), false);
            //RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

            //Console.WriteLine("Server is running...");
            //Console.ReadLine();

            //BoardsServerObject lObjBoards = (BoardsServerObject)Activator.GetObject(typeof(BoardsServerObject), "http://localhost:8820/Boards");
            //lObjBoards.DisconnectAll();

            //IChannel[] regChannels = ChannelServices.RegisteredChannels;
            //IChannel channel = (IChannel)ChannelServices.GetChannel(regChannels[0].ChannelName);

            //IChannel channel = (IChannel)ChannelServices.GetChannel(ConfigurationUtility.GetValue<string>("ChannelName"));

            //ChannelServices.UnregisterChannel(channel);
        }
    }
}
