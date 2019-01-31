using System.Windows.Controls;
using System.Windows.Media;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class PasswordBoxExtension
    {
        public static void ClearControl(this PasswordBox pObjPasswordBox)
        {
            pObjPasswordBox.Password = string.Empty;
            pObjPasswordBox.BorderBrush = Brushes.Black;
        }

        public static bool ValidRequired(this PasswordBox pObjPasswordBox)
        {
            if (string.IsNullOrEmpty(pObjPasswordBox.Password))
            {
                pObjPasswordBox.BorderBrush = Brushes.Red;
                return false;
            }
            else
            {
                pObjPasswordBox.BorderBrush = Brushes.Black;
                return true;
            }
        }
    }
}
