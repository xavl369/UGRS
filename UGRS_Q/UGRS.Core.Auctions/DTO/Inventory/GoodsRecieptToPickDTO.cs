using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Inventory
{
   public class GoodsRecieptToPickDTO
    {
       public string Folio { get; set; }
       public int Quantity { get; set; }
       public string Code { get; set; }
       public string Customer { get; set; }
       public long ItemId { get; set; }
       public string Item { get; set; }
       public string Related { get; set; }
       public long Id { get; set; }

    }
}
