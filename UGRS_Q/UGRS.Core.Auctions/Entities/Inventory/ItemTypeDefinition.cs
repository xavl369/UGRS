using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("ItemTypeDefinitions", Schema = "INVENTORY")]
    public class ItemTypeDefinition : BaseEntity
    {
        public AuctionTypeEnum AuctionType { get; set; }
        public long ItemTypeId { get; set; }
    }
}
