using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.Entities.Auctions
{
    [Table("Auctions", Schema = "AUCTIONS")]
    public class Auction : DocumentEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength(20), Required]
        public string Folio { get; set; }

        public AuctionTypeEnum Type { get; set; }

        public AuctionCategoryEnum Category { get; set; }

        public LocationEnum Location { get; set; }

        public string CostingCode { get; set; }

        [Required]
        public float Commission { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool ReOpened { get; set; }

        public DateTime? ReOpenedTime { get; set; }

        public bool ReProcessed { get; set; }

        #endregion

        #region External properties

        public virtual IList<Batch> Batches { get; set; }

        public virtual IList<Invoice> Invoices { get; set; }

        public virtual IList<JournalEntry> JournalEntries { get; set; }

        #endregion
    }
}
