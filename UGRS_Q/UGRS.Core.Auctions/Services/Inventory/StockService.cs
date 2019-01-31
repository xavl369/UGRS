using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Utility;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class StockService
    {
        private IBaseDAO<Stock> mObjStockDAO;

        public StockService(IBaseDAO<Stock> pObjStockDAO)
        {
            mObjStockDAO = pObjStockDAO;
        }

        public IQueryable<Stock> GetListByWhs()
        {
            string lStrCurrentWhs = GetCurrentWhs();
            return mObjStockDAO.GetEntitiesList().Where(x=>x.CurrentWarehouse == lStrCurrentWhs);
        }

        public void SaveOrUpdate(Stock pObjItemType)
        {
            mObjStockDAO.SaveOrUpdateEntity(pObjItemType);
        }

        public void Remove(long pLonId)
        {
            mObjStockDAO.RemoveEntity(pLonId);
        }

        private string GetCurrentWhs()
        {
            return ConfigurationUtility.GetValue<string>("AuctionsWarehouse").ToString();
        }

    }
}
