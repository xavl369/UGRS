using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCGoodsReceipt.xaml
    /// </summary>
    public partial class UCGoodsReceipt : UserControl
    {
        #region Attributes

        private InventoryServicesFactory mObjInventoryServicesFactory;
        private BusinessServicesFactory mObjBusinessServicesFactory;
        private AuctionsServicesFactory mObjAuctionServicesFactory = new AuctionsServicesFactory();
        private ListCollectionView mLcvListData = null; //Filtros
        private GoodsReceipt mObjGoodsReceipt;
        private Auction mObjAuction;
        private Item mObjItem;
        private Partner mObjPartner;
        private long mLonId = 0; //bandera si se requiere modificar/eliminar
        private Thread mObjWorker;

        #endregion

        #region Contructor

        public UCGoodsReceipt()
        {
            InitializeComponent();
            mObjInventoryServicesFactory = new InventoryServicesFactory();
            mObjBusinessServicesFactory = new BusinessServicesFactory();
            mObjAuction = mObjAuctionServicesFactory.GetAuctionService().GetActiveAuction();
        }

        #endregion

        #region Events

        #region Usercontrol

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdList.IsBlocked())
            {
                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();
                dpDate.SelectedDate = mObjAuction != null ? mObjAuction.Date : DateTime.Now;
            }
        }

        #endregion

        #region TextBox

        /// <summary>
        /// Realiza la busqueda o carga la ventana de dialogo
        /// </summary>
        private void txtPartnerId_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            Partner lObjPartner = new Partner();
            if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                string lStrText = (pObjSender as TextBox).Text;
                List<Partner> lLstObjPartners = mObjBusinessServicesFactory.GetPartnerService().SearchPartner(lStrText, FilterEnum.ACTIVE);

                if (lLstObjPartners.Count == 1)
                {
                    lObjPartner = lLstObjPartners[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjPartners, FilterEnum.ACTIVE);
                    lObjPartner = FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner;
                    (pObjSender as TextBox).Focusable = true;
                }
                (pObjSender as TextBox).Focus();
                LoadPartnerObject(lObjPartner);
            }

        }

        /// <summary>
        /// Realiza la busqueda o carga la ventana de dialogo
        /// </summary>
        private void txtItemId_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            Item lObjItem = new Item();
            if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                string lStrText = (pObjSender as TextBox).Text;
                List<Item> lLstObjItems = mObjInventoryServicesFactory.GetItemService().SearchItem(lStrText, FilterEnum.ACTIVE);

                if (lLstObjItems.Count == 1)
                {
                    lObjItem = lLstObjItems[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCItems = new UCSearchItem(lStrText, lLstObjItems, FilterEnum.ACTIVE);
                    lObjItem = FunctionsUI.ShowWindowDialog(lUCItems, Window.GetWindow(this)) as Item;
                    (pObjSender as TextBox).Focusable = true;
                }
                (pObjSender as TextBox).Focus();
                LoadItemObject(lObjItem);
            }

        }

        private void txtQuantity_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {

        }

        /// <summary>
        /// Realiza una busqueda en el datagrid.
        /// </summary>
        private void TextBox_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                // mLcvListData.Filter = new Predicate<object>(o => ((GoodsReceipt)o).Folio.ToUpper().Contains(txtSearch.Text.ToUpper()));
                mLcvListData.Filter = new Predicate<object>(o => (((GoodsReceipt)o).Folio != null) ? (((GoodsReceipt)o).Folio.ToUpper().Contains(txtSearch.Text.ToUpper())) : false
                    || ((GoodsReceipt)o).Customer.Name.ToUpper().Contains(txtSearch.Text.ToUpper())
                    || ((GoodsReceipt)o).Item.Name.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgGoodsReceipt.ItemsSource = mLcvListData;
        }

        /// <summary>
        /// Realiza una busqueda en la base de datos al precionar la tecla enter.
        /// </summary>
        private void txtSearch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgGoodsReceipt.ItemsSource = null;
                    List<GoodsReceipt> lLstPartners = mObjInventoryServicesFactory.GetGoodsReceiptService().GetList().Where(x => x.Active == true && x.Removed == false
                    && ((x.Folio.Contains(txtSearch.Text)) || (x.Customer.Name.Contains(txtSearch.Text)) || (x.Item.Name.Contains(txtSearch.Text)))).ToList();
                    mLcvListData = new ListCollectionView(lLstPartners);
                    dgGoodsReceipt.ItemsSource = mLcvListData;
                }
                else
                {
                    LoadDatagrid();
                }
            }
        }

        private void txtQuantity_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
        {
            if (char.IsDigit(pObjArgs.Text, pObjArgs.Text.Length - 1))
            {
                pObjArgs.Handled = false;
            }
            else
            {
                pObjArgs.Handled = true;
            }
        }

        #endregion

        #region DataGrid

        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgGoodsReceipts_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            var lVarGoodsReceipt = dgGoodsReceipt.SelectedItem as GoodsRecieptToPickDTO;

            mObjGoodsReceipt = mObjInventoryServicesFactory.GetGoodsReceiptService().GetList().Where(x => x.Id == lVarGoodsReceipt.Id).FirstOrDefault();

            LoadGoodsReceiptObject(mObjGoodsReceipt);
            mLonId = mObjGoodsReceipt.Id;

            btnSave.Content = "Modificar";
            lbltitle.Content = "Modificar registro";

            btnNew.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
        }

        #endregion

        #region Button

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (grReceipt.Valid() && ((mObjPartner != null && mObjItem != null) || mObjGoodsReceipt != null))
            {
                mObjWorker = new Thread(() => SaveGoodsReceipt());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Error", "Favor de completar los campos.", this.GetParent());
            }
        }

        /// <summary>
        /// Inicializa controles.
        /// </summary>
        private void btnNew_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            grReceipt.ClearControl();

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";

            tbCustomer.Text = string.Empty;
            tbItem.Text = string.Empty;

            mLonId = 0;

            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
            lblMessage.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Elimina un registro.
        /// </summary>
        private void btnDelete_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
                {
                    mObjInventoryServicesFactory.GetGoodsReceiptService().Remove(mLonId);
                    ResetForm();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgGoodsReceipt.ItemsSource = null; });
                List<GoodsRecieptToPickDTO> lLstGoodsReceipts = new List<GoodsRecieptToPickDTO>();
                if (mObjAuction != null)
                {
                    lLstGoodsReceipts = mObjInventoryServicesFactory.GetGoodsReceiptService().GetList().Where(x => x.Active == true
                        && x.Removed == false
                        && (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(mObjAuction.Date))).ToList()
                        .Select(x => new GoodsRecieptToPickDTO()
                        {
                            Customer = x.Customer.Name,
                            Code = x.Customer.Code,
                            Folio = x.Folio,
                            Item = x.Item.Name,
                            ItemId = x.ItemId,
                            Quantity = x.Quantity,
                            Related = x.Processed ? "Si" : "No",
                            Id = x.Id
                        }
                        ).ToList();
                }
                mLcvListData = new ListCollectionView(lLstGoodsReceipts);
                this.Dispatcher.Invoke(() => { dgGoodsReceipt.ItemsSource = mLcvListData; });
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

        private void SaveGoodsReceipt()
        {
            FormLoading(true);
            try
            {
                GoodsReceipt lObjGoodsReceipt = new GoodsReceipt();

                this.Dispatcher.Invoke(() =>
                {
                    lObjGoodsReceipt.Folio = txtFolio.Text;
                    lObjGoodsReceipt.Quantity = Convert.ToInt32(txtQuantity.Text); //verifcar solo numeros
                    lObjGoodsReceipt.Exported = false;
                    lObjGoodsReceipt.CustomerId = mObjPartner != null ? mObjPartner.Id : mObjGoodsReceipt.CustomerId;
                    lObjGoodsReceipt.BatchNumber = mObjPartner != null ? string.Format("{0}{1}", mObjPartner.ForeignName, DateTime.Now.ToString("yyMMddHHmmss")) : mObjGoodsReceipt.BatchNumber;
                    lObjGoodsReceipt.BatchDate = mLonId == 0 ? DateTime.Now : mObjGoodsReceipt.BatchDate;
                    lObjGoodsReceipt.ExpirationDate = Convert.ToDateTime(dpDate.Text);
                    lObjGoodsReceipt.ItemId = mObjItem != null ? mObjItem.Id : mObjGoodsReceipt.ItemId;

                    if (mLonId != 0)
                    {
                        lObjGoodsReceipt.Id = mLonId;
                    }
                });

                mObjInventoryServicesFactory.GetGoodsReceiptService().SaveOrUpdate(lObjGoodsReceipt);
                FormDefult(true);

                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();

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
                    grdGoodsReceipt.BlockUI();
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
                    grdGoodsReceipt.UnblockUI();
                }
            });
        }

        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        private void ResetForm()
        {
            grReceipt.ClearControl();

            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";

            dpDate.SelectedDate = mObjAuction != null ? mObjAuction.Date : DateTime.Now;
            tbCustomer.Text = string.Empty;
            tbItem.Text = string.Empty;

            mLonId = 0;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
            lblMessage.Visibility = Visibility.Collapsed;

            mObjGoodsReceipt = null;
            mObjItem = null;
            mObjPartner = null;
        }

        /// <summary>
        /// Carga los datos seleccionados en los controles.
        /// </summary>
        private void LoadGoodsReceiptObject(GoodsReceipt pObjReceipt)
        {
            txtFolio.Text = pObjReceipt.Folio;
            txtQuantity.Text = pObjReceipt.Quantity.ToString();
            txtItemId.Text = pObjReceipt.ItemId.ToString();
            txtCustomerId.Text = pObjReceipt.CustomerId.ToString();
            tbItem.Text = pObjReceipt.Item.Name;
            tbCustomer.Text = pObjReceipt.Customer.Name;
            dpDate.Text = pObjReceipt.ExpirationDate.ToShortDateString();
        }

        /// <summary>
        /// Coloca los valores en los controles
        /// </summary>
        private void LoadItemObject(Item pObjItem)
        {
            if (pObjItem != null)
            {
                txtItemId.Text = pObjItem.Code;
                tbItem.Text = pObjItem.Name;
                mObjItem = pObjItem;
            }
            else
            {
                txtItemId.Text = string.Empty;
                tbItem.Text = string.Empty;
                mObjItem = null;
            }
        }

        private void LoadPartnerObject(Partner pObjPartner)
        {
            if (pObjPartner != null)
            {
                txtCustomerId.Text = pObjPartner.Code;
                tbCustomer.Text = pObjPartner.Name;
                mObjPartner = pObjPartner;
            }
            else
            {
                txtCustomerId.Text = string.Empty;
                tbCustomer.Text = string.Empty;
                mObjPartner = null;
            }
        }

        #endregion
    }
}
