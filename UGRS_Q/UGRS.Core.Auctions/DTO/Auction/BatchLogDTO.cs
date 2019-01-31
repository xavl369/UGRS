using System;

namespace UGRS.Core.Auctions.DTO.Auction
{
    public class BatchLogDTO
    {
        public long Id { get; set; }
        public long AuctionId { get; set; }
        public string Auction { get; set; }
        public int Number { get; set; }
        public long BatchId { get; set; }
        public string BatchObject { get; set; }
        public int BatchNumber { get; set; }
        public long ModificationUserId { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ModificationUser { get; set; }
        public long CreationUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
    }
}
