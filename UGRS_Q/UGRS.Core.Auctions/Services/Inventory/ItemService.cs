using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class ItemService
    {
        private IBaseDAO<Item> mObjItemDAO;

        public ItemService(IBaseDAO<Item> pObjItemDAO)
        {
            mObjItemDAO = pObjItemDAO;
        }

        public Item Get(long pLonId)
        {
            return mObjItemDAO.GetEntity(pLonId);
        }

        public IQueryable<Item> GetList()
        {
            return mObjItemDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(Item pObjItem)
        {
            if (!Exists(pObjItem))
            {
                mObjItemDAO.SaveOrUpdateEntity(pObjItem);
            }
            else
            {
                throw new Exception("El artículo ingresado ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjItemDAO.RemoveEntity(pLonId);
        }

        /// <summary>
        /// Buscar los articulos
        /// </summary>
        public List<Item> SearchItem(string pStrItem, FilterEnum pEnmFilter)
        {
            IList<IQueryable<Item>> lLstObjQueries = new List<IQueryable<Item>>();
            IQueryable<Item> lLstObjItems = this.GetList().Where(x =>
            (
                pEnmFilter == FilterEnum.ACTIVE ? x.Active && x.ItemStatus == ItemStatusEnum.ACTIVE :
                pEnmFilter == FilterEnum.INACTIVE ? !x.Active && x.ItemStatus == ItemStatusEnum.INACTIVE : true
            ));

            lLstObjQueries.Add(lLstObjItems.Where(x => x.Code.ToUpper().Contains(pStrItem.ToUpper())));
            lLstObjQueries.Add(lLstObjItems.Where(x => x.Name.ToUpper().Contains(pStrItem.ToUpper())));
            lLstObjQueries.Add(lLstObjItems.Where(x => x.Code.ToUpper().Equals(pStrItem.ToUpper())));
            lLstObjQueries.Add(lLstObjItems.Where(x => x.Name.ToUpper().Equals(pStrItem.ToUpper())));

            IQueryable<Item> lLstObjBestQuery = lLstObjItems;
            int lIntBestRowCount = lLstObjItems.Count();

            for (int i = 0; i < lLstObjQueries.Count; i++)
            {
                int lIntCurrentRowCount = lLstObjQueries[i].Count();
                if (lIntCurrentRowCount > 0 && lIntCurrentRowCount < lIntBestRowCount)
                {
                    lLstObjBestQuery = lLstObjQueries[i];
                    lIntBestRowCount = lIntCurrentRowCount;
                }
            }

            return lLstObjBestQuery.ToList();    
        }

        private bool Exists(Item pObjItem)
        {
            return mObjItemDAO.GetEntitiesList().Where(x => x.Code == pObjItem.Code && x.Name == pObjItem.Name && x.Id != pObjItem.Id).Count() > 0 ? true : false;
        }

        public Item GetNextLevel(long pLonId)
        {
            int lIntLvl = mObjItemDAO.GetEntitiesList().Where(x => x.Id == pLonId).Select(x => x.Level).FirstOrDefault();

            int lIntNxtlvl = mObjItemDAO.GetEntitiesList().Where(x => (lIntLvl + 1) == x.Level).Count() > 0 ?
               mObjItemDAO.GetEntitiesList().Where(x => (lIntLvl + 1) == x.Level).Select(x => x.Level).FirstOrDefault() :
               mObjItemDAO.GetEntitiesList().Select(x=>x.Level).FirstOrDefault();

            return mObjItemDAO.GetEntitiesList().Where(x => x.Level == lIntNxtlvl
                ).FirstOrDefault();
        }

        public bool SameGender(ItemTypeGenderEnum pItemTypeGenderEnum, long lLonItemId)
        {
           return mObjItemDAO.GetEntitiesList().Where(x => x.Id == lLonItemId && x.Gender == pItemTypeGenderEnum).Select(x => x.Gender).Count() > 0 ? true : false;
        }
    }
}
