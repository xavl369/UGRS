using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension.List;
using UGRS.Core.Utility;


namespace UGRS.Core.Auctions.Services.Financials
{
    public static class FoodChargeCheckServiceExtension
    {
        public static List<FoodChargeCheckDTO> ToExtendedList(this IList<FoodChargeCheck> pLstObjCheckList)
        {
            return pLstObjCheckList.GroupBy(x => new { SellerId = x.SellerId, SellerCode = x.Seller.Code, SellerName = x.Seller.Name }).Select(y => new FoodChargeCheckDTO()
            {
                SellerId = y.Key.SellerId,
                SellerCode = y.Key.SellerCode,
                SellerName = y.Key.SellerName,

                Lines = y.Select(z => new FoodChargeCheckLineDTO()
                {
                    Id = z.Id,
                    BatchNumber = z.BatchNumber,
                    BatchDate = z.BatchDate,
                    ExpirationDate = z.ExpirationDate,
                    FoodCharge = z.FoodCharge,
                    FoodDeliveries = z.FoodDeliveries,
                    AlfalfaDeliveries = z.AlfalfaDeliveries,
                    ApplyFoodCharge = z.ApplyFoodCharge

                }).ToList()

            }).ToList();
        }

        public static IList<FoodChargeCheck> ToUnextendedList(this List<FoodChargeCheckDTO> pLstObjCheckList)
        {
            return pLstObjCheckList.SelectMany(x => x.Lines.Select(y => new { SellerId = x.SellerId, Line = y })).Select(y => new FoodChargeCheck()
            {
                Id = y.Line.Id,
                SellerId = y.SellerId,
                BatchNumber = y.Line.BatchNumber,
                BatchDate = y.Line.BatchDate,
                ExpirationDate = y.Line.ExpirationDate,
                FoodCharge = y.Line.FoodCharge,
                FoodDeliveries = y.Line.FoodDeliveries,
                AlfalfaDeliveries = y.Line.AlfalfaDeliveries,
                ApplyFoodCharge = y.Line.ApplyFoodCharge,

            }).ToList();
        }
    }

    public class FoodChargeCheckService
    {
        private IBaseDAO<FoodChargeCheck> mObjFoodChargeCheckListDAO;
        private IBaseDAO<Stock> mObjStockDAO;
        private IBaseDAO<GoodsReceipt> mObjGoodsReceiptDAO;
        private IBaseDAO<FoodDelivery> mObjFoodDeliveryDAO;
        private IBaseDAO<Configuration> mObjConfigurationDAO;
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;
        private IBaseDAO<Partner> mObjPartnerDAO;
        DAO.QueryManager mObjQueryManager = new DAO.QueryManager();

        public FoodChargeCheckService(
            IBaseDAO<FoodChargeCheck> pObjFoodChargeCheckListDAO,
            IBaseDAO<Stock> pObjStockDAO,
            IBaseDAO<GoodsReceipt> pObjGoodsReceiptDAO,
            IBaseDAO<FoodDelivery> pObjFoodDeliveryDAO,
            IBaseDAO<Configuration> pObjConfigurationDAO,
            IBaseDAO<Auction> pObjAuctionDAO,
            IBaseDAO<Batch> pObjBatchDAO,
            IBaseDAO<Partner> pobjParnterDAO)
        {
            mObjFoodChargeCheckListDAO = pObjFoodChargeCheckListDAO;
            mObjStockDAO = pObjStockDAO;
            mObjGoodsReceiptDAO = pObjGoodsReceiptDAO;
            mObjFoodDeliveryDAO = pObjFoodDeliveryDAO;
            mObjConfigurationDAO = pObjConfigurationDAO;
            mObjAuctionDAO = pObjAuctionDAO;
            mObjBatchDAO = pObjBatchDAO;
            mObjPartnerDAO = pobjParnterDAO;
        }

        public FoodChargeCheckService(IBaseDAO<FoodChargeCheck> pObjFoodChargeCheckListDAO)
        {
            mObjFoodChargeCheckListDAO = pObjFoodChargeCheckListDAO;
        }

        public IList<FoodChargeCheck> GetEntitiesList()
        {
            return mObjFoodChargeCheckListDAO.GetEntitiesList().ToList();

        }

