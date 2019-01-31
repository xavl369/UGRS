using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Inventory
{
   public class BatchExchangeDTO
    {
       public string Clasification { get; set; }
       public int Batch { get; set; }
       public int Quantity { get; set; }
       public string HasGoodIssue { get; set; }
       public long BatchId { get; set; }

    }
}
