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
   public class RejectedDAO
    {
         QueryManager mObjQueryManager;

        public RejectedDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public string GetMainRWhs(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetMainWhs").InjectSingleValue("UsrId",pIntUserSign);

                lObjRecordSet.DoQuery(lStrQuery);

                if(lObjRecordSet.RecordCount > 0)
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

        public string GetMainWhs(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetMainWhs").InjectSingleValue("UsrId", pIntUserSign);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(1).Value;
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

        public string GetRejectedQuery(string pStrMainWhs)
        {
            string x = this.GetSQL("GetRejectedToInvoice").InjectSingleValue("WhsCode", pStrMainWhs);
            return this.GetSQL("GetRejectedToInvoice").InjectSingleValue("WhsCode",pStrMainWhs);
        }

        public string GetFilteredRejected(string pStrMainWhs, string pStrCardCode)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("WhsCode", pStrMainWhs);
            lLstStrParameters.Add("CardCode", pStrCardCode);

            return this.GetSQL("GetFilteredRejected").Inject(lLstStrParameters);
        }

        public string SearchExistentInvoices(string pStrReference)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("SearchExistentInvoices").InjectSingleValue("Reference", pStrReference);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    
                    return lObjRecordSet.Fields.Item("Fecha").Value.ToString();
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

       public string GetArticleToInvoice()
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("GetArticleToInvoice");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {

                    return lObjRecordSet.Fields.Item(0).Value.ToString();
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

       public string GetDefaultDays()
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
               string lStrQuery = this.GetSQL("GetDays");

               lObjRecordSet.DoQuery(lStrQuery);

               if (lObjRecordSet.RecordCount > 0)
               {
                   string x = (string)lObjRecordSet.Fields.Item(0).Value;
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


       public string GetTaxCode(string pStrArticle)
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
               string lStrQuery = this.GetSQL("GetTaxCode").InjectSingleValue("Article",pStrArticle);

               lObjRecordSet.DoQuery(lStrQuery);

               if (lObjRecordSet.RecordCount > 0)
               {

                   return lObjRecordSet.Fields.Item(0).Value.ToString();
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

       public double GetPrice(string pStrArticle, string pStrWhsCode)
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
               lLstStrParameters.Add("Article", pStrArticle);
               lLstStrParameters.Add("WhsCode", pStrWhsCode);


               string lStrQuery = this.GetSQL("GetPrice").Inject(lLstStrParameters);

               lObjRecordSet.DoQuery(lStrQuery);

               if (lObjRecordSet.RecordCount > 0)
               {

                   return (double)lObjRecordSet.Fields.Item(0).Value;
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

       public string GetWhs(int pIntUserSign)
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               string lStrQuery = this.GetSQL("GetWhs").InjectSingleValue("UsrId", pIntUserSign);

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


       public int GetSerieForOutputs(int pIntUserSign)
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               string lStrQuery = this.GetSQL("GetSerieForOutPuts").InjectSingleValue("UsrSign", pIntUserSign);

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
       public long GetLastIdInspection()
       {
           Recordset lObjRecordSet = null;
           try
           {
               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               string lStrQuery = this.GetSQL("GetLastIdInsp");

               lObjRecordSet.DoQuery(lStrQuery);

               if (lObjRecordSet.RecordCount > 0)
               {
                   return (long)lObjRecordSet.Fields.Item(0).Value;
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




       internal int GetDraftKey(string pStrReference, string pStrItemCode)
       {
           Recordset lObjRecordSet = null;
           try
           {

               Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
               lLstStrParameters.Add("Reference", pStrReference);
               lLstStrParameters.Add("ItemCode", pStrItemCode);

               lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

               string lStrQuery = this.GetSQL("GetDraftKey").Inject(lLstStrParameters);

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
