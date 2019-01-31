using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Services.Reports;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class ReportsServiceFactory
    {
        public AuctionsReportsService GetAuctionsReportsService()
        {
            return new AuctionsReportsService(new BaseDAO<Auction>(), new BaseDAO<Batch>());
        }

        public BusinessReportService GetBusinessReportService()
        {
            return new BusinessReportService(new BaseDAO<Partner>());
        }

        public InventoryReportService GetInventoryReportService()
        {
            return new InventoryReportService(new BaseDAO<GoodsReceipt>());
        }
    }
}
