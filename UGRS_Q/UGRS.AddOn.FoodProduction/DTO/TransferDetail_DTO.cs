using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.FoodProduction.DTO
{
    public class TransferDetail_DTO
    {
        //Detail
        public int DocEntry { get; set; }
        public int LineNum { get; set; }

        public string ItemDescription { get; set; } 
        public string ItemCode { get; set; }
        public string FromWhsCode { get; set; } 
        public string WhsCode { get; set; } 
        public int Quantity { get; set; }
        public int U_GLO_BagsBales { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
    }
}
