using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Services.Auctions;
using UGRS.Core.Auctions.Services.Trades;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class AuctionsServicesFactory
    {
        public AuctionService GetAuctionService()
        {
            return new AuctionService(new BaseDAO<Auction>());
        }

        public AuctionStockService GetAuctionStockService()
        {
            return new AuctionStockService
            (
                new BaseDAO<Stock>(),
                new BaseDAO<Auction>(),
                new BaseDAO<Batch>(),
                new BaseDAO<BatchLine>(),
                new BaseDAO<GoodsIssue>(),
                new BaseDAO<GoodsReceipt>(),
                new BaseDAO<GoodsReturn>(),
                new BaseDAO<Item>(),
                new BaseDAO<ItemType>(),
                new BaseDAO<ItemDefinition>(),
                new BaseDAO<ItemTypeDefinition>(),
                new BaseDAO<Change>()
            );
        }

        public BatchService GetBatchService()
        {
            return new BatchService
            (
                new BaseDAO<Batch>(), 
                new BaseDAO<BatchLine>()
            );
        }

        public BatchAuctionService GetBatchAuctionService()
        {
            return new BatchAuctionService
            (
                new BaseDAO<Stock>(),
                new BaseDAO<Auction>(),
                new BaseDAO<Batch>(),
                new BaseDAO<BatchLine>(),
                new BaseDAO<GoodsIssue>(),
                new BaseDAO<GoodsReceipt>(),
                new BaseDAO<GoodsReturn>(),
                new BaseDAO<Item>(),
                new BaseDAO<ItemType>(),
                new BaseDAO<ItemDefinition>(),
                new BaseDAO<ItemTypeDefinition>(),
                new BaseDAO<Change>()
            );
        }

        public BatchLogService GetBatchLogService()
        {
            return new BatchLogService
            (
                new BaseDAO<Batch>(),
                new BaseDAO<Change>(),
                new BaseDAO<User>()
            );
        }

        public BatchLineService GetBatchLineService()
        {
            return new BatchLineService(new BaseDAO<BatchLine>());
        }

        public TradeService GetTradeService()
        {
            return new TradeService(new BaseDAO<Trade>());
        }
    }
}
