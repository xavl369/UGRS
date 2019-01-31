using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DAO.Inventory;
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Utility;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class AuctionService
    {
        private IStockAuditDAO mObjStockAuditDAO;
        private IBaseDAO<Auction> mObjAuctionDAO;
        private IBaseDAO<Batch> mObjBatchDAO;

        public AuctionService(IBaseDAO<Auction> pObjAuctionDAO)
        {
            mObjAuctionDAO = pObjAuctionDAO;
        }

        public Auction Get(long pLonAuctionId)
        {
            return mObjAuctionDAO.GetEntity(pLonAuctionId);
        }

        public Auction Get(string pStrFolio)
        {
            if (IsValid(pStrFolio))
            {
                return mObjAuctionDAO.GetEntitiesList().Where(x => x.Folio == pStrFolio).FirstOrDefault();
            }
            else
            {
                throw new Exception(string.Format("No se encontró ninguna subasta con el folio {0}.", pStrFolio));
            }
        }

        public Auction GetCurrentOrLast(AuctionCategoryEnum pEnmCategory = 0)
        {
            /* # Categories 
               - Subasta = 1,
               - Trato directo = 2,
               - Borregos = 3 
             */

            DateTime lDtmStartDate = DateTime.Now.Date;
            DateTime lDtmEndDate = DateTime.Now.Date.AddHours(24);

            IQueryable<Auction> lLsObjAuctionList = mObjAuctionDAO.GetEntitiesList().Where(x => x.Active && x.Opened);

            //if (pEnmCategory != 0)
            //{
            //    //lLsObjAuctionList = lLsObjAuctionList.Where(x=> x.Category == pEnmCategory);
            //}

            if (lLsObjAuctionList.Where(x => x.Date >= lDtmStartDate && x.Date <= lDtmEndDate).Count() > 0)
            {
                return lLsObjAuctionList.Where(x => x.Date >= lDtmStartDate && x.Date <= lDtmEndDate).FirstOrDefault();
            }
            else
            {
                return lLsObjAuctionList.FirstOrDefault();
            }
        }

        public IQueryable<Auction> GetListFilteredByCC()
        {
            string lStrCostingCode = GetCostingCode();
            return mObjAuctionDAO.GetEntitiesList().Where(x => x.CostingCode == lStrCostingCode);
        }

        public List<AuctionDTO> GetDetailedList()
        {
            return mObjAuctionDAO.GetEntitiesList().AsEnumerable().Select(x => new AuctionDTO(x)).ToList();
        }

        public void SaveOrUpdate(Auction pObjAuction)
        {
            if (!Exists(pObjAuction))
            {
                mObjAuctionDAO.SaveOrUpdateEntity(pObjAuction);
            }
            else
            {
                throw new Exception("La subasta ingresada ya existe.");
            }
        }

        public void Remove(long pLonAuctionId)
        {
            mObjAuctionDAO.RemoveEntity(pLonAuctionId);
        }

        public bool IsValid(string pStrFolio)
        {
            return mObjAuctionDAO.GetEntitiesList().Where(x => x.Folio == pStrFolio).Count() > 0;
        }

        //public List<Auction> SearchAuctions(string pStrAuction, FilterEnum pEnmFilter)
        //{
        //    return GetBestAuctionSearch(pStrAuction, GetAuctionByFilter(pEnmFilter).Where(x => x.Category == AuctionCategoryEnum.AUCTION));
        //}

        //public List<Auction> SearchDirectTradeAndSheep(string pStrAuction, FilterEnum pEnmFilter)
        //{
        //    return GetBestAuctionSearch(pStrAuction, GetAuctionByFilter(pEnmFilter).Where(x => x.Category == AuctionCategoryEnum.DIRECT_TRADE || x.Category == AuctionCategoryEnum.SHEEP));
        //}

        //public List<Auction> SearchDirectTrade(string pStrAuction, FilterEnum pEnmFilter)
        //{
        //    return GetBestAuctionSearch(pStrAuction, GetAuctionByFilter(pEnmFilter).Where(x => x.Category == AuctionCategoryEnum.DIRECT_TRADE));
        //}

        //public List<Auction> SearchSheep(string pStrAuction, FilterEnum pEnmFilter)
        //{
        //    return GetBestAuctionSearch(pStrAuction, GetAuctionByFilter(pEnmFilter).Where(x => x.Category == AuctionCategoryEnum.SHEEP));
        //}

        public List<Auction> SearchAuctions(string pStrAuction, FilterEnum pEnmFilter, AuctionSearchModeEnum pEnmSearchMode = 0)
        {
            return GetBestAuctionSearch
            (
                pStrAuction,
                GetAuctionByFilter(pEnmFilter).Where(x => pEnmSearchMode != 0 ?
                (
                    pEnmSearchMode == AuctionSearchModeEnum.AUCTION ?
                        x.Category == AuctionCategoryEnum.AUCTION :
                    pEnmSearchMode == AuctionSearchModeEnum.DIRECT_TRADE ?
                        x.Category == AuctionCategoryEnum.DIRECT_TRADE : true
                ) : true)
            );
        }

        public IList<Auction> SearchAuctions(string pStrAuction)
        {
            return GetBestAuctionSearch(pStrAuction, this.GetListFilteredByCC().Where(x => x.Active == true && x.Removed == false));
        }

        private IQueryable<Auction> GetAuctionByFilter(FilterEnum pEnmFilter)
        {
            string lStrCostingCode = GetCostingCode();

            return this.GetListFilteredByCC().Where(x =>// x.Active &&
                x.CostingCode == lStrCostingCode &&
            (
                pEnmFilter == FilterEnum.OPENED ? x.Opened :
                pEnmFilter == FilterEnum.CLOSED ? !x.Opened : true
            ));
        }

        private string GetCostingCode()
        {
            return ConfigurationUtility.GetValue<string>("CostCenter").ToString();
        }

        private List<Auction> GetBestAuctionSearch(string pStrAuction, IQueryable<Auction> pLstObjAuctions)
        {
            IList<IQueryable<Auction>> lLstObjQueries = new List<IQueryable<Auction>>();

            lLstObjQueries.Add(pLstObjAuctions.Where(x => x.Folio.ToUpper().Contains(pStrAuction.ToUpper())));
            lLstObjQueries.Add(pLstObjAuctions.Where(x => x.Folio.ToUpper().Equals(pStrAuction.ToUpper())));
            lLstObjQueries.Add(pLstObjAuctions.Where(x => x.Folio.ToUpper().StartsWith(pStrAuction.ToUpper())));
            lLstObjQueries.Add(pLstObjAuctions.Where(x => x.Folio.ToUpper().EndsWith(pStrAuction.ToUpper())));

            IQueryable<Auction> lLstObjBetterQuery = pLstObjAuctions;
            int lIntBetterRowCount = pLstObjAuctions.Count();

            for (int i = 0; i < lLstObjQueries.Count; i++)
            {
                int lIntCurrentRowCount = lLstObjQueries[i].Count();
                if (lIntCurrentRowCount > 0 && lIntCurrentRowCount < lIntBetterRowCount)
                {
                    lLstObjBetterQuery = lLstObjQueries[i];
                    lIntBetterRowCount = lIntCurrentRowCount;
                }
            }

            return lLstObjBetterQuery.OrderByDescending(x => x.Date).ToList();
        }

        private bool Exists(Auction pObjAuction)
        {
            return GetListFilteredByCC().Where(x => x.Folio == pObjAuction.Folio && x.Id != pObjAuction.Id).Count() > 0 ? true : false;
        }

        public bool IsActiveAuction()
        {
            return GetListFilteredByCC().Where(x => x.Opened).Count() > 0 ? true : false;
        }

        public Auction GetActiveAuction()
        {
            return GetListFilteredByCC().Where(x => x.Opened).FirstOrDefault();
        }

        public Auction GetReopenedActiveAuction()
        {
            return GetListFilteredByCC().Where(x => x.ReOpened && x.Opened).Select(x => x).FirstOrDefault();
        }
    }
}
