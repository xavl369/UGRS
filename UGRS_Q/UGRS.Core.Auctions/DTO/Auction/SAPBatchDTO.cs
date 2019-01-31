using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.DTO.Auction
{
    public class SAPBatchDTO
    {
        public string Seller { get; set; }
        public string BatchNumber { get; set; }
        public string SapArticle { get; set; }
        public int Quantity { get; set; }
        public float Weight { get; set; }
        public ItemTypeGenderEnum Gender {get; set;}
        public List<Batch> BatchesList { get; set; }
    }
}
