using System.ComponentModel;
using UGRS.Core.Application.Enum.Base;

namespace UGRS.Core.Application.Base
{
    public class BaseModel : INotifyPropertyChanged
    {
        private bool mBolInitialized;

        private ViewStatusEnum mEnmViewStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Initialized
        {
            get { return mBolInitialized; }
            set { mBolInitialized = value; }
        }

        public ViewStatusEnum ViewStatus
        {
            get { return mEnmViewStatus; }
            set
            {
                this.mEnmViewStatus = value;
                this.NotifyPropertyChanged("ViewStatus");
            }
        }

        protected void NotifyPropertyChanged(string pStrPropertyName)
        {
            PropertyChangedEventHandler lObjHandler = this.PropertyChanged;
            if (lObjHandler != null)
            {
                var e = new PropertyChangedEventArgs(pStrPropertyName);
                lObjHandler(this, e);
                Initialized = true;
            }
        }
    }
}
