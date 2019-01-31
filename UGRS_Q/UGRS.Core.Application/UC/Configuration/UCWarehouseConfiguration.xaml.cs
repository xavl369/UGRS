using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCWarehouseConfiguration.xaml
    /// </summary>
    public partial class UCWarehouseConfiguration : UserControl
    {
        public UCWarehouseConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
