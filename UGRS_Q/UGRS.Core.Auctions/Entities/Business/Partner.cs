using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Business;

namespace UGRS.Core.Auctions.Entities.Business
{
    [Table("Partners", Schema = "BUSINESS")]
    public class Partner : TemporaryEntity
    {
        #region Entity properties  
 
        [Column(TypeName = "varchar"), StringLength (15), Required]
        public string Code { get; set; }

        [Column(TypeName = "varchar"), StringLength(100), Required]
        public string Name { get; set; }

        [Column(TypeName = "varchar"), StringLength(100)]
        public string ForeignName { get; set; }

        [Column(TypeName = "varchar"), StringLength(32)]
        public string TaxCode { get; set; }

        public PartnerStatusEnum PartnerStatus { get; set; }
     
        #endregion

        #region External properties

        public virtual IList<Batch> SelledBatches { get; set; }

        public virtual IList<Batch> BuyedBatches { get; set; }

        public virtual IList<GoodsReceipt> GoodsReceipts { get; set; }

        public virtual IList<PartnerClassification> Classifications { get; set; }

        #endregion
    }
}
