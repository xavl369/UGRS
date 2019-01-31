using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.System
{
    public enum AccessTypeEnum : int
    {
        [DescriptionAttribute("Módulo")]
        MODULE = 1,
        [DescriptionAttribute("Sección")]
        SECTION = 2,
        [DescriptionAttribute("Menu")]
        MENU = 3,
        [DescriptionAttribute("Proceso")]
        PROCESS = 4,
        [DescriptionAttribute("Operación")]
        OPERATION = 5,
        [DescriptionAttribute("Función especial")]
        SPECIAL_FUNCTION = 6,
    }
}
