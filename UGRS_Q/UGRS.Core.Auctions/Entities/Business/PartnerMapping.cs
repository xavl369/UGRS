using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.Business;

namespace UGRS.Core.Auctions.Entities.Business
{
    [Table("PartnerMappings", Schema = "BUSINESS")]
    public class PartnerMapping : ExportEntity
    {
        public MappingTypeEnum Type { get; set; }

        public long PartnerId { get; set; }

        public long? NewPartnerId { get; set; }

        public bool Autorized { get; set; }

        public long AutorizedByUserId { get; set; }
    }
}
