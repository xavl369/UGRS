using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class DeductionCheckService
    {
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<DeductionCheck> mObjDeductionCheckDAO;
        private IBaseDAO<Trade> mObjTradeDAO;

        public DeductionCheckService(IBaseDAO<Auction> pObjAuctionDAO, IBaseDAO<DeductionCheck> pObjDeductionCheckDAO, IBaseDAO<Trade> pObjTradeDAO)
        {
            mObjAuctionDAO = pObjAuctionDAO;
            mObjDeductionCheckDAO = pObjDeductionCheckDAO;
            mObjTradeDAO = pObjTradeDAO;
        }

        public IQueryable<DeductionCheck> GetQueryList()
        {
            return mObjDeductionCheckDAO.GetEntitiesList();
        }

        public List<DeductionCheckDTO> GetList(long pLonAuctionId)
        {
            List<DeductionCheckDTO> lLstObjResult = new List<DeductionCheckDTO>();
            List<DeductionCheckDTO> lLstObjAuctionChecks = null;

            if (mObjAuctionDAO.GetEntitiesList().Where(x => x.Id == pLonAuctionId).Select(x => x.Category == Enums.Auctions.AuctionCategoryEnum.DIRECT_TRADE).FirstOrDefault())
            {

                lLstObjAuctionChecks = mObjTradeDAO.GetEntitiesList().Where(x => x.AuctionId == pLonAuctionId).Select(y => new DeductionCheckDTO()
                {
                    Id = 0,
                    AuctionId = y.AuctionId,
                    AuctionFolio = y.Auction.Folio,
                    SellerId = (long)y.SellerId,
                    SellerCode = y.Seller.Code,
                    SellerName = y.Seller.Name,
                    Deduct = true,
                    Comments = "",
                }).Distinct()
                .ToList();
            }
            else
            {
                lLstObjAuctionChecks = mObjAuctionDAO.GetEntitiesList()
                    .Where(x => x.Id == pLonAuctionId)
                    .SelectMany(x => x.Batches.Where(y => !y.Removed && y.Seller != null)
                    .Select(y => new DeductionCheckDTO()
                    {
                        Id = 0,
                        AuctionId = x.Id,
                        AuctionFolio = x.Folio,
                        SellerId = (long)y.SellerId,
                        SellerCode = y.Seller.Code,
                        SellerName = y.Seller.Name,
                        Deduct = true,
                        Comments = "",
                    }))
                    .Distinct()
                    .ToList();
            }


            var ll = mObjDeductionCheckDAO.GetEntitiesList().ToList();

            List<DeductionCheckDTO> lLstObjCurrentChecks = mObjDeductionCheckDAO.GetEntitiesList()
            .Where(x => x.AuctionId == pLonAuctionId && x.Seller != null)
            .Select(x => new DeductionCheckDTO()
            {
                Id = x.Id,
                AuctionId = x.AuctionId,
                AuctionFolio = x.Auction.Folio,
                SellerId = x.SellerId,
                SellerCode = x.Seller.Code,
                SellerName = x.Seller.Name,
                Deduct = x.Deduct,
                Comments = x.Comments
            })
            .ToList();

            foreach (var lObjCheck in lLstObjAuctionChecks)
            {
                if (lLstObjCurrentChecks.Where(x => x.SellerId == lObjCheck.SellerId).Count() > 0)
                {
                    lLstObjResult.Add(lLstObjCurrentChecks.First(x => x.SellerId == lObjCheck.SellerId));
                }
                else
                {
                    lLstObjResult.Add(lObjCheck);
                }
            }

            return lLstObjResult;
        }

        public bool IsMarkedForDeduce(long pLonAuctionId, long pLonSellerId)
        {
            return mObjDeductionCheckDAO.GetEntitiesList()
                    .Where(x => x.AuctionId == pLonAuctionId
                        && x.SellerId == pLonSellerId)
                    .Select(x => (bool?)x.Deduct)
                    .FirstOrDefault() ?? true;
        }

        public void SaveOrUpdateList(List<DeductionCheckDTO> pLstObjCheckList)
        {
            mObjDeductionCheckDAO.SaveOrUpdateEntitiesList(pLstObjCheckList.Select(x => new DeductionCheck()
            {
                Id = x.Id,
                AuctionId = x.AuctionId,
                SellerId = x.SellerId,
                Deduct = x.Deduct,
                Comments = x.Comments
            })
            .ToList());
        }
    }
}
