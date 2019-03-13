using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UGRS.AddOn.AccountingAccounts.Entities;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Utility;
using System.Diagnostics;
using UGRS.Core.SDK.UI;
using System.Threading.Tasks;
using System.Data;

namespace UGRS.AddOn.AccountingAccounts.DAO {

    public class AccountsDAO {

        public List<Nomina> GetAccounts(string pStrYear, string pStrPeriodo, string pStrNo, string pStrServer, string pStrDBName, string pStrUser, string pStrPassword){
            Recordset lObjRecordset = null;
           
            //se asigna el valor desde aqui para que mas adelante no ocurra excepcion de referencia no establecida: 15/11/2018
            //List<Nomina> lLstNomina = null;
            List<Nomina> lLstNomina = new List<Nomina>();


            try {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ServerName", pStrServer);
                lLstStrParameters.Add("DBName", pStrDBName);
                lLstStrParameters.Add("User", pStrUser);
                lLstStrParameters.Add("Password", pStrPassword);
                lLstStrParameters.Add("Year", pStrYear);
                lLstStrParameters.Add("Tipo", pStrPeriodo);
                lLstStrParameters.Add("Ipno", pStrNo);
                //lLstStrParameters.Add("IdEmp", pIntIdEmp.ToString());

                //SqlConnection conn = new SqlConnection(conecctionString);
                string lStrQuery = this.GetSQL("GetAccounts").Inject(lLstStrParameters);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if(lObjRecordset.RecordCount > 0) {
                    for(int i = 0; i < lObjRecordset.RecordCount; i++) {
                        Nomina lObjNomina = new Nomina();

                        lObjNomina.ACONS = int.Parse(lObjRecordset.Fields.Item("ACONS").Value.ToString());
                        lObjNomina.IPYEAR = int.Parse(lObjRecordset.Fields.Item("IPYEAR").Value.ToString());
                        lObjNomina.IPMES = int.Parse(lObjRecordset.Fields.Item("IPMES").Value.ToString());
                        lObjNomina.IPTIPO = int.Parse(lObjRecordset.Fields.Item("IPTIPO").Value.ToString());
                        lObjNomina.IPNO = int.Parse(lObjRecordset.Fields.Item("IPNO").Value.ToString());
                        lObjNomina.ATRAB = int.Parse(lObjRecordset.Fields.Item("ATRAB").Value.ToString());
                        lObjNomina.ACONC = int.Parse(lObjRecordset.Fields.Item("ACONC").Value.ToString());
                        lObjNomina.ICNOM = lObjRecordset.Fields.Item("ICNOM").Value.ToString();
                        lObjNomina.AIMPO = lObjRecordset.Fields.Item("AIMPO").Value.ToString();
                        lObjNomina.ACCTO_HN = lObjRecordset.Fields.Item("ACCTO_HN").Value.ToString();
                        lObjNomina.ICDNOM = lObjRecordset.Fields.Item("ICDNOM").Value.ToString();
                        lObjNomina.ADEP_HN = lObjRecordset.Fields.Item("ADEP_HN").Value.ToString();
                        lObjNomina.ICCTAC = lObjRecordset.Fields.Item("ICCTAC").Value.ToString();
                        lObjNomina.ICCTAA = lObjRecordset.Fields.Item("ICCTAA").Value.ToString();
                        lObjNomina.NIVEL = lObjRecordset.Fields.Item("NIVEL").Value.ToString();
                        lObjNomina.CUENTA = lObjRecordset.Fields.Item("CUENTA").Value.ToString();
                        lObjNomina.ACCTO = lObjRecordset.Fields.Item("ACCTO").Value.ToString();
                        lObjNomina.ADEP = lObjRecordset.Fields.Item("ADEP").Value.ToString();
                        lObjNomina.CUENTA1 = lObjRecordset.Fields.Item("CUENTA1").Value.ToString();
                        lObjNomina.CCTO_ID = lObjRecordset.Fields.Item("CCTO_ID").Value.ToString();
                        lObjNomina.UUID = lObjRecordset.Fields.Item("UUID").Value.ToString();
                        lObjNomina.CUENTA2 = lObjRecordset.Fields.Item("CUENTA2").Value.ToString();
                        lObjNomina.NNOM = lObjRecordset.Fields.Item("NNOM").Value.ToString();
                        lObjNomina.NRFC = lObjRecordset.Fields.Item("NRFC").Value.ToString();

                        //toma ligeramente mas tiempo
                        //foreach(Field field in lObjRecordset.Fields) {
                        //    lObjNomina.GetType().GetProperty(field.Name).SetValue(lObjNomina, field.Value);
                        //}

                        lLstNomina.Add(lObjNomina);
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch(Exception lObjException) {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
           
            return lLstNomina;
        }

        public string GetAccts(string pStrYear, string pStrPeriodo, string pStrNo, string pStrServer, string pStrDBName, string pStrUser, string pStrPassword)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("ServerName", pStrServer);
            lLstStrParameters.Add("DBName", pStrDBName);
            lLstStrParameters.Add("User", pStrUser);
            lLstStrParameters.Add("Password", pStrPassword);
            lLstStrParameters.Add("Year", pStrYear);
            lLstStrParameters.Add("Tipo", pStrPeriodo);
            lLstStrParameters.Add("Ipno", pStrNo);

            string lStrQuery = this.GetSQL("GetAccts").Inject(lLstStrParameters);

            return lStrQuery;

        }

        internal string CheckAccounts(IEnumerable<IGrouping<string, Nomina>> lVarGroupNom) {
            Recordset lObjRecordsetCT = null;
            string lStrMsgAccounts = "";

            try {
                lObjRecordsetCT = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                foreach(var lVarGp in lVarGroupNom) {

                    var QUERY = "Select * from OACT where AcctCode = '" + Regex.Replace(lVarGp.Key, @"[$' ']", "") + "' and FrozenFor='N'";
                    lObjRecordsetCT.DoQuery("Select * from OACT where AcctCode = '" + Regex.Replace(lVarGp.Key, @"[$' ']", "") + "' and FrozenFor='N'");

                    if(lObjRecordsetCT.RecordCount < 1) {
                        /*if (Regex.Replace(lVarGp.Key, @"[$' ']", "") != "2040010006000")
                        {*/
                        if (!string.IsNullOrEmpty(lVarGp.Key) && lVarGp.Key.Length == 13)
                        {
                            lStrMsgAccounts += lStrMsgAccounts + " " + Regex.Replace(lVarGp.Key, @"[$' ']", "") + " -";
                        }
                        //}
                    }
                }
            }
            catch(Exception lObjException) {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordsetCT);
            }

            return lStrMsgAccounts;
        }

        internal string CheckCostingCode(IEnumerable<IGrouping<string, Nomina>> lVarGpCC) {
            Recordset lObjRecordsetCC = null;
            string lStrMsgCostingCode = string.Empty;

            try {
                lObjRecordsetCC = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                foreach(var lVarGp in lVarGpCC) {
                    lObjRecordsetCC.DoQuery("Select * from OOCR where OcrCode = '" + lVarGp.Key + "' and active = 'Y'");

                    if(lObjRecordsetCC.RecordCount < 1) {
                        if(lVarGp.Key != "") {
                            lStrMsgCostingCode += lStrMsgCostingCode + " " + Regex.Replace(lVarGp.Key, @"[$' ']", "") + " -";
                        }
                    }
                }
            }
            catch(Exception lObjException) {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally {
                MemoryUtility.ReleaseComObject(lObjRecordsetCC);
            }

            return lStrMsgCostingCode;
        }
    }
}
