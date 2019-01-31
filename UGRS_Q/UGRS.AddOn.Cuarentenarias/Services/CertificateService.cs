using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Cuarentenarias.Services
{
    public class CertificateService
    {
        TableDAO<Certificate> mObjInspDAO;
        TableDAO<SICertificates> mObjSICertDAO;

        public CertificateService()
        {
            mObjInspDAO = new TableDAO<Certificate>();
            mObjSICertDAO = new TableDAO<SICertificates>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObjInsp"></param>
        /// <returns></returns>
        public int SaveCertificate(Certificate pObjCert)
        {
            int result = mObjInspDAO.Add(pObjCert);
            return result;
        }

        public int UpdateCertificate(Certificate pObjCert)
        {
            return mObjInspDAO.Update(pObjCert);
        }

        public int UpdateSICert(SICertificates pObjSICert)
        {
            return mObjSICertDAO.Update(pObjSICert);
        }


        public int DeleteInspectionDetails(string pStrCode)
        {
            int lIntResult = mObjInspDAO.Remove(pStrCode);
            return lIntResult;
        }
    }
}
