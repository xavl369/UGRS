using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;
using System.Linq;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Application.Auctions.Extensions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCCancelMovements.xaml
    /// </summary>
    public partial class UCCancelMovements : UserControl
    {

        #region Properties
        private Auction mObjAuction;
        private Batch mObjBatch;
        private AuctionsServicesFactory mObjAuctionsFactory = new AuctionsServicesFactory();
        private AuctionsServicesFactory mObjAuctionsServicesFactory = new AuctionsServicesFactory();
        private InventoryServicesFactory mObjInventoryServicesFactory = new InventoryServicesFactory();
        #endregion

        #region Constructor
        public UCCancelMovements()
        {
            InitializeComponent();
        }
        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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

        private void btnCancelMov_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Cancelar Movimiento", "¿Desea cancelar el movimiento?", "Si", "No", "") == true)
                {
                    CancelMovementDTO lObjCancelMovementDTO = (CancelMovementDTO)dgBatch.CurrentItem;
                    string lStrCancel = string.Empty;
                    if (lObjCancelMovementDTO.DocumentType == Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_ISSUE)
                    {
                        lStrCancel = CancelGoodsIssue(lObjCancelMovementDTO);
                    }
                    else if (lObjCancelMovementDTO.DocumentType == Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_RETURN)
                    {
                        lStrCancel = CancelGoodReturn(lObjCancelMovementDTO);
                    }

                    if (!string.IsNullOrEmpty(lStrCancel))
                    {
                        CustomMessageBox.Show("No fue posible cancelar", lStrCancel, this.GetParent());
                    }
                    else
                    {
                        CustomMessageBox.Show("", "Cancelación realizada con exito", this.GetParent());
                    }

                    LoadDatagrid();
                }
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("Error", ex.Message, this.GetParent());
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

        private void txtBatch_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Batch> lLstObjBatches = mObjAuctionsFactory.GetBatchService().SearchBatches(lStrText, mObjAuction.Id).ToList().Where(x=>x.BuyerId != null).ToList();

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                List<CancelMovementDTO> lLstCancelMovementDTO = new List<CancelMovementDTO>();
                for (int i = 0; i < dgBatch.Items.Count; i++)
                {
                    CancelMovementDTO lObjCancelMovementDTO = (CancelMovementDTO)dgBatch.Items[i];
                    lLstCancelMovementDTO.Add(lObjCancelMovementDTO);
                }

                lLstCancelMovementDTO = lLstCancelMovementDTO.Where(x => x.Canceled == false).ToList();
                int lIntQtyMovement = lLstCancelMovementDTO.Count();

                if (lIntQtyMovement > 0)
                {
                    if (CustomMessageBox.ShowOption("Cancelar Movimientos", "Se cancelaran " + lIntQtyMovement + " movimientos ¿Desea continuar?", "Si", "No", "", this.GetParent()) == true)
                    {
                        IList<string> lLstErrors = new List<string>();
                        foreach (CancelMovementDTO lObjCancelMovement in lLstCancelMovementDTO)
                        {
                            if (lObjCancelMovement.DocumentType == Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_ISSUE)
                            {
                                string lStrCancel = CancelGoodsIssue(lObjCancelMovement);
                                if (!string.IsNullOrEmpty(lStrCancel))
                                {
                                    lLstErrors.Add(lStrCancel);
                                }
                            }
                            else if (lObjCancelMovement.DocumentType == Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_RETURN)
                            {
                                string lStrCancel = CancelGoodReturn(lObjCancelMovement);
                                if (!string.IsNullOrEmpty(lStrCancel))
                                {
                                    lLstErrors.Add(lStrCancel);
                                }
                            }
                        }

                        if (lLstErrors.Count > 0)
                        {
                            string lStrMessage = string.Format("Error al cancelar {0}:\n",
                                string.Join("\n", lLstErrors.Select(x => string.Format("-{0}", x)).ToArray()));
                            CustomMessageBox.Show(lStrMessage);
                        }
                        else
                        {
                            CustomMessageBox.Show("", "Cancelación realizada con exito", this.GetParent());
                        }

                        LoadDatagrid();
                    }
                }
                else
                {
                    CustomMessageBox.Show("", "No existen movimientos a cancelar", this.GetParent());
                }

            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", ex.Message, this.GetParent());
                //row;
            }
        }
        #endregion

        #region Methods
        private string CancelGoodsIssue(CancelMovementDTO pObjCancelMovementDTO)
        {
            string lStrResult = string.Empty;
            try
            {
                List<GoodsIssue> lLstGoodsIssue = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsIssues).Where(y => y.BatchId == mObjBatch.Id).ToList();
                List<GoodsReturn> lLstGoodsReturnExit = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsReturns).Where(y => !y.Removed &&  y.BatchId == mObjBatch.Id && y.Delivered == true).ToList();

                int lIntQtyGoodIssue = lLstGoodsIssue != null ? lLstGoodsIssue.Sum(z => z.Quantity) : 0;
                int lIntQtyGoodReturnExit = lLstGoodsReturnExit != null ? lLstGoodsReturnExit.Sum(z => z.Quantity) : 0;

                if (lIntQtyGoodIssue - pObjCancelMovementDTO.GoodIssue.Quantity >= lIntQtyGoodReturnExit)
                {
                    pObjCancelMovementDTO.GoodIssue.Canceled = true;
                    mObjInventoryServicesFactory.GetGoodsIssueService().SaveOrUpdate(pObjCancelMovementDTO.GoodIssue);
                    mObjInventoryServicesFactory.GetGoodsIssueService().Remove(pObjCancelMovementDTO.MovementId);
                    lStrResult = "";
                }
                else
                {
                    lStrResult = "Favor de revisar las cantidades " + "Cantidad Salidas - Cantidad Salida a Cancelar >= Cantidad Devoluciones(Salida)";
                }
            }
            catch (Exception ex)
            {
                lStrResult = "Error al cancelar folio: " + pObjCancelMovementDTO.GoodIssue.Id + " Error: " + ex.Message;
                //CustomMessageBox.Show("Error al cancelar", ex.Message, this.GetParent());
            }

            return lStrResult;
        }

        private string CancelGoodReturn(CancelMovementDTO pObjCancelMovementDTO)
        {
            string lStrResult = string.Empty;
            try
            {
                bool lBolCancel = false;
                if (pObjCancelMovementDTO.GoodReturn.Delivered)
                {
                    List<GoodsReturn> lLstGoodsReturns = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsReturns).Where(y => y.BatchId == mObjBatch.Id).ToList();
                    int lIntQtyGoodsReturns = lLstGoodsReturns != null ? lLstGoodsReturns.Sum(z => z.Quantity) : 0;


                    if (lIntQtyGoodsReturns > pObjCancelMovementDTO.GoodReturn.Quantity)
                    {
                        lBolCancel = true;
                    }
                    else
                    {
                        lBolCancel = false;
                        lStrResult = "Favor de revisar las cantidades " + "La cantidad a cancelar debe de ser menor a la cantidad de salidas";
                    }
                }
                else
                {
                    lBolCancel = true;
                }

                if (lBolCancel)
                {
                    pObjCancelMovementDTO.GoodReturn.Canceled = true;
                    mObjInventoryServicesFactory.GetGoodsReturnService().SaveOrUpdate(pObjCancelMovementDTO.GoodReturn);
                    mObjInventoryServicesFactory.GetGoodsReturnService().Remove(pObjCancelMovementDTO.MovementId);
                    lStrResult = "";
                }
            }
            catch (Exception ex)
            {
                lStrResult = "Error al cancelar " + ex.Message;
            }
            return lStrResult;
        }

        private Auction ShowDialogAuction(object pObjSender, KeyEventArgs pObjArgs)
        {
            Auction lObjAuction = null;
            try
            {
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
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", ex.Message, this.GetParent());
            }
            return lObjAuction;
        }

        private void SetBatch(Batch pObjbatch)
        {
            try
            {
                if (pObjbatch != null)
                {
                    txtBatch.Text = pObjbatch.Number.ToString();
                    tbCustomer.Text = string.Format("{0}", pObjbatch.Buyer.Name); //pObjbatch.Buyer.ForeignName);
                    txtCustomer.Text = pObjbatch.BuyerId.ToString();
                    //txtCustomer.Text =
                    //txtPrice.Text = batch.Price.ToString();
                    //txtQuantity.Text = batch.Quantity.ToString();
                    //txtWeigh.Text = batch.Weight.ToString();
                    mObjBatch = pObjbatch;
                    //dgBatch.IsEnabled = true;

                    LoadDatagrid();
                }
                else
                {
                    txtBatch.Text = string.Empty;
                    ///mObjBatch = null;
                    dgBatch.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", ex.Message, this.GetParent());
               // throw;
            }
        }

        private void SetControlsAuction(Auction pObjAuction)
        {
            if (pObjAuction != null)
            {
                txtAuction.Text = pObjAuction.Folio;
                mObjAuction = pObjAuction;
                txtBatch.IsEnabled = true;
                txtBatch.Focus();
            }
            else
            {
                mObjAuction = null;
                txtAuction.Text = string.Empty;
                txtBatch.IsEnabled = false;
            }
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

        private void LoadDatagrid()
        {
            try
            {
                List<CancelMovementDTO> lLstCancelMovement = new List<CancelMovementDTO>();
                List<GoodsIssue> lLstGoodsIssue = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsIssues).Where(y => y.BatchId == mObjBatch.Id).ToList();
                List<GoodsReturn> lLstGoodsReturn = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsReturns).Where(y => y.BatchId == mObjBatch.Id && y.Delivered == false).ToList();
                List<GoodsReturn> lLstGoodsReturnExit = mObjAuctionsFactory.GetBatchAuctionService().GetBatches(mObjAuction.Id).SelectMany(x => x.GoodsReturns).Where(y => y.BatchId == mObjBatch.Id && y.Delivered == true).ToList();

                lLstCancelMovement.AddRange(lLstGoodsIssue.Select(x => new CancelMovementDTO
                {
                    MovementId = x.Id,
                    TypeMovement = "Salida de ganado " + x.Folio,
                    Quantity = x.Quantity,
                    DocumentType = Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_ISSUE,
                    Delivered = false,
                    Date = x.CreationDate,
                    Canceled = x.Canceled,
                    Status = x.Canceled == true ? "Cancelado" : "",
                    GoodIssue = x,
                }).ToList());

                lLstCancelMovement.AddRange(lLstGoodsReturn.Select(x => new CancelMovementDTO
                {
                    MovementId = x.Id,
                    TypeMovement = "Devolucion de ganado " + x.Folio,
                    Quantity = x.Quantity,
                    DocumentType = Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_RETURN,
                    Delivered = false,
                    Date = x.CreationDate,
                    Canceled = x.Canceled,
                    Status = x.Canceled == true ? "Cancelado" : "",
                    GoodReturn = x,
                }).ToList());

                lLstCancelMovement.AddRange(lLstGoodsReturnExit.Select(x => new CancelMovementDTO
                {
                    MovementId = x.Id,
                    TypeMovement = "Devolucion de ganado (salida) " + x.Folio,
                    Quantity = x.Quantity,
                    DocumentType = Core.Auctions.Enums.Inventory.DocumentTypeEnum.GOODS_RETURN,
                    Delivered = true,
                    Date = x.CreationDate,
                    Canceled = x.Canceled,
                    Status = x.Canceled == true ? "Cancelado" : "",
                    GoodReturn = x
                }).ToList());

                //lLstCancelMovement[lLstCancelMovement.Count - 1].Cancel = true;
                if (lLstCancelMovement.Where(x => x.Canceled == false).Count() > 0)
                {
                    lLstCancelMovement.Where(x => x.Canceled == false).OrderBy(y => y.Date).Last().Cancel = true;
                }
                dgBatch.ItemsSource = null;
                dgBatch.ItemsSource = lLstCancelMovement.OrderBy(x => x.Date);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", ex.Message, this.GetParent());
               // throw;
            }
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }
        #endregion




    }
}
