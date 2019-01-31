using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class FoodChargeLineService
    {
        private IBaseDAO<FoodChargeLine> mObjFoodChargeLineDAO;

        public FoodChargeLineService(IBaseDAO<FoodChargeLine> pObjFoodChargeLineDAO)
        {
            mObjFoodChargeLineDAO = pObjFoodChargeLineDAO;
        }

        public IQueryable<FoodChargeLine> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<FoodChargeLine> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(FoodChargeLine pObjFoodChargeLine)
        {
            mObjFoodChargeLineDAO.SaveOrUpdateEntity(pObjFoodChargeLine);
        }

        public void Remove(long pLonId)
        {
            mObjFoodChargeLineDAO.RemoveEntity(pLonId);
        }

        private IQueryable<FoodChargeLine> GetSortedList()
        {
            return mObjFoodChargeLineDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }
    }
}
