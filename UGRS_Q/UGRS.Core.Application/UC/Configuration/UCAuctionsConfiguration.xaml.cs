using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.UC.Configuration
{
    /// <summary>
    /// Interaction logic for UCAuctionsConfiguration.xaml
    /// </summary>
    public partial class UCAuctionsConfiguration : UserControl
    {
        public string Warehouse
        {
            get{ return txtWarehouse.Text;}
            set{ txtWarehouse.Text = value;}
        }

        public string Series
        {
            get { return txtSeries.Text; }
            set { txtSeries.Text = value; }
        }

        public UCAuctionsConfiguration()
        {
            InitializeComponent();
        }

        public bool Valid()
        {
            return this.grdForm.Valid();
        }
    }
}
