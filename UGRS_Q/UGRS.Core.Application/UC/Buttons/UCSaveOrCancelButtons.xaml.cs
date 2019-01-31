using System.Windows;
using System.Windows.Controls;

namespace UGRS.Core.Application.UC.Buttons
{
    /// <summary>
    /// Interaction logic for UCButtons.xaml
    /// </summary>
    public partial class UCSaveOrCancelButtons : UserControl
    {
        public UCSaveOrCancelButtons()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window lObjParent = Window.GetWindow(this);
            lObjParent.Close();
        }
    }
}
