using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Services.Security;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class SecurityServicesFactory
    {
        public PermissionService GetPermissionService()
        {
            return new PermissionService(
                new BaseDAO<Module>(), 
                new BaseDAO<Section>(), 
                new BaseDAO<Permission>(), 
                new BaseDAO<User>());
        }

        public AuthorizationService GetAuthorizationService()
        {
            return new AuthorizationService(
                new BaseDAO<User>(), 
                new BaseDAO<Permission>(), 
                new BaseDAO<Authorization>());
        }
    }
}
