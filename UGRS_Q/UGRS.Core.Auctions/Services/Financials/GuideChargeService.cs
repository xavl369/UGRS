using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public static class GuideChargeServiceExtension
    {
        public static IList<GuideCharge> ToEntityList(this List<GuideChargeDTO> pLstObjGuideCharges)
        {
            return pLstObjGuideCharges.AsEnumerable().Select(x => new GuideCharge()
            {
                Id = x.Id,
                AuctionId = x.AuctionId,
                SellerId = x.SellerId,
                Amount = x.Amount

            }).ToList();
        }
    }

    public class GuideChargeService
    {
        private IBaseDAO<GuideCharge> mObjGuideChargeDAO;
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;

        public GuideChargeService(IBaseDAO<GuideCharge> pObjGuideChargeDAO, IBaseDAO<Auction> pObjAuctionDAO, IBaseDAO<Batch> pObjBatchDAO)
        {
            mObjGuideChargeDAO = pObjGuideChargeDAO;
            mObjAuctionDAO = pObjAuctionDAO;
            mObjBatchDAO = pObjBatchDAO;
        }

        public IQueryable<GuideCharge> GetQueryList()
        {
            return mObjGuideChargeDAO.GetEntitiesList();
        }

        public IList<GuideChargeDTO> GetList(long pLonAuctionId)
        {
            return PopulateList(GetGuideChargesList(pLonAuctionId));
        }

        public IList<GuideChargeDTO> GetList(long pLonAuctionId, long pLonSellerId)
        {
            return PopulateList(GetGuideChargesList(pLonAuctionId, pLonSellerId));
        }

        public void SaveOrUpdateList(IList<GuideCharge> pLstObjGuideCharges)
        {
            mObjGuideChargeDAO.SaveOrUpdateEntitiesList(pLstObjGuideCharges);
        }

        private IList<GuideChargeDTO> PopulateList(IList<GuideChargeDTO> pLstObjGuideCharges)
        {
            IQueryable<GuideCharge> lLstObjGuideCharges = mObjGuideChargeDAO.GetEntitiesList();

            return pLstObjGuideCharges

            //Populate Id
            .Select(x =>
            {
                x.Id = lLstObjGuideCharges.Where(y => y.AuctionId == x.AuctionId && y.SellerId == x.SellerId).Select(z => z.Id).FirstOrDefault();
                return x;
            })

            //Populate Amount
            .Select(x =>
            {
                x.Amount = x.Id != 0 ? lLstObjGuideCharges.Where(y => y.Id == x.Id).Select(z => z.Amount).FirstOrDefault() : x.Amount;
                return x;
            })

            //To list
            .ToList();
        }

        #region Queries

        private IList<GuideChargeDTO> GetGuideChargesList(long pLonAuctionId)
        {
            return mObjAuctionDAO.GetEntitiesList()
                   .Where(x => x.Id == pLonAuctionId && !x.Processed)
                   .SelectMany(y => y.Batches.Where(z => !z.Removed && z.Seller != null).AsEnumerable().Select(a => new { Auction = y, Seller = a.Seller }))
                   .Select(x => new GuideChargeDTO()
                    {
                        AuctionId = x.Auction.Id,
                        AuctionFolio = x.Auction.Folio,
                        SellerId = x.Seller.Id,
                        SellerCode = x.Seller.Code,
                        SellerName = x.Seller.Name,
                        Amount = 0
                    })
                    .Distinct()
                    .ToList();
        }

        private IList<GuideChargeDTO> GetGuideChargesList(long pLonAuctionId, long pLonSellerId)
        {
            return mObjAuctionDAO.GetEntitiesList()
                   .Where(x => x.Id == pLonAuctionId && (!x.Processed || x.ReOpened) )
                   .SelectMany(y => y.Batches.Where(z => !z.Removed && z.Seller != null && z.SellerId == pLonSellerId).AsEnumerable().Select(a => new { Auction = y, Seller = a.Seller }))
                   .Select(x => new GuideChargeDTO()
                   {
                       AuctionId = x.Auction.Id,
                       AuctionFolio = x.Auction.Folio,
                       SellerId = x.Seller.Id,
                       SellerCode = x.Seller.Code,
                       SellerName = x.Seller.Name,
                       Amount = 0
                   })
                    .Distinct()
                    .ToList();
        }

        public IList<GuideCharge> GetGuideCharges(long pLonAuctionId)
        {
            return mObjGuideChargeDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId).ToList();
        }

        #endregion
    }
}
