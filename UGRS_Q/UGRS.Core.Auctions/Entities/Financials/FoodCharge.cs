using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("FoodCharges", Schema = "FINANCIALS")]
    public class FoodCharge : DocumentEntity
    {
        #region Entity properties

        [Required]
        public int Folio { get; set; }

        public int Batches { get; set; }

        public int TotalQuantity { get; set; }

        public float TotalWeight { get; set; }

        public float TotalFoodWeight { get; set; }

        #endregion

        #region External properties

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        public long SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }

        public virtual IList<FoodChargeLine> Lines { get; set; }

        #endregion
    }
}
