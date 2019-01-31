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
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCTrade.xaml
    /// </summary>
    public partial class UCTrades : UserControl
    {
        #region Attributes 
        private long mLonId = 0;//bandera si se requiere modificar/eliminar 
        private ListCollectionView mLcvListData = null; //Filtros 
        private string mStrCodeSeller;
        private int mIntNumber;

        BusinessServicesFactory mObjBusinessServicesFactory;
        AuctionsServicesFactory mObjAuctionsServicesFactory;
     
        // TradesServicesFactory
        Auction mObjAuction;
      
     
        Thread mObjWorker;
        Partner mObjSeller;
        Partner mObjBuyer;
        #endregion

        #region Constructor

        public UCTrades()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Evento al terminar de cargar la pantalla.-
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            //mObjWorker = new Thread(() => LoadDatagrid());
            //mObjWorker.Start();

            mObjBusinessServicesFactory = new BusinessServicesFactory();
            mObjAuctionsServicesFactory = new AuctionsServicesFactory();
            LoadDefaultAuction();
        }

        private void txtAuction_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    InternalAuctionSearch((pObjSender as TextBox));
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtCustomerBuyerId_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Partner> lLstObjPartners = mObjBusinessServicesFactory.GetPartnerService().SearchPartner(lStrText, FilterEnum.ACTIVE).Where(x => x.Code != mStrCodeSeller).ToList();

                    if (lLstObjPartners.Count == 1)
                    {
                        (pObjSender as TextBox).Focus();
                        SetBuyerObject(lLstObjPartners[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjPartners, FilterEnum.ACTIVE);
                        SetBuyerObject(FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner);
                        (pObjSender as TextBox).Focusable = true;
                    }

                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }

        }

        private void txtPartnerSellerId_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Partner> lLstObjPartners = mObjBusinessServicesFactory.GetPartnerService().SearchPartner(lStrText, FilterEnum.ACTIVE);

                    if (lLstObjPartners.Count == 1)
                    {
                        (pObjSender as TextBox).Focus();
                        SetSellerObject(lLstObjPartners[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjPartners, FilterEnum.ACTIVE);
                        SetSellerObject(FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner);

                        (pObjSender as TextBox).Focusable = true;
                    }

                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtAmount_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
        {
            if (char.IsDigit(pObjArgs.Text, pObjArgs.Text.Length - 1) || (pObjArgs.Text == "." && !(pObjSender as TextBox).Text.Contains('.')))
            {
                pObjArgs.Handled = false;
            }
            else
            {
                pObjArgs.Handled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateControls())
            {
                if (txtCustomerBuyerId.Text != txtCustomerSellerId.Text)
                {
                    mObjWorker = new Thread(() => SaveTrade());
                    mObjWorker.Start();
                }
                else
                {
                    CustomMessageBox.Show("Trato directo", "El comprador y vendedor deben de ser distintos", this.GetParent());
                }
            }
            else
            {
                FormDefult(true);
                CustomMessageBox.Show("Trato directo", "Favor de completar los campos.", this.GetParent());
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
                {
                    mObjAuctionsServicesFactory.GetTradeService().Remove(mLonId);
                    ClearControls();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void dgTrades_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Trade lObjTrade = dgTrades.SelectedItem as Trade;
            LoadControls(lObjTrade);
            mLonId = lObjTrade.Id;
            mIntNumber = lObjTrade.Number;
            btnSave.Content = "Modificar";
            lblTitle.Content = "Modificar registro";
            btnNew.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            mObjBuyer = lObjTrade.Buyer;
            mObjSeller = lObjTrade.Seller;
            mObjAuction = lObjTrade.Auction;
            txtWeight.Text = lObjTrade.Weight.ToString();
            txtAmount.Text = lObjTrade.Amount.ToString();
        }

        #endregion

        #region Methods

        #region Form
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
                    //grdList.BlockUI();
                    //txtSearch.IsEnabled = false;
                }
                else
                {
                    grdSaveControls.BlockUI();
                }
            });
        }

        private void FormDefult(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    //grdList.UnblockUI();
                    //txtSearch.IsEnabled = true;
                }
                else
                {
                    grdSaveControls.UnblockUI();
                }
            });
        }

        private void ClearControls()
        {
            mLonId = 0;
            mIntNumber = 0;
            txtAmount.Text = string.Empty;
            SetBuyerObject(null);
            SetSellerObject(null);
            btnSave.Content = "Guardar";
            lblTitle.Content = "Nuevo registro";
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
            txtWeight.Text = string.Empty;
            LoadDatagrid();

            List<Control> lLstControls = new List<Control>();
            lLstControls.Add(txtAmount);
            lLstControls.Add(txtCustomerBuyerId);
            lLstControls.Add(txtCustomerSellerId);
            lLstControls.Add(txtAuction);
            ModifycontrolcolorBlack(lLstControls);
        }
        
        private void LoadControls(Trade pObjTrade)
        {
            txtAuction.Text = pObjTrade.Auction.Folio;
            txtCustomerBuyerId.Text = pObjTrade.Buyer.Code;
            tbCustomerBuyer.Text = pObjTrade.Buyer.Name;
            txtCustomerSellerId.Text = pObjTrade.Seller.Code;
            tbCustomerSeller.Text = pObjTrade.Seller.Name;
            txtAmount.Text = pObjTrade.Amount.ToString();
        }

        #endregion

        #region Auction
        private void LoadDefaultAuction()
        {
            try
            {
                string lStrLastAuction = mObjAuctionsServicesFactory.GetAuctionService()
                                            .GetListFilteredByCC()
                                            .Where(x => x.Active 
                                                && x.Opened 
                                                && (x.Category == AuctionCategoryEnum.DIRECT_TRADE))
                                            .Count() > 0 ?
                                           mObjAuctionsServicesFactory.GetAuctionService()
                                           .GetListFilteredByCC()
                                           .Where(x => x.Active 
                                               && x.Opened 
                                               && (x.Category == AuctionCategoryEnum.DIRECT_TRADE))
                                           .OrderByDescending(y => y.Date)
                                           .Select(z => z.Folio)
                                           .FirstOrDefault() : string.Empty;

                this.Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(lStrLastAuction))
                    {
                        txtAuction.Text = lStrLastAuction;
                        InternalAuctionSearch(txtAuction);
                    }
                });
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
                });
            }
        }

        private void InternalAuctionSearch(TextBox pObjTextBox)
        {
            mObjAuction = new Auction();
            List<Auction> lLstObjAuctions = mObjAuctionsServicesFactory.GetAuctionService().SearchAuctions(pObjTextBox.Text, FilterEnum.OPENED).Where(x => x.Category == AuctionCategoryEnum.DIRECT_TRADE).ToList();
            if (lLstObjAuctions.Count == 1)
            {
                // SetAuctionObject(lLstObjAuctions[0]);
                mObjAuction = lLstObjAuctions[0];
                txtAuction.Text = lLstObjAuctions[0].Folio;
            }
            else
            {
                pObjTextBox.Focusable = false;
                UserControl lUCAuction = new UCSearchAuction(pObjTextBox.Text, lLstObjAuctions, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION);
                mObjAuction = (FunctionsUI.ShowWindowDialog(lUCAuction, this.GetParent()) as Auction);
                txtAuction.Text = mObjAuction == null ? "" : mObjAuction.Folio;
                pObjTextBox.Focusable = true;
            }
            if (mObjAuction != null)
            {
                LoadDatagrid();
                if (mObjAuction.Category == AuctionCategoryEnum.DIRECT_TRADE)
                {
                    txtWeight.Visibility = Visibility.Visible;
                    lblWeight.Visibility = Visibility.Visible;
                    dgTrades.Columns.Where(x => x.Header.Equals("Peso")).FirstOrDefault().Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtWeight.Visibility = Visibility.Collapsed;
                    lblWeight.Visibility = Visibility.Collapsed;
                    dgTrades.Columns.Where(x => x.Header.Equals("Peso")).FirstOrDefault().Visibility = Visibility.Visible;
                }
            }
        }
        
        #endregion

        #region Seller
        private void SetSellerObject(Partner pObjSeller)
        {
            try
            {
                mObjSeller = pObjSeller;
                // OnLoadSeller(pObjSeller);

                if (pObjSeller != null)
                {
                    txtCustomerSellerId.Text = pObjSeller.Code;
                    tbCustomerSeller.Text = pObjSeller.Name;
                    mStrCodeSeller = pObjSeller.Code;
                    txtCustomerBuyerId.Focus();
                }
                else
                {
                    ResetSeller();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetSeller()
        {
            txtCustomerSellerId.Text = string.Empty;
            tbCustomerSeller.Text = string.Empty;
        }
        #endregion

        #region Buyer
        private void SetBuyerObject(Partner pObjBuyer)
        {
            try
            {
                mObjBuyer = pObjBuyer;
                // OnLoadSeller(pObjSeller);

                if (pObjBuyer != null)
                {
                    txtCustomerBuyerId.Text = pObjBuyer.Code;
                    tbCustomerBuyer.Text = pObjBuyer.Name;
                    txtAmount.Focus();
                }
                else
                {
                    ResetBuyer();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetBuyer()
        {
            txtCustomerBuyerId.Text = string.Empty;
            tbCustomerBuyer.Text = string.Empty;
        }


        #endregion

        #region Trade
        private void SaveTrade()
        {
            FormLoading(true);
            try
            {
                Trade lObjTrade = new Trade();
                this.Dispatcher.Invoke(() =>
                {
                    lObjTrade.Number = mIntNumber > 0 ? mIntNumber : GetNextTradeNumber(); ;
                    lObjTrade.BuyerId = mObjBuyer.Id;
                    lObjTrade.SellerId = mObjSeller.Id;
                    lObjTrade.Amount = Convert.ToDecimal(txtAmount.Text);
                    lObjTrade.AuctionId = mObjAuction.Id;
                    lObjTrade.Weight = Convert.ToDouble(txtWeight.Text);

                    if (mLonId != 0)
                    {
                        lObjTrade.Id = mLonId;
                    }
                });

                mObjAuctionsServicesFactory.GetTradeService().SaveOrUpdateTrade(lObjTrade);
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
        
        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        private void LoadDatagrid()
        {
            FormLoading();

            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    dgTrades.ItemsSource = null;
                    List<Trade> lLstTrade = mObjAuctionsServicesFactory.GetTradeService().GetList().Where(x => !x.Removed && x.AuctionId == mObjAuction.Id).ToList();   
                    mLcvListData = new ListCollectionView(lLstTrade);
                });
                this.Dispatcher.Invoke(() =>
                {
                    dgTrades.ItemsSource = null;
                    dgTrades.ItemsSource = mLcvListData;
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


        private int GetNextTradeNumber()
        {
            if (txtAuction.ValidRequired() && mObjAuction != null)
            {
                return mObjAuctionsServicesFactory.GetTradeService().GetList().Where(x => x.AuctionId == mObjAuction.Id).Count() > 0 ?
                       mObjAuctionsServicesFactory.GetTradeService().GetList().Where(x => x.AuctionId == mObjAuction.Id).Max(y => y.Number) + 1 : 1;
            }
            else
            {
                throw new Exception("Favor de seleccionar una Subasta.");
            }
        }
        #endregion

        #region Validations

        /// <summary>
        /// Valida los controles.
        /// </summary>
        private bool ValidateControls()
        {
            List<Control> lLstControls = new List<Control>();
            bool lBoolOk = true;

            if (string.IsNullOrEmpty(txtCustomerBuyerId.Text) || mObjBuyer == null)
            {
                lLstControls.Add(txtCustomerBuyerId);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtCustomerSellerId.Text)|| mObjSeller == null)
            {
                lLstControls.Add(txtCustomerSellerId);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtAuction.Text) || mObjAuction == null || (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE))
            {
                lLstControls.Add(txtAuction);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                lLstControls.Add(txtAmount);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtWeight.Text))
            {
                lLstControls.Add(txtWeight);
                lBoolOk = false;
            }

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


        #endregion

        #endregion
    }
}
