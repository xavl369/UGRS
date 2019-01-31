using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Application.Auctions.Events;
using UGRS.Core.Application.Enum.Forms;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Auction;
using UGRS.Core.Auctions.DTO.Business;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.DTO.Utility;
using UGRS.Core.Extension;
using UGRS.Data.Auctions.Factories;
using UGRS.Application.Auctions.Extensions;
using QualisysConfig;
using System.Data.Entity;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Application.Auctions
{
    public partial class UCRightBatchForm : UserControl
    {
        #region Attributes

        private FormModeEnum mEnmFormMode;
        AuctionsServicesFactory mObjAuctionsFactory;
        InventoryServicesFactory mObjInventoryFactory;
        BusinessServicesFactory mObjBusinessFactory;

        Batch mObjBatch;
        Partner mObjTempSeller;
        Partner mObjSeller;
        Partner mObjBuyer;
        ItemType mObjItemType;
        ItemType mObjTempItemType;
        PartnerClassification mObjBuyerClassification;

        Thread mObjInternalWorker;

        #endregion

        #region Properties

        public long AuctionId { get; set; }
        public AuctionTypeEnum AuctionType { get; set; }
        public int BatchNumber { get; set; }

        private bool mBoolEdition = false;
        private bool mBoolAbleToSave = false;
        #endregion

        #region Constructor

        public UCRightBatchForm()
        {
            InitializeComponent();
            mEnmFormMode = FormModeEnum.DEFAULT;
            mObjAuctionsFactory = new AuctionsServicesFactory();
            mObjInventoryFactory = new InventoryServicesFactory();
            mObjBusinessFactory = new BusinessServicesFactory();
        }

        #endregion

        #region Events

        #region Form

        public event LoadPartnerEventHandler LoadSeller;
        public event LoadPartnerClassificationEventHandler LoadBuyerClassification;
        public event LoadItemTypeEventHandler LoadItemType;
        public event LoadBatchEventHandler LoadBatch;
        public event CompleteBatchEventHandler CompleteBatch;
        public event SaveBatchEventHandler SaveBatch;
        public event UndoBatchEventHandler UndoBatch;
        public event EditBatchEventHandler EditiBatch;
        public event ChangeFormModeEventHandler ChangeFormMode;

        private void OnLoadSeller(Partner pObjPartner)
        {
            if (LoadSeller != null)
            {
                LoadSeller(this, new LoadPartnerArgs(pObjPartner));
            }
        }

        private void OnLoadBuyerClassification(PartnerClassification pObjClassification)
        {
            if (LoadBuyerClassification != null)
            {
                LoadBuyerClassification(this, new LoadPartnerClassificationArgs(pObjClassification));
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

        private void OnCompleteBatch(Batch pObjBatch)
        {
            if (CompleteBatch != null)
            {
                CompleteBatch(this, new CompleteBatchArgs(pObjBatch));
            }
        }

        private void OnSaveBatch(Batch pObjBatch)
        {
            if (SaveBatch != null)
            {
                SaveBatch(this, new SaveBatchArgs(pObjBatch));
            }
        }

        private void OnUndoBatch(Batch pObjBatch)
        {
            if (UndoBatch != null)
            {
                UndoBatch(this, new UndoBatchArgs(pObjBatch));
            }
        }

        private void OnEditedBatch(Batch pObjBatch)
        {
            if(EditiBatch != null)
            {
                EditiBatch(this, new EditBatchArgs(pObjBatch));
            }
        }


        private void OnChangeFormMode(FormModeEnum pEnmFormMode)
        {
            if (ChangeFormMode != null)
            {
                ChangeFormMode(this, new ChangeFormModeArgs(pEnmFormMode));
            }
        }

        #endregion

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            LoadUnsoldsReasons();
        }

        private void UserControl_Unloaded(object pObjSender, RoutedEventArgs pObjArgs)
        {

        }

        #endregion

        #region TextBox

        private void txtSellerCode_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                mObjTempSeller = mObjSeller;

                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    DateTime lDtAuctionDate = mObjAuctionsFactory.GetAuctionService().GetActiveAuction().Date;
                    List<long> lLstLonSellersWithStock = mObjInventoryFactory.GetStockService().GetListByWhs().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && x.Quantity > 0).Select(x => x.CustomerId).Distinct().ToList();

                    lLstLonSellersWithStock.AddRange((mObjInventoryFactory.GetGoodsReceiptService().GetList().Where(x => (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(lDtAuctionDate)) && !x.Processed).Select(x => x.CustomerId).Distinct().ToList()));

                    List<Partner> lLstObjSellers = mObjBusinessFactory.GetPartnerService().SearchPartnerWithStock(lStrText, FilterEnum.ACTIVE, lLstLonSellersWithStock);

                    if (lLstObjSellers.Count == 1)
                    {
                        SetSellerObject(lLstObjSellers[0], true);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjSellers, FilterEnum.ACTIVE);
                        SetSellerObject(FunctionsUI.ShowWindowDialog(lUCPartner, this.GetParent()) as Partner, true);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }

            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtBuyerCode_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<PartnerClassification> lLstObjClassifications = mObjBusinessFactory.GetPartnerClassificationService().SearchPartner(lStrText, FilterEnum.ACTIVE);

                    if (lLstObjClassifications.Count == 1)
                    {
                        SetBuyerClassificationObject(lLstObjClassifications[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartnerClassification(lStrText, lLstObjClassifications, FilterEnum.ACTIVE);
                        SetBuyerClassificationObject
                        (
                            mObjBusinessFactory.GetPartnerClassificationService().GetClassification
                            (
                                FunctionsUI.ShowWindowDialog(lUCPartner, this.GetParent()) as PartnerClassificationDTO
                            )
                        );
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
                mObjTempItemType = mObjItemType;

                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;

                    List<ItemType> lLstObjItemTypes = mObjInventoryFactory.GetItemTypeService()
                        .SearchItemTypeByAuctionType(lStrText, AuctionType, FilterEnum.AUCTION)
                        .Where(x => !x.Removed
                            && (mObjInventoryFactory.GetItemDefinitionService().GetDefinitions(x.Id))).Select(y => y).ToList();

                    if (lLstObjItemTypes.Count == 1)
                    {
                        SetItemTypeObject(lLstObjItemTypes[0],true);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCItemType = new UCSearchItemType(lStrText, AuctionType, lLstObjItemTypes, FilterEnum.AUCTION);
                        SetItemTypeObject(FunctionsUI.ShowWindowDialog(lUCItemType, this.GetParent()) as ItemType,true);
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
            if (txtQuantity.Text.Length > 0 && txtQuantity.Text.Trim().Length != 0)
            {
                if (mObjBatch.Quantity == 0)
                {
                    CalculateAverageWeight();
                    ValidAvailabilityStock();
                }
                else
                {
                    CalculateAverageWeight();
                    ValidAviabilityEditedStock();
                }
            }
            else
            {
                txtQuantity.Text = string.Empty;
            }
        }

        private bool ValidAviabilityEditedStock()
        {

            if (mObjSeller != null && !string.IsNullOrEmpty(txtSellerCode.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                bool lBolChekced = chkReprogrammed.IsChecked ?? false;
                int lIntRequiredQuantity = Convert.ToInt32(txtQuantity.Text);

                int lIntAvailabilityStock = !lBolChekced ?
                mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForEditedBatchesInCurrentAuction(mObjBatch, mObjSeller.Id, Convert.ToInt32(txtQuantity.Text), mObjItemType.Gender) :
                mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogrammedEdited(mObjBatch, mObjSeller.Id, Convert.ToInt32(txtQuantity.Text), mObjItemType.Gender);


                if (lIntAvailabilityStock >= lIntRequiredQuantity)
                {
                    tblErrorQuantity.Visibility = Visibility.Collapsed;
                    mBoolAbleToSave = true;
                    return true;
                }
                else
                {
                    tblErrorQuantity.Visibility = Visibility.Visible;
                    mBoolAbleToSave = false;
                    return false;
                }
            }

            return true;
        }

        private void txtPrice_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                txtBuyerCode.Focus();
            }
        }

        private void txtPrice_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
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

        private void txtPrice_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (txtPrice.Text.Length > 0 && txtPrice.Text.Trim().Length != 0)
            {
                CalculateAmount();
            }
            else
            {
                txtPrice.Text = string.Empty;
            }
        }

        private void txtWeight_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                txtPrice.Focus();
            }
        }

        private void txtWeight_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
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

        private void txtWeight_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (txtWeight.Text.Length > 0 && txtWeight.Text.Trim().Length != 0)
            {
                CalculateAverageWeight();
                CalculateAmount();
            }
            else
            {
                txtWeight.Text = string.Empty;
            }
        }

        #endregion

        #region CheckBox

        private void chkUnsold_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if ((pObjSender as CheckBox).IsChecked == true)
            {
                SetBuyerClassificationObject(null);
                txtBuyerCode.IsEnabled = false;
                cboReason.Visibility = Visibility.Visible;
                cboReason.Focus();
            }
            else
            {
                if (mObjBatch.Quantity > 0)
                {
                    int lIntAvailableStock = !mObjBatch.Reprogrammed ?
                        mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender) :
                        mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender);

                    if(mBoolEdition)
                    {
                        lIntAvailableStock += mObjBatch.Quantity;
                    }

                    if ((lIntAvailableStock) > 0)
                    {
                        txtBuyerCode.IsEnabled = true;
                        OnCompleteBatch(GetBatchObject());
                    }
                    else
                    {
                        (pObjSender as CheckBox).IsChecked = true;
                    }
                }
                else
                {
                    txtBuyerCode.IsEnabled = true;

                    OnCompleteBatch(GetBatchObject());
                }
            }
        }

        private void chkUnsold_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        
        {
            if (pObjArgs.Key == Key.Enter )
            {
                Save();
            }

        }

        private void chkReprogrammed_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            int lIntAvailableStock = 0;
            bool lBoolActualState = mObjBatch.Reprogrammed;
            int lIntQuantity = mObjBatch.Quantity != Convert.ToInt32(txtQuantity.Text) ? Convert.ToInt32(txtQuantity.Text) : mObjBatch.Quantity;

            if (mObjBatch.Quantity > 0)
            {
                if ((pObjSender as CheckBox).IsChecked == true)
                {
                    lIntAvailableStock = mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender);
                }
                else
                {
                    lIntAvailableStock = mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender);
                }

                if (lIntAvailableStock <= 0 || lIntAvailableStock < lIntQuantity)
                {
                    (pObjSender as CheckBox).IsChecked = lBoolActualState;
                    ShowMessage("Atención", "No se tiene inventario suficiente para hacer ese cambio al lote");
                }
            }
        }

        private void chkReprogrammed_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                txtQuantity.Focus();
            }
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

        #region ComboBox

        private void cboReason_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            if (pObjArgs.Key == Key.Enter)
            {
                OnCompleteBatch(GetBatchObject());
            }
        }

        #endregion

        #region Button

        private void btnChangeWeight_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_WEIGHT) == true)
            {
                txtWeight.IsEnabled = true;
                txtWeight.Focus();
            }
        }

        private void btnChangePrice_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_PRICE) == true)
            {
                txtPrice.IsEnabled = true;
                txtPrice.Focus();
            }
        }

        private void btnChangeBuyer_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_BUYER) == true)
            {
                txtBuyerCode.IsEnabled = true;
                txtBuyerCode.Focus();
            }
        }

        private void btnChangeQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_QUANTITY) == true)
            {
                txtQuantity.IsEnabled = true;
                txtQuantity.Focus(); ;
            }
        }

        private void btnChangeSeller_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_SELLER) == true)
            {
                txtSellerCode.IsEnabled = true;
                txtSellerCode.Focus();
            }
        }

        private void btnReprogrammated_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_REPROGRAMMED) == true)
            {
                chkReprogrammed.IsEnabled = true;
            }
        }
        private void btnSold_Click(object sender, RoutedEventArgs e)
        {
            if (mObjAuctionsFactory.GetBatchService().IsEditableBatch(mObjBatch))
            {
                if (ShowAuthorizationForm(mObjBatch.Id, SpecialFunctionsEnum.CHANGE_NOTSOLD) == true)
                {
                    chkUnsold.IsEnabled = true;
                }
            }
            else
            {
                ShowMessage("No permitido", "No se puede modificar esta propiedad en este lote");
            }
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            UCBatchLog lObjUCLog = new UCBatchLog(mObjBatch != null ? mObjBatch.Id : 0);
            BaseForm lObjBaseForm = new BaseForm(Window.GetWindow(this));

            lObjBaseForm.tblTitle.Text = "Log de modificaciones";
            lObjBaseForm.MaxWidth = 1024;
            lObjBaseForm.grdContainer.Children.Add(lObjUCLog);
            lObjBaseForm.SizeToContent = System.Windows.SizeToContent.Height;
            lObjBaseForm.ResizeMode = System.Windows.ResizeMode.NoResize;

            if (lObjBaseForm.ShowDialog() == true)
            {
                BatchLogDTO lObjResult = lObjBaseForm.ResultObject as BatchLogDTO;
                if (lObjResult != null)
                {
                    SetBatchObject(lObjResult.BatchObject.JsonDeserialize<Batch>());
                    chpLog.Content = string.Format("Lote {0} ─ {1}", lObjResult.BatchNumber, lObjResult.ModificationDate.ToString());
                    UpdateFormMode(FormModeEnum.READ);
                }
            }
        }

        private void chpLog_DeleteClick(object sender, RoutedEventArgs e)
        {
            SetBatchObject(mObjAuctionsFactory.GetBatchService().GetEntity(mObjBatch != null ? mObjBatch.Id : 0));
            UpdateFormMode(FormModeEnum.EDIT);
        }

        private void btnStockDetails_Click(object sender, RoutedEventArgs e)
        {
            if (mObjSeller != null)
            {
                this.ShowCustomerStockDetails(AuctionId, mObjSeller.Id);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Form

        private bool? ShowAuthorizationForm(long pLonBatchId, SpecialFunctionsEnum pEnmFunction)
        {
            UCAuthorization lObjUCAuthorization = new UCAuthorization(pLonBatchId, pEnmFunction);
            BaseForm lObjBaseForm = new BaseForm(this.GetParent());

            lObjBaseForm.tblTitle.Text = "Autorización";
            lObjBaseForm.MinWidth = 300;
            lObjBaseForm.grdContainer.Children.Add(lObjUCAuthorization);
            lObjBaseForm.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            lObjBaseForm.ResizeMode = System.Windows.ResizeMode.NoResize;

            return lObjBaseForm.ShowDialog();
        }

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        public void EnableForm()
        {

            ResetBatch();
            grdRightBatch.EnableControl();
            txtSellerName.IsEnabled = false;
            txtBuyerName.IsEnabled = false;
            txtTaxCode.IsEnabled = false;
            txtWeight.IsEnabled = false;
            txtAverageWeight.IsEnabled = false;
            txtAmount.IsEnabled = false;
        }

        public void DisableForm()
        {
            ResetBatch();
            grdRightBatch.DisableControl();
        }

        public void UpdateFormMode(FormModeEnum pEnmFormMode)
        {
            grdRightBatch.DisableControl();
            switch (pEnmFormMode)
            {
                case FormModeEnum.NEW:
                    // Fields
                    txtSellerCode.IsEnabled = true;
                    txtTaxCode.IsEnabled = true;
                    txtQuantity.IsEnabled = true;
                    txtItemType.IsEnabled = true;
                    txtPrice.IsEnabled = true;
                    txtBuyerCode.IsEnabled = true;
                    // Checks
                    chkReprogrammed.IsEnabled = true;
                    chkUnsold.IsEnabled = true;
                    //Chips
                    chpBatch.Visibility = Visibility.Visible;
                    chpLog.Visibility = Visibility.Collapsed;
                    //Buttons
                    btnChangeSeller.IsEnabled = false;
                    btnChangeQuantity.IsEnabled = false;
                    btnReprogrammated.IsEnabled = false;
                    btnSold.IsEnabled = false;
                    //Combo
                    cboReason.IsEnabled = true;
                    break;
                case FormModeEnum.EDIT:
                    if (mObjBatch.Quantity > 0)
                    {

                        btnLog.IsEnabled = true;

                        if (DateTime.Now.TimeOfDay >= GetPreClosureTime())
                        {

                            btnChangeBuyer.IsEnabled = true;
                            btnChangeSeller.IsEnabled = true;
                            btnChangeQuantity.IsEnabled = true;
                            btnChangeWeight.IsEnabled = true;
                            btnChangePrice.IsEnabled = true;
                            btnReprogrammated.IsEnabled = true;
                            btnSold.IsEnabled = true;
                        }
                        else
                        {
                            if (mObjAuctionsFactory.GetBatchService().IsEditableBatch(mObjBatch))
                            {
                                txtSellerCode.IsEnabled = true;
                                txtQuantity.IsEnabled = true;
                                txtItemType.IsEnabled = true;
                                txtWeight.IsEnabled = true;
                                txtBuyerCode.IsEnabled = mObjBatch.Unsold ? false : true;
                                txtPrice.IsEnabled = true;
                                cboReason.IsEnabled = true;
                                chkReprogrammed.IsEnabled = true;
                                chkUnsold.IsEnabled = true;

                            }
                            else
                            {
                                btnChangeBuyer.IsEnabled = true;
                                btnChangeSeller.IsEnabled = true;
                                btnChangeQuantity.IsEnabled = true;
                                btnChangeWeight.IsEnabled = true;
                                btnChangePrice.IsEnabled = true;
                                btnReprogrammated.IsEnabled = true;
                                btnSold.IsEnabled = true;
                                txtItemType.IsEnabled = true;

                                cboReason.IsEnabled = true;
                            }
                            //Chips
                            chpBatch.Visibility = Visibility.Visible;
                            chpLog.Visibility = Visibility.Collapsed;
                        }

                    }
                    else
                    {
                        txtSellerCode.IsEnabled = true;
                        txtQuantity.IsEnabled = true;
                        txtItemType.IsEnabled = true;
                        txtWeight.IsEnabled = true;
                        txtBuyerCode.IsEnabled = mObjBatch.Unsold ? false : true;
                        txtPrice.IsEnabled = true;
                        txtItemType.IsEnabled = true;
                        cboReason.IsEnabled = true;
                        chkReprogrammed.IsEnabled = true;
                        chkUnsold.IsEnabled = true;

                        //Chips
                        chpBatch.Visibility = Visibility.Visible;
                        chpLog.Visibility = Visibility.Collapsed;
                    }
                    break;
                case FormModeEnum.READ:
                    // Buttons
                    btnLog.IsEnabled = true;
                    //Chips
                    chpBatch.Visibility = Visibility.Collapsed;
                    chpLog.Visibility = Visibility.Visible;
                    break;
                case FormModeEnum.DEFAULT:
                    chpBatch.Content = "";
                    chpLog.Content = "";
                    chpBatch.Visibility = Visibility.Collapsed;
                    chpLog.Visibility = Visibility.Collapsed;
                    break;
            }
            OnChangeFormMode(pEnmFormMode);
        }

        private TimeSpan GetPreClosureTime()
        {
            TimeSpan lObjTspn = TimeSpan.FromHours(Convert.ToInt32(QsConfig.GetValue<string>("PreClosureTime")));

            return lObjTspn;
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

        private void CalculateAmount()
        {
            if (!string.IsNullOrEmpty(txtPrice.Text) && !string.IsNullOrEmpty(txtWeight.Text))
            {
                double lDblWeightOrHeads = (bool)chkSellType.IsChecked ? Convert.ToDouble(txtQuantity.Text) : Convert.ToDouble(txtWeight.Text);
                double lDblPrice = Convert.ToDouble(txtPrice.Text);
                txtAmount.Text = (lDblWeightOrHeads * lDblPrice).ToString("C");
            }
        }

        private void LoadUnsoldsReasons()
        {
            cboReason.LoadDataSource<UnsoldMotiveEnum>();
        }

        private bool ValidCompleteBatch()
        {
            if (ValidRequiredFields() && ValidRequieredSellerAndItemType() && ValidRequieredBuyer())
            {
                bool lBoolChk = chkUnsold.IsChecked ?? false;

                if (ValidSellerAndBuyer(lBoolChk))
                {
                    if (ValidDifferentSellerAndBuyer(lBoolChk))
                    {
                        if (ValidAvailabilityStock() && ValidAvailabilityItemTypeStock())
                        {
                            return true;
                        }
                        else
                        {
                            CustomMessageBox.Show("Lote", "El vendedor no tiene suficiente Stock.", this.GetParent());
                        }
                    }
                    else
                    {
                        txtSellerCode.BorderBrush = Brushes.Red;
                        txtBuyerCode.BorderBrush = Brushes.Red;
                        txtBuyerCode.Focus();
                        CustomMessageBox.Show("Lote", "Vendedor y comprador no deben de ser el mismo.", this.GetParent());
                    }
                }
                else
                {
                    txtBuyerCode.BorderBrush = Brushes.Red;
                    txtBuyerCode.Focus();
                    CustomMessageBox.Show("Lote", "Agregue comprador", this.GetParent());
                }
            }
            else
            {
                CustomMessageBox.Show("Lote", "Favor de completar los campos.", this.GetParent());
            }
            return false;
        }

        private bool ValidRequiredFields()
        {
            return txtSellerCode.ValidRequired() &&
            txtSellerName.ValidRequired() &&
            txtTaxCode.ValidRequired() &&
            txtItemType.ValidRequired() &&
            txtQuantity.ValidRequired() &&
            txtWeight.ValidRequired() &&
            txtAverageWeight.ValidRequired() &&
            txtPrice.ValidRequired() &&
            txtAmount.ValidRequired() &&
            (
                chkUnsold.IsChecked == false &&
                txtBuyerCode.ValidRequired() &&
                txtBuyerName.ValidRequired() ||
                chkUnsold.IsChecked == true &&
                cboReason.ValidRequired()
            );
        }

        private bool ValidSellerAndBuyer(bool pchkUnsold)
        {
            if (mObjBuyer == null && pchkUnsold)
            {
                return true;
            }
            else if (mObjBuyer != null && !pchkUnsold)
            {
                return true;
            }

            return false;
            //var dd = (mObjSeller != null && (mObjBuyer != null && !pchkUnsold));
            //return (mObjSeller != null && (mObjBuyer != null && !pchkUnsold));
            //return (mObjSeller != null && mObjBuyer != null && mObjSeller.Id != mObjBuyer.Id || mObjSeller != null && mObjBuyer == null && pchkUnsold);
        }

        private bool ValidDifferentSellerAndBuyer(bool pChkUnsold)
        {

            if (mObjBuyer == null && pChkUnsold)
            {
                return true;
            }
            else if (!pChkUnsold && mObjSeller.Id != mObjBuyer.Id)
            {
                return true;
            }

            return false;
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


                //int lIntAvailabilityStock = !lBolChekced ?
                //mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForEditedBatchesInCurrentAuction(mObjBatch, mObjSeller.Id) :
                //mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogrammedEdited(mObjBatch, mObjSeller.Id);



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

                //int lIntAvailabilityStock = !lBolChekced ?
                //    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForCurrentAuction(mObjBatch, mObjSeller.Id, mObjItemType.Gender) :
                //    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogrammedEdited(mObjBatch, mObjSeller.Id, mObjItemType.Gender);

                int lIntAvailabilityStock = !lBolChekced ?
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender) :
                    mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, mObjSeller.Id, mObjItemType.Gender);

                //if (lIntAvailabilityStock >= lIntRequiredQuantity ||
                //    mObjBatch != null && mObjBatch.Id > 0 && (lIntAvailabilityStock + mObjBatch.Quantity) >= lIntRequiredQuantity)
                if (lIntAvailabilityStock >= lIntRequiredQuantity || mObjBatch != null && mObjBatch.Id > 0 && (lIntAvailabilityStock + mObjBatch.Quantity) >= lIntRequiredQuantity)
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

        private bool ValidRequieredSellerAndItemType()
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

        private bool ValidRequieredBuyer()
        {
            bool lBolResult = true;

            if (chkUnsold.IsChecked == false && mObjBuyer == null)
            {
                lBolResult = false;
                txtBuyerCode.BorderBrush = Brushes.Red;
            }

            return lBolResult;
        }

        #endregion

        #region Seller

        private void SetSellerObject(Partner pObjSeller, bool pBoolCheck = false)
        {
            try
            {


                if (pObjSeller != null)
                {
                    if (mObjBatch.Quantity == 0 || (pBoolCheck && ValidSellerStock(pObjSeller)))
                    {
                        mObjSeller = pObjSeller;
                        OnLoadSeller(pObjSeller);
                    }
                    else
                    {
                        mObjSeller = mObjTempSeller != null ? mObjTempSeller : pObjSeller;
                    }

                    txtSellerCode.Text = mObjSeller.Code;
                    txtSellerName.Text = mObjSeller.Name;
                    txtTaxCode.Text = mObjSeller.TaxCode;
                    btnStockDetails.IsEnabled = true;
                    mObjTempSeller = null;
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

        private bool ValidSellerStock(Partner pObjSeller)
        {
            int lIntAvailableStock = !mObjBatch.Reprogrammed ?
                mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(AuctionId, pObjSeller.Id, mObjBatch.ItemType.Gender) :
                mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(AuctionId, pObjSeller.Id, mObjBatch.ItemType.Gender);

            if (lIntAvailableStock >= mObjBatch.Quantity)
            {
                mBoolAbleToSave = true;
                return true;
            }
            else
            {
                mBoolAbleToSave = false;
                ShowMessage("Aviso", "El cliente seleccionado no cuenta con inventario suficiente, no se puede aplicar el cambio al lote");
                return false;
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

        #region Buyer

        private void SetBuyerClassificationObject(PartnerClassification pObjClassification)
        {
            try
            {
                mObjBuyerClassification = pObjClassification;
                mObjBuyer = pObjClassification != null ? pObjClassification.Customer : null;
                OnLoadBuyerClassification(pObjClassification);

                if (pObjClassification != null)
                {
                    txtBuyerCode.Text = pObjClassification.Number.ToString();
                    txtBuyerName.Text = pObjClassification.Customer.Name;
                    chkUnsold.Focus();
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
            txtBuyerCode.Text = string.Empty;
            txtBuyerName.Text = string.Empty;
        }

        #endregion

        #region ItemType

        private void SetItemTypeObject(ItemType pObjItemType, bool pBoolCheck = false)
        {
            try
            {


                if (pObjItemType != null)
                {
                    if (mObjBatch.Quantity == 0 || (pBoolCheck  && ValidItemSellType(pObjItemType)))
                    {
                        mObjItemType = pObjItemType;
                        OnLoadItemType(pObjItemType);
                    }
                    else
                    {

                        mObjItemType = mObjTempItemType != null ? mObjTempItemType : pObjItemType;

                    }

                    txtItemType.Text = mObjItemType.Name;
                    ValidAvailabilityItemTypeStock();
                    SetSellType(mObjItemType.SellType);
                    mObjTempItemType = null;
                }

                else
                {
                    ResetItemType();
                }

                txtPrice.Focus();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private bool ValidItemSellType(ItemType pObjItemType)
        {
            bool lBoolReturn = mObjBatch.SellType && pObjItemType.SellType == SellTypeEnum.Both || pObjItemType.SellType == SellTypeEnum.PerPrice ? true :
                !mObjBatch.SellType && pObjItemType.SellType == SellTypeEnum.Both || pObjItemType.SellType == SellTypeEnum.PerWeight ? true : false;

            if (!lBoolReturn)
            {
                ShowMessage("Aviso", "La categoria seleccionada no es del tipo de venta con la que se realizo el lote");
            }
            return lBoolReturn;
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
                    if (mObjBatch != null && mObjBatch.SellType)
                    {
                        chkSellType.Content = "Por Precio";
                        chkSellType.IsChecked = true;
                    }
                    else
                    {
                        chkSellType.Content = "Por Kilo";
                        chkSellType.IsChecked = false;
                    }

                    chkSellType.IsEnabled = true;
                    break;
            }
        }

        private void SetPerPriceConfig()
        {
            txtPrice.IsEnabled = false;
            txtAmount.IsEnabled = true;
        }

        private void ResetItemType()
        {
            txtItemType.Text = string.Empty;
        }

        #endregion

        #region Batch

        public void SetBatchObject(Batch pObjBatch,bool pBoolEdition = false)
        {
            mObjBatch = pObjBatch;
            mBoolEdition = true;
            mObjInternalWorker = new Thread(() => InternalSetBatchObject(pObjBatch));
            mObjInternalWorker.Start();
        }

        private void ResetBatch()
        {
            grdRightBatch.ClearControl();
            SetSellerObject(null);
            SetBuyerClassificationObject(null);
            SetItemTypeObject(null);
            chkReprogrammed.IsChecked = false;
            chkUnsold.IsChecked = false;
            txtWeight.IsEnabled = false;
            LoadUnsoldsReasons();
            cboReason.IsEnabled = false;
            UpdateFormMode(FormModeEnum.DEFAULT);

        }

        public Batch GetBatchObject()
        {
            try
            
            {

                Batch lObjBatch = new Batch();

                //Required fields
                lObjBatch.Id = mObjBatch != null ? mObjBatch.Id : 0;
                lObjBatch.Number = mObjBatch != null ? mObjBatch.Number : BatchNumber;
                lObjBatch.AuctionId = AuctionId;
                lObjBatch.SellerId = mObjSeller.Id;
                lObjBatch.ItemTypeId = mObjItemType.Id;
                lObjBatch.Reprogrammed = chkReprogrammed.IsChecked ?? false;

                //Quantities 
                lObjBatch.Quantity = !string.IsNullOrEmpty(txtQuantity.Text) ? Convert.ToInt32(txtQuantity.Text) : 0;
                lObjBatch.Weight = !string.IsNullOrEmpty(txtWeight.Text) ? float.Parse(txtWeight.Text) : 0;
                lObjBatch.Price = !string.IsNullOrEmpty(txtPrice.Text) ? Convert.ToDecimal(txtPrice.Text) : 0;
                lObjBatch.Amount = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Parse(txtAmount.Text, NumberStyles.Currency) : 0;
                lObjBatch.AverageWeight = !string.IsNullOrEmpty(txtAverageWeight.Text) ? Convert.ToSingle(txtAverageWeight.Text) : 0;

                //Casted fields
                lObjBatch.BuyerClassificationId = mObjBuyerClassification != null ? (long?)mObjBuyerClassification.Id : null;
                lObjBatch.BuyerId = mObjBuyer != null ? (long?)mObjBuyer.Id : null;
                lObjBatch.Unsold = (bool)chkUnsold.IsChecked;

                lObjBatch.UnsoldMotive = cboReason.SelectedItem != null ? (UnsoldMotiveEnum)((EnumDTO)cboReason.SelectedItem).Value : 0;

                lObjBatch.SellType = (bool)chkSellType.IsChecked;

                //if (lObjBatch.Unsold) { lObjBatch.UnsoldMotive = (UnsoldMotiveEnum)((EnumDTO)cboReason.SelectedItem).Value; }

                //Optional fields
                lObjBatch.Price = lObjBatch.Price == 0 && mObjBatch != null ?
                    mObjBatch.Price : lObjBatch.Price != 0 ? lObjBatch.Price : 0;

                lObjBatch.Amount = lObjBatch.Amount == 0 && mObjBatch != null ?
                    mObjBatch.Amount : lObjBatch.Amount != 0 ? lObjBatch.Amount : 0;

                lObjBatch.Gender = mObjItemType.Gender.GetDescription();

                return lObjBatch;
            }
            catch
            {
                return null;
            }
        }

        public bool Save()
        {
            try
            {
                if (mObjBatch.Quantity > 0)
                {
                    if (ValidAviabilityEditedStock())
                    {
                        mBoolAbleToSave = true;
                    }
                    else
                    {
                        mBoolAbleToSave = false;
                    }
                }
                else
                {
                    if (ValidCompleteBatch())
                    {
                        mBoolAbleToSave = true;
                    }
                    else
                    {
                        mBoolAbleToSave = false;
                    }
                }

                bool lBoolUnsold = chkUnsold.IsChecked ?? false;

                if (ValidSellerAndBuyer(lBoolUnsold))
                {
                    if (ValidDifferentSellerAndBuyer(lBoolUnsold))
                    {
                        if (mBoolAbleToSave || (mBoolAbleToSave && (lBoolUnsold && cboReason.ValidRequired())))
                        {
                            Batch lObjBatch = GetBatchObject();
                            mObjInternalWorker = new Thread(() => InternalSave(lObjBatch));
                            mObjInternalWorker.Start();
                            return true;
                        }
                    }
                    else
                    {
                        txtSellerCode.BorderBrush = Brushes.Red;
                        txtBuyerCode.BorderBrush = Brushes.Red;
                        txtBuyerCode.Focus();
                        CustomMessageBox.Show("Lote", "Vendedor y comprador no deben de ser el mismo.", this.GetParent());
                    }
                }
                else
                {
                    txtBuyerCode.BorderBrush = Brushes.Red;
                    txtBuyerCode.Focus();
                    CustomMessageBox.Show("Lote", "Agregue comprador", this.GetParent());
                }

            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
            }
            return false;
        }

        //public bool Save()
        //{
        //    try
        //    {
        //        if (mBoolEdition)
        //        {
        //            if (ValidAviabilityEditedStock())
        //            {
        //                mBoolAbleToSave = true;
        //            }
        //            else
        //            {
        //                mBoolAbleToSave = false;
        //            }
        //        }
        //        else
        //        {
        //            mBoolAbleToSave = true;
        //        }

        //        bool lBoolUnsold = chkUnsold.IsChecked ?? false;

        //        if (ValidSellerAndBuyer(lBoolUnsold))
        //        {
        //            if (ValidDifferentSellerAndBuyer(lBoolUnsold))
        //            {
        //                if ((mBoolAbleToSave && (lBoolUnsold && cboReason.ValidRequired())) || ValidCompleteBatch())
        //                {
        //                    Batch lObjBatch = GetBatchObject();
        //                    mObjInternalWorker = new Thread(() => InternalSave(lObjBatch));
        //                    mObjInternalWorker.Start();
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                txtSellerCode.BorderBrush = Brushes.Red;
        //                txtBuyerCode.BorderBrush = Brushes.Red;
        //                txtBuyerCode.Focus();
        //                CustomMessageBox.Show("Lote", "Vendedor y comprador no deben de ser el mismo.", this.GetParent());
        //            }
        //        }
        //        else
        //        {
        //            txtBuyerCode.BorderBrush = Brushes.Red;
        //            txtBuyerCode.Focus();
        //            CustomMessageBox.Show("Lote", "Agregue comprador", this.GetParent());
        //        }

        //    }
        //    catch (Exception lObjException)
        //    {
        //        ShowMessage("Error", lObjException.Message);
        //    }
        //    return false;
        //}



        public void Print()
        {
            if (ValidRequiredFields() && ValidRequieredSellerAndItemType())
            {
                InternalPrint(GetBatchObject());
            }
        }

        public void Print(Batch pObjBatch)
        {
            InternalPrint(pObjBatch);
        }

        private void InternalPrint(Batch pObjBatch)
        {
            try
            {
                UserControl lUCPrint = new UCPrintBatch(pObjBatch);
            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
            }
        }

        public bool Skip()
        {
            try
            {
                return ShowSkipDialog() ?? false;
            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
                return false;
            }
        }

        public bool IsSkipped(Batch pObjBatch)
        {
            return pObjBatch != null ? pObjBatch.SellerId == null && pObjBatch.ItemTypeId == null && pObjBatch.Quantity == 0 : false;
        }

        public bool Undo()
        {
            try
            {
                OnUndoBatch(GetBatchObject());
                SetBatchObject(null);
                return true;
            }
            catch (Exception lObjException)
            {
                ShowMessage("Error", lObjException.Message);
                return false;
            }
        }

        private bool? ShowSkipDialog()
        {
            UCSkipBatch lObjSkip = new UCSkipBatch(AuctionId);
            BaseForm lObjBaseForm = new BaseForm();
            lObjBaseForm.tblTitle.Text = "Saltar lote";
            lObjBaseForm.Owner = this.GetParent();
            lObjBaseForm.grdContainer.Children.Add(lObjSkip);
            lObjBaseForm.Width = 300;
            lObjBaseForm.SizeToContent = System.Windows.SizeToContent.Height;
            lObjBaseForm.ResizeMode = ResizeMode.NoResize;
            lObjSkip.txtBatch.Focus();
            return lObjBaseForm.ShowDialog();
        }

        #endregion

        #region Internal

        private void InternalSetBatchObject(Batch pObjBatch)
        {
            grdRightBatch.BlockUI();
            Thread.Sleep(300);

            try
            {
                //mObjBatch = pObjBatch;
                InvokeOnLoadBatch(pObjBatch);

                if (pObjBatch != null)
                {
                    Partner lObjSeller = pObjBatch.SellerId != null ? mObjBusinessFactory.GetPartnerService().GetEntity(pObjBatch.SellerId ?? 0) : null;
                    Partner lObjBuyer = pObjBatch.BuyerId != null ? mObjBusinessFactory.GetPartnerService().GetEntity(pObjBatch.BuyerId ?? 0) : null;
                    PartnerClassification lObjClassification = pObjBatch.BuyerClassificationId != null ? mObjBusinessFactory.GetPartnerClassificationService().GetEntity(pObjBatch.BuyerClassificationId ?? 0) : null;
                    //ItemType lObjItemType = pObjBatch.ItemTypeId != null ? mObjInventoryFactory.GetItemTypeService().GetEntity(pObjBatch.ItemTypeId ?? 0) : null;
                    ItemType lObjItemType = pObjBatch.ItemType != null ? pObjBatch.ItemType : pObjBatch.ItemTypeId != null ? mObjInventoryFactory.GetItemTypeService().GetEntity(pObjBatch.ItemTypeId ?? 0) : null;

                    InvokeSetBatchObject(pObjBatch, lObjSeller, lObjBuyer, lObjClassification, lObjItemType);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (IsSkipped(pObjBatch))
                        {
                            //Fields
                            txtWeight.IsEnabled = true;
                            txtPrice.IsEnabled = true;
                            txtBuyerCode.IsEnabled = true;
                            // Buttons
                            btnChangeWeight.IsEnabled = false;
                            btnChangePrice.IsEnabled = false;
                            btnChangeBuyer.IsEnabled = false;
                            btnChangeSeller.IsEnabled = false;
                            btnChangeQuantity.IsEnabled = false;
                            btnSold.IsEnabled = false;
                            btnReprogrammated.IsEnabled = false;
                        }
                    });
                }
                else
                {
                    InvokeResetBatch();
                }
            }
            catch (Exception lObjException)
            {
                grdRightBatch.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdRightBatch.UnblockUI();
            }
        }

        private void InternalSave(Batch pObjBatch)
        {
            grdRightBatch.BlockUI();

            try
            {
                mObjAuctionsFactory.GetBatchAuctionService().SaveOrUpdateBatch(pObjBatch);

                //Set buyer to show in display2
                pObjBatch.Buyer = mObjBuyer;

                this.Dispatcher.Invoke((Action)delegate
                {
                    OnSaveBatch(pObjBatch);
                    ResetBatch();
                });
            }
            catch (Exception lObjException)
            {
                grdRightBatch.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdRightBatch.UnblockUI();
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

        private void InvokeSetBatchObject(Batch pObjBatch, Partner pObjSeller, Partner pObjBuyer, PartnerClassification pObjClassification, ItemType pObjItemType)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                SetSellerObject(pObjSeller);
                SetBuyerClassificationObject(pObjClassification);
                SetItemTypeObject(pObjItemType);
                chkReprogrammed.IsChecked = pObjBatch.Reprogrammed;
                txtQuantity.Text = pObjBatch.Quantity.ToString();
                txtWeight.Text = pObjBatch.Weight.ToString();
                txtAverageWeight.Text = pObjBatch.AverageWeight.ToString();
                chkSellType.IsChecked = pObjBatch.SellType;

                if (pObjBatch.Price > 0) { txtPrice.Text = pObjBatch.Price.ToString(); }
                if (pObjBatch.Amount > 0) { txtAmount.Text = pObjBatch.Amount.ToString("C"); }

                chkUnsold.IsChecked = pObjBatch.Unsold;
                //cboReason.Visibility = pObjBatch.Unsold ? Visibility.Visible : Visibility.Collapsed;
                cboReason.Visibility = Visibility.Visible;

                if (pObjBatch.UnsoldMotive > 0) { cboReason.SelectValue((int)pObjBatch.UnsoldMotive); }
                if (!pObjBatch.Weight.Equals(0))
                {
                    txtPrice.Focus();
                }
                else
                {
                    txtWeight.IsEnabled = true;
                    txtWeight.Focus();
                }
                chpBatch.Content = string.Format("Lote {0}", pObjBatch.Number);
            });
        }

        private void InvokeResetBatch()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdRightBatch.ClearControl();
                SetSellerObject(null);
                SetBuyerClassificationObject(null);
                SetItemTypeObject(null);
                chkReprogrammed.IsChecked = false;
                chkUnsold.IsChecked = false;
                txtWeight.IsEnabled = false;
                LoadUnsoldsReasons();
                cboReason.Visibility = Visibility.Collapsed;
                chpBatch.Content = "";
                chpBatch.Visibility = Visibility.Collapsed;
                UpdateFormMode(FormModeEnum.DEFAULT);
            });
        }

        #endregion



        #endregion






    }
}