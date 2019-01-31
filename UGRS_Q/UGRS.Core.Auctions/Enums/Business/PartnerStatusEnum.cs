using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Business
{
    public enum PartnerStatusEnum : int
    {
        [DescriptionAttribute("Activo")]
        ACTIVE = 1,
        [DescriptionAttribute("Inactivo")]
        INACTIVE = 2,
        [DescriptionAttribute("Avanzado")]
        ADVANCED = 3,
    }
}
