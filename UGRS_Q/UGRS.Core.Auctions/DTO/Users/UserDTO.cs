
namespace UGRS.Core.Auctions.DTO.Users
{
    public class UserDTO
    {
        #region Entity properties

        public long Id { get; set; }

        public string Image { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        #endregion

        #region External properties

        public long UserTypeId { get; set; }

        public string UserType { get; set; }

        #endregion
    }
}
