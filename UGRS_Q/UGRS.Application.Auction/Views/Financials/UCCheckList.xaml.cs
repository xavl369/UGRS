using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Auctions.Services.Financials;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCFoodCharge.xaml
    /// </summary>
    public partial class UCCheckList : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsServices;
        private FinancialsServicesFactory mObjFinancialsServices;
        private BusinessServicesFactory mObjBusinessServices;
        private ListCollectionView mLcvListData = null; //Filtros
        private List<FoodChargeCheckDTO> mLstObjFoodChargeCheckList;
        private Thread mObjWorker = null;
        private Auction mObjAuction = null;
        private Partner mObjSeller = null;

        #endregion

        #region Constructor

        public UCCheckList()
        {
            InitializeComponent();
            mObjAuctionsServices = new AuctionsServicesFactory();
            mObjFinancialsServices = new FinancialsServicesFactory();
            mObjBusinessServices = new BusinessServicesFactory();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdFoodCharge.IsBlocked())
            {
                mObjWorker = new Thread(() => SetAuction(GetLastAuction()));
                mObjWorker.Start();
            }
        }

        private void btnExpandCollapse_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {
                Button ExpandCollapseObj = (Button)pObjSender;
                FoodChargeCheckDTO lObjChedk = ExpandCollapseObj.DataContext as FoodChargeCheckDTO;

                //Checa el objeto boton si es null o no
                if (ExpandCollapseObj != null)
                {
                    // Return the Contains which specified element
                    DataGridRow DgrSelectedRowObj = DataGridRow.GetRowContainingElement(ExpandCollapseObj);

                    // Check the DataGridRow Object is Null or Not
                    if (DgrSelectedRowObj != null)
                    {
                        // Si el boton ="+" expande los detalles
                        if (ExpandCollapseObj != null && ExpandCollapseObj.Content.ToString() == "+")
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Visible;
                            ExpandCollapseObj.Content = "-";
                        }
                        // else contrae los detalles
                        else
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Collapsed;
                            ExpandCollapseObj.Content = "+";
                        }
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show("Error", "No se pudieron consultar los detalles", this.GetParent());
            }
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

        private void txtSeller_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    InternalSellerSearch((pObjSender as TextBox));
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdFoodCharge.IsBlocked())
            {
                mObjWorker = new Thread(() => InternalSave());
                mObjWorker.Start();
            }
        }

        #endregion

        #region Methods

        public void UpdateCheckList(long pLonSellerId, List<FoodChargeCheckLineDTO> pObjCheckList)
        {
            foreach (var lObjCheck in mLstObjFoodChargeCheckList.Where(x => x.SellerId == pLonSellerId))
            {
                lObjCheck.Lines = pObjCheckList;
            }
        }

        private void InternalAuctionSearch(TextBox pObjTextBox)
        {
            List<Auction> lLstObjAuctions = mObjAuctionsServices.GetAuctionService().SearchAuctions(pObjTextBox.Text, FilterEnum.OPENED);

            if (lLstObjAuctions.Count == 1)
            {
                SetAuction(lLstObjAuctions[0]);
            }
            else
            {
                pObjTextBox.Focusable = false;
                UserControl lUCAuction = new UCSearchAuction(pObjTextBox.Text, lLstObjAuctions, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION);
                SetAuction(FunctionsUI.ShowWindowDialog(lUCAuction, this.GetParent()) as Auction);
                pObjTextBox.Focusable = true;
            }
        }

        private void InternalSellerSearch(TextBox pObjTextBox)
        {
            List<Partner> lLstObjSellers = mObjBusinessServices.GetPartnerService().SearchPartner(pObjTextBox.Text, FilterEnum.ACTIVE);

            if (lLstObjSellers.Count == 1)
            {
                SetSeller(lLstObjSellers[0]);
            }
            else
            {
                pObjTextBox.Focusable = false;
                UserControl lUCBusinessPartner = new UCSearchBusinessPartner(pObjTextBox.Text, lLstObjSellers, FilterEnum.ACTIVE);
                SetSeller(FunctionsUI.ShowWindowDialog(lUCBusinessPartner, this.GetParent()) as Partner);
                pObjTextBox.Focusable = true;
            }
        }

        private void InternalSave()
        {
            grdFoodCharge.BlockUI();

            try
            {
                if (AllLinesForPayment())
                {
                    mObjFinancialsServices.GetFoodChargeCheckService().SaveOrUpdateList(mLstObjFoodChargeCheckList.ToUnextendedList());
                    LoadDataGrid();
                    ShowMessage("Atención", "Cobro cargado correctamente");
                }
                else
                {
                    ShowMessage("Atención", "Verificar las líneas, no debe existir lote sin cobro o alfalfa y 3% como cobros");
                }
            }
            catch (Exception lObjException)
            {
                grdFoodCharge.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdFoodCharge.UnblockUI();
            }
        }

        private bool AllLinesForPayment()
        {
            if (mObjAuction.Location == LocationEnum.HERMOSILLO)
            {
                return mLstObjFoodChargeCheckList.SelectMany(x => x.Lines).Select(x => x).Where(x => (!x.AlfalfaDeliveries
                    && !x.FoodDeliveries && !x.ApplyFoodCharge) || (x.AlfalfaDeliveries && x.ApplyFoodCharge)).Count() > 0 ? false : true;
            }
            else
            {
                return true;
            }
        }

        private Auction GetLastAuction()
        {
            return mObjAuctionsServices.GetAuctionService().GetListFilteredByCC().FirstOrDefault(x => x.Opened && x.Category == AuctionCategoryEnum.AUCTION);
        }

        private void SetAuction(Auction pObjAuction)
        {
            mObjAuction = pObjAuction;

            this.Dispatcher.Invoke(() =>
            {
                if (mObjAuction != null)
                {
                    txtAuction.Text = pObjAuction.Folio;
                }
                else
                {
                    txtAuction.Text = string.Empty;
                }
            });

            LoadDataGrid();
        }

        public void SetSeller(Partner pObjSeller)
        {
            mObjSeller = pObjSeller;

            this.Dispatcher.Invoke(() =>
            {
                if (mObjSeller != null)
                {
                    txtSeller.Text = pObjSeller.Code;
                    tblSeller.Text = pObjSeller.Name;
                }
                else
                {
                    txtSeller.Text = string.Empty;
                    tblSeller.Text = string.Empty;
                }
            });

            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            grdFoodCharge.BlockUI();

            try
            {
                List<FoodChargeCheck> lLstObjFoodChargeCheckList = null;

                lLstObjFoodChargeCheckList = (mObjAuction != null && mObjSeller != null ?
                mObjFinancialsServices.GetFoodChargeCheckService().GetList(mObjAuction.Id, mObjSeller.Id) :

                mObjAuction != null ? mObjFinancialsServices.GetFoodChargeCheckService().GetList(mObjAuction.Id) :
                mObjFinancialsServices.GetFoodChargeCheckService().GetList()).ToList();

                mLstObjFoodChargeCheckList = lLstObjFoodChargeCheckList.ToExtendedList();

                mLcvListData = new ListCollectionView(mLstObjFoodChargeCheckList.OrderBy(x => x.SellerName).ToList());

                this.Dispatcher.Invoke(() =>
                {
                    dgCheckList.ItemsSource = null;
                    dgCheckList.ItemsSource = mLcvListData;
                });
            }
            catch (Exception lObjException)
            {
                grdFoodCharge.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdFoodCharge.UnblockUI();
            }
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

