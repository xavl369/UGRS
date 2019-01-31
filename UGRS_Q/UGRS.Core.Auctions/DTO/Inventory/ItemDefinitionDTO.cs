
namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class ItemDefinitionDTO
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public long ItemId { get; set; }
        public string Item { get; set; }
        public long ItemTypeId { get; set; }
        public string ItemType { get; set; }
    }
}
