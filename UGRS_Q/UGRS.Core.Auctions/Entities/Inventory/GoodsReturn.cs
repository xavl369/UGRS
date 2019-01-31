using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("GoodsReturns", Schema = "INVENTORY")]
    public class GoodsReturn : DocumentEntity
    {
        [Column(TypeName = "varchar"), StringLength(10)]
        public string Folio { get; set; }

        public int Number { get; set; }

        public int Quantity { get; set; }

        public float Weight { get; set; }

        [Column(TypeName = "varchar"), StringLength(500)]
        public string Remarks { get; set; }

        public bool Delivered { get; set; }

        public long BatchId { get; set; }

        public UnsoldMotiveEnum ReturnMotive { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
    }
}
