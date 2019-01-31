using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Application.Auctions.Events;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using UGRS.Object.WeighingMachine;
using System.Linq;
using System.Data.Entity;

namespace UGRS.Application.Auctions
{
    public partial class UCLeftBatchForm : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsFactory;
        private InventoryServicesFactory mObjInventoryFactory;
        private BusinessServicesFactory mObjBusinessFactory;

        private Partner mObjSeller;
        private ItemType mObjItemType;
        private Batch mObjBatch;

        private string mStrPreviousWeight = string.Empty;
        private string mStrPreviousQtty = string.Empty;

        private WeighingMachineServerObject mObjWeighingMachineServer;
        private WrapperObject mObjWrapperObject;
        private Guid mObjWeighingMachineConnection;
        private bool mBolWeighingMachineLoaded;

        private Thread mObjInternalWorker;

        #endregion

        #region Properties

        public long AuctionId { get; set; }

        public AuctionTypeEnum AuctionType { get; set; }

        public int BatchNumber { get; set; }

        #endregion

        #region Constructor

        public UCLeftBatchForm()
        {
            InitializeComponent();
            mObjAuctionsFactory = new AuctionsServicesFactory();
            mObjInventoryFactory = new InventoryServicesFactory();
            mObjBusinessFactory = new BusinessServicesFactory();
        }

        #endregion

        #region Events

        #region Form

        public event LoadPartnerEventHandler LoadSeller;
        public event LoadItemTypeEventHandler LoadItemType;
        public event LoadBatchEventHandler LoadBatch;
        public event EditBatchEventHandler EditBatch;
        public event ConfirmBatchEventHandler ConfirmBatch;

        private void OnLoadSeller(Partner pObjPartner)
        {
            if (LoadSeller != null)
            {
                LoadSeller(this, new LoadPartnerArgs(pObjPartner));
            }
        }

        private void OnLoadItemType(ItemType pObjItemType)
        {
            if (LoadItemType != null)
            {
                LoadItemType(this, new LoadItemTypeArgs(pObjItemType));
            }
        }

        private void OnLoadBatch(Batch pObjBatch)
        {
            if (LoadBatch != null)
            {
                LoadBatch(this, new LoadBatchArgs(pObjBatch));
            }
        }

        private void OnEditBatch(Batch pObjBatch)
        {
            if (EditBatch != null)
            {
                EditBatch(this, new EditBatchArgs(pObjBatch));
            }
        }

        private void OnConfirmBatch(Batch pObjBatch)
        {
            if (ConfirmBatch != null)
            {
                ConfirmBatch(this, new ConfirmBatchArgs(pObjBatch));
            }
        }

