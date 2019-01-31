using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.Permissions.Tables;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.Exceptions;
using UGRS.Core.Utility;
using UGRS.AddOn.Permissions.DTO;

namespace UGRS.AddOn.Permissions.DAO
{
    class EarringRanksDAO
    {
        QueryManager mObjQueryManager;

        public EarringRanksDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        //public IList<EarringRanksT> GetLines(string pStrBaseEntry)
        //{
        //    Recordset lObjRecordSet = null;
        //    IList<EarringRanksT> lLstObjResult = new List<EarringRanksT>();

        //    try
        //    {

        //        lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);




        //        string lStrQuery = this.GetSQL("GetLines").InjectSingleValue("BaseEntry",pStrBaseEntry);
        //        lObjRecordSet.DoQuery(lStrQuery);

        //        if (lObjRecordSet.RecordCount > 0)
        //        {

        //            for (int i = 0; i < lObjRecordSet.RecordCount; i++)
        //            {
        //                lLstObjResult.Add(new EarringRanksT()
        //                {
        //                    BaseEntry = lObjRecordSet.Fields.Item("U_BaseEntry").Value.ToString(),
        //                    EarringFrom = lObjRecordSet.Fields.Item("U_EarringFrom").Value.ToString(),
        //                    EarringTo = lObjRecordSet.Fields.Item("U_EarringTo").Value.ToString(),
        //                    RowCode = lObjRecordSet.Fields.Item("Code").Value.ToString()

        //                });
        //                lObjRecordSet.MoveNext();
        //            }




        //        }
        //    }
        //    catch (Exception lObjException)
        //    {

        //        throw new DAOException(lObjException.Message, lObjException);
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjRecordSet);
        //    }
        //    return lLstObjResult;
        //}

        public string GetLinesQuery(string pStrBaseEntry)
        {
            return this.GetSQL("GetLines").InjectSingleValue("BaseEntry", pStrBaseEntry);
        }
        public bool CheckBaseEntry(string pStrBaseEntry)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("CheckBaseEntry").InjectSingleValue("BaseEntry", pStrBaseEntry);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public bool CheckStoredRanks(string pStrEarringFrom, string pStrEarringTo)
        {
            Recordset lObjRecordSet = null;
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("EarringFrom", pStrEarringFrom);
                lLstStrParameters.Add("EarringTo", pStrEarringTo);

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("CheckStoredRanks").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public int GetDocEntry(string pStrDocNum)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetDocEntry").InjectSingleValue("DocNum", pStrDocNum);

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

        public int GetTotalCertHeads(int pIntDocEntry)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetTotalCertHeads").InjectSingleValue("DocEntry", pIntDocEntry);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value);
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

    }
}
