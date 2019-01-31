using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Base
{
    public enum FilterEnum : int
    {
        [DescriptionAttribute("Ninguno")]
        NONE = 1,
        [DescriptionAttribute("Temporal")]
        TEMPORARY = 2,
        [DescriptionAttribute("Activo")]
        ACTIVE = 3,
        [DescriptionAttribute("Inactivo")]
        INACTIVE = 4,
        [DescriptionAttribute("Abierto")]
        OPENED = 5,
        [DescriptionAttribute("Cerrado")]
        CLOSED = 6,
        [DescriptionAttribute("Cancelado")]
        CANCELED = 7,
        [DescriptionAttribute("Subasta")]
        AUCTION = 8,
    }
}
