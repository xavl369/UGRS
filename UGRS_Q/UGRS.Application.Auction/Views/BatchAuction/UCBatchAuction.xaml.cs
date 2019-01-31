using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Application.Auctions.Events;
using UGRS.Core.Application.Enum.Forms;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Services;
using UGRS.Data.Auctions.Factories;
using UGRS.Object.Boards;
using System.Linq;

namespace UGRS.Application.Auctions
{
    public partial class UCBatchAuction : UserControl
    {
        #region Attributes

        BoardsServerObject mObjBoardServer;
        BusinessServicesFactory mObjBusinessFactory = new BusinessServicesFactory();
        Guid mObjBoardConnection;
        bool mBolBoardLoaded;
        Thread mObjInternalWorker;

        #endregion

        #region Constructor

        public UCBatchAuction()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            LeftBatchForm.DisableForm();
            RightBatchForm.DisableForm();

            AuctionHeader.LoadAuction += new LoadAuctionEventHandler(OnLoadAuction);
            AuctionHeader.ChangeBatchNumber += new ChangeBatchNumberEventHandler(OnChangeBatchNumber);
            LeftBatchForm.LoadSeller += new LoadPartnerEventHandler(OnLoadLeftSeller);
            LeftBatchForm.LoadBatch += new LoadBatchEventHandler(OnLoadLeftBatch);
            LeftBatchForm.EditBatch += new EditBatchEventHandler(OnEditBatch);
            LeftBatchForm.ConfirmBatch += new ConfirmBatchEventHandler(OnConfirmBatch);
            RightBatchForm.CompleteBatch += new CompleteBatchEventHandler(OnCompleteBatch);
            RightBatchForm.SaveBatch += new SaveBatchEventHandler(OnSaveBatch);
            RightBatchForm.UndoBatch += new UndoBatchEventHandler(OnUndoBatch);
            RightBatchForm.ChangeFormMode += new ChangeFormModeEventHandler(OnChangeRightFormMode);

            if (!grdBatchAuction.IsBlocked())
            {
                mObjInternalWorker = new Thread(ConnectBoardsService);
                mObjInternalWorker.Start();
            }

            AuctionHeader.txtAuction.Focus();
          
        }

