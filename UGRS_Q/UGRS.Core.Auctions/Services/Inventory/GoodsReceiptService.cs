using System.Linq;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.DAO.Base;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class GoodsReceiptService
    {
        private IBaseDAO<GoodsReceipt> mObjGoodsReceiptDAO;

        public GoodsReceiptService(IBaseDAO<GoodsReceipt> pObjGoodsReceiptDAO)
        {
            mObjGoodsReceiptDAO = pObjGoodsReceiptDAO;
        }

        public IQueryable<GoodsReceipt> GetList()
        {
            return mObjGoodsReceiptDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(GoodsReceipt pObjGoodsReceipt)
        {
            mObjGoodsReceiptDAO.SaveOrUpdateEntity(pObjGoodsReceipt);
        }

        public void Remove(long pLonId)
        {
            mObjGoodsReceiptDAO.RemoveEntity(pLonId);
        }
    }
}
