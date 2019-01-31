using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UGRS.Core.Auctions.DTO.Auction;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Business;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Auctions.Services;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Exceptions;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Context;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Data.Auctions.Factories;
using QualisysConfig;
using System.Threading;

namespace UGRS.Object.Auctions.Services
{
    public class OperationsService
    {
        #region Attributes

        //CONTEXT
        AuctionsContext mObjAuctionsContext;

        //Service
        ItemBatchService mObjSapStockService;

        InventoryServicesFactory mObjInventoryFactory;

        StockService mObjStockService = new StockService();

        FinancialsServicesFactory mObjFinancialSerfviceFactory = new FinancialsServicesFactory();
        //SAP B1 QUERY MANAGER
        QueryManager mObjQueryManager;

        //Exceptions list
        IList<Exception> mLstObjExceptionList;

        #endregion

        #region Properties

        //CONTEXT
        private AuctionsContext AuctionsContext
        {
            get { return mObjAuctionsContext; }
            set { mObjAuctionsContext = value; }
        }

        public ItemBatchService SapStockService
        {
            get { return mObjSapStockService; }
            set { mObjSapStockService = value; }
        }

        //EXCEPTIONS
        private IList<Exception> ExceptionList
        {
            get { return mLstObjExceptionList; }
            set { mLstObjExceptionList = value; }
        }

        //AUCTIONS
        private TransactionDAO<Auction> AuctionDAO { get; set; }
        private TransactionDAO<Batch> BatchDAO { get; set; }
        private TransactionDAO<BatchLine> BatchLineDAO { get; set; }

        //BUSINESS
        private TransactionDAO<Partner> PartnerDAO { get; set; }
        private TransactionDAO<PartnerMapping> PartnerMappingDAO { get; set; }

        //FINANCIALS
        private TransactionDAO<FoodCharge> FoodChargeDAO { get; set; }
        private TransactionDAO<GuideCharge> GuideChargeDAO { get; set; }
        private TransactionDAO<Invoice> InvoiceDAO { get; set; }
        private TransactionDAO<InvoiceLine> InvoiceLineDAO { get; set; }
        private TransactionDAO<JournalEntry> JournalEntryDAO { get; set; }
        private TransactionDAO<JournalEntryLine> JournalEntryLineDAO { get; set; }

        //INVENTORY
        private TransactionDAO<GoodsIssue> GoodsIssueDAO { get; set; }
        private TransactionDAO<GoodsReceipt> GoodsReceiptDAO { get; set; }
        private TransactionDAO<GoodsReturn> GoodsReturnDAO { get; set; }
        private TransactionDAO<Stock> StockDAO { get; set; }

        //SAP B1 QUERY MANAGER
        private QueryManager QueryManager
        {
            get { return mObjQueryManager; }
            set { mObjQueryManager = value; }
        }

        List<SAPBatchDTO> mLstObjGoodsIssues = null;

        #endregion

        #region Contructor

        public OperationsService()
        {
            AuctionsContext = new AuctionsContext();
            SapStockService = new ItemBatchService();
            mObjInventoryFactory = new InventoryServicesFactory();
            QueryManager = new QueryManager();
            ExceptionList = new List<Exception>();
        }

        #endregion

        #region Methods

