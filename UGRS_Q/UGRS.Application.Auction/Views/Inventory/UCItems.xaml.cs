using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using System.Threading;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCItems.xaml
    /// </summary>
    public partial class UCItems : UserControl
    {

        InventoryServicesFactory mObjInventoryFactory = new InventoryServicesFactory();
        ListCollectionView mLcvListData = null; //Filtros
        long mLonId = 0;//bandera si se requiere modificar/eliminar
        bool mBolReadOnly;
        private Thread mObjWorker;

        public UCItems()
        {
            InitializeComponent();
        }

        public UCItems(bool pBolReadOnly)
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            mBolReadOnly = pBolReadOnly;
            if (pBolReadOnly)
            {
                brdNewRegistry.Visibility = Visibility.Collapsed;
                dgItems.Focus();
            }
        }

        public UCItems(bool pBolReadOnly, List<Item> lLstItem, string pStrSearch)
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
            mBolReadOnly = pBolReadOnly;

            mLcvListData = new ListCollectionView(lLstItem);
            dgItems.ItemsSource = mLcvListData;

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
                    dgItems.Focus();
                }
            }
        }

        #region Controls

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateControls())
            {
                mObjWorker = new Thread(() => SaveItem());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Artículos", "Favor de completar los campos.", this.GetParent());
            }
        }

        /// <summary>
        /// Inicializa controles.
        /// </summary>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        /// <summary>
        /// Elimina un registro.
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
                {
                    mObjInventoryFactory.GetItemService().Remove(mLonId);
                    loadDatagrid();
                    ClearControls();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mBolReadOnly)
            {
                ReturnItem();
            }
            else
            {
                Item lObjItem = dgItems.SelectedItem as Item;
                loadControls(lObjItem);
                mLonId = lObjItem.Id;
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Realiza una busqueda en el datagrid.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((Item)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()) || ((Item)o).Code.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgItems.ItemsSource = mLcvListData;
            if (mBolReadOnly)
            {
                dgItems.SelectedIndex = 0;
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
                        dgItems.ItemsSource = null;
                        List<Item> lLstPartners = mObjInventoryFactory.GetItemService().GetList().Where(x => x.Active == true && x.Removed == false
                            && ((x.Name.Contains(txtSearch.Text)) || (x.Code.Contains(txtSearch.Text)))).ToList();
                        mLcvListData = new ListCollectionView(lLstPartners);
                        dgItems.ItemsSource = mLcvListData;
                    }
                    else
                    {
                        loadDatagrid();
                    }
                }
                else
                {
                    dgItems.SelectedIndex = 0;
                    ReturnItem();
                }
            }
        }

        /// <summary>
        /// Evento al terminar de cargar la pantalla.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadCombobox();
            mObjWorker = new Thread(() => loadDatagrid());
            mObjWorker.Start();
        }
        #endregion

        /// <summary>
        /// Carga información en el combox.
        /// </summary>
        void loadCombobox()
        {
            //cbItemStatus.ItemsSource = Enum.GetValues(typeof(ItemStatusEnum)).Cast<ItemStatusEnum>();
        }

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        void loadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgItems.ItemsSource = null; });
                List<Item> lLstPartners = mObjInventoryFactory.GetItemService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lLstPartners);
                this.Dispatcher.Invoke(() => { dgItems.ItemsSource = mLcvListData; });
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

        private void SaveItem()
        {
            FormLoading(true);
            try
            {
                Item lObjItem = new Item();

                this.Dispatcher.Invoke(() =>
                {
                    lObjItem.Name = txtName.Text;
                    lObjItem.Code = txtCode.Text;

                    if (mLonId != 0)
                    {
                        lObjItem.Id = mLonId;
                    }
                });

                mObjInventoryFactory.GetItemService().SaveOrUpdate(lObjItem);

                this.Dispatcher.Invoke(() => ClearControls());
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
                    grdList.BlockUI();
                    txtSearch.IsEnabled = false;
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
                    grdList.UnblockUI();
                    txtSearch.IsEnabled = true;
                }
                else
                {
                    grdItems.UnblockUI();
                }
            });
        }

        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        private void ClearControls()
        {
            txtCode.Text = string.Empty;
            txtName.Text = string.Empty;

            List<Control> lLstControls = new List<Control>();
            lLstControls.Add(txtCode);
            lLstControls.Add(txtName);
            //lLstControls.Add(cbItemStatus);
            ModifycontrolcolorBlack(lLstControls);

            lblMessage.Visibility = Visibility.Collapsed;
            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;

            loadCombobox();
            //cbItemStatus.SelectedItem = null;
            loadDatagrid();
        }


        /// <summary>
        /// Carga los datos seleccionados en los controles.
        /// </summary>
        void loadControls(Item pObjItem)
        {
            txtCode.Text = pObjItem.Code;
            txtName.Text = pObjItem.Name;

            // cbItemStatus.SelectedItem = pObjItem.ItemStatus;

        }

        /// <summary>
        /// Valida los controles.
        /// </summary>
        private bool validateControls()
        {
            List<Control> lLstControls = new List<Control>();
            bool lBoolOk = true;

            if (string.IsNullOrEmpty(txtName.Text))
            {
                lLstControls.Add(txtName);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtCode.Text))
            {
                lLstControls.Add(txtCode);
                lBoolOk = false;
            }

            //if (string.IsNullOrEmpty(cbItemStatus.Text))
            //{
            //    lLstControls.Add(cbItemStatus);
            //    lBoolOk = false;
            //}

            //lblMessage.Visibility = Visibility.Visible;
            ModifycontrolcolorRed(lLstControls);
            return lBoolOk;
        }

        /// <summary>
        /// Cambia el color a rojo los controles enviados en la lista.
        /// </summary>
        private void ModifycontrolcolorRed(List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        /// Cambia el color a negro los controles enviados en la lista.
        /// </summary>
        private void ModifycontrolcolorBlack(List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Black;
            }
        }

        private void dgItems_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void ReturnItem()
        {
            Window lObjWindowParent = Window.GetWindow(this);
            lObjWindowParent.Close();
            WindowDialog lObjWindowDialog = lObjWindowParent as WindowDialog;

            Item lobjItem = dgItems.SelectedItem as Item;
            lObjWindowDialog.gObject = lobjItem as object;
        }

    }
}
