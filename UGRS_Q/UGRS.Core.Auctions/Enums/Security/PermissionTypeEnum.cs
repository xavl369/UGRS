using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Security
{
    public enum PermissionTypeEnum : int
    {
        [DescriptionAttribute("Usuario")]
        USER = 1,
        [DescriptionAttribute("Tipo de usuario")]
        USER_TYPE = 2
    }
}
