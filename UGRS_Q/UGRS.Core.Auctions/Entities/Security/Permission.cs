using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Entities.Security
{
    [Table("Permissions", Schema = "SECURITY")]
    public class Permission : BaseEntity
    {
        #region Entity properties

        [Required]
        public PermissionTypeEnum PermissionType { get; set; }

        [Required]
        public long PermissionId { get; set; }

        [Required]
        public AccessTypeEnum AccessType { get; set; }

        [Required]
        public long AccessId { get; set; }

        [Required]
        public bool AllowAccess { get; set; }

        #endregion
    }
}
