using System;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class ItemBatchDTO
    {
        public int Quantity { get; set; }

        public string BatchNumber { get; set; }

        public DateTime BatchDate { get; set; }

        public long ItemId { get; set; }

        public long CustomerId { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public bool ChargeFood { get; set; }
    }
}
