using System.Collections.Generic;
using System.ComponentModel;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class GoodsIssueDTO : INotifyPropertyChanged
    {
        #region Attributes

        private int mIntQuantityToPick;

        #endregion

        #region Properties

        public long ItemTypeId { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemTypeName { get; set; }

        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public int TotalQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int ReturnedDeliveriesQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public ItemTypeGenderEnum Gender { get; set; }

        public int QuantityToPick
        {
            get
            {
                return mIntQuantityToPick;
            }
            set
            {
                mIntQuantityToPick = value;
                OnPropertyChanged("QuantityToPick");
            }
        }

        public List<GoodsIssueLineDTO> Batches { get; set; }

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
