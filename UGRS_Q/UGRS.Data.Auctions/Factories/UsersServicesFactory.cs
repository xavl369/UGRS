using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Services.Users;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class UsersServicesFactory
    {
        public UserService GetUserService()
        {
            return new UserService(new BaseDAO<User>());
        }

        public UserTypeService GetUserTypeService()
        {
            return new UserTypeService(new BaseDAO<UserType>());
        }
    }
}
