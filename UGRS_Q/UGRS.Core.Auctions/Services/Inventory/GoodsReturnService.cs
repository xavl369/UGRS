using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class GoodsReturnService
    {
        private IBaseDAO<GoodsReturn> mObjGoodsReturnDAO;
        private IBaseDAO<Batch> mObjBatchDAO;

        public GoodsReturnService(IBaseDAO<GoodsReturn> pObjGoodsReturnDAO, IBaseDAO<Batch> pObjBatchDAO)
        {
            mObjGoodsReturnDAO = pObjGoodsReturnDAO;
            mObjBatchDAO = pObjBatchDAO;
        }

        public IQueryable<GoodsReturn> GetList()
        {
            return mObjGoodsReturnDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(GoodsReturn pObjGoodsReturn)
        {
            mObjGoodsReturnDAO.SaveOrUpdateEntity(pObjGoodsReturn);
        }

        public void Remove(long pLonId)
        {
            mObjGoodsReturnDAO.RemoveEntity(pLonId);
        }

        //public void CreateGoodsReturn(IList<DetailedBatchDTO> pLstObjDetailedBatches)
        //{
        //    foreach (var lObjDetailedBatch in pLstObjDetailedBatches)
        //    {
        //        CreateGoodsReturn(lObjDetailedBatch);
        //    }
        //}

        //public void CreateGoodsReturn(DetailedBatchDTO pObjDetailedBatch)
        //{
        //        //Valid availability
        //        if (pObjDetailedBatch.QuantityToPick > 0 && GetAvailableQuantity(pObjDetailedBatch) >= pObjDetailedBatch.QuantityToPick)
        //        {
        //            //Get item batches from lines
        //            foreach (var lObjLine in pObjDetailedBatch.Lines.Where(x => GetAvailableQuantity(pObjDetailedBatch, x) > 0))
        //            {
        //                if (pObjDetailedBatch.QuantityToPick > 0 && GetAvailableQuantity(pObjDetailedBatch) >= pObjDetailedBatch.QuantityToPick)
        //                {
        //                    //Get quantity to apply
        //                    int lIntQuantityToApply = pObjDetailedBatch.QuantityToPick > GetAvailableQuantity(pObjDetailedBatch, lObjLine) ? GetAvailableQuantity(pObjDetailedBatch, lObjLine) : pObjDetailedBatch.QuantityToPick;

        //                    //Create Goods Return
        //                    GoodsReturn lObjGoodsReturn = new GoodsReturn()
        //                    {
        //                        BatchId = pObjDetailedBatch.Id,
        //                        Quantity = lIntQuantityToApply,
        //                        //BatchNumber = lObjLine.BatchNumber,
        //                        //BatchDate = lObjLine.BatchDate,
        //                        //ItemId = lObjLine.ItemId,
        //                        Delivered = pObjDetailedBatch.Delivered,
        //                        Exported = false
        //                    };

        //                    //Add Goods Return
        //                    mObjGoodsReturnDAO.AddEntity(lObjGoodsReturn);

        //                    //Update quantities
        //                    pObjDetailedBatch.ReturnedQuantity += lIntQuantityToApply;
        //                    lObjLine.ReturnedQuantity += lIntQuantityToApply;
        //                    pObjDetailedBatch.QuantityToPick -= lIntQuantityToApply;

        //                    if (pObjDetailedBatch.Delivered)
        //                    {
        //                        pObjDetailedBatch.AvailableQuantityToReturnDelivery -= lIntQuantityToApply;
        //                        lObjLine.AvailableQuantityToReturnDelivery -= lIntQuantityToApply;
        //                    }
        //                    else
        //                    {
        //                        pObjDetailedBatch.AvailableQuantityToReturn -= lIntQuantityToApply;
        //                        lObjLine.AvailableQuantityToReturn -= lIntQuantityToApply;
        //                    }
        //                }
        //            }

        //            //Update Batch
        //            mObjBatchDAO.SaveOrUpdateEntity(mObjBatchDAO.GetEntity(pObjDetailedBatch.Id));
        //        }
        //    }

        public void CreateGoodsReturn(DetailedBatchDTO pObjDetailedBatch, int pIntQttyToReturn, int pIntDeliveredQtty)
        {
            //Valid availability
            if (pObjDetailedBatch.QuantityToPick > 0 && GetAvailableQuantity(pObjDetailedBatch, pIntQttyToReturn, pIntDeliveredQtty) >= pObjDetailedBatch.QuantityToPick)
            {
                //Get quantity to apply
                int lIntQuantityToApply = pObjDetailedBatch.QuantityToPick > GetAvailableQuantity(pObjDetailedBatch, pIntQttyToReturn, pIntDeliveredQtty) ? GetAvailableQuantity(pObjDetailedBatch, pIntQttyToReturn, pIntDeliveredQtty)
                            : pObjDetailedBatch.QuantityToPick;

                //Create Goods Return
                GoodsReturn lObjGoodsReturn = new GoodsReturn()
                {
                    BatchId = pObjDetailedBatch.Id,
                    Quantity = lIntQuantityToApply,
                    Weight = pObjDetailedBatch.Weight,
                    Delivered = pObjDetailedBatch.Delivered,
                    Exported = false,
                    ReturnMotive = pObjDetailedBatch.ReturnMotive
                };

                //Add Goods Return
                mObjGoodsReturnDAO.AddEntity(lObjGoodsReturn);

                //Update quantities
                pObjDetailedBatch.ReturnedQuantity += lIntQuantityToApply;
                //lObjLine.ReturnedQuantity += lIntQuantityToApply;
                pObjDetailedBatch.QuantityToPick -= lIntQuantityToApply;

                if (pObjDetailedBatch.Delivered)
                {
                    pObjDetailedBatch.AvailableQuantityToReturnDelivery -= lIntQuantityToApply;
                    //lObjLine.AvailableQuantityToReturnDelivery -= lIntQuantityToApply;
                }
                else
                {
                    pObjDetailedBatch.AvailableQuantityToReturn -= lIntQuantityToApply;
                    //lObjLine.AvailableQuantityToReturn -= lIntQuantityToApply;
                }



                ////Update Batch
                mObjBatchDAO.SaveOrUpdateEntity(mObjBatchDAO.GetEntity(pObjDetailedBatch.Id));
            }
        }


        private int GetAvailableQuantity(DetailedBatchDTO pObjBatch, int pintQttyToRet, int pIntDeliveredQtty)
        {
            return pObjBatch.Delivered ? pIntDeliveredQtty : pintQttyToRet;
        }

    }
}
