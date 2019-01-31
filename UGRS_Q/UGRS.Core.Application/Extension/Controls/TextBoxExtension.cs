using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UGRS.Core.Application.Extension.Controls
{
    public static class TextBoxExtension
    {
        public static void ClearControl(this TextBox pObjTextBox)
        {
            pObjTextBox.Text = string.Empty;
            pObjTextBox.BorderBrush = Brushes.Black;
        }

        public static bool ValidRequired(this TextBox pObjTextBox)
        {
            if (string.IsNullOrEmpty(pObjTextBox.Text))
            {
                pObjTextBox.BorderBrush = Brushes.Red;
                return false;
            }
            else
            {
                pObjTextBox.BorderBrush = Brushes.Black;
                return true;
            }
        }

        public static void FocusNext(this TextBox pObjTextBox)
        {
            FocusNavigationDirection lObjfocusDirection = FocusNavigationDirection.Next;
            TraversalRequest lObjRequest = new TraversalRequest(lObjfocusDirection);

            if (pObjTextBox != null)
            {
                pObjTextBox.MoveFocus(lObjRequest);
            }
        }
    }
}
