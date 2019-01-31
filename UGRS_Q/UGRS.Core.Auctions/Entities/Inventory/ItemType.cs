using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("ItemTypes", Schema = "INVENTORY")]
    public class ItemType : BaseEntity
    {
        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string Code { get; set; }

        [Column(TypeName = "varchar"), StringLength(100), Required]
        public string Name { get; set; }

        //public bool PerPrice { get; set; }

        public SellTypeEnum SellType { get;set;}

        public int Level { get; set; }

        public ItemTypeGenderEnum Gender { get; set; }

        #region External properties

        public long? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual ItemType Parent { get; set; }

        #endregion
    }
}
