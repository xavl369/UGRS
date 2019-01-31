using System;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class GoodsIssueLineDTO
    {
        public long BatchId { get; set; }
        public int Quantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int ReturnedDeliveriesQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public ItemTypeGenderEnum Gender { get; set; }
    }
}
