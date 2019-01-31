using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Business;

namespace UGRS.Application.Auctions.Utils
{
    public class PartnerMappingDTO
    {
        public Partner Partner { get; set; }
        //public MappingTypeEnum Type { get; set; }
        public Partner PartnerSAP { get; set; }
    }
}
