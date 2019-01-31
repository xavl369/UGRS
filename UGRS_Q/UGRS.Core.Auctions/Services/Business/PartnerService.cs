using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.Enums.Business;

namespace UGRS.Core.Auctions.Services.Business
{
    public class PartnerService
    {
        private IBaseDAO<Partner> mObjPartnerDAO;
        private IBaseDAO<PartnerMapping> mObjPartnerMappingDAO;

        public PartnerService(IBaseDAO<Partner> pObjPartnerDAO, IBaseDAO<PartnerMapping> pObjPartnerMappingDAO)
        {
            mObjPartnerDAO = pObjPartnerDAO;
            mObjPartnerMappingDAO = pObjPartnerMappingDAO;
        }

        public IQueryable<Partner> GetList()
        {
            return mObjPartnerDAO.GetEntitiesList();
        }

        public IQueryable<Partner> GetTemporaryList()
        {
            return mObjPartnerDAO.GetEntitiesList().Where(x => x.Temporary);
        }

        public IQueryable<Partner> GetTemporaryAndUnmappedList()
        {
            IList<long> lLstLonMapped = mObjPartnerMappingDAO.GetEntitiesList().Where(x=>x.NewPartnerId > 0).Select(x => x.PartnerId).ToList();

            var dd = mObjPartnerMappingDAO.GetEntitiesList().ToList();

            return mObjPartnerDAO.GetEntitiesList().Where(x => x.Temporary && !lLstLonMapped.Contains(x.Id));
        }

        public Partner GetEntity(long pLonId)
        {
            return mObjPartnerDAO.GetEntity(pLonId);
        }

        public void SaveOrUpdate(Partner pObjPartner)
        {
            if (!Exists(pObjPartner))
            {
                mObjPartnerDAO.SaveOrUpdateEntity(pObjPartner);
            }
            else
            {
                throw new Exception("El cliente ingresado ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjPartnerDAO.RemoveEntity(pLonId);
        }

        public List<Partner> SearchPartner(string pStrPartner, FilterEnum pEnmFilter)
        {
            IList<IQueryable<Partner>> lLstObjQueries = new List<IQueryable<Partner>>();
            IQueryable<Partner> lLstObjPartners = this.GetList().Where(x =>
            (
                pEnmFilter == FilterEnum.ACTIVE ? x.Active && x.PartnerStatus == PartnerStatusEnum.ACTIVE :
                pEnmFilter == FilterEnum.INACTIVE ? !x.Active && x.PartnerStatus == PartnerStatusEnum.INACTIVE : true
            ));

            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Code.ToUpper().Contains(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Name.ToUpper().Contains(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Code.ToUpper().Equals(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Name.ToUpper().Equals(pStrPartner.ToUpper())));

            lLstObjQueries.Add(lLstObjPartners.Where(x => (!string.IsNullOrEmpty(pStrPartner) ? x.ForeignName.ToUpper().Equals(pStrPartner.ToUpper()) : x.ForeignName == x.ForeignName)));

            IQueryable<Partner> lLstObjBetterQuery = lLstObjPartners;
            int lIntBetterRowCount = lLstObjPartners.Count();

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



        public List<Partner> SearchPartnerWithStock(string pStrPartner, FilterEnum pEnmFilter, List<long> plstCustomersWithStock)
        {
            IList<IQueryable<Partner>> lLstObjQueries = new List<IQueryable<Partner>>();
            IQueryable<Partner> lLstObjPartners = this.GetList().Where(x =>
            (
                pEnmFilter == FilterEnum.ACTIVE ? x.Active && x.PartnerStatus == PartnerStatusEnum.ACTIVE :
                pEnmFilter == FilterEnum.INACTIVE ? !x.Active && x.PartnerStatus == PartnerStatusEnum.INACTIVE : true
            )
            && plstCustomersWithStock.Contains(x.Id)
            );

            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Code.ToUpper().Contains(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Name.ToUpper().Contains(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Code.ToUpper().Equals(pStrPartner.ToUpper())));
            lLstObjQueries.Add(lLstObjPartners.Where(x => x.Name.ToUpper().Equals(pStrPartner.ToUpper())));

            lLstObjQueries.Add(lLstObjPartners.Where(x => (!string.IsNullOrEmpty(pStrPartner) ? x.ForeignName.ToUpper().Equals(pStrPartner.ToUpper()) : x.ForeignName == x.ForeignName)));

            IQueryable<Partner> lLstObjBetterQuery = lLstObjPartners;
            int lIntBetterRowCount = lLstObjPartners.Count();

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

        private bool Exists(Partner pObjPartner)
        {
            return mObjPartnerDAO.GetEntitiesList().Where(x => x.Name == pObjPartner.Name && x.Code == pObjPartner.Code && x.Id != pObjPartner.Id).Count() > 0 ? true : false;
        }
    }
}