        #endregion

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdContainer.IsBlocked())
            {
                mObjInternalWorker = new Thread(ConnectWeighingMachineService);
                mObjInternalWorker.Start();
            }
        }

        private void UserControl_Unloaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdContainer.IsBlocked())
            {
                mObjInternalWorker = new Thread(DisconnectWeighingMachineService);
                mObjInternalWorker.Start();
            }
        }

        #endregion

        #region TextBox

        private void txtSellerCode_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    DateTime lDtAuctionDate = mObjAuctionsFactory.GetAuctionService().GetActiveAuction().Date;
                    List<long> lLstLonSellersWithStock = mObjInventoryFactory.GetStockService().GetListByWhs().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && x.Quantity > 0).Select(x => x.CustomerId).Distinct().ToList();

                    lLstLonSellersWithStock.AddRange((mObjInventoryFactory.GetGoodsReceiptService().GetList().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && !x.Processed).Select(x => x.CustomerId).Distinct().ToList()));

                    List<Partner> lLstObjSellers = mObjBusinessFactory.GetPartnerService().SearchPartnerWithStock(lStrText, FilterEnum.ACTIVE, lLstLonSellersWithStock);

                    if (lLstObjSellers.Count == 1)
                    {
                        SetSellerObject(lLstObjSellers[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjSellers, FilterEnum.ACTIVE);
                        SetSellerObject(FunctionsUI.ShowWindowDialog(lUCPartner, this.GetParent()) as Partner);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtItemType_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;

                    //mObjInventoryFactory.GetItemDefinitionService().GetHeadTypeRelation(x.ItemId,lObjGoodsIssue.Batch.ItemTypeId)


                    List<ItemType> lLstObjItemTypes = mObjInventoryFactory.GetItemTypeService()
                        .SearchItemTypeByAuctionType(lStrText, AuctionType, FilterEnum.AUCTION)
                        .Where(x => !x.Removed
                            && (mObjInventoryFactory.GetItemDefinitionService().GetDefinitions(x.Id))).Select(y => y).ToList();



                    if (lLstObjItemTypes.Count == 1)
                    {
                        SetItemTypeObject(lLstObjItemTypes[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCItemType = new UCSearchItemType(lStrText, AuctionType, lLstObjItemTypes, FilterEnum.AUCTION);
                        SetItemTypeObject(FunctionsUI.ShowWindowDialog(lUCItemType, this.GetParent()) as ItemType);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtBatchSearch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Batch> lLstObjBatches = mObjAuctionsFactory.GetBatchService().SearchBatches(lStrText, AuctionId);

                    if (lLstObjBatches.Count == 1)
                    {
                        InternalEditBatch(lLstObjBatches[0]);

                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCSearchBatch = new UCSearchBatch(lStrText, lLstObjBatches, AuctionId);
                        InternalEditBatch(FunctionsUI.ShowWindowDialog(lUCSearchBatch, this.GetParent()) as Batch);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtQuantity_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                txtItemType.Focus();
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

        private void txtQuantity_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            CalculateAverageWeight();
            ValidAvailabilityStock();
        }

        #endregion

        #region CheckBox

        private void chkReprogrammed_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                txtQuantity.Focus();
            }
        }

        private void chkSumPrevious_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                btnConfirm.Focus();
            }
        }

        private void chkSumPrevious_Checked(object pObjSender, RoutedEventArgs pObjArgs)
        {
            tblPrevousWeight.Visibility = Visibility.Visible;
            tblPrevousWeightLabel.Visibility = Visibility.Visible;
            tblWeightLabel.Visibility = Visibility.Visible;
            tblPrevousQtty.Visibility = Visibility.Visible;
            tblPrevousQttyLabel.Visibility = Visibility.Visible;
            //txtQuantity.Text = string.Empty;
            tblPrevousQtty.Text = mStrPreviousQtty;
        }

        private void chkSumPrevious_Unchecked(object pObjSender, RoutedEventArgs pObjArgs)
        {
            tblPrevousWeight.Visibility = Visibility.Collapsed;
            tblPrevousWeightLabel.Visibility = Visibility.Collapsed;
            tblWeightLabel.Visibility = Visibility.Collapsed;
            tblPrevousQtty.Visibility = Visibility.Collapsed;
            tblPrevousQttyLabel.Visibility = Visibility.Collapsed;
            txtQuantity.Text = mStrPreviousQtty;
        }

        private void chkSellType_Checked(object sender, RoutedEventArgs e)
        {
            chkSellType.Content = "Por Precio";
        }

        private void chkSellType_Unchecked(object sender, RoutedEventArgs e)
        {
            chkSellType.Content = "Por Kilos";
        }

        #endregion

        #region Button

        private void btnCheck_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                txtWeight.Text = chkSumPrevious.IsChecked == true ?
                (
                    !string.IsNullOrEmpty(mStrPreviousWeight) ?

                        (Convert.ToDouble(tblPrevousWeight.Text) + Convert.ToDouble(tblWeight.Text)).ToString() :
                        tblWeight.Text

                ) : tblWeight.Text;

                tblPrevousQtty.Text = chkSumPrevious.IsChecked == true ?
                    (
                    !string.IsNullOrEmpty(mStrPreviousQtty) && !string.IsNullOrEmpty(txtQuantity.Text) ?
                    (Convert.ToInt32(mStrPreviousQtty) + Convert.ToInt32(txtQuantity.Text)).ToString() :
                    txtQuantity.Text
                    ) : string.Empty;

                if (chkSumPrevious.IsChecked == true)
                {
                    mStrPreviousQtty = tblPrevousQtty.Text;
                    txtQuantity.Text = string.Empty;
                    mStrPreviousWeight = txtWeight.Text;
                    tblPrevousWeight.Text = mStrPreviousWeight;
                }
                else
                {
                    mStrPreviousWeight = txtWeight.Text;
                    tblPrevousWeight.Text = mStrPreviousWeight;
                    mStrPreviousQtty = txtQuantity.Text;
                }




                CalculateAverageWeight();
                chkSumPrevious.Focus();
            }
        }

        private void btnConfirm_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {
                if (chkSumPrevious.IsChecked == true)
                {
                    txtQuantity.Text = mStrPreviousQtty;
                }

                if (grdLeftBatch.Valid() && ValidSellerAndItemType())
                {
                    if (ValidAvailabilityStock() && ValidAvailabilityItemTypeStock())
                    {
                        OnConfirmBatch(GetBatchObject());
                        BatchNumber = BatchNumber + 1;
                        chpBatch.Content = string.Format("Lote {0}", BatchNumber);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Lote", "Favor de completar los campos.", this.GetParent());
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Lote", lObjException.Message, this.GetParent());
            }
        }

        private void btnStockDetails_Click(object sender, RoutedEventArgs e)
        {
            if (mObjSeller != null)
            {
                this.ShowCustomerStockDetails(AuctionId, mObjSeller.Id);
            }
        }

        #endregion

        #region Weighing Machine Service

        private void OnDataReceived(string pStrValue)
        {
            try
            {
                tblWeight.Dispatcher.Invoke((Action)delegate
                {
                    tblWeight.Text = pStrValue;
                });
            }
            catch (Exception lObjException)
            {
                //Ignore
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Form

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        private void CalculateAverageWeight()
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text) && !string.IsNullOrEmpty(txtWeight.Text))
            {
                string lStrWeight = txtWeight.Text;

                double lDblQuantity = Convert.ToDouble(txtQuantity.Text);
                double lDblWeight = 0;

                try
                {
                    lDblWeight = Convert.ToDouble(lStrWeight);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Error", ex.Message, this.GetParent());
                }

                txtAverageWeight.Text = (lDblQuantity > 0 && lDblWeight > 0 ? (lDblWeight / lDblQuantity) : 0).ToString();
            }
        }

        private bool ValidAvailabilityStock()
        {
            if (mObjSeller != null && !string.IsNullOrEmpty(txtSellerCode.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                bool lBolChekced = chkReprogrammed.IsChecked ?? false;
                int lIntRequiredQuantity = Convert.ToInt32(txtQuantity.Text);

                int lIntAvailabilityStock = !lBolChekced ?
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, mObjSeller.Id) :
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, mObjSeller.Id);

                if (lIntAvailabilityStock >= lIntRequiredQuantity ||
                    mObjBatch != null && mObjBatch.Id > 0 && (lIntAvailabilityStock + mObjBatch.Quantity) >= lIntRequiredQuantity)
                {
                    tblErrorQuantity.Visibility = Visibility.Collapsed;
                    return true;
                }
                else
                {
                    tblErrorQuantity.Visibility = Visibility.Visible;
                    return false;
                }
            }
            return true;
        }

        private bool ValidAvailabilityItemTypeStock()
        {
            if (mObjSeller != null && !string.IsNullOrEmpty(txtSellerCode.Text) && !string.IsNullOrEmpty(txtQuantity.Text) && mObjItemType != null)
            {
                bool lBolChekced = chkReprogrammed.IsChecked ?? false;
                int lIntRequiredQuantity = Convert.ToInt32(txtQuantity.Text);

                int lIntAvailabilityStock = !lBolChekced ?
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender) :
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender);

                if (lIntAvailabilityStock >= lIntRequiredQuantity ||
                    mObjBatch != null && mObjBatch.Id > 0 && (lIntAvailabilityStock + mObjBatch.Quantity) >= lIntRequiredQuantity)
                {
                    tblErrorQuantity.Visibility = Visibility.Collapsed;
                    return true;
                }
                else
                {
                    tblErrorQuantity.Visibility = Visibility.Visible;
                    return false;
                }
            }
            return true;
        }

        public void EnableForm()
        {
            ResetBatch();
            chpBatch.Visibility = Visibility.Visible;
            grdLeftBatch.EnableControl();
            txtSellerName.IsEnabled = false;
            txtTaxCode.IsEnabled = false;
            txtWeight.IsEnabled = false;
            txtAverageWeight.IsEnabled = false;
            btnStockDetails.IsEnabled = false;
        }

        public void DisableForm()
        {
            ResetBatch();
            chpBatch.Visibility = Visibility.Collapsed;
            grdLeftBatch.DisableControl();
        }

        private bool ValidSellerAndItemType()
        {
            bool lBolResult = true;

            if (mObjSeller == null)
            {
                lBolResult = false;
                txtSellerCode.BorderBrush = Brushes.Red;
            }

            if (mObjItemType == null)
            {
                lBolResult = false;
                txtItemType.BorderBrush = Brushes.Red;
            }

            return lBolResult;
        }

        #endregion

        #region Seller

        private void SetSellerObject(Partner pObjSeller)
        {
            try
            {
                mObjSeller = pObjSeller;
                OnLoadSeller(pObjSeller);

                if (pObjSeller != null)
                {
                    txtSellerCode.Text = pObjSeller.Code;
                    txtSellerName.Text = pObjSeller.Name;
                    txtTaxCode.Text = pObjSeller.TaxCode;
                    btnStockDetails.IsEnabled = true;
                }
                else
                {
                    ResetSeller();
                }

                chkReprogrammed.Focus();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetSeller()
        {
            txtSellerCode.Text = string.Empty;
            txtSellerName.Text = string.Empty;
            txtTaxCode.Text = string.Empty;
            btnStockDetails.IsEnabled = false;
        }

        #endregion

        #region ItemType

        private void SetItemTypeObject(ItemType pObjItemType)
        {
            try
            {
                mObjItemType = pObjItemType;
                OnLoadItemType(pObjItemType);

                if (pObjItemType != null)
                {
                    txtItemType.Text = pObjItemType.Name;
                    ValidAvailabilityItemTypeStock();
                    SetSellType(pObjItemType.SellType);
                }
                else
                {
                    ResetItemType();
                }

                btnCheck.Focus();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void SetSellType(Core.Auctions.Enums.Inventory.SellTypeEnum sellTypeEnum)
        {
            switch (sellTypeEnum)
            {
                case Core.Auctions.Enums.Inventory.SellTypeEnum.PerWeight:
                    chkSellType.Content = "Por Kilo";
                    chkSellType.IsChecked = false;
                    chkSellType.IsEnabled = false;
                    break;
                case Core.Auctions.Enums.Inventory.SellTypeEnum.PerPrice:
                    chkSellType.Content = "Por Precio";
                    chkSellType.IsChecked = true;
                    chkSellType.IsEnabled = false;
                    break;
                case Core.Auctions.Enums.Inventory.SellTypeEnum.Both:
                    chkSellType.Content = "Por Kilo";
                    chkSellType.IsChecked = false;
                    chkSellType.IsEnabled = true;
                    break;
            }
        }

        private void ResetItemType()
        {
            txtItemType.Text = string.Empty;
        }

        #endregion

        #region Batch

        public void EnableBatchSearch()
        {
            txtBatchSearch.IsEnabled = true;
            txtBatchSearch.ClearControl();
        }

        public void DisableBatchSearch()
        {
            txtBatchSearch.IsEnabled = false;
            txtBatchSearch.ClearControl();
        }

        public void SetBatchObject(Batch pObjBatch)
        {
            mObjInternalWorker = new Thread(() => InternalSetBatchObject(pObjBatch));
            mObjInternalWorker.Start();
        }

        private void ResetBatch()
        {
            grdLeftBatch.ClearControl();
            SetSellerObject(null);
            SetItemTypeObject(null);
            chkReprogrammed.IsChecked = false;
            chkSumPrevious.IsChecked = false;
        }

        private Batch GetBatchObject()
        {
            Batch lObjBatch = new Batch();

            //Required fields
            lObjBatch.Id = mObjBatch != null ? mObjBatch.Id : 0;
            lObjBatch.Number = mObjBatch != null ? mObjBatch.Number : BatchNumber;
            lObjBatch.AuctionId = AuctionId;
            lObjBatch.SellerId = mObjSeller.Id;
            lObjBatch.ItemTypeId = mObjItemType.Id;
            lObjBatch.Reprogrammed = chkReprogrammed.IsChecked ?? false;
            lObjBatch.Quantity = Convert.ToInt32(txtQuantity.Text);
            lObjBatch.Weight = Convert.ToInt32(txtWeight.Text);
            lObjBatch.AverageWeight = Convert.ToSingle(txtAverageWeight.Text);
            lObjBatch.ItemType = mObjItemType;
            lObjBatch.ItemType.SellType = (bool)chkSellType.IsChecked ? Core.Auctions.Enums.Inventory.SellTypeEnum.PerPrice : Core.Auctions.Enums.Inventory.SellTypeEnum.PerWeight;
            lObjBatch.SellType = (bool)chkSellType.IsChecked ? true : false;
            //Optional fields
            lObjBatch.Price = mObjBatch != null ? mObjBatch.Price : 0;
            lObjBatch.Amount = mObjBatch != null ? mObjBatch.Amount : 0;
            lObjBatch.BuyerId = mObjBatch != null ? mObjBatch.BuyerId : null;
            lObjBatch.Unsold = mObjBatch != null ? mObjBatch.Unsold : false;

            if (lObjBatch.Unsold)
            {
                lObjBatch.UnsoldMotive = mObjBatch.UnsoldMotive;
            }

            return lObjBatch;
        }

        private void InternalEditBatch(Batch pObjBatch)
        {
            if (pObjBatch != null)
            {
                OnEditBatch(pObjBatch);
            }
        }

        #endregion

        #region Weighing Machine Service

        private void ConnectWeighingMachineService()
        {
            grdContainer.BlockUI();
            mBolWeighingMachineLoaded = false;

            try
            {
                mObjWeighingMachineServer = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
                mObjWrapperObject = new WrapperObject();

                mObjWeighingMachineServer.DataReceived += new WeighingMachineEventHandler(mObjWrapperObject.WrapperOnDataReceived);
                mObjWrapperObject.WrapperDataReceived += new WeighingMachineEventHandler(OnDataReceived);

                mObjWeighingMachineConnection = mObjWeighingMachineServer.Connect();
                mBolWeighingMachineLoaded = true;
            }
            catch (Exception lObjException)
            {
                grdContainer.UnblockUI();
                ShowMessage("Error", lObjException.Message);
                mBolWeighingMachineLoaded = false;
            }
            finally
            {
                grdContainer.UnblockUI();
            }
        }

        private void DisconnectWeighingMachineService()
        {
            grdContainer.BlockUI();

            try
            {
                if (mBolWeighingMachineLoaded)
                {
                    mObjWeighingMachineServer.Disconnect(mObjWeighingMachineConnection);
                }
            }
            catch (Exception lObjException)
            {
                grdContainer.UnblockUI();
                ShowMessage("Error", lObjException.Message);
                mBolWeighingMachineLoaded = false;
            }
            finally
            {
                grdContainer.UnblockUI();
            }
        }

        #endregion

        #region Internal

        private void InternalSetBatchObject(Batch pObjBatch)
        {
            grdContainer.BlockUI();
            Thread.Sleep(300);

            try
            {
                //mObjBatch = pObjBatch;
                InvokeOnLoadBatch(pObjBatch);

                if (pObjBatch != null)
                {
                    Partner lObjSeller = pObjBatch.SellerId != null ? mObjBusinessFactory.GetPartnerService().GetEntity(pObjBatch.SellerId ?? 0) : null;
                    ItemType lObjItemType = mObjInventoryFactory.GetItemTypeService().GetEntity(pObjBatch.ItemTypeId ?? 0);

                    InvokeSetBatchObject(pObjBatch, lObjSeller, lObjItemType);
                }
                else
                {
                    InvokeResetBatch();
                }
            }
            catch (Exception lObjException)
            {
                grdContainer.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdContainer.UnblockUI();
            }
        }

        #endregion

        #region Invoke

        private void InvokeOnLoadBatch(Batch pObjBatch)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                OnLoadBatch(pObjBatch);
            });
        }

        private void InvokeSetBatchObject(Batch pObjBatch, Partner pObjSeller, ItemType pObjItemType)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                SetSellerObject(pObjSeller);
                SetItemTypeObject(pObjItemType);
                chkReprogrammed.IsChecked = pObjBatch.Reprogrammed;
                txtQuantity.Text = pObjBatch.Quantity.ToString();
                txtWeight.Text = pObjBatch.Weight.ToString();
                txtAverageWeight.Text = pObjBatch.AverageWeight.ToString();
                chpBatch.Content = string.Format("Lote {0}", pObjBatch.Number);
                chpBatch.Visibility = Visibility.Visible;
            });
        }

        private void InvokeResetBatch()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdLeftBatch.ClearControl();
                SetSellerObject(null);
                SetItemTypeObject(null);
                chkReprogrammed.IsChecked = false;
                chkSumPrevious.IsChecked = false;
                chpBatch.Content = "";
                chpBatch.Visibility = Visibility.Collapsed;
            });
        }

        #endregion



        #endregion


    }
}