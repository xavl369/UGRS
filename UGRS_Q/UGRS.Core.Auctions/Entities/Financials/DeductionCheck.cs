using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("DeductionChecks", Schema = "FINANCIALS")]
    public class DeductionCheck : BaseEntity
    {
        public long AuctionId { get; set; }
        public long SellerId { get; set; }
        public bool Deduct { get; set; }
        public string Comments { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }
    }
}
