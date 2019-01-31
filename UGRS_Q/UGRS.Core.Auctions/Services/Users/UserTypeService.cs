using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Users;

namespace UGRS.Core.Auctions.Services.Users
{
    public class UserTypeService
    {
        private IBaseDAO<UserType> mObjUserTypeDAO;

        public UserTypeService(IBaseDAO<UserType> pObjUserTypeDAO)
        {
            mObjUserTypeDAO = pObjUserTypeDAO;
        }

        public IQueryable<UserType> GetList()
        {
            return mObjUserTypeDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(UserType pObjUserType)
        {
            if (!Exists(pObjUserType))
            {
                mObjUserTypeDAO.SaveOrUpdateEntity(pObjUserType);
            }
            else
            {
                throw new Exception("El tipo de usuario ingresado ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjUserTypeDAO.RemoveEntity(pLonId);
        }

        private bool Exists(UserType pObjUserType)
        {
            return mObjUserTypeDAO.GetEntitiesList().Where(x => x.Name == pObjUserType.Name && x.Id != pObjUserType.Id).Count() > 0 ? true : false;
        }
    }
}
