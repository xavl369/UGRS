// file:	Database\QueryManager.cs
// summary:	Implements the query manager class

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGRS.Core.Enums.Query;
using UGRS.Core.SDK.DI.Exceptions;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Models;
using System.Globalization;

namespace UGRS.Core.SDK.DI.DAO
{
    /// <summary> Manager for queries. </summary>
    /// <remarks> Ranaya, 09/05/2017. </remarks>

    public class QueryManager
    {
        public bool ExistsUserField(string pStrTableName, string pStrFieldName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("TableName", pStrTableName);
                lLstStrParameters.Add("FieldName", pStrFieldName);

                lStrQuery = this.GetSQL("ExistsUserField").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new QueryException(e.Message, e);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public bool Exists(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("ExistsTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetValue(string pStrSelectFieldName, string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("SelectFieldName", pStrSelectFieldName);
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetValueTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return lObjRecordSet.Fields.Item(pStrSelectFieldName).Value.ToString();
                }

                return string.Empty;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T GetTableObject<T>(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : Table
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetObjectTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return lObjRecordSet.GetTableObject<T>();
                }
                return null;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public IList<T> GetObjectsList<T>(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : Table
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            IList<T> lLstResult = new List<T>();
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("GetObjectTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstResult.Add(lObjRecordSet.GetTableObject<T>());
                        lObjRecordSet.MoveNext();
                    }

                    return lLstResult;
                }

                return null;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int Count(string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("CountTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("Count").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T Max<T>(string pStrFieldName, string pStrTableName) where T : IConvertible
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("FieldName", pStrFieldName);
                lLstStrParameters.Add("TableName", pStrTableName);

                lStrQuery = this.GetSQL("MaxTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (T)Convert.ChangeType(lObjRecordSet.Fields.Item("Max").Value.ToString(), typeof(T));
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public T Max<T>(string pStrFieldName, string pStrWhereFieldName, string pStrWhereFieldValue, string pStrTableName) where T : IConvertible
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("FieldName", pStrFieldName);
                lLstStrParameters.Add("TableName", pStrTableName);
                lLstStrParameters.Add("WhereFieldName", pStrWhereFieldName);
                lLstStrParameters.Add("WhereFieldValue", pStrWhereFieldValue);

                lStrQuery = this.GetSQL("ConditionalMaxTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (T)Convert.ChangeType(lObjRecordSet.Fields.Item("Max").Value.ToString(), typeof(T));
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int GetSeriesByName(string pStrObjectCode, string pStrSeriesName)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ObjectCode", pStrObjectCode);
                lLstStrParameters.Add("SeriesName", pStrSeriesName);

                lStrQuery = this.GetSQL("GetSeriesByNameTemplate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("Series").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int GetDocEntryByAuctionAndPartner(string pStrCardCode, string pStrAuction)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);
                lLstStrParameters.Add("Auction", pStrAuction);

                lStrQuery = this.GetSQL("GetDocEntry").Inject(lLstStrParameters);

                lStrQuery = string.Format(@"SELECT DocEntry FROM 
                                            OINV
                                            WHERE 
                                            NumAtCard = '{0}'
                                            AND
                                            CardCode = '{1}'", pStrAuction, pStrCardCode);

                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("DocEntry").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }


        public int GetDraftDocEntry(string pStrCardCode, string pStrAuction)
        {

            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);
                lLstStrParameters.Add("Auction", pStrAuction);

                lStrQuery = this.GetSQL("GetDraftDocEntry").Inject(lLstStrParameters);

                lStrQuery = string.Format(@"
                                                SELECT DocEntry FROM ODRF
                                                WHERE 
                                                NumAtCard = '{0}'
                                                AND
                                                CardCode = '{1}'", pStrAuction, pStrCardCode);

                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("DocEntry").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int GetPaymentCondition(string pStrCardCode)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);


                lStrQuery = this.GetSQL("GetPayCondition").Inject(lLstStrParameters);

                lStrQuery = string.Format(@"
                SELECT
                case when t1.PymntGroup like'%Contado%' then 10
                else t0.GroupNum
                end as GroupNumber

                FROM OCRD T0
                INNER JOIN OCTG T1 on T1.GroupNum = t0.GroupNum
                WHERE CardCode = '{0}' ", pStrCardCode);

                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("GroupNumber").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetAccountByType(string pStrAuxiliaryType)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrAuxiliaryType);

                lStrQuery = this.GetSQL("GetDraftDocEntry").Inject(lLstStrParameters);

                lStrQuery = string.Format(@"
                                            SELECT U_Value FROM [@UG_CONFIG]
                                            WHERE Name = '{0}'", pStrAuxiliaryType);

                lObjRecordSet.DoQuery(lStrQuery);

                return (string)lObjRecordSet.Fields.Item("U_Value").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetThreePercentArticle()
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"
                                            SELECT U_Value FROM [@UG_CONFIG]
                                            WHERE NAME = 'SU_ITEMPRP'
                                      ");

                lObjRecordSet.DoQuery(lStrQuery);

                return (string)lObjRecordSet.Fields.Item("U_Value").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetTaxCodeByArticle(string pStrArticle)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lStrQuery = string.Format(@"SELECT LnTaxCode FROM OTCX WHERE StrVal1 ='{0}'", pStrArticle);

                lObjRecordSet.DoQuery(lStrQuery);

                return (string)lObjRecordSet.Fields.Item("LnTaxCode").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public double GetTaxImport(string pStrTaxCode)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"
                                            SELECT Rate FROM OSTC WHERE Code = '{0}'", pStrTaxCode);

                lObjRecordSet.DoQuery(lStrQuery);

                return (double)lObjRecordSet.Fields.Item("Rate").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public int GetTransId(string pStrAuctionFolio)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"select TransId from ojdt where ref1 = '{0}' and  Memo like '%Cierre de subasta%'", pStrAuctionFolio);

                lObjRecordSet.DoQuery(lStrQuery);

                return (int)lObjRecordSet.Fields.Item("TransId").Value;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public double GetThreePercentPrice(string pStrItemCode,string pStrLocation)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"
                                            select T0.Price from ITM1 T0
                                            inner join OPLN T1 on T1.ListNum = T0.PriceList
                                            where U_GLO_Location = '{0}' and t0.ItemCode = '{1}'
                                      ",pStrLocation, pStrItemCode);

                lObjRecordSet.DoQuery(lStrQuery);

                return Convert.ToDouble(lObjRecordSet.Fields.Item("Price").Value);
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public bool BatchExists(string pStrAuctFolio, int pIntNumber)
        {

            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lStrQuery = string.Format(@"
                SELECT T0.U_Id
                  FROM [@UG_SU_BAHS] T0
                  INNER JOIN [@UG_SU_AUTN] T1 on t0.U_AuctionId = t1.U_Id 

                  where t1.U_Folio = '{0}' and t0.U_Number = {1}"
                    , pStrAuctFolio,pIntNumber.ToString());

                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? true : false;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public long GetBatchId(int pIntBatchNumber, string pStrAuctionFolio)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"
                                SELECT T0.U_Id
                                  FROM [@UG_SU_BAHS] T0
                                  INNER JOIN [@UG_SU_AUTN] T1 on t0.U_AuctionId = t1.U_Id 

                                  where t1.U_Folio = '{0}' and t0.U_Number = {1}
                                            ", pStrAuctionFolio,pIntBatchNumber.ToString());

                lObjRecordSet.DoQuery(lStrQuery);

                return Convert.ToInt64(lObjRecordSet.Fields.Item("U_Id").Value);
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public DateTime GetLastDateByLocation(string pStrFieldName, string pStrTableName, string pStrlocation)
        {

            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lStrQuery = string.Format(@"
                SELECT MAX({0}) AS Max
                  FROM {1}
                  where U_Location = '{2}'"
                    , pStrFieldName,pStrTableName ,pStrlocation);

                lObjRecordSet.DoQuery(lStrQuery);


                return lObjRecordSet.Fields.Item("Max").Value != null ? Convert.ToDateTime(lObjRecordSet.Fields.Item("Max").Value) : DateTime.MinValue;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public DateTime GetLastBatchDateByLocation(string pStrFieldName, string pStrTableName, string pStrlocation)
        {

            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lStrQuery = string.Format(@"
                SELECT MAX({0}) AS Max
                  FROM {1}
                  INNER JOIN [@UG_SU_AUTN] T1 on t0.U_AuctionId = t1.U_Id 
                  where T1.U_Location = '{2}'"
                    , pStrFieldName, pStrTableName, pStrlocation);

                lObjRecordSet.DoQuery(lStrQuery);


                return lObjRecordSet.Fields.Item("Max").Value != null ? Convert.ToDateTime(lObjRecordSet.Fields.Item("Max").Value) : DateTime.MinValue;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public long GetAuctionId(string pStrAuctionFolio)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                lStrQuery = string.Format(@"
                                  SELECT U_Id FROM [@UG_SU_AUTN]
                                    WHERE U_Folio = '{0}'
                                            ", pStrAuctionFolio);

                lObjRecordSet.DoQuery(lStrQuery);

                return Convert.ToInt64(lObjRecordSet.Fields.Item("U_Id").Value);
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

        }

        public SAPbouiCOM.DataTable GetBatchesByFilters(string pStrBuyer, string pStrSeller, string pStrSellerTaxCode, string pStrDate)
        {
            SAPbouiCOM.DataTable lDtbResult = new SAPbouiCOM.DataTable();
            Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                pStrDate = ValidStringDate(pStrDate);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Buyer", pStrBuyer);
                lLstStrParameters.Add("Seller", pStrSeller);
                lLstStrParameters.Add("SellerTaxCode", pStrSellerTaxCode);
                lLstStrParameters.Add("Date", pStrDate);

                string lStrQuery = this.GetSQL("GetBatchesByFilters").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);
                lDtbResult = (SAPbouiCOM.DataTable)lObjRecordset.ToDataTable();
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lDtbResult;
        }

        private string ValidStringDate(string pStrDate)
        {
            try
            {
                DateTime.Parse(pStrDate);
                return pStrDate;
            }
            catch
            {
                return "";
            }
        }

        public double GetDocTotal(string pStrCardCode, string pStrNumAtCard)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                string lStrQuery = string.Format(@"
    SELECT DocTotal FROM OINV WHERE NumAtCard = '{0}'  AND CardCode = '{1}'", pStrNumAtCard, pStrCardCode);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return Convert.ToDouble(lObjRecordset.Fields.Item(0).Value);
                }
            }
            catch
            {
                //Ignore
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return 0;
        }

        public int GetExportFormat()
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                string lStrQuery = string.Format(@"select ParamValue from ECM1 where ParamName in ('MappingDOC')");

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return Convert.ToInt32(lObjRecordset.Fields.Item(0).Value);
                }
            }
            catch
            {
                //Ignore
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return 0;
        }

    }
}
