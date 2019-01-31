using CrystalDecisions.CrystalReports.Engine;
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
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using UGRS.Reports.Auctions.Reports.Inventory;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Services;

namespace UGRS.Application.Auctions
{
    public partial class UCGoodIssues : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsServicesFactory = new AuctionsServicesFactory();
        private InventoryServicesFactory mObjInventoryServicesFactory = new InventoryServicesFactory();
        private BusinessServicesFactory mObjBusinessServicesFactory = new BusinessServicesFactory();
        private ListCollectionView mLcvListData = null; //Filtros
        private List<GoodsIssueDTO> mLstObjPickList = null;
        private Partner mObjBuyer;
        private Partner mObjSeller;
        private Auction mObjAuction;
        private Batch mObjBatch;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCGoodIssues()
        {

            InitializeComponent();
            mLstObjPickList = new List<GoodsIssueDTO>();
            mLcvListData = new ListCollectionView(mLstObjPickList);


        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (mObjAuctionsServicesFactory.GetAuctionService().GetReopenedActiveAuction() == null)
            {
                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();
            }
            else
            {
                ShowMessage("Atención", "No se pueden realizar movimientos en subastas abiertas por segunda vez");
                this.CloseInternalForm();
            }

        }

        #endregion

        #region TextBox

        private void txtCustomer_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    mObjBuyer = ShowDialogPartner(pObjSender, pObjArgs);
                    SetControlsBuyer(mObjBuyer);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtAuction_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    mObjAuction = ShowDialogAuction(pObjSender, pObjArgs);
                    SetControlsAuction(mObjAuction);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtCustomer_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (string.IsNullOrEmpty((pObjSender as TextBox).Text))
            {
                SetControlsBuyer(null);
                RefreshGrid();
            }
        }

        private void txtAuction_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (string.IsNullOrEmpty((pObjSender as TextBox).Text))
            {
                SetControlsAuction(null);
                RefreshGrid();
            }
        }

        private void txtSearch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                RefreshGrid();
            }
        }

        private void txtSearch_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                if (mLstObjPickList.Count == 0)
                    return;

                mLcvListData.Filter = new Predicate<object>(o => (((GoodsIssueDTO)o).BuyerCode.ToUpper().Contains(txtSearch.Text.ToUpper()))
                    || ((GoodsIssueDTO)o).BuyerName.ToUpper().Contains(txtSearch.Text.ToUpper())
                    || ((GoodsIssueDTO)o).ItemTypeName.ToUpper().Contains(txtSearch.Text.ToUpper())
                    || ((GoodsIssueDTO)o).ItemTypeCode.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgBatch.ItemsSource = mLcvListData;
        }

        private void txtQuantityToPick_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
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

        private void txtQuantityToPick_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            UpdateQuantityToPick(pObjSender as TextBox);
        }

        #endregion

        #region DataGrid

        private void dgBatch_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            mObjBatch = dgBatch.SelectedItem as Batch;

        }

        #endregion

        #region Button

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => SaveGoodIssues());
            mObjWorker.Start();
        }

        #endregion

        #endregion

        #region Methods

        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                Auction lObjAuction = mObjAuctionsServicesFactory.GetAuctionService().GetCurrentOrLast(AuctionCategoryEnum.AUCTION);
                mLstObjPickList = mObjInventoryServicesFactory.GetGoodsIssueService().GetListToPick(lObjAuction != null ? lObjAuction.Id : 0);
                mLcvListData = new ListCollectionView(mLstObjPickList);

                this.Dispatcher.Invoke(() =>
                {
                    dgBatch.ItemsSource = null;
                    dgBatch.ItemsSource = mLstObjPickList;

                    SetControlsAuction(lObjAuction);
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

        private void SaveGoodIssues()
        {
            FormLoading(true);
            try
            {
                var lLstObjPickList = mLstObjPickList.FindAll(x => x.QuantityToPick > 0);
                LogService.WriteInfo("PickList Done "+ lLstObjPickList.Count);
                if (lLstObjPickList.Count == 0)
                {
                    this.FormDefault(true);
                    this.ShowMessage("Salida de ganado", "No se encontraron elementos para dar salida.");
                    return;
                }

                if (lLstObjPickList.Exists(l => l.AvailableQuantity < l.QuantityToPick))
                {
                    this.FormDefault(true);
                    this.ShowMessage("Salida de ganado", "No puede devolver una cantidad mayor a la disponible.");
                    return;
                }
                LogService.WriteInfo("Printing");
                PrintGoodsIssue(mObjInventoryServicesFactory.GetGoodsIssueService().CreateGoodsIssues(lLstObjPickList).ToList());
                LogService.WriteInfo("Printed");

                this.FormDefault(true);
                this.Dispatcher.Invoke(() => LoadDatagrid());
                this.ShowMessage("Salida de ganado", "Salidas de ganado creadas con éxito.");
            }
            catch (Exception lObjException)
            {
                LogService.WriteInfo("Error "+ lObjException.Message +" " + lObjException.InnerException.Message);
                this.FormDefault(true);
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private void PrintGoodsIssue(List<long> pLstObjGoodsIssues)
        {
            ReportDocument lObjReportDocument = null;
            FormLoading();

            try
            {
                var lstObjPickReport = mObjInventoryServicesFactory.GetGoodsIssueService().GetList().ToList()
                .Where(x => pLstObjGoodsIssues.Contains(x.Id)).AsEnumerable()
                .Select(x => new UGRS.Core.Auctions.DTO.Reports.Inventory.GoodsIssueDTO()
                {
                    GoodsIssueFolio = x.Folio,
                    GoodsIssueDate = x.CreationDate.ToString("dd/MM/yyyy hh:mm tt"),
                    AuctionFolio = x.Batch.Auction.Folio,
                    CustomerCode = x.Batch.Buyer.Code,
                    CustomerName = x.Batch.Buyer.Name,
                    ItemTypeCode = x.Batch.ItemType.Code,
                    ItemTypeName = x.Batch.ItemType.Name,
                    Quantity = x.Quantity
                })
                .ToList();

                LogService.WriteInfo("Pick report list done");
                LogService.WriteInfo("Loading Report");
                lObjReportDocument = new rptGoodsIssues();
                lObjReportDocument.SetDataSource(lstObjPickReport);
                LogService.WriteInfo("Datasource setted");
                lObjReportDocument.Refresh();
                lObjReportDocument.PrintOptions.PrinterName = Auctions.Properties.Settings.Default.gStrBatchPrinter;
                LogService.WriteInfo("Printer selected");
                lObjReportDocument.PrintToPrinter(1, true, 0, 0);

                LogService.WriteInfo("Report loaded");
            }
            catch (Exception lObjException)
            {
                LogService.WriteInfo(lObjException.Message + " " + lObjException.InnerException.Message);
                FormDefault();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefault();
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
                    grdGoodIssues.BlockUI();
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
                    grdGoodIssues.UnblockUI();
                }
            });
        }

        private void UpdateQuantityToPick(TextBox pObjTextBox)
        {
            try
            {
                GoodsIssueDTO lObjListToPick = pObjTextBox.DataContext as GoodsIssueDTO;
                int lObjQuantityToPick = !string.IsNullOrEmpty(pObjTextBox.Text) ? Convert.ToInt32(pObjTextBox.Text) : 0;
                if (lObjQuantityToPick >= 0)
                {
                    if (lObjListToPick.AvailableQuantity >= lObjQuantityToPick)
                    {
                        lObjListToPick.QuantityToPick = lObjQuantityToPick;
                        pObjTextBox.BorderBrush = Brushes.Black;
                        mLstObjPickList[mLstObjPickList.FindIndex(x => x.BuyerId == lObjListToPick.BuyerId && x.ItemTypeId == lObjListToPick.ItemTypeId)] = lObjListToPick;
                    }
                    else
                    {
                        lObjListToPick.QuantityToPick = 0;
                        pObjTextBox.BorderBrush = Brushes.Red;
                        mLstObjPickList[mLstObjPickList.FindIndex(x => x.BuyerId == lObjListToPick.BuyerId && x.ItemTypeId == lObjListToPick.ItemTypeId)] = lObjListToPick;
                        CustomMessageBox.Show("Salida de ganado", "No puede devolver una cantidad mayor a el total disponible.", this.GetParent());
                    }
                }
                else
                {
                    lObjListToPick.QuantityToPick = 0;
                    pObjTextBox.BorderBrush = Brushes.Red;
                    CustomMessageBox.Show("Salida de ganado", "Ingrese una cantidad mayor a 0.", this.GetParent());
                }
            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
            }
        }

        private Auction ShowDialogAuction(object pObjSender, KeyEventArgs pObjArgs)
        {
            Auction lObjAuction = null;
            string lStrText = (pObjSender as TextBox).Text;
            List<Auction> lLstObjAuctions = mObjAuctionsServicesFactory.GetAuctionService().SearchAuctions(lStrText, FilterEnum.OPENED);

            if (lLstObjAuctions.Count == 1)
            {
                lObjAuction = lLstObjAuctions[0];
            }
            else
            {
                (pObjSender as TextBox).Focusable = false;
                UserControl lUCAuction = new UCSearchAuction(lStrText, lLstObjAuctions, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION);
                lObjAuction = FunctionsUI.ShowWindowDialog(lUCAuction, Window.GetWindow(this)) as Auction;
                (pObjSender as TextBox).Focusable = true;
            }
            (pObjSender as TextBox).Focus();

            if (lObjAuction == null)
                return null;

            MoveToNextUIElement(pObjArgs);
            return lObjAuction;
        }

        private Partner ShowDialogPartner(object pObjSender, KeyEventArgs pObjArgs)
        {
            Partner lObjPartner = null;
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

            if (lObjPartner == null)
                return null;

            MoveToNextUIElement(pObjArgs);
            return lObjPartner;
        }

        private void RefreshGrid()
        {
            mLstObjPickList = mObjInventoryServicesFactory.GetGoodsIssueService().GetListToPick
            (
                mObjAuction != null ? mObjAuction.Id : 0,
                mObjBuyer != null ? mObjBuyer.Id : 0
            );
            mLcvListData = new ListCollectionView(mLstObjPickList);
            dgBatch.ItemsSource = mLstObjPickList;
        }

        private void MoveToNextUIElement(KeyEventArgs pObjArgs)
        {
            FocusNavigationDirection lObjfocusDirection = FocusNavigationDirection.Next;
            TraversalRequest lObjrequest = new TraversalRequest(lObjfocusDirection);
            UIElement lObjElementWithFocus = Keyboard.FocusedElement as UIElement;

            if (lObjElementWithFocus != null)
            {
                if (lObjElementWithFocus.MoveFocus(lObjrequest)) pObjArgs.Handled = true;
            }
        }

        private void SetControlsBuyer(Partner pObjPartner)
        {
            if (pObjPartner != null)
            {
                txtCustomer.Text = pObjPartner.Code;
                tbCustomer.Text = pObjPartner.Name;
                mObjBuyer = pObjPartner;
                RefreshGrid();
            }
            else
            {
                txtCustomer.Text = string.Empty;
                tbCustomer.Text = string.Empty;
                mObjBuyer = null;
            }
        }

        private void SetControlsAuction(Auction pObjAuction)
        {
            if (pObjAuction != null)
            {
                txtAuction.Text = pObjAuction.Folio;
                mObjAuction = pObjAuction;
                RefreshGrid();
            }
            else
            {
                mObjAuction = null;
                txtAuction.Text = string.Empty;
            }
        }

        private bool ValidateControls(Grid pControl)
        {
            var lUnkValidate = FunctionsUI.ValidateControls(grIssues);
            grIssues = lUnkValidate.Item2;
            return lUnkValidate.Item1;
        }

        private List<BatchEasyDTO> ConvertToBatchDTO(List<Batch> pLstBatch)
        {
            AuctionsServicesFactory mObjServiceFactory = new AuctionsServicesFactory();
            List<BatchEasyDTO> lLstBatchDTO = mObjServiceFactory.GetBatchService().ConvertToDTO(pLstBatch, false).ToList();
            return lLstBatchDTO;
        }

        #endregion
    }
}
