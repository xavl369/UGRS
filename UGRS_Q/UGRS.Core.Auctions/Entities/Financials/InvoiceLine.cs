using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("InvoiceLines", Schema = "FINANCIALS")]
    public class InvoiceLine : ExportEntity
    {
        public int LineNum { get; set; }

        public int DocNum { get; set; }

        public int DocType { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string ItemCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string TaxCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string WarehouseCode { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string CostingCode { get; set; }

        public long InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
