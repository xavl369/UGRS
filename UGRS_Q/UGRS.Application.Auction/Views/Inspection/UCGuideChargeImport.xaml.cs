using Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Inspection;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Extension.String;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCGuideChargeImport : UserControl
    {
        #region Attributes

        private AuctionsServicesFactory mObjAuctionsServicesFactory;
        private FinancialsServicesFactory mObjFinancialsServicesFactory;

        private Auction mObjAuction;
        private List<ChargeDTO> mLstObjAuctionSellers;
        private List<ChargeDTO> mLstObjFileSellers;
        private List<ChargeDTO> mLstObjRelatedSellers;
        private ChargeDTO mObjSelectedChargeDTO;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCGuideChargeImport()
        {
            InitializeComponent();
            mObjAuctionsServicesFactory = new AuctionsServicesFactory();
            mObjFinancialsServicesFactory = new FinancialsServicesFactory();
        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDefaultAuction());
            mObjWorker.Start();
        }

        #endregion

        #region TextBox

        private void txtAuction_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    SetControlsAuctionSellers(this.ShowAuctionChooseDialog(pObjSender, pObjArgs, FilterEnum.ACTIVE, AuctionSearchModeEnum.AUCTION));
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
                SetControlsAuctionSellers(null);
            }
        }

        #endregion

        #region Button

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileToImport.Text))
            {
                this.ShowMessage("Error", "Favor de seleccionar un archivo.");
                return;
            }

            if (mObjAuction == null)
            {
                this.ShowMessage("Error", "Favor de seleccionar una subasta.");
                return;
            }

            if (mLstObjRelatedSellers == null)
            {
                this.ShowMessage("Error", "Favor de relacionar los vendedores de subasta con los cobros de las guias.");
                return;
            }

            if (!grdGuideChargeImport.IsBlocked())
            {
                mObjWorker = new Thread(() => InternalCreate());
                mObjWorker.Start();
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {

            if (mObjSelectedChargeDTO != null && !string.IsNullOrEmpty(txtFileToImport.Text))
            {
                UndoRelation();
                mObjSelectedChargeDTO = null;
            }


            //if (!string.IsNullOrEmpty(txtFileToImport.Text))
            //{
            //    SetControlsFileSellers(txtFileToImport.Text);
            //}
            //if (mObjAuction != null)
            //{
            //    SetControlsAuctionSellers(mObjAuction);
            //}
        }

        private void UndoRelation()
        {
            mLstObjRelatedSellers.Remove(mObjSelectedChargeDTO);
            //mLstObjFileSellers = GetFileSellers(txtFileToImport.Text).Where(x => x.SellerName != mLstObjRelatedSellers.Select(y => y.SellerName).FirstOrDefault()).ToList();
            mLstObjFileSellers = GetFileSellers(txtFileToImport.Text)
                   .Where(x => mLstObjRelatedSellers.All(p => p.SellerName != x.SellerName)).ToList();
            mLstObjAuctionSellers.Add(mObjSelectedChargeDTO);

            SetControlsRelatedSellers();
        }

        private void btnRelate_Click(object sender, RoutedEventArgs e)
        {
            ChargeDTO lObjAuctionSeller = dgAuctionUnrelatedSellers.SelectedItem as ChargeDTO;
            ChargeDTO lObjFileSeller = dgFileUnrelatedSellers.SelectedItem as ChargeDTO;

            if (lObjAuctionSeller == null)
            {
                this.ShowMessage("Error", "Favor de seleccionar un vendedor no relacionado en la Subasta.");
                return;
            }

            if (lObjFileSeller == null)
            {
                this.ShowMessage("Error", "Favor de seleccionar un vendedor no relacionado en el Archivo.");
                return;
            }

            mLstObjRelatedSellers.Add(new ChargeDTO()
            {
                SellerId = lObjAuctionSeller.SellerId,
                SellerCode = lObjAuctionSeller.SellerCode,
                SellerName = lObjAuctionSeller.SellerName,
                Amount = lObjFileSeller.Amount
            });

            mLstObjAuctionSellers.Remove(mLstObjAuctionSellers.First(x => x.Equals(lObjAuctionSeller)));
            mLstObjFileSellers.Remove(mLstObjFileSellers.First(x => x.Equals(lObjFileSeller)));

            dgAuctionUnrelatedSellers.ItemsSource = null;
            dgFileUnrelatedSellers.ItemsSource = null;
            dgRelatedSellers.ItemsSource = null;

            dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;
            dgFileUnrelatedSellers.ItemsSource = mLstObjFileSellers;
            dgRelatedSellers.ItemsSource = mLstObjRelatedSellers;
        }

        private void btnSearchPath_Click(object sender, RoutedEventArgs e)
        {
            ResetGrids();
            mLstObjAuctionSellers = GetAuctionSellers(mObjAuction.Id);
            dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;

            SetControlsFileSellers(GetFilePath());
        }

        private void ResetGrids()
        {
            mLstObjRelatedSellers = null;
            mLstObjFileSellers = null;
            mLstObjAuctionSellers = null;

            dgRelatedSellers.ItemsSource = null;
            dgFileUnrelatedSellers.ItemsSource = null;
            dgAuctionUnrelatedSellers.ItemsSource = null;

        }

        #endregion

        #endregion

        #region Methods

        private void LoadDefaultAuction()
        {
            FormLoading();
            try
            {
                Auction lObjAuction = mObjAuctionsServicesFactory.GetAuctionService().GetCurrentOrLast(AuctionCategoryEnum.AUCTION);
                this.Dispatcher.Invoke(() =>
                {
                    SetControlsAuctionSellers(lObjAuction);
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

        private string GetFilePath()
        {
            System.Windows.Forms.OpenFileDialog lObjOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            lObjOpenFileDialog.Title = "Seleccionar archivo";
            lObjOpenFileDialog.InitialDirectory = @"c:\";
            lObjOpenFileDialog.Filter = "Archivos Excel (*.xls, *.xlsx)|*.xls;*.xlsx|Archivos CSV (*.csv)|*.csv";
            lObjOpenFileDialog.DefaultExt = ".xlsx";
            lObjOpenFileDialog.FilterIndex = 1;
            lObjOpenFileDialog.RestoreDirectory = true;
            return lObjOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? lObjOpenFileDialog.FileName : "";
        }

        private void SetControlsAuctionSellers(Auction pObjAuction)
        {
            dgAuctionUnrelatedSellers.ItemsSource = null;
            txtAuction.Text = "";
            mObjAuction = null;

            if (pObjAuction != null)
            {
                if (HasGuideCharges(pObjAuction.Id) /*&& CustomMessageBox.ShowOption("La subasta seleccionada ya tiene cobros de guías.\n ¿Desea continuar?", "Si", "No", "") != true*/)
                {
                    LoadRelatedSellers(pObjAuction.Id);
                    mLstObjAuctionSellers = GetAuctionSellers(pObjAuction.Id)
                        .Where(x => mLstObjRelatedSellers.All(p => p.SellerName != x.SellerName)).ToList();
                    dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;
                    txtAuction.Text = pObjAuction.Folio;
                    mObjAuction = pObjAuction;
                    return;
                }

                mLstObjAuctionSellers = GetAuctionSellers(pObjAuction.Id);
                dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;
                txtAuction.Text = pObjAuction.Folio;
                mObjAuction = pObjAuction;
                SetControlsRelatedSellers();
            }
        }

        private void LoadRelatedSellers(long pLonAuctionId)
        {
            mLstObjRelatedSellers = mObjFinancialsServicesFactory.GetGuideChargeService().GetGuideCharges(pLonAuctionId).Select(x => new ChargeDTO()
            {
                SellerId = x.SellerId,
                SellerCode = x.Seller.Code,
                SellerName = x.Seller.Name,
                RFC = x.Seller.TaxCode,
                Amount = x.Amount
            }).ToList();

            dgRelatedSellers.ItemsSource = null;
            dgRelatedSellers.ItemsSource = mLstObjRelatedSellers;
        }

        private void SetControlsFileSellers(string pStrFilePath)
        {
            dgFileUnrelatedSellers.ItemsSource = null;
            txtFileToImport.Text = "";

            if (!string.IsNullOrEmpty(pStrFilePath))
            {
                mLstObjFileSellers = GetFileSellers(pStrFilePath);
                dgFileUnrelatedSellers.ItemsSource = mLstObjFileSellers;
                txtFileToImport.Text = pStrFilePath;
                SetControlsRelatedSellers();
            }
            else
            {
                mLstObjAuctionSellers = null;
                mLstObjFileSellers = null;
                mLstObjRelatedSellers = null;
                dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;
                dgFileUnrelatedSellers.ItemsSource = mLstObjFileSellers;
                dgRelatedSellers.ItemsSource = mLstObjRelatedSellers;
            }
        }

        private void SetControlsRelatedSellers()
        {
            dgAuctionUnrelatedSellers.ItemsSource = null;
            dgFileUnrelatedSellers.ItemsSource = null;
            dgRelatedSellers.ItemsSource = null;

            if (mLstObjAuctionSellers != null && mLstObjFileSellers != null)
            {
                if (mLstObjRelatedSellers == null)
                {
                    mLstObjRelatedSellers = GetRelatedSellers();
                }
                dgAuctionUnrelatedSellers.ItemsSource = mLstObjAuctionSellers;
                dgFileUnrelatedSellers.ItemsSource = mLstObjFileSellers;
                dgRelatedSellers.ItemsSource = mLstObjRelatedSellers;
            }
        }

        private List<ChargeDTO> GetAuctionSellers(long pLonAuction)
        {
            return mObjAuctionsServicesFactory.GetAuctionService().GetListFilteredByCC()
            .Where(x => x.Id == pLonAuction).SelectMany(x => x.Batches)
            .Where(x => x.SellerId != null)
            .Select(x => x.Seller).Distinct()
            .Select(x => new ChargeDTO()
            {
                SellerId = x.Id,
                SellerCode = x.Code,
                SellerName = x.Name,
                RFC = x.TaxCode,
                Amount = 0
            })
            .ToList();
        }

        private List<ChargeDTO> GetFileSellers(string pStrFilePath)
        {
            List<ChargeDTO> lLstObjResult = new List<ChargeDTO>();
            try
            {
                foreach (var lObjWorksheet in Workbook.Worksheets(pStrFilePath))
                {

                    var des = lObjWorksheet.Rows.Select(x => x).Where(x => x.Cells != null).Select(x => x.Cells).ToList();

                    var lObjValidRows = lObjWorksheet.Rows.Select(x => x)
                        .Where(x => x.Cells != null && !x.Cells.Select(y => y.Text).Contains("NOMBRE")
                            && !x.Cells.Select(y => y.Text).Contains("RFC")
                        && !string.IsNullOrEmpty(x.Cells.Select(y=>y.Text).ToString())
                            ).ToList();

                    foreach (var lObjRow in lObjValidRows)
                    {
                        lLstObjResult.Add(new ChargeDTO()
                        {
                            SellerId = 0,
                            SellerCode = "",
                            SellerName = lObjRow.Cells[0].tType.Equals("s") ? lObjRow.Cells[0].Text : lObjRow.Cells[0].Value,
                            RFC = lObjRow.Cells[1].tType.Equals("s") ? lObjRow.Cells[1].Text : lObjRow.Cells[1].Value,
                            Amount = Convert.ToDouble(lObjRow.Cells[2].tType.Equals("s") ? lObjRow.Cells[2].Text : lObjRow.Cells[2].Value),
                        });
                    }
                    //    foreach (var lObjRow in lObjWorksheet.Rows)
                    //    {

                    //        lLstObjResult.Add(new ChargeDTO()
                    //        {
                    //            SellerId = 0,
                    //            SellerCode = "",
                    //            SellerName = lObjRow.Cells[0].tType.Equals("s") ? lObjRow.Cells[0].Text : lObjRow.Cells[0].Value,
                    //            Amount = Convert.ToDouble(lObjRow.Cells[2].tType.Equals("s") ? lObjRow.Cells[2].Text : lObjRow.Cells[2].Value),
                    //        });
                    //    }
                }
            }
            catch (Exception lObjException)
            {
                lLstObjResult = new List<ChargeDTO>();
                this.ShowMessage("Error", lObjException.Message);
            }
            return lLstObjResult;
        }

        private List<ChargeDTO> GetRelatedSellers()
        {
            List<ChargeDTO> lLstObjResult = new List<ChargeDTO>();
            List<ChargeDTO> lLstObjAuctionSellersToRemove = new List<ChargeDTO>();
            List<ChargeDTO> lLstObjFileSellersToRemove = new List<ChargeDTO>();

            foreach (var lObjAuctionSeller in mLstObjAuctionSellers)
            {
                foreach (var lObjFileSeller in mLstObjFileSellers)
                {
                    if (lObjAuctionSeller.RFC.RelativeEqual(lObjFileSeller.RFC))
                    //if (lObjAuctionSeller.SellerName.RelativeEqual(lObjFileSeller.SellerName))
                    {
                        lLstObjResult.Add(new ChargeDTO()
                        {
                            SellerId = lObjAuctionSeller.SellerId,
                            SellerCode = lObjAuctionSeller.SellerCode,
                            SellerName = lObjAuctionSeller.SellerName,
                            RFC = lObjAuctionSeller.RFC,
                            Amount = lObjFileSeller.Amount
                        });

                        lLstObjAuctionSellersToRemove.Add(lObjAuctionSeller);
                        lLstObjFileSellersToRemove.Add(lObjFileSeller);
                    }
                }
            }

            foreach (var lObjSeller in lLstObjAuctionSellersToRemove)
            {
                mLstObjAuctionSellers.Remove(lObjSeller);
            }

            foreach (var lObjSeller in lLstObjFileSellersToRemove)
            {
                mLstObjFileSellers.Remove(lObjSeller);
            }

            return lLstObjResult;
        }

        private void InternalCreate()
        {
            grdGuideChargeImport.BlockUI();
            try
            {
                //Remove existing charges
                mObjFinancialsServicesFactory.GetGuideChargeService().SaveOrUpdateList
                (
                    mObjFinancialsServicesFactory.GetGuideChargeService().GetQueryList().Where(x => x.AuctionId == mObjAuction.Id).ToList().Select(x => { x.Removed = true; return x; }).ToList()
                );
                //Insert new charges
                mObjFinancialsServicesFactory.GetGuideChargeService().SaveOrUpdateList(mLstObjRelatedSellers.Select(x => new GuideCharge()
                {
                    AuctionId = mObjAuction.Id,
                    SellerId = x.SellerId,
                    Amount = x.Amount
                })
                .ToList());

                ResetForm();
                grdGuideChargeImport.UnblockUI();
                this.ShowMessage("Cobro de guías", "Los cambios se guardaron correctamente.");
            }
            catch (Exception lObjException)
            {
                grdGuideChargeImport.UnblockUI();
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdGuideChargeImport.UnblockUI();
            }
        }

        private bool HasGuideCharges(long pLonAuction)
        {
            return mObjFinancialsServicesFactory.GetGuideChargeService().GetQueryList().Where(x => x.AuctionId == pLonAuction).Count() > 0;
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdGuideChargeImport.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdGuideChargeImport.UnblockUI();
            });
        }

        private void ResetForm()
        {
            this.Dispatcher.Invoke(() =>
            {
                dgAuctionUnrelatedSellers.ItemsSource = null;
                dgFileUnrelatedSellers.ItemsSource = null;
                dgRelatedSellers.ItemsSource = null;
                SetControlsAuctionSellers(null);
                SetControlsFileSellers("");
            });
        }

        #endregion

        private void dgRelatedSellers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mObjSelectedChargeDTO = (ChargeDTO)dgRelatedSellers.CurrentItem;
        }
    }
}

