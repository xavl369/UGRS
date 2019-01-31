using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Inventory
{
   public class BatchSAPDTO
    {
       public long SellerId { get; set; }
       public long ArticleId { get; set; }
       public int Quantity { get; set; }
    }
}
