using System;
using System.Data.Entity;
using System.Linq;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Object.Auctions.Services
{
    public class BatchService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchService mObjSapBatchService;
        UGRS.Core.Auctions.Services.Auctions.BatchService mObjLocalBatchService;
        Core.SDK.DI.Auctions.Services.BusinessPartnerSevice mObjSapBusinessPartnerService;
        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchService SapBatchService
        {
            get { return mObjSapBatchService; }
            set { mObjSapBatchService = value; }
        }

        public UGRS.Core.Auctions.Services.Auctions.BatchService LocalBatchService
        {
            get { return mObjLocalBatchService; }
            set { mObjLocalBatchService = value; }
        }

        public Core.SDK.DI.Auctions.Services.BusinessPartnerSevice SapBusinessPartnerService
        {
            get { return mObjSapBusinessPartnerService; }
            set { mObjSapBusinessPartnerService = value; }
        }

        #endregion

        #region Contructor

        public BatchService()
        {
            SapBatchService = new UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchService();
            LocalBatchService = new UGRS.Core.Auctions.Services.Auctions.BatchService(new BaseDAO<Batch>(), new BaseDAO<BatchLine>());
            SapBusinessPartnerService = new Core.SDK.DI.Auctions.AuctionsServicesFactory().GetBusinessPartnerSevice();
        }

        #endregion

        #region Methods

        public void ExportBatches(DateTime pDtAuctionDate)
        {
            DateTime lDtmAuctOrLastDate = pDtAuctionDate != DateTime.MinValue ? pDtAuctionDate : GetLastCreationDate();
            var lLstCardCodes = SapBusinessPartnerService.GetCardCodesList().ToList();

            foreach (UGRS.Core.Auctions.Entities.Auctions.Batch lObjBatch in LocalBatchService.GetList().Where(x => x.CreationDate >= lDtmAuctOrLastDate).ToList())
            {
                if (lLstCardCodes.Contains(lObjBatch.Seller != null ? lObjBatch.Seller.Code : string.Empty)
                    && lObjBatch.Buyer != null ? lLstCardCodes.Contains(lObjBatch.Buyer.Code) : true)
                {
                    if (!SapBatchService.HasBeenImported(lObjBatch.Auction.Folio, lObjBatch.Number))
                    {
                        ExportBatch(lObjBatch);
                    }
                }
            }
        }

        public void UpdateBatches(DateTime pDtAuctionDate)
        {
            DateTime lDtmAuctOrLastModification = pDtAuctionDate != DateTime.MinValue ? pDtAuctionDate : GetLastCreationDate();

            //lDtmAuctOrLastModification = Convert.ToDateTime("2019-01-29 00:00:00");

            foreach (UGRS.Core.Auctions.Entities.Auctions.Batch lObjBatch in LocalBatchService.GetList().Where(x => x.ModificationDate >= lDtmAuctOrLastModification).ToList())
            {

                if(lObjBatch.Number == 432 || lObjBatch.Number ==78)
                {

                }

                if (SapBatchService.HasBeenUpdated(lObjBatch.Number,lObjBatch.Auction.Folio, lObjBatch))
                {
                    UpdateBatch(lObjBatch);
                }
            }
        }

        private void ExportBatch(UGRS.Core.Auctions.Entities.Auctions.Batch pObjBatch)
        {
            try
            {
                if (SapBatchService.Add(GetSAPBatch(pObjBatch)) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateBatch(UGRS.Core.Auctions.Entities.Auctions.Batch pObjBatch)
        {
            try
            {
                if (SapBatchService.Update(GetSAPBatch(pObjBatch),pObjBatch.Auction.Folio) != 0)
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private UGRS.Core.SDK.DI.Auctions.Tables.Batch GetSAPBatch(UGRS.Core.Auctions.Entities.Auctions.Batch pObjBatch)
        {
            bool lBoolPerPrice = pObjBatch.ItemType.SellType == Core.Auctions.Enums.Inventory.SellTypeEnum.PerPrice ? true : false;
            int lIntReturned = pObjBatch.GoodsReturns.Where(x => !x.Removed).Select(x => (int?)x.Quantity).Sum() ?? 0;
            int lIntQuantity = pObjBatch.Quantity - lIntReturned;

            float lFltWeight = lIntReturned != 0 ? lIntQuantity * pObjBatch.AverageWeight : pObjBatch.Weight;
            float lFlAverageW = lIntReturned != 0 ? lFltWeight / lIntQuantity : pObjBatch.AverageWeight;

            //decimal lDmlPrice = (lBolPerPrice ? (pObjBatch.Price / pObjBatch.Quantity * lIntQuantity) : pObjBatch.Price);
            //decimal lDmlAmount = (lBolPerPrice ? lDmlPrice : pObjBatch.Price * (decimal)lFltWeight);
            decimal lDmlPrice = pObjBatch.Price;
            decimal lDmlAmount = lIntReturned != 0 ? lDmlPrice * (Convert.ToDecimal(pObjBatch.Weight - pObjBatch.GoodsReturns.Where(x => !x.Removed)
                .Select(x => (float)x.Weight).Sum())) : lIntReturned != 0 && lBoolPerPrice ? pObjBatch.Price * lIntQuantity : pObjBatch.Amount;

            return new UGRS.Core.SDK.DI.Auctions.Tables.Batch()
            {
                Id = GetNextBatchId(),
                AuctionId = GetSapAuctionId(pObjBatch.Auction.Folio),
                Number = pObjBatch.Number,
                SellerId = pObjBatch.SellerId ?? 0,
                SellerCode = pObjBatch.Seller != null ? pObjBatch.Seller.Code : string.Empty,
                Seller = pObjBatch.Seller != null ? pObjBatch.Seller.Name : string.Empty,
                BuyerId = pObjBatch.BuyerId ?? 0,
                BuyerCode = pObjBatch.Buyer != null ? pObjBatch.Buyer.Code : string.Empty,
                Buyer = pObjBatch.Buyer != null ? pObjBatch.Buyer.Name : string.Empty,
                ItemTypeId = pObjBatch.ItemTypeId ?? 0,
                ItemType = pObjBatch.ItemType != null ? pObjBatch.ItemType.Name : string.Empty,
                AverageWeight = pObjBatch.AverageWeight,

                //Recalculate fields
                Quantity = lIntQuantity,
                Weight = lFltWeight,
                Amount = lDmlAmount,
                Price = lDmlPrice,
                Returned = lIntReturned,
                Reprogrammed = pObjBatch.Reprogrammed,
                Unsold = pObjBatch.Unsold,
                Gender = pObjBatch.Gender,
                UnsoldMotive = (int)pObjBatch.UnsoldMotive > 0 ? pObjBatch.UnsoldMotive.GetDescription() : string.Empty,
                Protected = pObjBatch.Protected,
                Removed = pObjBatch.Removed,
                Active = pObjBatch.Active,
                CreationDate = pObjBatch.CreationDate,
                CreationTime = pObjBatch.CreationDate,
                ModificationDate =pObjBatch.ModificationDate,
                ModificationTime = pObjBatch.ModificationDate
            };
        }

        private long GetNextBatchId()
        {
            try
            {
                return SapBatchService.GetNextBatchId();
            }
            catch
            {
                return 0;
            }
        }

        private long GetSapAuctionId(string pStrAuctFolio)
        {
            try
            {
                return SapBatchService.GetAuctIdByFolio(pStrAuctFolio);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime GetLastCreationDate()
        {
            try
            {
                return SapBatchService.GetLastCreationDate();
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }

        }

        private DateTime GetLastModificationDate()
        {
            try
            {
                return SapBatchService.GetLastModificationDate();
            }
            catch
            {
                return DateTime.Today.AddYears(-10);
            }
        }

        #endregion
    }
}
