using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("GuideCharges", Schema = "FINANCIALS")]
    public class GuideCharge : DocumentEntity
    {
        public double Amount { get; set; }

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        public long SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }
    }
}
