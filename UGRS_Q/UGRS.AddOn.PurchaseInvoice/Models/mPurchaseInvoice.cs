using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.PurchaseInvoice.Models
{
   public class mPurchaseInvoice
    {
        public String RFCProveedor { get; set; }
        public String NombreSocioNegocio { get; set; }
        public String Nombre { get; set; }
        public String RFCReceptor { get; set; }
        public List<Concepts> ConceptLines { get; set; }
        public String SubTotal { get; set; }
        public String Impuestos { get; set; }
        public String Total { get; set; }
        public String MonedaXml { get; set; }
        public String MonedaDocumento { get; set; }
        public String CardCode { get; set; }
        public string Retenciones { get; set; }
        public string ImpuestosTraslados { get; set; }
        public string Version { get; set; }
    }
}
