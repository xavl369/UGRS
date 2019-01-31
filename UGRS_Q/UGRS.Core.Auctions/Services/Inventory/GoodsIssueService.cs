using System.Linq;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Auctions;
using System.Collections.Generic;
using UGRS.Core.Auctions.Entities.Auctions;
using System;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Services;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class GoodsIssueService
    {
        private IBaseDAO<GoodsIssue> mObjGoodsIssueDAO;
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;

        public GoodsIssueService(IBaseDAO<GoodsIssue> pObjGoodsIssueDAO, IBaseDAO<Auction> pObjAuctionDAO, IBaseDAO<Batch> pObjBatchDAO)
        {
            mObjGoodsIssueDAO = pObjGoodsIssueDAO;
            mObjAuctionDAO = pObjAuctionDAO;
            mObjBatchDAO = pObjBatchDAO;
        }

        public IQueryable<GoodsIssue> GetList()
        {
            return mObjGoodsIssueDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(GoodsIssue pObjGoodsIssue)
        {
            mObjGoodsIssueDAO.SaveOrUpdateEntity(pObjGoodsIssue);
        }

        public void Remove(long pLonId)
        {
            mObjGoodsIssueDAO.RemoveEntity(pLonId);
        }

        public List<GoodsIssueDTO> GetListToPick(long pLonAuctionId = 0, long pLonBuyerId = 0)
        {
            List<GoodsIssueDTO> lLstObjResult = null;

            IQueryable<Batch> lLstObjBatchList = mObjAuctionDAO.GetEntitiesList()
                .Where(x => x.Active 
                    && x.Opened 
                    && (pLonAuctionId != 0 ? x.Id == pLonAuctionId : true))
                .SelectMany(x => x.Batches)
                .Where(x => !x.Removed 
                    && (pLonBuyerId != 0 ? x.BuyerId == pLonBuyerId : true));


            lLstObjResult = lLstObjBatchList
            .Where(x => x.BuyerId != null && x.ItemTypeId != null)
            .GroupBy(x => new BatchGroupDTO
            {
                BuyerId = (long)x.BuyerId,
                BuyerCode = x.Buyer.Code,
                BuyerName = x.Buyer.Name,
                Gender = x.ItemType.Gender,
                //ItemTypeId = (long)x.ItemTypeId,
                //ItemTypeCode = x.ItemType.Code,
                //ItemTypeName = x.ItemType.Name,
            })
            .Select(x => new GoodsIssueDTO()
            {
                BuyerId = (long)x.Key.BuyerId,
                BuyerCode = x.Key.BuyerCode,
                BuyerName = x.Key.BuyerName,
                Gender = x.Key.Gender,
                ItemTypeId = x.Select(y => (long)y.ItemTypeId).FirstOrDefault(),
                ItemTypeCode = x.Select(y => y.ItemType.Code).FirstOrDefault(),
                ItemTypeName = x.Select(y => y.ItemType.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => (int?)y.Quantity).Sum() ?? 0,

                DeliveredQuantity = x.SelectMany(y => y.GoodsIssues)
                .Where(y => !y.Removed)
                .Select(y => (int?)y.Quantity)
                .Sum() ?? 0,

                ReturnedDeliveriesQuantity = x.SelectMany(y => y.GoodsReturns)
                .Where(y => !y.Removed && y.Delivered)
                .Select(y => (int?)y.Quantity)
                .Sum() ?? 0,

                ReturnedQuantity = x.SelectMany(y => y.GoodsReturns)
                .Where(y => !y.Removed)
                .Select(y => (int?)y.Quantity)
                .Sum() ?? 0,

                AvailableQuantity = 0,
                QuantityToPick = 0,

                // Populate batches
                Batches = x.Select(y=> new GoodsIssueLineDTO()
                {
                    BatchId = y.Id,
                    Quantity = y.Quantity,
                    DeliveredQuantity = y.GoodsIssues.Where(z=> !z.Removed).Select(z=> (int?)z.Quantity).Sum() ?? 0,
                    ReturnedDeliveriesQuantity = y.GoodsReturns.Where(z=> !z.Removed && z.Delivered).Select(z=> (int?)z.Quantity).Sum() ?? 0,
                    ReturnedQuantity = y.GoodsReturns.Where(z=> !z.Removed).Select(z=> (int?)z.Quantity).Sum() ?? 0,
                    AvailableQuantity = 0,
                    Gender = y.ItemType.Gender
                })
                .ToList()
            })
            .ToList();

            lLstObjResult = lLstObjResult.Select(x =>
            {
                x.DeliveredQuantity = x.DeliveredQuantity - x.ReturnedDeliveriesQuantity;
                x.AvailableQuantity = x.TotalQuantity - x.DeliveredQuantity - x.ReturnedQuantity;
                x.Batches = x.Batches.Select(y =>
                {
                    y.DeliveredQuantity = y.DeliveredQuantity - y.ReturnedDeliveriesQuantity;
                    y.AvailableQuantity = y.Quantity - y.DeliveredQuantity - y.ReturnedQuantity;
                    return y;
                })
                .ToList();
                return x;
            })
            .ToList();

            return lLstObjResult.Where(x => x.AvailableQuantity > 0).ToList();
        }

        public void CreateGoodsIssues(IList<DetailedBatchDTO> pLstObjDetailedBatches)
        {
            //foreach (var lObjBatchItem in pLstObjDetailedBatches)
            //{
            //    //Valid availability
            //    if (lObjBatchItem.QuantityToPick > 0 && lObjBatchItem.AvailableQuantityToDelivery >= lObjBatchItem.QuantityToPick)
            //    {
            //        //Get item batches from lines
            //        foreach (var lObjLine in lObjBatchItem.Lines.Where(x => x.AvailableQuantityToDelivery > 0))
            //        {
            //            if (lObjBatchItem.QuantityToPick > 0 && lObjBatchItem.AvailableQuantityToDelivery >= lObjBatchItem.QuantityToPick)
            //            {
            //                //Get quantity to apply
            //                int lIntQuantityToApply = lObjBatchItem.QuantityToPick > lObjLine.AvailableQuantityToDelivery ? lObjLine.AvailableQuantityToDelivery : lObjBatchItem.QuantityToPick;

            //                //Create Goods Issue
            //                GoodsIssue lObjGoodIssue = new GoodsIssue()
            //                {
            //                    BatchId = lObjBatchItem.Id,
            //                    Quantity = lIntQuantityToApply,
            //                    //BatchNumber = lObjLine.BatchNumber,
            //                    //BatchDate = lObjLine.BatchDate,
            //                    //CustomerId = lObjBatchItem.BuyerId,
            //                    //ItemId = lObjLine.ItemId,
            //                    Exported = false
            //                };

            //                //Add Goods Issue
            //                mObjGoodsIssueDAO.AddEntity(lObjGoodIssue);

            //                //Batch
            //                lObjBatchItem.DeliveredQuantity += lIntQuantityToApply;
            //                lObjBatchItem.AvailableQuantityToDelivery -= lIntQuantityToApply;
            //                lObjBatchItem.QuantityToPick -= lIntQuantityToApply;

            //                //Line
            //                lObjLine.DeliveredQuantity += lIntQuantityToApply;
            //                lObjLine.AvailableQuantityToDelivery -= lIntQuantityToApply;
            //            }
            //        }

            //        //Update Batch
            //        mObjBatchDAO.SaveOrUpdateEntity(mObjBatchDAO.GetEntity(lObjBatchItem.Id));
            //    }
            //}
        }

        public List<long> CreateGoodsIssues(IList<GoodsIssueDTO> pLstObjPickList)
        {
            List<GoodsIssue> lLstObjResultList = new List<GoodsIssue>();

            foreach (var lObjListToPick in pLstObjPickList)
            {
                //Valid availability
                if (lObjListToPick.QuantityToPick > 0 && lObjListToPick.AvailableQuantity >= lObjListToPick.QuantityToPick)
                {
                    //Get item batches
                    foreach (var lObjBatch in lObjListToPick.Batches.Where(x=> x.AvailableQuantity > 0 && x.Gender == lObjListToPick.Gender))
                    {
                        if (lObjListToPick.QuantityToPick > 0 && lObjListToPick.AvailableQuantity >= lObjListToPick.QuantityToPick)
                        {
                            //Get quantity to apply
                            int lIntQuantityToApply = lObjListToPick.QuantityToPick > lObjBatch.AvailableQuantity ? lObjBatch.AvailableQuantity : lObjListToPick.QuantityToPick;

                            //Create Goods Issue
                            GoodsIssue lObjGoodIssue = new GoodsIssue()
                            {
                                BatchId = lObjBatch.BatchId,
                                Quantity = lIntQuantityToApply,
                                Exported = false
                            };

                            //Add Goods Issue
                            lLstObjResultList.Add(lObjGoodIssue);
                            //Batch
                            lObjListToPick.AvailableQuantity -= lIntQuantityToApply;
                            lObjListToPick.QuantityToPick -= lIntQuantityToApply;

                            //Line
                            lObjBatch.AvailableQuantity -= lIntQuantityToApply;
                        }
                    }
                }
            }

            int lIntNumber = GetNextNumber();
            foreach (long lLonBuyer in GetBuyersListByGoodsIssuesList(lLstObjResultList))
            {
                foreach (long lLonBatch in GetBatchesListByGoodsIssuesListAndBuyer(lLstObjResultList, lLonBuyer))
                {
                    foreach (GoodsIssue lObjGoodIssue in lLstObjResultList.Where(x=> x.BatchId == lLonBatch))
                    {
                        lObjGoodIssue.Number = lIntNumber;
                        lObjGoodIssue.Folio = string.Concat(DateTime.Now.ToString("yyMMdd"), lIntNumber.ToString("0000"));
                    }
                }
            }

            mObjGoodsIssueDAO.AddEntitiesList(lLstObjResultList);
            LogService.WriteInfo("Good Issue Created");
            return lLstObjResultList
                .Select(x=> x.Id)
                .ToList();
        }

        private List<long> GetBatchesListByGoodsIssuesListAndBuyer(List<GoodsIssue> pLstGoodsIssuesList, long pLonBuyer)
        {
            List<long> lLstLonBatches = pLstGoodsIssuesList
                .Select(x => x.BatchId)
                .ToList();

            return mObjBatchDAO.GetEntitiesList()
                .Where(x => lLstLonBatches.Contains(x.Id)
                    && x.BuyerId == pLonBuyer)
                .Select(x => x.Id)
                .ToList(); 
        }

        private List<long> GetBuyersListByGoodsIssuesList(List<GoodsIssue> pLstGoodsIssuesList)
        {
            List<long> lLstLonBatches = pLstGoodsIssuesList
                .Select(x => x.BatchId)
                .ToList();

            return mObjBatchDAO.GetEntitiesList()
                .Where(x => lLstLonBatches.Contains(x.Id)
                    && x.BuyerId != null
                    && x.BuyerId != 0)
                .Select(x => (long)x.BuyerId)
                .Distinct()
                .ToList();
        }

        private int GetNextNumber()
        {
            DateTime lDtmStartDate = DateTime.Now.Date;
            DateTime lDtmEndDate = DateTime.Now.Date.AddHours(24);

            return (mObjGoodsIssueDAO.GetEntitiesList()
                .Where(x => x.CreationDate >= lDtmStartDate
                    && x.CreationDate <= lDtmEndDate)
                .Select(x => (int?)x.Number)
                .FirstOrDefault() ?? 0) + 1;
        }
    }
}
