using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Auctions.Entities.Inventory;

namespace UGRS.Application.Auctions.AuctionClose
{
    /// <summary>
    /// Interaction logic for UCTransactionsPreview.xaml
    /// </summary>
    public partial class UCTransactionsPreview : UserControl
    {
        IList<GoodsReceipt> mLstObjTemporaryGoodsReceipts;
        IList<GoodsIssue> mLstObjBuyerGoodsIssues;
        IList<GoodsReturn> mLstObjGoodsReturns;
        public UCTransactionsPreview( IList<GoodsReceipt> pLstObjTemporaryGoodsReceipts, IList<GoodsIssue> pLstObjBuyerGoodsIssues, IList<GoodsReturn> pLstObjGoodsReturns)
        {
            InitializeComponent();
            mLstObjBuyerGoodsIssues = pLstObjBuyerGoodsIssues;
            mLstObjGoodsReturns = pLstObjGoodsReturns;
            mLstObjTemporaryGoodsReceipts = pLstObjTemporaryGoodsReceipts;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgGoodsIssue.ItemsSource = mLstObjBuyerGoodsIssues.ToList();
            dgGoodsReceipt.ItemsSource = mLstObjTemporaryGoodsReceipts.ToList();
            dgGoodsReturn.ItemsSource = mLstObjGoodsReturns.ToList();
        }

    }
}
