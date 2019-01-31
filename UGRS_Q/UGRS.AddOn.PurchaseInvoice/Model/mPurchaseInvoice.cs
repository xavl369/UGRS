using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.PurchaseInvoice.Model
{
   public class mPurchaseInvoice
    {
        public String Proveedor { get; set; }
        public String Nombre { get; set; }
        public String RFC { get; set; }
        public List<Concepts> ConceptLines { get; set; }
        public String SubTotal { get; set; }
        public String Impuestos { get; set; }
        public String Total { get; set; }
    }
}
