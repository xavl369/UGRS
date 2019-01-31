using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Auction
{
    public class FoodChargeDTO
    {
        public bool Selected { get; set; }
        public long SellerId { get; set; }
        public string SellerCardCode { get; set; }
        public string SellerName { get; set; }
        public int NoBatches { get; set; }
        public int Quantity { get; set; }
        public float Weight { get; set; }
        public float Factor { get; set; }
        public float WeightFood { get; set; }

    }
}
