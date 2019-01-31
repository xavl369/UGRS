// file:	services\auctionservice.cs
// summary:	Implements the auctionservice class

using System;
using System.Globalization;
using UGRS.Core.SDK.DI.Auctions.Tables;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    /// <summary> An Action service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class AuctionService
    {
        private TableDAO<Auction> mObjAuctionDAO;
        private QueryManager mObjQueryManager;

        public AuctionService()
        {
            mObjAuctionDAO = new TableDAO<Auction>();
            mObjQueryManager = new QueryManager();
        }

        #region SAP B1

        public int Add(Auction pObjRecord)
        {
            return mObjAuctionDAO.Add(pObjRecord);
        }

        public int Update(Auction pObjRecord)
        {
            pObjRecord.RowCode = GetCode(pObjRecord.Folio);
            return mObjAuctionDAO.Update(pObjRecord);
        }

        public int Remove(string pStrFolio)
        {
            string lStrCode = GetCode(pStrFolio);
            return mObjAuctionDAO.Remove(lStrCode);
        }

        #endregion

        public long GetNextId()
        {
            return mObjQueryManager.Max<long>("U_Id", "[@UG_SU_AUTN]") + 1;
        }


        public DateTime GetLastCreationDate(string pStrLocation)
        {
            //DateTime lDtmLastCreationDate = mObjQueryManager.Max<DateTime>("U_CreationDate", "[@UG_SU_AUTN]");  
            DateTime lDtmLastCreationDate = mObjQueryManager.GetLastDateByLocation("U_CreationDate", "[@UG_SU_AUTN]", pStrLocation);
            DateTime lDtmLastCreationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_CreationTime", "U_CreationDate", lDtmLastCreationDate.ToString("yyyy-MM-dd"), "[@UG_SU_AUTN]"));
            return lDtmLastCreationDate.Date.Add(lDtmLastCreationTime.TimeOfDay);
        }


        public DateTime GetLastModificationDate(string pStrLocation)
        {
            //DateTime lDtmLastModificationDate = mObjQueryManager.Max<DateTime>("U_ModificationDate", "[@UG_SU_AUTN]");
            DateTime lDtmLastModificationDate = mObjQueryManager.GetLastDateByLocation("U_ModificationDate", "[@UG_SU_AUTN]", pStrLocation);
            DateTime lDtmLastModificationTime = DateUtility.GetTime(mObjQueryManager.Max<int>("U_ModificationTime", "U_ModificationDate", lDtmLastModificationDate.ToString("yyyy-MM-dd"), "[@UG_SU_AUTN]"));
            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }

        public bool HasBeenImported(string pStrFolio)
        {
            return mObjQueryManager.Exists("U_Folio", pStrFolio, "[@UG_SU_AUTN]");
        }

        public bool HasBeenUpdated(string pStrFolio, DateTime pDtmModificationDate)
        {
            return !pDtmModificationDate.ToString("yyyy-MM-dd HH:mm").Equals(GetModificationDate(pStrFolio).ToString("yyyy-MM-dd HH:mm"));
        }

        private string GetCode(string pStrFolio)
        {
            return mObjQueryManager.GetValue("Code", "U_Folio", pStrFolio, "[@UG_SU_AUTN]");
        }

        private DateTime GetModificationDate(string pStrFolio)
        {
            DateTime lDtmLastModificationDate = Convert.ToDateTime(mObjQueryManager.GetValue("U_ModificationDate", "U_Folio", pStrFolio.ToString(), "[@UG_SU_AUTN]"));
            DateTime lDtmLastModificationTime = DateUtility.GetTime(Convert.ToInt32(mObjQueryManager.GetValue("U_ModificationTime", "U_Folio", pStrFolio.ToString(), "[@UG_SU_AUTN]")));

            var d = lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);

            return lDtmLastModificationDate.Date.Add(lDtmLastModificationTime.TimeOfDay);
        }

        public long GetAuctionId(string pStrFolio)
        {
            return mObjQueryManager.GetAuctionId(pStrFolio);
        }

        public int GetKeyByFolio(string pStrFolio)
        {
            return Convert.ToInt32(mObjQueryManager.GetValue("Code", "U_Folio", pStrFolio, "[@UG_SU_AUTN]"));
        }
    }
}
