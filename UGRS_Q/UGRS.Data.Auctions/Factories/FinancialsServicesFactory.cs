using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Services.Financials;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class FinancialsServicesFactory
    {
        public FoodDeliveryService GetDeliveryFoodService()
        {
            return new FoodDeliveryService(new BaseDAO<FoodDelivery>());
        }

        public UGRS.Core.Auctions.Services.Financials.FoodChargeService GetFoodChargeService()
        {
            return new UGRS.Core.Auctions.Services.Financials.FoodChargeService(new BaseDAO<FoodCharge>(), new BaseDAO<FoodChargeLine>());
        }

        public FoodChargeCheckService GetFoodChargeCheckService()
        {
            return new FoodChargeCheckService(
                new BaseDAO<FoodChargeCheck>(),
                new BaseDAO<Stock>(),
                new BaseDAO<GoodsReceipt>(),
                new BaseDAO<FoodDelivery>(),
                new BaseDAO<Configuration>(),
                new BaseDAO<Auction>(),
                new BaseDAO<Batch>(),
                new BaseDAO<Partner>());
        }

        public GuideChargeService GetGuideChargeService()
        {
            return new GuideChargeService(
                new BaseDAO<GuideCharge>(), 
                new BaseDAO<Auction>(), 
                new BaseDAO<Batch>());
        }

        public InvoiceService GetInvoiceService()
        {
            return new InvoiceService(new BaseDAO<Invoice>(), new BaseDAO<InvoiceLine>());
        }

        public InvoiceLineService GetInvoiceLineService()
        {
            return new InvoiceLineService(new BaseDAO<InvoiceLine>());
        }

        public JournalEntryService GetJournalEntryService()
        {
            return new JournalEntryService(new BaseDAO<JournalEntry>(), new BaseDAO<JournalEntryLine>());
        }

        public JournalEntryLineService GetJournalEntryLineService()
        {
            return new JournalEntryLineService(new BaseDAO<JournalEntryLine>());
        }

        public DeductionCheckService GetDeductionCheckService()
        {
            return new DeductionCheckService(new BaseDAO<Auction>(), new BaseDAO<DeductionCheck>(), new BaseDAO<Trade>());
        }
    }
}
