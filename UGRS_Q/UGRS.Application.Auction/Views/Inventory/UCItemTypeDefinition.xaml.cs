using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Extension;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCItemTypeDefinition.xaml
    /// </summary>
    public partial class UCItemTypeDefinition : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjInventoryFactory = new InventoryServicesFactory();
        private AuctionsServicesFactory mObjAuctionsFactory = new AuctionsServicesFactory();
        private ListCollectionView mLcvListData = null;
        private AuctionTypeEnum mEnmAuctionType = 0;
        private long mLonDefinitionId = 0;
        private long mLonItemTypeId = 0;
        private Thread mObjWorker;
        
        #endregion

        #region Constructor

        public UCItemTypeDefinition()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => LoadDataGrid());
            mObjWorker.Start();

            new Thread(() => LoadAuctionTypes()).Start();
        }

        #endregion

        #region TextBox

        private void txtItemType_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter)
                {
                    SetItemTypeObject(ShowItemTypeDialog(pObjSender, pObjArgs));
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void txtSearch_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((ItemTypeDefinitionDTO)o).AuctionTypeName.ToUpper().Contains(txtSearch.Text.ToUpper()) || 
                                                                 ((ItemTypeDefinitionDTO)o).ItemType.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgItemTypeDefinitions.ItemsSource = mLcvListData;
        }

        private void txtSearch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                LoadDataGrid();
            }
        }

        #endregion

        #region DataGrid

        private void dgItemTypeDefinitions_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            ItemTypeDefinitionDTO lObjItemTypeDefinition = dgItemTypeDefinitions.SelectedItem as ItemTypeDefinitionDTO;

            if (lObjItemTypeDefinition != null)
            {
                SetDefinitionObject(lObjItemTypeDefinition);
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                lblMessage.Visibility = Visibility.Collapsed;
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                txtItemType.BorderBrush = Brushes.Black;
            }
        }

        private void dgItemTypeDefinitions_PreviewKeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {

        }

        #endregion

        #region Button

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (grdItemTypeDefinition.Valid())
            {
                mObjWorker = new Thread(() => SaveItemTypeDefinition());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Definición de tipo de artículo", "Favor de completar todos los campos.", this.GetParent());
            }
        }

        private void btnNew_Click(object pObjSender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void btnDelete_Click(object pObjSender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", this.GetParent()) == true)
            {
                try
                {
                    mObjInventoryFactory.GetItemTypeDefinitionService().Remove(mLonDefinitionId);
                    ResetForm();
                    LoadDataGrid();
                }
                catch (Exception lObjException)
                {
                    CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        private void LoadDataGrid()
        {
            FormLoading();

            try
            {
                string lStrSearch = string.Empty;

                this.Dispatcher.Invoke((Action)delegate
                {
                    dgItemTypeDefinitions.ItemsSource = null;
                    lStrSearch = txtSearch.Text;
                });

                mLcvListData = new ListCollectionView(SearchItemTypeDefinitions(lStrSearch));

                this.Dispatcher.Invoke(() => 
                {
                    dgItemTypeDefinitions.ItemsSource = mLcvListData; 
                });
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

        private void LoadAuctionTypes()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    cboAuctionType.ItemsSource = UGRS.Core.Extension.Enum.EnumExtension.GetEnumItemList<AuctionTypeEnum>();
                    cboAuctionType.DisplayMemberPath = "Text";
                    cboAuctionType.SelectedValuePath = "Value";
                });
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

        private void SaveItemTypeDefinition()
        {
            FormLoading();

            try
            {
                ItemTypeDefinition lObjItemTypeDefinition = new ItemTypeDefinition();

                this.Dispatcher.Invoke(() =>
                {
                    lObjItemTypeDefinition.Id = mLonDefinitionId;
                    lObjItemTypeDefinition.ItemTypeId = mLonItemTypeId;
                    lObjItemTypeDefinition.AuctionType = cboAuctionType.SelectedValue.GetValue<AuctionTypeEnum>();
                });

                mObjInventoryFactory.GetItemTypeDefinitionService().SaveOrUpdate(lObjItemTypeDefinition);

                this.Dispatcher.Invoke(() => ResetForm());
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

        private List<ItemTypeDefinitionDTO> SearchItemTypeDefinitions(string pStrSearch)
        {
            return mObjInventoryFactory
                    .GetItemTypeDefinitionService()
                    .GetDefinitionsList()
                    .Where(x => x.AuctionTypeName.ToUpper().Contains(pStrSearch.ToUpper()) || x.ItemType.ToUpper().Contains(pStrSearch.ToUpper()))
                    .ToList();
        }

        private void SetItemTypeObject(ItemType pObjItemType)
        {
            if (pObjItemType != null)
            {
                mLonItemTypeId = pObjItemType.Id;
                txtItemType.Text = pObjItemType.Code;
                tbItemType.Text = pObjItemType.Name;
            }
            else
            {
                mLonItemTypeId = 0;
                txtItemType.Text = string.Empty;
                tbItemType.Text = string.Empty;
            }
            txtItemType.Focus();
        }

        private ItemType ShowItemTypeDialog(object pObjSender, KeyEventArgs pObjArgs)
        {
            ItemType lObjItemType = null;

            if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                List<ItemType> lLstObjItemTypes = mObjInventoryFactory.GetItemTypeService().SearchItemType((pObjSender as TextBox).Text, FilterEnum.ACTIVE);
                if (lLstObjItemTypes.Count == 1)
                {
                    lObjItemType = lLstObjItemTypes[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCItemType = new UCSearchItemType((pObjSender as TextBox).Text, lLstObjItemTypes, FilterEnum.ACTIVE);
                    lObjItemType = FunctionsUI.ShowWindowDialog(lUCItemType, Window.GetWindow(this)) as ItemType;
                    (pObjSender as TextBox).Focusable = true;
                }
            }
            return lObjItemType;
        }

        private void SetDefinitionObject(ItemTypeDefinitionDTO pObjItemTypeDefinition)
        {
            mLonDefinitionId = pObjItemTypeDefinition.Id;
            mEnmAuctionType = pObjItemTypeDefinition.AuctionType;
            mLonItemTypeId = pObjItemTypeDefinition.ItemTypeId;
            cboAuctionType.SelectedValue = (int)pObjItemTypeDefinition.AuctionType;
            txtItemType.Text = pObjItemTypeDefinition.ItemType;
        }

        private void ResetForm()
        {
            grdItemTypeDefinition.ClearControl();

            cboAuctionType.SelectedValue = 0;
            cboAuctionType.Text = "Favor de seleccionar";

            txtItemType.Text = string.Empty;
            tbItemType.Text = string.Empty;

            lblMessage.Visibility = Visibility.Collapsed;

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonDefinitionId = 0;

            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;

            LoadDataGrid();
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                txtSearch.IsEnabled = false;
                grdItemTypes.BlockUI();
            });
        }

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                txtSearch.IsEnabled = true;
                grdItemTypes.UnblockUI();
            });
        }

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke(()=> 
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        #endregion
    }
}
