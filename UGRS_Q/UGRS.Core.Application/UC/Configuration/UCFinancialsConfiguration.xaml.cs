using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCFinancialsConfiguration.xaml
    /// </summary>
    public partial class UCFinancialsConfiguration : UserControl
    {
        public UCFinancialsConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
