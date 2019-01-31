using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Services.Inventory;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class InventoryServicesFactory
    {
        public GoodsIssueService GetGoodsIssueService()
        {
            return new GoodsIssueService(new BaseDAO<GoodsIssue>(), new BaseDAO<Auction>(), new BaseDAO<Batch>());
        }

        public GoodsReceiptService GetGoodsReceiptService()
        {
            return new GoodsReceiptService(new BaseDAO<GoodsReceipt>());
        }

        public GoodsReturnService GetGoodsReturnService()
        {
            return new GoodsReturnService(new BaseDAO<GoodsReturn>(), new BaseDAO<Batch>());
        }

        public ItemService GetItemService()
        {
            return new ItemService(new BaseDAO<Item>());
        }

        public ItemTypeService GetItemTypeService()
        {
            return new ItemTypeService(new BaseDAO<ItemType>(), new BaseDAO<ItemTypeDefinition>());
        }

        public StockService GetStockService()
        {
            return new StockService(new BaseDAO<Stock>());
        }

        public ItemDefinitionService GetItemDefinitionService()
        {
            return new ItemDefinitionService(new BaseDAO<ItemDefinition>(), new BaseDAO<Item>(), new BaseDAO<ItemType>());
        }

        public ItemTypeDefinitionService GetItemTypeDefinitionService()
        {
            return new ItemTypeDefinitionService(new BaseDAO<ItemTypeDefinition>(), new BaseDAO<ItemType>());
        }
    }
}
