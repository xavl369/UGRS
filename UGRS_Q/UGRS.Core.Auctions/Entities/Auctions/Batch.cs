using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using System.Collections.Generic;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.Entities.Auctions
{
    [Table("Batches", Schema = "AUCTIONS")]
    public class Batch : ExportEntity
    {
        #region Entity properties

        [Required]
        public int Number { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public float Weight { get; set; }

        [Required]
        public float AverageWeight { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Amount { get; set; }

        public bool Reprogrammed { get; set; }

        public bool Unsold { get; set; }

        public UnsoldMotiveEnum UnsoldMotive { get; set; }

        public string Gender { get; set; }

        public bool SellType { get; set; }

        public virtual IList<BatchLine> Lines { get; set; }

        #endregion

        #region External properties

        public long? ItemTypeId { get; set; }

        [ForeignKey("ItemTypeId")]
        public virtual ItemType ItemType { get; set; }

        public long? SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }

        public long? BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public virtual Partner Buyer { get; set; }

        public long? BuyerClassificationId { get; set; }

        [ForeignKey("BuyerClassificationId")]
        public virtual PartnerClassification BuyerClassification { get; set; }

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        public virtual IList<GoodsIssue> GoodsIssues { get; set; }

        public virtual IList<GoodsReturn> GoodsReturns { get; set; }

        #endregion
    }
}
