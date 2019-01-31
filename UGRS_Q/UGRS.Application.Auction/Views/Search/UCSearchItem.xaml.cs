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
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchItem.xaml
    /// </summary>
    public partial class UCSearchItem : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjInventoryServicesFactory;
        private ListCollectionView mLcvListData = null;
        private FilterEnum mEnmFilter;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCSearchItem()
        {
            InitializeComponent();
            mObjInventoryServicesFactory = new InventoryServicesFactory();
        }

        public UCSearchItem(string pStrText, List<Item> pLstObjItems, FilterEnum pEnmFilter)
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

            mLcvListData = new ListCollectionView(pLstObjItems);
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
                ReturnResult(dgDataGrid.SelectedItem as Item);
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
            ReturnResult(dgDataGrid.SelectedItem as Item);
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
                    mLcvListData.Filter = new Predicate<object>(o => ((Item)o).Code.ToUpper().Contains(pStrText.ToUpper()) ||
                                                                     ((Item)o).Name.ToUpper().Contains(pStrText.ToUpper()));
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
                List<Item> lLstObjItemsList = mObjInventoryServicesFactory.GetItemService().SearchItem(pStrText, mEnmFilter);

                this.Dispatcher.Invoke(() =>
                {
                    dgDataGrid.ItemsSource = null;
                    mLcvListData = new ListCollectionView(lLstObjItemsList);
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

        private void ReturnResult(Item pObjItem)
        {
            WindowDialog lObjWindowDialog = this.GetParent() as WindowDialog;
            lObjWindowDialog.gObject = pObjItem as object;
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
