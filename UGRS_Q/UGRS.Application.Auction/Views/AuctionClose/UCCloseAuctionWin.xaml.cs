using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Effects;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Auctions.Services.Financials;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Auctions.Services.Reports;
using UGRS.Core.Auctions.DTO.Reports.Auctions;
using UGRS.Data.Auctions.Context;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Utility;
using UGRS.Core.Exceptions;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Extension.Enum;
using UGRS.Application.Auctions.Extensions;
using System.Windows.Input;
using UGRS.Core.Auctions.Enums.Base;
using System.Data.Entity;
using QualisysConfig;

namespace UGRS.Application.Auctions
{
    public partial class UCCloseAuctionWin : UserControl
    {
        #region Attributes

        //CONTEXT
        private AuctionsContext mObjAuctionsContext;

        private InventoryServicesFactory mObjInventoryFactory = new InventoryServicesFactory();
        private AuctionsServicesFactory mObjAuctionFactory = new AuctionsServicesFactory();
        private FinancialsServicesFactory mObjFinancialsFactory = new FinancialsServicesFactory();
        private SystemServicesFactory mObjSystemFactory = new SystemServicesFactory();
        private BusinessServicesFactory mObjBusinessFactory = new BusinessServicesFactory();

        //Exceptions list
        private IList<Exception> mLstObjExceptionList;

        private Auction mObjAuction;
        private long mLonAuctionId;

        private Partner mObjPartner = new Partner();
        private FoodCharge mObjFoodCharge = new FoodCharge();
        private ListCollectionView mLcvListData = null;

        private Thread mObjWorker;
        private Thread mObjInternalWorker;

        #endregion

        #region Properties

        //EXCEPTIONS
        private IList<Exception> ExceptionList
        {
            get { return mLstObjExceptionList; }
            set { mLstObjExceptionList = value; }
        }

        //CONTEXT
        private AuctionsContext AuctionsContext
        {
            get { return mObjAuctionsContext; }
            set { mObjAuctionsContext = value; }
        }

        //AUCTIONS
        private TransactionDAO<Auction> AuctionDAO { get; set; }
        private TransactionDAO<Batch> BatchDAO { get; set; }
        private TransactionDAO<BatchLine> BatchLineDAO { get; set; }

        //BUSINESS
        private TransactionDAO<Partner> PartnerDAO { get; set; }
        private TransactionDAO<PartnerMapping> PartnerMappingDAO { get; set; }

        //FINANCIALS
        private TransactionDAO<FoodChargeCheck> FoodChargeCheckDAO { get; set; }
        private TransactionDAO<FoodCharge> FoodChargeDAO { get; set; }
        private TransactionDAO<FoodChargeLine> FoodChargeLineDAO { get; set; }
        private TransactionDAO<GuideCharge> GuideChargeDAO { get; set; }
        private TransactionDAO<FoodDelivery> FoodDeliveryDAO { get; set; }
        private TransactionDAO<Invoice> InvoiceDAO { get; set; }
        private TransactionDAO<InvoiceLine> InvoiceLineDAO { get; set; }
        private TransactionDAO<JournalEntry> JournalEntryDAO { get; set; }
        private TransactionDAO<JournalEntryLine> JournalEntryLineDAO { get; set; }

        //INVENTORY
        private TransactionDAO<GoodsIssue> GoodsIssueDAO { get; set; }
        private TransactionDAO<GoodsReceipt> GoodsReceiptDAO { get; set; }
        private TransactionDAO<GoodsReturn> GoodsReturnDAO { get; set; }
        private TransactionDAO<Stock> StockDAO { get; set; }

        //SYSTEM
        private TransactionDAO<Configuration> ConfigurationDAO { get; set; }

        #endregion

        #region Contructor

        public UCCloseAuctionWin()
        {
            InitializeComponent();

            AuctionsContext = new AuctionsContext();
            //ExceptionList = new List<Exception>();

            //Auction pObjAuction
            //LoadAuctionData(pObjAuction);
            //LoadBatches(GetSoldBatchesList());
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(new ThreadStart(LoadCurrentOrLastAuction));
            mObjWorker.Start();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseInternalForm();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CountTemporaryBusinessPartner() > 0)
            {
                ShowConciliationWindow();
            }
            else
            {
                if (CheckStock() && CheckFoodChecks())
                {
                    DoCloseAuction();
                }
            }
        }

        private bool CheckFoodChecks()
        {
            //List<FoodChargeCheck> lLstFoodChargeCheck = mObjFinancialsFactory.GetFoodChargeCheckService().GetEntitiesList(mObjAuction.Id).ToList();
            IList<long> lLstFoodChargeSellers = mObjFinancialsFactory.GetFoodChargeCheckService().GetEntitiesList(mObjAuction.Id).Select(x => x.SellerId).Distinct().ToList();
            IList<long> lLstLonSellers = mObjAuction.Batches.Where(x => x.SellerId != null).Select(y => y.SellerId ?? 0).Distinct().ToList();
            IList<long> lLstSellers = mObjInventoryFactory.GetStockService().GetListByWhs()
                .Where(x => DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(mObjAuction.Date)
                && lLstLonSellers.Contains(x.CustomerId) && x.Payment).Select(x => x.CustomerId).Distinct().ToList();

            var dif = lLstFoodChargeSellers.Where(x => !lLstSellers.Contains(x)).ToList();


            if (lLstFoodChargeSellers.Count != lLstSellers.Count)
            {
                CustomMessageBox.Show("Error", "Favor de generar cargos de alimento");
                return false;
            }
            return true;
        }

        private bool CheckStock()
        {
            List<GoodsReceipt> lGoodsReceipt = mObjInventoryFactory.GetGoodsReceiptService().GetList().Where(x => !x.Processed && x.ExpirationDate == mObjAuction.Date).ToList();

            if (lGoodsReceipt.Count > 0)
            {
                CustomMessageBox.Show("Error", "Hay entradas temporales sin conciliar");
                return false;
            }

            return true;
        }

        private void txtFolio_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrAuctionText = (pObjSender as TextBox).Text;
                    mObjWorker = new Thread(() => InternalSearchAuction(lStrAuctionText));
                    mObjWorker.Start();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        #endregion

        #region Methods for form

