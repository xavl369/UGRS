using System.ComponentModel;
using UGRS.Core.Auctions.Entities.System;

namespace UGRS.Core.Auctions.DTO.Security
{
    public class SectionDTO : INotifyPropertyChanged
    {
        #region Attributes

        bool mBolActive;

        #endregion

        #region Properties

        public long Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

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

        public SectionDTO()
        {
            //Default constructor
        }

        public SectionDTO(Section pObjSection)
        {
            SetValues(pObjSection);
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

        void SetValues(Section pObjSection)
        {
            Id = pObjSection.Id;
            Position = pObjSection.Position;
            Name = pObjSection.Name;
            Description = pObjSection.Description;
            Path = pObjSection.Path;
            Active = pObjSection.Active;
        }

        #endregion
    }
}
