using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.DTO.Reports.Auctions
{
    public class ReportBatchDTO
    {
        #region Attributes

        private int mIntUnsoldMotiveId;
        private string mStrUnsoldMotive;

        #endregion

        #region Properties

        //Header
        public long AuctionId { get; set; }
        public string AuctionFolio { get; set; }

        //Detail
        public long BatchId { get; set; }
        public int BatchNumber { get; set; }
        public string SellerCode { get; set; }
        public string Seller { get; set; }
        public string BuyerCode { get; set; }
        public string Buyer { get; set; }
        public string BuyerClass { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemType { get; set; }
        public bool PerPrice { get; set; }
        public int Quantity { get; set; }
        public int Delivered { get; set; }
        public int Returned { get; set; }
        public int Available { get; set; }
        public float Weight { get; set; }
        public float AverageWeight { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public bool Unsold { get; set; }
        public string Gender { get; set; }

        public int UnsoldMotiveId
        {
            get
            {
                return mIntUnsoldMotiveId;
            }
            set
            {
                mIntUnsoldMotiveId = value;
                mStrUnsoldMotive = ((UnsoldMotiveEnum)mIntUnsoldMotiveId).GetDescription();
            }
        }

        public string UnsoldMotive
        {
            get
            {
                return mStrUnsoldMotive;
            }
        }

        #endregion

        #region Constructor

        public ReportBatchDTO()
        {
            //Default constructor
        }

        public ReportBatchDTO(dynamic pUnkObject)
        {
            SetAuctionsFields(pUnkObject);
            SetBatchFields(pUnkObject);
        }

        #endregion

        #region Methods

        private void SetAuctionsFields(dynamic pUnkObject)
        {
            AuctionId = pUnkObject.AuctionId;
            AuctionFolio = pUnkObject.AuctionId > 0 ? pUnkObject.Auction.Folio : string.Empty;
        }

        public void SetBatchFields(dynamic pUnkObject)
        {
            BatchId = pUnkObject.Id;
            BatchNumber = pUnkObject.Number;
            SellerCode = pUnkObject.SellerId != null && pUnkObject.SellerId > 0 ? pUnkObject.Seller.Code : string.Empty;
            Seller = pUnkObject.SellerId != null && pUnkObject.SellerId > 0 ? pUnkObject.Seller.Name : string.Empty;
            BuyerCode = pUnkObject.BuyerId != null && pUnkObject.BuyerId > 0 ? pUnkObject.Buyer.Code : string.Empty;
            Buyer = pUnkObject.BuyerId != null && pUnkObject.BuyerId > 0 ? pUnkObject.Buyer.Name : string.Empty;
            ItemCode = pUnkObject.ItemId > 0 ? pUnkObject.Item.Code : string.Empty;
            Item = pUnkObject.ItemId > 0 ? pUnkObject.Item.Name : string.Empty;
            ItemTypeCode = pUnkObject.ItemTypeId > 0 ? pUnkObject.ItemType.Code : string.Empty;
            ItemType = pUnkObject.ItemTypeId > 0 ? pUnkObject.ItemType.Name : string.Empty;
            Quantity = pUnkObject.Quantity;
            Weight = pUnkObject.Weight;
            AverageWeight = pUnkObject.AverageWeight;
            Price = pUnkObject.Price;
            Amount = pUnkObject.Amount;
            Unsold = pUnkObject.Unsold;
            UnsoldMotiveId = (int)pUnkObject.UnsoldMotive;
        }

        #endregion
    }
}
