using QualisysConfig;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Configurations.Models;
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Extension;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCAuction.xaml
    /// </summary>
    public partial class UCAuction : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionServiceFactory;
        private ListCollectionView mLcvListData = null; //Filtros
        private long mLonId = 0; //bandera si se requiere modificar/eliminar
        private bool mBolReadOnly;
        private Auction mObjAuction;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCAuction(bool pBolReadOnly, List<Auction> lLstPartner, string pStrSearch)
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            mBolReadOnly = pBolReadOnly;
            mLcvListData = new ListCollectionView(lLstPartner);
            dgAuctions.ItemsSource = mLcvListData;

            if (pBolReadOnly)
            {
                BrdNewRegistry.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(pStrSearch))
                {
                    txtSearch.Focus();
                    txtSearch.Text = pStrSearch;
                }
                else
                {
                    dgAuctions.Focus();
                }
            }
        }

        public UCAuction(bool pBolReadOnly)
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            mBolReadOnly = pBolReadOnly;
            if (pBolReadOnly)
            {
                BrdNewRegistry.Visibility = Visibility.Collapsed;
                dgAuctions.Focus();
            }
        }

        public UCAuction()
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();
        }

        public UCAuction(bool pBolReadOnly, string pStrSearch, List<Auction> pLstAuction)
        {
            InitializeComponent();
            mObjAuctionServiceFactory = new AuctionsServicesFactory();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            mBolReadOnly = pBolReadOnly;
            if (pBolReadOnly)
            {
                BrdNewRegistry.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(pStrSearch))
                {
                    txtSearch.Text = pStrSearch;
                    txtSearch.Focus();
                }
                else
                {
                    dgAuctions.Focus();
                }
            }

            mLcvListData = new ListCollectionView(pLstAuction);
            dgAuctions.ItemsSource = null;
            dgAuctions.ItemsSource = mLcvListData;
        }

        #endregion

        #region Events

        /// <summary>
        /// Evento al terminar de cargar la pantalla.-
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!grdList.IsBlocked() && !mBolReadOnly)
            {
                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();
                LoadAuctionTypes();
                LoadAuctionCategories();
                txtFolio.Text = GetNextFolio().ToString();
                txtCommission.Text = ConfigurationUtility.GetValue<string>("Comission").ToString();
                dpDate.SelectedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Guardar datos.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!OpenedAuctions())
            {
                if (ValidateControls())
                {

                    mObjWorker = new Thread(() => SaveAuction());
                    mObjWorker.Start();
                }
                else
                {
                    FormDefault(true);
                    CustomMessageBox.Show("Subastas", "Favor de completar los campos.", this.GetParent());
                }
            }
            else
            {
                FormDefault(true);
                CustomMessageBox.Show("Subastas", "Ya se encuentra una subasta abierta", this.GetParent());
            }
        }

        private bool OpenedAuctions()
        {
            return mObjAuctionServiceFactory.GetAuctionService().IsActiveAuction();
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
                    mObjAuctionServiceFactory.GetAuctionService().Remove(mLonId);
                    ClearControls();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        /// <summary>
        /// Realiza una busqueda en el datagrid.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mLcvListData != null)
            {
                if (String.IsNullOrEmpty(txtSearch.Text))
                {
                    mLcvListData.Filter = null;
                }
                else
                {
                    mLcvListData.Filter = new Predicate<object>(o => ((Auction)o).Folio.ToUpper().Contains(txtSearch.Text.ToUpper()));
                }
                dgAuctions.ItemsSource = mLcvListData;

                if (mBolReadOnly)
                {
                    dgAuctions.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Carga los datos al dar doble clic en en el DataGrid.
        /// </summary>
        private void dgLocations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mBolReadOnly)
            {
                ReturnAuction();
            }
            else
            {

                Auction lObjAuction = dgAuctions.SelectedItem as Auction;
                mObjAuction = lObjAuction;
                LoadControls(lObjAuction);
                mLonId = lObjAuction.Id;
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                if(!mObjAuctionServiceFactory.GetAuctionService().IsActiveAuction() && !mObjAuction.ReOpened)
                {
                    btnReopen.Visibility = Visibility.Visible;
                }
                else
                {
                    btnReopen.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void dgAuctions_KeyDown(object sender, KeyEventArgs e)
        {
            if (mBolReadOnly)
            {
                if (e.Key == Key.Enter)
                {
                    ReturnAuction();
                }
                if (char.IsLetterOrDigit((char)e.Key))
                {
                    txtSearch.Focus();
                }
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
                        dgAuctions.ItemsSource = null;
                        List<Auction> lLstAuctions = mObjAuctionServiceFactory.GetAuctionService()
                                                    .SearchAuctions(txtSearch.Text, mBolReadOnly ? FilterEnum.OPENED : FilterEnum.NONE)
                                                    .ToList();

                        mLcvListData = new ListCollectionView(lLstAuctions);
                        dgAuctions.ItemsSource = mLcvListData;
                    }
                    else
                    {
                        LoadDatagrid();
                    }
                }
                else
                {
                    dgAuctions.SelectedIndex = 0;
                    ReturnAuction();
                }
            }
        }

        /// <summary>
        /// Textbox solo permite números.
        /// </summary>
        private void txtCommission_PreviewTextInput(object sender, TextCompositionEventArgs pObjArgs)
        {
            if (char.IsDigit(pObjArgs.Text, pObjArgs.Text.Length - 1) || (pObjArgs.Text == "." && !(sender as TextBox).Text.Contains('.')))
            {
                pObjArgs.Handled = false;
            }
            else
            {
                pObjArgs.Handled = true;
            }
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategory.SelectedIndex != -1)
            {
                if (cbCategory.SelectedIndex == 0)
                {
                    txtCommission.Text = ConfigurationUtility.GetValue<string>("Comission").ToString();
                    //txtCommission.IsEnabled = false;
                }
                else
                {
                    txtCommission.IsEnabled = true;
                }
                txtFolio.Text = GetNextFolio();
            }
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(dpDate.Text))
            {
                txtFolio.Text = GetNextFolio();
            }
        }

        private void btnReopen_Click(object sender, RoutedEventArgs e)
        {
            ReOpenAuction();
            LoadDatagrid();
        }

        private void ReOpenAuction()
        {
            if (!mObjAuctionServiceFactory.GetAuctionService().IsActiveAuction())
            {
                mObjAuction.Active = true;
                mObjAuction.Opened = true;
                mObjAuction.ReOpened = true;
                mObjAuction.ReOpenedTime = DateTime.Now;

                mObjAuctionServiceFactory.GetAuctionService().SaveOrUpdate(mObjAuction);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Guarda una subasta.
        /// </summary>
        private void SaveAuction()
        {
            FormLoading(true);
            try
            {
                Auction lObjAuction = new Auction();
                this.Dispatcher.Invoke(() =>
                {
                    lObjAuction.Folio = txtFolio.Text;
                    lObjAuction.Commission = float.Parse(txtCommission.Text);
                    lObjAuction.Date = Convert.ToDateTime(dpDate.Text);
                    lObjAuction.Type = GetSelectedType();
                    lObjAuction.Category = GetSelectedCategory();
                    lObjAuction.Location = GetSelectedLocation();
                    lObjAuction.CostingCode = GetCostingCode();
                    lObjAuction.Opened = true;

                    if (mLonId != 0)
                    {
                        lObjAuction.Id = mLonId;
                    }
                });

                mObjAuctionServiceFactory.GetAuctionService().SaveOrUpdate(lObjAuction);
                this.Dispatcher.Invoke(() => ClearControls());
            }
            catch (Exception lObjException)
            {
                FormDefault(true);
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefault(true);
            }
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        /// <summary>
        /// Carga información en el combox.
        /// </summary>
        private void LoadAuctionTypes()
        {
            cbType.ItemsSource = EnumExtension.GetEnumItemList<AuctionTypeEnum>();
            cbType.DisplayMemberPath = "Text";
            cbType.SelectedValuePath = "Value";
        }

        /// <summary>
        /// Carga información en el combox.
        /// </summary>
        private void LoadAuctionCategories()
        {
            cbCategory.ItemsSource = EnumExtension.GetEnumItemList<AuctionCategoryEnum>();
            cbCategory.DisplayMemberPath = "Text";
            cbCategory.SelectedValuePath = "Value";
        }

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = null; });
                List<Auction> lLstAuctions = mObjAuctionServiceFactory.GetAuctionService().SearchAuctions(string.Empty, FilterEnum.NONE);
                mLcvListData = new ListCollectionView(lLstAuctions);

                this.Dispatcher.Invoke(() =>
                {
                    if (!mBolReadOnly)
                    {
                        dgAuctions.ItemsSource = null;
                        dgAuctions.ItemsSource = mLcvListData;
                    }
                });
            }
            catch (Exception lObjException)
            {
                FormDefault();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefault();
            }
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
                    grdSaveControls.BlockUI();
                }
            });
        }

        private void FormDefault(bool pBolForSave = false)
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
                    grdSaveControls.UnblockUI();
                }
            });
        }

        /// <summary>
        /// Obtiene el siguente folio de subastas.
        /// </summary>
        private string GetNextFolio()
        {
            try
            {
               return string.Format("{0}-{1}-{2}", GetCategoryAbreviation(GetSelectedCategory()), GetLocationAbreviation(GetSelectedLocation()), GetNextAuction());
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
                return "";
            }
        }
        
        /// <summary>
        ///Relaiza la busqueda de lotes
        /// </summary>
        private List<Batch> SearchBatches()
        {
            List<Batch> lLstAuct = mObjAuctionServiceFactory.GetBatchService().GetList().Where(x => x.Active == true && x.Removed == false && x.AuctionId == mLonId).ToList();
            return lLstAuct;
        }

        /// <summary>
        ///Relaiza la busqueda en la base de datos
        /// </summary>
        private List<Auction> SearchAuctions()
        {
            List<Auction> lLstAuction = mObjAuctionServiceFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Active == true && x.Removed == false
                             && x.Id == mLonId).ToList();
            return lLstAuction;
        }

        /// <summary>
        ///Relaiza la busqueda en la base de datos
        /// </summary>
        private List<Auction> SearchAuctions(string pStrFolio)
        {
            List<Auction> lLstAuction = mObjAuctionServiceFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Active == true && x.Removed == false
                             && x.Folio == pStrFolio).ToList();
            return lLstAuction;
        }

        /// <summary>
        /// Coloca los controles a su estado original.
        /// </summary>
        private void ClearControls()
        {
            mLonId = 0;
            txtCommission.Text = ConfigurationUtility.GetValue<string>("Comission").ToString();
            dpDate.SelectedDate = DateTime.Now;
            cbType.SelectedValue = null;
            cbCategory.SelectedValue = null;
            cbType.Text = "Seleccionar";
            cbCategory.Text = "Seleccionar";

            List<Control> lLstControls = new List<Control>();
            lLstControls.Add(txtCommission);
            lLstControls.Add(dpDate);
            lLstControls.Add(cbType);
            lLstControls.Add(cbCategory);
            this.ModifyControlColorBlack(lLstControls);

            lblMessage.Visibility = Visibility.Collapsed;
            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
            btnReopen.Visibility = Visibility.Collapsed;

            txtFolio.Text = GetNextFolio();
            LoadDatagrid();
        }

        /// <summary>
        /// Carga los datos seleccionados en los controles.
        /// </summary>
        private void LoadControls(Auction pObjAuction)
        {
            txtCommission.Text = pObjAuction.Commission.ToString();
            dpDate.Text = pObjAuction.Date.ToShortDateString();
            cbType.SelectedValue = (int)pObjAuction.Type;
            cbCategory.SelectedValue = (int)pObjAuction.Category;
            txtFolio.Text = pObjAuction.Folio;
        }

        /// <summary>
        /// Valida los controles.
        /// </summary>
        private bool ValidateControls()
        {
            List<Control> lLstControls = new List<Control>();
            bool lBoolOk = true;

            if (string.IsNullOrEmpty(txtCommission.Text))
            {
                lLstControls.Add(txtCommission);
                lBoolOk = false;
            }
            else
            {
                if (Convert.ToDouble(txtCommission.Text) <= 0)
                {
                    lLstControls.Add(cbType);
                    lBoolOk = false;
                }
            }

            if (string.IsNullOrEmpty(txtFolio.Text))
            {
                lLstControls.Add(txtFolio);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(cbCategory.Text))
            {
                lLstControls.Add(cbCategory);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(cbType.Text))
            {
                lLstControls.Add(cbType);
                lBoolOk = false;
            }

            this.ModifyControlColorRed(lLstControls);
            return lBoolOk;
        }

        private void ReturnAuction()
        {
            try
            {
                Auction lobjAuction = dgAuctions.SelectedItem as Auction;
                Window lObjWindowParent = Window.GetWindow(this);
                Window x = this.Parent as Window;
                WindowDialog lObjWindowDialog = lObjWindowParent as WindowDialog;
                lObjWindowDialog.gObject = lobjAuction as object;
                lObjWindowParent.Close();
            }
            catch
            {

            }
        }

        private string GetNextAuction()
        {
            AuctionCategoryEnum lEnmCategory = GetSelectedCategory();
            DateTime lDtmAutionDate = !string.IsNullOrEmpty(dpDate.Text) ? Convert.ToDateTime(dpDate.Text) : DateTime.Now;
            DateTime lDtmStartDate = new DateTime(lDtmAutionDate.Year, 1, 1);
            DateTime lDtmEndDate = new DateTime(lDtmAutionDate.Year, 12, 31);

            //List<Auction> lLstAuction = mObjAuctionServiceFactory.GetAuctionService().GetList().ToList();

            int lIntAuctionsCount = 0;
            string lStrCurrentFolio = string.Empty;

            if (mLonId > 0)
            {
                lStrCurrentFolio = mObjAuctionServiceFactory
                                   .GetAuctionService()
                                   .GetListFilteredByCC()
                                   .Where(x => x.Id == mLonId).Select(y => y.Folio)
                                   .FirstOrDefault();

                lStrCurrentFolio = lStrCurrentFolio.Substring(lStrCurrentFolio.Length - 4, 4);
                lIntAuctionsCount = Convert.ToInt32(lStrCurrentFolio) -1;
            }
            else
            {
                lIntAuctionsCount = mObjAuctionServiceFactory
                                    .GetAuctionService()
                                    .GetListFilteredByCC().Where(x => x.Date >= lDtmStartDate && x.Date <= lDtmEndDate && x.Category == lEnmCategory)
                                    .Count();
            }

            return string.Concat(lDtmAutionDate.ToString("yy"), (lIntAuctionsCount + 1).ToString("0000"));
        }

        private AuctionTypeEnum GetSelectedType()
        {
            return cbType.SelectedValue != null ? cbType.SelectedValue.GetValue<AuctionTypeEnum>() : 0;
        }

        private AuctionCategoryEnum GetSelectedCategory()
        {
            return cbCategory.SelectedValue != null ? cbCategory.SelectedValue.GetValue<AuctionCategoryEnum>() : 0;
        }

        private string GetCostingCode()
        {
            return QsConfig.GetValue<string>("CostCenter");
        }

        private LocationEnum GetSelectedLocation()
        {
            return ConfigurationUtility.GetValue<LocationEnum>("Location");
        }

        private string GetCategoryAbreviation(AuctionCategoryEnum pEnmCategory)
        {
            return GetCategoriesList().Where(x => x.Category == pEnmCategory).Select(x => x.Abbreviation).FirstOrDefault() ?? "";
        }

        private string GetLocationAbreviation(LocationEnum pEnmLocation)
        {
            return GetLocationsList().Where(x => x.Location == pEnmLocation).Select(x => x.Abbreviation).FirstOrDefault() ?? "";
        }

        private List<CategoryModel> GetCategoriesList()
        {
            return ConfigurationManager.GetSection("categories") as List<CategoryModel>;
        }

        private List<LocationModel> GetLocationsList()
        {
            return ConfigurationManager.GetSection("locations") as List<LocationModel>;
        }
          
        #endregion


    }
}
