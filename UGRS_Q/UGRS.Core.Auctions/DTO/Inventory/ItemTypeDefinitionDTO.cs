using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class ItemTypeDefinitionDTO
    {
        public long Id { get; set; }
        public long ItemTypeId { get; set; }
        public string ItemType { get; set; }
        public AuctionTypeEnum AuctionType { get; set; }
        public string AuctionTypeName { get; set; }
    }
}
