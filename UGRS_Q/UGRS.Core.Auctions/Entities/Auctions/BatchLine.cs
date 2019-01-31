using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Inventory;

namespace UGRS.Core.Auctions.Entities.Auctions
{
    [Table("BatchLines", Schema = "AUCTIONS")]
    public class BatchLine : ExportEntity
    {
        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "varchar"), StringLength(100)]
        public string BatchNumber { get; set; }

        public DateTime BatchDate { get; set; }

        public long ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        public long BatchId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }
    }
}
