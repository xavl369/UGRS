using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;

namespace UGRS.AddOn.Permissions.DAO
{
    public class PrefixesDAO
    {

        QueryManager mObjQueryManager;

        public PrefixesDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public int GetPrevCode()
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetPrevPrefix");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (int)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetPrevPrefix()
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetPrevPrefix");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(1).Value;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }
    }

}
