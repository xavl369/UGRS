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
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchBatches.xaml
    /// </summary>
    public partial class UCSearchBatch : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionServiceFactory;
        private ListCollectionView mLcvListData = null;
        private long mLonAuctionId;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCSearchBatch()
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();
        }

        public UCSearchBatch(string pStrText, List<Batch> pLstObjBatches, long pLonAuctionId)
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

            mLcvListData = new ListCollectionView(pLstObjBatches);
            dgDataGrid.ItemsSource = null;
            dgDataGrid.ItemsSource = mLcvListData;
            mLonAuctionId = pLonAuctionId;
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
                ReturnResult(dgDataGrid.SelectedItem as Batch);
            }
            else if (char.IsLetterOrDigit((char)pObjArgs.Key))
            {
                txtSearch.Focus();
                txtSearch_KeyDown(pObjSender, pObjArgs);
            }
            else if (pObjArgs.Key == Key.Escape)
            {
                this.GetParent().Close();
            }
        }

        private void dgDataGrid_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            ReturnResult(dgDataGrid.SelectedItem as Batch);
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
                    mLcvListData.Filter = new Predicate<object>(o => ((Batch)o).Number.ToString().ToUpper().Contains(pStrText.ToUpper()));
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
                List<Batch> lLstObjBatchesList = mObjAuctionServiceFactory.GetBatchService().SearchBatches(pStrText, mLonAuctionId);

                this.Dispatcher.Invoke(() =>
                {
                    dgDataGrid.ItemsSource = null;
                    mLcvListData = new ListCollectionView(lLstObjBatchesList);
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

        private void ReturnResult(Batch pObjBatch)
        {
            WindowDialog lObjWindowDialog = this.GetParent() as WindowDialog;
            lObjWindowDialog.gObject = pObjBatch as object;
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
