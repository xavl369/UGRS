using System;

namespace UGRS.Core.SDK.DI.Auctions.DTO
{
    public class ItemBatchDTO
    {
        public string ItemCode { get; set; }

        public string CardCode { get; set; }

        public string BatchNumber { get; set; }

        public string InitialWarehouse { get; set; }

        public string Folio { get; set; }

        public string CurrentWarehouse { get; set; }

        public bool ChargeFood { get; set; }

        public bool Payment { get; set; }

        public int Quantity { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}
