
using System.ComponentModel;
namespace UGRS.Core.Auctions.DTO.Inspection
{
    public class ChargeDTO : INotifyPropertyChanged
    {
        private double mDblAmount;

        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public string RFC { get; set; }

        public double Amount
        {
            get
            {
                return mDblAmount;
            }
            set
            {
                mDblAmount = value;
                OnPropertyChanged("Amount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
