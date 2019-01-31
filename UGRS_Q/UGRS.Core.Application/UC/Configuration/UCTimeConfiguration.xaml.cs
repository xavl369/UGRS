using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCTimeConfiguration.xaml
    /// </summary>
    public partial class UCTimeConfiguration : UserControl
    {
        public UCTimeConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