        public IList<FoodChargeCheck> GetEntitiesList(long pLonAuctionId = 1)
        {
            DateTime lObjDateTime = mObjAuctionDAO.GetEntity(pLonAuctionId).Date;

            IList<long> lLstLonSellers = mObjBatchDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId && x.SellerId != null).Select(y => y.SellerId ?? 0).Distinct().ToList();

            return mObjFoodChargeCheckListDAO.GetEntitiesList().Where(x => lLstLonSellers.Contains(x.SellerId) && DbFunctions.TruncateTime(x.ExpirationDate) == lObjDateTime).ToList();
        }

        public IList<FoodChargeCheck> GetList()
        {
            return PopulatedList(GetUpdatedCheckList().ToList());
        }

        public IList<FoodChargeCheck> GetList(long pLonAuctionId)
        {
            IList<long> lLstLonSellers = mObjBatchDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId && x.SellerId != null).Select(y => y.SellerId ?? 0).Distinct().ToList();

            string lStrDate = mObjAuctionDAO.GetEntity(pLonAuctionId).Date.ToString("yyyy-MM-dd");

            return PopulatedList(GetUpdatedCheckList(pLonAuctionId).Where(w => lLstLonSellers.Contains(w.SellerId) && w.ExpirationDate.ToString("yyyy-MM-dd") == lStrDate).ToList());
        }

        public IList<FoodChargeCheck> GetList(long pLonAuctionId, long pLonSellerId)
        {
            return PopulatedList(GetUpdatedCheckList().Where(w => w.SellerId == pLonSellerId).ToList());
        }

        //public bool ApplyFoodCharge(long pLonSellerId,DateTime pAuctionDate)
        //{
        //    return Exists(pLonSellerId, pAuctionDate) ? GetApplyFoodCharge(pStrBatchNumber, pLonSellerId) : CalculateApplyFoodCharge(pStrBatchNumber, pLonSellerId);
        //}

        public void SaveOrUpdateList(IList<FoodChargeCheck> pLstObjCheckList)
        {
            mObjFoodChargeCheckListDAO.SaveOrUpdateEntitiesList(pLstObjCheckList);
        }

        public void Save(FoodChargeCheck pObjFoodChargeCheck)
        {
            mObjFoodChargeCheckListDAO.SaveOrUpdateEntity(pObjFoodChargeCheck);
        }
        //private bool Exists(long pLonSellerId, DateTime pAuctionDate)
        //{
        //    return mObjFoodChargeCheckListDAO.GetEntitiesList().Where(x => x.BatchNumber.Equals(pStrBatchNumber) && x.SellerId == pLonSellerId).Count() > 0;
        //}

        private bool GetApplyFoodCharge(string pStrBatchNumber, long pLonSellerId)
        {
            return mObjFoodChargeCheckListDAO.GetEntitiesList().Where(x => x.BatchNumber.Equals(pStrBatchNumber) && x.SellerId == pLonSellerId).Select(y => y.ApplyFoodCharge).FirstOrDefault();
        }

        private bool CalculateApplyFoodCharge(string pStrBatchNumber, long pLonSellerId)
        {
            return GetUpdatedCheckList().Where(x => x.BatchNumber.Equals(pStrBatchNumber) && x.SellerId == pLonSellerId).Select(y => y.ApplyFoodCharge).FirstOrDefault();
        }

        private IList<FoodChargeCheck> PopulatedList(IList<FoodChargeCheck> pLstObjCheckList)
        {
            //Get check query list

            IQueryable<FoodChargeCheck> lLstObjCurrent = mObjFoodChargeCheckListDAO.GetEntitiesList();

            //Get update checks
            var lVarLstCheckList = pLstObjCheckList

            //Populate Id
            .Select(x =>
            {
                x.Id = lLstObjCurrent
                    .Where(y => y.BatchNumber == x.BatchNumber && y.BatchDate == x.BatchDate)
                    .Select(z => z.Id).FirstOrDefault();

                return x;
            })

            //Populate ApplyFoodCharge
            .Select(x =>
            {
                x.ApplyFoodCharge = lLstObjCurrent.Where(y => y.Id == x.Id).Select(z => z.ApplyFoodCharge).Count() > 0 ? lLstObjCurrent.Where(y => y.Id == x.Id).Select(z => z.ApplyFoodCharge).FirstOrDefault() : x.ApplyFoodCharge;
                    //!x.FoodCharge && (x.AlfalfaDeliveries || x.FoodDeliveries) ? lLstObjCurrent.Where(y => y.Id == x.Id).Select(z => z.ApplyFoodCharge).FirstOrDefault() : x.ApplyFoodCharge;
                return x;
            })

            //To list
            .ToList();

            //SaveOrUpdateList(lVarLstCheckList);

            return lVarLstCheckList;

        }

        #region Queries

        private IList<FoodChargeCheck> GetUpdatedCheckList(long pLonAuctionId = 0)
        {
            int lIntLocation = 0;
            IQueryable<FoodDelivery> lLstObjFoodDeliveries = mObjFoodDeliveryDAO.GetEntitiesList();

            string lStrAlfalfa = mObjConfigurationDAO.GetEntitiesList().Where(x => x.Key == ConfigurationKeyEnum.FOOD_ITEM_CODE).Select(y => y.Value).FirstOrDefault();
            if(pLonAuctionId != 0)
            {
                lIntLocation = (int)mObjAuctionDAO.GetEntity(pLonAuctionId).Location;
            }

            return GetStockItemBatches(pLonAuctionId)
                    .Concat(GetTemporaryStockItemBatches())
                    .ToList()
                    .Select(x =>
                    {
                        x.FoodDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode != lStrAlfalfa).Count() > 0;
                        x.AlfalfaDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode == lStrAlfalfa).Count() > 0;
                        //x.ApplyFoodCharge = (!x.FoodDeliveries || x.FoodDeliveries && !x.AlfalfaDeliveries) ? false : x.ApplyFoodCharge;
                        x.ApplyFoodCharge = lIntLocation != 0 && lIntLocation == (int)Enums.Auctions.LocationEnum.HERMOSILLO ?
                            x.FoodCharge && !x.AlfalfaDeliveries ? true :
                            !x.FoodCharge && x.AlfalfaDeliveries ? false :
                            !x.FoodDeliveries && !x.AlfalfaDeliveries ? true : false : x.ApplyFoodCharge;
                        return x;
                    })
                    .DistinctBy(y => new { y.BatchNumber, y.BatchDate, y.SellerId })
                    .ToList();

        }

        private IList<FoodChargeCheck> GetStockItemBatches(long pLonAuctionId)
        {

            DateTime lObjAuctDate =pLonAuctionId != 0 ? 
                mObjAuctionDAO.GetEntitiesList().Where(x => x.Id == pLonAuctionId).Select(x => x.Date).FirstOrDefault() : DateTime.MinValue;

            return mObjStockDAO.GetEntitiesList().ToList()
                .Where(x => x.Quantity > 0
                    && x.Payment
                    && lObjAuctDate != DateTime.MinValue ? x.ExpirationDate >= lObjAuctDate : true
                //&& lObjAuctDate != DateTime.MinValue ? x.ExpirationDate == lObjAuctDate : true
                    )
                    .AsEnumerable()
                    .Select(y => new FoodChargeCheck()
                    {
                        BatchNumber = y.BatchNumber,
                        BatchDate = y.CreationDate,
                        ExpirationDate = y.ExpirationDate,
                        SellerId = y.CustomerId,
                        Seller = y.Customer,
                        FoodCharge = y.ChargeFood
                    })
                    .ToList();


        }

        private IList<FoodChargeCheck> GetTemporaryStockItemBatches()
        {

            return mObjGoodsReceiptDAO.GetEntitiesList().ToList()
                    .Where(x => !x.Exported && !x.Processed && x.Quantity > 0)
                    .AsEnumerable()
                    .Select(y => new FoodChargeCheck()
                    {
                        BatchNumber = y.BatchNumber,
                        BatchDate = y.BatchDate,
                        ExpirationDate = y.BatchDate,
                        SellerId = y.CustomerId,
                        Seller = y.Customer,
                        FoodCharge = true
                    })
                    .ToList();
        }

        #endregion
    }
}
