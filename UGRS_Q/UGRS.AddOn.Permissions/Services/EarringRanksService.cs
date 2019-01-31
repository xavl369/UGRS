using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Permissions.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Permissions.Services
{
    class EarringRanksService
    {
       TableDAO<EarringRanksT> mObjEarringRanksDAO;

       public EarringRanksService()
        {
            mObjEarringRanksDAO = new TableDAO<EarringRanksT>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObjInsp"></param>
        /// <returns></returns>
       public int SaveRanks(EarringRanksT pObjCert)
        {
            int result = mObjEarringRanksDAO.Add(pObjCert);
            return result;
        }

       public int UpdateRanks(EarringRanksT pObjCert)
       {
           return mObjEarringRanksDAO.Update(pObjCert);
       }


       public int DeleteRanks(string pStrCode)
       {
           int lIntResult = mObjEarringRanksDAO.Remove(pStrCode);
           return lIntResult;
       }
    }
}
