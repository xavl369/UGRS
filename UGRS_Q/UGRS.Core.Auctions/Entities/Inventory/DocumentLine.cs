using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("DocumentLines", Schema = "INVENTORY")]
    public class DocumentLine : BaseEntity
    {
        public int Quantity { get; set; }

        public long ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        public long BatchId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }

        public long DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}
