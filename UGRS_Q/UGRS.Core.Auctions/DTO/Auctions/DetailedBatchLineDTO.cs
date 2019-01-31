using System;

namespace UGRS.Core.Auctions.DTO.Auctions
{
    public class DetailedBatchLineDTO
    {
        public long BatchId { get; set; }

        public long ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public string BatchNumber { get; set; }
        public DateTime BatchDate { get; set; }
        
        public int TotalQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int ReturnedQuantity { get; set; }

        public int TotalWeight { get; set; }
        public int DeliveredWeight { get; set; }
        public int ReturnedWeight { get; set; }

        public int AvailableQuantityToDelivery { get; set; }
        public int AvailableQuantityToReturn { get; set; }
        public int AvailableQuantityToReturnDelivery { get; set; }

        public int AvailableWeightToDelivery { get; set; }
        public int AvailableWeightToReturn { get; set; }
        public int AvailableWeightToReturnDelivery { get; set; }
    }
}
