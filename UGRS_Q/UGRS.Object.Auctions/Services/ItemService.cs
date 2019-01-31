using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Core.SDK.DI.Auctions.DTO;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Object.Auctions.Services
{
    public class ItemService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.ItemService mObjSapItemService;
        UGRS.Core.Auctions.Services.Inventory.ItemService mObjLocalItemService;

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.ItemService SapItemService
        {
            get { return mObjSapItemService; }
            set { mObjSapItemService = value; }
        }

        public UGRS.Core.Auctions.Services.Inventory.ItemService LocalItemService
        {
            get { return mObjLocalItemService; }
            set { mObjLocalItemService = value; }
        }

        #endregion

        #region Contructor

        public ItemService()
        {
            SapItemService = new UGRS.Core.SDK.DI.Auctions.Services.ItemService();
            LocalItemService = new UGRS.Core.Auctions.Services.Inventory.ItemService(new BaseDAO<Item>());
        }

        #endregion

        #region Methods

        public void ImportItems()
        {
            IList<string> lLstStrLocalItems = LocalItemService.GetList().Select(x => x.Code).ToList();

            foreach (string lStrItemCode in SapItemService.GetItemCodesList().Where(x => !lLstStrLocalItems.Contains(x)))
            {
                ImportItem(lStrItemCode);
            }
        }

        public void UpdateItems()
        {
            foreach (ItemDTO lObjItem in SapItemService.GetUpdatedItemsList())
            {
                if (ItemHasChanges(lObjItem))
                {
                    UpdateItem(lObjItem.ItemCode);
                }
            }
        }

        private DateTime GetLastCreationDate()
        {
            return LocalItemService.GetList().Count() > 0 ?
                   LocalItemService.GetList().Max(x => x.CreationDate) : DateTime.Today.AddYears(-10);
        }

        private DateTime GetLastModificationDate()
        {
            return LocalItemService.GetList().Count() > 0 ?
                   LocalItemService.GetList().Max(x => x.ModificationDate) : DateTime.Today.AddYears(-10);
        }

        private bool ItemHasBeenImported(string pStrItemCode)
        {
            return LocalItemService.GetList().Where(x => x.Code == pStrItemCode).Count() > 0 ? true : false;
        }

        private bool ItemHasChanges(ItemDTO pObjItem)
        {
            return LocalItemService.GetList().Where(x => x.Code == pObjItem.ItemCode && x.ModificationDate != pObjItem.UpdateDate).Count() > 0 ? true : false;
        }

        private void ImportItem(string pStrItemCode)
        {
            try
            {
                LocalItemService.SaveOrUpdate(GetItemByCode(pStrItemCode));
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private void UpdateItem(string pStrItemCode)
        {
            Item lObjCurrentItem = null;
            Item lObjNewItem = null;

            try
            {
                lObjCurrentItem = LocalItemService.GetList().FirstOrDefault(x => x.Code == pStrItemCode);
                lObjNewItem = GetItemByCode(pStrItemCode);

                lObjCurrentItem.Name = lObjNewItem.Name;
                lObjCurrentItem.ItemStatus = lObjNewItem.ItemStatus;
                lObjCurrentItem.CreationDate = lObjNewItem.CreationDate;
                lObjCurrentItem.ModificationDate = lObjNewItem.ModificationDate;

                LocalItemService.SaveOrUpdate(lObjCurrentItem);
            }
            catch (Exception lObjException)
            {
                LogUtility.Write(string.Format("[ERROR] {0}", lObjException.ToString()));
            }
        }

        private Item GetItemByCode(string pStrItemCode)
        {
            Item lObjItem = null;
            ItemDTO lObjItemDTO = null;

            lObjItemDTO = SapItemService.GetItemByCode(pStrItemCode);
            if (lObjItemDTO != null)
            {
                lObjItem = new Item()
                {
                    Code = lObjItemDTO.ItemCode,
                    Name = lObjItemDTO.ItemName,
                    ItemStatus = lObjItemDTO.Valid ? ItemStatusEnum.ACTIVE : ItemStatusEnum.INACTIVE,
                    CreationDate = lObjItemDTO.CreateDate,
                    ModificationDate = lObjItemDTO.UpdateDate > lObjItemDTO.CreateDate ?
                    lObjItemDTO.UpdateDate : lObjItemDTO.CreateDate
                };
            }

            return lObjItem;
        }

        #endregion
    }
}
