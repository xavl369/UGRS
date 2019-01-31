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
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.DTO.Business;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCBatchMassiveExchange.xaml
    /// </summary>
    public partial class UCBatchMassiveExchange : UserControl
    {

        private AuctionsServicesFactory mObjAuctionsServicesFactory = new AuctionsServicesFactory();
        private InventoryServicesFactory mObjInventoryServicesFactory = new InventoryServicesFactory();
        private BusinessServicesFactory mObjBusinessFactory = new BusinessServicesFactory();
        private PartnerClassification mObjBuyerClassification = new PartnerClassification();
        private PartnerClassification mObjBuyer2Classification = new PartnerClassification();
        private ListCollectionView mLcvListData = null; //Filtros
        private List<BatchExchangeDTO> mLstObjBatchExchange = null;
        private bool mBoolChanged = false;
        //private Partner mObjBuyer;
        //private Partner mObjBuyer2;
        private Auction mObjAuction;
        private Batch mObjBatch;
        private Thread mObjWorker;



        public UCBatchMassiveExchange()
        {
            InitializeComponent();
            mLstObjBatchExchange = new List<BatchExchangeDTO>();
            mLcvListData = new ListCollectionView(mLstObjBatchExchange);

        }



        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (mObjAuctionsServicesFactory.GetAuctionService().GetReopenedActiveAuction() != null)
            {


                ShowMessage("Atención", "No se pueden realizar movimientos en subastas abiertas por segunda vez");
                this.CloseInternalForm();
            }
            else
            {
                if (mObjAuctionsServicesFactory != null)
                {
                    SetControlsAuction(mObjAuctionsServicesFactory.GetAuctionService().GetActiveAuction());
                }
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

        private void txtAuction_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {

        }



        #region Methods

        private void FormLoading(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdExchange.BlockUI();
                    txtAuction.IsEnabled = false;
                }
                else
                {
                    grdBatchExchange.BlockUI();
                }
            });
        }

        private void LoadDatagrid()
        {
            //FormLoading();
            try
            {
                Auction lObjAuction = mObjAuctionsServicesFactory.GetAuctionService().GetCurrentOrLast(AuctionCategoryEnum.AUCTION);
                mLstObjBatchExchange = mObjAuctionsServicesFactory.GetBatchService().GetBatchExchangeListToPick(lObjAuction != null ? lObjAuction.Id : 0, mObjBuyerClassification.Id);
                mLcvListData = new ListCollectionView(mLstObjBatchExchange);

                this.Dispatcher.Invoke(() =>
                {
                    dgBatch.ItemsSource = null;
                    dgBatch.ItemsSource = mLstObjBatchExchange;

                    SetControlsAuction(lObjAuction);
                });

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void SetControlsAuction(Auction pObjAuction)
        {
            if (pObjAuction != null)
            {
                txtAuction.Text = pObjAuction.Folio;
                mObjAuction = pObjAuction;

                txtBuyer.IsEnabled = true;
            }
            else
            {
                txtAuction.Text = "";
                mObjAuction = null;
            }

        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }


        #endregion

        private void txtBuyer_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<PartnerClassification> lLstObjClassifications = mObjBusinessFactory.GetPartnerClassificationService()
                        .SearchPartner(lStrText, FilterEnum.ACTIVE).Where(x=> mObjBuyerClassification != null ? x.Id != mObjBuyerClassification.Id : true).ToList();

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
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }



        private void SetBuyerClassificationObject(PartnerClassification pObjClassification, TextBox pObjTextBox)
        {

            try
            {

                if (pObjTextBox.Name.Equals("txtBuyer"))
                {
                    mObjBuyerClassification = pObjClassification;
                }
                else
                {
                    mObjBuyer2Classification = pObjClassification;
                }
                //OnLoadBuyerClassification(pObjClassification);

                if (pObjClassification != null)
                {
                    pObjTextBox.Text = pObjClassification.Number.ToString();
                    if (pObjTextBox.Name.Equals("txtBuyer"))
                    {
                        lblBuyer.Content = string.Format("{0} {1}", pObjClassification.Customer.Name, pObjClassification.Customer.ForeignName);
                    }
                    else
                    {
                        lblBuyer2.Content = string.Format("{0} {1}", pObjClassification.Customer.Name, pObjClassification.Customer.ForeignName);
                    }

                    if (mObjBuyerClassification != null)
                    {
                        LoadDatagrid();
                    }
                }
                else
                {
                    ResetBuyer(pObjTextBox);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetBuyer(TextBox pObjTextbox)
        {
            pObjTextbox.Text = string.Empty;
            if (pObjTextbox.Name.Equals("txtBuyer"))
            {
                lblBuyer.Content = string.Empty;
                lblBuyer2.Content = string.Empty;
                txtBuyer2.Text = string.Empty;
                mObjBuyer2Classification = null;
                mObjBuyerClassification = null;

                txtBuyer2.IsEnabled = false;
            }
            else
            {
                lblBuyer2.Content = string.Empty;
                mObjBuyer2Classification = null;
            }
        }

        private void txtBuyer_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            
            TextBox lObjTextBox = pObjSender as TextBox;

            if ((pObjSender as TextBox).Text == string.Empty)
            {
                ResetBuyer(lObjTextBox);
            }
            if (mObjBuyerClassification != null)
            {
                txtBuyer2.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangeBatches();
                if (mBoolChanged)
                {
                    ShowMessage("Cambio Masivo", "Los lotes fueron cambiados de comprador");
                }
                else
                {
                    ShowMessage("Cambio Masivo", "No hay lotes validos para cambiar");
                }
                LoadDatagrid();
            }
            catch (Exception)
            {

            }
        }

        private void ChangeBatches()
        {
            mObjAuctionsServicesFactory.GetBatchService().SaveOrUpdateEntityList(GetChangedBatches());
        }

        private List<Batch> GetChangedBatches()
        {

            List<long> lLstLonIds = mLstObjBatchExchange.Select(y => y.BatchId).ToList();

            List<Batch> lLstObjBatches = mObjAuctionsServicesFactory.GetBatchService().GetList()
                .Where(x => lLstLonIds.Contains(x.Id) && x.GoodsIssues.Count() == 0).AsEnumerable()
                .Select(x =>
                {
                    x.BuyerId = mObjBuyer2Classification.CustomerId;
                    x.BuyerClassificationId = mObjBuyer2Classification.Id;
                    x.BuyerClassification = mObjBuyer2Classification;
                    x.Buyer = mObjBuyer2Classification.Customer; return x;
                }).ToList();

            mBoolChanged = lLstObjBatches.Count > 0 ? true : false;

            return lLstObjBatches;
        }



    }
}
