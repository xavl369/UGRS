using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.System
{
    public enum ChangeStatusEnum : int
    {
        [DescriptionAttribute("Pendiente")]
        PENDING = 1,
        [DescriptionAttribute("Rechazado")]
        REJECTED = 2,
        [DescriptionAttribute("Autorizado")]
        AUTHORIZED = 3
    }
}
