using System;
using System.Collections.Generic;
using UGRS.Core.SDK.DI.Auctions.DAO;
using UGRS.Core.SDK.DI.Auctions.DTO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class ItemService
    {
        private ItemDAO mObjItemDAO;

        public ItemService()
        {
            mObjItemDAO = new ItemDAO();
        }

        public IList<string> GetItemCodesList()
        {
            return mObjItemDAO.GetItemCodesList();
        }

        public IList<ItemDTO> GetUpdatedItemsList()
        {
            return mObjItemDAO.GetUpdatedItemsList();
        }

        public ItemDTO GetItemByCode(string pStrItemCode)
        {
            return mObjItemDAO.GetItemByCode(pStrItemCode);
        }

        public SAPbobsCOM.Items GetItemObject()
        {
            return (SAPbobsCOM.Items)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
        }
    }
}
