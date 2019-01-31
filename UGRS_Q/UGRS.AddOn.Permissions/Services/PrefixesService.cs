using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Permissions.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Permissions.Services
{
    class PrefixesService
    {
        TableDAO<PrefixesT> mObjPrefixDAO;

        public PrefixesService()
        {
            mObjPrefixDAO = new TableDAO<PrefixesT>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObjPrefix"></param>
        /// <returns></returns>
        public int SaveActivePrefix(PrefixesT pObjPrefix)
        {
            int result = mObjPrefixDAO.Add(pObjPrefix);
            return result;
        }

        public int UpdatePrevPrefix(PrefixesT pObjPrefix)
        {
            return mObjPrefixDAO.Update(pObjPrefix);
        }
    }
}
