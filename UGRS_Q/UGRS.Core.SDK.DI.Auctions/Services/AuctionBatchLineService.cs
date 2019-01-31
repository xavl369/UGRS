using System;
using UGRS.Core.SDK.DI.Auctions.Tables;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class AuctionBatchLineService
    {
        private TableDAO<BatchLine> mObjBatchLineDAO;
        private QueryManager mObjQueryManager;

        public AuctionBatchLineService()
        {
            mObjBatchLineDAO = new TableDAO<BatchLine>();
            mObjQueryManager = new QueryManager();
        }

        public int Add(BatchLine pObjRecord)
        {
            return mObjBatchLineDAO.Add(pObjRecord);
        }

        public int Update(BatchLine pObjRecord)
        {
            pObjRecord.RowCode = GetCode(pObjRecord.Id);
            return mObjBatchLineDAO.Update(pObjRecord);
        }

        public int Remove(string pStrDocEntry)
        {
            return mObjBatchLineDAO.Remove(pStrDocEntry);
        }

        public DateTime GetLastCreationDate(string pStrLocation)
        {
            //DateTime lDtmLastCreationDate = mObjQueryManager.Max<DateTime>("U_CreationDate", "[@UG_SU_BALN]");
            DateTime lDtmLastCreationDate = mObjQueryManager.GetLastBatchDateByLocation("U_CreationDate", "[@UG_SU_BALN]", pStrLocation);
            DateTime lDtmLastCreationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_CreationTime", "U_CreationDate", lDtmLastCreationDate.ToString("yyyy-MM-dd"), "[@UG_SU_BALN]"));
            return lDtmLastCreationDate.Date.Add(lDtmLastCreationTime.TimeOfDay);
        }

        public DateTime GetLastModificationDate(string pStrLocation)
        {
            //DateTime lDtmLastModificationDate = mObjQueryManager.Max<DateTime>("U_ModificationDate", "[@UG_SU_BALN]");
            DateTime lDtmLastModificationDate = mObjQueryManager.GetLastBatchDateByLocation("U_ModificationDate", "[@UG_SU_BALN]", pStrLocation);
            DateTime lDtmLastModificationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_ModificationTime", "U_ModificationDate", lDtmLastModificationDate.ToString("yyyy-MM-dd"), "[@UG_SU_BALN]"));
            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }

        public bool HasBeenImported(long pLonBatchLineId)
        {
            return mObjQueryManager.Exists("U_Id", pLonBatchLineId.ToString(), "[@UG_SU_BALN]");
        }

        public bool HasBeenUpdated(long pLonBatchLineId, DateTime pDtmModificationDate)
        {
            return !pDtmModificationDate.ToString("yyyy-MM-dd HH:mm").Equals(GetModificationDate(pLonBatchLineId).ToString("yyyy-MM-dd HH:mm"));
        }

        private string GetCode(long pLonBatchId)
        {
            return mObjQueryManager.GetValue("Code", "U_Id", pLonBatchId.ToString(), "[@UG_SU_BALN]");
        }

        private DateTime GetModificationDate(long pLonBatchLineId)
        {
            DateTime lDtmLastModificationDate = Convert.ToDateTime(mObjQueryManager.GetValue("U_ModificationDate", "U_Id", pLonBatchLineId.ToString(), "[@UG_SU_BALN]"));
            DateTime lDtmLastModificationTime = DateUtility.GetTime(Convert.ToInt32(mObjQueryManager.GetValue("U_ModificationTime", "U_Id", pLonBatchLineId.ToString(), "[@UG_SU_BALN]")));
            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }
    }
}
