using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using System.Linq;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchBusinessPartner.xaml
    /// </summary>
    public partial class UCSearchBusinessPartner : UserControl
    {
        #region Attributes

        private BusinessServicesFactory mObjBusinessServicesFactory;
        private InventoryServicesFactory mObjInventoryServiceFactory = new InventoryServicesFactory();
        private AuctionsServicesFactory mObjAuctionFactory = new AuctionsServicesFactory();
        private ListCollectionView mLcvListData = null;
        private FilterEnum mEnmFilter;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCSearchBusinessPartner()
        {
            InitializeComponent();
            mObjBusinessServicesFactory = new BusinessServicesFactory();
        }

        public UCSearchBusinessPartner(string pStrText, List<Partner> pLstObjPartners, FilterEnum pEnmFilter)
        {
            InitializeComponent();
            mObjBusinessServicesFactory = new BusinessServicesFactory();

            if (!string.IsNullOrEmpty(pStrText))
            {
                txtSearch.Text = pStrText;
                txtSearch.Focus();
            }
            else
            {
                dgDataGrid.Focus();
            }

            mLcvListData = new ListCollectionView(pLstObjPartners);
            dgDataGrid.ItemsSource = null;
            dgDataGrid.ItemsSource = mLcvListData;
            mEnmFilter = pEnmFilter;
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
                ReturnResult(dgDataGrid.SelectedItem as Partner);
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
            ReturnResult(dgDataGrid.SelectedItem as Partner);
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
                    mLcvListData.Filter = new Predicate<object>(o => ((Partner)o).Code.ToUpper().Contains(pStrText.ToUpper()) || 
                                                                     ((Partner)o).Name.ToUpper().Contains(pStrText.ToUpper()));
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
                DateTime lDtAuctionDate = mObjAuctionFactory.GetAuctionService().GetActiveAuction().Date;
                
                List<long> lLstLonSellersWithStock = mObjInventoryServiceFactory.GetStockService().GetListByWhs().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && x.Quantity > 0).Select(x => x.CustomerId).Distinct().ToList();

                lLstLonSellersWithStock.AddRange((mObjInventoryServiceFactory.GetGoodsReceiptService().GetList().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && !x.Processed).Select(x => x.CustomerId).Distinct().ToList()));

                //List<Partner> lLstObjBatchesList = mObjBusinessServicesFactory.GetPartnerService().SearchPartner(pStrText, mEnmFilter);
                List<Partner> lLstObjBatchesList = mObjBusinessServicesFactory.GetPartnerService().SearchPartnerWithStock(pStrText, FilterEnum.ACTIVE, lLstLonSellersWithStock); 

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

        private void ReturnResult(Partner pObjPartner)
        {
            WindowDialog lObjWindowDialog = this.GetParent() as WindowDialog;
            lObjWindowDialog.gObject = pObjPartner as object;
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
