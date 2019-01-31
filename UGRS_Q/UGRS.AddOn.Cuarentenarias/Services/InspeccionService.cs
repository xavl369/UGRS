using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Cuarentenarias.Services
{
    public class InspeccionService
    {
        TableDAO<InspeccionT> mObjInspDAO;

        public InspeccionService()
        {
            mObjInspDAO = new TableDAO<InspeccionT>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObjInsp"></param>
        /// <returns></returns>
        public int SaveInspeccion(InspeccionT pObjInsp)
        {
            int result = mObjInspDAO.Add(pObjInsp);
            return result;
        }

        public int UpdateInspeccion(InspeccionT pObjInsp)
        {
            return mObjInspDAO.Update(pObjInsp);
        }
    }
}