        private void LoadCurrentOrLastAuction()
        {
            this.FormLoading();
            try
            {
                Auction lObjAuction = mObjAuctionFactory.GetAuctionService().GetCurrentOrLast();
                LogUtility.Write("Object Auction loaded");
                this.Dispatcher.Invoke(() =>
                {
                    SetAuctionObject(lObjAuction);
                });
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

        private void InternalSearchAuction(string pStrAuctionText)
        {
            this.FormLoading();
            try
            {
                List<Auction> lLstObjAuctions = mObjAuctionFactory.GetAuctionService().SearchAuctions(pStrAuctionText, FilterEnum.OPENED, AuctionSearchModeEnum.ALL);
                this.Dispatcher.Invoke(() =>
                {
                    if (lLstObjAuctions.Count == 1)
                    {
                        SetAuctionObject(lLstObjAuctions[0]);
                    }
                    else
                    {
                        UserControl lUCAuction = new UCSearchAuction(pStrAuctionText, lLstObjAuctions, FilterEnum.OPENED, AuctionSearchModeEnum.ALL);
                        SetAuctionObject(FunctionsUI.ShowWindowDialog(lUCAuction, this.GetParent()) as Auction);
                    }
                });
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

        private void SetAuctionObject(Auction pObjAuction)
        {
            this.FormLoading();
            try
            {
                mObjAuction = pObjAuction;
                if (pObjAuction != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoadAuctionObject(pObjAuction);
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UnloadAuctionObject();
                    });
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

        private void LoadAuctionObject(Auction pObjAuction)
        {
            //Set auction
            mLonAuctionId = pObjAuction.Id;

            //Set auction information
            txtFolio.Text = pObjAuction.Folio;
            dpDate.Text = pObjAuction.Date.ToShortDateString();
            txtCommission.Text = pObjAuction.Commission.ToString();
            txtAuctionCategory.Text = pObjAuction.Category.GetDescription();
            txtAuctionType.Text = pObjAuction.Type.GetDescription();
            LogUtility.Write("Textboxses loaded");

            //Set financials information
            if (pObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) // DirectDeal
            {
                txtBatches.Text = GetBatchesList().Count().ToString();
                txtTrades.Text = "0";
                txtInvoices.Text = GetSellersList().Count().ToString();
            }
            else
            {
                txtTrades.Text = "0";
                txtTrades.Text = GetTradesList().Count().ToString();
                txtInvoices.Text = GetSellersTradeList().Count().ToString();
            }

            btnSave.IsEnabled = true;
            mObjInternalWorker = new Thread(new ThreadStart(LoadAuctionDetails));
            mObjInternalWorker.Start();
        }

        private void UnloadAuctionObject()
        {
            mLonAuctionId = 0;
            txtFolio.Text = "";
            dpDate.SelectedDate = null;
            txtCommission.Text = "";
            txtAuctionCategory.Text = "";
            txtAuctionType.Text = "";
            txtBatches.Text = "";
            txtInvoices.Text = "";
            txtTrades.Text = "";

            btnSave.IsEnabled = false;
            grdDetails.Children.Clear();
        }

        private void LoadAuctionDetails()
        {
            this.FormLoading();
            try
            {
                IList<Invoice> lLstObjInvoice = GetInvoicesList().Select(i => { i.Opened = false; return i; }).ToList();
                LogUtility.Write("invoice list loaded");
                IList<JournalEntry> lLstObjJournalEntries = GetJournalEntries().Select(i => { i.Opened = false; return i; }).ToList();
                LogUtility.Write("journals loaded");
                IList<GoodsReceipt> lLstObjTemporaryGoodsReceipts = GetTemporaryGoodsReceipts().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();
                LogUtility.Write("temporary receipts loaded");
                IList<GoodsIssue> lLstObjBuyerGoodsIssues = GetBuyerGoodsIssues().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();
                LogUtility.Write("good issues loaded");
                IList<GoodsReturn> lLstObjGoodsReturns = GetGoodsReturns().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();
                LogUtility.Write("goods return loaded");

                this.Dispatcher.Invoke(() =>
                {
                    grdDetails.Children.Add
                    (
                        new UCAuctionClosePreview
                        (
                            mObjAuction,
                            lLstObjInvoice,
                            lLstObjJournalEntries,
                            lLstObjTemporaryGoodsReceipts,
                            lLstObjBuyerGoodsIssues,
                            lLstObjGoodsReturns
                        )
                    );
                });
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

        private void FormLoading(string pStrMessage = "Por favor espere...")
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdCloseAuction.Effect = new BlurEffect() { Radius = 10 };
                grdLoading.Visibility = Visibility.Visible;
                txtWait.Text = pStrMessage;
            });
        }

        private void FormDefault(string pStrMessage = "Por favor espere...")
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdCloseAuction.Effect = null;
                grdLoading.Visibility = Visibility.Collapsed;
                txtWait.Text = pStrMessage;
            });
        }

        #endregion

        #region Methods for calculations

        #region Auction

        private int CountTemporaryBusinessPartner()
        {
            List<long> lLstPartnerIds = mObjBusinessFactory.GetPartnerMappingService().GetList().Where(x => x.Active && !x.Removed).Select(x => x.PartnerId).Distinct().ToList();
            return mObjBusinessFactory.GetPartnerService().GetList().Where(x => x.Active == true && x.Removed == false && (!lLstPartnerIds.Contains(x.Id) && x.Temporary == true)).Count();
        }

        private void DoCloseAuction()
        {
            mObjWorker = new Thread(() => CloseAuction());
            mObjWorker.Start();
        }

        private void CloseAuction()
        {
            string lStrProcessTitle = string.Empty;
            this.FormLoading();

            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    if (!string.IsNullOrEmpty(mObjAuction.Folio))
                    {
                        if (!mObjAuction.ReOpened)
                        {
                            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
                            {
                                lStrProcessTitle = "Generar cobros de alimento";
                                SetWaitStatus("Generando cobros de alimento...");
                                //GenerateFoodCharges();
                            }

                            lStrProcessTitle = "Generar facturas";
                            SetWaitStatus("Generando facturas...");
                            GenerateInvoices();

                            lStrProcessTitle = "Generar transacciones";
                            SetWaitStatus("Generando transacciones...");
                            GenerateInventoryTransactions();

                            lStrProcessTitle = "Cerrar subasta";
                            SetWaitStatus("Cerrando subasta...");

                            lStrProcessTitle = "Generar asientos";
                            SetWaitStatus("Generando asientos...");
                            GenerateJournalEntries();

                            //mObjAuction.Active = false; 
                        }
                        else
                        {
                            lStrProcessTitle = "Generar asientos";
                            SetWaitStatus("Generando asientos...");

                            ReOpenedJournalEntries();
                            //GenerateJournalEntries();
                        }

                        mObjAuction.Opened = false;
                        mObjAuction.ModificationDate = DateTime.Now;
                        AuctionDAO.SaveOrUpdateEntity(mObjAuction);
                    }
                }
                catch (Exception lObjException)
                {
                    this.FormDefault();
                    ExceptionList.Add(new DAOException("Error al cerrar subasta.", lObjException));
                }
                finally
                {
                    if (ExceptionList != null && ExceptionList.Count > 0)
                    {
                        foreach (Exception lObjException in ExceptionList)
                        {
                            LogUtility.WriteException(lObjException);
                        }

                        lObjTransaction.Rollback();
                        this.ShowMessage("Cerrar subasta", "No se pudo cerrar la subasta, favor de revisar el log.");
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        this.ShowMessage("Cerrar subasta", "Subasta cerrada correctamente.");
                    }

                    this.FormDefault();

                    this.Dispatcher.Invoke(() =>
                    {
                        this.CloseInternalForm();
                    });

                }
            }
        }



        private void SetWaitStatus(string pStrStatus)
        {
            this.Dispatcher.Invoke(() => { txtWait.Text = pStrStatus; });
        }

        private void InitializeDAO()
        {
            if (AuctionsContext == null)
            {
                AuctionsContext = new AuctionsContext();
            }

            //AUCTIONS
            AuctionDAO = new TransactionDAO<Auction>(AuctionsContext);
            BatchDAO = new TransactionDAO<Batch>(AuctionsContext);
            BatchLineDAO = new TransactionDAO<BatchLine>(AuctionsContext);

            //BUSINESS
            PartnerDAO = new TransactionDAO<Partner>(AuctionsContext);
            PartnerMappingDAO = new TransactionDAO<PartnerMapping>(AuctionsContext);

            //FINANCIALS
            FoodChargeCheckDAO = new TransactionDAO<FoodChargeCheck>(AuctionsContext);
            FoodChargeDAO = new TransactionDAO<FoodCharge>(AuctionsContext);
            FoodChargeLineDAO = new TransactionDAO<FoodChargeLine>(AuctionsContext);
            GuideChargeDAO = new TransactionDAO<GuideCharge>(AuctionsContext);
            FoodDeliveryDAO = new TransactionDAO<FoodDelivery>(AuctionsContext);
            InvoiceDAO = new TransactionDAO<Invoice>(AuctionsContext);
            InvoiceLineDAO = new TransactionDAO<InvoiceLine>(AuctionsContext);
            JournalEntryDAO = new TransactionDAO<JournalEntry>(AuctionsContext);
            JournalEntryLineDAO = new TransactionDAO<JournalEntryLine>(AuctionsContext);

            //INVENTORY
            GoodsIssueDAO = new TransactionDAO<GoodsIssue>(AuctionsContext);
            GoodsReceiptDAO = new TransactionDAO<GoodsReceipt>(AuctionsContext);
            GoodsReturnDAO = new TransactionDAO<GoodsReturn>(AuctionsContext);
            StockDAO = new TransactionDAO<Stock>(AuctionsContext);

            //SYSTEM
            ConfigurationDAO = new TransactionDAO<Configuration>(AuctionsContext);
        }

        #endregion

        #region Invoice

        private void GenerateInvoices()
        {
            IList<Invoice> lLstObjInvoices = GetInvoicesList().Select(i => { i.Opened = false; return i; }).ToList();

            for (int i = 0; i < lLstObjInvoices.Count; i++)
            {
                SetWaitStatus(string.Format("Generando facturas {0} de {1}", (i + 1), lLstObjInvoices.Count));
                SaveOrUpdateInvoice(lLstObjInvoices[i]);
            }
        }

        private IList<Invoice> GetInvoicesList()
        {

            //Result
            IList<Invoice> lLstObjInvoices = new List<Invoice>();

            //Declarations
            string lStrAuctionsWarehouse = GetConfiguration(ConfigurationKeyEnum.AUCTIONS_WAREHOUSE);
            string lStrFoodWarehouse = GetConfiguration(ConfigurationKeyEnum.FOOD_WAREHOUSE);
            string lStrCostingCode = GetConfiguration(ConfigurationKeyEnum.AUCTION_COSTING_CODE);
            string lStrSeries = GetConfiguration(ConfigurationKeyEnum.DOCUMENTS_SERIES);
            string lStrFoodItemCode = GetConfiguration(ConfigurationKeyEnum.FOOD_ITEM_CODE);
            string lStrFoodTaxCode = GetConfiguration(ConfigurationKeyEnum.FOOD_TAX_CODE);
            double lDblFoodPrice = Convert.ToDouble(GetConfiguration(ConfigurationKeyEnum.FOOD_ITEM_PRICE));
            string lStrComissionItemCode = GetConfiguration(ConfigurationKeyEnum.COMISSION_ITEM_CODE);
            string lStrComissionTaxCode = GetConfiguration(ConfigurationKeyEnum.COMISSION_TAX_CODE);
            string lStrThreePercentCode = GetThreePercent();
            LogUtility.Write("Three Percent");

            //Service
            DeductionCheckService lObjDeductionService = mObjFinancialsFactory.GetDeductionCheckService();
            LogUtility.Write("deduction");
            //Food charge
            IList<FoodCharge> lLstObjFoodCharges = GetFoodChargesList();
            LogUtility.Write("charge list");
            IQueryable<Partner> lLstSellerObjetList;

            //DirectDeal
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE)
            {
                lLstSellerObjetList = GetSellerObjectsList();
            }
            else
            {
                lLstSellerObjetList = GetTradeObjectsList();
            }
            try
            {
                foreach (Partner lObjSeller in lLstSellerObjetList.OrderBy(x=>x.Name))
                {

                    Invoice lObjInvoice = new Invoice();

                    //Get header
                    lObjInvoice.DocType = "I";
                    lObjInvoice.DocNum = 0;
                    lObjInvoice.DocEntry = 0;
                    lObjInvoice.CardCode = lObjSeller.Code;
                    lObjInvoice.AuctionId = mLonAuctionId;
                    lObjInvoice.NumAtCard = mObjAuction.Folio;
                    lObjInvoice.Series = lStrSeries;
                    lObjInvoice.Date = DateTime.Now;
                    lObjInvoice.DueDate = DateTime.Now;
                    lObjInvoice.CreditPayment = !lObjDeductionService.IsMarkedForDeduce(mObjAuction.Id, lObjSeller.Id);
                    lObjInvoice.Comments = string.Format("Factura generada desde la subasta {0}", mObjAuction.Folio);

                    //Initialize lines
                    lObjInvoice.Lines = new List<InvoiceLine>();

                    if (HasSoldBatches(lObjSeller.Id) || mObjAuction.Category == AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
                    {
                        //Add comission
                        lObjInvoice.Lines.Add(new InvoiceLine
                        {
                            ItemCode = lStrComissionItemCode,
                            Quantity = 1,
                            WarehouseCode = lStrAuctionsWarehouse,
                            TaxCode = lStrComissionTaxCode,
                            Price = CalculateComissionBySeller(lObjSeller.Id),
                            CostingCode = lStrCostingCode,
                        });
                    }
                    LogUtility.Write("Comission line" + lLstObjInvoices.Count.ToString());

                    List<FoodChargeCheck> lLstFoodCharges = mObjFinancialsFactory.GetFoodChargeCheckService().GetList(mObjAuction.Id)
                        .Where(x => x.SellerId == lObjSeller.Id && x.ExpirationDate >= mObjAuction.Date && x.ApplyFoodCharge).ToList();

                    LogUtility.Write("Food charges list" + lLstFoodCharges.Count.ToString());

                    if (lLstFoodCharges.Where(x => x.SellerId == lObjSeller.Id).Count() > 0)
                    {
                        lObjInvoice.Lines.Add(new InvoiceLine
                        {
                            ItemCode = lStrThreePercentCode,
                            Quantity = 1,
                            WarehouseCode = lStrFoodWarehouse,
                            TaxCode = lStrFoodTaxCode,
                            Price = 0,
                            CostingCode = lStrCostingCode,
                        });
                    }
                    LogUtility.Write("three percent line");
                    if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE)//DirectDeal
                    {
                        if (lLstObjFoodCharges.Where(x => x.SellerId == lObjSeller.Id).Count() > 0)
                        {
                            //Add food charge
                            lObjInvoice.Lines.Add(new InvoiceLine
                            {
                                ItemCode = lStrFoodItemCode,
                                Quantity = lLstObjFoodCharges.Where(x => x.SellerId == lObjSeller.Id).Select(y => (float?)y.TotalFoodWeight).Sum() ?? 0,
                                WarehouseCode = lStrFoodWarehouse,
                                TaxCode = lStrFoodTaxCode,
                                Price = lDblFoodPrice,
                                CostingCode = lStrCostingCode,
                                DocType = 15,
                                DocNum = GetDocNum(lObjSeller.Code, lStrFoodItemCode, lLstObjFoodCharges.Where(x => x.SellerId == lObjSeller.Id).Select(x => x.Batches).FirstOrDefault()),
                                LineNum = GetLine(lObjSeller.Code, lStrFoodItemCode, lLstObjFoodCharges.Where(x => x.SellerId == lObjSeller.Id).Select(x => x.Batches).FirstOrDefault()),
                            });
                        }
                        LogUtility.Write("Food charge line");
                        if (HasFoodDelivery(lObjSeller.Code, lStrFoodItemCode))
                        {
                            //Get food deliveries
                            IList<InvoiceLine> lLstObjFoodDeliveryInvoiceLines = GetDeliveriesFoodBySeller(lObjSeller.Code).Select(f => new InvoiceLine()
                            {
                                ItemCode = f.ItemCode,
                                Price = (double)f.Price,
                                Quantity = f.Quantity,
                                WarehouseCode = f.WhsCode,
                                TaxCode = f.TaxCode != null && f.TaxCode != "" ? f.TaxCode : "V0",
                                CostingCode = lStrCostingCode,
                                DocType = 15,
                                DocNum = f.DocEntry,
                                LineNum = f.LineNum,

                            }).ToList();

                            lObjInvoice.Lines = lObjInvoice.Lines.Concat(lLstObjFoodDeliveryInvoiceLines).ToList();
                        }
                        LogUtility.Write("Delivery line");
                    }

                    decimal lDecEarned = GetEarningsBySeller(lObjSeller);
                    LogUtility.Write("earned");
                    decimal lDecDebt = Convert.ToDecimal(lObjInvoice.Lines.Sum(x => x.Price * x.Quantity));
                    LogUtility.Write("debit");
                    if (lDecEarned > lDecDebt && lDecEarned > 0)
                    {
                        lObjInvoice.PaymentCondition = -1;
                        lObjInvoice.PayMethod = "17";
                        lObjInvoice.MainUsage = "P01";

                        lObjInvoice.Payment = GetDeductionCheck(lObjSeller.Id);
                        LogUtility.Write("Deduction Check" + lObjInvoice.Payment.ToString());
                    }
                    else if (lDecEarned < lDecDebt && lDecEarned >= 0)
                    {
                        lObjInvoice.PaymentCondition = 0;
                        lObjInvoice.PayMethod = "99";
                        lObjInvoice.MainUsage = "P01";

                        lObjInvoice.Payment = GetDeductionCheck(lObjSeller.Id);
                        LogUtility.Write("Deduction Check" + lObjInvoice.Payment.ToString());
                    }
                    else if (lDecEarned < lDecDebt && lDecEarned < 0)
                    {
                        lObjInvoice.PaymentCondition = 0;
                        lObjInvoice.PayMethod = "99";
                        lObjInvoice.MainUsage = "P01";

                        lObjInvoice.Payment = false;
                    }

                    if (lObjInvoice.Lines.Count > 0)
                    {
                        lLstObjInvoices.Add(lObjInvoice);
                    }
                }
            }

            catch (Exception lob)
            {
                this.ShowMessage("Error", lob.Message + "Inner exception: " + lob.InnerException.Message);
            }
            LogUtility.Write("Invoices done");

            return lLstObjInvoices;
        }

        private int GetDocNum(string pstrCardCode, string pstrItemCode, int pIntBatch)
        {
            return mObjFinancialsFactory.GetDeliveryFoodService().GetList()
                .Where(x => !x.Processed
                    && x.CardCode == pstrCardCode
                    && x.ItemCode == pstrItemCode
                    && x.BatchNumber == pIntBatch.ToString()).Select(x => x.DocEntry).FirstOrDefault();
        }

        private int GetLine(string pstrCardCode, string pstrItemCode, int pIntBatch)
        {
            return mObjFinancialsFactory.GetDeliveryFoodService().GetList()
                .Where(x => !x.Processed
                    && x.CardCode == pstrCardCode
                    && x.ItemCode == pstrItemCode
                    && x.BatchNumber == pIntBatch.ToString()).Select(x => x.LineNum).FirstOrDefault();
        }

        private bool GetDeductionCheck(long pLonSellerId)
        {
            var d = mObjFinancialsFactory.GetDeductionCheckService().GetList(mLonAuctionId)
                .Where(x => x.SellerId == pLonSellerId && x.Deduct).FirstOrDefault();

            return mObjFinancialsFactory.GetDeductionCheckService().GetList(mLonAuctionId)
                .Where(x => x.SellerId == pLonSellerId && x.Deduct).Count() > 0 ? true : false;
        }

        private decimal GetEarningsBySeller(Partner lObjSeller)
        {
            decimal lDecImport = 0;

            var lVarBatchesList = GetBatchesList().Where(x => x.SellerId == lObjSeller.Id && x.BuyerId != null && !x.Unsold && x.Quantity > 0).ToList();

            decimal lDecTotalAmount = lVarBatchesList.Count > 0 ? lVarBatchesList.Sum(x => x.Amount) : 0;

            decimal lDecGuideAmount = Convert.ToDecimal(GetGuideChargeAmountBySeller(lObjSeller.Id));

            lDecImport = lDecTotalAmount - lDecGuideAmount;

            if (mObjAuction.Category == AuctionCategoryEnum.DIRECT_TRADE)
            {
                lDecImport = mObjAuctionFactory.GetTradeService().GetList().Where(x => x.AuctionId == mLonAuctionId && x.SellerId == lObjSeller.Id).Select(x => x.Amount).FirstOrDefault();
            }

            return lDecImport;
        }

        private bool HasSoldBatches(long pLonSellerId)
        {
            return GetBatchesListBySeller(pLonSellerId).Where(x => !x.Unsold).Count() > 0;
        }

        private double CalculateComissionBySeller(long pLonSellerId)
        {
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {

                List<Batch> lLstBatches = GetBatchesListBySeller(pLonSellerId).Where(x => !x.Unsold).ToList();

                float lFlTotalComission = (mObjAuction.Commission * (float)(lLstBatches.Select(y => (decimal?)y.Amount).Sum() ?? 0));

                float lFlReturnedAmount = GetReturnedAmount(lLstBatches);

                return lFlTotalComission - lFlReturnedAmount;
            }
            else
            {
                return (mObjAuction.Commission * (float)(GetTradesListBySeller(pLonSellerId).Select(x => (decimal?)x.Amount).Sum() ?? 0));
            }
        }



        private float GetReturnedAmount(List<Batch> pLstBatches)
        {
            List<GoodsReturn> lLstGoodsReturns = pLstBatches.SelectMany(x => x.GoodsReturns).Where(x => !x.Removed).ToList();

            float lFlReturnedAmount = 0;

            foreach (var item in pLstBatches)
            {
                lFlReturnedAmount += (float)(item.Price * (decimal)(lLstGoodsReturns.Where(x => x.BatchId == item.Id).Select(x => (decimal?)x.Weight).Sum() ?? 0));
            }

            return lFlReturnedAmount * mObjAuction.Commission;
        }



        #endregion

        #region Journal Entry
        private void ReOpenedJournalEntries()
        {
            JournalEntry lObjJournalEntry = mObjFinancialsFactory.GetJournalEntryService().GetList().Where(x => x.AuctionId == mObjAuction.Id).FirstOrDefault();

            if (!lObjJournalEntry.Processed)
            {
                UpdateJournalEntry(lObjJournalEntry);
            }
            else
            {
                lObjJournalEntry = GenerateJournalEntry();
                SaveOrUpdateJournalEntry(lObjJournalEntry);
            }


        }

        private JournalEntry GenerateJournalEntry()
        {
            //Declarations
            string lStrWarehouse = GetConfiguration(ConfigurationKeyEnum.AUCTIONS_WAREHOUSE);
            string lStrCostingCode = GetConfiguration(ConfigurationKeyEnum.AUCTION_COSTING_CODE);
            string lStrSeries = GetConfiguration(ConfigurationKeyEnum.DOCUMENTS_SERIES);

            string lStrDebtorsAccount = GetConfiguration(ConfigurationKeyEnum.DEBTORS_ACCOUNT);
            string lStrCreditorsAccount = GetConfiguration(ConfigurationKeyEnum.CREDITORS_ACCOUNT);
            string lStrGuidesAccount = GetConfiguration(ConfigurationKeyEnum.GUIDES_ACCOUNT);
            string lStrNoPaymentGuidesAccount = GetNoPaymmentGuides();
            //string lStrCostingCode = GetLocation();

            JournalEntry lObjJournalEntry = new JournalEntry();
            lObjJournalEntry.AuctionId = mLonAuctionId;
            lObjJournalEntry.Series = lStrSeries;
            lObjJournalEntry.Reference = mObjAuction.Folio;
            lObjJournalEntry.Number = GetNextJournalEntryNumber();
            lObjJournalEntry.Lines = new List<JournalEntryLine>();

            JournalEntryLine lObjLine = null;

            IEnumerable<ReportBatchDTO> lLstBatchDTO;
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                lLstBatchDTO = GetBatchesList().Where(x => x.SellerId != null && x.BuyerId != null && !x.Unsold).ToDTO().Where(x => x.Quantity > 0);
            }
            else
            {
                lLstBatchDTO = GetTradesList().Where(x => x.SellerId != null && x.BuyerId != null).ToDTOTrade();
            }

            foreach (ReportBatchDTO lObjBatch in lLstBatchDTO)
            {
                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrDebtorsAccount;
                lObjLine.ContraAccount = lStrCreditorsAccount;
                lObjLine.Debit = (double)lObjBatch.Amount;
                lObjLine.Credit = 0;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Seller != null ? lObjBatch.SellerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjLine.Commentaries = GetDeductionComments(lObjBatch.SellerCode);
                lObjJournalEntry.Lines.Add(lObjLine);

                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrCreditorsAccount;
                lObjLine.ContraAccount = lStrDebtorsAccount;
                lObjLine.Debit = 0;
                lObjLine.Credit = (double)lObjBatch.Amount;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Buyer != null ? lObjBatch.BuyerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjJournalEntry.Lines.Add(lObjLine);
            }



            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                foreach (Partner lObjSeller in GetSellerObjectsList())
                {
                    bool lBoolPayment = mObjFinancialsFactory.GetDeductionCheckService().IsMarkedForDeduce(mObjAuction.Id, lObjSeller.Id);
                    double lDblAmount = GetGuideChargeAmountBySeller(lObjSeller.Id);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lStrGuidesAccount;
                    lObjLine.ContraAccount = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.Debit = 0;
                    lObjLine.Credit = lDblAmount;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.ContraAccount = lStrGuidesAccount;
                    lObjLine.Debit = lDblAmount;
                    lObjLine.Credit = 0;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);
                }
            }
            return lObjJournalEntry;
        }



        private void UpdateJournalEntry(JournalEntry pObjJournalEntry)
        {
            IList<JournalEntry> lLstObjJournalEntries = new List<JournalEntry>();

            //Declarations
            string lStrWarehouse = GetConfiguration(ConfigurationKeyEnum.AUCTIONS_WAREHOUSE);
            string lStrCostingCode = GetConfiguration(ConfigurationKeyEnum.AUCTION_COSTING_CODE);
            string lStrSeries = GetConfiguration(ConfigurationKeyEnum.DOCUMENTS_SERIES);

            string lStrDebtorsAccount = GetConfiguration(ConfigurationKeyEnum.DEBTORS_ACCOUNT);
            string lStrCreditorsAccount = GetConfiguration(ConfigurationKeyEnum.CREDITORS_ACCOUNT);
            string lStrGuidesAccount = GetConfiguration(ConfigurationKeyEnum.GUIDES_ACCOUNT);
            string lStrNoPaymentGuidesAccount = GetNoPaymmentGuides();


            JournalEntry lObjJournalEntry = pObjJournalEntry;
            lObjJournalEntry.Lines = null;
            lObjJournalEntry.Lines = new List<JournalEntryLine>();

            JournalEntryLine lObjLine = null;

            IEnumerable<ReportBatchDTO> lLstBatchDTO;
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                lLstBatchDTO = GetBatchesList().Where(x => x.SellerId != null && x.BuyerId != null && !x.Unsold).ToDTO().Where(x => x.Quantity > 0);
            }
            else
            {
                lLstBatchDTO = GetTradesList().Where(x => x.SellerId != null && x.BuyerId != null).ToDTOTrade();
            }

            foreach (ReportBatchDTO lObjBatch in lLstBatchDTO)
            {
                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrDebtorsAccount;
                lObjLine.ContraAccount = lStrCreditorsAccount;
                lObjLine.Debit = (double)lObjBatch.Amount;
                lObjLine.Credit = 0;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Seller != null ? lObjBatch.SellerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjLine.Commentaries = GetDeductionComments(lObjBatch.SellerCode);
                lObjJournalEntry.Lines.Add(lObjLine);

                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrCreditorsAccount;
                lObjLine.ContraAccount = lStrDebtorsAccount;
                lObjLine.Debit = 0;
                lObjLine.Credit = (double)lObjBatch.Amount;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Buyer != null ? lObjBatch.BuyerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjJournalEntry.Lines.Add(lObjLine);
            }

            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                foreach (Partner lObjSeller in GetSellerObjectsList())
                {
                    bool lBoolPayment = mObjFinancialsFactory.GetDeductionCheckService().IsMarkedForDeduce(mObjAuction.Id, lObjSeller.Id);
                    double lDblAmount = GetGuideChargeAmountBySeller(lObjSeller.Id);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lStrGuidesAccount;
                    lObjLine.ContraAccount = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.Debit = 0;
                    lObjLine.Credit = lDblAmount;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.ContraAccount = lStrGuidesAccount;
                    lObjLine.Debit = lDblAmount;
                    lObjLine.Credit = 0;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);
                }
            }

            SaveOrUpdateJournalEntry(lObjJournalEntry);
        }

        private void GenerateJournalEntries()
        {
            IList<JournalEntry> lLstObjJournalEntries = GetJournalEntries().Select(i => { i.Opened = false; return i; }).ToList();

            for (int i = 0; i < lLstObjJournalEntries.Count; i++)
            {
                SetWaitStatus(string.Format("Generando asientos {0} de {1}", (i + 1), lLstObjJournalEntries.Count));
                SaveOrUpdateJournalEntry(lLstObjJournalEntries[i]);
            }
        }

        private IList<JournalEntry> GetJournalEntries()
        {
            //Result
            IList<JournalEntry> lLstObjJournalEntries = new List<JournalEntry>();

            //Declarations
            string lStrWarehouse = GetConfiguration(ConfigurationKeyEnum.AUCTIONS_WAREHOUSE);
            string lStrCostingCode = GetConfiguration(ConfigurationKeyEnum.AUCTION_COSTING_CODE);
            string lStrSeries = GetConfiguration(ConfigurationKeyEnum.DOCUMENTS_SERIES);
            string lStrDebtorsAccount = GetConfiguration(ConfigurationKeyEnum.DEBTORS_ACCOUNT);
            string lStrCreditorsAccount = GetConfiguration(ConfigurationKeyEnum.CREDITORS_ACCOUNT);
            string lStrGuidesAccount = GetConfiguration(ConfigurationKeyEnum.GUIDES_ACCOUNT);
            LogUtility.Write("Guides Account");
            string lStrNoPaymentGuidesAccount = GetNoPaymmentGuides();
            LogUtility.Write("No paymment guides");

            JournalEntry lObjJournalEntry = new JournalEntry();
            lObjJournalEntry.AuctionId = mLonAuctionId;
            lObjJournalEntry.Series = lStrSeries;
            lObjJournalEntry.Reference = mObjAuction.Folio;
            lObjJournalEntry.Number = GetNextJournalEntryNumber();
            lObjJournalEntry.Lines = new List<JournalEntryLine>();

            JournalEntryLine lObjLine = null;

            IEnumerable<ReportBatchDTO> lLstBatchDTO;
            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                lLstBatchDTO = GetBatchesList().Where(x => x.SellerId != null && x.BuyerId != null && !x.Unsold).ToDTO().Where(x => x.Quantity > 0);
            }
            else
            {
                lLstBatchDTO = GetTradesList().Where(x => x.SellerId != null && x.BuyerId != null).ToDTOTrade();
            }

            foreach (ReportBatchDTO lObjBatch in lLstBatchDTO)
            {

                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrCreditorsAccount;
                lObjLine.ContraAccount = lStrDebtorsAccount;
                lObjLine.Debit = 0;
                lObjLine.Credit = (double)lObjBatch.Amount;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Seller != null ? lObjBatch.SellerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjLine.Commentaries = GetDeductionComments(lObjBatch.SellerCode);
                lObjJournalEntry.Lines.Add(lObjLine);

                lObjLine = new JournalEntryLine();
                lObjLine.AccountCode = lStrDebtorsAccount;
                lObjLine.ContraAccount = lStrCreditorsAccount;
                lObjLine.Debit = (double)lObjBatch.Amount;
                lObjLine.Credit = 0;
                lObjLine.AuxiliaryType = 1;
                lObjLine.Auxiliary = lObjBatch.Buyer != null ? lObjBatch.BuyerCode : string.Empty;
                lObjLine.CostingCode = lStrCostingCode;
                lObjLine.Reference = string.Format("Lote {0}", lObjBatch.BatchNumber);
                lObjLine.Remarks = string.Empty;
                lObjJournalEntry.Lines.Add(lObjLine);
            }

            if (mObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE) //DirectDeal
            {
                foreach (Partner lObjSeller in GetSellerObjectsList())
                {
                    bool lBoolPayment = mObjFinancialsFactory.GetDeductionCheckService().IsMarkedForDeduce(mObjAuction.Id, lObjSeller.Id);
                    double lDblAmount = GetGuideChargeAmountBySeller(lObjSeller.Id);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lStrGuidesAccount;
                    lObjLine.ContraAccount = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.Debit = 0;
                    lObjLine.Credit = lDblAmount;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);

                    lObjLine = new JournalEntryLine();
                    lObjLine.AccountCode = lBoolPayment ? lStrCreditorsAccount : lStrNoPaymentGuidesAccount;
                    lObjLine.ContraAccount = lStrGuidesAccount;
                    lObjLine.Debit = lDblAmount;
                    lObjLine.Credit = 0;
                    lObjLine.AuxiliaryType = 1;
                    lObjLine.Auxiliary = lObjSeller.Code;
                    lObjLine.CostingCode = lStrCostingCode;
                    lObjLine.Reference = "Guía";
                    lObjLine.Remarks = string.Empty;
                    lObjJournalEntry.Lines.Add(lObjLine);
                }
            }
            lLstObjJournalEntries.Add(lObjJournalEntry);

            LogUtility.Write("Journal finished");
            return lLstObjJournalEntries;
        }

        private string GetDeductionComments(string pStrSellerCode)
        {
            string lStrReturn = mObjFinancialsFactory.GetDeductionCheckService().GetList(mLonAuctionId)
               .Where(x => x.SellerCode == pStrSellerCode && !x.Deduct).Select(x => x.Comments).FirstOrDefault();

            return !string.IsNullOrEmpty(lStrReturn) ? lStrReturn : " ";
        }

        private int GetNextJournalEntryNumber()
        {
            return (mObjFinancialsFactory.GetJournalEntryService().GetList().Count() > 0 ?
                   mObjFinancialsFactory.GetJournalEntryService().GetList().Select(x => x.Number).Max() : 0) + 1;
        }

        #endregion

        #region Food Delivery

        private bool HasFoodDelivery(string pStrSellerCardCode)
        {
            return mObjFinancialsFactory.GetDeliveryFoodService().GetList().Where(x => x.CardCode == pStrSellerCardCode && !x.Processed).Count() > 0;
        }

        private bool HasFoodDelivery(string pStrSellerCardCode, string pStrItemCode)
        {
            return mObjFinancialsFactory.GetDeliveryFoodService().GetList().Where(x => x.CardCode == pStrSellerCardCode /*&& x.ItemCode == pStrItemCode*/ && !x.Processed).Count() > 0;
        }

        private IList<FoodDelivery> GetDeliveriesFoodBySeller(string pStrCardCode)
        {
            var des = mObjFinancialsFactory.GetDeliveryFoodService().GetList().Where(x => !x.Processed && x.CardCode == pStrCardCode && x.Opened).ToList();

            return mObjFinancialsFactory.GetDeliveryFoodService().GetList().Where(x => !x.Processed && x.CardCode == pStrCardCode && x.Opened).ToList();
        }

        private IList<InvoiceLine> GetInvoiceLines(IList<FoodDelivery> pLstObjDeliveries, string pStrCostingCode)
        {
            return pLstObjDeliveries.Select(x => new InvoiceLine()
            {
                ItemCode = x.ItemCode,
                Price = (double)x.Price,
                Quantity = x.Quantity,
                WarehouseCode = x.WhsCode,
                TaxCode = x.TaxCode,
                CostingCode = pStrCostingCode

            }).ToList();
        }

        #endregion

        #region Food Charge

        private void GenerateFoodCharges()
        {
            IList<FoodCharge> lLstObjFoodCharges = GetFoodChargesList().Select(i => { i.Opened = false; return i; }).ToList();

            for (int i = 0; i < lLstObjFoodCharges.Count; i++)
            {
                SetWaitStatus(string.Format("Generando cobros de alimento {0} de {1}", (i + 1), lLstObjFoodCharges.Count));
                SaveOrUpdateFoodCharge(lLstObjFoodCharges[i]);
            }
        }

        #region Get objects and lists

        private IList<FoodCharge> GetFoodChargesList()
        {
            IList<FoodCharge> lLstObjResult = new List<FoodCharge>();
            //IList<StockBatchDTO> lLstObjStockBatches = mObjInventoryFactory.GetStockAuditService().GetBatchStocksList();
            int lIntNextFolio = GetNextFoodChargeFolio();

            foreach (var lLonSellerId in GetSellersList())
            {
                //Initialize object
                FoodCharge lObjFoodCharge = new FoodCharge();

                //Unreprogrammed batches list
                IQueryable<Batch> lLstObjBatches = GetUnreprogrammedBatchesListBySeller(lLonSellerId);

                //Get header
                lObjFoodCharge.AuctionId = mObjAuction.Id;
                lObjFoodCharge.SellerId = lLonSellerId;
                lObjFoodCharge.Folio = lIntNextFolio;
                lObjFoodCharge.Batches = lLstObjBatches.Count();
                lObjFoodCharge.TotalQuantity = lLstObjBatches.Select(x => (int?)x.Quantity).Sum() ?? 0;
                lObjFoodCharge.TotalWeight = lLstObjBatches.Select(x => (float?)x.Weight).Sum() ?? 0;

                //Get lines
                PopulateFoodChargeLines(lLstObjBatches, lObjFoodCharge);

                //Calculate with lines
                lObjFoodCharge.TotalFoodWeight = lObjFoodCharge.Lines.Select(x => (float?)x.FoodWeight).Sum() ?? 0;

                //Add
                if (lObjFoodCharge.Lines != null && lObjFoodCharge.Lines.Count > 0)
                {
                    lLstObjResult.Add(lObjFoodCharge);
                    lIntNextFolio++;
                }
            }

            return lLstObjResult;
        }

        private void PopulateFoodChargeLines(IQueryable<Batch> pLstObjBatches, FoodCharge pObjFoodCharge)
        {
            FoodChargeCheckService lObjService = mObjFinancialsFactory.GetFoodChargeCheckService();

            pObjFoodCharge.Lines = new List<FoodChargeLine>();

            //List<string> lLstStrFoodCharges = lObjService.GetList()
            //    .Where(x => DbFunctions.TruncateTime(x.BatchDate) == DbFunctions.TruncateTime(mObjAuction.Date)).Select(x=>x.BatchNumber).Distinct().ToList();

            //List<Stock> lLstStock = StockDAO.GetEntitiesList().Where(x => lLstStrFoodCharges.Contains(x.BatchNumber)).ToList();

            //foreach (var lVarStock in lLstStock)
            //{
            //    int lIntQtty = lVarStock.Quantity;

            //    foreach (var lVarBatch in pLstObjBatches.Where(x=>x.SellerId == lVarStock.CustomerId &&  mObjInventoryFactory.GetItemDefinitionService()
            //            .GetArticleRelation((long?)x.ItemTypeId) == lVarStock.ItemId))
            //    {

            //    }
            //}
            //foreach (var lVarFoodCharge in lLstObjFoodChargesCheck.OrderBy(x=>x.))
            //{
            //    int lIntQuantity = StockDAO.GetEntitiesList().Where(x => x.BatchNumber.Equals(lVarFoodCharge.BatchNumber)).Select(x => x.Quantity).FirstOrDefault();
            //    long llonItem = StockDAO.GetEntitiesList().Where(x=>x.BatchNumber.Equals(lVarFoodCharge.BatchNumber)).Select(x=>x.ItemId).FirstOrDefault();

            //    foreach (var lVarBatch in pLstObjBatches
            //        .Where(x => x.SellerId == lVarFoodCharge.SellerId && mObjInventoryFactory.GetItemDefinitionService()
            //            .GetArticleRelation((long?)x.ItemTypeId) == llonItem).ToList())
            //    {

            //    }
            //}
            foreach (Batch lObjBatch in pLstObjBatches)
            {
                //foreach (BatchLine lObjLine in lObjBatch.Where(x => !x.Removed))
                //{
                //if (lObjService.ApplyFoodCharge((long)lObjBatch.SellerId),lObjBatch.Auction.Date)
                //{
                //    pObjFoodCharge.Lines.Add(GetFoodChargeLine(lObjBatch, lObjLine));
                //}
                //}
            }
        }

        private FoodChargeLine GetFoodChargeLine(Batch lObjBatch, BatchLine lObjLine)
        {
            FoodChargeLine lObjFoodChargeLine = new FoodChargeLine();

            lObjFoodChargeLine.AuctionBatch = lObjBatch.Number;
            lObjFoodChargeLine.ItemBatch = lObjLine.BatchNumber;
            lObjFoodChargeLine.AverageWeight = lObjBatch.AverageWeight;
            lObjFoodChargeLine.Quantity = lObjLine.Quantity;
            lObjFoodChargeLine.LineEntryDate = lObjLine.BatchDate;
            lObjFoodChargeLine.LineSaleDate = lObjBatch.CreationDate;
            lObjFoodChargeLine.Days = CalculateStayDays(lObjFoodChargeLine.LineEntryDate, lObjFoodChargeLine.LineSaleDate);
            lObjFoodChargeLine.FoodWeight = lObjFoodChargeLine.Quantity * lObjFoodChargeLine.AverageWeight * lObjFoodChargeLine.Days * 0.03f;

            return lObjFoodChargeLine;
        }

        private float GetTotalWeightBySeller(long pLonSellerId)
        {
            return GetBatchesListBySeller(pLonSellerId).Select(x => (float?)x.Weight).Sum() ?? 0;
        }

        private int GetTotalQuantityBySeller(long pLonSellerId)
        {
            return GetUnreprogrammedBatchesListBySeller(pLonSellerId).Select(x => (int?)x.Quantity).Sum() ?? 0;
        }

        private int GetNextFoodChargeFolio()
        {
            return (mObjFinancialsFactory.GetFoodChargeService().GetList().Count() > 0 ? mObjFinancialsFactory.GetFoodChargeService().GetList().Select(x => x.Folio).Max() : 0) + 1;
        }

        #endregion

        #region Calculates

        private float CalculateWeightFood(float pFltFactor, float pFltBatchWeight)
        {
            return pFltFactor * pFltBatchWeight;
        }

        private float CalculateWeightFood(float pFltStayDays, float pFltProportional, float pFltBatchWeight)
        {
            return CalculateFactor(pFltStayDays, pFltProportional) * pFltBatchWeight;
        }

        private float CalculateWeightFood(DateTime pDtmDateEntry, DateTime pDtmDateSold, float pFltProportional, float pFltBatchWeight)
        {
            return CalculateFactor(CalculateStayDays(pDtmDateEntry, pDtmDateSold), pFltProportional) * pFltBatchWeight;
        }

        private float CalculateWeightFood(DateTime pDtmDateEntry, DateTime pDtmDateSold, int pIntLineQuantity, int pIntBatchQuantity, float pFltBatchWeight)
        {
            return CalculateFactor(CalculateStayDays(pDtmDateEntry, pDtmDateSold), CalculateProportional(pIntLineQuantity, pIntBatchQuantity)) * pFltBatchWeight;
        }

        private float CalculateStayDays(DateTime pDtmDateEntry, DateTime pDtmDateSold)
        {
            //Always subtract 30 minutes to balance the queue time
            pDtmDateSold = pDtmDateSold.AddMinutes(-30);

            //Calculate hours diference
            double lDblHours = (float)(pDtmDateSold - pDtmDateEntry).TotalHours;

            //Calculate days
            double lDblDays = lDblHours / 24;

            //Round days
            lDblDays = Math.Round(lDblDays, 4);

            //Return
            return (float)lDblDays;
        }

        private float CalculateProportional(int pIntLineQuantity, int pIntBatchQuantity)
        {
            if (pIntBatchQuantity > 0)
            {
                double lDblProportional = Convert.ToDouble(pIntLineQuantity) / Convert.ToDouble(pIntBatchQuantity);
                lDblProportional = Math.Round(lDblProportional, 4);
                return (float)lDblProportional;
            }
            return 0;
        }

        private float CalculateFactor(float pFltStayDays, float pFltProportional)
        {
            double lDblFactor = pFltStayDays * 0.03 * pFltProportional;
            return (float)Math.Round(lDblFactor, 6);
        }

        #endregion

        #endregion

        #region Guide Charge

        private IList<GuideChargeDTO> GetGuideChargesList()
        {
            return mObjFinancialsFactory.GetGuideChargeService().GetList(mLonAuctionId);
        }

        private IList<GuideChargeDTO> GetGuideChargesListBySeller(long pLonSellerId)
        {
            return mObjFinancialsFactory.GetGuideChargeService().GetList(mLonAuctionId, pLonSellerId);
        }

        private double GetGuideChargeAmountBySeller(long pLonSellerId)
        {
            return GetGuideChargesListBySeller(pLonSellerId).Select(x => x.Amount).FirstOrDefault();
        }

        #endregion

        #region Inventory Transactions

        private void GenerateInventoryTransactions()
        {
            IList<GoodsReceipt> lLstObjTemporaryGoodsReceipts = GetTemporaryGoodsReceipts().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();
            IList<GoodsIssue> lLstObjBuyerGoodsIssues = GetBuyerGoodsIssues().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();
            IList<GoodsReturn> lLstObjGoodsReturns = GetGoodsReturns().AsEnumerable().Select(x => { x.Opened = false; return x; }).ToList();

            int lIntIndex = 1;
            int lIntTotal = lLstObjTemporaryGoodsReceipts.Count + lLstObjBuyerGoodsIssues.Count + lLstObjGoodsReturns.Count;

            foreach (GoodsReceipt lObjGoodsReceipt in lLstObjTemporaryGoodsReceipts)
            {
                SetWaitStatus(string.Format("Generando transacciones {0} de {1}", lIntIndex, lIntTotal));
                GoodsReceiptDAO.SaveOrUpdateEntity(lObjGoodsReceipt);
                lIntIndex++;
            }

            foreach (GoodsIssue lObjGoodsIssue in lLstObjBuyerGoodsIssues)
            {
                SetWaitStatus(string.Format("Generando transacciones {0} de {1}", lIntIndex, lIntTotal));
                GoodsIssueDAO.SaveOrUpdateEntity(lObjGoodsIssue);
                lIntIndex++;
            }

            foreach (GoodsReturn lObjGoodsReturn in lLstObjGoodsReturns)
            {
                SetWaitStatus(string.Format("Generando transacciones {0} de {1}", lIntIndex, lIntTotal));
                GoodsReturnDAO.SaveOrUpdateEntity(lObjGoodsReturn);
                lIntIndex++;
            }
        }

        #endregion

        #region Queries

        private IQueryable<long> GetSellersList()
        {
            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.SellerId != null)
                    .Select(x => (long)x.SellerId)
                    .Distinct();
        }

        private IQueryable<Partner> GetSellerObjectsList()
        {

            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.SellerId != null)
                    .Select(x => x.Seller)
                    .Distinct();
        }

        private IQueryable<Batch> GetBatchesList()
        {

            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.SellerId != null);
        }

        private IQueryable<Batch> GetBatchesListBySeller(long pLonSellerId)
        {
            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.SellerId == pLonSellerId);


        }

        private IQueryable<Batch> GetUnreprogrammedBatchesListBySeller(long pLonSellerId)
        {
            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.SellerId == pLonSellerId && !x.Reprogrammed);
        }

        private IQueryable<BatchLine> GetBatchLinesListBySeller(long pLonSellerId)
        {
            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatchLinesBySeller(mObjAuction.Id, pLonSellerId);
        }

        private IQueryable<GoodsReceipt> GetTemporaryGoodsReceipts()
        {
            var dd = mObjInventoryFactory.GetGoodsReceiptService().GetList();

            return mObjInventoryFactory.GetGoodsReceiptService().GetList().Where(x => x.Quantity > 0 && x.Opened);
        }

        #region DirectDeal

        //DirectDeal
        private IQueryable<Trade> GetTradesList()
        {
            return mObjAuctionFactory.GetTradeService().GetList().Where(x => x.AuctionId == mLonAuctionId && x.SellerId != null);
        }

        private IQueryable<Trade> GetTradesListBySeller(long pLonSellerId)
        {
            return GetTradesList().Where(x => x.SellerId == pLonSellerId);
        }

        private IQueryable<Partner> GetTradeObjectsList()
        {
            return GetTradesList().Select(y => y.Seller).Distinct();
        }

        private IQueryable<long> GetSellersTradeList()
        {
            return GetTradesList().Select(y => y.Seller.Id).Distinct();
        }
        #endregion

        private IQueryable<GoodsIssue> GetBuyerGoodsIssues()
        {
            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.BuyerId != null)
                    .SelectMany(x => x.GoodsIssues.Where(y => y.Quantity > 0 && y.Opened));
        }

        private IQueryable<GoodsReturn> GetGoodsReturns()
        {
            var ds = mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id);

            return mObjAuctionFactory.GetBatchAuctionService()
                    .GetBatches(mObjAuction.Id)
                    .Where(x => x.BuyerId != null)
                    .SelectMany(x => x.GoodsReturns.Where(y => y.Quantity > 0
                        && y.Opened));
        }

        #endregion

        #region Shows

        private void ShowConciliationWindow()
        {
            UserControl lObjUC = new UCConciliationPartner();
            ShowWindowDialog(lObjUC);
        }

        private object ShowWindowDialog(UserControl pUCUserControl)
        {
            WindowDialog lobjWindow = new WindowDialog();
            lobjWindow.Owner = Window.GetWindow(this);
            lobjWindow.Width = 900;
            lobjWindow.Height = 500;
            lobjWindow.grContent.Children.Add(pUCUserControl);

            object lObjObject = new object();
            if (lobjWindow.ShowDialog() == false)
            {
                lObjObject = lobjWindow.gObject;
            }
            return lObjObject;
        }

        #endregion

        #region Load

        //private void LoadAuctionData(Auction pObjAuction)
        //{
        //    //Set auction
        //    mObjAuction = pObjAuction;
        //    mLonAuctionId = pObjAuction.Id;

        //    //Set auction information
        //    txtFolio.Text = pObjAuction.Folio;
        //    txtCommission.Text = pObjAuction.Commission.ToString();
        //    dpDate.Text = pObjAuction.Date.ToShortDateString();
        //    txtAuctionCategory.Text = pObjAuction.Category.GetDescription();
        //    txtAuctionType.Text = pObjAuction.Type.GetDescription();

        //    //Set financials information
        //    if (pObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE && pObjAuction.Category != AuctionCategoryEnum.SHEEP)//DirectDeal
        //    {
        //        txtBatches.Text = GetBatchesList().Count().ToString();
        //        txtInvoices.Text = GetSellersList().Count().ToString();
        //    }
        //    else
        //    {
        //        txtBatches.Text = GetTradesList().Count().ToString();
        //        txtInvoices.Text = GetSellersTradeList().Count().ToString();
        //    }
        //}

        //private void LoadBatches(IList<Batch> pLstObjBatchesList)
        //{
        //    mLcvListData = new ListCollectionView(pLstObjBatchesList.ToList());
        //    dgBatch.ItemsSource = mLcvListData;
        //}

        //private IList<Batch> GetSoldBatchesList()
        //{
        //    return GetBatchesList().Where(x => !x.Unsold).ToList();
        //}

        #endregion

        #region Services

        private string GetConfiguration(ConfigurationKeyEnum pEnmKey)
        {
            return mObjSystemFactory.GetConfigurationService().GetByKey(pEnmKey);
        }

        private void SaveOrUpdateFoodCharge(FoodCharge pObjFoodCharge)
        {
            IList<FoodChargeLine> lLstObjLines = pObjFoodCharge.Lines;
            pObjFoodCharge.Lines = null;

            FoodChargeDAO.SaveOrUpdateEntity(pObjFoodCharge);

            if (lLstObjLines != null && lLstObjLines.Count > 0)
            {
                FoodChargeLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.FoodChargeId = pObjFoodCharge.Id; return x; }).ToList());
            }
        }

        private void SaveOrUpdateInvoice(Invoice pObjInvoice)
        {
            IList<InvoiceLine> lLstObjLines = pObjInvoice.Lines;
            pObjInvoice.Lines = null;

            InvoiceDAO.SaveOrUpdateEntity(pObjInvoice);

            if (lLstObjLines != null && lLstObjLines.Count > 0)
            {
                InvoiceLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.InvoiceId = pObjInvoice.Id; return x; }).ToList());
            }
        }

        private void SaveOrUpdateJournalEntry(JournalEntry pObjJournalEntry)
        {
            IList<JournalEntryLine> lLstObjLines = pObjJournalEntry.Lines;
            pObjJournalEntry.Lines = null;

            JournalEntryDAO.SaveOrUpdateEntity(pObjJournalEntry);

            if (lLstObjLines != null && lLstObjLines.Count > 0)
            {
                JournalEntryLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.JournalEntryId = pObjJournalEntry.Id; return x; }).ToList());
            }
        }

        //#region Guide Charge 

        //private IList<GuideChargeDTO> GetInternalGuideChargesList(long pLonAuctionId)
        //{
        //    return PopulateGuideChargesList(GetDeepGuideChargesList(pLonAuctionId));
        //}

        //private IList<GuideChargeDTO> GetInternalGuideChargesList(long pLonAuctionId, long pLonSellerId)
        //{
        //    return PopulateGuideChargesList(GetDeepGuideChargesList(pLonAuctionId, pLonSellerId));
        //}

        //private IList<GuideChargeDTO> PopulateGuideChargesList(IList<GuideChargeDTO> pLstObjGuideCharges)
        //{
        //    IQueryable<GuideCharge> lLstObjGuideCharges = GuideChargeDAO.GetEntitiesList();

        //    return pLstObjGuideCharges

        //    //Populate Id
        //    .Select(x =>
        //    {
        //        x.Id = lLstObjGuideCharges.Where(y => y.AuctionId == x.AuctionId && y.SellerId == x.SellerId).Select(z => z.Id).FirstOrDefault();
        //        return x;
        //    })

        //    //Populate Amount
        //    .Select(x =>
        //    {
        //        x.Amount = x.Id != 0 ? lLstObjGuideCharges.Where(y => y.Id == x.Id).Select(z => z.Amount).FirstOrDefault() : x.Amount;
        //        return x;
        //    })

        //    //To list
        //    .ToList();
        //}

        //private IList<GuideChargeDTO> GetDeepGuideChargesList(long pLonAuctionId)
        //{
        //    return AuctionDAO.GetEntitiesList()
        //           .Where(x => x.Id == pLonAuctionId && !x.Processed)
        //           .SelectMany(y => y.Batches.Where(z => !z.Removed && z.Seller != null).AsEnumerable().Select(a => new { Auction = y, Seller = a.Seller }))
        //           .Select(x => new GuideChargeDTO()
        //           {
        //               AuctionId = x.Auction.Id,
        //               AuctionFolio = x.Auction.Folio,
        //               SellerId = x.Seller.Id,
        //               SellerCode = x.Seller.Code,
        //               SellerName = x.Seller.Name,
        //               Amount = 0
        //           })
        //            .Distinct()
        //            .ToList();
        //}

        //private IList<GuideChargeDTO> GetDeepGuideChargesList(long pLonAuctionId, long pLonSellerId)
        //{
        //    return AuctionDAO.GetEntitiesList()
        //           .Where(x => x.Id == pLonAuctionId && !x.Processed)
        //           .SelectMany(y => y.Batches.Where(z => !z.Removed && z.Seller != null && z.SellerId == pLonSellerId).AsEnumerable().Select(a => new { Auction = y, Seller = a.Seller }))
        //           .Select(x => new GuideChargeDTO()
        //           {
        //               AuctionId = x.Auction.Id,
        //               AuctionFolio = x.Auction.Folio,
        //               SellerId = x.Seller.Id,
        //               SellerCode = x.Seller.Code,
        //               SellerName = x.Seller.Name,
        //               Amount = 0
        //           })
        //            .Distinct()
        //            .ToList();
        //}

        //private bool ApplyFoodCharge(string pStrBatchNumber, long pLonSellerId)
        //{
        //    return FoodChargeCheckDAO.GetEntitiesList().Where(x => x.BatchNumber.Equals(pStrBatchNumber) && x.SellerId == pLonSellerId).Select(y => y.ApplyFoodCharge).FirstOrDefault();
        //}

        //#endregion

        #endregion

        #endregion

        private string GetLocation()
        {
            return ConfigurationUtility.GetValue<LocationEnum>("Location").GetDescription();
        }

        private string GetNoPaymmentGuides()
        {
            return QsConfig.GetValue<string>("NoPaymmentGuides");
        }

        private string GetThreePercent()
        {
            return QsConfig.GetValue<string>("ThreePercent");
        }
    }
}
