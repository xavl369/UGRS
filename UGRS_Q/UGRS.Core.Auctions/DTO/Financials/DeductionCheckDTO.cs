using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Financials
{
    public class DeductionCheckDTO : INotifyPropertyChanged
    {
        #region Attributes

        private bool mBolDeduct = false;
        private string mStrComment = "";

        #endregion

        #region Properties

        public long Id { get; set; }
        public long AuctionId { get; set; }
        public string AuctionFolio { get; set; }
        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }

        public bool Deduct
        {
            get
            {
                return mBolDeduct;
            }
            set
            {
                if (mBolDeduct != value)
                {
                    mBolDeduct = value;
                    OnPropertyChanged("Deduct");
                }
            }
        }

        public string Comments
        {
            get
            {
                return mStrComment;
            }
            set
            {
                if (mStrComment != value)
                {
                    mStrComment = value;
                    OnPropertyChanged("Comments");
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
