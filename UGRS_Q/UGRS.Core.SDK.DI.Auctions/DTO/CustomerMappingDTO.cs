using System;
using UGRS.Core.SDK.DI.Auctions.Enum;

namespace UGRS.Core.SDK.DI.Auctions.DTO
{
    [Serializable]
    public class CustomerMappingDTO
    {
        public long LocalPartnerId { get; set; }

        public MappingTypeEnum Type { get; set; }

        public string SapPartnerCardCode { get; set; }

        public bool Autorize { get; set; }
    }
}