        public void ConciliatePartners()
        {
            DIApplication.Company.StartTransaction();
            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    if (ExistsUnprocessedBusinessPartnerMapping())
                    {
                        ProcessBusinessPartner(GetUnprocessedBusinessPartnerMapping());
                    }
                }
                catch (Exception lObjException)
                {
                    ExceptionList.Add(new DAOException("Error al procesar clientes temporales.", lObjException));
                }
                finally
                {
                    if (ExceptionList != null && ExceptionList.Count > 0)
                    {
                        foreach (Exception lObjException in ExceptionList)
                        {
                            LogUtility.WriteException(lObjException);
                        }

                        try
                        {
                            if (DIApplication.Company.InTransaction)
                            {
                                lObjTransaction.Rollback();
                                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                LogUtility.WriteError("No se pudieron procesar los clientes temporales");
                            }

                        }
                        catch (Exception ex)
                        {
                            LogUtility.WriteError(ex.Message);
                        }
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        LogUtility.WriteInfo("Clientes temporales procesados correctamente.");
                    }
                }
            }
        }

        public void ConciliateStock(DateTime pDtAuctionDate)
        {
            DIApplication.Company.StartTransaction();
            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    ConciliateTemporaryStock(GetSellerTemporaryGoodsReceipts(pDtAuctionDate), GetStockList(pDtAuctionDate));
                }
                catch (Exception lObjException)
                {
                    ExceptionList.Add(new DAOException("Error al procesar inventarios temporales.", lObjException));
                }
                finally
                {
                    if (ExceptionList != null && ExceptionList.Count > 0)
                    {
                        foreach (Exception lObjException in ExceptionList)
                        {
                            LogUtility.WriteException(lObjException);
                        }

                        try
                        {
                            if (DIApplication.Company.InTransaction)
                            {
                                lObjTransaction.Rollback();
                                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                LogUtility.WriteError("No se pudieron procesar las entradas temporales");
                            }

                        }
                        catch (Exception ex)
                        {
                            LogUtility.WriteError(ex.Message);
                        }
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        LogUtility.WriteInfo("Entradas temporales procesadas correctamente.");
                    }
                }
            }
        }

        private void ConciliateTemporaryStock(IList<GoodsReceipt> pLstGoodsReceipt, IList<Stock> pLstStock)
        {
            try
            {

                List<GoodsReceipt> lLstGoodReceiptsToUpdate = pLstGoodsReceipt.Where(x => pLstStock
                    .Select(y => new { y.EntryFolio, y.CustomerId, y.ItemId, y.Quantity })
                    .Any(y => y.EntryFolio == x.Folio
                        && y.CustomerId == x.CustomerId
                        && y.ItemId == x.ItemId
                        && y.Quantity == x.Quantity
                        )).Select(x =>
                {
                    x.Processed = true;
                    x.ProcessedDate = DateTime.Now;
                    return x;
                }).ToList();


                GoodsReceiptDAO.SaveOrUpdateEntitiesList(lLstGoodReceiptsToUpdate);
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al procesar las entradas temporales"), lObjException));
            }
        }


        public void ProcessClosedAuctions()
        {
            InitializeDAO();
            if (ExistsUnprocessedAcutions())
            {
                ProcessAuctions(GetUnprocessedAcutionsList());

            }
        }

        public void ProcessReOpenedAuctions()
        {
            DIApplication.Company.StartTransaction();
            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    ProcessReOpenedAuct(GetProcessedAndReopenedAuctions());

                }
                catch (Exception lObjException)
                {
                    ExceptionList.Add(new DAOException("Error al procesar subasta.", lObjException));
                }
                finally
                {
                    if (ExceptionList != null && ExceptionList.Count > 0)
                    {
                        foreach (Exception lObjException in ExceptionList)
                        {
                            LogUtility.WriteException(lObjException);
                        }

                        try
                        {
                            if (DIApplication.Company.InTransaction)
                            {
                                lObjTransaction.Rollback();
                                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                LogUtility.WriteError("No se pudieron procesar las subastas.");
                            }

                        }
                        catch (Exception ex)
                        {
                            LogUtility.WriteError(ex.Message);
                        }
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        LogUtility.WriteInfo("Subastas procesadas correctamente.");
                    }
                }
            }
        }

        private void ProcessReOpenedAuct(IList<Auction> pLstProcessedAndRAuctions)
        {
            try
            {
                foreach (var lVarAuction in pLstProcessedAndRAuctions)
                {
                    ProcessReOpenedAuction(lVarAuction);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al procesar las subastas.", lObjException));
            }
        }

        private void ProcessReOpenedAuction(Auction pObjAuction)
        {
            try
            {
                ProcessCounterJournalEntries(GetProcessedJournalEntry(pObjAuction.Id));

                ProcessJournalEntries(GetUnprocessedJournalEntriesList(pObjAuction.Id));

                pObjAuction.ReProcessed = true;
                pObjAuction.ProcessedDate = DateTime.Now;
                AuctionDAO.SaveOrUpdateEntity(pObjAuction);
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al procesar la subasta {0}.", pObjAuction.Folio), lObjException));
            }

        }

        private void ProcessCounterJournalEntries(IList<JournalEntry> pLstProcessedJournals)
        {
            try
            {
                foreach (JournalEntry lObjJournalEntry in pLstProcessedJournals)
                {
                    CounterJournalEntry(lObjJournalEntry);
                    ChangeJournalEntryStatus(lObjJournalEntry.Auction.Folio);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al procesar los asientos contables.", lObjException));
                if (!DIApplication.Company.InTransaction)
                {
                    DIApplication.Company.StartTransaction();
                }
            }

        }

        private void ChangeJournalEntryStatus(string pStrAuctionFolio)
        {

            SAPbobsCOM.JournalEntries lObjJournalEntry = null;
            try
            {
                lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lObjJournalEntry.GetByKey(GetTransId(pStrAuctionFolio));
                lObjJournalEntry.UserFields.Fields.Item("U_GLO_Cancel").Value = "Y";

                if (lObjJournalEntry.Update() != 0)
                {

                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al procesar el asiento contable de la subasta '{0}'.", pStrAuctionFolio), lObjException));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjJournalEntry);
            }

        }

        private void CounterJournalEntry(JournalEntry pObjJournalEntry)
        {
            SAPbobsCOM.JournalEntries lObjJournalEntry = null;

            try
            {
                //Pupulate header
                lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lObjJournalEntry.GetByKey(GetTransId(pObjJournalEntry.Auction.Folio));
                //Cancel JournalEntry
                if (lObjJournalEntry.Cancel() == 0)
                {
                    pObjJournalEntry.ExportDate = DateTime.Now;
                    pObjJournalEntry.ProcessedDate = DateTime.Now;
                    JournalEntryDAO.SaveOrUpdateEntity(pObjJournalEntry);
                }

            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al procesar el asiento contable de la subasta '{0}'.", pObjJournalEntry.Auction.Folio), lObjException));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjJournalEntry);
            }
        }

        private int GetTransId(string pStrAuctionFolio)
        {
            return mObjQueryManager.GetTransId(pStrAuctionFolio);
        }

        private IList<JournalEntry> GetProcessedJournalEntry(long pLonAuctionId)
        {
            return JournalEntryDAO
        .GetEntitiesList()
        .Where(x => !x.Opened
            && !x.Canceled
            && (x.Processed && x.Auction.ReOpened)
            && !x.Auction.ReProcessed
            && x.AuctionId == pLonAuctionId)
        .ToList();

        }

        public void MoveRemainingStockToRejectionWarehouse()
        {

            DIApplication.Company.StartTransaction();

            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    Auction lObjAuction = AuctionDAO.GetEntitiesList()
                                            .Where(x => x.Processed
                                                && x.ProcessedDate != null)
                                            .OrderByDescending(x => x.ProcessedDate)
                                            .FirstOrDefault();

                    DateTime lDtmStartDate = lObjAuction.Date.Date;
                    DateTime lDtmEndDate = lObjAuction.Date.Date.AddHours(24);

                    ProcessRemainingStockGoodsIssues(lObjAuction, StockDAO.GetEntitiesList()
                        .Where(x => x.Quantity > 0
                            && x.ExpirationDate >= lDtmStartDate
                            && x.ExpirationDate <= lDtmEndDate)
                        .Select(y => new GoodsIssue()
                    {
                        Quantity = y.Quantity,
                        //BatchNumber = y.BatchNumber,
                        //BatchDate = y.CreationDate,
                        //ItemId = y.ItemId,
                        //Item = y.Item,
                        BatchId = 0,
                        Batch = null
                    })
                    .ToList());

                    ProcessRemainingStockGoodsReceipts(lObjAuction, StockDAO.GetEntitiesList()
                        .Where(x => x.Quantity > 0
                            && x.ExpirationDate >= lDtmStartDate
                            && x.ExpirationDate <= lDtmEndDate)
                        .Select(y => new GoodsReceipt()
                    {
                        Quantity = y.Quantity,
                        BatchNumber = y.BatchNumber,
                        BatchDate = y.CreationDate,
                        CustomerId = y.CustomerId,
                        Customer = y.Customer,
                        ItemId = y.ItemId,
                        Item = y.Item
                    })
                    .ToList());
                }
                catch (Exception lObjException)
                {
                    ExceptionList.Add(new DAOException("Error al mover el ganado al almacén de rechazos", lObjException));
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
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        LogUtility.WriteError("No se pudo mover el ganado al almacén de rechazos.");
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        LogUtility.WriteInfo("Se ha movido el ganado al almacén de rechazos correctamente.");
                    }
                }
            }
        }

        public void ProcessPayments()
        {
            //DIApplication.Company.StartTransaction();
            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                try
                {
                    InitializeDAO();

                    //Delete Drafts
                    RemoveInvoicedDrafs(GetProcessedUnpayedInvoiceList());

                    //PayMents
                    GeneratePayments(GetProcessedUnpayedInvoiceList());

                }
                catch (Exception lObjException)
                {
                    ExceptionList.Add(new DAOException("Error al procesar pagos.", lObjException));
                }
                finally
                {
                    if (ExceptionList != null && ExceptionList.Count > 0)
                    {
                        foreach (Exception lObjException in ExceptionList)
                        {
                            LogUtility.WriteException(lObjException);
                        }

                        try
                        {
                            if (DIApplication.Company.InTransaction)
                            {
                                lObjTransaction.Rollback();
                                //DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                LogUtility.WriteError("No se pudieron procesar los pagos.");
                            }

                        }
                        catch (Exception ex)
                        {
                            LogUtility.WriteError(ex.Message);
                        }
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        //DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        LogUtility.WriteInfo("Pagos procesados correctamente.");
                    }
                }
            }
        }

        #region Initialize

        private void InitializeDAO()
        {
            try
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
                FoodChargeDAO = new TransactionDAO<FoodCharge>(AuctionsContext);
                GuideChargeDAO = new TransactionDAO<GuideCharge>(AuctionsContext);
                InvoiceDAO = new TransactionDAO<Invoice>(AuctionsContext);
                InvoiceLineDAO = new TransactionDAO<InvoiceLine>(AuctionsContext);
                JournalEntryDAO = new TransactionDAO<JournalEntry>(AuctionsContext);
                JournalEntryLineDAO = new TransactionDAO<JournalEntryLine>(AuctionsContext);

                //INVENTORY
                GoodsIssueDAO = new TransactionDAO<GoodsIssue>(AuctionsContext);
                GoodsReceiptDAO = new TransactionDAO<GoodsReceipt>(AuctionsContext);
                GoodsReturnDAO = new TransactionDAO<GoodsReturn>(AuctionsContext);
                StockDAO = new TransactionDAO<Stock>(AuctionsContext);
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al inicializar el acceso a datos.", lObjException));
            }
        }

        #endregion

        #region Process

        private void ProcessBusinessPartner(IList<PartnerMapping> pLstObjPartnerMappingList)
        {
            try
            {
                foreach (PartnerMapping lObjMapping in pLstObjPartnerMappingList)
                {

                    ConciliateBusinessPartner(PartnerDAO.GetEntity(lObjMapping.PartnerId), PartnerDAO.GetEntity(lObjMapping.NewPartnerId ?? 0));

                    //Mark as exported
                    lObjMapping.Exported = true;
                    lObjMapping.ExportDate = DateTime.Now;
                    PartnerMappingDAO.SaveOrUpdateEntity(lObjMapping);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al procesar los socios de negocio.", lObjException));
            }
        }

        private void ProcessAuctions(IList<Auction> pLstObjAuctionsList)
        {
            try
            {
                foreach (Auction lObjAuction in pLstObjAuctionsList)
                {
                    LogUtility.Write("Processing Auction :" + lObjAuction.Folio);
                    ProcessAuction(lObjAuction);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al procesar las subastas.", lObjException));
            }
        }

        private void ProcessAuction(Auction pObjAuction)
        {

            mLstObjGoodsIssues = new List<SAPBatchDTO>();
            List<Invoice> lLstInvoice = new List<Invoice>();
            bool lBoolProcessedJournal = false;

            if (pObjAuction.Category != AuctionCategoryEnum.DIRECT_TRADE)
            {
                using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
                {
                    //Generate GoodsIssues and GoodsReceipts
                    if (!ProcessInventoryStock(pObjAuction, GetUnprocessedBatchesList().Where(x => x.AuctionId == pObjAuction.Id && x.Quantity > 0).ToList(),
                        GetAuctionStockList(pObjAuction.Date), GetAuctionGoodsBuyerdReceipts(pObjAuction.Id).ToList()))
                    {
                        lObjTransaction.Rollback();
                        LogUtility.Write("Can't process Stock (GoodsIssues/GoodsReceipts)");
                        return;
                    }
                    else
                    {
                        lObjTransaction.Commit();
                        LogUtility.Write("Stock(GoodsIssues/GoodsReceipts) Processed");
                    }
                }
                //Generate Food Charges Lines 
                GetFoodCostsBySeller(GetUnprocessedInvoiceList(pObjAuction.Id).ToList(), mLstObjGoodsIssues, pObjAuction);
                LogUtility.Write("Food Charges Processed");

            }

            //Journal entries
            using (var lObjTransaction = AuctionsContext.Database.BeginTransaction())
            {
                lBoolProcessedJournal = ProcessJournalEntries(GetUnprocessedJournalEntriesList(pObjAuction.Id));
                if (lBoolProcessedJournal)
                {
                    lObjTransaction.Commit();
                    LogUtility.Write("Journal Entries Processed");
                }
                else
                {
                    lObjTransaction.Rollback();
                }
            }

            //Create Drafts
            ProcessDrafts(GetUnprocessedInvoiceList(pObjAuction.Id).ToList());
            LogUtility.Write("Drafts Processed");

            //Invoices
            ProcessInvoices(GetUnprocessedInvoiceList(pObjAuction.Id).ToList(), pObjAuction.Id);
            LogUtility.Write("Invoices Processed");

            //Mark as processed
            if (lBoolProcessedJournal)
            {
                pObjAuction.Processed = true;
                pObjAuction.ProcessedDate = DateTime.Now;
                AuctionDAO.SaveOrUpdateEntity(pObjAuction);
                LogUtility.Write("Auction Processed and marked");
            }

        }

        private bool AllInvoicesProcessed(long pLonAuctionId)
        {
            int lIntInvoiceQtty = InvoiceDAO.GetEntitiesList()
                .Where(x => !x.Opened
                    && x.AuctionId == pLonAuctionId
                    && !x.Canceled
                    && !x.Auction.ReOpened).ToList().Count();

            int lIntProcessedInvQtty = InvoiceDAO
                    .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && !x.Exported
                        && !x.Processed
                        && x.AuctionId == pLonAuctionId
                        && !x.Auction.ReOpened)
                    .ToList().Count();

            return lIntInvoiceQtty == lIntProcessedInvQtty ? true : false;
        }

        private bool AllDocumentsProcessed(long pLonAuctionId)
        {
            int lIntJournalQtty = JournalEntryDAO
             .GetEntitiesList()
             .Where(x => !x.Opened
                 && !x.Canceled
                 && x.AuctionId == pLonAuctionId)
             .ToList().Count();

            int lIntProcessedJournalQtty = JournalEntryDAO
             .GetEntitiesList()
             .Where(x => !x.Opened
                 && !x.Canceled
                 && !x.Exported
                 && !x.Processed
                 && x.AuctionId == pLonAuctionId)
             .ToList().Count();

            return (lIntJournalQtty == lIntProcessedJournalQtty) ? true : false;
        }

        private List<Invoice> GetFoodCostsBySeller(List<Invoice> pLstInvoice, List<SAPBatchDTO> mLstObjGoodsIssues, Auction pObjAuction)
        {

            double lDbTotalCharge = 0;
            string lStrThreePercentCode = mObjQueryManager.GetThreePercentArticle();
            string error = string.Empty;

            List<Invoice> lLstInvoices = new List<Invoice>();
            List<InvoiceLine> lLstObjInvoiceLine = new List<InvoiceLine>();


            List<string> lLstFoodCharges = mObjFinancialSerfviceFactory.GetFoodChargeCheckService().GetList()
                .Where(x => mLstObjGoodsIssues.Select(y => y.BatchNumber).Contains(x.BatchNumber) && x.ApplyFoodCharge).Select(x => x.BatchNumber).ToList();

            try
            {
                var lLstSellerCharges = mLstObjGoodsIssues.Where(x => lLstFoodCharges.Contains(x.BatchNumber)).GroupBy(x => x.Seller);

                foreach (var lVarSeller in lLstSellerCharges)
                {
                    if (lVarSeller.Key == "CL00001980")
                    {

                    }
                    InvoiceLine lObjInvoiceLine = pLstInvoice.Where(x => x.CardCode == lVarSeller.Key).SelectMany(x => x.Lines).Where(x => x.ItemCode == lStrThreePercentCode).FirstOrDefault();
                    error = string.Format("{0} {1}", lVarSeller.Key, lObjInvoiceLine.Id);

                    lDbTotalCharge = 0;
                    foreach (var lVarSellerBatch in lVarSeller)
                    {
                        foreach (var lVarBatch in lVarSellerBatch.BatchesList)
                        {
                            lDbTotalCharge += GetFoodCharge(lVarBatch.Weight, lVarBatch.CreationDate, lVarSellerBatch.BatchNumber);
                        }
                    }

                    lObjInvoiceLine.ItemCode = lStrThreePercentCode;
                    lObjInvoiceLine.Quantity = lDbTotalCharge;
                    lObjInvoiceLine.Price = mObjQueryManager.GetThreePercentPrice(lStrThreePercentCode, GetLocation());
                    lObjInvoiceLine.CostingCode = GetCostCenter();
                    lObjInvoiceLine.WarehouseCode = GetWareHouse();
                    lObjInvoiceLine.TaxCode = mObjQueryManager.GetTaxCodeByArticle(lStrThreePercentCode);

                    mObjFinancialSerfviceFactory.GetInvoiceLineService().SaveOrUpdate(lObjInvoiceLine);

                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write("Error in food charges " + lObjException.Message +" "+ error);
            }

            return lLstInvoices;
        }

        private double GetFoodCharge(float pFlWeight, DateTime pCreationDate, string pStrSapBatchNumber)
        {
            double lDbFoodCharge = 0;
            DateTime lll = pCreationDate.AddMinutes(-30);


            double lDbDays = (pCreationDate.AddMinutes(-30) - mObjStockService.LocalStockService.GetListByWhs().Where(x => x.BatchNumber == pStrSapBatchNumber)
                .Select(x => x.CreationDate).FirstOrDefault()
                ).TotalDays;

            return lDbFoodCharge = ((pFlWeight * (lDbDays)) * 0.03);
        }


        private double GetFoodChargeAmount(float pflWeight, string pStrBatchNumber)
        {
            double lDbFoodCharge = 0;
            double lDbtDays = (mObjStockService.LocalStockService.GetListByWhs().Where(x => x.BatchNumber == pStrBatchNumber).Select(x => x.CreationDate).FirstOrDefault()
                - DateTime.Now).Days;

            return lDbFoodCharge = ((pflWeight * (lDbtDays + 1)) * 0.3);
        }

        private void RemoveInvoicedDrafs(IList<Invoice> pLstObjInvoices)
        {
            foreach (var lVarInvoice in pLstObjInvoices)
            {
                RemoveDraft(lVarInvoice);
            }
        }

        private void RemoveDraft(Invoice lVarInvoice)
        {
            SAPbobsCOM.Documents lObjDocument = null;
            try
            {
                lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);

                int lIntDocEntry = GetDraftDocEntry(lVarInvoice.CardCode, lVarInvoice.NumAtCard);

                lObjDocument.GetByKey(lIntDocEntry);

                if (lIntDocEntry > 0)
                {
                    lObjDocument.Remove();
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al eliminar la preliminar.", lObjException));
            }
        }

        private int GetDraftDocEntry(string pStrCardCode, string pStrAuction)
        {
            return QueryManager.GetDraftDocEntry(pStrCardCode, pStrAuction);
        }

        private void ProcessDrafts(IList<Invoice> pLstObjInvoices)
        {
            try
            {
                foreach (Invoice lObjInvoice in pLstObjInvoices)
                {
                    ProcessDraft(lObjInvoice);
                    //Thread.Sleep(10000);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al procesar las preliminares.", lObjException));
                if (!DIApplication.Company.InTransaction)
                {
                    DIApplication.Company.StartTransaction();
                }
            }
        }

        private void ProcessDraft(Invoice pObjInvoice)
        {
            SAPbobsCOM.Documents lObjDocument = null;

            try
            {
                //Populate header
                lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);

                lObjDocument.DocObjectCodeEx = "13";
                lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                lObjDocument.CardCode = pObjInvoice.CardCode;
                lObjDocument.DocDate = pObjInvoice.Date;
                lObjDocument.DocDueDate = pObjInvoice.DueDate;
                lObjDocument.Comments = pObjInvoice.Comments;
                lObjDocument.PaymentMethod = pObjInvoice.PayMethod != null ? pObjInvoice.PayMethod : "99";
                lObjDocument.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = pObjInvoice.MainUsage != null ? pObjInvoice.MainUsage : "P01";
                lObjDocument.Series = GetInvoiceSeries();
                lObjDocument.NumAtCard = pObjInvoice.NumAtCard;

                //Add lines
                if (pObjInvoice.Lines != null && pObjInvoice.Lines.Count > 0)
                {
                    foreach (InvoiceLine lObjLine in pObjInvoice.Lines.Where(x => !x.Removed).ToList())
                    {
                        lObjDocument.Lines.ItemCode = lObjLine.ItemCode;
                        lObjDocument.Lines.WarehouseCode = lObjLine.WarehouseCode;
                        lObjDocument.Lines.TaxCode = lObjLine.TaxCode;
                        lObjDocument.Lines.Quantity = lObjLine.Quantity;
                        lObjDocument.Lines.Price = lObjLine.Price;
                        lObjDocument.Lines.COGSCostingCode = lObjLine.CostingCode;


                        if (lObjLine.DocNum != 0)
                        {
                            lObjDocument.Lines.BaseEntry = lObjLine.DocNum;
                            lObjDocument.Lines.BaseLine = lObjLine.LineNum;
                            lObjDocument.Lines.BaseType = lObjLine.DocType;
                        }

                        lObjDocument.Lines.Add();
                    }
                }

                HandleSapBoOperation(lObjDocument.Add());
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError(string.Format("Error al procesar el borrador del vendedor '{0}' en la subasta '{1}, {2}'.", pObjInvoice.CardCode, pObjInvoice.NumAtCard, lObjException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocument);
            }
        }

        private void GeneratePayments(IList<Invoice> pLstObjInvoices)
        {
            foreach (var lVarInvoice in pLstObjInvoices)
            {
                LogUtility.Write("Creating payment for: " + lVarInvoice.CardCode);
                CreatePayment(lVarInvoice);
                LogUtility.Write("Invoice payed");
                Thread.Sleep(10000);
            }
        }

        private void CreatePayment(Invoice pObjInvoice)
        {
            string lStrCardCode = pObjInvoice.CardCode;
            Invoice lObjInvoice = pObjInvoice;
            double lDoubTotal = mObjQueryManager.GetDocTotal(pObjInvoice.CardCode, pObjInvoice.Auction.Folio);

            int lIntDocEntry = GetInvoiceDocEntry(lStrCardCode, lObjInvoice.NumAtCard);

            try
            {
                if (pObjInvoice.Payment && lIntDocEntry != 0)
                {
                    SAPbobsCOM.Payments lObjPayment = (SAPbobsCOM.Payments)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                    lObjPayment.CardCode = lStrCardCode;

                    lObjPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments;
                    lObjPayment.DocDate = DateTime.Now;
                    lObjPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                    lObjPayment.UserFields.Fields.Item("U_FZ_AuxiliarType").Value = "1";
                    lObjPayment.UserFields.Fields.Item("U_GLO_PaymentType").Value = "GLPGO";
                    lObjPayment.UserFields.Fields.Item("U_FZ_Auxiliar").Value = lObjInvoice.CardCode;
                    lObjPayment.UserFields.Fields.Item("U_FZ_FolioAuction").Value = lObjInvoice.Auction.Folio;
                    lObjPayment.UserFields.Fields.Item("U_FechaPago").Value = lObjInvoice.CreationDate.ToString("dd-MM-yyyy");
                    lObjPayment.UserFields.Fields.Item("U_HoraPago").Value = lObjInvoice.CreationDate.ToString("HH:mm");
                    lObjPayment.UserFields.Fields.Item("U_B1SYS_PmntMethod").Value = "17";

                    lObjPayment.Series = GetPaymentSeries();//GetInvoiceSeries();

                    lObjPayment.CashSum = lDoubTotal;

                    string lstr = GetAccountByType(GetPaymentAccount());

                    lObjPayment.CashAccount = GetAccountByType(GetPaymentAccount());

                    lObjPayment.Invoices.DocEntry = GetInvoiceDocEntry(lStrCardCode, lObjInvoice.NumAtCard);
                    lObjPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                    lObjPayment.Invoices.SumApplied = lDoubTotal;

                    HandleSapBoOperation(lObjPayment.Add());

                    lObjInvoice.Payed = true;
                    lObjInvoice.PayedDate = DateTime.Now;

                    if (!lObjInvoice.Processed)
                    {
                        lObjInvoice.Processed = true;
                        lObjInvoice.ProcessedDate = DateTime.Now;
                    }

                    InvoiceDAO.SaveOrUpdateEntity(lObjInvoice);
                }
            }
            catch (Exception lObjException)
            {
                lObjInvoice.Payed = true;
                lObjInvoice.PayedDate = DateTime.Now;

                if (!lObjInvoice.Processed)
                {
                    lObjInvoice.Processed = true;
                    lObjInvoice.ProcessedDate = DateTime.Now;
                }
                InvoiceDAO.SaveOrUpdateEntity(lObjInvoice);

                ExceptionList.Add(new DAOException("Error al procesar el pago", lObjException));
            }
            finally
            {
                //DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            }
        }



        private double GetTaxImport(Invoice lObjInvoice)
        {
            double lDbImport = 0;

            foreach (var item in lObjInvoice.Lines)
            {

                lDbImport += item.Price * (GetTaxByCode(item.TaxCode) / 100);
            }

            return lDbImport;
        }

        private double GetTaxByCode(string pStrTaxCode)
        {
            return mObjQueryManager.GetTaxImport(pStrTaxCode);
        }


        private int GetInvoiceDocEntry(string pStrCode, string pStrNumAtCard)
        {
            return QueryManager.GetDocEntryByAuctionAndPartner(pStrCode, pStrNumAtCard);
        }

        private List<long?> GetSellersByAuction(Auction pObjAuction)
        {

            List<long?> ll = pObjAuction.Batches.Select(x => (long?)x.SellerId).ToList();

            return pObjAuction.Batches.Select(x => (long?)x.SellerId).Distinct().ToList();

        }

        private IList<GoodsReceipt> GetAuctionGoodsBuyerdReceipts(long pLonAuctionId)
        {
            List<GoodsReceipt> lLstObjGoodsReceipt = new List<GoodsReceipt>();
            LogUtility.Write("getting goods receipts");
            if (!AuctionDAO.GetEntity(pLonAuctionId).ReOpened)
            {
                lLstObjGoodsReceipt = GetUnprocessedAndSoldBatchesList().ToList().Where(x => x.AuctionId == pLonAuctionId).Select(x => new GoodsReceipt()
                {
                    Quantity = x.Quantity - x.GoodsReturns.Where(y => !y.Removed).Sum(y => y.Quantity) -
                    ((x.GoodsIssues.Sum(y => y.Quantity) - x.GoodsReturns.Where(y => !y.Removed && y.Delivered).Sum(y => y.Quantity))),
                    BatchNumber = x.Buyer != null ? string.Format("{0}",
                     DateTime.Now.ToString("yyMMddHHmmss")) : string.Empty,
                    ItemId = Convert.ToInt64(x.ItemTypeId),
                    Customer = x.Buyer
                }).ToList();
                LogUtility.Write("getting goods receipts step 2");

                if (GetUnprocessedAndSoldBatchesList().ToList().Where(x => x.AuctionId == pLonAuctionId).SelectMany(x => x.GoodsReturns).Count() > 0)
                {
                    lLstObjGoodsReceipt.AddRange(GetUnprocessedAndSoldBatchesList().ToList().Where(x => x.AuctionId == pLonAuctionId && x.GoodsReturns.Count > 0).Select(x => new GoodsReceipt()
                    {
                        Quantity = x.GoodsReturns.Count > 0 ? x.GoodsReturns.Where(d => !d.Removed).Sum(y => y.Quantity) : 0,
                        BatchNumber = x.GoodsReturns.Count > 0 ? string.Format("{0}", DateTime.Now.ToString("yyMMddHHmmss")) : string.Empty,
                        ItemId = Convert.ToInt64(x.ItemTypeId),
                        Customer = x.Seller
                    }).ToList());
                }
                LogUtility.Write("getting goods receipts step 3");
                if (GetUnprocessedBatchesList().ToList().Where(x => x.AuctionId == pLonAuctionId && x.Unsold).Select(x => x).Count() > 0)
                {

                    lLstObjGoodsReceipt.AddRange(
                        GetUnprocessedNotSoldBatchesList().ToList().Where(x => x.AuctionId == pLonAuctionId && !x.Reprogrammed)
                        .GroupBy(x => new { seller = x.Seller, gender = x.ItemType.Gender })
                        .Select(x => new GoodsReceipt()
                        {
                            Quantity = GetNotSoldQuantity(x.Key.seller.Id, pLonAuctionId, x.Key.gender),
                            BatchNumber = x.Key.seller != null ? string.Format("{0}", DateTime.Now.ToString("yyMMddHHmmss")) : string.Empty,
                            ItemId = (long)x.Select(y => y.ItemTypeId).FirstOrDefault(),
                            Customer = x.Key.seller
                        }).ToList()
                        );


                }
            }
            LogUtility.Write("getting goods receipts done");
            return lLstObjGoodsReceipt;
        }

        private int GetNotSoldQuantity(long? pLonSellerId, long pLonAuctionId, ItemTypeGenderEnum itemTypeGenderEnumtemId)
        {

            int lIntNotSales = GetUnprocessedBatchesList()
               .Where(x => x.AuctionId == pLonAuctionId
                   && x.Unsold
                   && !x.Reprogrammed
                   && x.ItemType.Gender == itemTypeGenderEnumtemId
                   && x.SellerId == pLonSellerId)
               .Sum(x => (int?)x.Quantity) ?? 0;

            int lIntReprogramedSales = GetUnprocessedBatchesList()
               .Where(x => x.AuctionId == pLonAuctionId
                   && !x.Unsold
                   && x.Reprogrammed
                   && x.ItemType.Gender == itemTypeGenderEnumtemId
                   && x.SellerId == pLonSellerId)
                   .Sum(x => (int?)x.Quantity) ?? 0;

            return (lIntNotSales - lIntReprogramedSales);
        }


        private void ProcessInvoices(IList<Invoice> pLstObjInvoicesList, long pLonAuctionId)
        {
            try
            {
                LogUtility.Write("Invoice List: " + pLstObjInvoicesList.Count.ToString());
                foreach (Invoice lObjInvoice in pLstObjInvoicesList)
                {
                    LogUtility.Write("Processing client's invoice: " + lObjInvoice.CardCode);
                    ProcessInvoice(lObjInvoice);
                    LogUtility.Write("Invoice Processed");
                    //Thread.Sleep(10000);
                }
            }
            catch (Exception lObjException)
            {
                IList<Invoice> lLstInvoices = GetUnprocessedInvoiceList(pLonAuctionId).ToList();
                if (lLstInvoices.Count != 0)
                {
                    ProcessInvoices(lLstInvoices, pLonAuctionId);
                }
            }
        }

        private void ProcessInvoice(Invoice pObjInvoice)
        {
            SAPbobsCOM.Documents lObjDocument = null;
            FoodDelivery lObjFoodDeliveries = new FoodDelivery();
            try
            {
                //Populate header
                lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                lObjDocument.CardCode = pObjInvoice.CardCode;
                lObjDocument.DocDate = pObjInvoice.Date;
                lObjDocument.DocDueDate = pObjInvoice.DueDate;
                lObjDocument.Comments = pObjInvoice.Comments;
                lObjDocument.PaymentGroupCode = GetPaymentCondition(pObjInvoice);
                lObjDocument.PaymentMethod = pObjInvoice.PayMethod != null ? pObjInvoice.PayMethod : "17";
                lObjDocument.UserFields.Fields.Item("U_B1SYS_MainUsage").Value = pObjInvoice.MainUsage != null ? pObjInvoice.MainUsage : "P01";

                lObjDocument.Series = GetInvoiceSeries();
                lObjDocument.NumAtCard = pObjInvoice.NumAtCard;

                LogUtility.Write("Head Done");
                //Add lines

                if (pObjInvoice.Lines != null && pObjInvoice.Lines.Count > 0)
                {
                    foreach (InvoiceLine lObjLine in pObjInvoice.Lines.Where(x => !x.Removed).ToList())
                    {

                        lObjDocument.Lines.ItemCode = lObjLine.ItemCode;
                        lObjDocument.Lines.WarehouseCode = lObjLine.WarehouseCode;
                        lObjDocument.Lines.TaxCode = lObjLine.TaxCode;
                        lObjDocument.Lines.Quantity = lObjLine.Quantity;
                        lObjDocument.Lines.Price = lObjLine.Price;
                        lObjDocument.Lines.COGSCostingCode = lObjLine.CostingCode;

                        if (lObjLine.DocNum != 0)
                        {
                            lObjDocument.Lines.BaseEntry = lObjLine.DocNum;
                            lObjDocument.Lines.BaseLine = lObjLine.LineNum;
                            lObjDocument.Lines.BaseType = lObjLine.DocType;

                            //lObjFoodDeliveries = mObjFinancialSerfviceFactory.GetDeliveryFoodService().GetList().Where(x => x.DocEntry == lObjLine.DocNum).FirstOrDefault();
                            //lObjFoodDeliveries.Processed = true;
                            //lObjFoodDeliveries.Exported = true;
                            //lObjFoodDeliveries.ExportDate = DateTime.Now;
                            //lObjFoodDeliveries.ProcessedDate = DateTime.Now;

                            //mObjFinancialSerfviceFactory.GetDeliveryFoodService().SaveOrUpdate(lObjFoodDeliveries);
                        }

                        lObjDocument.Lines.Add();

                        lObjLine.Exported = true;
                        lObjLine.ExportDate = DateTime.Now;
                        InvoiceLineDAO.SaveOrUpdateEntity(lObjLine);


                    }
                }
                lObjDocument.EDocGenerationType = SAPbobsCOM.EDocGenerationTypeEnum.edocGenerateLater;

                lObjDocument.EDocExportFormat = mObjQueryManager.GetExportFormat();


                //Handle operation
                HandleSapBoOperation(lObjDocument.Add());


                //Mark as exported and processed
                pObjInvoice.PaymentCondition = lObjDocument.PaymentGroupCode;
                pObjInvoice.Exported = true;
                pObjInvoice.ExportDate = DateTime.Now;
                pObjInvoice.Processed = true;
                pObjInvoice.ProcessedDate = DateTime.Now;
                InvoiceDAO.SaveOrUpdateEntity(pObjInvoice);

            }
            catch (Exception lObjException)
            {
                pObjInvoice.PaymentCondition = lObjDocument.PaymentGroupCode;
                pObjInvoice.Exported = true;
                pObjInvoice.ExportDate = DateTime.Now;
                pObjInvoice.Processed = true;
                pObjInvoice.ProcessedDate = DateTime.Now;
                pObjInvoice.Payed = true;
                pObjInvoice.PayedDate = DateTime.Now;
                InvoiceDAO.SaveOrUpdateEntity(pObjInvoice);
                LogUtility.WriteError(string.Format("Error al procesar la factura del vendedor '{0}' en la subasta '{1}, {2} - {3}'."
                    , pObjInvoice.CardCode, pObjInvoice.NumAtCard, lObjException.Message, lObjException.InnerException.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocument);
            }
        }


        private int GetPaymentCondition(Invoice pObjInvoice)
        {
            switch (pObjInvoice.PaymentCondition)
            {
                case 0:
                    return mObjQueryManager.GetPaymentCondition(pObjInvoice.CardCode);
                default:
                    return -1;
            }
        }

        private bool ProcessJournalEntries(IList<JournalEntry> pLstObjJournalEntriesList)
        {
            bool lBoolProcessed = true;
            try
            {
                foreach (JournalEntry lObjJournalEntry in pLstObjJournalEntriesList)
                {
                    lBoolProcessed = ProcessJournalEntry(lObjJournalEntry);
                }
            }
            catch (Exception lObjException)
            {
                lBoolProcessed = false;
            }
            return lBoolProcessed;
        }

        private bool ProcessJournalEntry(JournalEntry pObjJournalEntry)
        {
            SAPbobsCOM.JournalEntries lObjJournalEntry = null;
            bool lBoolJournalProcessed = true;
            try
            {
                //Pupulate header
                lObjJournalEntry = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                lObjJournalEntry.DueDate = DateTime.Today;
                lObjJournalEntry.TaxDate = DateTime.Today;
                lObjJournalEntry.AutoVAT = SAPbobsCOM.BoYesNoEnum.tYES;
                lObjJournalEntry.TransactionCode = "SUB";
                lObjJournalEntry.Reference = pObjJournalEntry.Auction.Folio;
                lObjJournalEntry.Series = GetJournalEntrySeries();
                lObjJournalEntry.Memo = "Cierre de subasta " + DateTime.Now.ToShortDateString();

                //Add lines
                if (pObjJournalEntry.Lines != null && pObjJournalEntry.Lines.Count > 0)
                {
                    foreach (JournalEntryLine lObjLine in pObjJournalEntry.Lines)
                    {
                        lObjJournalEntry.Lines.AccountCode = GetAccountByType(lObjLine.AccountCode);
                        lObjJournalEntry.Lines.ContraAccount = GetAccountByType(lObjLine.ContraAccount);
                        lObjJournalEntry.Lines.CostingCode = lObjLine.CostingCode;
                        lObjJournalEntry.Lines.Credit = lObjLine.Credit;
                        lObjJournalEntry.Lines.Debit = lObjLine.Debit;
                        lObjJournalEntry.Lines.Reference1 = lObjLine.Reference;
                        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Auxiliar").Value = lObjLine.Auxiliary;
                        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_TypeAux").Value = lObjLine.AuxiliaryType.ToString();
                        lObjJournalEntry.Lines.UserFields.Fields.Item("U_SU_Folio").Value = pObjJournalEntry.Auction.Folio;
                        lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_Coments").Value = lObjLine.Commentaries != null ? lObjLine.Commentaries : "";
                        if (lObjLine.Reference.Equals("Guía"))
                        {
                            lObjJournalEntry.Lines.UserFields.Fields.Item("U_GLO_CodeMov").Value = "1";
                        }
                        lObjJournalEntry.Lines.Add();

                        lObjLine.Exported = true;
                        lObjLine.ExportDate = DateTime.Now;
                        JournalEntryLineDAO.SaveOrUpdateEntity(lObjLine);
                    }
                }

                //Handle operation
                DIApplication.Company.StartTransaction();
                HandleSapBoOperation(lObjJournalEntry.Add());

                pObjJournalEntry.Exported = true;
                pObjJournalEntry.ExportDate = DateTime.Now;
                pObjJournalEntry.Processed = true;
                pObjJournalEntry.ProcessedDate = DateTime.Now;
                JournalEntryDAO.SaveOrUpdateEntity(pObjJournalEntry);

                lBoolJournalProcessed = true;
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al procesar el asiento contable de la subasta '{0}'.", pObjJournalEntry.Auction.Folio), lObjException));
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                lBoolJournalProcessed = false;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjJournalEntry);
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            }
            return lBoolJournalProcessed;
        }

        private string GetAccountByType(string pStrType)
        {
            return mObjQueryManager.GetAccountByType(pStrType);
        }

        private void ProcessSellerTemporaryGoodsReceipts(IList<GoodsReceipt> pLstObjGoodsReceiptsList)
        {
            try
            {
                foreach (GoodsReceipt lObjGoodsReceipt in pLstObjGoodsReceiptsList)
                {
                    ProcessSellerTemporaryGoodsReceipt(lObjGoodsReceipt);
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException("Error al exportar las entradas temporales del los vendedores.", lObjException));
                if (!DIApplication.Company.InTransaction)
                {
                    DIApplication.Company.StartTransaction();
                }
            }
        }

        private void ProcessSellerTemporaryGoodsReceipt(GoodsReceipt pObjGoodsReceipt)
        {
            SAPbobsCOM.Documents lObjDocument = null;

            try
            {
                //Populate header
                lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                lObjDocument.Series = GetGoodsReceiptSeries();
                lObjDocument.DocDate = pObjGoodsReceipt.CreationDate;
                lObjDocument.DocDueDate = pObjGoodsReceipt.CreationDate;

                if (!string.IsNullOrEmpty(pObjGoodsReceipt.Folio))
                {
                    lObjDocument.UserFields.Fields.Item("U_GLO_Ticket").Value = pObjGoodsReceipt.Folio;
                }

                lObjDocument.Comments = pObjGoodsReceipt.Remarks;
                lObjDocument.JournalMemo = "Entrada de mercancías";

                //Pupulate line
                lObjDocument.Lines.ItemCode = pObjGoodsReceipt.Item.Code;
                lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
                lObjDocument.Lines.Quantity = pObjGoodsReceipt.Quantity;

                //Pupulate batch number
                lObjDocument.Lines.BatchNumbers.Quantity = pObjGoodsReceipt.Quantity;
                lObjDocument.Lines.BatchNumbers.BatchNumber = pObjGoodsReceipt.BatchNumber;
                lObjDocument.Lines.BatchNumbers.ExpiryDate = pObjGoodsReceipt.BatchDate;
                lObjDocument.Lines.BatchNumbers.AddmisionDate = pObjGoodsReceipt.BatchDate;
                lObjDocument.Lines.BatchNumbers.ManufacturerSerialNumber = pObjGoodsReceipt.Customer.Code;
                lObjDocument.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = pObjGoodsReceipt.BatchDate.ToString("Hm");
                lObjDocument.Lines.BatchNumbers.Add();

                //Add line
                lObjDocument.Lines.Add();

                //handle operation
                HandleSapBoOperation(lObjDocument.Add());

                //Mark as exported and processed
                pObjGoodsReceipt.Exported = true;
                pObjGoodsReceipt.ExportDate = DateTime.Now;
                pObjGoodsReceipt.Processed = true;
                pObjGoodsReceipt.ProcessedDate = DateTime.Now;
                GoodsReceiptDAO.SaveOrUpdateEntity(pObjGoodsReceipt);
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al exportar la entrada temporal de ganado del vendedor '{0}'.", pObjGoodsReceipt.Customer.Code), lObjException));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocument);
            }
        }

        private bool ProcessInventoryStock(Auction pObjAuction, IList<Batch> pLstObjBatches, IList<Stock> pLstLocalAuctStock, IList<GoodsReceipt> pLstGoodsReceipt)
        {
            SAPbobsCOM.Documents lObjGoodsIssues = null;
            SAPbobsCOM.Documents lObjGoodsReceipts = null;

            lObjGoodsIssues = pLstObjBatches.Count > 0 ? ProcessAuctionsGoodsIssues(pObjAuction, pLstObjBatches, pLstLocalAuctStock) : null;
            LogUtility.Write("GoodsIssues prepared");
            lObjGoodsReceipts = pLstGoodsReceipt.Count > 0 ? ProcessAuctionsGoodsReceipts(pObjAuction, pLstGoodsReceipt) : null;
            LogUtility.Write("Goodsreceipts prepared");

            try
            {

                if (lObjGoodsIssues != null && lObjGoodsReceipts != null)
                {
                    DIApplication.Company.StartTransaction();
                    HandleSapBoOperation(lObjGoodsIssues.Add());
                    LogUtility.Write("GoodsIssues Done");
                    HandleSapBoOperation(lObjGoodsReceipts.Add());
                    LogUtility.Write("GoodsReceipts Done");
                    DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    LogUtility.Write("Inventory Donde");
                }

                //Mas as exported and processed
                GoodsIssueDAO.SaveOrUpdateEntitiesList(GetUnprocessedAndSoldBatchesList()
                .SelectMany(b => b.GoodsIssues)
                .Where(x => !x.Removed)
                .Select(g =>
                {
                    g.Exported = true;
                    g.ExportDate = DateTime.Now;
                    g.Processed = true;
                    g.ProcessedDate = DateTime.Now;
                    return g;
                })
                .ToList());
                LogUtility.Write("GoodIssues Processed and marked");

                GoodsReturnDAO.SaveOrUpdateEntitiesList(GetUnprocessedAndSoldBatchesList()
                .SelectMany(b => b.GoodsReturns)
                .Where(x => !x.Removed)
                .Select(g =>
                {
                    g.Exported = true;
                    g.ExportDate = DateTime.Now;
                    g.Processed = true;
                    g.ProcessedDate = DateTime.Now;
                    return g;
                })
                .ToList());
                LogUtility.Write("GoodsReturns Processed and marked");

                BatchDAO.SaveOrUpdateEntitiesList(GetUnprocessedBatchesList().Where(x => x.AuctionId == pObjAuction.Id && x.Quantity > 0).Select(
                    g =>
                    {
                        g.Exported = true;
                        g.ExportDate = DateTime.Now;
                        return g;
                    }
                    ).ToList());
                LogUtility.Write("Batches Processed and marked as exported");
            }
            catch (Exception lObjException)
            {
                LogUtility.Write("Inventory failed");
                LogUtility.WriteError("Error " + lObjException.Message + " " + lObjException.InnerException);
                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                return false;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjGoodsIssues);
                MemoryUtility.ReleaseComObject(lObjGoodsReceipts);
            }

            return true;
        }

        private SAPbobsCOM.Documents ProcessAuctionsGoodsIssues(Auction pObjAuction, IList<Batch> pLstObjBatches, IList<Stock> pLstLocalAuctStock)
        {
            SAPbobsCOM.Documents lObjDocument = null;
            string lStrActualBatch = string.Empty;

            List<SAPBatchDTO> lLstObjGoodsIssues = GetGoodIssues(pLstObjBatches, pLstLocalAuctStock);

            mLstObjGoodsIssues = lLstObjGoodsIssues;

            var lVarGBatch = lLstObjGoodsIssues.GroupBy(X => X.SapArticle).Select(x => x).ToList();
            try
            {
                if (lLstObjGoodsIssues.Count > 0)
                {
                    if (!pObjAuction.ReOpened)
                    {
                        //Populate header
                        lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
                        lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                        lObjDocument.Series = GetGoodsIssueSeries();
                        lObjDocument.DocDate = DateTime.Now;
                        lObjDocument.DocDueDate = DateTime.Now;
                        lObjDocument.Comments = string.Format("Salidas de mercancías de los vendores de la subasta {0}", pObjAuction.Folio);
                        lObjDocument.UserFields.Fields.Item("U_GLO_InMo").Value = "S-GAN";
                        lObjDocument.JournalMemo = "Salidas de mercancías";

                        //Add line for each goods issue

                        foreach (var lVarBatch in lVarGBatch)
                        {
                            //Pupulate line
                            lObjDocument.Lines.ItemCode = lVarBatch.Key;

                            lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
                            int xd = lVarBatch.Where(x => x.SapArticle == lVarBatch.Key).Sum(x => x.Quantity);
                            lObjDocument.Lines.Quantity = lVarBatch.Where(x => x.SapArticle == lVarBatch.Key).Sum(x => x.Quantity);

                            foreach (var item in lVarBatch)
                            {
                                lObjDocument.Lines.BatchNumbers.Quantity = item.Quantity;

                                lStrActualBatch = item.BatchNumber;
                                lObjDocument.Lines.BatchNumbers.BatchNumber = item.BatchNumber;
                                lObjDocument.Lines.BatchNumbers.Add();
                            }
                            lObjDocument.Lines.Add();
                        }

                    }
                }

            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError("Can't process Goodsissues, error with: " + lStrActualBatch);
                ExceptionList.Add(new DAOException(string.Format("Error al exportar las salidas de ganado de los vendedores de la subasta '{0}'.", pObjAuction.Folio), lObjException));
            }

            return lObjDocument;
        }

        private SAPbobsCOM.Documents ProcessAuctionsGoodsReceipts(Auction pObjAuction, IList<GoodsReceipt> pLstGoodsReceipt)
        {
            SAPbobsCOM.Documents lObjDocument = null;
            var lVarX = pLstGoodsReceipt.Where(x => x.Quantity > 0).ToList();
            var lVarGoodsReceipts = lVarX.GroupBy(x => new { sapItem = GetSAPArticle(x.ItemId) }).ToList();
            try
            {
                if (!pObjAuction.ReOpened && lVarGoodsReceipts.Count > 0)
                {
                    //Populate header
                    lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                    lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;

                    lObjDocument.Series = GetGoodsReceiptSeries();
                    lObjDocument.DocDate = DateTime.Now;
                    lObjDocument.DocDueDate = DateTime.Now;
                    lObjDocument.UserFields.Fields.Item("U_GLO_InMo").Value = "E-GAN";
                    lObjDocument.UserFields.Fields.Item("U_GLO_BusinessPartner").Value = "XXXX";
                    lObjDocument.UserFields.Fields.Item("U_GLO_Guide").Value = "XXXX";
                    lObjDocument.UserFields.Fields.Item("U_GLO_CheckIn").Value = DateTime.Now.ToString("HH:mm");
                    lObjDocument.Comments = string.Format("Entradas de mercancías de los compradores de la subasta {0}", pObjAuction.Folio);
                    lObjDocument.JournalMemo = "Entrada de mercancías";

                    //Add line for each goods issue

                    foreach (var lvarGoodReceipt in lVarGoodsReceipts)
                    {
                        lObjDocument.Lines.ItemCode = lvarGoodReceipt.Key.sapItem;
                        string dede = lvarGoodReceipt.Key.sapItem;

                        lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
                        int ded = lvarGoodReceipt.Where(x => GetSAPArticle(x.ItemId) == lvarGoodReceipt.Key.sapItem.ToString()).Sum(x => x.Quantity);
                        lObjDocument.Lines.Quantity = lvarGoodReceipt.Where(x => GetSAPArticle(x.ItemId) == lvarGoodReceipt.Key.sapItem.ToString()).Sum(x => x.Quantity);

                        foreach (var item in lvarGoodReceipt)
                        {
                            lObjDocument.Lines.BatchNumbers.Quantity = item.Quantity;
                            lObjDocument.Lines.BatchNumbers.BatchNumber = item.BatchNumber;
                            lObjDocument.Lines.BatchNumbers.AddmisionDate = DateTime.Now;
                            lObjDocument.Lines.BatchNumbers.ManufacturerSerialNumber = item.Customer.Code;
                            lObjDocument.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = DateTime.Now.ToString("HH:mm");

                            lObjDocument.Lines.BatchNumbers.Add();

                        }
                        lObjDocument.Lines.Add();
                    }
                }
                LogUtility.Write("Goods receipts done");
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al exportar las entradas de ganado de los compradores de la subasta '{0}'.", pObjAuction.Folio), lObjException));
            }

            return lObjDocument;
        }

        //private void ProcessAuctionsGoodsIssues(Auction pObjAuction,/* IList<GoodsIssue> pLstObjGoodsIssuesList*/ IList<Batch> pLstObjBatches, IList<Stock> pLstLocalAuctStock)
        //{


        //    SAPbobsCOM.Documents lObjDocument = null;


        //    List<SAPBatchDTO> lLstObjGoodsIssues = GetGoodIssues(pLstObjBatches, pLstLocalAuctStock);

        //    mLstObjGoodsIssues = lLstObjGoodsIssues;

        //    var lVarGBatch = lLstObjGoodsIssues.GroupBy(X => X.SapArticle).Select(x => x).ToList();
        //    try
        //    {
        //        if (lLstObjGoodsIssues.Count > 0)
        //        {
        //            if (!pObjAuction.ReOpened)
        //            {
        //                //Populate header
        //                lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
        //                lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
        //                lObjDocument.Series = GetGoodsIssueSeries();
        //                lObjDocument.DocDate = DateTime.Now;
        //                lObjDocument.DocDueDate = DateTime.Now;
        //                lObjDocument.Comments = string.Format("Salidas de mercancías de los vendores de la subasta {0}", pObjAuction.Folio);
        //                lObjDocument.UserFields.Fields.Item("U_GLO_InMo").Value = "S-GAN";
        //                lObjDocument.JournalMemo = "Salidas de mercancías";

        //                //Add line for each goods issue

        //                foreach (var lVarBatch in lVarGBatch)
        //                {
        //                    //Pupulate line
        //                    lObjDocument.Lines.ItemCode = lVarBatch.Key;

        //                    lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
        //                    int xd = lVarBatch.Where(x => x.SapArticle == lVarBatch.Key).Sum(x => x.Quantity);
        //                    lObjDocument.Lines.Quantity = lVarBatch.Where(x => x.SapArticle == lVarBatch.Key).Sum(x => x.Quantity);

        //                    foreach (var item in lVarBatch)
        //                    {
        //                        lObjDocument.Lines.BatchNumbers.Quantity = item.Quantity;

        //                        lObjDocument.Lines.BatchNumbers.BatchNumber = item.BatchNumber;
        //                        lObjDocument.Lines.BatchNumbers.Add();
        //                    }
        //                    lObjDocument.Lines.Add();
        //                }
        //                DIApplication.Company.StartTransaction();
        //                HandleSapBoOperation(lObjDocument.Add());
        //                DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
        //            }
        //        }
        //    }
        //    catch (Exception lObjException)
        //    {
        //        ExceptionList.Add(new DAOException(string.Format("Error al exportar las salidas de ganado de los vendedores de la subasta '{0}'.", pObjAuction.Folio), lObjException));
        //        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        //        //if (!DIApplication.Company.InTransaction)
        //        //{
        //        //    DIApplication.Company.StartTransaction();
        //        //}

        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjDocument);
        //    }
        //}

        //private void ProcessAuctionsGoodsReceipts(Auction pObjAuction, IList<GoodsReceipt> pLstGoodsReceipt)
        //{
        //    SAPbobsCOM.Documents lObjDocument = null;
        //    var lVarX = pLstGoodsReceipt.Where(x => x.Quantity > 0).ToList();
        //    var lVarGoodsReceipts = lVarX.GroupBy(x => new { sapItem = GetSAPArticle(x.ItemId) }).ToList();
        //    try
        //    {
        //        if (!pObjAuction.ReOpened && lVarGoodsReceipts.Count > 0)
        //        {
        //            //Populate header
        //            lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
        //            lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;

        //            lObjDocument.Series = GetGoodsReceiptSeries();
        //            lObjDocument.DocDate = DateTime.Now;
        //            lObjDocument.DocDueDate = DateTime.Now;
        //            lObjDocument.UserFields.Fields.Item("U_GLO_InMo").Value = "E-GAN";
        //            lObjDocument.UserFields.Fields.Item("U_GLO_BusinessPartner").Value = "XXXX";
        //            lObjDocument.UserFields.Fields.Item("U_GLO_Guide").Value = "XXXX";
        //            lObjDocument.UserFields.Fields.Item("U_GLO_CheckIn").Value = DateTime.Now.ToString("HH:mm");
        //            lObjDocument.Comments = string.Format("Entradas de mercancías de los compradores de la subasta {0}", pObjAuction.Folio);
        //            lObjDocument.JournalMemo = "Entrada de mercancías";

        //            //Add line for each goods issue

        //            foreach (var lvarGoodReceipt in lVarGoodsReceipts)
        //            {
        //                lObjDocument.Lines.ItemCode = lvarGoodReceipt.Key.sapItem;
        //                string dede = lvarGoodReceipt.Key.sapItem;

        //                lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
        //                int ded = lvarGoodReceipt.Where(x => GetSAPArticle(x.ItemId) == lvarGoodReceipt.Key.sapItem.ToString()).Sum(x => x.Quantity);
        //                lObjDocument.Lines.Quantity = lvarGoodReceipt.Where(x => GetSAPArticle(x.ItemId) == lvarGoodReceipt.Key.sapItem.ToString()).Sum(x => x.Quantity);

        //                foreach (var item in lvarGoodReceipt)
        //                {
        //                    lObjDocument.Lines.BatchNumbers.Quantity = item.Quantity;
        //                    lObjDocument.Lines.BatchNumbers.BatchNumber = item.BatchNumber;
        //                    //lObjDocument.Lines.BatchNumbers.ExpiryDate = DateTime.Now;
        //                    lObjDocument.Lines.BatchNumbers.AddmisionDate = DateTime.Now;
        //                    lObjDocument.Lines.BatchNumbers.ManufacturerSerialNumber = item.Customer.Code;
        //                    lObjDocument.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = DateTime.Now.ToString("HH:mm");

        //                    lObjDocument.Lines.BatchNumbers.Add();

        //                }
        //                lObjDocument.Lines.Add();
        //            }

        //            DIApplication.Company.StartTransaction();
        //            HandleSapBoOperation(lObjDocument.Add());
        //            DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
        //        }
        //        LogUtility.Write("Goods receipts done");
        //    }
        //    catch (Exception lObjException)
        //    {
        //        ExceptionList.Add(new DAOException(string.Format("Error al exportar las entradas de ganado de los compradores de la subasta '{0}'.", pObjAuction.Folio), lObjException));

        //        DIApplication.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        //        //if (!DIApplication.Company.InTransaction)
        //        //{
        //        //    DIApplication.Company.StartTransaction();
        //        //}

        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjDocument);
        //    }
        //}

        private string GetSAPArticle(long pLonItemtypeId)
        {
            return mObjInventoryFactory.GetItemDefinitionService().GetArticle(pLonItemtypeId);
        }

        private List<SAPBatchDTO> GetGoodIssues(IList<Batch> pLstObjBatches, IList<Stock> pLstLocalAuctStock)
        {
            List<SAPBatchDTO> lLstObjSAPBatch = new List<SAPBatchDTO>();
            List<BatchDTO> lLstMissingBatches = new List<BatchDTO>();
            List<Batch> lLstTempBatch = null;
            bool lBoolCreateBatch = false;

            List<BatchDTO> lLstObjSAPBatches = pLstObjBatches.Where(x => !x.Reprogrammed)
                .GroupBy(x => new { sellerId = x.SellerId, articleId = (mObjInventoryFactory.GetItemDefinitionService().GetArticleRelation((long?)x.ItemTypeId)) })
                .Select(cl => new BatchDTO
                {
                    SellerId = cl.Key.sellerId,
                    Seller = cl.Select(x => x.Seller.Code).FirstOrDefault(),
                    ArticleId = cl.Key.articleId,
                    Quantity = cl.Sum(c => c.Quantity),
                    Gender = cl.Select(t => t.ItemType.Gender).FirstOrDefault(),
                    Weight = cl.Sum(w => w.Weight),
                    BatchesList = cl.Select(x => x).ToList()
                }).ToList();

            foreach (var lVarBatches in lLstObjSAPBatches)
            {
                int lIntHeads = lVarBatches.Quantity;
                List<Batch> lLstBatches = lVarBatches.BatchesList;

                foreach (var lVarStock in pLstLocalAuctStock)
                {
                    lBoolCreateBatch = false;
                    SAPBatchDTO lObjSAPBatch = new SAPBatchDTO();
                    lObjSAPBatch.Gender = lVarBatches.Gender;
                    if (lIntHeads > 0)
                    {
                        if (lVarStock.CustomerId == lVarBatches.SellerId)
                        {
                            if (lVarStock.CustomerId == 1980)
                            { 
                            }

                            if (lVarStock.ItemId == lVarBatches.ArticleId)
                            {
                                if ((lVarStock.Quantity >= lIntHeads) && lVarStock.Quantity != 0)
                                {
                                    lObjSAPBatch.Quantity = lIntHeads;
                                    lIntHeads -= lVarStock.Quantity;
                                    lObjSAPBatch.Weight = lVarBatches.Weight;
                                    lObjSAPBatch.Seller = lVarBatches.Seller;
                                    lVarStock.Quantity = lVarStock.Quantity - lObjSAPBatch.Quantity;
                                    lBoolCreateBatch = true;
                                }
                                else if ((lVarStock.Quantity < lIntHeads) && lVarStock.Quantity != 0)
                                {
                                    lObjSAPBatch.Quantity = lVarStock.Quantity;
                                    lIntHeads = lIntHeads - lVarStock.Quantity;
                                    lObjSAPBatch.Weight = CalculateWeight(lVarBatches.Weight, lVarStock.Quantity, lIntHeads);
                                    lObjSAPBatch.Seller = lVarBatches.Seller;
                                    lVarStock.Quantity -= lVarStock.Quantity;
                                    lVarBatches.Weight -= lObjSAPBatch.Weight;
                                    lLstTempBatch = lLstBatches;
                                    lLstTempBatch = CalculateBatches(lLstTempBatch, lObjSAPBatch.Quantity, lIntHeads).ToList();

                                    lBoolCreateBatch = true;
                                }
                            }
                        }

                        if (lBoolCreateBatch)
                        {
                            lObjSAPBatch.BatchesList = lLstTempBatch != null ? lLstTempBatch : lLstBatches;
                            //lObjSAPBatch.BatchesList = lLstBatches;
                            lObjSAPBatch.BatchNumber = lVarStock.BatchNumber;
                            lObjSAPBatch.SapArticle = lVarStock.Item.Code;
                            lLstObjSAPBatch.Add(lObjSAPBatch);

                            if (lLstTempBatch != null)
                            {
                                lLstBatches = lLstBatches.Where(x => lLstTempBatch.Any(y => y.Id == x.Id && y.Quantity != x.Quantity)).Select(x =>
                                {
                                    x.Quantity = x.Quantity - lLstTempBatch.Where(y => y.Id == x.Id).Select(y => y.Quantity).FirstOrDefault();
                                    x.Weight = x.Weight - lLstTempBatch.Where(y => y.Id == x.Id).Select(y => y.Weight).FirstOrDefault();
                                    return x;
                                }).ToList();


                                lLstTempBatch = null;
                            }
                        }


                    }
                }

                if ((lIntHeads == lVarBatches.Quantity) || (lIntHeads > 0))
                {
                    lVarBatches.Quantity = lIntHeads;
                    lLstMissingBatches.Add(lVarBatches);
                }

            }

            if (lLstMissingBatches.Count > 0)
            {
                LogUtility.Write("Searching Missing Batches");
                List<SAPBatchDTO> lMissingBatches = SearchSAPBatch(lLstMissingBatches, pLstLocalAuctStock.Where(x => x.Quantity > 0).ToList());

                if (lMissingBatches.Count > 0)
                {
                    lLstObjSAPBatch.AddRange(lMissingBatches);
                }

            }

            return lLstObjSAPBatch;
        }

        private IList<Batch> CalculateBatches(IList<Batch> pLstBatches, int pIntTotQtty, int pIntHeads)
        {

            IList<Batch> lLstBatches = new List<Batch>();
            int lIntQtty = pIntTotQtty;
            int lIntQttyLeft = pIntHeads;


            foreach (var item in pLstBatches)
            {
                Batch lObjBatch = new Batch();

                lObjBatch.Id = item.Id;
                int lIntTQtty = item.Quantity;

                if (lIntTQtty > 0 && lIntTQtty >= lIntQtty && lIntQtty != 0)
                {
                    lIntTQtty -= lIntQtty;
                    lIntQtty -= lIntQtty;

                    lObjBatch.Quantity = lIntTQtty == 0 ? item.Quantity : item.Quantity - lIntTQtty;
                    lObjBatch.Weight = lIntTQtty == 0 ? item.Weight : (item.Quantity * item.Weight) / (item.Quantity + lIntTQtty);
                    lObjBatch.CreationDate = item.CreationDate;

                    lLstBatches.Add(lObjBatch);
                }
                else if (lIntTQtty > 0 && lIntTQtty < lIntQtty && lIntQtty != 0)
                {
                    lIntQtty -= lIntTQtty;
                    lIntTQtty -= lIntTQtty;

                    lObjBatch.Quantity = lIntTQtty == 0 ? item.Quantity : item.Quantity - lIntTQtty;
                    lObjBatch.Weight = lIntTQtty == 0 ? item.Weight : (lIntTQtty * item.Weight) / item.Quantity;
                    lObjBatch.CreationDate = item.CreationDate;

                    lLstBatches.Add(lObjBatch);
                }

            }


            return lLstBatches;
        }

        private float CalculateWeight(float pFlTotWeight, int pIntTotQtty, int pIntHeads)
        {
            return (pIntHeads * pFlTotWeight) / pIntTotQtty;
        }

        private List<SAPBatchDTO> SearchSAPBatch(List<BatchDTO> lLstMissingBatches, IList<Stock> pLstLocalAuctStock)
        {

            List<SAPBatchDTO> lLstSAPBatch = new List<SAPBatchDTO>();

            bool lBoolCreateBatch = false;


            foreach (var lVarStock in pLstLocalAuctStock)
            {
                foreach (var lVarMissingBatch in lLstMissingBatches.Where(x => x.SellerId == lVarStock.CustomerId))
                {
                    SAPBatchDTO lObjSAPBatch = new SAPBatchDTO();
                    int lIntHeads = lVarMissingBatch.Quantity;
                    int lIntMinLevel = mObjInventoryFactory.GetItemService().GetList().Min(x => x.Level);
                    int lIntMaxLevel = mObjInventoryFactory.GetItemService().GetList().Max(x => x.Level);
                    List<Batch> lLstBatches = lVarMissingBatch.BatchesList;

                    while ((lIntMinLevel != lIntMaxLevel + 1) && lVarStock.Quantity > 0 && lVarMissingBatch.Quantity > 0)
                    {
                        lVarMissingBatch.ArticleId = lIntMinLevel;

                        if (lVarStock.ItemId == lVarMissingBatch.ArticleId)
                        {
                            if ((lVarStock.Quantity >= lIntHeads) && lVarStock.Quantity != 0)
                            {
                                lObjSAPBatch.Quantity = lIntHeads;
                                lIntHeads = lVarStock.Quantity - lIntHeads;
                                lVarStock.Quantity = lVarStock.Quantity - lObjSAPBatch.Quantity;
                                lObjSAPBatch.Weight = lVarMissingBatch.Weight;
                                lObjSAPBatch.Seller = lVarMissingBatch.Seller;
                                lVarMissingBatch.Quantity = lVarMissingBatch.Quantity - lObjSAPBatch.Quantity;
                                //lObjSAPBatch.BatchesList.Concat(lVarMissingBatch.BatchesList);
                                lBoolCreateBatch = true;
                            }
                            else if ((lVarStock.Quantity < lIntHeads) && lVarStock.Quantity != 0)
                            {
                                lObjSAPBatch.Quantity = lVarStock.Quantity;
                                lIntHeads = lIntHeads - lVarStock.Quantity;
                                lVarStock.Quantity -= lVarStock.Quantity;
                                lObjSAPBatch.Weight = lVarMissingBatch.Weight;
                                lObjSAPBatch.Seller = lVarMissingBatch.Seller;
                                lVarMissingBatch.Quantity = lIntHeads;
                                //lObjSAPBatch.BatchesList.Concat(lVarMissingBatch.BatchesList);
                                lBoolCreateBatch = true;
                            }
                        }

                        if (lBoolCreateBatch)
                        {
                            lObjSAPBatch.BatchesList = lLstBatches;
                            lObjSAPBatch.BatchNumber = lVarStock.BatchNumber;
                            lObjSAPBatch.SapArticle = lVarStock.Item.Code;
                            lLstSAPBatch.Add(lObjSAPBatch);
                            lBoolCreateBatch = false;
                        }

                        lIntMinLevel++;
                    }
                }
            }

            return lLstSAPBatch;
        }

        private void ProcessRemainingStockGoodsIssues(Auction pObjAuction, IList<GoodsIssue> pLstObjGoodsIssuesList)
        {
            SAPbobsCOM.Documents lObjDocument = null;

            try
            {
                if (pLstObjGoodsIssuesList != null && pLstObjGoodsIssuesList.Count > 0)
                {
                    //Populate header
                    lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
                    lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    lObjDocument.Series = GetGoodsIssueSeries();
                    lObjDocument.DocDate = DateTime.Now;
                    lObjDocument.DocDueDate = DateTime.Now;
                    lObjDocument.Comments = string.Format("Salidas de mercancías rechazadas en la subasta '{0}'", pObjAuction.Folio);
                    lObjDocument.JournalMemo = "Salidas de mercancías";

                    //Add line for each goods issue
                    foreach (GoodsIssue lObjGoodsIssue in pLstObjGoodsIssuesList)
                    {
                        //Pupulate line
                        //lObjDocument.Lines.ItemCode = lObjGoodsIssue.Item.Code;
                        lObjDocument.Lines.WarehouseCode = GetAuctionsWarehouse();
                        lObjDocument.Lines.Quantity = lObjGoodsIssue.Quantity;

                        //Pupulate batch number
                        lObjDocument.Lines.BatchNumbers.Quantity = lObjGoodsIssue.Quantity;
                        //lObjDocument.Lines.BatchNumbers.BatchNumber = lObjGoodsIssue.BatchNumber;
                        lObjDocument.Lines.BatchNumbers.Add();

                        //Add line
                        lObjDocument.Lines.Add();
                    }

                    //Handle operation
                    HandleSapBoOperation(lObjDocument.Add());
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al exportar las salidas de ganado rechazado.", pObjAuction.Folio), lObjException));
                if (!DIApplication.Company.InTransaction)
                {
                    DIApplication.Company.StartTransaction();
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocument);
            }
        }

        private void ProcessRemainingStockGoodsReceipts(Auction pObjAuction, IList<GoodsReceipt> pLstObjGoodsReceiptsList)
        {
            SAPbobsCOM.Documents lObjDocument = null;

            try
            {
                if (pLstObjGoodsReceiptsList != null && pLstObjGoodsReceiptsList.Count > 0)
                {
                    //Populate header
                    lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                    lObjDocument.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    lObjDocument.Series = 288;//GetGoodsReceiptSeries();
                    lObjDocument.DocDate = DateTime.Now;
                    lObjDocument.DocDueDate = DateTime.Now;
                    lObjDocument.Comments = string.Format("Entradas de mercancías rechazadas en la subasta '{0}'", pObjAuction.Folio);
                    lObjDocument.JournalMemo = "Entradas de mercancías";

                    //Add line for each goods goods receipt
                    foreach (GoodsReceipt lObjGoodsReceipt in pLstObjGoodsReceiptsList)
                    {
                        //Pupulate line
                        lObjDocument.Lines.ItemCode = lObjGoodsReceipt.Item.Code;
                        lObjDocument.Lines.WarehouseCode = GetRejectionWarehouse();
                        lObjDocument.Lines.Quantity = lObjGoodsReceipt.Quantity;

                        //Pupulate batch number
                        lObjDocument.Lines.BatchNumbers.Quantity = lObjGoodsReceipt.Quantity;
                        lObjDocument.Lines.BatchNumbers.BatchNumber = string.Format("{0}{1}", lObjGoodsReceipt.Customer.ForeignName, DateTime.Now.ToString("yyMMddHHmmss"));
                        lObjDocument.Lines.BatchNumbers.ExpiryDate = DateTime.Now;
                        lObjDocument.Lines.BatchNumbers.AddmisionDate = DateTime.Now;
                        lObjDocument.Lines.BatchNumbers.ManufacturerSerialNumber = lObjGoodsReceipt.Customer.Code;
                        lObjDocument.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = DateTime.Now.ToString("Hm");
                        lObjDocument.Lines.BatchNumbers.Add();

                        //Add line
                        lObjDocument.Lines.Add();
                    }

                    //Handle operation
                    HandleSapBoOperation(lObjDocument.Add());
                }
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al exportar las entradas de ganado rechazado.", pObjAuction.Folio), lObjException));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocument);
            }
        }

        #endregion

        #region Validations

        private bool GoodsReceiptHasBeenExported(string pStrFolio)
        {
            return QueryManager.Count("U_GLO_Ticket", pStrFolio, "OIGN") > 0;
        }

        #endregion

        #region Conciliate

        private void ConciliateBusinessPartner(Partner pObjCurrentPartner, Partner pObjNewPartner)
        {
            try
            {

                //Change current partner with new one
                pObjCurrentPartner.Code = pObjNewPartner.Code;
                pObjCurrentPartner.TaxCode = pObjNewPartner.TaxCode;
                pObjCurrentPartner.Name = pObjNewPartner.Name;
                pObjCurrentPartner.ForeignName = pObjNewPartner.ForeignName;

                //Update new partner
                pObjNewPartner.Removed = true;
                pObjNewPartner.Temporary = false;
                pObjNewPartner.SyncDate = DateTime.Now;
                pObjNewPartner.ModificationDate = DateTime.Now;
                PartnerDAO.SaveOrUpdateEntity(pObjNewPartner);

                //Update current partner
                pObjCurrentPartner.Temporary = false;
                pObjCurrentPartner.SyncDate = DateTime.Now;
                PartnerDAO.SaveOrUpdateEntity(pObjCurrentPartner);
                ;
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al conciliar al socio de negocio '{0}' con '{1}'.", pObjCurrentPartner.Name, pObjNewPartner.Name), lObjException));
            }
        }

        #endregion

        #region Exports

        private void ExportBusinessPartner(Partner pObjPartner)
        {
            SAPbobsCOM.BusinessPartners lObjCustomer = null;

            try
            {
                //Save on SAP B1 database
                lObjCustomer = (SAPbobsCOM.BusinessPartners)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                lObjCustomer.Series = GetBusinessPartnerSeries();
                lObjCustomer.CardName = pObjPartner.Name;
                lObjCustomer.CardForeignName = pObjPartner.ForeignName;
                lObjCustomer.FederalTaxID = pObjPartner.TaxCode;

                //Handle exception
                HandleSapBoOperation(lObjCustomer.Add());

                //Save on Local database
                pObjPartner.Temporary = false;
                pObjPartner.SyncDate = DateTime.Now;
                PartnerDAO.SaveOrUpdateEntity(pObjPartner);
            }
            catch (Exception lObjException)
            {
                ExceptionList.Add(new DAOException(string.Format("Error al exportar al socio de negocio '{0}'."), lObjException));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjCustomer);
            }
        }

        #endregion

        #region Get entities and series

        private Partner GetLocalPartner(long pLonPartnerId)
        {
            return PartnerDAO.GetEntitiesList().FirstOrDefault(x => x.Id == pLonPartnerId);
        }

        private string GetAuctionsWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("AuctionsWarehouse");
        }

        private string GetRejectionWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("RejectionWarehouse");
        }

        private int GetBusinessPartnerSeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oBusinessPartners).ToString(), ConfigurationUtility.GetValue<string>("BusinessPartnerSeriesName"));
        }

        private int GetInvoiceSeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oInvoices).ToString(), ConfigurationUtility.GetValue<string>("AuctionsSeriesName"));
        }

        private int GetPaymentSeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oIncomingPayments).ToString(), ConfigurationUtility.GetValue<string>("AuctionsSeriesName"));
        }

        private int GetJournalEntrySeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oJournalEntries).ToString(), ConfigurationUtility.GetValue<string>("AuctionsSeriesName"));
        }

        private int GetGoodsReceiptSeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oInventoryGenEntry).ToString(), ConfigurationUtility.GetValue<string>("AuctionsSeriesName"));
        }

        private int GetGoodsIssueSeries()
        {
            return QueryManager.GetSeriesByName(((int)SAPbobsCOM.BoObjectTypes.oInventoryGenExit).ToString(), ConfigurationUtility.GetValue<string>("AuctionsSeriesName"));
        }

        private string GetPaymentMethod(bool pBolIsCreditPayment)
        {
            if (pBolIsCreditPayment)
            {
                return ConfigurationUtility.GetValue<string>("CreditPaymentMethod");
            }
            else
            {
                return ConfigurationUtility.GetValue<string>("CashPaymentMethod");
            }
        }

        private int GetPaymentTerms(bool pBolIsCreditPayment)
        {
            if (pBolIsCreditPayment)
            {
                return ConfigurationUtility.GetValue<int>("CreditPaymentTerms");
            }
            else
            {
                return ConfigurationUtility.GetValue<int>("CashPaymentTerms");
            }
        }

        #endregion

        #region Get entities lists

        private IList<PartnerMapping> GetUnprocessedBusinessPartnerMapping()
        {
            return PartnerMappingDAO
                    .GetEntitiesList()
                    .Where(x => !x.Exported)
                    .ToList();
        }

        private IList<Auction> GetUnprocessedAcutionsList()
        {
            return AuctionDAO
                    .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && !x.Processed)
                    .ToList();
        }

        private IList<Auction> GetProcessedAndReopenedAuctions()
        {
            return AuctionDAO
                    .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && (x.Processed && x.ReOpened)
                        && !x.ReProcessed)
                    .ToList();
        }

        private IList<Batch> GetUnprocessedBatchesList()
        {
            return GetUnprocessedAcutionsList().SelectMany(x => x.Batches).Where(y => !y.Removed && !y.Exported && !y.Auction.ReOpened).ToList();
        }

        private IList<Batch> GetUnprocessedNotSoldBatchesList()
        {
            return GetUnprocessedAcutionsList().SelectMany(x => x.Batches).Where(y => !y.Removed && y.Unsold && !y.Exported && !y.Auction.ReOpened).ToList();
        }

        private IList<Batch> GetUnprocessedAndSoldBatchesList()
        {
            return GetUnprocessedAcutionsList().SelectMany(x => x.Batches).Where(y => !y.Removed && !y.Unsold && !y.Exported && !y.Auction.ReOpened).ToList();
        }

        private IList<Stock> GetAuctionStockList(DateTime pDtAuctionDate)
        {
            return mObjInventoryFactory.GetStockService().GetListByWhs().Where(x => x.Quantity > 0 && (DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(pDtAuctionDate))).OrderBy(x => x.CreationDate).ToList();
        }

        private IList<Invoice> GetUnprocessedInvoiceList(long pLonAuctionId)
        {
            return InvoiceDAO
                    .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && !x.Exported
                        && !x.Processed
                        && x.AuctionId == pLonAuctionId
                        && !x.Auction.ReOpened)
                    .ToList();
        }

        private IList<Invoice> GetProcessedInvoiceList(long pLonAuctionid)
        {
            return InvoiceDAO
                 .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && x.Exported
                        && x.Processed
                        && x.AuctionId == pLonAuctionid
                        && !x.Auction.ReOpened)
                    .ToList();
        }

        private IList<Invoice> GetProcessedUnpayedInvoiceList()
        {

            return InvoiceDAO
                     .GetEntitiesList()
                        .Where(x => !x.Opened
                            && !x.Canceled
                            && x.Exported
                            && x.Processed
                            && x.Payment
                            && !x.Payed
                            && !x.Auction.ReOpened)
                        .ToList();
        }

        private IList<JournalEntry> GetUnprocessedJournalEntriesList(long pLonAuctionId)
        {

            return JournalEntryDAO
                    .GetEntitiesList()
                    .Where(x => !x.Opened
                        && !x.Canceled
                        && !x.Exported
                        && !x.Processed
                        && x.AuctionId == pLonAuctionId)
                    .ToList();
        }

        private IList<GoodsReceipt> GetSellerTemporaryGoodsReceipts(DateTime pDtAuctionDate)
        {

            return GoodsReceiptDAO
                    .GetEntitiesList()
                    .Where(x =>
                        (pDtAuctionDate != DateTime.MinValue ? x.ExpirationDate == pDtAuctionDate : true)
                        && !x.Opened
                        && !x.Canceled
                        && !x.Exported
                        && !x.Processed
                        && x.Quantity > 0)
                    .ToList();
        }

        private IList<Stock> GetStockList(DateTime pDtAuctionDate)
        {
            return StockDAO.GetEntitiesList().Where(x =>
                (pDtAuctionDate != DateTime.MinValue ? x.ExpirationDate == pDtAuctionDate : true)
                ).ToList();
        }

        private IList<GoodsIssue> GetAuctionGoodsIssues(long pLonAuctionId)
        {

            return GetUnprocessedBatchesList().Where(x => x.AuctionId == pLonAuctionId)
                .Select(x => new GoodsIssue()
                {
                    Quantity = x.Quantity
                    - x.GoodsReturns.Where(y => !y.Removed).Select(z => (int?)z.Quantity).Sum() ?? 0,

                    BatchId = x.Id,
                    Batch = x
                }).Where(y => y.Quantity > 0).ToList();


            //return GetUnprocessedAndSoldBatchesList()
            //    .Where(x => x.AuctionId == pLonAuctionId)
            //    .SelectMany(y => y.Lines.Select(z => new { Batch = y, Line = z }))
            //    .Select(x => new GoodsIssue()
            //    {
            //        //(+) Quantity
            //        Quantity = x.Line.Quantity

            //        //(-) Returned
            //        - x.Batch.GoodsReturns.Where(y => !y.Removed /*&& y.ItemId == x.Line.ItemId && y.BatchNumber == x.Line.BatchNumber*/).Select(z => (int?)z.Quantity).Sum() ?? 0,

            //        //BatchNumber = x.Line.BatchNumber,
            //        //BatchDate = x.Line.BatchDate,
            //        //ItemId = x.Line.ItemId,
            //        //Item = x.Line.Item,
            //        BatchId = x.Batch.Id,
            //        Batch = x.Batch,
            //    })
            //    .Where(x => x.Quantity > 0)
            //    .ToList();
        }

        private IList<GoodsReceipt> GetAuctionGoodsReceipts(long pLonAuctionId)
        {
            return GetUnprocessedAndSoldBatchesList()
                .Where(x => x.AuctionId == pLonAuctionId)
                .SelectMany(y => y.Lines.Select(z => new { Batch = y, Line = z }))
                .Select(x => new GoodsReceipt()
                {
                    //(+) Quantity
                    Quantity = x.Line.Quantity -

                    //(-) Delivered
                    (x.Batch.GoodsIssues.Where(y => !y.Removed /*&& y.ItemId == x.Line.ItemId && y.BatchNumber == x.Line.BatchNumber*/).Select(z => (int?)z.Quantity).Sum() ?? 0
                    - x.Batch.GoodsReturns.Where(y => !y.Removed /*&& y.ItemId == x.Line.ItemId && y.BatchNumber == x.Line.BatchNumber*/ && y.Delivered).Select(z => (int?)z.Quantity).Sum() ?? 0)

                    //(-) Returned
                    - x.Batch.GoodsReturns.Where(y => !y.Removed /*&& y.ItemId == x.Line.ItemId && y.BatchNumber == x.Line.BatchNumber*/).Select(z => (int?)z.Quantity).Sum() ?? 0,

                    BatchNumber = x.Line.BatchNumber,
                    BatchDate = x.Line.BatchDate,
                    CustomerId = (long)x.Batch.BuyerId,
                    Customer = x.Batch.Buyer,
                    ItemId = x.Line.ItemId,
                    Item = x.Line.Item,
                })
                .Where(x => x.Quantity > 0)
                .ToList();
        }

        #endregion

        #region Exists unprocessed

        private bool ExistsUnprocessedBusinessPartnerMapping()
        {
            return PartnerMappingDAO.GetEntitiesList().Where(x => !x.Exported).Count() > 0;
        }

        private bool ExistsUnprocessedAcutions()
        {
            return AuctionDAO.GetEntitiesList().Where(x => !x.Opened && !x.Canceled && !x.Processed).Count() > 0;
        }

        private bool ExistsUnprocessedInvoices()
        {
            return InvoiceDAO.GetEntitiesList().Where(x => !x.Opened && !x.Canceled && !x.Exported && !x.Processed).Count() > 0;
        }

        private bool ExistsUnprocessedJournalEntries()
        {
            return JournalEntryDAO.GetEntitiesList().Where(x => !x.Opened && !x.Canceled && !x.Exported && !x.Processed).Count() > 0;
        }

        private bool ExistsUnprocessedGoodsReceipts()
        {
            return GoodsReceiptDAO.GetEntitiesList().Where(x => !x.Opened && !x.Canceled && !x.Exported && !x.Processed).Count() > 0;
        }

        private bool ExistsUnprocessedGoodsIssues()
        {
            return GoodsIssueDAO.GetEntitiesList().Where(x => !x.Opened && !x.Canceled && !x.Exported && !x.Processed).Count() > 0;
        }

        #endregion

        #region Exception

        private void HandleSapBoOperation(int pIntResult)
        {
            if (pIntResult != 0)
            {
                LogUtility.WriteError(DIApplication.Company.GetLastErrorDescription());
                HandleException.SapBo(pIntResult);
            }
            LogUtility.Write("Object key: " + DIApplication.Company.GetNewObjectKey().ToString());
        }

        #endregion

        #endregion

        #region Configuration

        private string GetPaymentAccount()
        {
            return QsConfig.GetValue<string>("PaymentAccount");
        }

        private string GetCostCenter()
        {
            return QsConfig.GetValue<string>("CostCenter");
        }

        private string GetWareHouse()
        {
            return QsConfig.GetValue<string>("FoodWarehouse");
        }

        private string GetLocation()
        {
            return QsConfig.GetValue<string>("AuctionsWarehouse");
        }
        #endregion



    }
}
