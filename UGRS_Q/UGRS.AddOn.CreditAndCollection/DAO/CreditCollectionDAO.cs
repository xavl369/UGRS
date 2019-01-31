using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.CreditAndCollection.DTO;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;

namespace UGRS.AddOn.CreditAndCollection.DAO
{
    public class CreditCollectionDAO
    {
        QueryManager mObjQueryManager;

        public CreditCollectionDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public string GetMatrixList(string pStrDate, string pStrFolio, string pStrAcc)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("Date", pStrDate);
            lLstStrParameters.Add("Auction", pStrFolio);
            lLstStrParameters.Add("Account", pStrAcc);
            lLstStrParameters.Add("DateNow", GetDateNow());


            string x = this.GetSQL("GetMatrixData").Inject(lLstStrParameters);
            return this.GetSQL("GetMatrixData").Inject(lLstStrParameters);
        }

        public IList<string> GetComboList()
        {
            Recordset lObjRecordSet = null;
            IList<string> lLstObjResult = new List<string>();
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetComboData");
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstObjResult.Add((string)lObjRecordSet.Fields.Item(1).Value);
                        lObjRecordSet.MoveNext();

                    }
                }
            }
            catch (Exception lObjException)
            {
                string er = DIApplication.Company.GetLastErrorDescription();
                LogUtility.WriteError("Error al consultar: " + er); 
                LogUtility.WriteError("Error al consultar: " + lObjException);
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }

        private string GetDateNow()
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);


                string lStrQuery = this.GetSQL("GetServerDate");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError("Error al consultar: " + lObjException);
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }


    }
}
