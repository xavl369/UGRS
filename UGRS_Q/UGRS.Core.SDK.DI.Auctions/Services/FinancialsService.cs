using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.DAO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class FinancialsService
    {
        FinancialsDAO mObjFinancialsDAO;

        public FinancialsService()
        {
            mObjFinancialsDAO = new FinancialsDAO();
        }

        public IList<object> GetDeliveriesFood(string pStrWhsCode, string pStrCardCode)
        {
            return mObjFinancialsDAO.GetDeliveriesFood(pStrWhsCode, pStrCardCode);
        }

        public string GetPrice(string pStrWhsCode, string pStrItemCode)
        {
            return mObjFinancialsDAO.GetPrice(pStrWhsCode, pStrItemCode);
        }

        public double GetDocTotal(string pStrCardCode, string pStrNumAtCard)
        {
            return mObjFinancialsDAO.GetDocTotal(pStrCardCode, pStrNumAtCard);
        }
    }
}
