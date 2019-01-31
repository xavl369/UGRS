using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Extension.Enum;

namespace UGRS.Object.Auctions.Services
{
    public class BatchLineService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchLineService mObjSapBatchLineService;
        UGRS.Core.Auctions.Services.Auctions.BatchLineService mObjLocalBatchLineService;

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchLineService SapBatchLineService
        {
            get { return mObjSapBatchLineService; }
            set { mObjSapBatchLineService = value; }
        }

        public UGRS.Core.Auctions.Services.Auctions.BatchLineService LocalBatchLineService
        {
            get { return mObjLocalBatchLineService; }
            set { mObjLocalBatchLineService = value; }
        }

        #endregion

        #region Contructor

        public BatchLineService()
        {
            SapBatchLineService = new UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchLineService();
            LocalBatchLineService = new UGRS.Core.Auctions.Services.Auctions.BatchLineService(new BaseDAO<UGRS.Core.Auctions.Entities.Auctions.BatchLine>());
        }

        #endregion

        #region Methods

        public void ExportBatchLines(string pStrLocation)
        {
            DateTime lDtmLastCreationDate = GetLastCreationDate(pStrLocation);
            foreach (UGRS.Core.Auctions.Entities.Auctions.BatchLine lObjBatch in LocalBatchLineService.GetList().Where(x => x.CreationDate >= lDtmLastCreationDate).ToList())
            {
                if (!SapBatchLineService.HasBeenImported(lObjBatch.Id))
                {
                    ExportBatchLine(lObjBatch);
                }
            }
        }

        public void UpdateBatchLines(string pStrLocation)
        {
            DateTime lDtmLastModificationDate = GetLastModificationDate(pStrLocation);
            foreach (UGRS.Core.Auctions.Entities.Auctions.BatchLine lObjBatch in LocalBatchLineService.GetList().Where(x => x.ModificationDate >= lDtmLastModificationDate).ToList())
            {
                if (SapBatchLineService.HasBeenUpdated(lObjBatch.Id, lObjBatch.ModificationDate))
                {
                    UpdateBatchLine(lObjBatch);
                }
            }
        }

        private void ExportBatchLine(UGRS.Core.Auctions.Entities.Auctions.BatchLine pObjBatch)
        {
            try
            {
                if (SapBatchLineService.Add(GetSAPBatch(pObjBatch)) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateBatchLine(UGRS.Core.Auctions.Entities.Auctions.BatchLine pObjBatch)
        {
            try
            {
                if (SapBatchLineService.Update(GetSAPBatch(pObjBatch)) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private UGRS.Core.SDK.DI.Auctions.Tables.BatchLine GetSAPBatch(UGRS.Core.Auctions.Entities.Auctions.BatchLine pObjBatch)
        {
            int lIntReturned = pObjBatch.Batch.GoodsReturns.Where(x => !x.Removed && x.Id == pObjBatch.ItemId && x.Batch.Number.ToString() == pObjBatch.BatchNumber).Select(x => (int?)x.Quantity).Sum() ?? 0;
            int lIntQuantity = pObjBatch.Batch.Quantity - lIntReturned;

            return new UGRS.Core.SDK.DI.Auctions.Tables.BatchLine()
            {
                Id = pObjBatch.Id,
                BatchId = pObjBatch.BatchId,
                ItemId = pObjBatch.ItemId,
                Item = pObjBatch.Item.Name,

                //Recalculate fields
                Quantity = lIntQuantity,
                Returned = lIntReturned,

                Protected = pObjBatch.Protected,
                Removed = pObjBatch.Removed,
                Active = pObjBatch.Active,
                CreationDate = pObjBatch.CreationDate,
                CreationTime = pObjBatch.CreationDate,
                ModificationDate = pObjBatch.ModificationDate,
                ModificationTime = pObjBatch.ModificationDate
            };
        }

        private DateTime GetLastCreationDate(string pStrLocation)
        {
            try
            {
                return SapBatchLineService.GetLastCreationDate(pStrLocation);
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }

        }

        private DateTime GetLastModificationDate(string pStrLocation)
        {
            try
            {
                return SapBatchLineService.GetLastModificationDate(pStrLocation);
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }
        }

        #endregion
    }
}
