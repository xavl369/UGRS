using System.Windows.Controls;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCComingSoon.xaml
    /// </summary>
    public partial class UCError : UserControl
    {
        public UCError()
        {
            InitializeComponent();
        }

        public UCError(string pStrTitle, string pStrMessage)
        {
            InitializeComponent();
            tblTitle.Text = pStrTitle;
            tblMessage.Text = pStrMessage;
        }

        public UCError(string pStrTitle, string pStrMessage, MaterialDesignThemes.Wpf.PackIconKind pEnmIcon)
        {
            InitializeComponent();
            tblTitle.Text = pStrTitle;
            tblMessage.Text = pStrMessage;
            imgIcon.Kind = pEnmIcon;
        }
    }
}
