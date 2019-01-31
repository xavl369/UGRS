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
using UGRS.Core.Auctions.DTO.Catalogs;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Catalogs;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.DTO.Utility;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCItemTypes.xaml
    /// </summary>
    public partial class UCItemTypes : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjServiceFactory = new InventoryServicesFactory();
        private ListCollectionView mLcvListData = null;
        private bool mBolReadOnly;
        private long mLonId = 0; //bandera si se requiere modificar/eliminar
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCItemTypes()
        {
            InitializeComponent();
        }

        public UCItemTypes(bool pBolReadOnly)
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            mBolReadOnly = pBolReadOnly;
            if (pBolReadOnly)
            {
                brdNewRegistry.Visibility = Visibility.Collapsed;
                dgItemType.Focus();
            }
        }

        public UCItemTypes(bool pBolReadOnly, List<ItemTypeDTO> lLstItem, string pStrSearch)
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
            mBolReadOnly = pBolReadOnly;

            mLcvListData = new ListCollectionView(lLstItem);
            dgItemType.ItemsSource = mLcvListData;

            if (pBolReadOnly)
            {
                brdNewRegistry.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(pStrSearch))
                {
                    txtSearch.Focus();
                    txtSearch.Text = pStrSearch;
                }
                else
                {
                    dgItemType.Focus();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Evento al terminar de cargar la pantalla.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mBolReadOnly)
            {
                LoadComboBoxes();
                mObjWorker = new Thread(() => LoadDataGrid());
                mObjWorker.Start();
            }
        }

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (grdItemType.Valid())
            {
                mObjWorker = new Thread(() => SaveOrUpdate());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Categoría de subasta", "Favor de completar los campos.", Window.GetWindow(this));
            }
        }

        /// <summary>
        /// Inicializa controles.
        /// </summary>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        /// <summary>
        /// Elimina un registro.
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
            {
                mObjServiceFactory.GetItemDefinitionService().Remove(mLonId);
                ResetForm();
            }
        }

        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgAuctionType_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mBolReadOnly)
            {
                ReturnItem();
            }
            else
            {
                ItemTypeDTO lObjItemType = dgItemType.SelectedItem as ItemTypeDTO;
                lblMessage.Visibility = Visibility.Collapsed;
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;

                SetItemTypeObject(lObjItemType);
            }
        }

        //private void cbCategory_DropDownClosed(object sender, EventArgs e)
        //{
        //    ItemType lObjCategory = cbCategory.SelectedItem as ItemType;
        //    if (lObjCategory != null)
        //    {
        //        List<ItemType> lObjItemTypes = mObjServiceFactory.GetItemTypeService().GetList().Where(x => x.Active == true && x.Level == 2 && x.Parent == lObjCategory.Id).ToList();
        //        cbParentSubCategory.ItemsSource = lObjItemTypes;
        //        cbParentSubCategory.DisplayMemberPath = "Name";
        //    }
        //}

        /// <summary>
        /// Selecion de combobox en cascada.
        /// </summary>
        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnumDTO lObjCategory = cbCategory.SelectedItem as EnumDTO;
            if (lObjCategory != null)
            {
                if (cbLevel.Text == "3")
                {
                    LoadComboBox(cbParentSubCategory, 2, (int)lObjCategory.Value);
                    cbParentSubCategory.IsEnabled = true;
                }
            }

        }

        private void cbLevel_DropDownClosed(object sender, EventArgs e)
        {
            LoadComboBox(cbCategory, 1);
            LoadComboBox(cbParentSubCategory, 2);

            switch (cbLevel.Text)
            {
                case "1":
                    cbCategory.IsEnabled = false;
                    cbParentSubCategory.IsEnabled = false;
                    break;
                case "2":
                    cbCategory.IsEnabled = true;
                    cbParentSubCategory.IsEnabled = false;
                    break;
                case "3":
                    cbCategory.IsEnabled = true;
                    cbParentSubCategory.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Realiza la busqueda en el datagrid.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((ItemTypeDTO)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()) || 
                                                                 ((ItemTypeDTO)o).Code.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgItemType.ItemsSource = mLcvListData;
            if (dgItemType.Items.Count == 0)
            {
                lblNotFound.Visibility = Visibility.Visible;
                dgItemType.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNotFound.Visibility = Visibility.Collapsed;
                dgItemType.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Realiza una busqueda en la base de datos al precionar la tecla enter.
        /// </summary>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!mBolReadOnly)
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        dgItemType.ItemsSource = null;
                        List<ItemType> lLstPartners = mObjServiceFactory.GetItemTypeService().GetList().Where(x => x.Active == true && x.Removed == false
                            && ((x.Name.Contains(txtSearch.Text)) || (x.Code.Contains(txtSearch.Text)))).ToList();
                        mLcvListData = new ListCollectionView(lLstPartners);
                        dgItemType.ItemsSource = mLcvListData;
                    }
                    else
                    {
                        LoadDataGrid();
                    }
                }
                else
                {
                    ReturnItem();
                }
            }
        }

        private void dgItemType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (mBolReadOnly)
            {
                if (e.Key == Key.Enter)
                {
                    ReturnItem();
                }
                if (char.IsLetterOrDigit((char)e.Key))
                {
                    txtSearch.Focus();
                }
            }
        }

        #endregion

        #region Methods

        private void LoadComboBoxes()
        {
            LoadComboBox(cbCategory, 1);
            LoadComboBox(cbParentSubCategory, 2);
            cbSellType.LoadDataSource<SellTypeEnum>();
            cbLevel.LoadDataSource<ItemTypeLevelEnum>();
            cbGender.LoadDataSource<ItemTypeGenderEnum>();

            cbParentSubCategory.IsEnabled = false;
            cbCategory.IsEnabled = false;
        }

        private void FormLoading(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdList.BlockUI();
                    txtSearch.IsEnabled = false;
                }
                else
                {
                    grdItemContent.BlockUI();
                }
            });
        }

        private void FormDefult(bool pBolForSave = false)
        {
            if (!pBolForSave)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    grdList.UnblockUI();
                    txtSearch.IsEnabled = true;
                });
            }
            else
            {
                grdItemContent.UnblockUI();
            }
        }

        /// <summary>
        /// Carga información en el combox.
        /// </summary>
        private void LoadComboBox(ComboBox pObjCombobox, int pIntLevel)
        {
            pObjCombobox.LoadDataSource(mObjServiceFactory.GetItemTypeService().GetList().Where(x => x.Active == true && x.Level == pIntLevel).ToList().ToEnumList());
        }

        private void LoadComboBox(ComboBox pObjCombobox, int pIntLevel, long pLonParentId)
        {
            pObjCombobox.LoadDataSource(mObjServiceFactory.GetItemTypeService().GetList().Where(x => x.Active == true && x.Level == pIntLevel && x.ParentId == pLonParentId).ToList().ToEnumList());
        }

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        private void LoadDataGrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgItemType.ItemsSource = null; });
                List<ItemTypeDTO> lLstObjItemTypes = mObjServiceFactory.GetItemTypeService().GetCustomList().OrderBy(a => a.Level).ThenBy(b => b.Name).ToList();

                //List<ItemType> lLstAuctionTypes = mObjServiceFactory.GetItemTypeService().GetList().Where(a => a.Active == true && a.Removed == false).OrderBy(b => b.Level).ThenBy(c => c.Name).ToList();

                //foreach (ItemType lObjItemType in lLstAuctionTypes)//Verificar eficiencia, utilizar join?
                //{
                //    List<ItemType> lObjItemParent = mObjServiceFactory.GetItemTypeService().GetList().Where(x => x.Id == lObjItemType.ParentId).ToList();
                //    if (lObjItemParent.Count > 0)
                //    {
                //        lObjItemType.ParentName = lObjItemParent[0];
                //    }
                //}

                mLcvListData = new ListCollectionView(lLstObjItemTypes);
                this.Dispatcher.Invoke(() => { dgItemType.ItemsSource = mLcvListData; });
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

        private void SaveOrUpdate()
        {
            FormLoading(true);
            try
            {
                ItemType lObjItemType = new ItemType();

                this.Dispatcher.Invoke(() =>
                {
                    lObjItemType.Name = txtName.Text;
                    lObjItemType.Code = txtCode.Text;
                    //lObjItemType.PerPrice = Convert.ToBoolean(chkPrice.IsChecked);
                    lObjItemType.SellType = (SellTypeEnum)cbSellType.SelectedValue;
                    lObjItemType.Level = (int)cbLevel.SelectedValue;
                    lObjItemType.Gender = (ItemTypeGenderEnum)cbGender.SelectedValue;

                    switch (Convert.ToInt32(cbLevel.Text))
                    {
                        case 1:
                            lObjItemType.ParentId = null;
                            break;
                        case 2:
                            lObjItemType.ParentId = (int)cbCategory.SelectedValue;
                            break;
                        case 3:
                            lObjItemType.ParentId = (int)cbParentSubCategory.SelectedValue;
                            break;
                    }

                    if (mLonId != 0)
                    {
                        lObjItemType.Id = mLonId;
                    }
                });

                mObjServiceFactory.GetItemTypeService().SaveOrUpdate(lObjItemType);

                FormDefult(true);

                this.Dispatcher.Invoke(() => ResetForm());
            }
            catch (Exception lObjException)
            {
                FormDefult(true);
                ShowMessage("Error", lObjException.Message);
            }
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        /// <summary>
        /// Valida los controles.
        /// </summary>
        private bool Valid()
        {
            bool lBolOK = true;
            if (string.IsNullOrEmpty(txtName.Text))
            {
                txtName.BorderBrush = Brushes.Red;
                lBolOK = false;
            }
            if (string.IsNullOrEmpty(txtCode.Text))
            {
                txtCode.BorderBrush = Brushes.Red;
                lBolOK = false;
            }
            if (string.IsNullOrEmpty(cbGender.Text))
            {
                cbGender.BorderBrush = Brushes.Red;
                lBolOK = false;
            }

            lblMessage.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(cbLevel.Text))
            {
                cbLevel.BorderBrush = Brushes.Red;
                lBolOK = false;
            }
            switch (Convert.ToInt16(cbLevel.Text))
            {
                case 1:
                    break;

                case 2:
                    if (cbCategory.SelectedValue == null)
                    {
                        cbCategory.BorderBrush = Brushes.Red;
                        lBolOK = false;
                    }
                    break;

                case 3:
                    if (cbParentSubCategory.SelectedValue == null)
                    {
                        cbParentSubCategory.BorderBrush = Brushes.Red;
                        cbCategory.BorderBrush = Brushes.Red;
                        lBolOK = false;
                    }
                    break;
            }

            return lBolOK;
        }

        private void ReturnItem()
        {
            Window lObjWindowParent = Window.GetWindow(this);
            lObjWindowParent.Close();
            WindowDialog lObjWindowDialog = lObjWindowParent as WindowDialog;

            ItemTypeDTO lobjItem = dgItemType.SelectedItem as ItemTypeDTO;
            lObjWindowDialog.gObject = lobjItem as object;
        }

        private void ResetForm()
        {
            grdItemType.ClearControl();
            //chkPrice.IsChecked = false;

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;

            LoadDataGrid();
            LoadComboBoxes();
            
        }

        private void SetItemTypeObject(ItemTypeDTO pObjItemType)
        {
            LoadComboBoxes();

            mLonId = pObjItemType.Id;
            //chkPrice.IsChecked = pObjItemType.PerPrice;
            cbSellType.SelectValue(pObjItemType.SellTypeid);
            txtCode.Text = pObjItemType.Code;
            txtName.Text = pObjItemType.Name;
            cbLevel.SelectValue(pObjItemType.Level);
            cbGender.SelectValue(pObjItemType.GenderId);

            switch (pObjItemType.Level)
            {
                case 2:
                    cbCategory.SelectValue((int)pObjItemType.CategoryId);
                    cbCategory.IsEnabled = true;
                    break;

                case 3:
                    cbCategory.SelectValue((int)pObjItemType.CategoryId);
                    cbCategory.IsEnabled = true;
                    cbParentSubCategory.SelectValue((int)pObjItemType.SubCategoryId);
                    cbParentSubCategory.IsEnabled = true;
                    break;
            }
        }

        #endregion
    }
}
