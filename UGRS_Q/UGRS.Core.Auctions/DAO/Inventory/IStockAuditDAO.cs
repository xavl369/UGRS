using System.Collections.Generic;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.DAO.Inventory
{
    public interface IStockAuditDAO
    {
        IList<BatchLine> GetBatchLines(long pLonCustomerId, long pLonAuctionId, long pLonItemType, int pIntQuantity);

        bool ExistItemTypeMapping(long pLonItemType);

        int GetAllStockByCustomer(long pLonCustomerId, long pLonAuctionId);

        int GetAvailableStockByCustomer(long pLonCustomerId, long pLonAuctionId);

        int GetSoldStockByCustomer(long pLonCustomerId, long pLonAuctionId);

        int GetPurchasedStockByCustomer(long pLonCustomerId, long pLonAuctionId);

        int GetAllSoldsByAuction(long pLonAuctionId);

        IList<StockAuditDTO> GetAllStockItemsByCustomer(long pLonCustomerId, long pLonAuctionId);

        IList<StockAuditDTO> GetAvailableStockItemsByCustomer(long pLonCustomerId, long pLonAuctionId);

        IList<StockAuditDTO> GetSoldStockItemsByCustomer(long pLonCustomerId, long pLonAuctionId);

        IList<StockAuditDTO> GetPurchasedStockItemsByCustomer(long pLonCustomerId, long pLonAuctionId);

        bool ValidAvailabilityStock(long pLonCustomerId, long pLonAuctionId, int pIntQuantity);

        IList<StockBatchDTO> GetBatchStocksList();
    }
}
