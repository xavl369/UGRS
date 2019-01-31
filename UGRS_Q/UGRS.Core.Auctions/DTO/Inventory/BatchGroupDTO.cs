
using UGRS.Core.Auctions.Enums.Inventory;
namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class BatchGroupDTO
    {
        public long BatchId { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public long ItemTypeId { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemTypeName { get; set; }
        public ItemTypeGenderEnum Gender { get; set; }
    }
}
