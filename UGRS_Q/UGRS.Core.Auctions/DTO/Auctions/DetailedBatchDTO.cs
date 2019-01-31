using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Enums.Auctions;

namespace UGRS.Core.Auctions.DTO.Auctions
{
    public class DetailedBatchDTO : INotifyPropertyChanged
    {
        #region Attributes

        private int mIntQuantityToPick;
        private bool mBolDelivered;
        private float mFltWeight;

        #endregion

        #region Properties

        public long Id { get; set; }
        public int Number { get; set; }

        public long AuctionId { get; set; }
        public string AuctionFolio { get; set; }

        public long ItemTypeId { get; set; }
        public string ItemTypeCode { get; set; }
        public string ItemTypeName { get; set; }

        public long SellerId { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }

        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }

        public int TotalQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int ReturnedQuantity { get; set; }

        public float TotalWeight { get; set; }
        public float DeliveredWeight { get; set; }
        public float ReturnedWeight { get; set; }

        public int AvailableQuantityToDelivery { get; set; }
        public int AvailableQuantityToReturn { get; set; }
        public int AvailableQuantityToReturnDelivery { get; set; }

        public float AvailableWeightToDelivery { get; set; }
        public float AvailableWeightToReturn { get; set; }
        public float AvailableWeightToReturnDelivery { get; set; }
 
        public UnsoldMotiveEnum ReturnMotive {get;set;}

        //public IList<DetailedBatchLineDTO> Lines { get; set; }

        public int QuantityToPick
        {
            get
            {
                return mIntQuantityToPick;
            }
            set
            {
                mIntQuantityToPick = value;
                OnPropertyChanged("QuantityToPick");
            }
        }

        public bool Delivered 
        { 
            get
            {
                return mBolDelivered;
            }
            set
            {
                mBolDelivered = value;
                OnPropertyChanged("Delivered");
            }
        }

        public float Weight
        {
            get
            {
                return mFltWeight;
            }
            set
            {
                mFltWeight = value;
                OnPropertyChanged("Weight");
            }
        }

        #endregion

        #region Constructor

        public DetailedBatchDTO()
        {

        }

