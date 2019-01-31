using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Financials
{
    [Table("FoodDeliveries", Schema = "FINANCIALS")]
    public class FoodDelivery : DocumentEntity
    {
        [Column(TypeName = "varchar"), StringLength(1)]
        public string DocType { get; set; }

        public int DocNum { get; set; }

        public int DocEntry { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string CardCode { get; set; }

        public int LineNum { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string WhsCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string ItemCode { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string BatchNumber { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string TaxCode { get; set; }

        public double Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
