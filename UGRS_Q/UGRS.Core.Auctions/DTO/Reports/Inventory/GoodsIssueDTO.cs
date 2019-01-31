using System;

namespace UGRS.Core.Auctions.DTO.Reports.Inventory
{
    public class GoodsIssueDTO
    {
        public string GoodsIssueFolio { get; set; }
        public string GoodsIssueDate { get; set; }
        public string AuctionFolio { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemTypeName { get; set; }
        public int Quantity { get; set; }
    }
}
