using System;

namespace UGRS.Core.Auctions.DTO.Financials
{
    [Serializable]
    public class DeliveryFoodDTO
    {
        public string DocType { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public string CardCode { get; set; }
        public int LineNum { get; set; }
        public string WhsCode { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
