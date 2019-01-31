using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.Users
{
    [Table("UserTypes", Schema = "USERS")]
    public class UserType : BaseEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength(250)]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        #endregion
    }
}
