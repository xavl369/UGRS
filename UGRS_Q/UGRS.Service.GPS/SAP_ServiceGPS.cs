using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UGRS.Core.SDK.DI.GPS.Services;
using UGRS.Core.Utility;

namespace UGRS.Service.GPS
{
    partial class SAP_ServiceGPS : ServiceBase
    {
        readonly Timer tmrService;
        readonly int mIntTimerInterval;
        private bool mBolProcessInProgress;
        private SetupService mObjSetupService;

        public SAP_ServiceGPS()
        {
            InitializeComponent();
            mObjSetupService = new SetupService();
            tmrService = new Timer();
            mIntTimerInterval = 900000;
        }

        protected override void OnStart(string[] args)
        {
            mBolProcessInProgress = false;
            mObjSetupService.InitializeTables();
           // eventLog1.WriteEntry("In onStart.");
            var timer = new Timer { AutoReset = true, Interval = 900000 };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }
        protected override void OnContinue()
        {
            mBolProcessInProgress = false;
            // eventLog1.WriteEntry("In onStart.");
            var timer = new Timer { AutoReset = true, Interval = 900000 };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tmrService.Enabled = false;
            try
            {
                if (!mBolProcessInProgress)
                {
                    mBolProcessInProgress = true;

                    LogUtility.Write("Proceso de importar Reporte de distancia iniciado");
                    string lStrPathKM = ConfigurationManager.AppSettings["PathKM"];
                    //@"C:\Users\amartinez\Desktop\csv\KM";
                    if (!string.IsNullOrEmpty(lStrPathKM))
                    {
                        List<string> lLstFilesKM = ImportFiles.FindFiles(lStrPathKM);
                        ReadCsvKm.VerifyFilesKM(lLstFilesKM, lStrPathKM);
                    }
                    LogUtility.Write("Proceso reporte de distancia terminado");
                    LogUtility.Write("Proceso de Importar reporte de horas de motor iniciado");
                    string lStrPathTime = ConfigurationManager.AppSettings["PathTIME"];
                    //@"C:\Users\amartinez\Desktop\csv\TIME";
                    if (!string.IsNullOrEmpty(lStrPathTime)){
                        List<string> lLstFilesTime = ImportFiles.FindFiles(lStrPathTime);
                    ReadCsvTime.VerifyFilesTime(lLstFilesTime, lStrPathTime);
                    }
                    
                    LogUtility.Write("Proceso horas de motor terminado");

                    mBolProcessInProgress = false;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Exception ie = ex.InnerException;
                while (ie != null)
                {
                    msg += "||" + ie.Message;
                    ie = ie.InnerException;
                }
                LogUtility.Write(msg);
            }
            finally
            {

            }
            tmrService.Enabled = true;
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
