using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Auctions
{
    public class BatchChargeDTO
    {
        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerNmae { get; set; }

        public long AuctionId { get; set; }
        public string AuctionFolio { get; set; }
    }
}
