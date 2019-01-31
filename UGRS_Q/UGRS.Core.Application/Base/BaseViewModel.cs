using System.ComponentModel;

namespace UGRS.Core.Application.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string pStrPropertyName)
        {
            PropertyChangedEventHandler lObjHandler = this.PropertyChanged;
            if (lObjHandler != null)
            {
                var e = new PropertyChangedEventArgs(pStrPropertyName);
                lObjHandler(this, e);
            }
        }
    }
}
