using System;

namespace UGRS.Core.SDK.DI.Auctions.DTO
{
    public class ItemDTO
    {
        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public bool Valid { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
