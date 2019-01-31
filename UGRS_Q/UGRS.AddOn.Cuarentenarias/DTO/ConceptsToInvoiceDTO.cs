using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.DTO
{
    public class ConceptsToInvoiceDTO
    {
        public string ItemCode { get; set; }
        public int DocEntry { get; set; }
        public string ObjType { get; set; }
        public int LineNum { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double PricePerArt { get; set; }
        public string TaxCode { get; set; }
        public string WhsCode { get; set; }
        public string UomCode { get; set; }
        public string IdInspection { get; set; }
        public string InvoiceType { get; set; }
    }
}
