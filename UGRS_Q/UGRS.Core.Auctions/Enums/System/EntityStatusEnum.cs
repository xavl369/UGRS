using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.System
{
    public enum EntityStatusEnum : int
    {
        [DescriptionAttribute("Activo")]
        ACTIVE = 1,
        [DescriptionAttribute("Inactivo")]
        INACTIVE = 2
      

    }
}
