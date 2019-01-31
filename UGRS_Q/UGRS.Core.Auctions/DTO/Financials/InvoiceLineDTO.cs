using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Financials
{
     [Serializable]
    public class InvoiceLineDTO
    {
         public string ItemCode { get; set; }
         public string ItemName { get; set; }
         public double Quantity { get; set; }
         public double Price { get; set; }
         public double Import { get; set; }
    }
}
