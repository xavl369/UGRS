using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.FoodProduction.DTO
{
    public class TransferHeader_DTO
    {        
        //Header
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int Series { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime TaxDate { get; set; }
        public string Filler { get; set; }
        public string ToWhsCode { get; set; }
        public string JrnlMemo { get; set; }
        public string Comments { get; set; }

        public string U_PL_WhsReq { get; set; }
        public string U_GLO_Alert { get; set; }
        public string U_CO_TypeInvoice { get; set; }
        public int U_MQ_OrigenFol { get; set; }    

    }   
}
