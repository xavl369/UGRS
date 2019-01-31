using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchAuctions.xaml
    /// </summary>
    public partial class UCSearchAuction : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionServiceFactory;
        private ListCollectionView mLcvListData = null;
        private FilterEnum mEnmFilter;
        private AuctionSearchModeEnum mEnmSearchMode;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCSearchAuction()
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();
        }

        public UCSearchAuction(string pStrText, List<Auction> pLstObjAuctions, FilterEnum pEnmFilter, AuctionSearchModeEnum pEnmSearchMode)
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();

            if (!string.IsNullOrEmpty(pStrText))
            {
                txtSearch.Text = pStrText;
                txtSearch.Focus();
            }
            else
            {
                dgDataGrid.Focus();
            }

            mLcvListData = new ListCollectionView(pLstObjAuctions);
            dgDataGrid.ItemsSource = null;
            dgDataGrid.ItemsSource = mLcvListData;
            mEnmFilter = pEnmFilter;
            mEnmSearchMode = pEnmSearchMode;
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            Focusable = true;
            Keyboard.Focus(this);
        }

        private void dgDataGrid_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                ReturnResult(dgDataGrid.SelectedItem as Auction);
            }
            else if (char.IsLetterOrDigit((char)pObjArgs.Key))
            {
                txtSearch.Focus();
                txtSearch_KeyDown(pObjSender, pObjArgs);
            }
            else if(pObjArgs.Key == Key.Escape)
            {
                this.GetParent().Close();
            }
        }

        private void dgDataGrid_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            ReturnResult(dgDataGrid.SelectedItem as Auction);
        }

        private void txtSearch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                if (!grdSearch.IsBlocked())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    mObjWorker = new Thread(() => Search(lStrText));
                    mObjWorker.Start();
                }
            }
            else if (pObjArgs.Key == Key.Escape)
            {
                this.GetParent().Close();
            }
        }

        private void txtSearch_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            string lStrText = (pObjSender as TextBox).Text;
            Filter(lStrText);
        }

        #endregion

        #region Methods

        private void Filter(string pStrText)
        {
            if (mLcvListData != null)
            {
                if (string.IsNullOrEmpty(pStrText))
                {
                    mLcvListData.Filter = null;
                }
                else
                {
                    mLcvListData.Filter = new Predicate<object>(o => ((Auction)o).Folio.ToUpper().Contains(pStrText.ToUpper()));
                }

                dgDataGrid.ItemsSource = mLcvListData;
                dgDataGrid.SelectedIndex = 0;
            }
        }

        private void Search(string pStrText)
        {
            grdSearch.BlockUI();

            try
            {
                List<Auction> lLstObjAuctionsList = mObjAuctionServiceFactory.GetAuctionService().SearchAuctions(pStrText, mEnmFilter, mEnmSearchMode);

                this.Dispatcher.Invoke(() =>
                {
                    dgDataGrid.ItemsSource = null;
                    mLcvListData = new ListCollectionView(lLstObjAuctionsList);
                    dgDataGrid.ItemsSource = mLcvListData;
                });
            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdSearch.UnblockUI();
            }
        }

        private void ReturnResult(Auction pObjAcution)
        {
            WindowDialog lObjWindowDialog = this.GetParent() as WindowDialog;
            lObjWindowDialog.gObject = pObjAcution as object;
            this.GetParent().Close();
        }

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke(() =>
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        #endregion
    }
}
