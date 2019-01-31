using System;

namespace UGRS.Core.SDK.DI.Auctions.DTO
{
    public class CustomerDTO
    {
        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string CardFName { get; set; }

        public string TaxCode { get; set; }

        public bool Valid { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime UpdateHour { get; set; }
    }
}
