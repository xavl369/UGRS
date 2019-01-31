using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Services.Business
{
    public class PartnerMappingService
    {
        private IBaseDAO<PartnerMapping> mObjPartnerMappingDAO;

        public PartnerMappingService(IBaseDAO<PartnerMapping> pObjPartnerMappingDAO)
        {
            mObjPartnerMappingDAO = pObjPartnerMappingDAO;
        }

        public IQueryable<PartnerMapping> GetList()
        {
            return mObjPartnerMappingDAO.GetEntitiesList();
        }

        public PartnerMapping GetEntity(long pLonId)
        {
            return mObjPartnerMappingDAO.GetEntity(pLonId);
        }

        public void SaveOrUpdate(PartnerMapping pObjPartnerMapping)
        {
            if (!Exists(pObjPartnerMapping))
            {
                mObjPartnerMappingDAO.SaveOrUpdateEntity(pObjPartnerMapping);
            }
            else
            {
                throw new Exception("El mapeo del socio de negocio temporal ingresado ya se encuentra registrado.");
            }
        }

        public void SaveOrUpdateList(IList<PartnerMapping> pLstObjPartnerMapping)
        {
            mObjPartnerMappingDAO.SaveOrUpdateEntitiesList(pLstObjPartnerMapping);
        }

        public void Remove(long pLonId)
        {
            mObjPartnerMappingDAO.RemoveEntity(pLonId);
        }

        private bool Exists(PartnerMapping pObjPartnerMapping)
        {
            return mObjPartnerMappingDAO
                    .GetEntitiesList()
                    .Where(x => x.PartnerId == pObjPartnerMapping.PartnerId 
                        && x.NewPartnerId == pObjPartnerMapping.NewPartnerId 
                        && x.Id != pObjPartnerMapping.Id)
                    .Count() > 0 ? true : false;
        }
    }
}
