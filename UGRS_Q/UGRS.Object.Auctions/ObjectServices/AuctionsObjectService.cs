using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Extension.Enum;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Object.Auctions.ObjectServices
{
    public class AuctionsObjectService
    {
        UGRS.Data.Auctions.Factories.AuctionsServicesFactory mObjAuctionsFactory;
        UGRS.Core.SDK.DI.Auctions.AuctionsServicesFactory mObjSAPAuctionFactory;

        public AuctionsObjectService()
        { 
            mObjAuctionsFactory = new AuctionsServicesFactory();
        }

        public void ExportAuctionsAndBatches()
        {
            DateTime lDtmToday = DateTime.Now;
            UGRS.Core.SDK.DI.Auctions.Services.AuctionService lObjSAPAuctionsService = mObjSAPAuctionFactory.GetAuctionService();
            IList<Auction> lLstObjAuctionsList = mObjAuctionsFactory.GetAuctionService().GetList().Where(x => x.ModificationDate == lDtmToday).ToList();

            foreach (Auction lObjAuction in lLstObjAuctionsList)
            {
                //Ask for if not exists the auction in SAP B1 data base
                if(!lObjSAPAuctionsService.HasBeenImported(lObjAuction.Id))
                {
                    lObjSAPAuctionsService.Add(GetSAPAuction(lObjAuction));
                }
                else if (lObjSAPAuctionsService.HasBeenUpdated(lObjAuction.Id, lObjAuction.ModificationDate))
                {
                    lObjSAPAuctionsService.Update(GetSAPAuction(lObjAuction));
                }
            }
        }

        public void ExportBatches(IList<Batch> lObjBatchesList)
        {
            UGRS.Core.SDK.DI.Auctions.Services.AuctionBatchService lObjSAPBatchService = mObjSAPAuctionFactory.GetAuctionBatchService();

            foreach (Batch lObjBacth in lObjBatchesList)
            {
                //Ask for if not exists the batch in SAP B1 data base
                if (!lObjSAPBatchService.HasBeenImported(lObjBacth.Id))
                {
                    lObjSAPBatchService.Add(GetSAPBatch(lObjBacth));
                }
                else if (lObjSAPBatchService.HasBeenUpdated(lObjBacth.Id, lObjBacth.ModificationDate))
                {
                    lObjSAPBatchService.Update(GetSAPBatch(lObjBacth));
                }
            }
        }

        private UGRS.Core.SDK.DI.Auctions.Tables.Auction GetSAPAuction(Auction pObjAuction)
        {
            return new UGRS.Core.SDK.DI.Auctions.Tables.Auction()
            {
                Id = pObjAuction.Id,
                LocationId = pObjAuction.LocationId,
                Location = pObjAuction.Location.Name,
                Folio = pObjAuction.Folio,
                TypeId = pObjAuction.TypeId,
                Type = pObjAuction.Type.Name,
                CategoryId = pObjAuction.CategoryId,
                Category = pObjAuction.Category.Name,
                Commission = pObjAuction.Commission,
                Date = pObjAuction.Date,
                Opened = pObjAuction.Opened,
                Protected = pObjAuction.Protected,
                Removed = pObjAuction.Removed,
                Active = pObjAuction.Active,
                CreationDate = pObjAuction.CreationDate,
                CreationTime = pObjAuction.CreationDate,
                ModificationDate = pObjAuction.ModificationDate,
                ModificationTime = pObjAuction.ModificationDate
            };
        }

        private UGRS.Core.SDK.DI.Auctions.Tables.Batch GetSAPBatch(Batch pObjBatch)
        {
            return new UGRS.Core.SDK.DI.Auctions.Tables.Batch()
            {
                Id = pObjBatch.Id,
                AuctionId = pObjBatch.AuctionId,
                Number = pObjBatch.Number,
                SellerId = pObjBatch.SellerId,
                Seller = pObjBatch.Seller.Name,
                BuyerId = pObjBatch.BuyerId ?? 0, //?? default(long),
                Buyer = pObjBatch.Buyer.Name,
                ItemId = pObjBatch.ItemId,
                Item = pObjBatch.Item.Name,
                ItemTypeId = pObjBatch.ItemTypeId,
                ItemType = pObjBatch.ItemType.Name,
                Quantity = pObjBatch.Quantity,
                Weight = pObjBatch.Weight,
                AverageWeight = pObjBatch.AverageWeight,
                Price = pObjBatch.Price,
                Amount = pObjBatch.Amount,
                Unsold = pObjBatch.Unsold,
                UnsoldMotiveId = (int)pObjBatch.UnsoldMotive,
                UnsoldMotive = pObjBatch.UnsoldMotive.GetDescription(),
                Protected = pObjBatch.Protected,
                Removed = pObjBatch.Removed,
                Active = pObjBatch.Active,
                CreationDate = pObjBatch.CreationDate,
                CreationTime = pObjBatch.CreationDate,
                ModificationDate = pObjBatch.ModificationDate,
                ModificationTime = pObjBatch.ModificationDate
            };
        }
    }
}
