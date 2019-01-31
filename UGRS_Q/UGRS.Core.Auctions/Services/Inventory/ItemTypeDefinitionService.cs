using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Inventory;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Extension.Enum;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class ItemTypeDefinitionService
    {
        private IBaseDAO<ItemTypeDefinition> mObjItemTypeDefinitionDAO;
        private IBaseDAO<ItemType> mObjItemTypeDAO;

        public ItemTypeDefinitionService(IBaseDAO<ItemTypeDefinition> pObjItemTypeDefinitionDAO, IBaseDAO<ItemType> pObjItemTypeDAO)
        {
            mObjItemTypeDefinitionDAO = pObjItemTypeDefinitionDAO;
            mObjItemTypeDAO = pObjItemTypeDAO;
        }

        public IQueryable<ItemTypeDefinition> GetList()
        {
            return mObjItemTypeDefinitionDAO.GetEntitiesList();
        }

        public IList<ItemTypeDefinitionDTO> GetDefinitionsList()
        {
            return
            (
                from D in mObjItemTypeDefinitionDAO.GetEntitiesList().Select(d => new { d.Id, d.ItemTypeId, d.AuctionType }).AsEnumerable()
                join T in mObjItemTypeDAO.GetEntitiesList().Select(t => new { t.Id, t.Name }).AsEnumerable() on D.ItemTypeId equals T.Id
                select new ItemTypeDefinitionDTO
                {
                    Id = D.Id,
                    ItemTypeId = D.ItemTypeId,
                    ItemType = T.Name,
                    AuctionType = D.AuctionType
                }
            )
            .ToList()
            .Select(x => { x.AuctionTypeName = x.AuctionType.GetDescription(); return x; })
            .ToList();
        }

        public void SaveOrUpdate(ItemTypeDefinition pObjItemDefinition)
        {
            if (!Exists(pObjItemDefinition))
            {
                mObjItemTypeDefinitionDAO.SaveOrUpdateEntity(pObjItemDefinition);
            }
            else
            {
                throw new Exception("La definición ingresada ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjItemTypeDefinitionDAO.RemoveEntity(pLonId);
        }

        private bool Exists(ItemTypeDefinition pObjItemDefinition)
        {
            return mObjItemTypeDefinitionDAO
                .GetEntitiesList()
                .Where(x => x.ItemTypeId == pObjItemDefinition.ItemTypeId
                    && x.AuctionType == pObjItemDefinition.AuctionType
                    && x.Id != pObjItemDefinition.Id)
                .Count() > 0 ? true : false;
        }
    }
}
