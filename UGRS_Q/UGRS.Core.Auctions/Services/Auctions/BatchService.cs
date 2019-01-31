using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public static class BatchServiceExtension
    {
        public static IList<DetailedBatchDTO> ToDetailedList(this IQueryable<Batch> pLstObjBatches)
        {
            return pLstObjBatches.AsEnumerable().Select(x => new DetailedBatchDTO(x)).ToList();
        }

        public static IList<DetailedBatchDTO> ToDetailedList(this IList<Batch> pLstObjBatches)
        {
            return pLstObjBatches.Select(x => new DetailedBatchDTO(x)).ToList();
        }
    }

    public class BatchService
    {
        #region Attributes

        private IBaseDAO<Batch> mObjBatchDAO;
        private IBaseDAO<BatchLine> mObjBatchLineDAO;

        #endregion

        #region Constructor

        public BatchService(IBaseDAO<Batch> pObjBatchDAO, IBaseDAO<BatchLine> pObjBatchLineDAO)
        {
            mObjBatchDAO = pObjBatchDAO;
            mObjBatchLineDAO = pObjBatchLineDAO;
        }

        #endregion

        #region Methods

        public Batch GetEntity(long pLonId)
        {
            return mObjBatchDAO.GetEntity(pLonId);
        }

        public IQueryable<Batch> GetList()
        {
            return mObjBatchDAO.GetEntitiesList();
        }

        public IList<DetailedBatchDTO> GetDetailedListBySeller(long pLonSellerId)
        {
            return GetList().Where(x => x.SellerId == pLonSellerId).ToDetailedList();
        }

        public IList<DetailedBatchDTO> GetDetailedListByBuyer(long pLonBuyerId)
        {
            return GetList().Where(x => x.BuyerId == pLonBuyerId).ToDetailedList();
        }

        public void SaveOrUpdate(Batch pObjBatch)
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

        public void SaveOrUpdateEntityList(List<Batch> pLstBatch)
        {
            mObjBatchDAO.SaveOrUpdateEntitiesList(pLstBatch);
        }

        public void Remove(long pLonId)
        {
            mObjBatchDAO.RemoveEntity(pLonId);
        }

        public IList<BatchEasyDTO> ConvertToDTO(List<Batch> pLstBatch, bool pBolIsReturn)
        {
            return pLstBatch.Select(b => new BatchEasyDTO
            {
                Id = b.Id,
                Number = b.Number,
                AuctionFolio = (b.Auction == null) ? string.Empty : b.Auction.Folio,
                ItemTypeCode = (b.ItemType == null) ? string.Empty : b.ItemType.Code,
                ItemTypeName = (b.ItemType == null) ? string.Empty : b.ItemType.Name,
                SellerId = (b.SellerId == null) ? 0 : b.SellerId,
                SellerName = (b.Seller == null) ? string.Empty : b.Seller.Name,
                BuyerId = (b.BuyerId == null) ? 0 : b.BuyerId,
                Buyer = (b.Buyer == null) ? string.Empty : b.Buyer.Name,

                TotalQuantity = b.Quantity,
                DeliveredQuantity = b.GoodsIssues.Sum(g => g.Quantity) - b.GoodsReturns.Sum(gr => gr.Quantity),
                ReturnedQuantity = b.GoodsReturns.Sum(gr => gr.Quantity),
                AvailableQuantity = pBolIsReturn ?
                    b.Quantity - b.GoodsIssues.Sum(g => g.Quantity) - b.GoodsReturns.Sum(gr => gr.Quantity) :
                    b.Quantity - b.GoodsIssues.Sum(g => g.Quantity),

                ExitQuantity = 0,
                ReturnQuantity = 0,

                //Lines = b.Lines.Select(l => new DetailedBatchLineDTO
                //{
                //    BatchId = l.BatchId,
                //    ItemId = l.ItemId,
                //    ItemName = l.Item.Name,
                //    TotalQuantity = l.Quantity,
                //    AvailableToReturnDelivery = pBolIsReturn ?
                //        l.Quantity - b.GoodsIssues.Where(g => g.ItemId == l.ItemId).Sum(gi => gi.Quantity) - b.GoodsReturns.Where(g => g.ItemId == l.ItemId).Sum(gr => gr.Quantity) :
                //        l.Quantity - b.GoodsIssues.Where(g => g.ItemId == l.ItemId).Sum(gi => gi.Quantity)

                //}).ToList()

            }).ToList();
        }

        public List<Batch> SearchBatches(string pStrBatch, long pLonAuctionId)
        {
            IList<IQueryable<Batch>> lLstObjQueries = new List<IQueryable<Batch>>();
            IQueryable<Batch> lLstObjBatches = this.GetList().Where(x => x.Active && x.AuctionId == pLonAuctionId);

            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().Contains(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().Equals(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().StartsWith(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().EndsWith(pStrBatch)));

            IQueryable<Batch> lLstObjBetterQuery = lLstObjBatches;
            int lIntBetterRowCount = lLstObjBatches.Count();

            for (int i = 0; i < lLstObjQueries.Count; i++)
            {
                int lIntCurrentRowCount = lLstObjQueries[i].Count();
                if (lIntCurrentRowCount > 0 && lIntCurrentRowCount < lIntBetterRowCount)
                {
                    lLstObjBetterQuery = lLstObjQueries[i];
                    lIntBetterRowCount = lIntCurrentRowCount;
                }
            }

            return lLstObjBetterQuery.ToList();
        }

        public List<Batch> SearchSoldBatches(string pStrBatch, long pLonAuctionId)
        {
            IList<IQueryable<Batch>> lLstObjQueries = new List<IQueryable<Batch>>();
            IQueryable<Batch> lLstObjBatches = this.GetList().Where(x => x.Active && x.AuctionId == pLonAuctionId && !x.Unsold && x.Quantity > 0);

            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().Contains(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().Equals(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().StartsWith(pStrBatch)));
            lLstObjQueries.Add(lLstObjBatches.Where(x => x.Number.ToString().EndsWith(pStrBatch)));

            IQueryable<Batch> lLstObjBetterQuery = lLstObjBatches;
            int lIntBetterRowCount = lLstObjBatches.Count();

            for (int i = 0; i < lLstObjQueries.Count; i++)
            {
                int lIntCurrentRowCount = lLstObjQueries[i].Count();
                if (lIntCurrentRowCount > 0 && lIntCurrentRowCount < lIntBetterRowCount)
                {
                    lLstObjBetterQuery = lLstObjQueries[i];
                    lIntBetterRowCount = lIntCurrentRowCount;
                }
            }

            return lLstObjBetterQuery.ToList();
        }

        private bool Exists(Batch pObjBatch)
        {
            return mObjBatchDAO.GetEntitiesList().Where(x => x.Number == pObjBatch.Number && x.AuctionId == pObjBatch.AuctionId && x.Id != pObjBatch.Id).Count() > 0 ? true : false;
        }

        #endregion


        public List<BatchExchangeDTO> GetBatchExchangeListToPick(long pLonAuctionId, long pLonBuyerClassificationId)
        {
            List<BatchExchangeDTO> lLstObjBatchExchange = null;

            IQueryable<Batch> lLstObjBatchList = mObjBatchDAO.GetEntitiesList()
                .Where(x => x.Active
                    && (pLonAuctionId != 0 ? x.AuctionId == pLonAuctionId : false)
                    && x.Auction.Opened
                    && (pLonBuyerClassificationId != 0 ? x.BuyerClassificationId == pLonBuyerClassificationId : false)
                    && !x.Removed
                    && x.Quantity > 0
                    );

            lLstObjBatchExchange = lLstObjBatchList
                .GroupBy(x => new BatchGroupDTO
                {
                    BatchId = (long)x.Id,
                    BuyerId = (long)x.BuyerId,
                    BuyerCode = x.Buyer.Code,
                    BuyerName = x.Buyer.Name,
                    ItemTypeId = (long)x.ItemTypeId,
                    ItemTypeName = x.ItemType.Name,

                })
                .Select(x => new BatchExchangeDTO()
                {
                    Batch = x.Select(y => y.Number).FirstOrDefault(),
                    Clasification = x.Key.ItemTypeName,
                    Quantity = x.Select(y => y.Quantity).FirstOrDefault(),
                    HasGoodIssue = (x.SelectMany(y => y.GoodsIssues).Where(y => !y.Removed).Count() + x.SelectMany(y => y.GoodsReturns).Where(y => !y.Removed).Count()) > 0 ? "Si" : "No",
                    BatchId = x.Select(y => y.Id).FirstOrDefault()
                }
                ).ToList();

            return lLstObjBatchExchange;
        }

        public int GetNextBatchNumber(long pLonAuctionId)
        {

            return mObjBatchDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId).Count() > 0 ?
                 mObjBatchDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId).Max(y => y.Number) + 1 : 1;
        }

        public bool IsEditableBatch(Batch batch)
        {
            int lIntMaxBatch = mObjBatchDAO.GetEntitiesList().Where(x => x.AuctionId == batch.AuctionId && x.Quantity > 0).Max(y => y.Number);
            int lIntMinBatch = lIntMaxBatch - 1 > 0 ? lIntMaxBatch - 1 : 1;


            return (lIntMaxBatch >= batch.Number && lIntMinBatch <= batch.Number);
        }
    }
}
