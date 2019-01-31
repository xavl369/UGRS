using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Users;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Extension.Security;

namespace UGRS.Core.Auctions.Services.Users
{
    public class UserService
    {
        private IBaseDAO<User> mObjUserDAO;

        public UserService(IBaseDAO<User> pObjUserDAO)
        {
            mObjUserDAO = pObjUserDAO;
        }

        public IQueryable<User> GetList()
        {
            return mObjUserDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(User pObjUser)
        {
            if (!ExistsUser(pObjUser.UserName, pObjUser.Id))
            {
                pObjUser.Password = pObjUser.Password.Encode();
                mObjUserDAO.SaveOrUpdateEntity(pObjUser);
            }
            else
            {
                throw new Exception("El usuario ingresado ya existe.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjUserDAO.RemoveEntity(pLonId);
        }

        public UserDTO Login(string pStrUserName, string pStrPassword)
        {
            User lObjUser = mObjUserDAO.GetEntitiesList().ToList().FirstOrDefault(x => x.UserName == pStrUserName && x.Password == pStrPassword.Encode());

            if (lObjUser != null)
            {
                return new UserDTO()
                {
                    Id = lObjUser.Id,
                    Image = lObjUser.Image,
                    UserName = lObjUser.UserName,
                    EmailAddress = lObjUser.EmailAddress,
                    FirstName = lObjUser.FirstName,
                    LastName = lObjUser.LastName,
                    UserTypeId = lObjUser.UserTypeId,
                    UserType = lObjUser.UserType.Name
                };
            }
            else
            {
                throw new Exception("Usuario y/o contraseña inválido.");
            }
        }

        public UserDTO GetUserDTO(long pLonIdUser)
        {
            User objUser = mObjUserDAO.GetEntity(pLonIdUser);

            return new UserDTO()
            {
                Id = objUser.Id,
                UserName = objUser.UserName,
                EmailAddress = objUser.EmailAddress,
                FirstName = objUser.FirstName,
                LastName = objUser.LastName,
                UserTypeId = objUser.UserTypeId,
                UserType = objUser.UserType.Name
            };
        }

        public bool ChangePassword(long pLonIdUser, string pStrOldPassword, string pStrNewPassword)
        {
            bool lBolResult = false;
            User lObjUser = mObjUserDAO.GetEntity(pLonIdUser);

            if (ValidPassword(pStrOldPassword, lObjUser.Password))
            {
                if (!lObjUser.Protected)
                {
                    lObjUser.Password = pStrNewPassword.Encode();
                    mObjUserDAO.SaveOrUpdateEntity(lObjUser);
                    lBolResult = true;
                }
                else
                {
                    throw new Exception("Registro protegido contra escritura.");
                }
            }
            else
            {
                throw new Exception("La contraseña ingresada no es correcta.");
            }

            return lBolResult;
        }

        public bool ExistsEmail(string pStrEmail, long pLonIdUsuario)
        {
            return mObjUserDAO.GetEntitiesList().Where(x => x.EmailAddress == pStrEmail && x.Id != pLonIdUsuario).Count() > 0 ? true : false;
        }

        public bool ExistsUser(string pStrUser, long pLonIdUsuario)
        {
            return mObjUserDAO.GetEntitiesList().Where(x => x.UserName == pStrUser && x.Id != pLonIdUsuario).Count() > 0 ? true : false;
        }

        private bool ValidPassword(string pStrPassword, string pStrEncodedPassword)
        {
            return pStrEncodedPassword.Equals(pStrPassword.Encode());
        }
    }
}
