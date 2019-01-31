using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Data.Auctions.Factories;
using System.Linq;
using UGRS.Core.Application.Extension.Controls;
using System.Text.RegularExpressions;
using UGRS.Core.Extension;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.DTO.Catalogs;
//using UGRS.Core.SDK.DI.Auctions.DTO;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCItemDefinitions.xaml
    /// </summary>
    public partial class UCItemDefinitions : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjInventoryFactory = new InventoryServicesFactory();
        private ListCollectionView mLcvListData = null; //Filtros
        private Item mObjItem;
        private ItemTypeDTO mObjItemType;
        private long mLonId = 0; //bandera si se requiere modificar/eliminar
        private bool mBolReadOnly;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCItemDefinitions()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjEventsArgs)
        {
            mObjWorker = new Thread(() => LoadDataGrid());
            mObjWorker.Start();
        }

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjEventsArgs)
        {
            if (grdItemDefinition.Valid())
            {
                mObjWorker = new Thread(() => SaveItemDefinition());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Definición de artículo", "Favor de completar todos los campos.", Window.GetWindow(this));
            }
        }

        private void btnNew_Click(object pObjSender, System.Windows.RoutedEventArgs pObjEventsArgs)
        {
            ResetForm();
        }

        private void btnDelete_Click(object pObjSender, System.Windows.RoutedEventArgs pObjEventsArgs)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
            {
                try
                {
                    mObjInventoryFactory.GetItemDefinitionService().Remove(mLonId);
                    ResetForm();
                    LoadDataGrid();
                }
                catch (Exception lObjException)
                {
                    CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
                }
            }
        }

        private void txtOrder_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjEventsArgs)
        {
            pObjEventsArgs.Handled = !pObjEventsArgs.Text.IsNumber();
        }

        private void txtItem_KeyDown(object pObjSender, KeyEventArgs pObjEventsArgs)
        {
            try
            {
                SetItemObject(ShowItemDialog(pObjSender, pObjEventsArgs));
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void txtItemType_KeyDown(object pObjSender, KeyEventArgs pObjEventsArgs)
        {
            try
            {
                SetItemTypeObject(ShowItemTypeDialog(pObjSender, pObjEventsArgs));
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!mBolReadOnly)
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        dgItemDefinitions.ItemsSource = null;
                        List<ItemDefinitionDTO> lLstObjItemDefinitions = mObjInventoryFactory.GetItemDefinitionService().GetDefinitionsList().Where(x => x.Item.Contains(txtSearch.Text) || x.ItemType.Contains(txtSearch.Text)).ToList();
                        mLcvListData = new ListCollectionView(lLstObjItemDefinitions);
                        dgItemDefinitions.ItemsSource = mLcvListData;
                    }
                    else
                    {
                        LoadDataGrid();
                    }
                }
                else
                {
                    dgItemDefinitions.SelectedIndex = 0;
                    ReturnItemDefinition();
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((ItemDefinitionDTO)o).Item.ToUpper().Contains(txtSearch.Text.ToUpper()) || ((ItemDefinitionDTO)o).ItemType.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgItemDefinitions.ItemsSource = mLcvListData;
            if (mBolReadOnly)
            {
                dgItemDefinitions.SelectedIndex = 0;
            }
        }

        private void dgItemDefinitions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mBolReadOnly)
            {
                ReturnItemDefinition();
            }
            else
            {
                ItemDefinitionDTO lObjItemDefinition = dgItemDefinitions.SelectedItem as ItemDefinitionDTO;
                SetItemDefinitionObject(lObjItemDefinition);

                mLonId = lObjItemDefinition.Id;
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
            }
        }

        private void dgItemDefinitions_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (mBolReadOnly)
            {
                if (e.Key == Key.Enter)
                {
                    ReturnItemDefinition();
                }
                if (char.IsLetterOrDigit((char)e.Key))
                {
                    txtSearch.Focus();
                }
            }
        }

        #endregion

        #region Methods

        private void LoadDataGrid()
        {
            FormLoading();
            try
            {
                string lStrTxtSearch = string.Empty;
                this.Dispatcher.Invoke((Action)delegate
                {
                    dgItemDefinitions.ItemsSource = null;
                    lStrTxtSearch = txtSearch.Text;
                });

                List<ItemDefinitionDTO> lLstObjItemDefinitions = mObjInventoryFactory.GetItemDefinitionService().GetDefinitionsList().Where(x => x.Item.Contains(lStrTxtSearch) || x.ItemType.Contains(lStrTxtSearch)).OrderBy(a => a.ItemType).ThenBy(b => b.Order).ToList();
                mLcvListData = new ListCollectionView(lLstObjItemDefinitions);
                this.Dispatcher.Invoke(() => { dgItemDefinitions.ItemsSource = mLcvListData; });
            }
            catch (Exception lObjException)
            {
                FormDefult();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefult();
            }
        }

        private void SaveItemDefinition()
        {
            FormLoading(true);
            try
            {
                ItemDefinition lObjItemDefinition = new ItemDefinition();

                this.Dispatcher.Invoke(() =>
                {
                    lObjItemDefinition.Id = mLonId;
                    lObjItemDefinition.Order = Convert.ToInt32(txtOrder.Text);
                    lObjItemDefinition.ItemId = mObjItem.Id;
                    lObjItemDefinition.ItemTypeId = mObjItemType.Id;
                });

                mObjInventoryFactory.GetItemDefinitionService().SaveOrUpdate(lObjItemDefinition);

                this.Dispatcher.Invoke(() => ResetForm());
            }
            catch (Exception lObjException)
            {
                FormDefult(true);
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefult(true);
            }
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private void FormLoading(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    txtSearch.IsEnabled = false;
                    grdList.BlockUI();
                }
                else
                {
                    grdItems.BlockUI();
                }
            });
        }

        private void FormDefult(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    txtSearch.IsEnabled = true;
                    grdList.UnblockUI();
                }
                else
                {
                    grdItems.UnblockUI();
                }
            });
        }

        private Item ShowItemDialog(object pObjSender, KeyEventArgs pObjEventsArgs)
        {
            Item lObjItem = null;

            if (pObjEventsArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                List<Item> lLstItem = SearchItem((pObjSender as TextBox).Text);
                if (lLstItem.Count == 1)
                {
                    lObjItem = lLstItem[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCItem = new UCItems(true, lLstItem, (pObjSender as TextBox).Text);
                    lObjItem = FunctionsUI.ShowWindowDialog(lUCItem, Window.GetWindow(this)) as Item;
                    (pObjSender as TextBox).Focusable = true;
                }
                (pObjSender as TextBox).Focus();
                (pObjSender as TextBox).FocusNext();
            }

            return lObjItem;
        }

        private ItemTypeDTO ShowItemTypeDialog(object pObjSender, KeyEventArgs pObjEventsArgs)
        {
            ItemTypeDTO lObjItemType = null;

            if (pObjEventsArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                List<ItemTypeDTO> lLstItemType = SearchItemType((pObjSender as TextBox).Text);
                if (lLstItemType.Count == 1)
                {
                    lObjItemType = lLstItemType[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCItemType = new UCItemTypes(true, lLstItemType, (pObjSender as TextBox).Text);
                    lObjItemType = FunctionsUI.ShowWindowDialog(lUCItemType, Window.GetWindow(this)) as ItemTypeDTO;
                    (pObjSender as TextBox).Focusable = true;
                }
            }

            return lObjItemType;
        }

        private void SetItemObject(Item pObjItem)
        {
            if (pObjItem != null)
            {
                mObjItem = pObjItem;
                txtItem.Text = pObjItem.Code;
                tbItem.Text = pObjItem.Name;
                btnSave.Focus();
            }
        }

        private void SetItemTypeObject(ItemTypeDTO pObjItemType)
        {
            if (pObjItemType != null)
            {
                mObjItemType = pObjItemType;
                txtItemType.Text = pObjItemType.Code;
                tbItemType.Text = pObjItemType.Name;
                txtItemType.Focus();
            }
        }

        private void SetItemDefinitionObject(ItemDefinitionDTO pObjItemDefinitionDTO)
        {
            SetItemObject(mObjInventoryFactory.GetItemService().Get(pObjItemDefinitionDTO.ItemId));
            SetItemTypeObject(mObjInventoryFactory.GetItemTypeService().GetCustomEntity(pObjItemDefinitionDTO.ItemTypeId));
            txtOrder.Text = pObjItemDefinitionDTO.Order.ToString();
        }

        private List<Item> SearchItem(string pStrItem)
        {
            return mObjInventoryFactory.GetItemService().GetList().Where(x => x.Active == true && ((x.Name.Contains(pStrItem)) || (x.Code.Contains(pStrItem)))).ToList();
        }

        private List<ItemTypeDTO> SearchItemType(string pStrItemType)
        {
            return mObjInventoryFactory.GetItemTypeService().GetCustomList().Where(x => x.Level == 3 && ( x.Name.ToUpper().Contains(pStrItemType.ToUpper()) ||
                                                                                        x.Code.ToUpper().Contains(pStrItemType.ToUpper()))).ToList();
        }

        private void ReturnItemDefinition()
        {
            Window lObjWindowParent = Window.GetWindow(this);
            lObjWindowParent.Close();
            WindowDialog lObjWindowDialog = lObjWindowParent as WindowDialog;

            ItemDefinitionDTO lObjItemDefinition = dgItemDefinitions.SelectedItem as ItemDefinitionDTO;
            lObjWindowDialog.gObject = lObjItemDefinition as object;
        }

        private void ResetForm()
        {
            grdItemDefinition.ClearControl();

            tbItem.Text = string.Empty;
            tbItemType.Text = string.Empty;
            lblMessage.Visibility = Visibility.Collapsed;

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;

            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;

            LoadDataGrid();
        }

        #endregion
    }
}
