using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Services.Business;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class BusinessServicesFactory
    {
        public PartnerService GetPartnerService()
        {
            return new PartnerService(new BaseDAO<Partner>(), new BaseDAO<PartnerMapping>());
        }

        public PartnerClassificationService GetPartnerClassificationService()
        {
            return new PartnerClassificationService(new BaseDAO<PartnerClassification>());
        }

        public PartnerMappingService GetPartnerMappingService()
        {
            return new PartnerMappingService(new BaseDAO<PartnerMapping>());
        }
    }
}
