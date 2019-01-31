using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Auctions.Enums.Base;
using System.Linq;

namespace UGRS.Application.Auctions
{
    public partial class UCDeductionCheck : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsServicesFactory;
        private FinancialsServicesFactory mObjFinancialsServicesFactory;

        private Auction mObjAuction;
        private List<DeductionCheckDTO> mLstObjCheckList;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCDeductionCheck()
        {
            InitializeComponent();
            mObjAuctionsServicesFactory = new AuctionsServicesFactory();
            mObjFinancialsServicesFactory = new FinancialsServicesFactory();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => LoadDefaultAuction());
            mObjWorker.Start();
        }

        private void txtAuction_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    SetControlsAuction(this.ShowAuctionChooseDialog(pObjSender, pObjArgs, FilterEnum.ACTIVE, AuctionSearchModeEnum.AUCTION));
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

        private void tbnDeduct_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {

                DeductionCheckDTO lObjCheck = (pObjSender as ToggleButton).DataContext as DeductionCheckDTO;
                lObjCheck.Id = GetDeductionId(lObjCheck.AuctionId, lObjCheck.SellerId);
                mLstObjCheckList[mLstObjCheckList.FindIndex(x => x.AuctionId == lObjCheck.AuctionId && x.SellerId == lObjCheck.SellerId)] = lObjCheck;
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private long GetDeductionId(long plonAuctionId, long plonSellerId)
        {
            long lLonId = mObjFinancialsServicesFactory.GetDeductionCheckService().GetList(plonAuctionId).Where(x => x.SellerId == plonSellerId).Select(x => x.Id).FirstOrDefault();
            return lLonId != null ? lLonId : 0;
        }

        private void txtComments_LostFocus(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {
                DeductionCheckDTO lObjCheck = (pObjSender as TextBox).DataContext as DeductionCheckDTO;
                mLstObjCheckList[mLstObjCheckList.FindIndex(x => x.AuctionId == lObjCheck.AuctionId && x.SellerId == lObjCheck.SellerId)] = lObjCheck;
            }
            catch (Exception lObjException)
            {
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => SaveDeductionCheckList());
            mObjWorker.Start();
        }

        private void btnCancel_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => LoadDefaultAuction());
            mObjWorker.Start();
        }

        #endregion

        #region Methods

        private void LoadDefaultAuction()
        {
            this.FormLoading();
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
                this.FormDefault();
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                this.FormDefault();
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
                LoadCheckList(mObjAuction.Id);
            }
            else
            {
                txtAuction.Text = "";
                txtDate.Text = "";
                txtType.Text = "";
                mObjAuction = null;
            }
        }

        private void LoadCheckList(long pLonAuctionId)
        {
            this.FormLoading();
            try
            {
                List<DeductionCheckDTO> lLstObjCheckList = mObjFinancialsServicesFactory.GetDeductionCheckService().GetList(pLonAuctionId).OrderBy(x=>x.SellerName).ToList();
                this.Dispatcher.Invoke(() =>
                {
                    dgDeductionCheck.ItemsSource = null;
                    mLstObjCheckList = null;

                    if (lLstObjCheckList != null)
                    {
                        dgDeductionCheck.ItemsSource = lLstObjCheckList;
                        mLstObjCheckList = lLstObjCheckList;
                    }
                });
            }
            catch (Exception lObjException)
            {
                this.FormDefault();
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                this.FormDefault();
            }
        }

        private void SaveDeductionCheckList()
        {
            this.FormLoading();
            try
            {
                this.mObjFinancialsServicesFactory.GetDeductionCheckService().SaveOrUpdateList(mLstObjCheckList);
                this.FormDefault();
                this.ShowMessage("Deducciones", "Los cambios se han guardado correctamente.");
            }
            catch (Exception lObjException)
            {
                this.FormDefault();
                this.ShowMessage("Error", lObjException.Message);
            }
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdDeductionCheck.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdDeductionCheck.UnblockUI();
            });
        }

        #endregion
    }
}
