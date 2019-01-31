using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using System.Text.RegularExpressions;
using UGRS.AddOn.AccountingAccounts.DTO;

namespace UGRS.AddOn.AccountingAccounts.DAO
{
    public class EmployeesDAO
    {
        public List<int> GetEmployees(string pStrYear, string pStrPeriodo, string pStrNo, string pStrServer, string pStrDBName, string pStrUser, string pStrPassword)
        {
            Recordset lObjRecordset = null;
            List<int> lLstEmployeesId = null;

            try
            {
                lLstEmployeesId = new List<int>();

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ServerName", pStrServer);
                lLstStrParameters.Add("DBName", pStrDBName);
                lLstStrParameters.Add("User", pStrUser);
                lLstStrParameters.Add("Password", pStrPassword);
                lLstStrParameters.Add("Year", pStrYear);
                lLstStrParameters.Add("Tipo", pStrPeriodo);
                lLstStrParameters.Add("Ipno", pStrNo);
                string lStrQuery = this.GetSQL("GetEmployees").Inject(lLstStrParameters);
                //this.UIAPIRawForm.DataSources.DataTables.Item("RESULT").ExecuteQuery(lStrQuery);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        string lStrEmpId = lObjRecordset.Fields.Item("ATRAB").Value.ToString();

                        lLstEmployeesId.Add(int.Parse(lStrEmpId));

                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstEmployeesId;
        }

        public List<EmployeesDTO> GetEmployeeID()
        {
            Recordset lObjRecordset = null;
            List<EmployeesDTO> lLstEmployeesId = null;


            try
            {
                lLstEmployeesId = new List<EmployeesDTO>();

                string lStrQuery = this.GetSQL("GetIdEmployees");

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstEmployeesId.Add(new EmployeesDTO()
                        {
                            IdEmpNomina = string.IsNullOrEmpty(lObjRecordset.Fields.Item("empID").Value.ToString()) ? (int?)null : int.Parse(lObjRecordset.Fields.Item("empID").Value.ToString()), //(int?)lObjRecordset.Fields.Item("empID").Value, //SAP
                            IdEmpSAP = string.IsNullOrEmpty(lObjRecordset.Fields.Item("ExtEmpNo").Value.ToString()) ? (int?)null : int.Parse(lObjRecordset.Fields.Item("ExtEmpNo").Value.ToString()),
                            FullName = lObjRecordset.Fields.Item("FullName").Value.ToString()
                        });

                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lLstEmployeesId;
        }
    }
}
