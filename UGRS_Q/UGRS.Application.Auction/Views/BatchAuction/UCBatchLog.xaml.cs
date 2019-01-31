using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Auctions.DTO.Auction;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCBatchLog : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsFactory;
        private long mLonBatchId;

        #endregion

        #region Constructor

        public UCBatchLog(long pLonBatchId)
        {
            InitializeComponent();
            mLonBatchId = pLonBatchId;
            mObjAuctionsFactory = new AuctionsServicesFactory();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgLog.ItemsSource = null;
            dgLog.ItemsSource = mObjAuctionsFactory.GetBatchLogService().GetBatchLogList(mLonBatchId);
        }

        private void dgPartner_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Window lObjWindowParent = Window.GetWindow(this);
                BaseForm lObjWindowDialog = lObjWindowParent as BaseForm;
                BatchLogDTO lObjLog = dgLog.SelectedItem as BatchLogDTO;
                lObjWindowDialog.ResultObject = lObjLog as object;
                lObjWindowDialog.DialogResult = true;
                lObjWindowParent.Close();
            }
            catch
            {
                //Ignore
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.GetParent().Close();
        }

        #endregion
    }
}
