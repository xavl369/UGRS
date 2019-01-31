using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("FoodChargeLines", Schema = "FINANCIALS")]
    public class FoodChargeLine : ExportEntity
    {
        #region Entity properties

        public int AuctionBatch { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string ItemBatch { get; set; }

        public float AverageWeight { get; set; }

        public int Quantity { get; set; }

        public DateTime LineEntryDate { get; set; }

        public DateTime LineSaleDate { get; set; }

        public float Factor { get; set; }

        public float Days { get; set; }

        public float FoodWeight { get; set; }

        #endregion

        #region External properties

        public long FoodChargeId { get; set; }

        [ForeignKey("FoodChargeId")]
        public virtual FoodCharge FoodCharge { get; set; }

        #endregion
    }
}
