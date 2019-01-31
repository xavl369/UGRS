using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class FoodChargeService
    {
        private IBaseDAO<FoodCharge> mObjFoodChargeDAO;

        public FoodChargeService(IBaseDAO<FoodCharge> pObjFooCharge)
        {
            mObjFoodChargeDAO = pObjFooCharge;
        }

        public FoodCharge Get(long pLonId)
        {
            return mObjFoodChargeDAO.GetEntity(pLonId);
        }

        public FoodCharge Get(int pIntNumber)
        {

            return mObjFoodChargeDAO.GetEntitiesList().Where(x => x.Folio == pIntNumber).FirstOrDefault();
        }

        public IQueryable<FoodCharge> GetList()
        {
            return mObjFoodChargeDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(FoodCharge pObjFoodCharge)
        {

            mObjFoodChargeDAO.SaveOrUpdateEntity(pObjFoodCharge);
            //if(!Exist)
        }

        public void Remove(long pLonID)
        {
            mObjFoodChargeDAO.RemoveEntity(pLonID);
        }

        /// <summary>
        ///Valida si existe una cobro con el folio ingresado
        /// </summary>
        public bool IsValid(int pIntFolio)
        {
            return mObjFoodChargeDAO.GetEntitiesList().Where(x => x.Folio == pIntFolio).Count() > 0;
        }

        public IList<FoodCharge> SearchAuctions(int pIntFood)
        {
            return this.GetList().Where(x => x.Active == true && x.Removed == false && x.Folio == pIntFood).ToList();
        }
    }
}
