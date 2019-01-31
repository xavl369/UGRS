using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.DTO
{
   public class InspectDetailDTO
    {
       public string Type { get; set; }
       public int Quantity { get; set; }
       public string Commentary { get; set; }
       public string RowCode { get; set; }

       public string TypeMov { get; set; }
    }
}
