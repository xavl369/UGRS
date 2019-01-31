using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Object.Auctions.Services;

namespace UGRS.Object.Auctions
{
    public class AuctionsServerObject : MarshalByRefObject
    {
        #region Constructor

        public AuctionsServerObject()
        {
            DIApplication.DIConnect();
        }

        #endregion

        #region Methods

        #region Business Partners

        public void ImportCustomers()
        {
            new BusinessPartnerService().ImportCustomers();
        }

        public void UpdateCustomers()
        {
            new BusinessPartnerService().UpdateCustomers();
        }

        public IList<CustomerDTO> SearchBusinessPartner(string pStrFilter)
        {
            return new BusinessPartnerService().SearchBusinessPartner(pStrFilter);
        }

        //To implement on Auctions
        public void BusinessPartnerMapping(string pStrMappingList)
        {
            //Deserialize
            IList<CustomerMappingDTO> lLstObjMapping = pStrMappingList.JsonDeserialize<IList<CustomerMappingDTO>>();

            //Save mapping
            new BusinessPartnerService().BusinessPartnerMapping(lLstObjMapping);
        }

        #endregion

        #region Inventory

        public void ImportItems()
        {
            new ItemService().ImportItems();
        }

        public void UpdateItems()
        {
            new ItemService().UpdateItems();
        }

        public void ImportStocks(string pStrWhsCode, DateTime pAuctionDate)
        {
            new StockService().ImportStocks(pStrWhsCode,pAuctionDate);
        }

        public void UpdateStocks(string pStrWhsCode)
        {
            new StockService().UpdateStocks(pStrWhsCode);
        }

        #endregion

        #region Auctions

        public void InitializeTablesAndFields()
        {
            new SetupService().InitializeTablesAndFields();
        }

        public void ExportAuctions(string pStrLocation)
        {
            new AuctionService().ExportAuctions(pStrLocation);
        }

        public void UpdateAuctions(string pStrLocation)
        {
            new AuctionService().UpdateAuctions(pStrLocation);
        }

        public void ExportBatches(DateTime pDtmAuctionDate)
        {
            new BatchService().ExportBatches(pDtmAuctionDate);
        }

        public void UpdateBatches(DateTime pDtmAuctionDate)
        {
            new BatchService().UpdateBatches(pDtmAuctionDate);
        }

        public void ExportBatchLines(string pStrLocation)
        {
            new BatchLineService().ExportBatchLines(pStrLocation);
        }

        public void UpdateBatchesLine(string pStrLocation)
        {
            new BatchLineService().UpdateBatchLines(pStrLocation);
        }

        #endregion

        #region Financials
        
        //To implement on Auctions
        public int CreateJournalEntry(string pStrFolio, string pStrSellerCardCode, string pStrBuyerCardCode, double pDblAmount)
        {
             int lIntJournalEntry = new FinancialsService().CreateJournalEntry(pStrFolio, pStrSellerCardCode, pStrBuyerCardCode, pDblAmount);
             return lIntJournalEntry;

        }

        public string GetDeliveriesFood(string pStrWhsCode, string pStrCardCode)
        {
            return new FinancialsService().GetDeliveriesFood(pStrWhsCode, pStrCardCode);
        }

        public string GetPrice(string pStrWhsCode, string pStrItemCode)
        {
            return new FinancialsService().GetPrice(pStrWhsCode, pStrItemCode);
        }

        #endregion

        #region SAP B1

        public int GetLastSapErrorCode()
        {
            return DIApplication.Connected && DIApplication.Company != null ? DIApplication.Company.GetLastErrorCode() : 0;
        }

        public string GetLastSapErrorDescription()
        {
            return DIApplication.Connected && DIApplication.Company != null ? DIApplication.Company.GetLastErrorDescription() : string.Empty;
        }

        #endregion

        #endregion

        #region Other

        public override object InitializeLifetimeService()
        {
            return null;
        }

        ~AuctionsServerObject()
        {
            RemotingServices.Disconnect(this);
        }

        #endregion
    }
}
