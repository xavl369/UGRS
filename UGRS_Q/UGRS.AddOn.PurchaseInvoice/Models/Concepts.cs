using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.PurchaseInvoice.Models
{
    public class Concepts
    {
        public String Descripcion { get; set; }
        public String NumeroCuenta { get; set; }
        public String Articulo { get; set; }
        public String Cantidad { get; set; }
        public String ValorUnitario { get; set; }
        public String Importe { get; set; }
        public String Unidad { get; set; }
        public string ClaveItmProd { get; set; }
    }
}
