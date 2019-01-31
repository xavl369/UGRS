// file:	services\batchservice.cs
// summary:	Implements the batchservice class

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Auctions.Tables;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    /// <summary> A batch service. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    public class AuctionBatchService
    {
        private TableDAO<Batch> mObjBatchDAO;
        private QueryManager mObjQueryManager;

        public AuctionBatchService()
        {
            mObjBatchDAO = new TableDAO<Batch>();
            mObjQueryManager = new QueryManager();
        }

        public int Add(Batch pObjRecord)
        {
            return mObjBatchDAO.Add(pObjRecord);
        }

        public int Update(Batch pObjRecord, string pStrAuctFolio)
        {
            pObjRecord.RowCode = GetCode(pObjRecord.Number, pStrAuctFolio);
            return mObjBatchDAO.Update(pObjRecord);
        }

        public int Remove(string pStrDocEntry)
        {
            return mObjBatchDAO.Remove(pStrDocEntry);
        }

        //public DataTable GetBatchesByFilters(string pStrBuyer, string pStrSeller, string pStrSellerTaxCode, string pStrDate)
        //{

        //    DataTable lDtbResult = new DataTable();
        //    Recordset lObjRecordset = null;

        //    try
        //    {
        //        lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
        //        pStrDate = ValidStringDate(pStrDate);

        //        Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
        //        lLstStrParameters.Add("Buyer", pStrBuyer);
        //        lLstStrParameters.Add("Seller", pStrSeller);
        //        lLstStrParameters.Add("SellerTaxCode", pStrSellerTaxCode);
        //        lLstStrParameters.Add("Date", pStrDate);

        //        string lStrQuery = this.GetSQL("GetBatchesByFilters").Inject(lLstStrParameters);
        //        lObjRecordset.DoQuery(lStrQuery);
        //        lDtbResult = lObjRecordset.ToDataTable();
        //    }
        //    catch (Exception lObjException)
        //    {
        //        throw new DAOException(lObjException.Message, lObjException);
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjRecordset);
        //    }

        //    return lDtbResult;
        //}

        public DataTable GetBatchesByFilters(string pStrBuyer, string pStrSeller, string pStrSellerTaxCode, string pStrDate)
        {
           return (DataTable)mObjQueryManager.GetBatchesByFilters(pStrBuyer, pStrSeller, pStrSellerTaxCode, pStrDate);
        }


        public DateTime GetLastCreationDate()
        {
            DateTime lDtmLastCreationDate = mObjQueryManager.Max<DateTime>("U_CreationDate", "[@UG_SU_BAHS]");
            DateTime lDtmLastCreationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_CreationTime", "U_CreationDate", lDtmLastCreationDate.ToString("yyyy-MM-dd"), "[@UG_SU_BAHS]"));
            return lDtmLastCreationDate.Date.Add(lDtmLastCreationTime.TimeOfDay);
        }

        public DateTime GetLastModificationDate()
        {
            DateTime lDtmLastModificationDate = mObjQueryManager.Max<DateTime>("U_ModificationDate", "[@UG_SU_BAHS]");
            DateTime lDtmLastModificationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_ModificationTime", "U_ModificationDate", lDtmLastModificationDate.ToString("yyyy-MM-dd"), "[@UG_SU_BAHS]"));
            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }

        public bool HasBeenImported(string pStrFolio, int pIntBatchNumber)
        {
            //return mObjQueryManager.Exists("U_Id", pLonBatchId.ToString(), "[@UG_SU_BAHS]");
            return mObjQueryManager.BatchExists(pStrFolio, pIntBatchNumber);
        }

        public bool HasBeenUpdated(int pIntBatchNumber, string pStrAuctFolio, DateTime pDtmModificationDate)
        {
            return !pDtmModificationDate.ToString("yyyy-MM-dd HH:mm").Equals(GetModificationDate(pIntBatchNumber, pStrAuctFolio).ToString("yyyy-MM-dd HH:mm"));
        }

        private string GetCode(int pIntBatchNumber, string pStrAuctionFolio)
        {

            long lLonBatchId = mObjQueryManager.GetBatchId(pIntBatchNumber, pStrAuctionFolio);
            return mObjQueryManager.GetValue("Code", "U_Id", lLonBatchId.ToString(), "[@UG_SU_BAHS]");
        }

        private DateTime GetModificationDate(int pIntBatchNumber, string pStrAuctionFolio)
        {
            long lLonBatchId = mObjQueryManager.GetBatchId(pIntBatchNumber, pStrAuctionFolio);

            DateTime lDtmLastModificationDate = Convert.ToDateTime(mObjQueryManager.GetValue("U_ModificationDate", "U_Id", lLonBatchId.ToString(), "[@UG_SU_BAHS]"));
            DateTime lDtmLastModificationTime = DateUtility.GetTime(Convert.ToInt32(mObjQueryManager.GetValue("U_ModificationTime", "U_Id", lLonBatchId.ToString(), "[@UG_SU_BAHS]")));
            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }

        private string ValidStringDate(string pStrDate)
        {
            try
            {
                DateTime.Parse(pStrDate);
                return pStrDate;
            }
            catch
            {
                return "";
            }
        }

        public long GetAuctIdByFolio(string pStrAuctFolio)
        {
            return Convert.ToInt64(mObjQueryManager.GetValue("U_Id", "U_Folio", pStrAuctFolio, "[@UG_SU_AUTN]"));
        }

        public long GetNextBatchId()
        {
            return mObjQueryManager.Max<long>("U_Id", "[@UG_SU_BAHS]") + 1;
        }
    }
}
