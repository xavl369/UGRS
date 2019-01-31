using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.DTO.Auctions
{
    public class BatchEasyDTO : INotifyPropertyChanged
    {
        private int mIntReturnQuantity;
        private int mIntExitQuantity;

        #region Properties

        public long? Id { get; set; }
        public int Number { get; set; }
        public string AuctionFolio { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemTypeName { get; set; }
        public long? SellerId { get; set; }
        public string SellerName { get; set; }
        public long? BuyerId { get; set; }
        public string Buyer { get; set; }
        public int TotalQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public int ExitQuantity
        {
            get
            {
                return mIntExitQuantity;
            }
            set
            {
                mIntExitQuantity = value;
                OnPropertyChanged("ExitQuantity");
            }
        }

        public int ReturnQuantity
        {
            get
            {
                return mIntReturnQuantity;
            }
            set
            {
                mIntReturnQuantity = value;
                OnPropertyChanged("ReturnQuantity");
            }
        }
        
        public IList<DetailedBatchLineDTO> Lines { get; set; }

        public bool Delivered { get; set; }

        #endregion

        #region Constructor

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

        #region Methods

        #endregion
    }
}
