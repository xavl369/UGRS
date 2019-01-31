using System.Collections.Generic;
using System.ComponentModel;

namespace UGRS.Core.Auctions.DTO.Financials
{
    public class FoodChargeCheckDTO : INotifyPropertyChanged
    {
        #region Attributes

        public List<FoodChargeCheckLineDTO> mLstObjLines;

        #endregion

        #region Properties

        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }

        public List<FoodChargeCheckLineDTO> Lines
        {
            get
            {
                return mLstObjLines;
            }
            set
            {
                if (mLstObjLines != value)
                {
                    mLstObjLines = value;
                    OnPropertyChanged("Lines");
                }
            }
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
