using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Services;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class ItemDefinitionService
    {
        private IBaseDAO<ItemDefinition> mObjItemDefinitionDAO;
        private IBaseDAO<Item> mObjItemDAO;
        private IBaseDAO<ItemType> mObjItemTypeDAO;

        public ItemDefinitionService(IBaseDAO<ItemDefinition> pObjItemDefinitionDAO, IBaseDAO<Item> pObjItemDAO, IBaseDAO<ItemType> pObjItemTypeDAO)
        {
            mObjItemDefinitionDAO = pObjItemDefinitionDAO;
            mObjItemDAO = pObjItemDAO;
            mObjItemTypeDAO = pObjItemTypeDAO;
        }

        public IQueryable<ItemDefinition> GetList()
        {
            return mObjItemDefinitionDAO.GetEntitiesList();
        }

        public IList<ItemDefinitionDTO> GetDefinitionsList()
        {
            return
            (
                from D in mObjItemDefinitionDAO.GetEntitiesList().ToList().Select(d => new { d.Id, d.Order, d.ItemId, d.ItemTypeId }).AsEnumerable()
                join I in mObjItemDAO.GetEntitiesList().ToList().Select(i => new { i.Id, i.Name }).AsEnumerable() on D.ItemId equals I.Id
                join T in mObjItemTypeDAO.GetEntitiesList().ToList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
                select new ItemDefinitionDTO
                {
                    Id = D.Id,
                    Order = D.Order,
                    ItemId = D.ItemId,
                    Item = I.Name,
                    ItemTypeId = D.ItemTypeId,
                    ItemType = T.Name
                }

            ).ToList();
        }

        public bool GetHeadTypeRelation(long pLonItemId, long? pLonItemTypeId)
        {

            return (from D in mObjItemDefinitionDAO.GetEntitiesList().ToList().Select(d => new { d.Id, d.Order, d.ItemId, d.ItemTypeId }).AsEnumerable()
               join I in mObjItemDAO.GetEntitiesList().Select(i => new { i.Id, i.Name }).AsEnumerable() on D.ItemId equals I.Id
               join T in mObjItemTypeDAO.GetEntitiesList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
               where D.ItemId == pLonItemId && D.ItemTypeId == pLonItemTypeId
               select new ItemDefinitionDTO
               {
                   Id = D.Id,
                   Order = D.Order,
                   ItemId = D.ItemId,
                   Item = I.Name,
                   ItemTypeId = D.ItemTypeId,
                   ItemType = T.Name
               }

               ).ToList().Count > 0 ? true: false;
        }


        public long GetArticleRelation(long? pLonItemTypeId)
        {
            return (from D in mObjItemDefinitionDAO.GetEntitiesList().ToList().Select(d => new { d.Id, d.Order, d.ItemId, d.ItemTypeId }).AsEnumerable()
                    join I in mObjItemDAO.GetEntitiesList().ToList().Select(i => new { i.Id, i.Name }).AsEnumerable() on D.ItemId equals I.Id
                    join T in mObjItemTypeDAO.GetEntitiesList().ToList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
                    where D.ItemTypeId == pLonItemTypeId
                    select D.ItemId
             ).FirstOrDefault();
        }

        public string GetArticle(long? pLonItemTypeId)
        {

            return (from D in mObjItemDefinitionDAO.GetEntitiesList().ToList().Select(d => new { d.Id, d.Order, d.ItemId, d.ItemTypeId }).AsEnumerable()
                    join I in mObjItemDAO.GetEntitiesList().ToList().Select(i => new { i.Id, i.Name, i.Code }).AsEnumerable() on D.ItemId equals I.Id
                    join T in mObjItemTypeDAO.GetEntitiesList().ToList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
                    where D.ItemId == GetArticleRelation(pLonItemTypeId) && D.ItemTypeId == pLonItemTypeId
                    select I.Code

               ).FirstOrDefault();
        }


        public void SaveOrUpdate(ItemDefinition pObjItemDefinition)
        {
            if (!Exists(pObjItemDefinition))
            {
                mObjItemDefinitionDAO.SaveOrUpdateEntity(pObjItemDefinition);
            }
            else
            {
                throw new Exception("La definición ingresada ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjItemDefinitionDAO.RemoveEntity(pLonId);
        }

        private bool Exists(ItemDefinition pObjItemDefinition)
        {
            return mObjItemDefinitionDAO.GetEntitiesList().ToList().Where(x => x.ItemTypeId == pObjItemDefinition.ItemTypeId && x.ItemId == pObjItemDefinition.ItemId && x.Id != pObjItemDefinition.Id).Count() > 0 ? true : false;
        }

        public bool GetDefinitions(long pLonItemTypeId)
        {
            return (from D in mObjItemDefinitionDAO.GetEntitiesList().ToList().Select(d => new { d.Id, d.Order, d.ItemId, d.ItemTypeId }).AsEnumerable()
                    join I in mObjItemDAO.GetEntitiesList().ToList().Select(i => new { i.Id, i.Name }).AsEnumerable() on D.ItemId equals I.Id
                    join T in mObjItemTypeDAO.GetEntitiesList().ToList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
                    where D.ItemTypeId == pLonItemTypeId
                    select new ItemDefinitionDTO
                    {
                        Id = D.Id,
                        Order = D.Order,
                        ItemId = D.ItemId,
                        Item = I.Name,
                        ItemTypeId = D.ItemTypeId,
                        ItemType = T.Name
                    }

                ).ToList().Count > 0 ? true : false;
        }


    }
}
