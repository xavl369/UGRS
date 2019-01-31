using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.DAO;
using UGRS.Core.SDK.DI.Auctions.DTO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class DeliveryFoodService
    {
        private DeliveryFoodDAO mObjDeliveryFoodDAO;

        public DeliveryFoodService()
        {
            mObjDeliveryFoodDAO = new DeliveryFoodDAO();
        }

        public IList<int> GetDeliveriesFoodList(string pStrWhsCode)
        {
            return mObjDeliveryFoodDAO.GetDeliveriesFoodList(pStrWhsCode);
        }

        public IList<DeliveryFoodDTO> GetUpdatedDeliveriesFoodList(string pStrWhsCode)
        {
            return mObjDeliveryFoodDAO.GetUpdatedDeliveriesFoodList(pStrWhsCode);
        }

        public IList<DeliveryFoodDTO> GetDeliveriesFood(int pIntDocEntry)
        {
            return mObjDeliveryFoodDAO.GetDeliveriesFood(pIntDocEntry);
        }
    }
}
