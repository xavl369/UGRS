using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class FoodChargeService
    {
        private IBaseDAO<FoodCharge> mObjFoodChargeDAO;
        private IBaseDAO<FoodChargeLine> mObjFoodChargeLineDAO;

        public FoodChargeService(IBaseDAO<FoodCharge> pObjFoodChargeDAO, IBaseDAO<FoodChargeLine> pObjFoodChargeLineDAO)
        {
            mObjFoodChargeDAO = pObjFoodChargeDAO;
            mObjFoodChargeLineDAO = pObjFoodChargeLineDAO;
        }

        public IQueryable<FoodCharge> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<FoodCharge> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(FoodCharge pObjFoodCharge)
        {
            if (!Exists(pObjFoodCharge))
            {
                IList<FoodChargeLine> lLstObjLines = pObjFoodCharge.Lines;
                pObjFoodCharge.Lines = null;

                mObjFoodChargeDAO.SaveOrUpdateEntity(pObjFoodCharge);

                if (lLstObjLines != null && lLstObjLines.Count > 0)
                {
                    mObjFoodChargeLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.FoodChargeId = pObjFoodCharge.Id; return x; }).ToList());
                }
            }
            else
            {
                throw new Exception("El cobro de alimento capturado ya se encuentra registrado.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjFoodChargeDAO.RemoveEntity(pLonId);
        }

        private IQueryable<FoodCharge> GetSortedList()
        {
            return mObjFoodChargeDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }

        private bool Exists(FoodCharge pObjFoodCharge)
        {
            return mObjFoodChargeDAO
                    .GetEntitiesList()
                    .Where(x => x.AuctionId == pObjFoodCharge.AuctionId 
                        && x.SellerId == pObjFoodCharge.SellerId 
                        && x.Id != pObjFoodCharge.Id)
                    .Count() > 0 ? true : false;
        }
    }
}
