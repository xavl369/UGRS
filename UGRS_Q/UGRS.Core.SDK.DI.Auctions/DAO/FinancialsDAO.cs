using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.DAO
{
    public class FinancialsDAO
    {
        public IList<object> GetDeliveriesFood(string pStrWhsCode, string pStrCardCode)
        {
            Recordset lObjRecordset = null;
            IList<object> lLstUnkResult = new List<object>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(GetDeliveriesFoodQuery(pStrWhsCode, pStrCardCode));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstUnkResult.Add(new 
                        {
                            DocType = lObjRecordset.Fields.Item("DocType").Value.ToString(),
                            DocNum = Convert.ToInt32(lObjRecordset.Fields.Item("DocNum").Value.ToString()),
                            DocEntry = Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString()),
                            CardCode = lObjRecordset.Fields.Item("CardCode").Value.ToString(),
                            LineNum = Convert.ToInt32(lObjRecordset.Fields.Item("LineNum").Value.ToString()),
                            WhsCode = lObjRecordset.Fields.Item("WhsCode").Value.ToString(),
                            ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString(),
                            Quantity = Convert.ToDouble(lObjRecordset.Fields.Item("Quantity").Value.ToString()),
                            Price = Convert.ToDecimal( lObjRecordset.Fields.Item("Price").Value.ToString())
                        });
                        lObjRecordset.MoveNext();
                    }
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }
            return lLstUnkResult;
        }

        public string GetPrice(string pStrWhsCode, string pStrItemCode )
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            string lStrPrice = "";

            try
            {
                string lStrQuery = GetItemPrice(pStrWhsCode, pStrItemCode);
                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lStrPrice = lObjRecordset.Fields.Item(1).Value.ToString();
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

            return lStrPrice;
        }

        private string GetDeliveriesFoodQuery(string pStrWhsCode, string pStrCardCode)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("WhsCode", pStrWhsCode);
            lLstStrParameters.Add("CardCode", pStrCardCode);
            return this.GetSQL("GetDeliveriesFood").Inject(lLstStrParameters);
        }

        private string GetItemPrice(string pStrWhsCode, string pStrItemCode)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("WhsCode", pStrWhsCode);
            lLstStrParameters.Add("ItemCode", pStrItemCode);
            return this.GetSQL("GetPrice").Inject(lLstStrParameters);
        }

        public double GetDocTotal(string pStrCardCode, string pStrNumAtCard)
        {
            SAPbobsCOM.Recordset lObjRecordset = null;
            try
            {
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);
                lLstStrParameters.Add("NumAtCard", pStrNumAtCard);

                string lStrQuery = this.GetSQL("GetDocTotalByFilters").Inject(lLstStrParameters);

                lObjRecordset = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    return Convert.ToDouble(lObjRecordset.Fields.Item(1).Value);
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
