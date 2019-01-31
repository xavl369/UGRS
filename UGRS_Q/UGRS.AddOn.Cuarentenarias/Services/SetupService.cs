using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.Tables;

namespace UGRS.AddOn.Cuarentenarias.Services
{
   public class SetupService
    {
       private UGRS.Core.SDK.DI.DAO.TableDAO<Certificate> mObjCertificateDAO;

       public SetupService()
       {
           mObjCertificateDAO = new Core.SDK.DI.DAO.TableDAO<Certificate>();
       }

       public void InitializeTables()
       {
           mObjCertificateDAO.Initialize();
       }
    }
}
