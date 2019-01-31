using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.DTO.Inventory
{
    public class BatchDivisionDTO : INotifyPropertyChanged
    {
        private int mIntBuyerClassification;
        private string mStrBuyerName;
        

        private int mIntHeadQtty { get; set; }
        public int HeadQtty { get; set; }
        public float TotalWeight { get; set; }
        public float AverageWeight { get; set; }
        public bool CellActive { get; set; }



        public int BuyerClassification 
        {
            get
            {
                return mIntBuyerClassification;
            }
            set
            {
                mIntBuyerClassification = value;
                OnPropertyChanged("BuyerClassification");
            }
        }
        public string BuyerName
        {
            get
            {
                return mStrBuyerName;
            }
            set
            {
                mStrBuyerName = value;
                OnPropertyChanged("BuyerName");
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