        private void UserControl_Unloaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (!grdBatchAuction.IsBlocked())
            {
                mObjInternalWorker = new Thread(DisconnectBoardsService);
                mObjInternalWorker.Start();
            }
        }

        private void OnLoadAuction(object pObjSender, LoadAuctionArgs pObjArgs)
        {
            if (pObjArgs.Auction != null)
            {
                //Enable
                LeftBatchForm.EnableForm();
                LeftBatchForm.EnableBatchSearch();
                LeftBatchForm.txtSellerCode.Focus();
                //RightBatchForm.EnableForm();
            }
            else
            {
                //Disable
                LeftBatchForm.DisableForm();
                LeftBatchForm.DisableBatchSearch();
                AuctionHeader.txtAuction.Focus();
                RightBatchForm.DisableForm();
            }

            btnSkip.IsEnabled = pObjArgs.Auction != null ? true : false;
            LeftBatchForm.AuctionId = pObjArgs.Auction != null ? pObjArgs.Auction.Id : 0;
            LeftBatchForm.AuctionType = pObjArgs.Auction != null ? pObjArgs.Auction.Type : 0;
            RightBatchForm.AuctionId = pObjArgs.Auction != null ? pObjArgs.Auction.Id : 0;
            RightBatchForm.AuctionType = pObjArgs.Auction != null ? pObjArgs.Auction.Type : 0;
            QuantitiesDetail.AuctionId = pObjArgs.Auction != null ? pObjArgs.Auction.Id : 0;
        }

        private void OnChangeBatchNumber(object pObjSender, ChangeBatchNumberArgs pObjArgs)
        {
            LeftBatchForm.BatchNumber = pObjArgs.BatchNumber;
            RightBatchForm.BatchNumber = pObjArgs.BatchNumber;
            LeftBatchForm.chpBatch.Content = string.Format("Lote {0}", pObjArgs.BatchNumber);
        }

        private void OnLoadLeftSeller(object pObjSender, LoadPartnerArgs pObjArgs)
        {
            if (pObjArgs.Partner != null)
            {
                QuantitiesDetail.LoadQuantities(pObjArgs.Partner.Id);
            }
            else
            {
                QuantitiesDetail.ResetCustomerStock();
            }
        }

        private void OnLoadLeftBatch(object pObjSender, LoadBatchArgs pObjArgs)
        {
            if (pObjArgs.Batch != null)
            {
                AuctionHeader.SetBatchNumber(pObjArgs.Batch.Number);
            }
        }

        private void OnEditBatch(object pObjSender, EditBatchArgs pObjArgs)
        {
            if (pObjArgs.Batch != null)
            {

                AuctionHeader.SetEditionBatchNumber(pObjArgs.Batch.Number);
                //AuctionHeader.txtBatch.Text = pObjArgs.Batch.Number.ToString();
                RightBatchForm.EnableForm();
                RightBatchForm.SetBatchObject(pObjArgs.Batch,true);
                RightBatchForm.UpdateFormMode(FormModeEnum.EDIT);

                //Enable buttons
                btnSave.IsEnabled = true;
                btnPrint.IsEnabled = true;
                btnUndo.IsEnabled = false;
            }
            else
            {
                //Disable buttons
                btnSave.IsEnabled = false;
                btnPrint.IsEnabled = false;
                btnUndo.IsEnabled = false;
            }
        }

        private void OnConfirmBatch(object pObjSender, ConfirmBatchArgs pObjArgs)
        {
            if (pObjArgs.Batch != null)
            {
                AuctionHeader.SetNextBatchNumber();
                DisplayInBoardOne(pObjArgs.Batch);
                LeftBatchForm.EnableForm();
                RightBatchForm.EnableForm();
                RightBatchForm.SetBatchObject(pObjArgs.Batch);
                RightBatchForm.UpdateFormMode(FormModeEnum.NEW);

                //Enable buttons
                btnSave.IsEnabled = true;
                btnPrint.IsEnabled = true;
                btnUndo.IsEnabled = true;
            }
            else
            {
                //Disable buttons
                btnSave.IsEnabled = false;
                btnPrint.IsEnabled = false;
                btnUndo.IsEnabled = false;
            }
        }

        private void OnCompleteBatch(object pObjSender, CompleteBatchArgs pObjArgs)
        {
            btnSave.Focus();
        }

        private void OnSaveBatch(object pObjSender, SaveBatchArgs pObjArgs)
        {
            if (pObjArgs != null)
            {
                AuctionHeader.SetNextBatchNumber();
                RightBatchForm.Print(pObjArgs.Batch);
                DisplayInBoardTwo(pObjArgs.Batch);
            }
        }

        private void OnUndoBatch(object pObjSender, UndoBatchArgs pObjArgs)
        {
            if(pObjArgs.Batch != null)
            {
                LeftBatchForm.SetBatchObject(pObjArgs.Batch);
            }
        }

        private void OnChangeRightFormMode(object pObjSender, ChangeFormModeArgs pObjArgs)
        {
            pnlButtons.DisableControl();
            btnSkip.IsEnabled = true;

            switch(pObjArgs.FormMode)
            {
                case FormModeEnum.NEW:
                    pnlButtons.EnableControl();
                    break;
                case FormModeEnum.EDIT:
                    pnlButtons.EnableControl();
                    btnUndo.IsEnabled = false;
                    break;
            }
        }

        #endregion

        #region Button

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            RightBatchForm.Save();
        }

        private void btnPrint_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            RightBatchForm.Print();
        }

        private void btnUndo_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            RightBatchForm.Undo();
        }

        private void btnSkip_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (RightBatchForm.Skip())
            {
                AuctionHeader.SetNextBatchNumber();
                LeftBatchForm.txtSellerCode.Focus();
            }
        }

        #endregion

        #endregion

        #region Methods

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        #region Boards Service

        private void ConnectBoardsService()
        {
            mBolBoardLoaded = false;

            try
            {
                mObjBoardServer = (BoardsServerObject)Activator.GetObject(typeof(BoardsServerObject), "http://localhost:8820/Boards");
                mObjBoardConnection = mObjBoardServer.Connect();
                mBolBoardLoaded = true;

                LogService.WriteInfo("Boards connected");

                //Initialize display in blank
                DisplayInBoardOne(new Batch());
                DisplayInBoardTwo(new Batch());
            }
            catch (Exception lObjException)
            {
                ShowMessage("Conexión a pantallas", lObjException.Message);
                mBolBoardLoaded = false;
            }
        }

        private void DisconnectBoardsService()
        {
            try
            {
                if (mBolBoardLoaded)
                {
                    mObjBoardServer.Disconnect(mObjBoardConnection);
                }
            }
            catch (Exception lObjException)
            {
                ShowMessage("Conexión a pantallas", lObjException.Message);
                mBolBoardLoaded = false;
            }
        }

        public void DisplayInBoardOne(Batch pObjBatch)
        {
            try
            {
                
                if (mBolBoardLoaded)
                {
                    mObjBoardServer.WriteDisplayOne
                    (
                        //pObjBatch.Quantity.ToString(), 
                        //pObjBatch.Weight.ToString("###0"), 
                        //pObjBatch.AverageWeight.ToString("###0.0")
                        pObjBatch.Quantity,
                        pObjBatch.Weight,
                        pObjBatch.AverageWeight
                    );
                }
                else
                {
                    ShowMessage("Pantalla 1", "La pantalla no se encuentra conectada.");
                }
            }
            catch (Exception lObjException)
            {
                ShowMessage("Pantalla 1", lObjException.Message);
            }
        }

        public void DisplayInBoardTwo(Batch pObjbatch)
        {
            try
            {
                //string lStrClassif = pObjbatch.BuyerClassificationId != null && pObjbatch.BuyerClassificationId > 0
                //        ? GetCustomerClassificationCode((long)pObjbatch.BuyerClassificationId).ToString() : string.Empty;
                if (mBolBoardLoaded)
                {
                    string lStrClassificationCode = pObjbatch.BuyerClassificationId != null && pObjbatch.BuyerClassificationId > 0 
                        ? GetCustomerClassificationCode((long)pObjbatch.BuyerClassificationId).ToString() : string.Empty;

                    mObjBoardServer.WriteDisplayTwo
                    (
                        pObjbatch.Number.ToString(),
                        pObjbatch.Quantity,
                        pObjbatch.Weight,
                        pObjbatch.AverageWeight,
                        //(pObjbatch.Buyer != null ? 
                        //    (pObjbatch.Buyer.Code.Length > 3 ? 
                        //        pObjbatch.Buyer.Code.Substring(pObjbatch.Buyer.Code.Length - 3, 3) : 
                        //        pObjbatch.Buyer.Code
                        //    ) : 
                        //    string.Empty
                        //),
                        (!string.IsNullOrEmpty(lStrClassificationCode)? 
                        (
                        lStrClassificationCode.Length > 3 ? 
                        lStrClassificationCode.Substring(lStrClassificationCode.Length - 3,3) :
                        lStrClassificationCode
                        ) :
                        string.Empty
                        ),
                        (pObjbatch.Price > 999 ?
                            pObjbatch.Price / 10 :
                            pObjbatch.Price
                        )
                    );
                }
                else
                {
                    ShowMessage("Pantalla 2", "La pantalla no se encuentra conectada.");
                }
            }
            catch (Exception lObjException)
            {
                ShowMessage("Pantalla 2", lObjException.Message);
            }
        }


        private string GetCustomerClassificationCode(long pLonCustomerId)
        {
            return mObjBusinessFactory.GetPartnerClassificationService().GetList().Where(x => x.Id == pLonCustomerId).Count() > 0 ?
                   mObjBusinessFactory.GetPartnerClassificationService().GetList().Where(x => x.Id == pLonCustomerId).Select(y => y.Number).FirstOrDefault().ToString("000") : string.Empty;
        }

        #endregion

        #endregion
    }
}

