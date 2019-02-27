using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.DAO
{
    public class ItemBatchDAO
    {
        QueryManager mObjQueryManager;

        public ItemBatchDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public IList<ItemBatchDTO> GetItemBatchesListByWarehouse(string pStrWhsCode, DateTime pAuctionDate)
        {
            Recordset lObjRecordset = null;
            IList<ItemBatchDTO> lLstObjResult = new List<ItemBatchDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);
                lLstStrParameters.Add("ActAuctionDate",  pAuctionDate == DateTime.MinValue ? "" : pAuctionDate.ToString("yyyy-MM-dd"));

                string lStrQuery = this.GetSQL("GetItemBatchesListByWarehouse").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjResult.Add(new ItemBatchDTO()
                        {
                            ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString(),
                            CardCode = lObjRecordset.Fields.Item("CardCode").Value.ToString(),
                            BatchNumber = lObjRecordset.Fields.Item("BatchNumber").Value.ToString(),
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

            return lLstObjResult;
        }

        public IList<ItemBatchDTO> GetUpdatedItemBatchesListByWarehouse(string pStrWhsCode)
        {
            Recordset lObjRecordset = null;
            IList<ItemBatchDTO> lLstObjResult = new List<ItemBatchDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);

                string lStrQuery = this.GetSQL("GetUpdatedItemBatchesListByWarehouse").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjResult.Add(new ItemBatchDTO()
                        {
                            ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString(),
                            CardCode = lObjRecordset.Fields.Item("CardCode").Value.ToString(),
                            BatchNumber = lObjRecordset.Fields.Item("BatchNumber").Value.ToString(),

                            UpdateDate = Convert.ToDateTime(lObjRecordset.Fields.Item("UpdatedDate").Value),

                            CreateDate = Convert.ToDateTime(lObjRecordset.Fields.Item("InDate").Value) != DateTime.MinValue ?
                            Convert.ToDateTime(lObjRecordset.Fields.Item("InDate").Value.ToString()) : DateTime.MinValue,
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

            return lLstObjResult;
        }

        public ItemBatchDTO GetItemBacthByFilters(string pStrWhsCode, string pStrItemCode, string pStrBatchNumber)
        {
            Recordset lObjRecordset = null;
            ItemBatchDTO lObjResult = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                lLstStrParameters.Add("BatchNumber", pStrBatchNumber);

                string lStrQuery = this.GetSQL("GetItemBatcheByFilters").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lObjResult = GetItemBatch(lObjRecordset);
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

            return lObjResult;
        }

        public IList<ItemBatchDTO> GetItemBatchesListByFilters(string pStrCardCode, string pStrItemCode, string pStrWhsCode)
        {
            Recordset lObjRecordset = null;
            IList<ItemBatchDTO> lLstObjResult = new List<ItemBatchDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("WhsCode", pStrWhsCode);
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                lLstStrParameters.Add("CardCode", pStrCardCode);

                string lStrQuery = this.GetSQL("GetBatchNumerListByFilters").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjResult.Add(GetItemBatch(lObjRecordset));
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

            return lLstObjResult;
        }

        private ItemBatchDTO GetItemBatch(Recordset pObjRecordset)
        {
            ItemBatchDTO lObjResult = new ItemBatchDTO();

            lObjResult.ItemCode = pObjRecordset.Fields.Item("ItemCode").Value.ToString();
            lObjResult.CardCode = pObjRecordset.Fields.Item("CardCode").Value.ToString();
            lObjResult.BatchNumber = pObjRecordset.Fields.Item("BatchNumber").Value.ToString();
            lObjResult.InitialWarehouse = pObjRecordset.Fields.Item("InitialWarehouse").Value.ToString();
            lObjResult.CurrentWarehouse = pObjRecordset.Fields.Item("CurrentWarehouse").Value.ToString();
            lObjResult.Folio = pObjRecordset.Fields.Item("Folio").Value.ToString();

            lObjResult.ChargeFood =
                pObjRecordset.Fields.Item("ChargeFood").Value.ToString().Equals("Y") ||
                pObjRecordset.Fields.Item("ChargeFood").Value.ToString().Equals("S");

            lObjResult.Payment = pObjRecordset.Fields.Item("Payment").Value.ToString().Equals("Y") ||
                pObjRecordset.Fields.Item("Payment").Value.ToString().Equals("S");

            lObjResult.Quantity = Convert.ToInt32(pObjRecordset.Fields.Item("Quantity").Value.ToString());

            lObjResult.ExpirationDate = pObjRecordset.Fields.Item("ExpDate") != null ?
            Convert.ToDateTime(pObjRecordset.Fields.Item("ExpDate").Value.ToString()) : DateTime.MinValue;

            lObjResult.CreateDate = pObjRecordset.Fields.Item("InDate") != null ?
            Convert.ToDateTime(pObjRecordset.Fields.Item("InDate").Value.ToString()) : DateTime.MinValue;

            lObjResult.UpdateDate = pObjRecordset.Fields.Item("UpdatedDate") != null ?
            Convert.ToDateTime(pObjRecordset.Fields.Item("UpdatedDate").Value.ToString()) : DateTime.MinValue;

            return lObjResult;
        }
    }
}
