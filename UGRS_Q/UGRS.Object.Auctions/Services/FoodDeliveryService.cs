using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Extension.List;
using System.Data.Entity;

namespace UGRS.Object.Auctions.Services
{
    public class FoodDeliveryService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.DeliveryFoodService mObjSapDeliveryFoodService;
        UGRS.Core.Auctions.Services.Financials.FoodDeliveryService mObjLocalDeliveryFoodService;

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.DeliveryFoodService SapDeliveryFoodService
        {
            get { return mObjSapDeliveryFoodService; }
            set { mObjSapDeliveryFoodService = value; }
        }

        public UGRS.Core.Auctions.Services.Financials.FoodDeliveryService LocalDeliveryFoodService
        {
            get { return mObjLocalDeliveryFoodService; }
            set { mObjLocalDeliveryFoodService = value; }
        }

        #endregion

        #region Contructor

        public FoodDeliveryService()
        {
            SapDeliveryFoodService = new UGRS.Core.SDK.DI.Auctions.Services.DeliveryFoodService();
            LocalDeliveryFoodService = new UGRS.Core.Auctions.Services.Financials.FoodDeliveryService(new BaseDAO<UGRS.Core.Auctions.Entities.Financials.FoodDelivery>());

        }

        #endregion

        #region Methods

        public void ImportFoodDeliveries(string pStrWhsCode)
        {
            IList<int> lLstIntLocalDocEntries = LocalDeliveryFoodService.GetList().Select(x => x.DocEntry).ToList();

            foreach (int lIntDocEntry in SapDeliveryFoodService.GetDeliveriesFoodList(pStrWhsCode).Where(x => !lLstIntLocalDocEntries.Contains(x)))
            {
                ImportFoodDelivery(lIntDocEntry);
            }
        }

        public void UpdateFoodDeliveries(string pStrWhsCode)
        {
            foreach (DeliveryFoodDTO lObjDeliveryFood in SapDeliveryFoodService.GetUpdatedDeliveriesFoodList(pStrWhsCode))
            {
                if (FoodDeliveryHasChanges(lObjDeliveryFood))
                {
                    
                    UpdateFoodDelivery(lObjDeliveryFood.DocEntry);
                }
            }
        }

        private bool FoodDeliveryHasChanges(DeliveryFoodDTO pObjDeliveryFood)
        {
            return LocalDeliveryFoodService.GetList().Where(x => x.DocEntry == pObjDeliveryFood.DocEntry && (x.ModificationDate != pObjDeliveryFood.UpdateDate 
                || x.Opened != pObjDeliveryFood.Opened)).Count() > 0 ? true : false;
        }

        private void ImportFoodDelivery(int pIntDocEntry)
        {
            try
            {
                foreach (FoodDelivery lObjFoodDelivery in GetFoodDeliveries(pIntDocEntry))
                {
                    LocalDeliveryFoodService.SaveOrUpdate(lObjFoodDelivery);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateFoodDelivery(int pIntDocEntry)
        {
            try
            {
                IQueryable<FoodDelivery> lLstObjDeliveries = LocalDeliveryFoodService.GetList();

                foreach (FoodDelivery lObjFoodDelivery in GetFoodDeliveries(pIntDocEntry)
                .Select(x =>
                {
                    x.Id = lLstObjDeliveries.Where(y => y.DocEntry == x.DocEntry && y.LineNum == x.LineNum).Select(y => y.Id).FirstOrDefault();
                    return x;
                }))
                {
                    LocalDeliveryFoodService.SaveOrUpdate(lObjFoodDelivery);
                }
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private IList<FoodDelivery> GetFoodDeliveries(int pIntDocEntry)
        {
            IList<FoodDelivery> lLstObjFoodDeliveries = new List<FoodDelivery>();

            foreach (DeliveryFoodDTO lObjDelivery in SapDeliveryFoodService.GetDeliveriesFood(pIntDocEntry))
            {
                lLstObjFoodDeliveries.Add(new FoodDelivery()
                {
                    DocType = lObjDelivery.DocType,
                    DocNum = lObjDelivery.DocNum,
                    DocEntry = lObjDelivery.DocEntry,
                    CardCode = lObjDelivery.CardCode,
                    LineNum = lObjDelivery.LineNum,
                    WhsCode = lObjDelivery.WhsCode,
                    TaxCode = lObjDelivery.TaxCode,
                    BatchNumber = lObjDelivery.BatchNumber,
                    ItemCode = lObjDelivery.ItemCode,
                    Quantity = lObjDelivery.Quantity,
                    Price = lObjDelivery.Price,
                    CreationDate = lObjDelivery.CreateDate,
                    ModificationDate = lObjDelivery.UpdateDate,
                    Opened = lObjDelivery.Opened
                });
            }

            return lLstObjFoodDeliveries;
        }

        #endregion

    }
}
