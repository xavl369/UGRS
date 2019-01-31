using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.Models;
using UGRS.Core.SDK.Connection;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.Configuration
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Credenciales();
            try
            {
                Application oApp = null;
                if (args.Length < 1)
                {
                    oApp = new Application();
                }
                else
                {
                    oApp = new Application(args[0]);
                }

                ConnectDIApplication();

                Menu MyMenu = new Menu();
                MyMenu.AddMenuItems();
                oApp.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);

                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private static void ConnectDIApplication()
        {
            DIApplication.DIConnect((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
        }

        //static void Credenciales()
        //{
        //    Credentials lObjCredentials = new Credentials();

        //    lObjCredentials.LicenseServer = "localhost";
        //    lObjCredentials.UserName = "manager";
        //    lObjCredentials.Password = "sap123";
        //    lObjCredentials.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
        //    lObjCredentials.SQLServer = "MARTINEZA";
        //    lObjCredentials.SQLUserName = "sa";
        //    lObjCredentials.SQLPassword = "qualisys123";
        //    lObjCredentials.DataBaseName = "UGRS";
        //    lObjCredentials.Language = SAPbobsCOM.BoSuppLangs.ln_English;

        //    DIConnection lObjDIConnection = new DIConnection();
        //    lObjDIConnection.ConnectToDI(lObjCredentials);

        //    if (lObjDIConnection.Company != null && lObjDIConnection.Company.Connected)
        //    {
        //        DIApplication.DIConnect(lObjDIConnection.Company);
        //    }
        //}

        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    break;
                default:
                    break;
            }
        }
    }
}
