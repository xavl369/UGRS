using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Core.Auctions.Services.Auctions
{
    public class BatchLineService
    {
        private IBaseDAO<BatchLine> mObjBatchLineDAO;

        public BatchLineService(IBaseDAO<BatchLine> pObjBatchLineDAO)
        {
            mObjBatchLineDAO = pObjBatchLineDAO;
        }

        public IQueryable<BatchLine> GetList()
        {
            return mObjBatchLineDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(BatchLine pObjBatchLine)
        {
            if (!Exists(pObjBatchLine))
            {
                mObjBatchLineDAO.SaveOrUpdateEntity(pObjBatchLine);
            }
            else
            {
                throw new Exception("La linea de lote ingresada ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjBatchLineDAO.RemoveEntity(pLonId);
        }

        private bool Exists(BatchLine pObjBatchLine)
        {
            return mObjBatchLineDAO.GetEntitiesList().Where(x => x.BatchId == pObjBatchLine.BatchId &&
                                                                 x.ItemId == pObjBatchLine.ItemId &&
                                                                 x.Quantity == pObjBatchLine.Quantity &&
                                                                 x.Id != pObjBatchLine.Id).Count() > 0 ? true : false;
        }
    }
}
