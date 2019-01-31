using System.Collections.Generic;
using System.ComponentModel;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.DTO.Security
{
    public class SpecialFunctionDTO : INotifyPropertyChanged
    {
        #region Attributes

        bool mBolActive;

        #endregion

        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        public bool Active
        {
            get
            {
                return mBolActive;
            }
            set
            {
                if (mBolActive != value)
                {
                    mBolActive = value;
                    OnPropertyChanged("Active");
                }
            }
        }

        #endregion

        #region Constructor

        public SpecialFunctionDTO()
        {
            //Default constructor
        }

        public SpecialFunctionDTO(EnumItem pObjEnumItem)
        {
            SetValues(pObjEnumItem);
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

        #region Methods

        void SetValues(EnumItem pObjEnumItem)
        {
            Id = pObjEnumItem.Value;
            Name = pObjEnumItem.Text;
            Active = false;
        }

        #endregion
    }
}
