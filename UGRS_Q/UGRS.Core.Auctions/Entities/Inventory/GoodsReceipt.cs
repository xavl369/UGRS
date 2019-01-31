using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("GoodsReceipts", Schema = "INVENTORY")]
    public class GoodsReceipt : DocumentEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength (50)]
        public string Folio { get; set; }

        public int Quantity {get; set;}

        [Column(TypeName = "varchar"), StringLength(100)]
        public string BatchNumber { get; set; }

        public DateTime BatchDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Column(TypeName = "varchar"), StringLength(500)]
        public string Remarks { get; set; }

        #endregion

        #region External properties

        public long CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Partner Customer { get; set; }

        public long ItemId {get; set;}

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        #endregion
    }
}
