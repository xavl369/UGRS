using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;
using UGRS.Core.Application.Base;

namespace UGRS.Application.Auctions.Models
{
    public class LoginModel : BaseModel, IDataErrorInfo
    {
        private string mStrUserName;
        private SecureString mStrPassword;

        [Required(ErrorMessage = "Favor de ingresar su nombre de usuario.")]
        public string UserName
        {
            get { return mStrUserName; }
            set
            {
                this.mStrUserName = value;
                this.NotifyPropertyChanged("UserName");
            }
        }

        public SecureString Password
        {
            get { return mStrPassword; }
            set
            {
                this.mStrPassword = value;
                this.NotifyPropertyChanged("Password");
            }
        }

        #region IDataErrorInfo Members

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                if(this.Initialized)
                {
                    if (columnName == "UserName")
                    {
                        if (string.IsNullOrEmpty(UserName))
                            return "Favor de ingresar.";

                        if (UserName.Length < 3)
                            return "Favor de ingresar un nombre de usuario mayor a 3 caracteres.";
                    }
                    if (columnName == "Password")
                    {
                        if (string.IsNullOrEmpty(Password.ToString()))
                            return "Favor de ingresar su contraseña.";
                    }
                }
                return string.Empty;
            }
        }

        #endregion
    }
}
