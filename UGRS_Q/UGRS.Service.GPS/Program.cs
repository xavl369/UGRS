using System.Collections.Generic;
using UGRS.Core.SDK.Connection;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.Models;
using System.Configuration;
using System.ServiceProcess;
using UGRS.Core.SDK.DI.GPS;
using UGRS.Core.SDK.DI.Configuration;
using UGRS.Core.Utility;

namespace UGRS.Service.GPS
{
    class Program
    {
        static ConfigurationServicesFactory mObjConfigurationService = new ConfigurationServicesFactory();
        static void Main(string[] args)
        {
            Credentials lObjCredentials = new Credentials();
            
            //lObjCredentials.LicenseServer = ConfigurationManager.AppSettings["LicenseServer"].ToString();
            lObjCredentials.UserName = ConfigurationManager.AppSettings["UsernameSAP"].ToString();
            lObjCredentials.Password = ConfigurationManager.AppSettings["PasswordSAP"].ToString();
            switch (ConfigurationManager.AppSettings["DBServer"].ToString())
            {
                case "MSSQL2012":
                    lObjCredentials.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
                    break;
                case "MSSQL2014":
                    lObjCredentials.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
                    break;
                case "MSSQL2016":
                    lObjCredentials.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
                    break;
                default:
                    break;
            }
            //lObjCredentials.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            lObjCredentials.SQLServer = ConfigurationManager.AppSettings["SQLServer"].ToString();
            lObjCredentials.SQLUserName = ConfigurationManager.AppSettings["UsernameSQL"].ToString();
            lObjCredentials.SQLPassword = ConfigurationManager.AppSettings["PasswordSQL"].ToString();
            lObjCredentials.DataBaseName = ConfigurationManager.AppSettings["DBName"].ToString();
            //lObjCredentials.Language = SAPbobsCOM.BoSuppLangs.ln_English;

            DIConnection lObjDIConnection = new DIConnection();
            lObjDIConnection.ConnectToDI(lObjCredentials);

            if (lObjDIConnection.Company != null && lObjDIConnection.Company.Connected)
            {
                DIApplication.DIConnect(lObjDIConnection.Company);
            }
            //IList<string> list = new List<string>() { "raul", "martin", "anaya", "rojo" };
            //Console.WriteLine(string.Join(",", list.ToArray()));
            //Console.ReadLine();

            GPSFactoryServices lObjGPSFactoryServices = new GPSFactoryServices();
            lObjGPSFactoryServices.GetSetupService().InitializeTables();
            UGRS.Core.SDK.DI.Configuration.ConfigurationServicesFactory lObjConfigurationFactoryServices = new UGRS.Core.SDK.DI.Configuration.ConfigurationServicesFactory();



            //ServiceBase[] ServicesToRun = new ServiceBase[] 
            //{
            //    new SAP_ServiceGPS()
            //};
            //ServiceBase.Run(ServicesToRun);





            string lStrPathKM = ConfigurationManager.AppSettings["PathKM"].ToString();
            string lStrPathTime = ConfigurationManager.AppSettings["PathTime"].ToString();

            if (!string.IsNullOrEmpty(lStrPathKM))
            {
                //@"C:\Users\amartinez\Desktop\csv\KM";
                List<string> lLstFilesKM = ImportFiles.FindFiles(lStrPathKM);
                ReadCsvKm.VerifyFilesKM(lLstFilesKM, lStrPathKM);
            }
            else
            {
                LogUtility.Write("Error al momento de consultar el directorio Kilometros recorridos");
            }
            if (!string.IsNullOrEmpty(lStrPathTime))
            {
                //@"C:\Users\amartinez\Desktop\csv\TIME";
                List<string> lLstFilesTime = ImportFiles.FindFiles(lStrPathTime);
               // ReadCsvTime.VerifyFilesTime(lLstFilesTime, lStrPathTime);
            }
            else
            {
                LogUtility.Write("Error al momento de consultar el directorio de Horas de motor");
            }

            //ReadCSV.ReadKilometers(@"C:\testkm.csv");
            //ReadCSV.ReadTimeEngine(@"C:\test.csv");
        }

    }
}
