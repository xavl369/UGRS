using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.Inventory;
using System.Web;
using System.Web.Script.Serialization;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class AuctionStockService
    {
        #region Attributes

        private IBaseDAO<Stock> mObjStockDAO;
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;
        private IBaseDAO<BatchLine> mObjBatchLineDAO;
        private IBaseDAO<GoodsIssue> mObjGoodsIssueDAO;
        private IBaseDAO<GoodsReceipt> mObjGoodsReceiptDAO;
        private IBaseDAO<GoodsReturn> mObjGoodsReturnDAO;
        private IBaseDAO<Item> mObjItemDAO;
        private IBaseDAO<ItemType> mObjItemTypeDAO;
        private IBaseDAO<ItemDefinition> mObjItemDefinitionDAO;
        private IBaseDAO<ItemTypeDefinition> mObjItemTypeDefinitionDAO;
        private IBaseDAO<Change> mObjChangeDAO;

        #endregion

        #region Constructor

        public AuctionStockService(
            IBaseDAO<Stock> pObjStockDAO,
            IBaseDAO<Auction> pObjAuctionDAO,
            IBaseDAO<Batch> pObjBatchDAO,
            IBaseDAO<BatchLine> pObjBatchLineDAO,
            IBaseDAO<GoodsIssue> pObjGoodsIssueDAO,
            IBaseDAO<GoodsReceipt> pObjGoodsReceiptDAO,
            IBaseDAO<GoodsReturn> pObjGoodsReturnDAO,
            IBaseDAO<Item> pObjItemDAO,
            IBaseDAO<ItemType> pObjItemTypeDAO,
            IBaseDAO<ItemDefinition> pObjItemDefinitionDAO,
            IBaseDAO<ItemTypeDefinition> pObjItemTypeDefinitionDAO,
            IBaseDAO<Change> pObjChangeDAO)
        {
            mObjStockDAO = pObjStockDAO;
            mObjAuctionDAO = pObjAuctionDAO;
            mObjBatchDAO = pObjBatchDAO;
            mObjBatchLineDAO = pObjBatchLineDAO;
            mObjGoodsIssueDAO = pObjGoodsIssueDAO;
            mObjGoodsReceiptDAO = pObjGoodsReceiptDAO;
            mObjGoodsReturnDAO = pObjGoodsReturnDAO;
            mObjItemDAO = pObjItemDAO;
            mObjItemTypeDAO = pObjItemTypeDAO;
            mObjItemDefinitionDAO = pObjItemDefinitionDAO;
            mObjItemTypeDefinitionDAO = pObjItemTypeDefinitionDAO;
            mObjChangeDAO = pObjChangeDAO;
        }

        #endregion

        #region Methods
        public IList<Stock> GetStockList()
        {
           return mObjStockDAO.GetEntitiesList().ToList();
        }

        #region Quantities

        public int GetAvailableQuantityForFutureAuctions(long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate != lDtmAuctionDate
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate != lDtmAuctionDate
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntBaseStock + lIntTemporaryStock;
        }

        public int GetAvailableQuantityForFutureAuctions(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate != lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate != lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntBaseStock + lIntTemporaryStock;
        }

        public int GetAvailableQuantityForCurrentAuction(long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntBaseStock + lIntTemporaryStock - (lIntSales - lIntReturns);
        }

        public int GetAvailableQuantityForCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntBaseStock + lIntTemporaryStock - (lIntSales - lIntReturns);
        }

        public int GetAvailableQuantityForAuctionOnCurrentAuction(long pLonAuctionId, ItemTypeGenderEnum pEnmGender = 0)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.Unsold
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;


            return lIntBaseStock + lIntTemporaryStock - lIntNotSales - lIntSales ;
        }

        public int GetAvailableQuantityForAuctionOnCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;


            lIntBaseStock = mObjAuctionDAO.GetEntity(pLonAuctionId).ReOpened ? GetStockFromChanges(GetBatch(pLonCustomer, lDtmAuctionDate, pEnmGender)) : lIntBaseStock;


            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && x.SellerId == pLonCustomer
                    && !x.Reprogrammed
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReprogramedSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches).Where(x => !x.Removed
                && !x.Unsold
                && x.SellerId == pLonCustomer
                && x.Reprogrammed
                && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true)
                ).Sum(x => (int?)x.Quantity) ?? 0;

            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Reprogrammed
                    && x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;


            return lIntBaseStock + lIntTemporaryStock - lIntNotSales - lIntSales;
        }

        private int GetStockFromChanges(string pStrBatch)
        {
            if (!string.IsNullOrEmpty(pStrBatch))
            {
                string lStrNameEntity = typeof(Stock).Name;

               List<string> lLstObject = mObjChangeDAO.GetEntitiesList().Where(x => x.ObjectType.Contains(lStrNameEntity)
                    && x.Object.Contains("\"BatchNumber\": \"" + pStrBatch + "\"")
                    && x.ChangeType == Enums.System.ChangeTypeEnum.INSERT).Select(x=>x.Object).ToList();

               return DeserializeObject(lLstObject);
            }
            return 0;
        }

        private int DeserializeObject(List<string> lStrObject)
        {
            int lIntQuantity = 0;

            foreach (var item in lStrObject)
            {
                Stock lObjStock = new JavaScriptSerializer().Deserialize<Stock>(item);
                lIntQuantity += lObjStock.Quantity;
            }


            return lIntQuantity;
        }

        private string GetBatch(long pLonCustomer, DateTime lDtmAuctionDate, ItemTypeGenderEnum pEnmGender)
        {

            var lObjStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer).FirstOrDefault();

            return lObjStock != null ? lObjStock.BatchNumber : string.Empty;
        }

        public int GetAvailableQuantityForEditedBatchesInCurrentAuction(Batch pObjBatch, long pLonCustomer, int pIntQtty, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pObjBatch.AuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            int lIntBaseStock = mObjStockDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            lIntBaseStock = mObjAuctionDAO.GetEntity(pObjBatch.AuctionId).ReOpened ? GetStockFromChanges(GetBatch(pLonCustomer, lDtmAuctionDate, pEnmGender)) : lIntBaseStock;

            int lIntTemporaryStock = mObjGoodsReceiptDAO.GetEntitiesList()
                .Where(x => (pEnmGender != ItemTypeGenderEnum.Otro ? x.Item.Gender == pEnmGender : true)
                    && x.ExpirationDate == lDtmAuctionDate
                    && x.CustomerId == pLonCustomer
                    && !x.Processed
                    && x.Quantity > 0)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && x.SellerId == pLonCustomer
                    && !x.Reprogrammed
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReprogramedSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches).Where(x => !x.Removed
                && !x.Unsold
                && x.SellerId == pLonCustomer
                && x.Reprogrammed
                && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true)
                ).Sum(x => (int?)x.Quantity) ?? 0;

            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Reprogrammed
                    && x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            //int lIntReturns = mObjAuctionDAO.GetEntitiesList()
            //    .Where(x => x.Id == pObjBatch.AuctionId)
            //    .SelectMany(x => x.Batches)
            //    .Where(x => !x.Removed
            //        && x.SellerId == pLonCustomer
            //        && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
            //    .SelectMany(x => x.GoodsReturns)
            //    .Where(x => !x.Removed)
            //    .Sum(x => (int?)x.Quantity) ?? 0;

            int value = pObjBatch.Quantity;


            return (lIntBaseStock + lIntTemporaryStock - lIntNotSales - lIntSales ) + value;
            //return lIntBaseStock + lIntTemporaryStock - (lIntNotSales - lIntReprogramedSales) - (lIntSales - lIntReturns);
        }

        public int GetAvailableQuantityForReprogramOnCurrentAuction(long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.Unsold
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntNotSales + lIntReturns;
        }

        public int GetAvailableQuantityForReprogrammedEdited(Batch pObjBatch, long pLonCustomer, int pIntQtty, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Reprogrammed
                    && x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReprogramedSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches).Where(x => !x.Removed
                && !x.Unsold
                && x.SellerId == pLonCustomer
                && x.Reprogrammed
                && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true)
                ).Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntInverseSale = pObjBatch.Unsold && !pObjBatch.Reprogrammed ? pObjBatch.Quantity : 0;

            int lIntInverseReprogrammed = pObjBatch.Reprogrammed && !pObjBatch.Unsold ? pObjBatch.Quantity : 0;

            return (((lIntNotSales - lIntInverseSale) - (lIntReprogramedSales - lIntInverseReprogrammed)) + lIntReturns);
        }

        public int GetAvailableQuantityForReprogramOnCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Reprogrammed
                    && x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReprogramedSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches).Where(x => !x.Removed
                && !x.Unsold
                && x.SellerId == pLonCustomer
                && x.Reprogrammed
                && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true)
                ).Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return (lIntNotSales - lIntReprogramedSales) + lIntReturns;
        }

        public int GetSalesQuantityOnCurrentAuction(long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntSales - lIntReturns;
        }

        public int GetSalesQuantityOnCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntSales - lIntReturns;
        }

        public int GetPurchasesQuantityOnCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Unsold
                    && x.BuyerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.BuyerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;

            return lIntSales - lIntReturns;
        }


        #endregion

        #region Validations

        //public bool ValidStockAvailabilityForAuction(int pIntQuantity, long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        //{
        //    return GetAvailableQuantityForCurrentAuction(pLonAuctionId, pEnmGender) >= pIntQuantity;
        //}

        //public bool ValidStockAvailabilityForAuction(int pIntQuantity, long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        //{
        //    return GetAvailableQuantityForCurrentAuction(pLonAuctionId, pLonCustomer, pEnmGender) >= pIntQuantity;
        //}

        //public bool ValidStockAvailabilityForReprogram(int pIntQuantity, long pLonAuctionId, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        //{
        //    return GetAvailableQuantityForReprogramOnCurrentAuction(pLonAuctionId, pEnmGender) >= pIntQuantity;
        //}

        //public bool ValidStockAvailabilityForReprogram(int pIntQuantity, long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        //{
        //    return GetAvailableQuantityForReprogramOnCurrentAuction(pLonAuctionId, pLonCustomer, pEnmGender) >= pIntQuantity;
        //}

        #endregion

        #endregion
    }
}
