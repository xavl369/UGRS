using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("Stocks", Schema = "INVENTORY")]
    public class Stock : BaseEntity
    {
        [Column(TypeName = "varchar"), StringLength(100), Required]
        public string BatchNumber { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string CurrentWarehouse { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string InitialWarehouse { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string EntryFolio { get; set; }

        public bool ChargeFood { get; set; }

        public bool Payment { get; set; }

        public DateTime ExpirationDate { get; set; }

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
