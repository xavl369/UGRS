using System;

namespace UGRS.Core.SDK.DI.Auctions.DTO
{
    public class DeliveryFoodDTO
    {
        public string DocType { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public bool Opened { get; set; }
        public string CardCode { get; set; }
        public int LineNum { get; set; }
        public string WhsCode { get; set; }
        public string TaxCode { get; set; }
        public string BatchNumber { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
