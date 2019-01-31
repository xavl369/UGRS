using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("JournalEntries", Schema = "FINANCIALS")]
    public class JournalEntry : DocumentEntity
    {
        #region Entity properties

        public int Number { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Series { get; set; }

        [Column(TypeName = "text")]
        public string Remarks { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Reference { get; set; }

        #endregion

        #region External properties

        public virtual IList<JournalEntryLine> Lines { get; set; }

        public long AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        #endregion
    }
}
