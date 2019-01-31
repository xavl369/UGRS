using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Users
{
    [Table("Users", Schema = "USERS")]
    public class User : BaseEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string UserName { get; set; }

        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string EmailAddress { get; set; }

        [Column(TypeName = "text")]
        public string Password { get; set; }

        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar"), StringLength(50), Required]
        public string LastName { get; set; }

        #endregion

        #region External properties

        public long UserTypeId { get; set; }

        [Column(TypeName = "text")]
        public string Image { get; set; }

        [ForeignKey("UserTypeId")]
        public virtual UserType UserType { get; set; }

        #endregion
    }
}
