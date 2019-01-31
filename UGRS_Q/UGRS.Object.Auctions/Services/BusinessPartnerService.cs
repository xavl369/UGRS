using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Business;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.SDK.DI.Auctions.Enum;
using UGRS.Core.Utility;

namespace UGRS.Object.Auctions.Services
{
    [Serializable]
    public class BusinessPartnerService
    {
        #region Attributes

        Core.SDK.DI.Auctions.Services.BusinessPartnerSevice mObjSapBusinessPartnerService;
        Core.Auctions.Services.Business.PartnerService mObjLocalBusinessPartnerService;
        Core.Auctions.Services.Auctions.BatchService mObjLocalBatchService;
        Core.Auctions.Services.Inventory.GoodsReceiptService mObjLocalGoodsReceiptService;
        Core.Auctions.Services.Inventory.GoodsIssueService mObjLocalGoodsIssueService;
        Core.Auctions.Services.Inventory.GoodsReturnService mObjLocalGoodsReturnService;

        #endregion

        #region Properties

        public Core.SDK.DI.Auctions.Services.BusinessPartnerSevice SapBusinessPartnerService
        {
            get { return mObjSapBusinessPartnerService; }
            set { mObjSapBusinessPartnerService = value; }
        }

        public Core.Auctions.Services.Business.PartnerService LocalBusinessPartnerService
        {
            get { return mObjLocalBusinessPartnerService; }
            set { mObjLocalBusinessPartnerService = value; }
        }

        public Core.Auctions.Services.Auctions.BatchService LocalBatchService
        {
            get { return mObjLocalBatchService; }
            set { mObjLocalBatchService = value; }
        }

        public Core.Auctions.Services.Inventory.GoodsReceiptService LocalGoodsReceiptService
        {
            get { return mObjLocalGoodsReceiptService; }
            set { mObjLocalGoodsReceiptService = value; }
        }

        public Core.Auctions.Services.Inventory.GoodsIssueService LocalGoodsIssueService
        {
            get { return mObjLocalGoodsIssueService; }
            set { mObjLocalGoodsIssueService = value; }
        }

        public Core.Auctions.Services.Inventory.GoodsReturnService LocalGoodsReturnService
        {
            get { return mObjLocalGoodsReturnService; }
            set { mObjLocalGoodsReturnService = value; }
        }

        #endregion

        #region Contructor

        public BusinessPartnerService()
        {
            SapBusinessPartnerService = new Core.SDK.DI.Auctions.AuctionsServicesFactory().GetBusinessPartnerSevice();
            LocalBusinessPartnerService = new Data.Auctions.Factories.BusinessServicesFactory().GetPartnerService();
            LocalBatchService = new Data.Auctions.Factories.AuctionsServicesFactory().GetBatchService();
            LocalGoodsReceiptService = new Data.Auctions.Factories.InventoryServicesFactory().GetGoodsReceiptService();
            LocalGoodsIssueService = new Data.Auctions.Factories.InventoryServicesFactory().GetGoodsIssueService();
            LocalGoodsReturnService = new Data.Auctions.Factories.InventoryServicesFactory().GetGoodsReturnService();
        }

        #endregion 

        #region Methods

        public void ImportCustomers()
         {
            IList<string> lLstStrLocalCardCodes = LocalBusinessPartnerService.GetList().Select(x => x.Code).ToList();

            foreach (string lStrCardCode in SapBusinessPartnerService.GetCardCodesList().Where(x=> !lLstStrLocalCardCodes.Contains(x)))
            {
                ImportCustomer(lStrCardCode);
            }
        }

        public void UpdateCustomers()
        {
            foreach (CustomerDTO lObjCustomer in SapBusinessPartnerService.GetUpdatedCardCodesList())
            {
                if (CustomerHasChanges(lObjCustomer))
                {
                    UpdateCustomer(lObjCustomer.CardCode);
                }
            }
        }

        public IList<CustomerDTO> SearchBusinessPartner(string pStrFilter)
        {
            return SapBusinessPartnerService.SearchBusinessPartner(pStrFilter);
        }

        public void BusinessPartnerMapping(IList<CustomerMappingDTO> pLstObjMapping) 
        {
            foreach (CustomerMappingDTO lObjMapping in pLstObjMapping.Where(x=> x.Autorize))
            {
                Partner lObjPartner = LocalBusinessPartnerService.GetEntity(lObjMapping.LocalPartnerId);

                if (lObjMapping.Type == UGRS.Core.SDK.DI.Auctions.Enum.MappingTypeEnum.NEW)
                {
                    ExportBusinessPartner(lObjPartner);
                }
                else if (lObjMapping.Type == UGRS.Core.SDK.DI.Auctions.Enum.MappingTypeEnum.EXISTING)
                {
                    if (!CustomerHasBeenImported(lObjMapping.SapPartnerCardCode))
                    {
                        ImportCustomer(lObjMapping.SapPartnerCardCode);
                    }

                    Partner lObjMappedPartner = mObjLocalBusinessPartnerService.GetList().Where(x => x.Code == lObjMapping.SapPartnerCardCode).FirstOrDefault();

                    if(lObjMappedPartner!= null)
                    {
                        UpdateAllDocuments(lObjPartner.Id, lObjMappedPartner.Id);
                        LocalBusinessPartnerService.Remove(lObjPartner.Id);
                    }
                }
            }
        }

        #endregion

        private DateTime GetLastCreationDate()
        {
            return LocalBusinessPartnerService.GetList().Where(b=> !b.Temporary).Count() > 0 ?
                   LocalBusinessPartnerService.GetList().Where(b => !b.Temporary).Max(x => x.CreationDate) : DateTime.Today.AddYears(-10);
        }

