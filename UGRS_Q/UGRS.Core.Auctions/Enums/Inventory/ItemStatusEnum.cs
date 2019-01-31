using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Inventory
{
    public enum ItemStatusEnum : int
    {
        [DescriptionAttribute("Activo")]
        ACTIVE = 1,
        [DescriptionAttribute("Inactivo")]
        INACTIVE = 2,
        [DescriptionAttribute("Avanzado")]
        ADVANCED = 3,
    }
}
