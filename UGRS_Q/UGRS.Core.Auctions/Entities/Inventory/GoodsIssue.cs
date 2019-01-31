using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("GoodsIssues", Schema = "INVENTORY")]
    public class GoodsIssue : DocumentEntity
    {
        [Column(TypeName = "varchar"), StringLength(10)]
        public string Folio { get; set; }

        public int Number { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "varchar"), StringLength(500)]
        public string Remarks { get; set; }

        public long BatchId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
    }
}
