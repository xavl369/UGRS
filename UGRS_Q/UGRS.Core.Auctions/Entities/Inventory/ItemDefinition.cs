using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("ItemDefinitions", Schema = "INVENTORY")]
    public class ItemDefinition : BaseEntity
    {
        public int Order { get; set; }
        
        public long ItemId { get; set; }

        public long ItemTypeId { get; set; }
    }
}
