using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Business
{
    [Table("PartnerClassifications", Schema = "BUSINESS")]
    public class PartnerClassification : CatalogEntity
    {
        public int Number { get; set; }

        public long CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Partner Customer { get; set; }
    }
}
