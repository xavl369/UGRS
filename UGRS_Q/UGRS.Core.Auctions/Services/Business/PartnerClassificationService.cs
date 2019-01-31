using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Business;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.Enums.Business;

namespace UGRS.Core.Auctions.Services.Business
{
    public class PartnerClassificationService
    {
        private IBaseDAO<PartnerClassification> mObjPartnerClassificationDAO;

        public PartnerClassificationService(IBaseDAO<PartnerClassification> pObjPartnerClassificationDAO)
        {
            mObjPartnerClassificationDAO = pObjPartnerClassificationDAO;
        }

        public IQueryable<PartnerClassification> GetList()
        {
            return mObjPartnerClassificationDAO.GetEntitiesList();
        }

        public PartnerClassification GetEntity(long pLonId)
        {
            return mObjPartnerClassificationDAO.GetEntity(pLonId);
        }

        public void SaveOrUpdate(PartnerClassification pObjPartnerClassification)
        {
            if (!Exists(pObjPartnerClassification))
            {
                mObjPartnerClassificationDAO.SaveOrUpdateEntity(pObjPartnerClassification);
            }
            else
            {
                throw new Exception("La clasificación ingresada ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjPartnerClassificationDAO.RemoveEntity(pLonId);
        }

        public PartnerClassification SearchPartnerClassification(int pIntNumber)
        {
            return mObjPartnerClassificationDAO.GetEntitiesList().Where(x => x.Number == pIntNumber).FirstOrDefault();
        }

        public int GetNextNumber()
        {
            return (mObjPartnerClassificationDAO.GetEntitiesList().Select(x => (int?)x.Number).Max() ?? 0) + 1;
        }

        public PartnerClassification GetClassification(PartnerClassificationDTO pObjClassification)
        {
            return pObjClassification != null ? mObjPartnerClassificationDAO.GetEntity(pObjClassification.Id) : null;
        }

        public List<PartnerClassificationDTO> ParseToDto(IList<PartnerClassification> pLstObjList)
        {
            return pLstObjList.Select(x => new PartnerClassificationDTO()
            {
                Id = x.Id,
                Number = x.Number,
                Name = x.Name,
                CustomerId = x.CustomerId,
                CustomerCode = x.Customer.Code,
                CustomerName = x.Customer.Name,
            })
            .ToList();
        }

        public List<PartnerClassification> SearchPartner(string pStrText, FilterEnum pEnmFilter)
        {
            IList<IQueryable<PartnerClassification>> lLstObjQueries = new List<IQueryable<PartnerClassification>>();
            IQueryable<PartnerClassification> lLstObjClassifications = this.GetList().Where(x =>
            (
                pEnmFilter == FilterEnum.ACTIVE ? x.Active && x.Customer.PartnerStatus == PartnerStatusEnum.ACTIVE :
                pEnmFilter == FilterEnum.INACTIVE ? !x.Active && x.Customer.PartnerStatus == PartnerStatusEnum.INACTIVE : true
            ));

            lLstObjQueries.Add(lLstObjClassifications.Where(x => x.Number.ToString().Equals(pStrText)));
            lLstObjQueries.Add(lLstObjClassifications.Where(x => x.Customer.Code.ToUpper().Contains(pStrText.ToUpper())));
            lLstObjQueries.Add(lLstObjClassifications.Where(x => x.Customer.Name.ToUpper().Contains(pStrText.ToUpper())));
            lLstObjQueries.Add(lLstObjClassifications.Where(x => x.Customer.Code.ToUpper().Equals(pStrText.ToUpper())));
            lLstObjQueries.Add(lLstObjClassifications.Where(x => x.Customer.Name.ToUpper().Equals(pStrText.ToUpper())));

            IQueryable<PartnerClassification> lLstObjBetterQuery = lLstObjClassifications;
            int lIntBetterRowCount = lLstObjClassifications.Count();

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

        private bool Exists(PartnerClassification pObjPartnerClassification)
        {
            return mObjPartnerClassificationDAO
                    .GetEntitiesList()
                    .Where(x => x.Name == pObjPartnerClassification.Name
                        && x.Number == pObjPartnerClassification.Number
                        && x.Id != pObjPartnerClassification.Id)
                    .Count() > 0;
        }

        public long GetPartnerByClassificationId(long pIntClassificationId)
        {
            return mObjPartnerClassificationDAO.GetEntitiesList().Where(x => x.Id == pIntClassificationId).Select(x => x.CustomerId).FirstOrDefault();
        }
    }
}
