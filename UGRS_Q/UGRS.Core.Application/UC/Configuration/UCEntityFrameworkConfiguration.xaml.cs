using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCEntityFrameworkConfiguration.xaml
    /// </summary>
    public partial class UCEntityFrameworkConfiguration : UserControl
    {
        public UCEntityFrameworkConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
