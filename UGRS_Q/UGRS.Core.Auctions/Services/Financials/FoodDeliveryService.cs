using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class FoodDeliveryService
    {
        private IBaseDAO<FoodDelivery> mObjFoodDeliveryDAO;

        public FoodDeliveryService(IBaseDAO<FoodDelivery> pObjFoodDeliveryDAO)
        {
            mObjFoodDeliveryDAO = pObjFoodDeliveryDAO;
        }

        public IQueryable<FoodDelivery> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<FoodDelivery> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(FoodDelivery pObjFoodDelivery)
        {
            if (!Exists(pObjFoodDelivery))
            {
                mObjFoodDeliveryDAO.SaveOrUpdateEntity(pObjFoodDelivery);
            }
            else
            {
                throw new Exception("La entrega de mercancía ya se encuentra registrada.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjFoodDeliveryDAO.RemoveEntity(pLonId);
        }

        private IQueryable<FoodDelivery> GetSortedList()
        {
            return mObjFoodDeliveryDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }

        private bool Exists(FoodDelivery pObjFoodDelivery)
        {
            return mObjFoodDeliveryDAO
                    .GetEntitiesList()
                    .Where(x => x.DocNum == pObjFoodDelivery.DocNum
                        && x.DocEntry == pObjFoodDelivery.DocEntry
                        && x.LineNum == pObjFoodDelivery.LineNum 
                        && x.Id != pObjFoodDelivery.Id)
                    .Count() > 0 ? true : false;
        }
    }

}
