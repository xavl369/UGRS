using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Reports.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.Services.Reports
{
    public static class AuctionsReportsServiceExtension
    {
        public static IQueryable<Batch> FilterByStartDate(this IQueryable<Batch> pLstObjBatches, DateTime? pDtmStartDate)
        {
            if (pDtmStartDate != null)
            {
                pDtmStartDate = ((DateTime)pDtmStartDate).Date;
                return pLstObjBatches.Where(x => x.Auction != null && x.Auction.Date >= ((DateTime)pDtmStartDate));
            }
            else
            {
                return pLstObjBatches;
            }
        }

        public static IQueryable<Batch> FilterByEndDate(this IQueryable<Batch> pLstObjBatches, DateTime? pDtmEndDate)
        {
            if (pDtmEndDate != null)
            {
                pDtmEndDate = ((DateTime)pDtmEndDate).Date.AddHours(24);
                return pLstObjBatches.Where(x => x.Auction != null && x.Auction.Date <= ((DateTime)pDtmEndDate));
            }
            else
            {
                return pLstObjBatches;
            }
        }

        public static IQueryable<Batch> FilterByAuction(this IQueryable<Batch> pLstObjBatches, string pStrAuctionFolio)
        {
            return !string.IsNullOrEmpty(pStrAuctionFolio) ?
                  pLstObjBatches.Where(x => x.AuctionId > 0 && x.Auction != null && x.Auction.Folio == pStrAuctionFolio) :
                  pLstObjBatches;
        }

        public static IQueryable<Batch> FilterByBuyer(this IQueryable<Batch> pLstObjBatches, string pStrBuyerCardCode)
        {
            return !string.IsNullOrEmpty(pStrBuyerCardCode) ?
                  pLstObjBatches.Where(x => x.BuyerId != null && x.BuyerId > 0 && x.Buyer != null && x.Buyer.Code == pStrBuyerCardCode) :
                  pLstObjBatches;
        }

        public static IQueryable<Batch> FilterBySeller(this IQueryable<Batch> pLstObjBatches, string pStrSellerCardCode)
        {
            return !string.IsNullOrEmpty(pStrSellerCardCode) ?
                  pLstObjBatches.Where(x => x.SellerId != null && x.SellerId > 0 && x.Seller != null && x.Seller.Code == pStrSellerCardCode) :
                  pLstObjBatches;
        }

        public static IQueryable<Batch> FilterByStatus(this IQueryable<Batch> pLstObjBatches, BatchStatusFilterEnum pEnmStatus)
        {
            return pEnmStatus != BatchStatusFilterEnum.ALL ?
                pLstObjBatches.Where(x => x.Unsold == (pEnmStatus == BatchStatusFilterEnum.UNSOLD)) :
                pLstObjBatches;
        }

        public static IList<ReportBatchDTO> ToDTO(this IQueryable<Batch> pLstObjBatches,BatchStatusFilterEnum pEnum = BatchStatusFilterEnum.ALL)
        {
            var lLstBatches = pLstObjBatches.ToList();

            if (pEnum != BatchStatusFilterEnum.SOLD)
            {
                lLstBatches.AddRange(
                GetReturnsAsNotSold(pLstObjBatches.SelectMany(x => x.GoodsReturns).Where(x => !x.Removed).ToList()));
            }

            
            return lLstBatches.Where(x => x.Quantity > 0).Select(b => new ReportBatchDTO()
            {
                AuctionId = b.AuctionId,
                AuctionFolio = b.AuctionId > 0 ? b.Auction.Folio : string.Empty,
                BatchId = !b.Unsold ? b.Id : 0,
                BatchNumber =b.Number,
                SellerCode = b.SellerId != null && b.SellerId > 0 ? b.Seller.Code : string.Empty,
                Seller = b.SellerId != null && b.SellerId > 0 ? b.Seller.Name : string.Empty,
                BuyerCode = b.BuyerId != null && b.BuyerId > 0  ? b.Buyer.Code : string.Empty,
                Buyer = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Name : string.Empty,
                BuyerClass = b.BuyerClassificationId != null && b.BuyerClassificationId > 0 ? b.BuyerClassification.Number.ToString() : string.Empty,
                ItemTypeCode = b.ItemTypeId > 0 ? b.ItemType.Code : string.Empty,
                ItemType = b.ItemTypeId > 0 ? b.ItemType.Name : string.Empty,
                PerPrice = b.SellType,
                Available = b.Quantity,
                Quantity = b.Quantity,
                Returned = b.GoodsReturns.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0,
                Delivered = (b.GoodsIssues.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0) - (b.GoodsReturns.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0),
                Weight = !b.Unsold ? b.Weight - (b.GoodsReturns.Where(x => !x.Removed && !x.Delivered).Select(y => (float?)y.Weight).Sum() ?? 0) : b.Weight,
                AverageWeight = b.AverageWeight,
                Price = b.Price,
                Amount = !b.Unsold ? b.Amount : 0,
                Unsold = b.Unsold,
                UnsoldMotiveId = (int)b.UnsoldMotive,
                Gender = b.Gender
            })
            .AsEnumerable()
            .Select(b =>
            {
                b.AverageWeight = !b.Unsold ?(b.Quantity - b.Returned) > 0 ? b.Weight / (b.Quantity - b.Returned) : 0 : b.AverageWeight;
                b.Available = (b.Quantity - b.Delivered - b.Returned);
                b.Weight = !b.Unsold ? ((b.Quantity - b.Returned) * b.AverageWeight): b.Weight;
                b.Quantity = !b.Unsold ? (b.Quantity - b.Returned) : b.Quantity;
                b.Amount = !b.Unsold ? (b.PerPrice ? (b.Price * (b.Quantity)) : b.Price * Convert.ToDecimal(b.Weight)) : 0;
                b.Unsold = b.Quantity == 0 || b.Unsold ? true : false;

                return b;
            })
            .ToList();
        }

        public static IList<ReportBatchDTO> ToBuyerDTO(this IQueryable<Batch> pLstObjBatches)
        {
            var lLstBatches = pLstObjBatches.ToList();

            return lLstBatches.Where(x => x.Quantity > 0).Select(b => new ReportBatchDTO()
            {
                AuctionId = b.AuctionId,
                AuctionFolio = b.AuctionId > 0 ? b.Auction.Folio : string.Empty,
                BatchId = !b.Unsold ? b.Id : 0,
                BatchNumber = b.Number,
                SellerCode = b.SellerId != null && b.SellerId > 0 ? b.Seller.Code : string.Empty,
                Seller = b.SellerId != null && b.SellerId > 0 ? b.Seller.Name : string.Empty,
                BuyerCode = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Code : string.Empty,
                Buyer = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Name : string.Empty,
                BuyerClass = b.BuyerClassificationId != null && b.BuyerClassificationId > 0 ? b.BuyerClassification.Number.ToString() : string.Empty,
                ItemTypeCode = b.ItemTypeId > 0 ? b.ItemType.Code : string.Empty,
                ItemType = b.ItemTypeId > 0 ? b.ItemType.Name : string.Empty,
                PerPrice = b.SellType,
                Available = b.Quantity,
                Quantity = b.Quantity,
                Returned = b.GoodsReturns.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0,
                Delivered = (b.GoodsIssues.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0) - (b.GoodsReturns.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0),
                Weight = !b.Unsold ? b.Weight - (b.GoodsReturns.Where(x => !x.Removed && !x.Delivered).Select(y => (float?)y.Weight).Sum() ?? 0) : b.Weight,
                AverageWeight = b.AverageWeight,
                Price = b.Price,
                Amount = !b.Unsold ? b.Amount : 0,
                Unsold = b.Unsold,
                UnsoldMotiveId = (int)b.UnsoldMotive,
                Gender = b.Gender
            })
            .AsEnumerable()
            .Select(b =>
            {
                b.AverageWeight = !b.Unsold ? (b.Quantity - b.Returned) > 0 ? b.Weight / (b.Quantity - b.Returned) : 0 : b.AverageWeight;
                b.Available = (b.Quantity - b.Delivered - b.Returned);
                b.Weight = !b.Unsold ? ((b.Quantity - b.Returned) * b.AverageWeight) : b.Weight;
                b.Quantity = !b.Unsold ? (b.Quantity - b.Returned) : b.Quantity;
                b.Amount = !b.Unsold ? (b.PerPrice ? (b.Price * (b.Quantity)) : b.Price * Convert.ToDecimal(b.Weight)) : 0;
                b.Unsold = b.Quantity == 0 || b.Unsold ? true : false;

                return b;
            })
            .ToList();
        }

        public static IList<ReportBatchDTO> ToInternalDTO(this IQueryable<Batch> pLstObjBatches)
        {
            var lLstBatches = pLstObjBatches.ToList();

            lLstBatches.AddRange(
            GetReturnsAsNotSold(pLstObjBatches.SelectMany(x => x.GoodsReturns).Where(x =>!x.Removed).ToList()));


            return lLstBatches.Where(x => x.Quantity > 0).Select(b => new ReportBatchDTO()
            {
                AuctionId = b.AuctionId,
                AuctionFolio = b.AuctionId > 0 ? b.Auction.Folio : string.Empty,
                BatchId = !b.Unsold ? b.Id : 0,
                BatchNumber = b.Number,//!b.Unsold ? b.Number : 0,
                SellerCode = b.SellerId != null && b.SellerId > 0 ? b.Seller.Code : string.Empty,
                Seller = b.SellerId != null && b.SellerId > 0 ? b.Seller.Name : string.Empty,
                BuyerCode = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Code : string.Empty,
                Buyer = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Name : string.Empty,
                ItemTypeCode = b.ItemTypeId > 0 ? b.ItemType.Code : string.Empty,
                ItemType = b.ItemTypeId > 0 ? b.ItemType.Name : string.Empty,
                PerPrice = b.SellType,
                Available = b.Quantity,
                Quantity = b.Quantity,
                Returned = b.GoodsReturns.Where(x => !x.Removed ).Select(y => (int?)y.Quantity).Sum() ?? 0,
                Delivered = (b.GoodsIssues.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0) - (b.GoodsReturns.Where(x => !x.Removed).Select(y => (int?)y.Quantity).Sum() ?? 0),
                Weight = b.Weight,//!b.Unsold ? b.Weight - (b.GoodsReturns.Where(x => !x.Removed && !x.Delivered).Select(y => (float?)y.Weight).Sum() ?? 0) : b.Weight,
                AverageWeight = b.AverageWeight,// b.GoodsReturns.Count > 0 ? (b.Weight / (b.Quantity - (b.GoodsReturns.Where(x => !x.Removed).Select(y => y.Quantity).Sum()))) : b.AverageWeight
                Price = b.Price,
                Amount = !b.Unsold ? b.Amount : 0,
                Unsold = b.Unsold,
                UnsoldMotiveId = (int)b.UnsoldMotive
            }).ToList();
        }


        private static IEnumerable<Batch> GetReturnsAsNotSold(List<GoodsReturn> pLstGoodsReturns)
        {

            return pLstGoodsReturns.Select(x => new Batch
               {
                   Id = x.BatchId,
                   Number = x.Batch.Number,
                   Quantity = x.Quantity,
                   Weight = x.Weight,
                   AverageWeight = x.Weight / x.Quantity,
                   Price = 0,//x.Batch.Price,
                   Amount = 0,
                   Unsold = true,
                   UnsoldMotive = x.ReturnMotive,
                   SellerId = x.Batch.SellerId,
                   Seller = x.Batch.Seller,
                   ItemTypeId = x.Batch.ItemTypeId,
                   ItemType = x.Batch.ItemType,
                   AuctionId = x.Batch.AuctionId,
                   Auction = x.Batch.Auction,
                   BuyerId = x.Batch.BuyerId,
                   Buyer = x.Batch.Buyer,
                   SellType = x.Batch.SellType,
                   Gender = x.Batch.Gender,
                   GoodsReturns = x.Batch.GoodsReturns,
                   GoodsIssues = x.Batch.GoodsIssues,

               }).ToList();
        }


        public static IList<ReportBatchDTO> ToDTOTrade(this IQueryable<Trade> pLstObjTrades)
        {
            return pLstObjTrades.Select(b => new ReportBatchDTO()
            {
                AuctionId = b.AuctionId,
                AuctionFolio = b.AuctionId > 0 ? b.Auction.Folio : string.Empty,
                BatchId = b.Id,
                BatchNumber = b.Number,
                SellerCode = b.SellerId != null && b.SellerId > 0 ? b.Seller.Code : string.Empty,
                Seller = b.SellerId != null && b.SellerId > 0 ? b.Seller.Name : string.Empty,
                BuyerCode = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Code : string.Empty,
                Buyer = b.BuyerId != null && b.BuyerId > 0 ? b.Buyer.Name : string.Empty,
                Amount = b.Amount
            }).ToList();
        }
    }

    public class AuctionsReportsService
    {
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;

        public AuctionsReportsService(IBaseDAO<Auction> pObjAuctionDAO, IBaseDAO<Batch> pObjBatchDAO)
        {
            mObjAuctionDAO = pObjAuctionDAO;
            mObjBatchDAO = pObjBatchDAO;
        }

        private IQueryable<Batch> GetBatchesList()
        {
            return mObjBatchDAO.GetEntitiesList();
        }

        public IList<ReportBatchDTO> GetBatchesBySeller(DateTime? pDtmStartDate, DateTime? pDtmEndDate, string pStrAuctionFolio, string pStrSellerCardCode, BatchStatusFilterEnum pEnmStatus)
        {
            return GetBatchesList()
                   .FilterByStartDate(pDtmStartDate)
                   .FilterByEndDate(pDtmEndDate)
                   .FilterByAuction(pStrAuctionFolio)
                   .FilterBySeller(pStrSellerCardCode)
                   .FilterByStatus(pEnmStatus)
                   .ToDTO(pEnmStatus);
        }

        public IList<ReportBatchDTO> GetInternalBatchesBySeller(DateTime? pDtmStartDate, DateTime? pDtmEndDate, string pStrAuctionFolio, string pStrSellerCardCode, BatchStatusFilterEnum pEnmStatus)
        {
            return GetBatchesList()
                   .FilterByStartDate(pDtmStartDate)
                   .FilterByEndDate(pDtmEndDate)
                   .FilterByAuction(pStrAuctionFolio)
                   .FilterBySeller(pStrSellerCardCode)
                   .FilterByStatus(pEnmStatus)
                   .ToInternalDTO();
        }


        public IList<ReportBatchDTO> GetBatchesByBuyer(DateTime? pDtmStartDate, DateTime? pDtmEndDate, string pStrAuctionFolio, string pStrBuyerCardCode, BatchStatusFilterEnum pEnmStatus)
        {
            return GetBatchesList()
                   .FilterByStartDate(pDtmStartDate)
                   .FilterByEndDate(pDtmEndDate)
                   .FilterByAuction(pStrAuctionFolio)
                   .FilterByBuyer(pStrBuyerCardCode)
                   .FilterByStatus(pEnmStatus)
                   .ToDTO(pEnmStatus);
        }

        public IList<ReportBatchDTO> GetBatchesByBuyer(DateTime? pDtmStartDate, DateTime? pDtmEndDate, string pStrAuctionFolio, string pStrBuyerCardCode)
        {
            return GetBatchesList()
                   .FilterByStartDate(pDtmStartDate)
                   .FilterByEndDate(pDtmEndDate)
                   .FilterByAuction(pStrAuctionFolio)
                   .FilterByBuyer(pStrBuyerCardCode)
                   .ToBuyerDTO();
        }
    }
}
