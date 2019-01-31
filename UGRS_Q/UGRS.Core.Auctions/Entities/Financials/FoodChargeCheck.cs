using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("FoodChargeChecks", Schema = "FINANCIALS")]
    public class FoodChargeCheck : BaseEntity
    {
        [Column(TypeName = "varchar"), StringLength(100)]
        public string BatchNumber { get; set; }

        public DateTime BatchDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool FoodCharge { get; set; }

        public bool FoodDeliveries { get; set; }

        public bool AlfalfaDeliveries { get; set; }

        public bool ApplyFoodCharge { get; set; }

        public long SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual Partner Seller { get; set; }
    }
}
