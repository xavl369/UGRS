using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using UGRS.Application.Auctions.AuctionClose;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using System;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCAuctionClosePreview.xaml
    /// </summary>
    public partial class UCAuctionClosePreview : UserControl
    {
        Auction mObjAuction = new Auction();
        IList<Invoice> mLstInvoice;
        IList<JournalEntry> mLstJournal;
        IList<GoodsReceipt> mLstObjTemporaryGoodsReceipts;
        IList<GoodsIssue> mLstObjBuyerGoodsIssues;
        IList<GoodsReturn> mLstObjGoodsReturns;
      
        public UCAuctionClosePreview(Auction pObjAuction, IList<Invoice> pLstInvoices, IList<JournalEntry> pLstJournal, IList<GoodsReceipt> pLstObjTemporaryGoodsReceipts, IList<GoodsIssue> pLstObjBuyerGoodsIssues, IList<GoodsReturn> pLstObjGoodsReturns)
        {
            InitializeComponent();
            mObjAuction = pObjAuction;
            mLstInvoice = pLstInvoices;
            mLstJournal = pLstJournal; 
            mLstObjBuyerGoodsIssues = pLstObjBuyerGoodsIssues;
            mLstObjGoodsReturns = pLstObjGoodsReturns;
            mLstObjTemporaryGoodsReceipts = pLstObjTemporaryGoodsReceipts;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE)//DirectDeal
            {
                ExpDirectDeal.Visibility = Visibility.Collapsed;
                ExpBatches.Visibility = Visibility.Visible; 
                ExpBatches.Content = new UCBatchesList(mObjAuction);
            }
            else
            {
                ExpDirectDeal.Visibility = Visibility.Visible;
                ExpBatches.Visibility = Visibility.Collapsed;
                ExpDirectDeal.Content = new UCDirectDealList(mObjAuction);
            }
            
             ExpInvoice.Content = new UCInvoiceList(mLstInvoice);
             ExpJourney.Content = new UCJournalEntryList(mLstJournal);
             ExpTransactions.Content = new UCTransactionsPreview(mLstObjTemporaryGoodsReceipts, mLstObjBuyerGoodsIssues, mLstObjGoodsReturns);
            //UCBatchesList(mObjInvoice)
            //ExpAsientos.Content = new UC
        }

        private void ExpBatches_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                UIElementCollection lObjElements = SkpExpander.Children;
                //List<FrameworkElement> lLstObjElements = lObjElements.Cast<FrameworkElement>().ToList();

                foreach (Expander lObjExpander in lObjElements)
                {
                    if (lObjExpander.Header != (sender as Expander).Header)
                    {
                        lObjExpander.IsExpanded = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
