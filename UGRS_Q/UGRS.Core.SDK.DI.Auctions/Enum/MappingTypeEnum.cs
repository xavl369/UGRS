using System.ComponentModel;

namespace UGRS.Core.SDK.DI.Auctions.Enum
{
    public enum MappingTypeEnum : int
    {
        [DescriptionAttribute("Existente")]
        EXISTING = 1,
        [DescriptionAttribute("Nuevo")]
        NEW = 2,
    }
}
