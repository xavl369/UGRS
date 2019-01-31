using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Reports.Business;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Services.Reports
{
    public static class BusinessReportServiceExtension
    {
        public static IQueryable<Partner> FilterTemporary(this IQueryable<Partner> pLstObjPartners)
        {
            return pLstObjPartners.Where(x => x.Temporary);
        }

        public static IList<PartnerDTO> ToDTO(this IQueryable<Partner> pLstObjPartners)
        {
            return pLstObjPartners.Select(b => new PartnerDTO()
            {
                PartnerId = b.Id,
                Code = b.Code,
                Name = b.Name,
                PartnerStatusId = (int)b.PartnerStatus,
                Temporary = b.Temporary

            }).ToList();
        }
    }

    public class BusinessReportService
    {
        private IBaseDAO<Partner> mObjPartnerDAO;

        public BusinessReportService(IBaseDAO<Partner> pObjPartnerDAO)
        {
            mObjPartnerDAO = pObjPartnerDAO;
        }

        private IQueryable<Partner> GetPartnersList()
        {
            return mObjPartnerDAO.GetEntitiesList();
        }

        public IList<PartnerDTO> GetTemporaryBusinessPartners()
        {
            return GetPartnersList()
                   .FilterTemporary()
                   .ToDTO();
        }
    }
}
