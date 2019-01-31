
namespace UGRS.Core.Auctions.DTO.Reports.Inventory
{
    public class GoodsReceiptDTO
    {
        #region Properties

        public long GoodsReceiptId { get; set; }
        public string Folio { get; set; }
        public int Quantity { get; set; }
        public bool Exported { get; set; }
        public string Remarks { get; set; }
        public long CustomerId { get; set; }
        public string Customer { get; set; }
        public long ItemId { get; set; }
        public string Item { get; set; }

        #endregion

        #region Constructor

        public GoodsReceiptDTO()
        {
            //Default constructor
        }

        public GoodsReceiptDTO(dynamic pUnkObject)
        {
            SetEntityFields(pUnkObject);
        }

        #endregion

        #region Methods

        private void SetEntityFields(dynamic pUnkObject)
        {
            GoodsReceiptId = pUnkObject.Id;
            Folio = pUnkObject.Folio;
            Quantity = pUnkObject.Quantity;
            Exported = pUnkObject.Exported;
            Remarks = pUnkObject.Remarks;
            CustomerId = pUnkObject.CustomerId;
            Customer = pUnkObject.CustomerId > 0 ? pUnkObject.Customer.Name : string.Empty;
            ItemId = pUnkObject.ItemId;
            Item = pUnkObject.ItemId > 0 ? pUnkObject.Item.Name : string.Empty;
        }

        #endregion
    }
}
