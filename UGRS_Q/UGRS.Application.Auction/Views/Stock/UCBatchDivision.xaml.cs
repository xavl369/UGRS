using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Auctions.DTO.Business;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Application.Auctions.Extensions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCBatchDivision.xaml
    /// </summary>
    public partial class UCBatchDivision : UserControl
    {
        #region Attributes
        private AuctionsServicesFactory mObjAuctionsFactory = new AuctionsServicesFactory();
        private InventoryServicesFactory mObjInventoryServicesFactory = new InventoryServicesFactory();
        private BusinessServicesFactory mObjBusinessFactory = new BusinessServicesFactory();

        private PartnerClassification mObjBuyerClassification = new PartnerClassification();

        private ListCollectionView mLcvListData = null; //Filtros
        private List<BatchDivisionDTO> mLstObjBatchDivision = null;

        private int mIntHeadQuantity = 0;
        private float mFlWeight = 0;
        private bool mBoolSetWeight = false;
        private int mIntPrevBatchNumber = 0;

        private Auction mObjAuction;
        private Batch mObjBatch;
        private Thread mObjWorker;
        #endregion

        #region Constructor
        public UCBatchDivision()
        {
            InitializeComponent();
        }
        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (mObjAuctionsFactory.GetAuctionService().GetReopenedActiveAuction() == null)
            {
                SetControlsAuction(mObjAuctionsFactory.GetAuctionService().GetActiveAuction());

                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();
            }
            else
            {
                ShowMessage("Atención", "No se pueden realizar movimientos en subastas abiertas por segunda vez");
                this.CloseInternalForm();
            }
        }


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
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtBatch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Batch> lLstObjBatches = mObjAuctionsFactory.GetBatchService().SearchSoldBatches(lStrText, mObjAuction.Id);

                    if (lLstObjBatches.Count == 1)
                    {
                        SetBatch(lLstObjBatches[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCSearchBatch = new UCSearchBatch(lStrText, lLstObjBatches, mObjAuction.Id);
                        SetBatch(FunctionsUI.ShowWindowDialog(lUCSearchBatch, this.GetParent()) as Batch);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void txtBuyerClassification_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (mObjBatch != null)
                {
                    if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                    {
                        string lStrText = (pObjSender as TextBox).Text;
                        List<PartnerClassification> lLstObjClassifications = mObjBusinessFactory.GetPartnerClassificationService().SearchPartner(lStrText, FilterEnum.ACTIVE).Where(x => x.Id != mObjBatch.BuyerClassificationId).ToList();

                        if (lLstObjClassifications.Count == 1)
                        {
                            SetBuyerClassificationObject(lLstObjClassifications[0], pObjSender as TextBox);
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
                                ),
                                pObjSender as TextBox
                            );
                            //(pObjSender as TextBox).Focus();
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("Error", "Debe seleccionar un lote para dividirlo", this.GetParent());
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void SetBuyerClassificationObject(PartnerClassification partnerClassification, TextBox pObjTextbox)
        {

            if (partnerClassification != null)
            {
                BatchDivisionDTO lObjBatchDivisionDTO = (BatchDivisionDTO)dgBatch.CurrentItem;
                lObjBatchDivisionDTO.BuyerClassification = partnerClassification.Number;
                lObjBatchDivisionDTO.BuyerName = partnerClassification.Name;

                pObjTextbox.Text = partnerClassification.Number.ToString();
                mObjBuyerClassification = partnerClassification;


                if (dgBatch.Items.Count == dgBatch.SelectedIndex + 1)
                {
                    //new row
                    BatchDivisionDTO lObj = new BatchDivisionDTO();
                    mLstObjBatchDivision.Add(lObj);
                    dgBatch.Items.Refresh();
                }
            }
            else
            {
                pObjTextbox.Text = string.Empty;
                mObjBuyerClassification = null;
            }
        }

        private void txtHeadQtty_PreviewTextInput(object sender, TextCompositionEventArgs pObjArgs)
        {
            if (char.IsDigit(pObjArgs.Text, pObjArgs.Text.Length - 1))
            {
                if (Convert.ToInt32((pObjArgs.Text)) >= 0)
                {
                    pObjArgs.Handled = false;
                }
                else
                {
                    pObjArgs.Handled = true;
                }
            }
            else
            {
                pObjArgs.Handled = true;
            }
        }

        private void txtHeadQtty_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckQuantities(sender as TextBox);
        }

        private void txtTotalWeight_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckWeight(sender as TextBox);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int lIntHeads = GetHeads();

            if (lIntHeads > 0)
            {
                if (Convert.ToInt32(txtQuantity.Text) - lIntHeads > 0)
                {
                    if (CheckBuyers())
                    {
                        DivideBuyers();
                    }
                    else
                    {
                        CustomMessageBox.Show("Atención", "No puede quedar alguna linea sin comprador", this.GetParent());
                    }
                }
                else
                {
                    CustomMessageBox.Show("Atención", "El lote original no puede quedar en 0", this.GetParent());
                }
            }
        }

        private void txtTotalWeight_GotFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            (pObjSender as TextBox).Text = "0";
        }

        private void txtHeadQtty_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (Convert.ToInt32((pObjSender as TextBox).Text) > 0)
            {
                BatchDivisionDTO lObjBatchDivisionDTO = (BatchDivisionDTO)dgBatch.CurrentItem;
                lObjBatchDivisionDTO.CellActive = true;
                dgBatch.Items.Refresh();
            }
        }

        private void dgBatch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Delete && dgBatch.Items.Count == dgBatch.SelectedIndex + 1)
                {
                    pObjArgs.Handled = true;
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void dgBatch_PreviewKeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Delete && dgBatch.Items.Count == dgBatch.SelectedIndex + 1)
                {
                    pObjArgs.Handled = true;
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        #endregion

        #region Methods

        private void SetControlsAuction(Auction pObjAuction)
        {
            if (pObjAuction != null)
            {
                txtAuction.Text = pObjAuction.Folio;
                mObjAuction = pObjAuction;

                txtBatch.IsEnabled = true;
            }
            else
            {
                txtAuction.Text = "";
                mObjAuction = null;
            }

        }

        private void ResetBatch()
        {
            txtBuyer.Text = string.Empty;
            txtBatch.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtWeigh.Text = string.Empty;
        }

        private void LoadDatagrid()
        {
            //FormLoading();
            try
            {
                //mLstObjBatchDivision = mObjAuctionsFactory.GetBatchService().GetBatchDivisionListToPick(mObjAuction != null ? mObjAuction.Id : 0, mObjBatch.Id);
                mLstObjBatchDivision = new List<BatchDivisionDTO>();

                this.Dispatcher.Invoke(() =>
                {
                    dgBatch.ItemsSource = null;
                    BatchDivisionDTO lObj = new BatchDivisionDTO();
                    mLstObjBatchDivision.Add(lObj);
                    dgBatch.ItemsSource = mLstObjBatchDivision;
                    dgBatch.Items.Refresh();


                    //SetControlsAuction(lObjAuction);
                });

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void DivideBuyers()
        {
            try
            {
                //Add new Batches
                mObjAuctionsFactory.GetBatchService().SaveOrUpdateEntityList(GetNewBatches());
                //Update Divided Batch
                mObjAuctionsFactory.GetBatchAuctionService().SaveOrUpdateBatch(GetEditedBatch(), true);

                ResetForm();

                ShowMessage("División de lote", "El lote se dividio correctamente");
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void ResetForm()
        {

            txtBatch.Text = string.Empty;
            mObjBatch = null;
            txtBuyer.Text = string.Empty;
            mObjBuyerClassification = null;
            txtPrice.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtWeigh.Text = string.Empty;

            mLstObjBatchDivision = null;
            dgBatch.ItemsSource = null;

            LoadDatagrid();
            //dgBatch.Items.Clear();
            //dgBatch.Items.Refresh();
            dgBatch.IsEnabled = false;
        }

        private Batch GetEditedBatch()
        {
            mObjBatch.Quantity = mObjBatch.Quantity - GetHeads();
            mObjBatch.Weight = mObjBatch.Weight - GetTotalWeights();
            mObjBatch.AverageWeight = mObjBatch.Weight / mObjBatch.Quantity;
            mObjBatch.Amount = mObjBatch.Price * Convert.ToDecimal(mObjBatch.Weight);

            return mObjBatch;
        }


        private int GetHeads()
        {
            return mLstObjBatchDivision.Where(x => x.HeadQtty > 0).Sum(x => x.HeadQtty);
        }

        private List<Batch> GetNewBatches()
        {
            return mLstObjBatchDivision.Where(x => x.HeadQtty > 0).Select(x => new Batch()
            {
                Id = 0,
                Number = GetNextBatchNumber(), //mObjAuctionsFactory.GetBatchService().GetNextBatchNumber(mObjAuction.Id),
                AuctionId = mObjAuction.Id,
                Quantity = x.HeadQtty,
                Weight = x.TotalWeight,
                AverageWeight = x.AverageWeight,
                Price = mObjBatch.Price,
                Amount = mObjBatch.Price * Convert.ToDecimal(x.TotalWeight),
                Reprogrammed = false,

                SellerId = mObjBatch.SellerId,
                ItemTypeId = mObjBatch.ItemTypeId,

                BuyerClassificationId = x.BuyerClassification,
                BuyerId = mObjBusinessFactory.GetPartnerClassificationService().GetPartnerByClassificationId(x.BuyerClassification)

            }).ToList();
        }

        private int GetNextBatchNumber()
        {
            int lIntNextBatchNumber = 0;

            if (mIntPrevBatchNumber == 0)
            {
                lIntNextBatchNumber = mObjAuctionsFactory.GetBatchService().GetNextBatchNumber(mObjAuction.Id);
            }
            else
            {
                lIntNextBatchNumber = mIntPrevBatchNumber + 1;

            }

            mIntPrevBatchNumber = lIntNextBatchNumber;

            return lIntNextBatchNumber;
        }

        private void CheckQuantities(TextBox pObjtextBox)
        {
            BatchDivisionDTO lObjBatchDivisionDTO = (BatchDivisionDTO)dgBatch.CurrentItem;

            lObjBatchDivisionDTO.HeadQtty = 0;
            mIntHeadQuantity = GetBatchQuantity(mObjBatch) - GetHeads();
            if (!string.IsNullOrEmpty(pObjtextBox.Text) && pObjtextBox.Text.Length > 0 && pObjtextBox.Text.Trim().Length != 0)
                if ((Convert.ToInt32(pObjtextBox.Text) <= mIntHeadQuantity) && (Convert.ToInt32(pObjtextBox.Text) > 0))
                {
                    lObjBatchDivisionDTO.TotalWeight = GetWeight(pObjtextBox.Text);
                    lObjBatchDivisionDTO.HeadQtty = Convert.ToInt32(pObjtextBox.Text);
                    lObjBatchDivisionDTO.AverageWeight = lObjBatchDivisionDTO.TotalWeight / lObjBatchDivisionDTO.HeadQtty;
                    lObjBatchDivisionDTO.CellActive = true;
                }
                else
                {
                    lObjBatchDivisionDTO.TotalWeight = 0;
                    lObjBatchDivisionDTO.HeadQtty = 0;
                    lObjBatchDivisionDTO.AverageWeight = 0;
                    lObjBatchDivisionDTO.BuyerClassification = 0;
                    lObjBatchDivisionDTO.BuyerName = string.Empty;
                    lObjBatchDivisionDTO.CellActive = false;
                }
            UpdateDataGrid(lObjBatchDivisionDTO.HeadQtty);
        }

        private void CheckWeight(TextBox pObjtextBox)
        {
            float lFlWeight = 0;
            if (pObjtextBox.Text.Length > 0 && pObjtextBox.Text.Trim().Length > 0)
            {
                lFlWeight = float.Parse(pObjtextBox.Text);
            }
            
BatchDivisionDTO lObjBatchDivisionDTO = (BatchDivisionDTO)dgBatch.CurrentItem;

            if (lFlWeight > 0 && lFlWeight <= mFlWeight)
            {
                lObjBatchDivisionDTO.TotalWeight = 0;
                if ((GetTotalWeights() + float.Parse(pObjtextBox.Text)) <= mFlWeight)
                {
                    lObjBatchDivisionDTO.TotalWeight = float.Parse(pObjtextBox.Text);
                    lObjBatchDivisionDTO.AverageWeight = lObjBatchDivisionDTO.TotalWeight / lObjBatchDivisionDTO.HeadQtty;
                }
                else
                {
                    lObjBatchDivisionDTO.TotalWeight = GetWeight(lObjBatchDivisionDTO.HeadQtty.ToString());
                    lObjBatchDivisionDTO.AverageWeight = lObjBatchDivisionDTO.TotalWeight / lObjBatchDivisionDTO.HeadQtty;
                }
            }
            else
            {
                CustomMessageBox.Show("Atención", "El peso total no debe ser mayor al peso del lote original");
            }


            UpdateDataGrid(lObjBatchDivisionDTO.HeadQtty);
        }

        private float GetTotalWeights()
        {
            var dede = mLstObjBatchDivision.Where(x => x.HeadQtty > 0).Sum(x => x.TotalWeight);

            return mLstObjBatchDivision.Where(x => x.HeadQtty > 0).Sum(x => x.TotalWeight);
        }

        private float GetWeight(string pStrHeads)
        {
            int lIntHeads = (Convert.ToInt32(pStrHeads));
            float lFloatTotalWeights = GetTotalWeights();

            float lFloatReturn = lIntHeads > 0 ? (lIntHeads * mFlWeight) / GetBatchQuantity(mObjBatch) : 0;

            return lFloatTotalWeights + lFloatReturn <= mObjBatch.Weight ? lFloatReturn : mObjBatch.Weight - lFloatTotalWeights;
        }

        private void UpdateDataGrid(int pIntHeadQtty)
        {
            try
            {
                if (dgBatch.Items.Count == dgBatch.SelectedIndex + 1 && pIntHeadQtty > 0)
                {
                    //new row
                    BatchDivisionDTO lObj = new BatchDivisionDTO();
                    mLstObjBatchDivision.Add(lObj);
                }
                dgBatch.Items.Refresh();
            }
            catch (Exception)
            {


            }


        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private void SetBatch(Batch batch)
        {
            if (batch != null && GetBatchQuantity(batch) > 0)
            {
                txtBatch.Text = batch.Number.ToString();
                txtBuyer.Text = string.Format("{0} {1}", batch.Buyer.Name, batch.Buyer.ForeignName);
                txtPrice.Text = batch.Price.ToString();
                txtQuantity.Text = GetBatchQuantity(batch).ToString();
                txtWeigh.Text = GetTotalWeight(batch).ToString();//batch.Weight.ToString();//
                mObjBatch = batch;
                mObjBatch.Weight = batch.Weight;
                dgBatch.IsEnabled = true;
                mIntHeadQuantity = batch.Quantity;
                mFlWeight = GetTotalWeight(batch);
                LoadDatagrid();
            }
            else
            {
                ShowMessage("Aviso", "El lote no cuenta con existencias disponibles");
                DefaultForm();
            }
        }

        private float GetTotalWeight(Batch pObjBatch)
        {
            return pObjBatch.Weight - pObjBatch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Weight);
        }

        private void DefaultForm()
        {
            txtBatch.Text = string.Empty;
            mObjBatch = null;
            dgBatch.IsEnabled = false;
        }

        private int GetBatchQuantity(Batch batch)
        {
            //var de = batch.Quantity - batch.GoodsIssues.Where(x => !x.Removed).Sum(x => x.Quantity) - batch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Quantity);
            return batch.Quantity - batch.GoodsReturns.Where(x => !x.Removed).Sum(x => x.Quantity);
        }

        private bool CheckBuyers()
        {
            return mLstObjBatchDivision.Where(x => x.BuyerClassification == 0).Count() - 1 > 0 ? false : true;
        }

        #endregion

    }
}
