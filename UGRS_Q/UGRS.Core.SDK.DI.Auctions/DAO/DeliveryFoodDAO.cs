using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.DAO
{
    public class DeliveryFoodDAO
    {
        public IList<int> GetDeliveriesFoodList(string pStrWhsCode)
        {
            Recordset lObjRecordset = null;
            IList<int> lLstIntDocEntries = new List<int>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);

                lObjRecordset.DoQuery(this.GetSQL("GetDeliveriesFoodList").Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstIntDocEntries.Add(Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString()));
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

            return lLstIntDocEntries;
        }

        public IList<DeliveryFoodDTO> GetUpdatedDeliveriesFoodList(string pStrWhsCode)
        {
            Recordset lObjRecordset = null;
            IList<DeliveryFoodDTO> lLstObjDeliveriesFood = new List<DeliveryFoodDTO>();
            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);
                string pstr = this.GetSQL("GetUpdatedDeliveriesFoodList").Inject(lLstStrParameters);

                lObjRecordset.DoQuery(this.GetSQL("GetUpdatedDeliveriesFoodList").Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {

                        lLstObjDeliveriesFood.Add(GetDelivery(lObjRecordset));
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

            return lLstObjDeliveriesFood;
        }

        public IList<DeliveryFoodDTO> GetDeliveriesFood(int pIntDocEntry)
        {
            Recordset lObjRecordset = null;
            IList<DeliveryFoodDTO> lLstObjDeliveriesFood = new List<DeliveryFoodDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("DocEntry", pIntDocEntry.ToString());

                lObjRecordset.DoQuery(this.GetSQL("GetDeliveryFood").Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjDeliveriesFood.Add(GetDelivery(lObjRecordset));
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
            return lLstObjDeliveriesFood;
        }

        private DeliveryFoodDTO GetDelivery(Recordset lObjRecordset)
        {
            DeliveryFoodDTO lObjDeliveryFood = new DeliveryFoodDTO();

            lObjDeliveryFood.DocEntry = Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString());
            lObjDeliveryFood.DocNum = Convert.ToInt32(lObjRecordset.Fields.Item("DocNum").Value.ToString());
            lObjDeliveryFood.DocType = lObjRecordset.Fields.Item("DocType").Value.ToString();
            lObjDeliveryFood.CardCode = lObjRecordset.Fields.Item("CardCode").Value.ToString();
            lObjDeliveryFood.WhsCode = lObjRecordset.Fields.Item("WhsCode").Value.ToString();
            lObjDeliveryFood.ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString();
            lObjDeliveryFood.TaxCode = lObjRecordset.Fields.Item("TaxCode").Value.ToString();
            lObjDeliveryFood.BatchNumber = lObjRecordset.Fields.Item("BatchNumber").Value.ToString();
            lObjDeliveryFood.LineNum = Convert.ToInt32(lObjRecordset.Fields.Item("LineNum").Value.ToString());
            lObjDeliveryFood.Quantity = Convert.ToDouble(lObjRecordset.Fields.Item("Quantity").Value.ToString());
            lObjDeliveryFood.Price = Convert.ToDecimal(lObjRecordset.Fields.Item("Price").Value.ToString());
            lObjDeliveryFood.Opened = lObjRecordset.Fields.Item("DocStatus").Value.ToString() != "O" ? false : true;

            string dd = lObjRecordset.Fields.Item("CreateTS").Value.ToString();
            string pd = lObjRecordset.Fields.Item("UpdateTS").Value.ToString();

            DateTime lObjCreationHour = DateTime.Now;

            DateTime lObjModificationHour = DateTime.Now;

            lObjCreationHour = DateTime.ParseExact(dd.PadLeft(6, '0'), "HHmmss", null);

            lObjModificationHour = DateTime.ParseExact(pd.PadLeft(6, '0'), "HHmmss", null);


            lObjDeliveryFood.CreateDate = lObjRecordset.Fields.Item("CreateDate").Value != null ?
            Convert.ToDateTime(lObjRecordset.Fields.Item("CreateDate").Value.ToString()) : DateTime.MinValue;

            lObjDeliveryFood.UpdateDate = lObjRecordset.Fields.Item("UpdateDate").Value != null ?
            Convert.ToDateTime(lObjRecordset.Fields.Item("UpdateDate").Value.ToString()) : DateTime.MinValue;


            lObjDeliveryFood.CreateDate = lObjDeliveryFood.CreateDate.Date.Add(lObjCreationHour.TimeOfDay);
            lObjDeliveryFood.UpdateDate = lObjDeliveryFood.UpdateDate.Date.Add(lObjModificationHour.TimeOfDay);

            return lObjDeliveryFood;
        }
    }
}
