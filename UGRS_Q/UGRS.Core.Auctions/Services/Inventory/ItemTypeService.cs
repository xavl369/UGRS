using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Catalogs;
using UGRS.Core.Auctions.Entities.Inventory;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;

namespace UGRS.Core.Auctions.Services.Inventory
{
    public class ItemTypeService
    {
        private IBaseDAO<ItemType> mObjItemTypeDAO;
        private IBaseDAO<ItemTypeDefinition> mObjItemTypeDefinitionDAO;


        public ItemTypeService(IBaseDAO<ItemType> pObjItemTypeDAO, IBaseDAO<ItemTypeDefinition> pObjItemTypeDefinitionDAO)
        {
            mObjItemTypeDAO = pObjItemTypeDAO;
            mObjItemTypeDefinitionDAO = pObjItemTypeDefinitionDAO;
        }

        public ItemType GetEntity(long pLonId)
        {
            return mObjItemTypeDAO.GetEntity(pLonId);
        }

        public ItemTypeDTO GetCustomEntity(long pLonId)
        {
            return GetCustomList().FirstOrDefault(x=> x.Id == pLonId);
        }

        public IQueryable<ItemType> GetList()
        {
            return mObjItemTypeDAO.GetEntitiesList();
        }

        public IList<ItemTypeDTO> GetCustomList()
        {
            return FillCustomList(mObjItemTypeDAO.GetEntitiesList().Where(x=> x.Active));
        }

        public void SaveOrUpdate(ItemType pObjItemType)
        {
            if (!Exists(pObjItemType))
            {
                mObjItemTypeDAO.SaveOrUpdateEntity(pObjItemType);
            }
            else
            {
                throw new Exception("El tipo de artículo ingresado ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjItemTypeDAO.RemoveEntity(pLonId);
        }

        public List<ItemType> SearchItemTypeByAuctionType(string pStrText, AuctionTypeEnum pEnmAuctionType, FilterEnum pEnmFilter)
        {
            int lIntMaxLevel = this.GetList().Select(x => x.Level).Max();

            IList<long> lLstLonDefinitions = mObjItemTypeDefinitionDAO
                                                .GetEntitiesList()
                                                    .Where(x => x.AuctionType == pEnmAuctionType)
                                                        .Select(y => y.ItemTypeId).ToList();

            IQueryable<ItemType> lLstObjItemTypes = this.GetList().Where(x => x.Active && x.Level == lIntMaxLevel 
                && lLstLonDefinitions.Contains(x.Id));

            return GetBestSearch(pStrText, pEnmFilter, lLstObjItemTypes).ToList();
        }

        public List<ItemType> SearchItemType(string pStrText, FilterEnum pEnmFilter)
        {
            return Search(pStrText, pEnmFilter).ToList();
        }

        public List<ItemTypeDTO> SearchCustomItemType(string pStrText, FilterEnum pEnmFilter)
        {
            return FillCustomList(Search(pStrText, pEnmFilter)).ToList();
        }

        private IQueryable<ItemType> Search(string pStrText, FilterEnum pEnmFilter)
        {
            return GetBestSearch(pStrText, pEnmFilter, this.GetList().Where(x => x.Active));
        }

        private IQueryable<ItemType> GetBestSearch(string pStrText, FilterEnum pEnmFilter, IQueryable<ItemType> pLstObjItemTypes)
        {
            IList<IQueryable<ItemType>> lLstObjQueries = new List<IQueryable<ItemType>>();

            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Code.ToUpper().Contains(pStrText.ToUpper())));
            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Name.ToUpper().Contains(pStrText.ToUpper())));
            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Code.ToUpper().StartsWith(pStrText.ToUpper())));
            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Name.ToUpper().StartsWith(pStrText.ToUpper())));
            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Code.ToUpper().EndsWith(pStrText.ToUpper())));
            lLstObjQueries.Add(pLstObjItemTypes.Where(x => x.Name.ToUpper().EndsWith(pStrText.ToUpper())));

            IQueryable<ItemType> lLstObjBestQuery = pLstObjItemTypes;
            int lIntBestRowCount = pLstObjItemTypes.Count();

            for (int i = 0; i < lLstObjQueries.Count; i++)
            {
                int lIntCurrentRowCount = lLstObjQueries[i].Count();
                if (lIntCurrentRowCount > 0 && lIntCurrentRowCount < lIntBestRowCount)
                {
                    lLstObjBestQuery = lLstObjQueries[i];
                    lIntBestRowCount = lIntCurrentRowCount;
                }
            }

            return lLstObjBestQuery;
        }

        private IList<ItemTypeDTO> FillCustomList(IQueryable<ItemType> pObjItemType)
        {
            return pObjItemType.Select(x => new ItemTypeDTO()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                //PerPrice = x.PerPrice,
                Level = x.Level,
                GenderId = (int)x.Gender,
                SellTypeid = (int)x.SellType,
                CategoryId = x.Parent != null && x.Parent.Parent != null? x.Parent.ParentId ?? 0:
                             x.Parent != null ? x.ParentId?? 0 : 0,

                Category = x.Parent != null && x.Parent.Parent != null? x.Parent.Parent.Name:
                           x.Parent != null ? x.Parent.Name : "",

                SubCategoryId = x.Parent != null && x.Parent.Parent != null ? x.ParentId ?? 0 : 0,

                SubCategory = x.Parent != null && x.Parent.Parent != null ? x.Parent.Name : "",

            }).ToList();
        }

        private bool Exists(ItemType pObjItemType)
        {
            return mObjItemTypeDAO.GetEntitiesList().Where(x => x.Code == pObjItemType.Code && x.Name == pObjItemType.Name && x.Id != pObjItemType.Id).Count() > 0 ? true : false;
        }
    }
}
