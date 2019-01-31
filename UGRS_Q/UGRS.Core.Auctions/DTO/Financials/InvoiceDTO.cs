using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace UGRS.Core.Auctions.DTO.Financials
{
    [Serializable]
    public class InvoiceDTO
    {
        public string NumAtCard { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public double Import { get; set; }
        public List<InvoiceLineDTO> Lines { get; set; }
        //public string 
    }
}
