using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Exceptions;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class BatchAuctionService
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

        public BatchAuctionService(
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

        #region Auction

        public int GetSoldQuantityByAuction(long pLonAuctionId)
        {
            int lIntSoldQuantity = mObjBatchDAO.GetEntitiesList()
                                    .Where(x => x.AuctionId == pLonAuctionId && !x.Unsold)
                                    .Select(y => (int?)y.Quantity)
                                    .Sum() ?? 0;

            int lIntReturnedQuantity = mObjBatchDAO.GetEntitiesList()
                                        .Where(x => x.AuctionId == pLonAuctionId && !x.Unsold)
                                        .SelectMany(x => x.GoodsReturns)
                                        .Where(x => !x.Removed)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            return lIntSoldQuantity - lIntReturnedQuantity;
        }

        #endregion

        #region Batch

        public void SaveOrUpdateBatch(Batch pObjBatch, bool pBoolEdited = false)
        {
            if (IsSkipedBatch(pObjBatch) ||
            (
                !pObjBatch.Reprogrammed ?
                GetAvailableQuantityForEditedOnCurrentAuction(pObjBatch, (long)pObjBatch.SellerId, GetGender((long)pObjBatch.ItemTypeId)) :
                GetAvailableQuantityForReprogramEditedOnCurrentAuction(pObjBatch, (long)pObjBatch.SellerId, GetGender((long)pObjBatch.ItemTypeId))) > 0 ||
                pBoolEdited
            )
            {
                if (!Exists(pObjBatch))
                {
                    mObjBatchDAO.SaveOrUpdateEntity(pObjBatch);
                }
                else
                {
                    throw new Exception("El lote ingresado ya existe.");
                }
            }
            else
            {
                throw new Exception("No existen cabezas disponibles para registrar el lote.");
            }
        }

        private bool IsSkipedBatch(Batch pObjBatch)
        {
            return pObjBatch.SellerId == null && pObjBatch.ItemTypeId == null && pObjBatch.BuyerId == null && pObjBatch.Quantity == 0;
        }

        private bool Exists(Batch pObjBatch)
        {
            return mObjBatchDAO.GetEntitiesList().Where(x => x.Number == pObjBatch.Number && x.AuctionId == pObjBatch.AuctionId && x.Id != pObjBatch.Id).Count() > 0 ? true : false;
        }

        private void RemoveBatchLines(Batch pObjBatch)
        {
            mObjBatchLineDAO.SaveOrUpdateEntitiesList
            (
                mObjBatchLineDAO.GetEntitiesList()
                .Where(x => x.BatchId == pObjBatch.Id)
                .AsEnumerable()
                .Select(y => { y.Removed = true; return y; })
                .ToList()
            );
        }

        private void CreateBatchLines(Batch pObjBatch)
        {
            if (pObjBatch.Id != 0)
            {

                mObjBatchLineDAO.SaveOrUpdateEntitiesList(GetBatchLines
                (
                    pObjBatch.AuctionId,
                    pObjBatch.Id,
                    pObjBatch.SellerId ?? 0,
                    pObjBatch.ItemTypeId ?? 0,
                    pObjBatch.Quantity,
                    pObjBatch.Reprogrammed
                ));
            }
        }

        private IList<BatchLine> GetBatchLines(long pLonAuctionId, long pLonBatchId, long pLonSellerId, long pLonItemTypeId, int pIntQuantity, bool pBolReprogrammed)
        {
            IList<BatchLine> lLstObjResultLines = new List<BatchLine>();

            if (ExistItemTypeMapping(pLonItemTypeId))
            {
                int lIntRemainingQuantity = pIntQuantity;
                IList<ItemBatchDTO> lLstObjAvailableItemBatches = !pBolReprogrammed ?
                    //Available stock (base and temporary)
                    GetAvailableItemBatchesBySeller(pLonAuctionId, pLonSellerId) :
                    //Shared stock (unsolds and returns)
                    GetDeepAvailableItemBatchesBySeller(pLonAuctionId, pLonSellerId);

                IList<ItemDefinitionDTO> lLstObjDefinitions = GetDefinitions(pLonItemTypeId);

                //Get batch line from available item batches based on item definitions
                foreach (ItemDefinitionDTO lObjDefinition in lLstObjDefinitions.OrderBy(x => x.Order))
                {
                    if (lLstObjAvailableItemBatches.Where(x => x.ItemId == lObjDefinition.ItemId && x.Quantity > 0).Count() > 0 && lIntRemainingQuantity > 0)
                    {
                        ItemBatchDTO lObjItemBatch = lLstObjAvailableItemBatches.First(x => x.ItemId == lObjDefinition.ItemId && x.Quantity > 0);
                        int lIntQuantityToPick = lObjItemBatch.Quantity > lIntRemainingQuantity ? lIntRemainingQuantity : lObjItemBatch.Quantity;

                        lLstObjResultLines.Add(new BatchLine()
                        {
                            Quantity = lIntQuantityToPick,
                            BatchNumber = lObjItemBatch.BatchNumber,
                            BatchDate = lObjItemBatch.BatchDate,
                            ItemId = lObjItemBatch.ItemId,
                            BatchId = pLonBatchId
                        });

                        lObjItemBatch.Quantity -= lIntQuantityToPick;
                        lIntRemainingQuantity -= lIntQuantityToPick;
                    }
                }

                //Get batch line form available item batches
                foreach (ItemBatchDTO lObjItemBatch in lLstObjAvailableItemBatches)
                {
                    if (lObjItemBatch.Quantity > 0 && lIntRemainingQuantity > 0)
                    {
                        int lIntQuantityToPick = lObjItemBatch.Quantity > lIntRemainingQuantity ? lIntRemainingQuantity : lObjItemBatch.Quantity;

                        lLstObjResultLines.Add(new BatchLine()
                        {
                            Quantity = lIntQuantityToPick,
                            BatchNumber = lObjItemBatch.BatchNumber,
                            BatchDate = lObjItemBatch.BatchDate,
                            ItemId = lObjItemBatch.ItemId,
                            BatchId = pLonBatchId

                        });

                        lObjItemBatch.Quantity -= lIntQuantityToPick;
                        lIntRemainingQuantity -= lIntQuantityToPick;
                    }
                }

                if (lIntRemainingQuantity != 0)
                {
                    throw new DAOException("El cliente no tiene suficiente Stock.");
                }
            }
            else
            {
                throw new DAOException("El tipo de cabeza seleccionado no tiene artículos mapeados.");
            }

            return lLstObjResultLines;
        }

        #endregion

        #region Quantities

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

            lIntBaseStock = mObjAuctionDAO.GetEntity(pLonAuctionId).ReOpened ? GetStockFromChangesByBatch(GetBatch(pLonCustomer, lDtmAuctionDate, pEnmGender)) : lIntBaseStock;


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

            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.Unsold
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

            return lIntBaseStock + lIntTemporaryStock - lIntNotSales - (lIntSales - lIntReturns);
        }

        public int GetAvailableQuantityForEditedOnCurrentAuction(Batch pObjBatch, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
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

            lIntBaseStock = mObjAuctionDAO.GetEntity(pObjBatch.AuctionId).ReOpened ? GetStockFromChangesByBatch(GetBatch(pLonCustomer, lDtmAuctionDate, pEnmGender)) : lIntBaseStock;

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

            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && !x.Reprogrammed
                    && x.Unsold
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;


            return (lIntBaseStock + lIntTemporaryStock - lIntNotSales - (lIntSales - lIntReturns)) + pObjBatch.Quantity;
        }


        public int GetAvailableQuantityForReprogramOnCurrentAuction(long pLonAuctionId, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
        {
            int lIntNotSales = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pLonAuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.Unsold
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

            return lIntNotSales + lIntReturns;
        }

        public int GetAvailableQuantityForReprogramEditedOnCurrentAuction(Batch pObjBatch, long pLonCustomer, ItemTypeGenderEnum pEnmGender = ItemTypeGenderEnum.Otro)
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

            int lIntReturns = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Id == pObjBatch.AuctionId)
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed
                    && x.SellerId == pLonCustomer
                    && (pEnmGender != ItemTypeGenderEnum.Otro ? x.ItemType.Gender == pEnmGender : true))
                .SelectMany(x => x.GoodsReturns)
                .Where(x => !x.Removed)
                .Sum(x => (int?)x.Quantity) ?? 0;


            return (lIntNotSales + lIntReturns) + pObjBatch.Quantity;
        }


        #endregion

        #region Queries

        private ItemTypeGenderEnum GetGender(long pLonItemType)
        {
            return mObjItemTypeDAO.GetEntitiesList().Where(x => x.Id == pLonItemType).Select(x => x.Gender).FirstOrDefault();
        }

        #endregion

        #region Seller

        public int GetAvailableQuantityBySeller(long pLonSellerId)
        {
            //(+) Base stock
            return GetBaseStockQuantityBySeller(pLonSellerId) +

                   //(+) Temporary stock
                   GetTemporaryStockQuantityBySeller(pLonSellerId) -

                   //(-) Solds
                   GetSoldQuantityBySeller(pLonSellerId);
        }

        public int GetAvailableQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            //(+) Base stock
            return GetBaseStockQuantityBySeller(pLonAuctionId, pLonSellerId) +

                   //(+) Temporary stock
                   GetTemporaryStockQuantityBySeller(pLonAuctionId, pLonSellerId) -

                   //(-) Solds
                   GetSoldQuantityBySeller(pLonAuctionId, pLonSellerId);
        }

        public int GetDeepAvailableQuantityBySeller(long pLonSellerId)
        {
            //(+) Returns
            return GetReturnedQuantityBySeller(pLonSellerId) +

                   //(+) Unsolds --> and unreprogrammed
                   GetUnreprogrammedAndUnsoldQuantityBySeller(pLonSellerId) -

                   //(-) Solds --> and reprogrammed
                   GetReprogrammedAndSoldQuantityBySeller(pLonSellerId);
        }

        public int GetDeepAvailableQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            //(+) Returns
            return GetReturnedQuantityBySeller(pLonAuctionId, pLonSellerId) +

                   //(+) Unsolds --> and unreprogrammed
                   GetUnreprogrammedAndUnsoldQuantityBySeller(pLonAuctionId, pLonSellerId) -

                   //(-) Solds --> and reprogrammed
                   GetReprogrammedAndSoldQuantityBySeller(pLonAuctionId, pLonSellerId);
        }

        public int GetBaseStockQuantityBySeller(long pLonSellerId)
        {
            return GetStockBySeller(pLonSellerId)
                    .Select(y => (int?)y.Quantity)
                    .Sum() ?? 0;
        }

        public int GetBaseStockQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {

            int lIntBaseStock = 0;

            Auction lObjAuction = mObjAuctionDAO.GetEntity(pLonAuctionId);

            if (lObjAuction.ReOpened)
            {
                lIntBaseStock = GetStockFromChanges(pLonSellerId, lObjAuction.Date.ToString("yyyy-MM-dd"));
            }
            else
            {
                lIntBaseStock = GetStockBySeller(pLonAuctionId, pLonSellerId)
                    .Select(y => (int?)y.Quantity)
                    .Sum() ?? 0;
            }


            return lIntBaseStock;
        }

        private int GetStockFromChanges(long pLonSellerId, string pStrAuctionDate)
        {
            if (pLonSellerId != 0)
            {
                string lStrNameEntity = typeof(Stock).Name;

                List<string> lLstStrObject = mObjChangeDAO.GetEntitiesList().Where(x => x.ObjectType.Contains(lStrNameEntity)
                     && x.Object.Contains("\"CustomerId\": " + pLonSellerId)
                     && x.Object.Contains("\"ExpirationDate\": \"" + pStrAuctionDate)
                     && x.ChangeType == Enums.System.ChangeTypeEnum.INSERT
                     ).Select(x => x.Object).ToList();

                return DeserializeObject(lLstStrObject);
            }
            return 0;
        }

        private int GetStockFromChangesByBatch(string pStrBatch)
        {
            if (!string.IsNullOrEmpty(pStrBatch))
            {
                string lStrNameEntity = typeof(Stock).Name;

                List<string> lLstStrObject = mObjChangeDAO.GetEntitiesList().Where(x => x.ObjectType.Contains(lStrNameEntity)
            && x.Object.Contains("\"BatchNumber\": \"" + pStrBatch + "\"")
            
            && x.ChangeType == Enums.System.ChangeTypeEnum.INSERT
            ).Select(x => x.Object).ToList();

                return DeserializeObject(lLstStrObject);
            }
            return 0;
        }

        private int DeserializeObject(List<string> pLstStrObjects)
        {
            int lIntQuantity = 0;

            foreach (var item in pLstStrObjects)
            {
                Stock lObjStock = new JavaScriptSerializer().Deserialize<Stock>(item);

                lIntQuantity += lObjStock.Quantity;
            }


            return lIntQuantity;
        }

        private string GetBatch(long pLonSellerId, DateTime dateTime, ItemTypeGenderEnum pGenderType)
        {
            return mObjStockDAO.GetEntitiesList().Where(x => x.CustomerId == pLonSellerId && x.ExpirationDate == dateTime).FirstOrDefault().BatchNumber;
        }

        public int GetTemporaryStockQuantityBySeller(long pLonSellerId)
        {
            return GetTemporaryGoodsReceiptsBySeller(pLonSellerId)
                    .Select(y => (int?)y.Quantity)
                    .Sum() ?? 0;
        }

        public int GetTemporaryStockQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetTemporaryGoodsReceiptsBySeller(pLonAuctionId, pLonSellerId)
                    .Select(y => (int?)y.Quantity)
                    .Sum() ?? 0;
        }

        public int GetSoldQuantityBySeller(long pLonSellerId)
        {
            int lIntSoldQuantity = GetBatchesBySeller(pLonSellerId)
                                    .Where(x => !x.Unsold)
                                    .Select(x => (int?)x.Quantity)
                                    .Sum() ?? 0;

            int lIntReturnedQuantity = GetBatchesBySeller(pLonSellerId)
                                        .Where(x => !x.Unsold)
                                        .SelectMany(x => x.GoodsReturns)
                                        .Where(x => !x.Removed)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            return lIntSoldQuantity - lIntReturnedQuantity;
        }

        public int GetSoldQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            int lIntSoldQuantity = GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                                    .Where(x => !x.Unsold)
                                    .Select(x => (int?)x.Quantity)
                                    .Sum() ?? 0;

            int lIntReturnedQuantity = GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                                        .Where(x => !x.Unsold)
                                        .SelectMany(x => x.GoodsReturns)
                                        .Where(x => !x.Removed)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            return lIntSoldQuantity - lIntReturnedQuantity;
        }

        public int GetReturnedQuantityBySeller(long pLonSellerId)
        {
            return GetGoodsReturnsBySeller(pLonSellerId)
                    .Select(a => (int?)a.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReturnedQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetGoodsReturnsBySeller(pLonAuctionId, pLonSellerId)
                    .Select(a => (int?)a.Quantity)
                    .Sum() ?? 0;
        }

        public int GetDeliveredQuantityBySeller(long pLonSellerId)
        {
            return GetGoodsIssuesBySeller(pLonSellerId)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        public int GetDeliveredQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetGoodsIssuesBySeller(pLonAuctionId, pLonSellerId)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        public int GetUnreprogrammedAndUnsoldQuantityBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId)
                    .Where(x => x.Unsold && !x.Reprogrammed)
                    .Select(x => (int?)x.Quantity)
                    .Sum() ?? 0;
        }

        public int GetUnreprogrammedAndUnsoldQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                    .Where(x => x.Unsold && !x.Reprogrammed)
                    .Select(x => (int?)x.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReprogrammedAndSoldQuantityBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId)
                    .Where(x => !x.Unsold && x.Reprogrammed)
                    .Select(x => (int?)x.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReprogrammedAndSoldQuantityBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                    .Where(x => !x.Unsold && x.Reprogrammed)
                    .Select(x => (int?)x.Quantity)
                    .Sum() ?? 0;
        }

        #endregion

        #region Buyer

        public int GetPurchasedQuantityByBuyer(long pLonBuyerId)
        {
            int lIntPurchasedQuantity = GetBatchesByBuyer(pLonBuyerId)
                                        .Where(x => !x.Unsold)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            int lIntReturnedQuantity = GetBatchesByBuyer(pLonBuyerId)
                                        .Where(x => !x.Unsold)
                                        .SelectMany(x => x.GoodsReturns)
                                        .Where(x => !x.Removed)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            return lIntPurchasedQuantity - lIntReturnedQuantity;
        }

        public int GetPurchasedQuantityByBuyer(long pLonAuctionId, long pLonBuyerId)
        {
            int lIntPurchasedQuantity = GetBatchesByBuyer(pLonAuctionId, pLonBuyerId)
                                        .Where(x => !x.Unsold)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            int lIntReturnedQuantity = GetBatchesByBuyer(pLonAuctionId, pLonBuyerId)
                                        .Where(x => !x.Unsold)
                                        .SelectMany(x => x.GoodsReturns)
                                        .Where(x => !x.Removed)
                                        .Select(x => (int?)x.Quantity)
                                        .Sum() ?? 0;

            return lIntPurchasedQuantity - lIntReturnedQuantity;
        }

        public int GetReturnedQuantityByBuyer(long pLonBuyerId)
        {
            return GetGoodsReturnsByBuyer(pLonBuyerId)
                    .Where(g => !g.Removed)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReturnedQuantityByBuyer(long pLonAuctionId, long pLonBuyerId)
        {
            return GetGoodsReturnsByBuyer(pLonAuctionId, pLonBuyerId)
                    .Where(g => !g.Removed)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReceivedQuantityByBuyer(long pLonBuyerId)
        {
            return GetGoodsIssuesByBuyer(pLonBuyerId)
                    .Where(g => !g.Removed)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        public int GetReceivedQuantityByBuyer(long pLonAuctionId, long pLonBuyerId)
        {
            return GetGoodsIssuesByBuyer(pLonAuctionId, pLonBuyerId)
                    .Where(g => !g.Removed)
                    .Select(h => (int?)h.Quantity)
                    .Sum() ?? 0;
        }

        #endregion

        #region ItemBatch

        public IList<ItemBatchDTO> GetAvailableItemBatchesBySeller(long pLonSellerId)
        {
            //(+) Base stock
            return GetBaseItemBatchesBySeller(pLonSellerId).ToList()

                    //(+) Temporary stock
                    .Concat(GetTemporaryItemBatchesBySeller(pLonSellerId)).ToList()

                    //(-) Picket stcok
                    .Concat(GetPickedItemBatchesBySeller(pLonSellerId)
                    .Select(x => { x.Quantity = (x.Quantity * -1); return x; }).ToList())

                    // Group and sum
                    .GroupBy(g => new { g.BatchNumber, g.BatchDate, g.ItemId, g.CustomerId })
                    .Select(i => new ItemBatchDTO()
                    {
                        Quantity = i.Select(j => (int?)j.Quantity).Sum() ?? 0,
                        BatchNumber = i.Key.BatchNumber,
                        BatchDate = i.Key.BatchDate,
                        ItemId = i.Key.ItemId,
                        CustomerId = i.Key.CustomerId
                    })
                    .OrderByDescending(x => x.BatchDate)
                    .ToList();
        }

        public IList<ItemBatchDTO> GetAvailableItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            //(+) Base stock
            return GetBaseItemBatchesBySeller(pLonAuctionId, pLonSellerId).ToList()

                    //(+) Temporary stock
                    .Concat(GetTemporaryItemBatchesBySeller(pLonAuctionId, pLonSellerId)).ToList()

                    //(-) Picket stcok
                    .Concat(GetPickedItemBatchesBySeller(pLonAuctionId, pLonSellerId)
                    .Select(x => { x.Quantity = (x.Quantity * -1); return x; }).ToList())

                    // Group and sum
                    .GroupBy(g => new { g.BatchNumber, g.BatchDate, g.ItemId, g.CustomerId })
                    .Select(i => new ItemBatchDTO()
                    {
                        Quantity = i.Select(j => (int?)j.Quantity).Sum() ?? 0,
                        BatchNumber = i.Key.BatchNumber,
                        BatchDate = i.Key.BatchDate,
                        ItemId = i.Key.ItemId,
                        CustomerId = i.Key.CustomerId
                    })
                    .OrderByDescending(x => x.BatchDate)
                    .ToList();
        }

        public IList<ItemBatchDTO> GetDeepAvailableItemBatchesBySeller(long pLonSellerId)
        {
            //(+) Returns
            return GetReturnedItemBatchesBySeller(pLonSellerId).ToList()

                   //(+) Unsolds --> and unreprogrammed
                   .Concat(GetUnreprogrammedAndUnsoldItemBatchesBySeller(pLonSellerId)).ToList()

                   //(-) Solds --> and reprogrammed
                   .Concat(GetReprogrammedAndSoldItemBatchesBySeller(pLonSellerId)
                       .Select(x => { x.Quantity = (x.Quantity * -1); return x; }).ToList())

                   // Group and sum
                   .GroupBy(g => new { g.BatchNumber, g.BatchDate, g.ItemId, g.CustomerId })
                   .Select(i => new ItemBatchDTO()
                   {
                       Quantity = i.Select(j => (int?)j.Quantity).Sum() ?? 0,
                       BatchNumber = i.Key.BatchNumber,
                       BatchDate = i.Key.BatchDate,
                       ItemId = i.Key.ItemId,
                       CustomerId = i.Key.CustomerId
                   })
                   .OrderByDescending(x => x.BatchDate)
                   .ToList();
        }

        public IList<ItemBatchDTO> GetDeepAvailableItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            //(+) Returns
            return GetReturnedItemBatchesBySeller(pLonAuctionId, pLonSellerId).ToList()

                   //(+) Unsolds --> and unreprogrammed
                   .Concat(GetUnreprogrammedAndUnsoldItemBatchesBySeller(pLonAuctionId, pLonSellerId)).ToList()

                   //(-) Solds --> and reprogrammed
                   .Concat(GetReprogrammedAndSoldItemBatchesBySeller(pLonAuctionId, pLonSellerId)
                       .Select(x => { x.Quantity = (x.Quantity * -1); return x; }).ToList())

                   // Group and sum
                   .GroupBy(g => new { g.BatchNumber, g.BatchDate, g.ItemId, g.CustomerId })
                   .Select(i => new ItemBatchDTO()
                   {
                       Quantity = i.Select(j => (int?)j.Quantity).Sum() ?? 0,
                       BatchNumber = i.Key.BatchNumber,
                       BatchDate = i.Key.BatchDate,
                       ItemId = i.Key.ItemId,
                       CustomerId = i.Key.CustomerId
                   })
                   .OrderByDescending(x => x.BatchDate)
                   .ToList();
        }

        public IList<ItemBatchDTO> GetBaseItemBatchesBySeller(long pLonSellerId)
        {
            return GetStockBySeller(pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.CreationDate,
                        ItemId = x.ItemId,
                        CustomerId = x.CustomerId

                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetBaseItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetStockBySeller(pLonAuctionId, pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.CreationDate,
                        ItemId = x.ItemId,
                        CustomerId = x.CustomerId

                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetTemporaryItemBatchesBySeller(long pLonSellerId)
        {
            return GetTemporaryGoodsReceiptsBySeller(pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = x.CustomerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetTemporaryItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetTemporaryGoodsReceiptsBySeller(pLonAuctionId, pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = x.CustomerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetReturnedItemBatchesBySeller(long pLonSellerId)
        {
            return GetGoodsReturnsBySeller(pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        //BatchNumber = x.BatchNumber,
                        //BatchDate = x.BatchDate,
                        //ItemId = x.ItemId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetReturnedItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetGoodsReturnsBySeller(pLonAuctionId, pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        //BatchNumber = x.BatchNumber,
                        //BatchDate = x.BatchDate,
                        //ItemId = x.ItemId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetUnreprogrammedAndUnsoldItemBatchesBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId)
                    .Where(x => x.Unsold && !x.Reprogrammed)
                    .SelectMany(y => y.Lines)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetUnreprogrammedAndUnsoldItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                    .Where(x => x.Unsold && !x.Reprogrammed)
                    .SelectMany(y => y.Lines)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetReprogrammedAndSoldItemBatchesBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId)
                    .Where(x => !x.Unsold && x.Reprogrammed)
                    .SelectMany(y => y.Lines)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetReprogrammedAndSoldItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuctionId, pLonSellerId)
                    .Where(x => !x.Unsold && x.Reprogrammed)
                    .SelectMany(y => y.Lines)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetPickedItemBatchesBySeller(long pLonSellerId)
        {
            return GetBatchLinesBySeller(pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetPickedItemBatchesBySeller(long pLonAuctionId, long pLonSellerId)
        {
            return GetBatchLinesBySeller(pLonAuctionId, pLonSellerId)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetPickedItemBatchesBySeller(long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatchLinesBySeller(pLonSellerId, pBolReprogrammed)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        public IList<ItemBatchDTO> GetPickedItemBatchesBySeller(long pLonAuctionId, long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatchLinesBySeller(pLonAuctionId, pLonSellerId, pBolReprogrammed)
                    .Select(x => new ItemBatchDTO()
                    {
                        Quantity = x.Quantity,
                        BatchNumber = x.BatchNumber,
                        BatchDate = x.BatchDate,
                        ItemId = x.ItemId,
                        CustomerId = pLonSellerId
                    })
                    .ToList();
        }

        #endregion

        #region ItemDefinition

        public bool ExistItemTypeMapping(long pLonItemTypeId)
        {
            return ExistsMapping(pLonItemTypeId) || ExistsSubCategoryMapping(pLonItemTypeId) || ExistsCategoryMapping(pLonItemTypeId) || pLonItemTypeId == long.Parse("0");
        }

        private IList<ItemDefinitionDTO> GetDefinitions(long pLonItemTypeId)
        {
            int lIntIndex = 0;

            IList<ItemDefinitionDTO> lLstObjType = GetDefinitionsListByType(pLonItemTypeId);
            IList<ItemDefinitionDTO> lLstObjCategory = GetDefinitionsListByType(GetItemTypeParentId(pLonItemTypeId));
            IList<ItemDefinitionDTO> lLstObjSubCategory = GetDefinitionsListByType(GetItemTypeParentId(GetItemTypeParentId(pLonItemTypeId)));

            lLstObjType.ToList().ForEach(x => { x.Order = lIntIndex; lIntIndex++; });
            lLstObjCategory.ToList().ForEach(x => { x.Order = lIntIndex; lIntIndex++; });
            lLstObjSubCategory.ToList().ForEach(x => { x.Order = lIntIndex; lIntIndex++; });

            return lLstObjType
                   .Concat(lLstObjCategory)
                   .Concat(lLstObjSubCategory)
                   .ToList();
        }

        private IList<ItemDefinitionDTO> GetDefinitionsListByType(long pLonItemTypeId)
        {
            return mObjItemDefinitionDAO.GetEntitiesList()
                    .Where(x => x.ItemTypeId == pLonItemTypeId)
                       .OrderBy(a => a.Order)
                           .Select(i => new ItemDefinitionDTO()
                           {
                               Id = i.Id,
                               Order = i.Order,
                               ItemId = i.ItemId,
                               Item = string.Empty,
                               ItemTypeId = i.ItemTypeId,
                               ItemType = string.Empty

                           }).ToList();
        }

        private bool ExistsMapping(long pLongItemType)
        {
            return mObjItemDefinitionDAO.GetEntitiesList()
                    .Where(x => !x.Removed && x.ItemTypeId == pLongItemType)
                        .Count() > 0;
        }

        private bool ExistsCategoryMapping(long pLongItemType)
        {
            return ExistsMapping(GetItemTypeParentId(pLongItemType));
        }

        private bool ExistsSubCategoryMapping(long pLongItemType)
        {
            return ExistsMapping(GetItemTypeParentId(GetItemTypeParentId(pLongItemType)));
        }

        private long GetItemTypeParentId(long pLongItemType)
        {
            return mObjItemTypeDAO.GetEntitiesList().Where(x => x.Id == pLongItemType).Count() > 0 ?
                   mObjItemTypeDAO.GetEntitiesList().Where(x => x.Id == pLongItemType).Select(y => y.ParentId).FirstOrDefault() ?? 0 : 0;
        }

        #endregion

        #region Queries

        #region Auction

        public IQueryable<Auction> GetAuctions()
        {
            return mObjAuctionDAO.GetEntitiesList().Where(x => !x.Processed);
        }

        public IQueryable<Auction> GetAuctions(long pLonAuction)
        {
            return mObjAuctionDAO.GetEntitiesList().Where(x => (!x.Processed || x.ReOpened) && x.Id == pLonAuction);
        }

        #endregion

        #region Batch

        public IQueryable<Batch> GetBatches()
        {
            return GetAuctions().SelectMany(x => x.Batches).Where(y => !y.Removed && y.Quantity > 0);
        }

        public IQueryable<Batch> GetBatches(long pLonAuction)
        {
            return GetAuctions(pLonAuction).SelectMany(x => x.Batches).Where(y => !y.Removed && y.Quantity > 0);
        }

        public IQueryable<Batch> GetBatchesBySeller(long pLonSellerId)
        {
            return GetBatches().Where(x => x.SellerId == pLonSellerId);
        }

        public IQueryable<Batch> GetBatchesBySeller(long pLonAuction, long pLonSellerId)
        {
            return GetBatches(pLonAuction).Where(x => x.SellerId == pLonSellerId);
        }

        public IQueryable<Batch> GetBatchesByBuyer(long pLonBuyerId)
        {
            return GetBatches().Where(x => x.BuyerId == pLonBuyerId);
        }

        public IQueryable<Batch> GetBatchesByBuyer(long pLonAuction, long pLonBuyerId)
        {
            return GetBatches(pLonAuction).Where(x => x.BuyerId == pLonBuyerId);
        }

        public IQueryable<Batch> GetBatchesBySeller(long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatches().Where(x => x.SellerId == pLonSellerId && x.Reprogrammed == pBolReprogrammed);
        }

        public IQueryable<Batch> GetBatchesBySeller(long pLonAuction, long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatches(pLonAuction).Where(x => x.SellerId == pLonSellerId && x.Reprogrammed == pBolReprogrammed);
        }

        #endregion

        #region BatchLine

        public IQueryable<BatchLine> GetBatchLinesBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId).SelectMany(x => x.Lines).Where(x => !x.Removed);
        }

        public IQueryable<BatchLine> GetBatchLinesBySeller(long pLonAuction, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuction, pLonSellerId).SelectMany(x => x.Lines).Where(x => !x.Removed);
        }

        public IQueryable<BatchLine> GetBatchLinesBySeller(long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatchesBySeller(pLonSellerId, pBolReprogrammed).SelectMany(x => x.Lines).Where(x => !x.Removed);
        }

        public IQueryable<BatchLine> GetBatchLinesBySeller(long pLonAuction, long pLonSellerId, bool pBolReprogrammed)
        {
            return GetBatchesBySeller(pLonAuction, pLonSellerId, pBolReprogrammed).SelectMany(x => x.Lines).Where(x => !x.Removed);
        }

        #endregion

        #region Stock

        public IQueryable<Stock> GetStockBySeller(long pLonSellerId)
        {
            return mObjStockDAO.GetEntitiesList().Where(x => !x.Removed && x.CustomerId == pLonSellerId && x.Quantity > 0);
        }

        public IQueryable<Stock> GetStockBySeller(long pLonAuctionId, long pLonSellerId)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            return mObjStockDAO.GetEntitiesList()
                    .Where(x => !x.Removed && x.CustomerId == pLonSellerId && x.ExpirationDate == lDtmAuctionDate && x.Quantity > 0);
        }

        #endregion

        #region GoodsReceipt

        public IQueryable<GoodsReceipt> GetTemporaryGoodsReceiptsBySeller(long pLonSellerId)
        {
            return mObjGoodsReceiptDAO.GetEntitiesList().Where(x => !x.Removed && !x.Exported && !x.Processed && x.CustomerId == pLonSellerId && x.Quantity > 0);
        }

        public IQueryable<GoodsReceipt> GetTemporaryGoodsReceiptsBySeller(long pLonAuctionId, long pLonSellerId)
        {
            DateTime lDtmAuctionDate = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .Select(x => x.Date)
                    .FirstOrDefault();

            return mObjGoodsReceiptDAO.GetEntitiesList()
                    .Where(x => !x.Removed && !x.Exported && !x.Processed && x.CustomerId == pLonSellerId && x.ExpirationDate == lDtmAuctionDate && x.Quantity > 0);
        }

        //public IQueryable<GoodsReceipt> GetGoodsReceiptsByBuyer(long pLonBuyerId)
        //{
        //    return GetBatchesByBuyer(pLonBuyerId).SelectMany(x => x.GoodsReceipts).Where(y => !y.Removed);
        //}

        //public IQueryable<GoodsReceipt> GetGoodsReceiptsByBuyer(long pLonAuction, long pLonBuyerId)
        //{
        //    return GetBatchesByBuyer(pLonAuction, pLonBuyerId).SelectMany(x => x.GoodsReceipts).Where(y => !y.Removed);
        //}

        #endregion

        #region GoodsIssue

        public IQueryable<GoodsIssue> GetGoodsIssuesBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId).SelectMany(x => x.GoodsIssues).Where(y => !y.Removed);
        }

        public IQueryable<GoodsIssue> GetGoodsIssuesBySeller(long pLonAuction, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuction, pLonSellerId).SelectMany(x => x.GoodsIssues).Where(y => !y.Removed);
        }

        public IQueryable<GoodsIssue> GetGoodsIssuesByBuyer(long pLonBuyerId)
        {
            return GetBatchesByBuyer(pLonBuyerId).SelectMany(x => x.GoodsIssues).Where(y => !y.Removed);
        }

        public IQueryable<GoodsIssue> GetGoodsIssuesByBuyer(long pLonAuction, long pLonBuyerId)
        {
            return GetBatchesByBuyer(pLonAuction, pLonBuyerId).SelectMany(x => x.GoodsIssues).Where(y => !y.Removed);
        }

        #endregion

        #region GoodsReturn

        public IQueryable<GoodsReturn> GetGoodsReturnsBySeller(long pLonSellerId)
        {
            return GetBatchesBySeller(pLonSellerId).SelectMany(x => x.GoodsReturns).Where(y => !y.Removed);
        }

        public IQueryable<GoodsReturn> GetGoodsReturnsBySeller(long pLonAuction, long pLonSellerId)
        {
            return GetBatchesBySeller(pLonAuction, pLonSellerId).SelectMany(x => x.GoodsReturns).Where(y => !y.Removed);
        }

        public IQueryable<GoodsReturn> GetGoodsReturnsByBuyer(long pLonBuyerId)
        {
            return GetBatchesByBuyer(pLonBuyerId).SelectMany(x => x.GoodsReturns).Where(y => !y.Removed);
        }

        public IQueryable<GoodsReturn> GetGoodsReturnsByBuyer(long pLonAuction, long pLonBuyerId)
        {
            return GetBatchesByBuyer(pLonAuction, pLonBuyerId).SelectMany(x => x.GoodsReturns).Where(y => !y.Removed);
        }

        #endregion

        #endregion

        #endregion
    }
}
