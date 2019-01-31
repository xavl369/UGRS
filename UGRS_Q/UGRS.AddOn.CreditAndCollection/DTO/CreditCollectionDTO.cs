using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.CreditAndCollection.DTO
{
    public class CreditCollectionDTO
    {
        public string Seller { get; set; }
        public double TotCredit { get;set;}
        public double TotDebit { get; set; }
        public double TotInvoice { get; set; }
        public string AuctionNumb { get; set; }
        public int AuxType { get; set; }
        public string Autorized { get; set; }
    }
}
