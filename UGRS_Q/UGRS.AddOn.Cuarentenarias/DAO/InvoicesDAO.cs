using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using SAPbobsCOM;
using UGRS.Core.Utility;
using UGRS.Core.Exceptions;
using UGRS.AddOn.Cuarentenarias.DTO;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.Cuarentenarias.DAO
{
    public class InvoicesDAO
    {
        QueryManager mObjQueryManager;

        public InvoicesDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public string GetInspectionsQuery(string pStrExpDate, int pIntUserSign)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("DateInsp", pStrExpDate);
            lLstStrParameters.Add("UserSign", pIntUserSign.ToString());
            //var a = "dadasd {Nombre}".InjectSingleValue("Name", "Raul");
            string x = this.GetSQL("GetInspectionsToInv").Inject(lLstStrParameters);
            return this.GetSQL("GetInspectionsToInv").Inject(lLstStrParameters);
        }

        public bool SearchCustomUGRS(int pIntIdInsp)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("SearchCustomUGRS").InjectSingleValue("IdInsp", pIntIdInsp);

                lObjRecordSet.DoQuery(lStrQuery);

                if ((string)lObjRecordSet.Fields.Item(0).Value == "Y")
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

        public bool SearchDrafts(string pStrReference, string pWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Reference", pStrReference);
                lLstStrParameters.Add("Corral", pWhsCode);
                string lStrQuery = this.GetSQL("SearchDrafts").Inject(lLstStrParameters);
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

        public bool SearchInvoices(string pStrReference, string pWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Reference", pStrReference);
                lLstStrParameters.Add("Corral", pWhsCode);
                string lStrQuery = this.GetSQL("SearchInvoices").Inject(lLstStrParameters);
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

        public IList<ConceptsToInvoiceDTO> GetChargesToInvoce(int lIntInsp, string lStrWhsCode, double lDbTotalWeight, int lIntSeries, string lStrCheck)
        {
            Recordset lObjRecordSet = null;
            IList<ConceptsToInvoiceDTO> lLstObjResult = new List<ConceptsToInvoiceDTO>();
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("IdInsp", lIntInsp.ToString());
                lLstStrParameters.Add("WhsCode", lStrWhsCode);
                lLstStrParameters.Add("TotWeight", lDbTotalWeight.ToString());
                lLstStrParameters.Add("Serie", lIntSeries.ToString());
                lLstStrParameters.Add("ThreeP", lStrCheck);

                string lStrQuery = this.GetSQL("GetCharges").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {

                        //int x = (int)lObjRecordSet.Fields.Item("DocEntry").Value;
                        //string y = lObjRecordSet.Fields.Item("ItemCode").Value.ToString();
                        //int w = int.Parse(lObjRecordSet.Fields.Item("ObjType").Value.ToString());
                        //int g = (int)lObjRecordSet.Fields.Item("LineNum").Value;
                        //double gh = double.Parse(lObjRecordSet.Fields.Item("Quantity").Value.ToString());
                        //double fdif = double.Parse(lObjRecordSet.Fields.Item("Price").Value.ToString());
                        //string jj = lObjRecordSet.Fields.Item("TaxCode").Value.ToString();
                        //string soe = lObjRecordSet.Fields.Item("WhsCode").Value.ToString();
                        //string fjfj = lObjRecordSet.Fields.Item("UomCode").Value.ToString();      


                        if (lObjRecordSet.Fields.Item("Price").Value.ToString() != "")
                        {
                            lLstObjResult.Add(new ConceptsToInvoiceDTO()
                          {



                              DocEntry = (int)lObjRecordSet.Fields.Item("DocEntry").Value,
                              ItemCode = lObjRecordSet.Fields.Item("ItemCode").Value.ToString(),
                              ObjType = lObjRecordSet.Fields.Item("ObjType").Value.ToString(),
                              LineNum = (int)lObjRecordSet.Fields.Item("LineNum").Value,
                              Quantity = double.Parse(lObjRecordSet.Fields.Item("Quantity").Value.ToString()),
                              Price = double.Parse(lObjRecordSet.Fields.Item("Price").Value.ToString()),
                              TaxCode = lObjRecordSet.Fields.Item("TaxCode").Value.ToString(),
                              WhsCode = lObjRecordSet.Fields.Item("WhsCode").Value.ToString(),
                              UomCode = lObjRecordSet.Fields.Item("UomCode").Value.ToString()

                          });
                            lObjRecordSet.MoveNext();
                        }
                        else
                        {
                            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("El total viene núlo"
                 , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception lObjException)
            {
                string er = DIApplication.Company.GetLastErrorDescription();
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }


        public string GetBatchToCancel(string pStrExpDate, string pStrCardCode, string pStrMainWhs, string pStrIdInsp)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("ExpDate", pStrExpDate);
            lLstStrParameters.Add("CardCode", pStrCardCode);
            lLstStrParameters.Add("MainWhs", pStrMainWhs);
            lLstStrParameters.Add("IdInsp", pStrIdInsp);

            string x = this.GetSQL("GetBatchToCancel").Inject(lLstStrParameters);
            return this.GetSQL("GetBatchToCancel").Inject(lLstStrParameters);
        }

        public int GetDocEntry(string pStrReference)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetDocEntry").InjectSingleValue("Reference", pStrReference);

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


        public string GetBatch(int pIntBaseEntry, string pStrWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("BaseEntry", pIntBaseEntry.ToString());
                lLstStrParameters.Add("WhsCode", pStrWhsCode);

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetBatch").Inject(lLstStrParameters);

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
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

        }

        public string GetPriceByWhs(string pStrItemCode, string pStrWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                lLstStrParameters.Add("WhsCode", pStrWhsCode);

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetPriceByWhs").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return Convert.ToString(lObjRecordSet.Fields.Item(0).Value);
                }
                else
                {
                    return "0";
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



        internal int GetDraftKey(string pStrReference)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetDraftByRef").InjectSingleValue("Reference", pStrReference);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (int)lObjRecordSet.Fields.Item("DocEntry").Value;
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

        internal string GetCurrencyByBP(string pStrCardCode)
        {

            Recordset lObjRecordSet = null;
            try
            {


                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCurrencyByBP").InjectSingleValue("CardCode", pStrCardCode);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
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

        public IList<ConceptsDTO> GetConceptsDlls()
        {
            Recordset lObjRecordSet = null;
            IList<ConceptsDTO> lLstObjResult = new List<ConceptsDTO>();
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("GetConceptsDlls");
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstObjResult.Add(new ConceptsDTO()
                                {

                                    Value = (string)lObjRecordSet.Fields.Item("U_Value").Value,
                                    Description = (string)lObjRecordSet.Fields.Item("Name").Value,

                                });
                        lObjRecordSet.MoveNext();
                    }


                }
            }
            catch (Exception lObjException)
            {
                string er = DIApplication.Company.GetLastErrorDescription();
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }


        public IList<ConceptsDTO> GetConceptsMXN()
        {
            Recordset lObjRecordSet = null;
            IList<ConceptsDTO> lLstObjResult = new List<ConceptsDTO>();
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("GetConceptsMX");
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstObjResult.Add(new ConceptsDTO()
                        {

                            Value = (string)lObjRecordSet.Fields.Item("U_Value").Value,
                            Description = (string)lObjRecordSet.Fields.Item("Name").Value,

                        });
                        lObjRecordSet.MoveNext();
                    }


                }
            }
            catch (Exception lObjException)
            {
                string er = DIApplication.Company.GetLastErrorDescription();
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }

        public string GetCostingCode(string pStrUsrName)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCostingCode").InjectSingleValue("UsrName", pStrUsrName);

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
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }
        public string GetPrincipalWhs(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetMainWhs").InjectSingleValue("UsrId", pIntUserSign);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item("principalWhs").Value;
                }
                else
                {
                    return "0";
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



        internal IList<ConceptsToInvoiceDTO> GetChargesToInvoce(string pStrXmLString)
        {
            Recordset lObjRecordSet = null;
            IList<ConceptsToInvoiceDTO> lLstObjResult = new List<ConceptsToInvoiceDTO>();
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCharges").InjectSingleValue("xmlString", pStrXmLString);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {

                        if (Convert.ToInt32(lObjRecordSet.Fields.Item("DocEntry").Value) >= 0)
                        {

                            if (lObjRecordSet.Fields.Item("Price").Value.ToString() != "")
                            {
                                lLstObjResult.Add(new ConceptsToInvoiceDTO()
                                {


                                    IdInspection = Convert.ToString(lObjRecordSet.Fields.Item("Inspeccion").Value),
                                    DocEntry = Convert.ToInt32(lObjRecordSet.Fields.Item("DocEntry").Value),
                                    ItemCode = lObjRecordSet.Fields.Item("ItemCode").Value.ToString(),
                                    ObjType = (lObjRecordSet.Fields.Item("ObjType").Value.ToString()),
                                    LineNum = Convert.ToInt32(lObjRecordSet.Fields.Item("LineNum").Value),
                                    Quantity = double.Parse(lObjRecordSet.Fields.Item("Quantity").Value.ToString()),
                                    Price = double.Parse(lObjRecordSet.Fields.Item("Price").Value.ToString()),
                                    TaxCode = lObjRecordSet.Fields.Item("TaxCode").Value.ToString(),
                                    WhsCode = lObjRecordSet.Fields.Item("WhsCode").Value.ToString(),
                                    UomCode = lObjRecordSet.Fields.Item("UomCode").Value.ToString(),
                                    InvoiceType = lObjRecordSet.Fields.Item("TipoFC").Value.ToString()

                                });
                                lObjRecordSet.MoveNext();
                            }
                            else
                            {
                                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("El total viene núlo para " + lObjRecordSet.Fields.Item("ItemCode").Value.ToString()
                     , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                lObjRecordSet.MoveNext();
                            }
                        }
                        else
                        {
                            string w = lObjRecordSet.Fields.Item("ObjType").Value.ToString();
                            int xfs = Convert.ToInt32(lObjRecordSet.Fields.Item("DocEntry").Value);

                            lLstObjResult.Add(new ConceptsToInvoiceDTO()
                            {
                                DocEntry = Convert.ToInt32(lObjRecordSet.Fields.Item("DocEntry").Value),
                                ObjType = (string)lObjRecordSet.Fields.Item("ObjType").Value,
                            });
                            lObjRecordSet.MoveNext();
                        }
                    }
                }
            }
            catch (Exception lObjException)
            {
                string er = DIApplication.Company.GetLastErrorDescription();
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }

        internal string GetServerDate()
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
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

    }
}
