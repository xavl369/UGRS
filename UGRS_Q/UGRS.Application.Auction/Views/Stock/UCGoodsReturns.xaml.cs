using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Extension.Enum;
using UGRS.Data.Auctions.Factories;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Utility;
using System.Collections.Generic;
using System.Linq;

namespace UGRS.Application.Auctions
{
    public partial class UCGoodsReturns : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsServicesFactory;
        private InventoryServicesFactory mObjInventoryServicesFactory;
        private BusinessServicesFactory mObjBusinessServicesFactory;

        private Auction mObjAuction;
        private Batch mObjBatch;
        private DetailedBatchDTO mObjDetailedBatch;
        private Thread mObjWorker;

        private int mIntAvailableQtty = 0;
        private int mIntAvailableDeliveredQtty = 0;

        private float mFlAviableWeight = 0;
        private float mFlAviableDeliveredWeight = 0;

        private float mFlAverageWeight = 0;
        private float mFlAverageDeliveredWeight = 0;
        #endregion

        #region Contructor

        public UCGoodsReturns()
        {
            mObjAuctionsServicesFactory = new AuctionsServicesFactory();
            mObjInventoryServicesFactory = new InventoryServicesFactory();
            mObjBusinessServicesFactory = new BusinessServicesFactory();
            InitializeComponent();

        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (mObjAuctionsServicesFactory.GetAuctionService().GetReopenedActiveAuction() == null)
            {
                mObjWorker = new Thread(() => LoadDefaultAuction());
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

        private void txtAuction_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    SetControlsAuction(this.ShowAuctionChooseDialog(pObjSender, pObjArgs, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION));
                }
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private void txtAuction_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (string.IsNullOrEmpty((pObjSender as TextBox).Text))
            {
                SetControlsAuction(null);
            }
        }

