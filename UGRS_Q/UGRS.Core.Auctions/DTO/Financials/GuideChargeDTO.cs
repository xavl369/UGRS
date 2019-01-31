using System.ComponentModel;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.DTO.Financials
{
    public class GuideChargeDTO : INotifyPropertyChanged
    {
        #region Attributes

        private double mDblAmount;

        #endregion

        #region Properties

        public long Id { get; set; }
        public long AuctionId { get; set; }
        public string AuctionFolio { get; set; }
        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }

        public double Amount
        {
            get
            {
                return mDblAmount;
            }
            set
            {
                if (mDblAmount != value)
                {
                    mDblAmount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }

        #endregion

        #region Constructor

        public GuideChargeDTO()
        {

        }

        public GuideChargeDTO(GuideCharge pObjGuideCharge)
        {
            Id = pObjGuideCharge.Id;
            AuctionId  = pObjGuideCharge.AuctionId;
            AuctionFolio = pObjGuideCharge.Auction.Folio;
            SellerId  = pObjGuideCharge.AuctionId;
            SellerCode = pObjGuideCharge.Seller.Code;
            SellerName = pObjGuideCharge.Seller.Name;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
