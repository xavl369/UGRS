using System.Collections.Generic;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.Utility;
using SAPbobsCOM;
using System;
using UGRS.Core.Exceptions;
using System.Reflection;
using UGRS.Core.SDK.DI.FoodProduction.Tables;


namespace UGRS.Core.SDK.DI.FoodProduction.DAO
{
    public class ReceptionTransferDAO
    {
        QueryManager mObjQueryManager;
        public ReceptionTransferDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public string GetTransferDetailQuery(string pStrDocEntry)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();            
            lLstStrParameters.Add("DocEntry", pStrDocEntry);
            return this.GetSQL("TransferDetail").Inject(lLstStrParameters);
        }

        public string GetTransferHeaderQuery(string pStrId,string pStrDocEntry)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("Warehouse", pStrId);
            lLstStrParameters.Add("DocEntry", pStrDocEntry);

            return this.GetSQL("TransferHeader").Inject(lLstStrParameters);
        }


        public int getSeries(string pStrUserID,string pStrObjectCode)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            int lStrSeries = 0;
            try
            {
                //ReceptionTransferDAO mObjTicketDAO = new ReceptionTransferDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("UserID", pStrUserID);
                lLstStrParameters.Add("ObjCode", pStrObjectCode);
                string lStrQuery = this.GetSQL("GetSeries").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrSeries = Convert.ToInt32(lObjRecordset.Fields.Item(0).Value.ToString());
                }
            }
            catch
            {
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lStrSeries;
        }

        public string getWareHouse(string pStrUserID)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrWareHouse = "";
            try
            {
                //ReceptionTransferDAO mObjTicketDAO = new ReceptionTransferDAO();
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("UserID", pStrUserID);
                string lStrQuery = this.GetSQL("GetWarehouse").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrWareHouse = lObjRecordset.Fields.Item(0).Value.ToString();
                }
            }
            catch
            {
                // UIApplication.ShowMessageBox(string.Format("InitDataSourcesException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lStrWareHouse;
        }

        public string SearchWhsTransit(string pStrWhsTransit)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("Warehouse", pStrWhsTransit);
            return this.GetSQL("SearchWhsTransit").Inject(lLstStrParameters);
        }

        public string SearchWhsTransitDetail(string pStrDocEntry)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("DocEntry", pStrDocEntry);
            return this.GetSQL("SearchWhsTransitDetail").Inject(lLstStrParameters);
        }
        
    }
}