        private void txtBatch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    SetControlsBatch(this.ShowBatchChooseDialog(pObjSender, pObjArgs, mObjAuction != null ? mObjAuction.Id : 0));
                }
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private void txtBatch_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (string.IsNullOrEmpty((pObjSender as TextBox).Text))
            {
                SetControlsBatch(null);
            }
        }

        private void txtQuantityToReturn_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
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

        private void txtQuantityToReturn_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            UpdateQuantityToPick(pObjSender as TextBox);
        }

        private void txtWeightToReturn_PreviewTextInput(object pObjSender, TextCompositionEventArgs pObjArgs)
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

        private void txtWeightToReturn_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtWeightToReturn.Text))
            {
                if (IsDelivered())
                {
                    if (float.Parse(txtWeightToReturn.Text) > mFlAviableDeliveredWeight)
                    {
                        txtWeightToReturn.Text = "";
                        txtWeightToReturn.BorderBrush = Brushes.Red;
                        this.ShowMessage("Devolución de ganado", "No puede devolver un peso mayor a el total disponible.");
                    }
                    else
                    {
                        txtWeightToReturn.BorderBrush = Brushes.Black;
                    }
                }
                else
                {
                    if (float.Parse(txtWeightToReturn.Text) > mFlAviableWeight)
                    {
                        txtWeightToReturn.Text = "";
                        txtWeightToReturn.BorderBrush = Brushes.Red;
                        this.ShowMessage("Devolución de ganado", "No puede devolver un peso mayor a el total disponible.");
                    }
                    else
                    {
                        txtWeightToReturn.BorderBrush = Brushes.Black;
                    }
                }
            }
        }

        #endregion

        #region ToggleButton

        private void tbnDelivered_Checked(object sender, RoutedEventArgs e)
        {
            if (mObjDetailedBatch != null)
            {
                //txtAvailableBatchQuantity.Text = mObjDetailedBatch.AvailableQuantityToReturnDelivery.ToString();
                txtAvailableBatchQuantity.Text = mIntAvailableDeliveredQtty.ToString();
                txtAviableWeight.Text = mFlAviableDeliveredWeight.ToString();
                txtAverageWeight.Text = mFlAverageDeliveredWeight.ToString();
                mObjDetailedBatch.Delivered = true;
            }
        }

        private void tbnDelivered_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mObjDetailedBatch != null)
            {
                //txtAvailableBatchQuantity.Text = mObjDetailedBatch.AvailableQuantityToReturn.ToString();
                txtAvailableBatchQuantity.Text = mIntAvailableQtty.ToString();
                txtAviableWeight.Text = mFlAviableWeight.ToString();
                txtAverageWeight.Text = mFlAverageWeight.ToString();
                mObjDetailedBatch.Delivered = false;
            }
        }

        #endregion

        #region Button

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => SaveGoodsReturns());
            mObjWorker.Start();
        }

        #endregion

        #endregion

        #region Methods

        private void LoadUnsoldsReasons()
        {
            cboReason.LoadDataSource<UnsoldMotiveEnum>();
        }

        private void LoadDefaultAuction()
        {
            FormLoading();
            try
            {
                Auction lObjAuction = mObjAuctionsServicesFactory.GetAuctionService().GetCurrentOrLast(AuctionCategoryEnum.AUCTION);
                this.Dispatcher.Invoke(() =>
                {
                    SetControlsAuction(lObjAuction);
                });
            }
            catch (Exception lObjException)
            {
                FormDefault();
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefault();
            }
        }

        private void SetControlsAuction(Auction pObjAuction)
        {
            if (pObjAuction != null)
            {
                txtAuction.Text = pObjAuction.Folio;
                txtDate.Text = pObjAuction.Date.ToShortDateString();
                txtType.Text = pObjAuction.Type.GetDescription();
                mObjAuction = pObjAuction;
                LoadUnsoldsReasons();
            }
            else
            {
                txtAuction.Text = "";
                txtDate.Text = "";
                txtType.Text = "";
                mObjAuction = null;
            }
            SetControlsBatch(null);
        }

        private void SetControlsBatch(Batch pObjBatch)
        {
            if (pObjBatch != null)
            {
                mObjAuction = mObjAuctionsServicesFactory.GetAuctionService().GetActiveAuction();
                mIntAvailableQtty = GetQuantityToReturn(pObjBatch);//GetAvailableQttyToReturn(pObjBatch) - GetDifference(pObjBatch);
                mIntAvailableDeliveredQtty = GetDeliveredQtty(pObjBatch);//- GetDeliveredDifference(pObjBatch);
                mFlAviableWeight = GetAvailableWeight(pObjBatch);
                mFlAviableDeliveredWeight = GetAviableDeliveredWeight(pObjBatch);
                mFlAverageWeight = GetAverageWeight(pObjBatch);
                mFlAverageDeliveredWeight = GetAverageDeliveredWeight(pObjBatch);

                txtBatch.Text = pObjBatch.Number.ToString();
                txtSeller.Text = pObjBatch.Seller != null ? pObjBatch.Seller.Name : "";
                txtBuyer.Text = pObjBatch.Buyer != null ? pObjBatch.Buyer.Name : "";
                txtItemType.Text = pObjBatch.ItemType != null ? pObjBatch.ItemType.Name : "";
                txtBatchQuantity.Text = pObjBatch.Quantity.ToString();
                txtTotalWeight.Text = pObjBatch.Weight.ToString();
                txtAverageWeight.Text = mFlAverageWeight.ToString(); //pObjBatch.AverageWeight.ToString();
                txtQuantityToReturn.Text = "";
                txtWeightToReturn.Text = "";
                mObjBatch = pObjBatch;
                mObjDetailedBatch = new DetailedBatchDTO(pObjBatch);
                txtAvailableBatchQuantity.Text = mIntAvailableQtty.ToString();//mObjDetailedBatch.AvailableQuantityToReturn.ToString();
                txtAviableWeight.Text = mFlAviableWeight.ToString();
                tbnDelivered.IsChecked = false;
                txtQuantityToReturn.IsEnabled = true;
                tbnDelivered.IsEnabled = true;
                cboReason.SelectedIndex = -1;
                cboReason.IsEnabled = true;
            }
            else
            {
                txtBatch.Text = "";
                txtSeller.Text = "";
                txtBuyer.Text = "";
                txtItemType.Text = "";
                txtBatchQuantity.Text = "";
                txtTotalWeight.Text = "";
                txtAverageWeight.Text = "";
                txtQuantityToReturn.Text = "";
                txtWeightToReturn.Text = "";
                txtAviableWeight.Text = "";
                mObjBatch = null;
                mObjDetailedBatch = null;
                txtAvailableBatchQuantity.Text = "";
                tbnDelivered.IsChecked = false;
                txtQuantityToReturn.IsEnabled = false;
                tbnDelivered.IsEnabled = false;
                cboReason.SelectedIndex = -1;
                cboReason.IsEnabled = false;
                

                mIntAvailableQtty = 0;
                mIntAvailableDeliveredQtty = 0;
            }
        }




        private float GetAverageDeliveredWeight(Batch pObjBatch)
        {
            return mIntAvailableDeliveredQtty > 0 ? mFlAviableDeliveredWeight / ((mIntAvailableQtty + mIntAvailableDeliveredQtty) > pObjBatch.Quantity ? pObjBatch.Quantity : mIntAvailableDeliveredQtty) : 0;
        }

        private float GetAverageWeight(Batch pObjBatch)
        {
            return mIntAvailableQtty > 0 ? mFlAviableWeight / ((mIntAvailableQtty + mIntAvailableDeliveredQtty) > pObjBatch.Quantity ? pObjBatch.Quantity : mIntAvailableQtty) : 0;
        }


        private float GetAvailableWeight(Batch pObjBatch)
        {
            return pObjBatch.Weight - pObjBatch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Weight);
        }

        private float GetAviableDeliveredWeight(Batch pObjBatch)
        {
            float lFlActualWeight = pObjBatch.Weight - pObjBatch.GoodsReturns.Where(x =>!x.Removed).Sum(x => x.Weight);

            return mIntAvailableDeliveredQtty > 0 ? lFlActualWeight : 0;
        }


        private int GetQuantityToReturn(Batch pObjBatch)
        {
            //int lIntTotalQtty = mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //    && x.ItemType.Gender == pObjBatch.ItemType.Gender).Sum(x => x.Quantity) - (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //    && x.ItemTypeId == pObjBatch.ItemTypeId).SelectMany(x => x.GoodsIssues).Where(x => !x.Removed).Sum(x => x.Quantity)) -
            //    pObjBatch.GoodsReturns.Where(x=>!x.Removed).Sum(x => x.Quantity);
            //   // (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //   //&& x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsReturns).Where(x => !x.Removed).Sum(x => x.Quantity));

            //int lIntActualBatchQtty =  pObjBatch.Quantity - pObjBatch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Quantity);

            //int lIntQtty = lIntActualBatchQtty <= lIntTotalQtty ? lIntActualBatchQtty : lIntTotalQtty;

            //return lIntQtty;

            int lIntExistence =  mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
                && x.ItemType.Gender == pObjBatch.ItemType.Gender).Sum(x => x.Quantity) - (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
                && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsIssues).Where(x => !x.Removed).Sum(x => x.Quantity)) -
                (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
               && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsReturns).Where(x => !x.Removed && !x.Delivered).Sum(x => x.Quantity));
               
            int lIntRetDel = pObjBatch.GoodsReturns.Where(x => !x.Removed && x.Delivered).Sum(x => x.Quantity);

            int lIntRet = pObjBatch.GoodsReturns.Where(x => !x.Removed && !x.Delivered).Sum(x => x.Quantity);

            int lIntTotal = pObjBatch.Quantity - lIntRetDel - lIntRet;

            return lIntExistence > 0 ? lIntTotal <= lIntExistence ? lIntTotal : lIntExistence : 0;

        }


        private int GetDeliveredQtty(Batch pObjBatch)
        {
            //int lint = mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //   && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsIssues).Where(x => !x.Removed).Sum(x => x.Quantity);

            //int lIntTotaDeliveredlQtty = mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //   && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsIssues).Where(x => !x.Removed).Sum(x => x.Quantity) -
            //   pObjBatch.GoodsReturns.Where(x => !x.Removed && x.Delivered).Sum(x => x.Quantity);
            //   // (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
            //   //&& x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsReturns).Where(x => !x.Removed).Sum(x => x.Quantity));

            //int lIntActualBatchQtty = pObjBatch.Quantity - pObjBatch.GoodsReturns.Where(x => !x.Removed && x.Delivered).Sum(x => x.Quantity);

            //return lIntActualBatchQtty <= lIntTotaDeliveredlQtty ? lIntActualBatchQtty : lIntTotaDeliveredlQtty;

            int lIntGoodIssues = mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
                && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsIssues).Where(x => !x.Removed).Sum(x => x.Quantity)
                - (mObjAuction.Batches.Where(x => !x.Unsold && x.BuyerId == pObjBatch.BuyerId
               && x.ItemType.Gender == pObjBatch.ItemType.Gender).SelectMany(x => x.GoodsReturns).Where(x => !x.Removed && x.Delivered).Sum(x => x.Quantity));

            int lIntRetDel = pObjBatch.GoodsReturns.Where(x => !x.Removed && x.Delivered).Sum(x => x.Quantity);

            int lIntRet = pObjBatch.GoodsReturns.Where(x => !x.Removed && !x.Delivered).Sum(x => x.Quantity);

            int lIntTotal = pObjBatch.Quantity - lIntRetDel - lIntRet;

            return lIntGoodIssues > 0 ? lIntTotal <= lIntGoodIssues ? lIntTotal : lIntGoodIssues : 0;
        }


        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private void SaveGoodsReturns()
        {
            FormLoading();
            try
            {
                if (ValidForm())
                {
                    SetReturnMotive();
                    SetWeights();
                    mObjInventoryServicesFactory.GetGoodsReturnService().CreateGoodsReturn(mObjDetailedBatch, mIntAvailableQtty, mIntAvailableDeliveredQtty);
                    this.ShowMessage("Devolución de ganado", "Devolución de ganado creada con éxito.");

                }
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                this.FormDefault();

            }
        }

        private void SetWeights()
        {
            this.Dispatcher.Invoke(() =>
            {
                mObjDetailedBatch.Weight = float.Parse(txtWeightToReturn.Text);
            });
        }

        private void SetReturnMotive()
        {
            this.Dispatcher.Invoke(() =>
           {
               mObjDetailedBatch.ReturnMotive = (UnsoldMotiveEnum)cboReason.SelectedValue;
           });

        }

        private bool ValidForm()
        {
            bool lBolValid = true;

            if (string.IsNullOrEmpty(GetBatchText()))
            {
                this.ShowMessage("Devolución de ganado", "Favor de seleccionar un lote.");
                return false;
            }

            if (string.IsNullOrEmpty(GetQuantityToReturnText()))
            {
                this.ShowMessage("Devolución de ganado", "Favor de ingresar la cantidad a devolver.");
                return false;
            }
            else
            {
                int lIntQtyToReturn = Convert.ToInt32(GetQuantityToReturnText());
                if ((IsDelivered() ? mIntAvailableDeliveredQtty : mIntAvailableQtty) < lIntQtyToReturn)
                {
                    this.ShowMessage("Devolución de ganado", "No puede devolver una cantidad mayor a la disponible.");
                    return false;
                }
            }

            if (!CboSelection())
            {
                this.ShowMessage("Devolución de ganado", "Seleccione un motivo para la devolucion");
                return false;
            }
            //if(mObjDetailedBatch.Lines.Count == 0)
            //{
            //    this.ShowMessage("Devolución de ganado", "El lote no cuenta con líneas.");
            //    return false;
            //}

            return lBolValid;
        }

        private bool CboSelection()
        {
            return (bool)this.Dispatcher.Invoke(new Func<bool>(() =>
            {
                return cboReason.SelectedIndex >= 0 ? true : false;
            }));
        }


        private string GetBatchText()
        {
            return (string)this.Dispatcher.Invoke(new Func<string>(() =>
            {
                return txtBatch.Text;
            }));
        }

        private string GetQuantityToReturnText()
        {
            return (string)this.Dispatcher.Invoke(new Func<string>(() =>
            {
                return txtQuantityToReturn.Text;
            }));
        }

        private bool IsDelivered()
        {
            return (bool)this.Dispatcher.Invoke(new Func<bool>(() =>
            {
                return tbnDelivered.IsChecked ?? false;
            }));
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdGoodsReturnForm.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdGoodsReturnForm.UnblockUI();
                SetInitialForm();
            });
        }

        private void SetInitialForm()
        {
            txtBatch.Text = string.Empty;
        }

        private void UpdateQuantityToPick(TextBox pObjTextBox)
        {
            try
            {
                int lObjQuantityToPick = !string.IsNullOrEmpty(pObjTextBox.Text) ? Convert.ToInt32(pObjTextBox.Text) : 0;

                if (lObjQuantityToPick >= 0)
                {
                    if (BatchRealQuantity(mObjBatch) >= lObjQuantityToPick)
                    {
                        float lFlWeight = IsDelivered() ? mFlAverageDeliveredWeight : mFlAverageWeight;

                        if ((IsDelivered() ? mIntAvailableDeliveredQtty : mIntAvailableQtty) >= lObjQuantityToPick)
                        {
                            mObjDetailedBatch.QuantityToPick = lObjQuantityToPick;
                            txtWeightToReturn.Text = (lFlWeight * lObjQuantityToPick).ToString();
                            pObjTextBox.BorderBrush = Brushes.Black;
                        }
                        else
                        {
                            mObjDetailedBatch.QuantityToPick = 0;
                            txtWeightToReturn.Text = "";
                            txtQuantityToReturn.Text = "";
                            txtAverageWeight.Text = "";
                            pObjTextBox.BorderBrush = Brushes.Red;
                            this.ShowMessage("Devolución de ganado", "No puede devolver una cantidad mayor a el total disponible.");
                        }
                    }
                    else
                    {
                        mObjDetailedBatch.QuantityToPick = 0;
                        txtWeightToReturn.Text = "";
                        txtQuantityToReturn.Text = "";
                        txtAverageWeight.Text = "";
                        pObjTextBox.BorderBrush = Brushes.Red;
                        this.ShowMessage("Devolución de ganado", "No puede devolver una cantidad mayor a la disponible en el lote");
                    }
                }
                else
                {
                    mObjDetailedBatch.QuantityToPick = 0;
                    txtWeightToReturn.Text = "";
                    txtQuantityToReturn.Text = "";
                    txtAverageWeight.Text = "";
                    pObjTextBox.BorderBrush = Brushes.Red;
                    this.ShowMessage("Devolución de ganado", "Ingrese una cantidad mayor a 0.");
                }
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private int BatchRealQuantity(Batch pObjBatch)
        {
            int lIntGoodReturns = pObjBatch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Quantity);

            return pObjBatch.Quantity - lIntGoodReturns;
        }

        private void UpdateWeightToPick(TextBox pObjTextBox)
        {
            try
            {
                float lFltWeightToPick = !string.IsNullOrEmpty(pObjTextBox.Text) ? float.Parse(pObjTextBox.Text) : 0;
                int lIntQuantityToPick = !string.IsNullOrEmpty(txtQuantityToReturn.Text) ? Convert.ToInt32(txtQuantityToReturn.Text) : 0;

                if (lFltWeightToPick > 0 && lIntQuantityToPick > 0)
                {
                    if ((IsDelivered() ?
                            (mObjDetailedBatch.AvailableWeightToReturnDelivery - (mObjDetailedBatch.AvailableQuantityToReturnDelivery - lIntQuantityToPick)) :
                            (mObjDetailedBatch.AvailableWeightToReturn - (mObjDetailedBatch.AvailableQuantityToReturn - lIntQuantityToPick)))
                        >= lFltWeightToPick)
                    {
                        pObjTextBox.BorderBrush = Brushes.Black;
                    }
                    else
                    {
                        mObjDetailedBatch.QuantityToPick = 0;
                        txtWeightToReturn.Text = "";
                        txtQuantityToReturn.Text = "";
                        pObjTextBox.BorderBrush = Brushes.Red;
                        this.ShowMessage("Devolución de ganado", "No puede devolver un peso mayor al disponible.");
                    }
                }
                else
                {
                    mObjDetailedBatch.QuantityToPick = 0;
                    txtWeightToReturn.Text = "";
                    txtQuantityToReturn.Text = "";
                    pObjTextBox.BorderBrush = Brushes.Red;
                    this.ShowMessage("Devolución de ganado", "Ingrese una cantidad mayor a 0.");
                }
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        #endregion
    }
}
