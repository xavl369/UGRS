
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;

namespace UGRS.AddOn.AccountingAccounts.Utils
{
    public class Tools
    {
        /// <summary>
        /// FormExists
        /// </summary>
        /// <param name="formUid"></param>
        /// <returns></returns>
        public bool FormExists(string formUid)
        {
            bool exist = false;
            try
            {
                string[] lArrCountForms = new string[Application.SBO_Application.Forms.Count];
                //foreach (int[] lArrCountForms in Application.SBO_Application.Forms.Count)
                for (int i = 0; i < lArrCountForms.Count() ; i++)
                {
                    if (formUid == Application.SBO_Application.Forms.Item(i).UniqueID.ToString())
                    {
                        exist = true;                        
                        break;
                    }
                    else
                    {
                        exist = false;
                    }
                }                
            }
            catch (Exception e)
            {
                Application.SBO_Application.StatusBar.SetText(e.Message,SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            return exist;
        }

        /// <summary>
        /// ItemExist
        /// </summary>
        /// <param name="pStrItemName"></param>
        /// <param name="pObjForm"></param>
        /// <returns></returns>
        private bool ItemExist(string pStrItemName, SAPbouiCOM.Form pObjForm)
        {
            try
            {
                pObjForm.Items.Item(pStrItemName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckCurrencyRate(SAPbobsCOM.Company pObjCompany)
        {
            DateTime curDate = DateTime.Today;
            bool foundExchangeRate = false;
            Recordset oRecordSet = null;
            SBObob oSBObob = null;            
            //SAPbobsCOM.Company mObjCompany = null;        
            try
            {
                oSBObob = (SAPbobsCOM.SBObob)pObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);                
                oRecordSet = (Recordset)UGRS.Core.SDK.DI.DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                try
                {
                    oRecordSet = oSBObob.GetCurrencyRate("USD", curDate);
                    var y = oRecordSet.RecordCount;

                    if (y >= 1)
                    {
                        foundExchangeRate = true;
                        //if (logWriter.DetLog == DetalleLog.FULL)
                        //{
                        //    logWriter.AppendLog("(9) Tipo de cambio ya registrado", currentCompany);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    foundExchangeRate = false;  //-532462766: //Actualice el tipo de cambio                    
                }
            }
            catch (Exception ex)
            {
                foundExchangeRate = false;

            }
            MemoryUtility.ReleaseComObject(oSBObob);
            return foundExchangeRate;
        }

        public string CheckAccounts(SqlConnection pObjSqlCon, SqlCommand pObjSqlCmd, SAPbobsCOM.Company pObjCompany, string pStrYear, string pStrPeriodo, string pStrNo)
        {
            Recordset oRecordSet = null;
            SqlDataReader lObjDR = null;
            string lStrMsg_Accounts = "";
            try
            {
                pObjSqlCmd = new SqlCommand("SP_GetDistinctAccounts", pObjSqlCon);
                pObjSqlCmd.CommandType = CommandType.StoredProcedure;
                pObjSqlCmd.Parameters.Add("@IPYEAR", SqlDbType.Int, 4).Value = Convert.ToInt32(pStrYear);
                pObjSqlCmd.Parameters.Add("@IPTIPO", SqlDbType.Int, 2).Value = Convert.ToInt32(pStrPeriodo);
                pObjSqlCmd.Parameters.Add("@IPNO", SqlDbType.Int, 2).Value = Convert.ToInt32(pStrNo);

                lObjDR = pObjSqlCmd.ExecuteReader();
                if (lObjDR.HasRows)
                {
                    while (lObjDR.Read())
                    {
                        oRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                        oRecordSet.DoQuery("Select * from OACT where AcctCode = '" + Regex.Replace(lObjDR["CUENTA"].ToString(), @"[$' ']", "") + "' and FrozenFor='N'");
                        int lIntRecords = oRecordSet.RecordCount;
                        if (lIntRecords < 1)
                        {
                            lStrMsg_Accounts = lStrMsg_Accounts + " " + Regex.Replace(lObjDR["CUENTA"].ToString(), @"[$' ']", "") + " -";
                        }                                                
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);                            
            }
            finally
            {
                MemoryUtility.ReleaseComObject(oRecordSet);
                lObjDR.Close();
            }
            return lStrMsg_Accounts;
        }

        public string CheckCostingCode(SqlConnection pObjSqlCon, SqlCommand pObjSqlCmd, SAPbobsCOM.Company pObjCompany, string pStrYear, string pStrPeriodo, string pStrNo)
        {
            string lStrMsg_CostingCode = "";
            Recordset oRecordSet = null;
            SqlDataReader lObjDR = null;
            try
            {
                pObjSqlCmd = new SqlCommand("SP_GetDistinctCostingCode", pObjSqlCon);
                pObjSqlCmd.CommandType = CommandType.StoredProcedure;
                pObjSqlCmd.Parameters.Add("@IPYEAR", SqlDbType.Int, 4).Value = Convert.ToInt32(pStrYear);
                pObjSqlCmd.Parameters.Add("@IPTIPO", SqlDbType.Int, 2).Value = Convert.ToInt32(pStrPeriodo);
                pObjSqlCmd.Parameters.Add("@IPNO", SqlDbType.Int, 2).Value = Convert.ToInt32(pStrNo);

                lObjDR = pObjSqlCmd.ExecuteReader();
                if (lObjDR.HasRows)
                {
                    while (lObjDR.Read())
                    {
                        oRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                        oRecordSet.DoQuery("Select * from OOCR where OcrCode = '" + lObjDR["CUENTA2"].ToString() + "' and active = 'Y'");
                        int lIntRecords = oRecordSet.RecordCount;
                        if (lIntRecords < 1)
                        {
                            lStrMsg_CostingCode = lStrMsg_CostingCode + " " + Regex.Replace(lObjDR["CUENTA2"].ToString(), @"[$' ']", "") + " -";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lStrMsg_CostingCode = "";
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);                            
            }
            finally
            {
                MemoryUtility.ReleaseComObject(oRecordSet);
                lObjDR.Close();                
            }
           return lStrMsg_CostingCode;
        }

    }
}
