using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Application.Auctions.Events;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Application.Auctions
{
    public partial class UCAuctionHeader : UserControl
    {
        #region Attributes

        AuctionsServicesFactory mObjAuctionsFactory;
        Auction mObjAuction;
        Thread mObjInternalWorker;

        #endregion

        #region Constructor

        public UCAuctionHeader()
        {
            InitializeComponent();
            mObjAuctionsFactory = new AuctionsServicesFactory();
        }

        #endregion

        #region Events

        public event LoadAuctionEventHandler LoadAuction;

        public event ChangeBatchNumberEventHandler ChangeBatchNumber;

        private void OnLoadAuction(Auction pObjAuction)
        {
            if (LoadAuction != null)
            {
                LoadAuction(this, new LoadAuctionArgs(pObjAuction));
            }
        }

        private void OnChangeBatchNumber(int pIntBatchNumber)
        {
            if (ChangeBatchNumber != null)
            {
                ChangeBatchNumber(this, new ChangeBatchNumberArgs(pIntBatchNumber));
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!grdHeaderAuction.IsBlocked())
            {
                mObjInternalWorker = new Thread(() => LoadDefaultAuction());
                mObjInternalWorker.Start();
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

        #endregion

        #region Methods

        public void SetNextBatchNumber()
        {
            try
            {
                SetBatchNumber(GetNextBatchNumber());
                OnChangeBatchNumber(GetNextBatchNumber());
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Numero lote", lObjException.Message, this.GetParent());
            }
        }

        private int GetNextBatchNumber()
        {
            if (txtAuction.ValidRequired() && mObjAuction != null)
            {
                return mObjAuctionsFactory.GetBatchService().GetList().Where(x => x.AuctionId == mObjAuction.Id).Count() > 0 ?
                       mObjAuctionsFactory.GetBatchService().GetList().Where(x => x.AuctionId == mObjAuction.Id).Max(y => y.Number) + 1 : 1;
            }
            else
            {
                throw new Exception("Favor de seleccionar una Subasta.");
            }
        }

        public void SetEditionBatchNumber(int pIntBatchNumber)
        {
            try
            {
                SetBatchNumber(pIntBatchNumber);
                OnChangeBatchNumber(GetNextBatchNumber());
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Numero lote", lObjException.Message, this.GetParent());
            }
        }

        public void SetBatchNumber(int pIntBatchNumber)
        {
            if (pIntBatchNumber > 0)
            {
                txtBatch.Text = pIntBatchNumber.ToString();
            }
            else
            {
                SetNextBatchNumber();
            }
            OnChangeBatchNumber(pIntBatchNumber);
        }

        private void SetAuctionObject(Auction pObjAcution)
        {
            try
            {
                mObjAuction = pObjAcution;
                OnLoadAuction(pObjAcution);

                if (pObjAcution != null)
                {
                    txtAuction.Text = pObjAcution.Folio;
                    txtCategory.Text = pObjAcution.Category.GetDescription();
                    txtType.Text = pObjAcution.Type.GetDescription();
                    txtCommission.Text = pObjAcution.Commission.ToString();
                    dtpDate.SelectedDate = pObjAcution.Date;
                    SetNextBatchNumber();
                }
                else
                {
                    ResetAuction();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetAuction()
        {
            grdHeaderAuction.ClearControl();
            dtpDate.ClearValue(DatePicker.SelectedDateProperty);
            dtpDate.SelectedDate = null;

        }

        private void LoadDefaultAuction()
        {
            grdHeaderAuction.BlockUI();
            Thread.Sleep(300);

            try
            {
                string lStrLastAuction = mObjAuctionsFactory
                                            .GetAuctionService().GetListFilteredByCC().Where(x => x.Active && x.Opened).Count() > 0 ? 
                                           mObjAuctionsFactory
                                           .GetAuctionService().GetListFilteredByCC().Where(x => x.Active && x.Opened).OrderByDescending(y => y.Date)
                                           .Select(z => z.Folio).FirstOrDefault() : string.Empty;

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
                grdHeaderAuction.UnblockUI();
                this.Dispatcher.Invoke(() =>
                {
                    CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
                });
            }
            finally
            {
                grdHeaderAuction.UnblockUI();
            }
        }

        private void InternalAuctionSearch(TextBox pObjTextBox)
        {
            List<Auction> lLstObjAuctions = mObjAuctionsFactory.GetAuctionService().SearchAuctions(pObjTextBox.Text, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION);
            if (lLstObjAuctions.Count == 1)
            {
                SetAuctionObject(lLstObjAuctions[0]);
            }
            else
            {
                pObjTextBox.Focusable = false;
                UserControl lUCAuction = new UCSearchAuction(pObjTextBox.Text, lLstObjAuctions, FilterEnum.OPENED, AuctionSearchModeEnum.AUCTION);
                SetAuctionObject(FunctionsUI.ShowWindowDialog(lUCAuction, this.GetParent()) as Auction);
                pObjTextBox.Focusable = true;
            }
        }

        #endregion


    }
}
