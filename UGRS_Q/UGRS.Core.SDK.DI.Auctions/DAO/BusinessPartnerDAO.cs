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
    public class BusinessPartnerDAO
    {
        QueryManager mObjQueryManager;

        public BusinessPartnerDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public IList<string> GetCardCodesList()
        {
            Recordset lObjRecordset = null;
            IList<string> lLstStrResult = new List<string>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(this.GetSQL("GetCardCodesList"));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstStrResult.Add(lObjRecordset.Fields.Item("CardCode").Value.ToString());
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

        public IList<CustomerDTO> GetUpdatedCardCodesList()
        {
            Recordset lObjRecordset = null;
            IList<CustomerDTO> lLstObjResult = new List<CustomerDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                lObjRecordset.DoQuery(this.GetSQL("GetUpdatedCardCodesList"));

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {

                        lLstObjResult.Add(new CustomerDTO()
                        {
                            CardCode = lObjRecordset.Fields.Item("CardCode").Value.ToString(),
                            UpdateDate = Convert.ToDateTime(lObjRecordset.Fields.Item("UpdateDate").Value.ToString()),
                            UpdateHour = lObjRecordset.Fields.Item("UpdateDate").Value != null ?
                            Convert.ToDateTime(lObjRecordset.Fields.Item("UpdateDate").Value.ToString()).AddTicks(
                            lObjRecordset.Fields.Item("UpdateTS").Value != null ?
                            Convert.ToInt32(lObjRecordset.Fields.Item("UpdateTS").Value.ToString()) * TimeSpan.TicksPerSecond : 0) : DateTime.MinValue,
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

        public CustomerDTO GetCustomerByCode(string pStrCardCode)
        {
            Recordset lObjRecordset = null;
            CustomerDTO lObjCustomer = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);

                string lStrQuery = this.GetSQL("GetCustomerByCardCode").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lObjCustomer = GetCustomer(lObjRecordset);
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

            return lObjCustomer;
        }

        public DateTime GetUpdateDateByCode(string pStrCardCode)
        {
            return Convert.ToDateTime(mObjQueryManager.GetValue("UpdateDate", "CardCode", pStrCardCode, "OCRD"));
        }

        public IList<CustomerDTO> SearchBusinessPartner(string pStrFilter)
        {
            Recordset lObjRecordset = null;
            IList<CustomerDTO> lLstObjResult = new List<CustomerDTO>();

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Filter", pStrFilter);

                string lStrQuery = this.GetSQL("SearchCustomer").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordset.RecordCount; i++)
                    {
                        lLstObjResult.Add(GetCustomer(lObjRecordset));
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

        private CustomerDTO GetCustomer(Recordset pObjRecordset)
        {
            return new CustomerDTO()
            {
                CardCode = pObjRecordset.Fields.Item("CardCode").Value.ToString(),
                CardName = pObjRecordset.Fields.Item("CardName").Value.ToString(),
                CardFName = pObjRecordset.Fields.Item("CardFName").Value.ToString(),
                TaxCode = pObjRecordset.Fields.Item("LicTradNum").Value.ToString(),
                Valid = pObjRecordset.Fields.Item("validFor").Value.ToString() == "Y" ? true : false,

                CreateDate = pObjRecordset.Fields.Item("CreateDate").Value != null ?
                Convert.ToDateTime(pObjRecordset.Fields.Item("CreateDate").Value.ToString()) : DateTime.MinValue,

                UpdateDate = pObjRecordset.Fields.Item("UpdateDate").Value != null ?
                Convert.ToDateTime(pObjRecordset.Fields.Item("UpdateDate").Value.ToString()) : DateTime.MinValue,

                UpdateHour = pObjRecordset.Fields.Item("UpdateDate").Value != null ?
                           Convert.ToDateTime(pObjRecordset.Fields.Item("UpdateDate").Value.ToString()).AddTicks(
                           pObjRecordset.Fields.Item("UpdateTS").Value != null ?
                           Convert.ToInt32(pObjRecordset.Fields.Item("UpdateTS").Value.ToString()) * TimeSpan.TicksPerSecond : 0) : DateTime.MinValue,


            };
        }
    }
}
