using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using System.Linq;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchItemType.xaml
    /// </summary>
    public partial class UCSearchItemType : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjInventoryServicesFactory;
        private ListCollectionView mLcvListData = null;
        private AuctionTypeEnum mEnmAuctionType;
        private FilterEnum mEnmFilter;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCSearchItemType()
        {
            InitializeComponent();
            mObjInventoryServicesFactory = new InventoryServicesFactory();
        }

        public UCSearchItemType(string pStrText, List<ItemType> pLstObjItemTypes, FilterEnum pEnmFilter)
        {
            InitializeComponent();
            mObjInventoryServicesFactory = new InventoryServicesFactory();

            if (!string.IsNullOrEmpty(pStrText))
            {
                txtSearch.Text = pStrText;
                txtSearch.Focus();
            }
            else
            {
                dgDataGrid.Focus();
            }

            mLcvListData = new ListCollectionView(pLstObjItemTypes);
            dgDataGrid.ItemsSource = null;
            dgDataGrid.ItemsSource = mLcvListData;
            mEnmFilter = pEnmFilter;
        }

        public UCSearchItemType(string pStrText, AuctionTypeEnum pEnmAuctionType, List<ItemType> pLstObjItemTypes, FilterEnum pEnmFilter)
        {
            InitializeComponent();
            mObjInventoryServicesFactory = new InventoryServicesFactory();

            if (!string.IsNullOrEmpty(pStrText))
            {
                txtSearch.Text = pStrText;
                txtSearch.Focus();
            }
            else
            {
                dgDataGrid.Focus();
            }

            mLcvListData = new ListCollectionView(pLstObjItemTypes);
            dgDataGrid.ItemsSource = null;
            dgDataGrid.ItemsSource = mLcvListData;

            mEnmAuctionType = pEnmAuctionType;
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
                ReturnResult(dgDataGrid.SelectedItem as ItemType);
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
            ReturnResult(dgDataGrid.SelectedItem as ItemType);
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
                    mLcvListData.Filter = new Predicate<object>(o => ((ItemType)o).Code.ToUpper().Contains(pStrText.ToUpper()) ||
                                                                     ((ItemType)o).Name.ToUpper().Contains(pStrText.ToUpper()));
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
                List<ItemType> lLstObjItemTypesList = new List<ItemType>();

                if (mEnmFilter == FilterEnum.AUCTION)
                {
                    lLstObjItemTypesList = mObjInventoryServicesFactory.GetItemTypeService().SearchItemTypeByAuctionType(pStrText, mEnmAuctionType, mEnmFilter).Where(x => (mObjInventoryServicesFactory.GetItemDefinitionService().GetDefinitions(x.Id))).ToList();
                }
                else
                {
                    lLstObjItemTypesList = mObjInventoryServicesFactory.GetItemTypeService().SearchItemType(pStrText, mEnmFilter);
                }

                this.Dispatcher.Invoke(() =>
                {
                    dgDataGrid.ItemsSource = null;
                    mLcvListData = new ListCollectionView(lLstObjItemTypesList);
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

        private void ReturnResult(ItemType pObjItemType)
        {
            WindowDialog lObjWindowDialog = this.GetParent() as WindowDialog;
            lObjWindowDialog.gObject = pObjItemType as object;
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

