using System;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class StockBatchDTO
    {
        public long CustomerId { get; set; }

        public long ItemId { get; set; }

        public string BatchNumber { get; set; }

        public int Quantity { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
