using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension.Security;

namespace UGRS.Core.Auctions.Services.Security
{
    public class AuthorizationService
    {
        private IBaseDAO<User> mObjUserDAO;
        private IBaseDAO<Permission> mObjPermissionDAO;
        private IBaseDAO<Authorization> mObjAuthorizationDAO;

        public AuthorizationService(IBaseDAO<User> pObjUserDAO, IBaseDAO<Permission> pObjPermissionDAO, IBaseDAO<Authorization> pObjAuthorizationDAO)
        {
            mObjUserDAO = pObjUserDAO;
            mObjPermissionDAO = pObjPermissionDAO;
            mObjAuthorizationDAO = pObjAuthorizationDAO;
        }

        public bool Authorize(string pStrUser, string pStrPassword, SpecialFunctionsEnum pEnmFunction)
        {
            var lObjUser = mObjUserDAO.GetEntitiesList()
                .Where(x=> x.UserName == pStrUser)
                .Select(x=> new 
                {
                    Id = x.Id, 
                    UserTypeId = x.UserTypeId,
                    UserName = x.UserName, 
                    Password = x.Password
                })
                .ToList()
                .Where(x=> x.Password == pStrPassword.Encode())
                .FirstOrDefault();

            if (lObjUser != null)
            {
                return mObjPermissionDAO.GetEntitiesList()
                    .Where(x =>
                        x.AccessType == AccessTypeEnum.SPECIAL_FUNCTION &&
                        x.AccessId == (long)pEnmFunction &&
                        (
                            (x.PermissionType == PermissionTypeEnum.USER && x.PermissionId == lObjUser.Id) ||
                            (x.PermissionType == PermissionTypeEnum.USER_TYPE && x.PermissionId == lObjUser.UserTypeId)
                        ) &&
                        x.AllowAccess
                     ).Count() > 0;
            }
            else
            {
                throw new Exception("Usuario y/o contraseña inválido.");
            }
        }

        public long GetUserId(string pStrUser)
        {
            return mObjUserDAO.GetEntitiesList()
                .Where(x => x.UserName == pStrUser)
                .Select(x => (long?)x.Id)
                .FirstOrDefault() ?? 0;
        }

        public IQueryable<Authorization> GetList()
        {
            return mObjAuthorizationDAO.GetEntitiesList();
        }

        public void Save(Authorization pObjAuthorization)
        {
            mObjAuthorizationDAO.AddEntity(pObjAuthorization);
        }
    }
}