        private DateTime GetLastModificationDate()
        {
            return LocalBusinessPartnerService.GetList().Where(b => !b.Temporary).Count() > 0 ?
                   LocalBusinessPartnerService.GetList().Where(b => !b.Temporary).Max(x => x.CreationDate) : DateTime.Today.AddYears(-10);
        }

        private bool CustomerHasBeenImported(string pStrCardCode)
        {
            return LocalBusinessPartnerService.GetList().Where(x => x.Code == pStrCardCode).Count() > 0 ? true : false;
        }

        private bool CustomerHasChanges(CustomerDTO pObjCustomer)
        {
            return LocalBusinessPartnerService.GetList().Where(x => x.Code == pObjCustomer.CardCode && x.ModificationDate != pObjCustomer.UpdateHour).Count() > 0 ? true : false;
        }

        private void ImportCustomer(string pStrCardCode)
        {
            try
            {
                LocalBusinessPartnerService.SaveOrUpdate(GetBusinessPartnerByCode(pStrCardCode));
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateCustomer(string pStrCardCode)
        {
            Partner lObjCurrentPartner = null;
            Partner lObjNewPartner = null;

            try
            {
                lObjCurrentPartner = LocalBusinessPartnerService.GetList().FirstOrDefault(x => x.Code == pStrCardCode);
                lObjNewPartner = GetBusinessPartnerByCode(pStrCardCode);

                lObjCurrentPartner.Name = lObjNewPartner.Name;
                lObjCurrentPartner.ForeignName = lObjNewPartner.ForeignName;
                lObjCurrentPartner.TaxCode = lObjNewPartner.TaxCode;
                lObjCurrentPartner.PartnerStatus = lObjNewPartner.PartnerStatus;
                lObjCurrentPartner.CreationDate = lObjNewPartner.CreationDate;
                lObjCurrentPartner.ModificationDate = lObjNewPartner.ModificationDate;

                LocalBusinessPartnerService.SaveOrUpdate(lObjCurrentPartner);
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void ExportBusinessPartner(Partner pObjPartner)
        {
            SAPbobsCOM.BusinessPartners lObjCustomer = null;

            try
            {
                lObjCustomer = SapBusinessPartnerService.GetBusinessPartnerObject();
                lObjCustomer.CardCode = pObjPartner.Code;
                lObjCustomer.CardName = pObjPartner.Name;
                lObjCustomer.FederalTaxID = pObjPartner.TaxCode;

                if (lObjCustomer.Add() == 0)
                {
                    pObjPartner.Temporary = false;
                    LocalBusinessPartnerService.SaveOrUpdate(pObjPartner);
                }
                else
                {
                    LogUtility.Write(string.Format("[ERROR] {0}", DIApplication.Company.GetLastErrorDescription()));
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjCustomer);
            }
        }

        private Partner GetBusinessPartnerByCode(string pStrCardCode)
        {
            Partner lObjPartner = null;
            CustomerDTO lObjCustomer = null;

            lObjCustomer = SapBusinessPartnerService.GetCustomerByCardCode(pStrCardCode);
            if (lObjCustomer != null)
            {
                lObjPartner = new Partner()
                {
                    Code = lObjCustomer.CardCode,
                    Name = lObjCustomer.CardName,
                    ForeignName = lObjCustomer.CardFName,
                    TaxCode = lObjCustomer.TaxCode,
                    PartnerStatus = lObjCustomer.Valid ? PartnerStatusEnum.ACTIVE : PartnerStatusEnum.INACTIVE,
                    CreationDate = lObjCustomer.CreateDate,
                    ModificationDate = lObjCustomer.UpdateHour > lObjCustomer.CreateDate ? 
                    lObjCustomer.UpdateHour : lObjCustomer.CreateDate,
                    Temporary = false
                };
            }

            return lObjPartner;
        }

        private void UpdateAllDocuments(long pLonCurrentPartnerId, long pLonNewPartnerId)
        {
            try
            {
                //Update batches
                foreach (UGRS.Core.Auctions.Entities.Auctions.Batch lObjBatch in LocalBatchService.GetList().Where(x=> x.SellerId == pLonCurrentPartnerId))
                {
                    lObjBatch.SellerId = pLonNewPartnerId;
                    LocalBatchService.SaveOrUpdate(lObjBatch);
                }
                foreach (UGRS.Core.Auctions.Entities.Auctions.Batch lObjBatch in LocalBatchService.GetList().Where(x => x.BuyerId == pLonCurrentPartnerId))
                {
                    lObjBatch.BuyerId = pLonNewPartnerId;
                    LocalBatchService.SaveOrUpdate(lObjBatch);
                }
                //Update goods receipts
                foreach (UGRS.Core.Auctions.Entities.Inventory.GoodsReceipt lObjGoodsReceipt in LocalGoodsReceiptService.GetList().Where(x => x.CustomerId == pLonCurrentPartnerId))
                {
                    lObjGoodsReceipt.CustomerId = pLonNewPartnerId;
                    LocalGoodsReceiptService.SaveOrUpdate(lObjGoodsReceipt);
                }
                //Update goods issues
                foreach (UGRS.Core.Auctions.Entities.Inventory.GoodsIssue lObjGoodsIssue in LocalGoodsIssueService.GetList())
                {
                    LocalGoodsIssueService.SaveOrUpdate(lObjGoodsIssue);
                }
                //Update goods return
                foreach (UGRS.Core.Auctions.Entities.Inventory.GoodsReturn lObjGoodsReturn in LocalGoodsReturnService.GetList())
                {
                    LocalGoodsReturnService.SaveOrUpdate(lObjGoodsReturn);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }
    }
}
