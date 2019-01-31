using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCWeighingMachineConfiguration.xaml
    /// </summary>
    public partial class UCWeighingMachineConfiguration : UserControl
    {
        public UCWeighingMachineConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
