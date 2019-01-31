using System;
using System.ComponentModel;

namespace UGRS.Core.Auctions.DTO.Financials
{
    public class FoodChargeCheckLineDTO : INotifyPropertyChanged
    {
        #region Attributes

        bool mBolApplyFoodCharge;
        
        #endregion

        #region Properties

        public long Id { get; set; }

        public string BatchNumber { get; set; }

        public DateTime BatchDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool FoodCharge { get; set; }

        public bool FoodDeliveries { get; set; }

        public bool AlfalfaDeliveries { get; set; }

        public bool ApplyFoodCharge
        {
            get
            {
                return mBolApplyFoodCharge;
            }
            set
            {
                if (mBolApplyFoodCharge != value)
                {
                    mBolApplyFoodCharge = value;
                    OnPropertyChanged("ApplyFoodCharge");
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
