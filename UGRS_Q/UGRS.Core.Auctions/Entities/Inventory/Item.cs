using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Auctions.Entities.Base;
using System.Collections.Generic;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("Items", Schema = "INVENTORY")]
    public class Item : BaseEntity
    {
        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string Code { get; set; }

        [Column(TypeName = "varchar"), StringLength(100), Required]
        public string Name { get; set; }

        public ItemTypeGenderEnum Gender { get; set; }

        public ItemStatusEnum ItemStatus { get; set; }

        public int Level { get; set; }

        #region External properties

        public virtual IList<BatchLine> BatchLines { get; set; }

        public virtual IList<GoodsReceipt> GoodsReceipts { get; set; }

        #endregion
    }
}
