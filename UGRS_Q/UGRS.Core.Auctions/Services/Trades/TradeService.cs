using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.Services.Trades
{


    public class TradeService
    {
        #region Attributes
        private IBaseDAO<Trade> mObjTradeDAO;

        #endregion

        public TradeService(IBaseDAO<Trade> pObjTradeDAO)
        {
            mObjTradeDAO = pObjTradeDAO;
        }

        public IQueryable<Trade> GetList(){
            return mObjTradeDAO.GetEntitiesList();
        }

        public void SaveOrUpdateTrade(Trade pObjTrade)
        {
            if (pObjTrade.Id == 0)
            {
                mObjTradeDAO.AddEntity(pObjTrade);
            }
            else
            {
                mObjTradeDAO.UpdateEntity(pObjTrade, pObjTrade.Id);
            }
        }

        public void Remove(long pLonId)
        {
            mObjTradeDAO.RemoveEntity(pLonId);
        }

     

    }
}
