using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Auctions
{
    [Table("Trades", Schema = "AUCTIONS")]
    public class Trade : BaseEntity
    {
        [Required]
        public int Number { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public long? SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }

        public long? BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public virtual Partner Buyer { get; set; }

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        public double Weight { get; set; }
    }
}
