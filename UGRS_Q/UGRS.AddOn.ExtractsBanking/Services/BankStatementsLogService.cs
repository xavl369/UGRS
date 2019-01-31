using System;
using System.Configuration;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;

namespace UGRS.AddOn.ExtractsBanking.Services
{
    public static class BankStatementsLogService
    {
        private static bool mBolIsFullLog = false;

        static BankStatementsLogService()
        {
            mBolIsFullLog = ConfigurationManager.AppSettings["FullLog"] == null ? false : ConfigurationManager.AppSettings["FullLog"].Equals("true") ? true : false;
        }

        public static bool IsFullLog
        {
            get { return mBolIsFullLog; }
        }

        public static void WriteInfo(string pStrMessage)
        {
            LogUtility.Write(string.Format("[INFO] {0}", pStrMessage));
        }

        public static void WriteSuccess(string pStrMessage)
        {
            if(IsFullLog)
            {
                LogUtility.Write(string.Format("[SUCCESS] {0}", pStrMessage));
            }
        }

        public static void WriteWarning(string pStrMessage)
        {
            if (IsFullLog)
            {
                LogUtility.Write(string.Format("[WARNING] {0}", pStrMessage));
            }
        }

        public static void WriteError(string pStrMessage)
        {
            LogUtility.Write(string.Format("[ERROR] {0}", pStrMessage));
        }

        public static void WriteError(int pIntCode)
        {
            LogUtility.Write(string.Format("[ERROR] Code: {0}\t Message: {1}", pIntCode, DIApplication.Company.GetLastErrorDescription()));
        }

        public static void WriteError(Exception pObjException)
        {
            if(IsFullLog)
            {
                WriteError(pObjException.ToString());
            }
            else
            {
                WriteError(pObjException.Message);
            }
        }

    }
}