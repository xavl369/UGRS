using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.DAO;
using UGRS.Core.SDK.DI.Auctions.DTO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class ItemBatchService
    {
        private ItemBatchDAO mObjItemBatchDAO;

        public ItemBatchService()
        {
            mObjItemBatchDAO = new ItemBatchDAO();
        }

        public IList<ItemBatchDTO> GetItemBatchesListByWarehouse(string pStrWhsCode, DateTime pAuctionDate)
        {
            return mObjItemBatchDAO.GetItemBatchesListByWarehouse(pStrWhsCode, pAuctionDate);
        }

        public IList<ItemBatchDTO> GetUpdatedItemBatchesListByWarehouse(string pStrWhsCode)
        {
            return mObjItemBatchDAO.GetUpdatedItemBatchesListByWarehouse(pStrWhsCode);
        }

        public IList<ItemBatchDTO> GetItemBatchesListByFilters(string pStrCardCode, string pStrItemCode, string pStrWhsCode)
        {
            return mObjItemBatchDAO.GetItemBatchesListByFilters(pStrCardCode, pStrItemCode, pStrWhsCode);
        }

        public ItemBatchDTO GetItemBatchByFilters(string pStrWarehouse, string pStrItemCode, string pStrBatchNumber)
        {
            return mObjItemBatchDAO.GetItemBacthByFilters(pStrWarehouse, pStrItemCode, pStrBatchNumber);
        }
    }
}
