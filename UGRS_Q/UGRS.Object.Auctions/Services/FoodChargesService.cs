using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;
using UGRS.Core.Extension.List;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Data.Auctions.Factories;
using UGRS.Data.Auctions.Context;

namespace UGRS.Object.Auctions.Services
{
    public class FoodChargesService
    {


        UGRS.Core.Auctions.Services.Financials.FoodChargeCheckService mObjLocalFoodChargeService;
        FinancialsServicesFactory mObjFinancialService;
        UGRS.Data.Auctions.Factories.AuctionsServicesFactory mObjAuctionService;

        //UGRS.Core.Auctions.Services.Financials.FoodDeliveryService mObjLocalDeliveryFoodService;
        //UGRS.Core.Auctions.Services.Inventory.StockService mObjLocalStockService;

        public UGRS.Core.Auctions.Services.Financials.FoodChargeCheckService LocalFoodChargeService
        {
            get { return mObjLocalFoodChargeService; }
            set { mObjLocalFoodChargeService = value; }
        }



        //public UGRS.Core.Auctions.Services.Financials.FoodDeliveryService LocalDeliveryFoodService
        //{
        //    get { return mObjLocalDeliveryFoodService; }
        //    set { mObjLocalDeliveryFoodService = value; }
        //}

        //public UGRS.Core.Auctions.Services.Inventory.StockService LocalStockService
        //{
        //    get { return mObjLocalStockService; }
        //    set { mObjLocalStockService = value; }
        //}

        public FoodChargesService()
        {
            LocalFoodChargeService = new Core.Auctions.Services.Financials.FoodChargeCheckService
                (new BaseDAO<UGRS.Core.Auctions.Entities.Financials.FoodChargeCheck>());
            mObjFinancialService = new FinancialsServicesFactory();
            mObjAuctionService = new Data.Auctions.Factories.AuctionsServicesFactory();
            //LocalDeliveryFoodService = new Core.Auctions.Services.Financials.FoodDeliveryService(new BaseDAO<UGRS.Core.Auctions.Entities.Financials.FoodDelivery>());
            //LocalStockService = new Core.Auctions.Services.Inventory.StockService(new BaseDAO<UGRS.Core.Auctions.Entities.Inventory.Stock>());
        }


        public void SetFoodChargesChecks(DateTime pObjAuctionDate)
        {
            try
            {
                DateTime lObjDate = pObjAuctionDate != DateTime.MinValue ? pObjAuctionDate : DateTime.Now;

                string lStrAlfalfa = "";

                var lLstCurrent = LocalFoodChargeService.GetEntitiesList();


                IQueryable<FoodDelivery> lLstObjFoodDeliveries = mObjFinancialService.GetDeliveryFoodService().GetList();


                IList<FoodChargeCheck> lLstLocalStock = GetChargesPerStock(pObjAuctionDate).Select(x =>
                    {
                        x.Id = lLstCurrent.Where(y => y.BatchNumber == x.BatchNumber && y.BatchDate == x.BatchDate).Select(z => z.Id).FirstOrDefault();
                        x.FoodDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode != lStrAlfalfa).Count() > 0;
                        x.AlfalfaDeliveries = lLstObjFoodDeliveries.Where(y => y.BatchNumber == x.BatchNumber && y.ItemCode == lStrAlfalfa).Count() > 0;
                        x.ApplyFoodCharge = x.FoodCharge && !x.AlfalfaDeliveries ? true :
                            !x.FoodCharge && x.AlfalfaDeliveries ? false :
                            !x.FoodDeliveries && !x.AlfalfaDeliveries ? true : false;
                        return x;
                    })
                    .DistinctBy(y => new { y.BatchNumber, y.BatchDate, y.SellerId })
                    .ToList(); ;

                foreach (var item in lLstLocalStock)
                {
                    LocalFoodChargeService.Save(item);
                }

            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }


        private IList<FoodChargeCheck> GetChargesPerStock(DateTime pDate)
        {

            //List<FoodChargeCheck> lLstLocalStock = new List<FoodChargeCheck>();

            //FoodChargeCheck ll = new FoodChargeCheck()
            //{
            //    ApplyFoodCharge = true,
            //    AlfalfaDeliveries = true,
            //    BatchNumber = "02312313123",
            //    BatchDate = DateTime.Now,
            //    CreationDate = DateTime.Now,
            //    FoodCharge = true,
            //    FoodDeliveries = false,
            //    SellerId = 1,
            //};
            //lLstLocalStock.Add(ll);

            //FoodChargeCheck l3l = new FoodChargeCheck()
            //{
            //    ApplyFoodCharge = true,
            //    AlfalfaDeliveries = true,
            //    BatchNumber = "02312asdfa313123",
            //    BatchDate = DateTime.Now,
            //    CreationDate = DateTime.Now,
            //    FoodCharge = true,
            //    FoodDeliveries = false,
            //    SellerId = 1,
            //};

            //lLstLocalStock.Add(l3l);

            //return lLstLocalStock;



            return mObjAuctionService.GetAuctionStockService().GetStockList()
                     .Where(x =>
                         x.Quantity > 0 &&
                         x.Payment &&
                         x.ExpirationDate.ToString("yyyy-MM-dd") == pDate.ToString("yyyy-MM-dd"))
                         .AsEnumerable()
                         .Select(x => new FoodChargeCheck()
                         {
                             BatchNumber = x.BatchNumber,
                             BatchDate = x.CreationDate,
                             SellerId = x.CustomerId,
                             Seller = x.Customer,
                             FoodCharge = x.ChargeFood
                         }).ToList();
        }

    }
}
