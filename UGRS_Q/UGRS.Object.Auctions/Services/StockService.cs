using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Extension.List;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Object.Auctions.Services
{
    public class StockService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.ItemBatchService mObjSapStockService;
        UGRS.Core.Auctions.Services.Inventory.StockService mObjLocalStockService;
        UGRS.Core.Auctions.Services.Inventory.ItemService mObjLocalItemService;
        UGRS.Core.Auctions.Services.Business.PartnerService mObjLocalBusinessPartnerService;


        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.ItemBatchService SapStockService
        {
            get { return mObjSapStockService; }
            set { mObjSapStockService = value; }
        }

        public UGRS.Core.Auctions.Services.Inventory.StockService LocalStockService
        {
            get { return mObjLocalStockService; }
            set { mObjLocalStockService = value; }
        }

        public UGRS.Core.Auctions.Services.Inventory.ItemService LocalItemService
        {
            get { return mObjLocalItemService; }
            set { mObjLocalItemService = value; }
        }

        public UGRS.Core.Auctions.Services.Business.PartnerService LocalBusinessPartnerService
        {
            get { return mObjLocalBusinessPartnerService; }
            set { mObjLocalBusinessPartnerService = value; }
        }


        #endregion

        #region Contructor

        public StockService()
        {
            SapStockService = new UGRS.Core.SDK.DI.Auctions.Services.ItemBatchService();
            LocalStockService = new UGRS.Core.Auctions.Services.Inventory.StockService(new BaseDAO<Stock>());
            LocalItemService = new UGRS.Core.Auctions.Services.Inventory.ItemService(new BaseDAO<Item>());
            LocalBusinessPartnerService = new UGRS.Core.Auctions.Services.Business.PartnerService(new BaseDAO<Partner>(), new BaseDAO<PartnerMapping>());
        }

        #endregion

        #region Methods

        public void ImportStocks(string pStrWhsCode, DateTime pAuctionDate)
        {
            foreach (ItemBatchDTO lObjStock in GetStockToImport(pStrWhsCode,pAuctionDate))
            {
                ImportStock(pStrWhsCode, lObjStock.ItemCode, lObjStock.BatchNumber);
            }
        }

        public void UpdateStocks(string pStrWhsCode)
        {
            foreach (ItemBatchDTO lObjStock in SapStockService.GetUpdatedItemBatchesListByWarehouse(pStrWhsCode))
            {     
                if (StockHasChanges(lObjStock))
                {
                    UpdateStock(pStrWhsCode, lObjStock);
                }
            }
        }

        private DateTime GetLastCreationDate()
        {
            #if DEBUG

            var a = LocalStockService.GetList().Count() > 0;

            #endif

            return LocalStockService.GetListByWhs().Count() > 0 ?
                   LocalStockService.GetListByWhs().Max(x => x.CreationDate) : DateTime.Today.AddYears(-10);
        }

        private DateTime GetLastModificationDate()
        {
            return LocalStockService.GetListByWhs().Count() > 0 ?
                   LocalStockService.GetListByWhs().Max(x => x.ModificationDate) : DateTime.Today.AddYears(-10);
        }

        private bool StockHasBeenImported(ItemBatchDTO pObjStock)
        {
            return LocalStockService.GetListByWhs().Where(x => x.Item.Code == pObjStock.ItemCode && x.BatchNumber == pObjStock.BatchNumber).Count() > 0 ? true : false;
        }

        private bool StockHasChanges(ItemBatchDTO pObjStock)
        {
            string lStrLocalStockDate = LocalStockService.GetListByWhs().Where(x => x.Item.Code == pObjStock.ItemCode && x.BatchNumber == pObjStock.BatchNumber).Select(x => x.ModificationDate).FirstOrDefault().ToString("yyyy-MM-dd HH:mm:ss");

            string lStrSAPStockDate = pObjStock.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss");

            return lStrLocalStockDate != lStrSAPStockDate ? true : false;
        }

        private void ImportStock(string pStrWarehouse, string pStrItemCode, string pStrBatchNumber)
        {
            try
            {
                LocalStockService.SaveOrUpdate(GetStockByFilters(pStrWarehouse, pStrItemCode, pStrBatchNumber));
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateStock(string pStrWarehouse, ItemBatchDTO pObjItemBatch)
        {
            Stock lObjCurrentStock = null;
            Stock lObjNewStock = null;

            try
            {

                lObjCurrentStock = LocalStockService.GetListByWhs().FirstOrDefault(x => x.Item.Code == pObjItemBatch.ItemCode
                    && x.BatchNumber == pObjItemBatch.BatchNumber);
                if (lObjCurrentStock != null)
                {

                lObjNewStock = GetStockByFilters(pStrWarehouse, pObjItemBatch.ItemCode, pObjItemBatch.BatchNumber);

                    lObjCurrentStock.Quantity = lObjNewStock.Quantity;
                    lObjCurrentStock.CreationDate = lObjNewStock.CreationDate;
                    lObjCurrentStock.ModificationDate = lObjNewStock.ModificationDate;
                    lObjCurrentStock.ExpirationDate = lObjNewStock.ExpirationDate;
                    lObjCurrentStock.CustomerId = lObjNewStock.CustomerId;

                    LocalStockService.SaveOrUpdate(lObjCurrentStock);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private Stock GetStockByFilters(string pStrWarehouse, string pStrItemCode, string pStrBatchNumber)
        {
            Stock lObjStock = null;
            ItemBatchDTO lObjItemBatch = null;

            lObjItemBatch = SapStockService.GetItemBatchByFilters(pStrWarehouse, pStrItemCode, pStrBatchNumber);
            if (lObjItemBatch != null)
            {
                lObjStock = new Stock()
                {
                    ItemId = GetItemIdByCode(lObjItemBatch.ItemCode),
                    CustomerId = GetCustomerIdByCode(lObjItemBatch.CardCode),
                    InitialWarehouse = lObjItemBatch.InitialWarehouse,
                    CurrentWarehouse = lObjItemBatch.CurrentWarehouse,
                    ChargeFood = lObjItemBatch.ChargeFood,
                    BatchNumber = lObjItemBatch.BatchNumber,
                    Quantity = lObjItemBatch.Quantity,
                    CreationDate = lObjItemBatch.CreateDate,
                    ModificationDate = lObjItemBatch.UpdateDate > lObjItemBatch.CreateDate?
                    lObjItemBatch.UpdateDate : lObjItemBatch.CreateDate,
                    ExpirationDate = lObjItemBatch.ExpirationDate,
                    Payment = lObjItemBatch.Payment,
                    EntryFolio = lObjItemBatch.Folio
                };
            }

            return lObjStock;
        }

        private long GetItemIdByCode(string pStrItemCode)
        {
            return LocalItemService.GetList().Where(x => x.Code == pStrItemCode).Select(y => y.Id).FirstOrDefault();
        }

        private long GetCustomerIdByCode(string pStrCustomerCode)
        {
            return LocalBusinessPartnerService.GetList().Where(x => x.Code == pStrCustomerCode).Select(y => y.Id).FirstOrDefault();
        }

        private IList<ItemBatchDTO> GetStockToImport(string pStrWhsCode,DateTime pAuctionDate)
        {
            //Get Stock from SAP B1
            IList<ItemBatchDTO> lLstObjSapStock = SapStockService.GetItemBatchesListByWarehouse(pStrWhsCode,pAuctionDate);

            //Get Local Stock
            IList<string> lLstStrLocalStock = LocalStockService.GetListByWhs()
                .Select(x => x.BatchNumber)
                .ToList();

            //Get imported customers for valid that customer exists before then import his stock
            IList<string> lLstStrImportedCustomer = LocalBusinessPartnerService
                .GetList()
                .Select(x => x.Code)
                .ToList();

            //Get imported items for valid that item exists before then import the customer's stock
            IList<string> lLstStrImportedItems = LocalItemService
                .GetList()
                .Select(x => x.Code)
                .ToList();

            return lLstObjSapStock.Where(x => !lLstStrLocalStock.Contains(x.BatchNumber) && lLstStrImportedCustomer.Contains(x.CardCode) && lLstStrImportedItems.Contains(x.ItemCode)).ToList();
        }


        //public void SetFoodChargesChecks(DateTime pObjAuctionDate)
        //{
        //    try
        //    {
        //        DateTime lObjDate = pObjAuctionDate != DateTime.MinValue ? pObjAuctionDate : DateTime.Now;

        //        string lStrAlfalfa = "";

        //        var lLstCurrent = mObjFinancialService.GetFoodChargeCheckService().GetEntitiesList();


        //        IQueryable<FoodDelivery> lLstObjFoodDeliveries = mObjFinancialService.GetDeliveryFoodService().GetList();


        //        IList<FoodChargeCheck> lLstLocalStock = GetChargesPerStock(pObjAuctionDate).Select(x =>
        //        {
        //            x.Id = lLstCurrent.Where(y => y.BatchNumber == x.BatchNumber && y.BatchDate == x.BatchDate).Select(z => z.Id).FirstOrDefault();
        //            x.FoodDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode != lStrAlfalfa).Count() > 0;
        //            x.AlfalfaDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode == lStrAlfalfa).Count() > 0;
        //            x.ApplyFoodCharge = x.FoodCharge && !x.AlfalfaDeliveries ? true :
        //                !x.FoodCharge && x.AlfalfaDeliveries ? false :
        //                !x.FoodDeliveries && !x.AlfalfaDeliveries ? true : false;
        //            return x;
        //        })
        //            .DistinctBy(y => new { y.BatchNumber, y.BatchDate, y.SellerId })
        //            .ToList(); ;

        //        mObjFinancialService.GetFoodChargeCheckService().SaveOrUpdateList(lLstLocalStock);
        //    }
        //    catch (Exception lObjException)
        //    {
        //        LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
        //    }
        //}


        //private IList<FoodChargeCheck> GetChargesPerStock(DateTime pDate)
        //{

        //    return LocalStockService.GetList()
        //             .Where(x =>
        //                 x.Quantity > 0 &&
        //                 x.Payment &&
        //                 DbFunctions.TruncateTime(x.ExpirationDate) == DbFunctions.TruncateTime(pDate))
        //                 .AsEnumerable()
        //                 .Select(x => new FoodChargeCheck()
        //                 {
        //                     BatchNumber = x.BatchNumber,
        //                     BatchDate = x.CreationDate,
        //                     SellerId = x.CustomerId,
        //                     Seller = x.Customer,
        //                     FoodCharge = x.ChargeFood
        //                 }).ToList();
        //}


        #endregion
    }
}