        public DetailedBatchDTO(Batch pObjBatch)
        {
            SetBatchValues(pObjBatch);
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods
        
        private void SetBatchValues(Batch pObjBatch)
        {
            Id = pObjBatch.Id;
            Number = pObjBatch.Number;

            AuctionId = pObjBatch.AuctionId;
            AuctionFolio = pObjBatch.Auction != null ? pObjBatch.Auction.Folio : string.Empty;

            ItemTypeId = pObjBatch.ItemTypeId ?? 0;
            ItemTypeCode = pObjBatch.ItemType != null ? pObjBatch.ItemType.Code : string.Empty;
            ItemTypeName = pObjBatch.ItemType != null ? pObjBatch.ItemType.Name : string.Empty;

            SellerId = pObjBatch.SellerId ?? 0;
            SellerCode = pObjBatch.Seller != null ? pObjBatch.Seller.Code : string.Empty;
            SellerName = pObjBatch.Seller != null ? pObjBatch.Seller.Name : string.Empty;

            BuyerId = pObjBatch.BuyerId ?? 0;
            BuyerCode = pObjBatch.Buyer != null ? pObjBatch.Buyer.Code : string.Empty;
            BuyerName = pObjBatch.Buyer != null ? pObjBatch.Buyer.Name : string.Empty;

            // Quantities
            TotalQuantity = pObjBatch.Quantity;
            DeliveredQuantity = GetDeliveredQuantity(pObjBatch) - GetReturnedQuantity(pObjBatch, true);
            ReturnedQuantity = GetReturnedQuantity(pObjBatch);

            // Weights
            TotalWeight = pObjBatch.Weight;
            DeliveredWeight = GetDeliveredWeight(pObjBatch);
            ReturnedWeight = GetReturnedWeight(pObjBatch);

                                // (+) Total quantity
            AvailableQuantityToDelivery = pObjBatch.Quantity

                                // (-) Delivered quantity             
                                - DeliveredQuantity

                                // (-) Returned quantity
                                - ReturnedQuantity;

            AvailableQuantityToReturn = AvailableQuantityToDelivery;
            AvailableQuantityToReturnDelivery = DeliveredQuantity;

            QuantityToPick = 0;

            //Lines = GetLines(pObjBatch);
        }

        //private IList<DetailedBatchLineDTO> GetLines(Batch pObjBatch)
        //{
        //    return pObjBatch.Lines != null ? 
        //           pObjBatch.Lines.AsEnumerable().Select(x => new DetailedBatchLineDTO()
        //           {
        //               BatchId = x.BatchId,
        //               ItemId = x.ItemId,
        //               ItemCode = x.Item != null ? x.Item.Code : string.Empty,
        //               ItemName = x.Item != null ? x.Item.Name : string.Empty,
        //               BatchNumber = x.BatchNumber,
        //               BatchDate = x.BatchDate,

        //               // Quantities
        //               TotalQuantity = x.Quantity,
        //               DeliveredQuantity = (GetDeliveredQuantity(pObjBatch) - GetReturnedQuantity(pObjBatch, true)),
        //               ReturnedQuantity = GetReturnedQuantity(pObjBatch),

        //               // Weights
        //               TotalWeight = x.

        //                                   // (+) Total quantity
        //               AvailableQuantityToDelivery = x.Quantity

        //                                   // (-) Delivered quantity
        //                                   - DeliveredQuantity

        //                                   // (-) Returned quantity
        //                                   - ReturnedQuantity,

        //               AvailableQuantityToReturn = AvailableQuantityToDelivery,
        //               AvailableQuantityToReturnDelivery = DeliveredQuantity

        //           }).ToList() : new List<DetailedBatchLineDTO>();
        //}

        private int GetDeliveredQuantity(Batch pObjBatch)
        {
            return pObjBatch.GoodsIssues
                    .Where(x => !x.Removed)
                    .Sum(y => y.Quantity);
        }

        private int GetReturnedQuantity(Batch pObjBatch)
        {
            return pObjBatch.GoodsReturns
                    .Where(x => !x.Removed)
                    .Sum(y => y.Quantity);
        }

        private int GetReturnedQuantity(Batch pObjBatch, bool pBolDelivered)
        {
            return pObjBatch.GoodsReturns
                    .Where(x => !x.Removed 
                        && x.Delivered == pBolDelivered)
                    .Sum(y => y.Quantity);
        }

        private float GetDeliveredWeight(Batch pObjBatch)
        {
            // Delivery quantity * current average weight
            return (float)(GetDeliveredQuantity(pObjBatch) - GetReturnedQuantity(pObjBatch, true)) * GetCurrentAverageWeight(pObjBatch);
        }

        private float GetReturnedWeight(Batch pObjBatch)
        {
            return pObjBatch.GoodsReturns
                    .Where(x => !x.Removed)
                    .Sum(y => (float?)y.Weight) ?? 0;
        }

        private float GetReturnedWeight(Batch pObjBatch, bool pBolDelivered)
        {
            return pObjBatch.GoodsReturns
                    .Where(x => !x.Removed
                        && x.Delivered == pBolDelivered)
                    .Sum(y => (float?)y.Weight) ?? 0;
        }

        private float GetCurrentAverageWeight(Batch pObjBatch)
        {
            // (Weight - ReturnedWeight) / (Quantity - ReturnedQuantity)
            return (pObjBatch.Weight - pObjBatch.GoodsReturns.Where(x => !x.Removed).Select(x => (float?)x.Weight).Sum() ?? 0) / 
                   (pObjBatch.Quantity - GetReturnedQuantity(pObjBatch));
        }

        #endregion
    }
}
