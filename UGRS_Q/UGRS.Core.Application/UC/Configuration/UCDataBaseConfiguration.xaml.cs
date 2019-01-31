using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCDataBaseConfiguration.xaml
    /// </summary>
    public partial class UCDataBaseConfiguration : UserControl
    {
        public UCDataBaseConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
