using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Cuarentenarias.Services
{
    public class InspectionDetailsService
    {
        TableDAO<InspectionDetails> mObjInspDetailsDAO;

        public InspectionDetailsService()
        {
            mObjInspDetailsDAO = new TableDAO<InspectionDetails>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObjInsp"></param>
        /// <returns></returns>
        public int SaveInspectionDetails(InspectionDetails pObjInspDetails)
        {
            int result = mObjInspDetailsDAO.Add(pObjInspDetails);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStrCode"></param>
        /// <returns></returns>
        public int DeleteInspectionDetails(string pStrCode)
        {
            int lIntResult = mObjInspDetailsDAO.Remove(pStrCode);
            return lIntResult;
        }

    }
}
