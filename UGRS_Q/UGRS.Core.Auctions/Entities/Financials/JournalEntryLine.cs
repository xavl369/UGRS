using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("JournalEntryLines", Schema = "FINANCIALS")]
    public class JournalEntryLine : ExportEntity
    {
        #region Entity properties

        public int LineNum { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string CostingCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string AccountCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string ContraAccount { get; set; }

        public double Debit { get; set; }

        public double Credit { get; set; }

        [Column(TypeName = "text")]
        public string Remarks { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Reference { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Auxiliary { get; set; }

        public int AuxiliaryType { get; set; }

        public string Commentaries { get; set; }

        #endregion

        #region External properties

        public long JournalEntryId { get; set; }

        [ForeignKey("JournalEntryId")]
        public virtual JournalEntry JournalEntry { get; set; }

        #endregion
    }
}
