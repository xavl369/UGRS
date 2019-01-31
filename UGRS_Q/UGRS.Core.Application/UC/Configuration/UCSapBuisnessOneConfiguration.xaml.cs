using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCSapBuisnessOneConfiguration.xaml
    /// </summary>
    public partial class UCSapBuisnessOneConfiguration : UserControl
    {
        public UCSapBuisnessOneConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
