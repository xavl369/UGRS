using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.CreditAndCollection.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.CreditAndCollection.Services
{
   public class CreditAndCollectionService
    {
         TableDAO<CreditAndCollectionTable> mObjCreditCollectionDAO;

         public CreditAndCollectionService()
        {
            mObjCreditCollectionDAO = new TableDAO<CreditAndCollectionTable>();
        }

          public int SaveAutorization(CreditAndCollectionTable pObjCreditCollectionDAO)
        {
            int result = mObjCreditCollectionDAO.Add(pObjCreditCollectionDAO);
            return result;
        }

    }
}
