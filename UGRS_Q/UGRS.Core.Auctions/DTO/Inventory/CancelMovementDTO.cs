using System;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class CancelMovementDTO
    {
        public long MovementId { get; set; }
        public string TypeMovement { get; set; }
        public int Quantity { get; set; }
        public bool Cancel { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public bool Delivered { get; set; }
        public DateTime Date { get; set; }
        public bool Canceled { get;set; }
        public string Status { get; set; }
        public GoodsIssue GoodIssue { get; set; }
        public GoodsReturn GoodReturn { get; set; }
    }
}
