using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UGRS.Core.Auctions.Entities.System;

namespace UGRS.Core.Auctions.DTO.Security
{
    public class ModuleDTO : INotifyPropertyChanged
    {
        #region Attributes

        bool mBolActive;
        bool mBolExpanded;
        
        #endregion

        #region Properties

        public long Id { get; set;}
        public int Position {get; set;}
        public string Icon {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
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

        public bool Expanded
        {
            get
            {
                return mBolExpanded;
            }
            set
            {
                if (mBolExpanded != value)
                {
                    mBolExpanded = value;
                    OnPropertyChanged("Expanded");
                }
            }
        }

        public IList<SectionDTO> Sections { get; set; }
        
        #endregion

        #region Constructor

        public ModuleDTO()
        {
            //Default constructor
        }

        public ModuleDTO(Module pObjModule)
        {
            SetValues(pObjModule);
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

        void SetValues(Module pObjModule)
        {
            Id = pObjModule.Id;
            Position = pObjModule.Position;
            Icon = pObjModule.Icon;
            Name = pObjModule.Name;
            Description = pObjModule.Description;
            Path = pObjModule.Path;
            Active = pObjModule.Active;
            Expanded = false;

            if (pObjModule.Sections != null && pObjModule.Sections.Count > 0)
            {
                Sections = pObjModule.Sections.Select(x => new SectionDTO(x)).ToList();
            }
        }

        #endregion
    }
}
