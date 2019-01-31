using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCLocationConfiguration.xaml
    /// </summary>
    public partial class UCLocationConfiguration : UserControl
    {
        public UCLocationConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
