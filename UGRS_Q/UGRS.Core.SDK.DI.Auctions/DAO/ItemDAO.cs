using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Auctions.DAO
{
    public class ItemDAO
    {
        QueryManager mObjQueryManager;

        public ItemDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public IList<string> GetItemCodesList()
        {
            Recordset lObjRecordset = null;
            IList<string> lLstStrResult = new List<string>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Property", GetAuctionsItemProperty());

                lObjRecordset.DoQuery(this.GetSQL("GetItemCodesList").Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstStrResult.Add(lObjRecordset.Fields.Item("ItemCode").Value.ToString());
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

            return lLstStrResult;
        }

        public IList<ItemDTO> GetUpdatedItemsList()
        {
            Recordset lObjRecordset = null;
            IList<ItemDTO> lLstObjResult = new List<ItemDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Property", GetAuctionsItemProperty());

                lObjRecordset.DoQuery(this.GetSQL("GetUpdatedItemCodesList").Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjResult.Add(new ItemDTO()
                        {
                            ItemCode = lObjRecordset.Fields.Item("ItemCode").Value.ToString(),
                            UpdateDate = Convert.ToDateTime(lObjRecordset.Fields.Item("UpdateDate").Value.ToString())
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

        public ItemDTO GetItemByCode(string pStrItemCode)
        {
            Recordset lObjRecordset = null;
            ItemDTO lObjItem = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);

                string lStrQuery = this.GetSQL("GetItemByCode").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lObjItem = GetItem(lObjRecordset);
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

            return lObjItem;
        }

        public DateTime GetUpdateDate(string pStrItemCode)
        {
            return Convert.ToDateTime(mObjQueryManager.GetValue("UpdateDate", "ItemCode", pStrItemCode, "OITM"));
        }

        private ItemDTO GetItem(Recordset pObjRecordset)
        {
            return new ItemDTO()
            {
                ItemCode = pObjRecordset.Fields.Item("ItemCode").Value.ToString(),
                ItemName = pObjRecordset.Fields.Item("ItemName").Value.ToString(),
                Valid = pObjRecordset.Fields.Item("validFor").Value.ToString() == "Y" ? true : false,

                CreateDate = pObjRecordset.Fields.Item("CreateDate").Value != null ?
                Convert.ToDateTime(pObjRecordset.Fields.Item("CreateDate").Value.ToString()) : DateTime.MinValue,

                UpdateDate = pObjRecordset.Fields.Item("UpdateDate").Value != null ?
                Convert.ToDateTime(pObjRecordset.Fields.Item("UpdateDate").Value.ToString()) : DateTime.MinValue,
            };
        }

        private string GetAuctionsItemProperty()
        {
            return ConfigurationUtility.GetValue<string>("AuctionsItemProperty");
        }
    }
}
