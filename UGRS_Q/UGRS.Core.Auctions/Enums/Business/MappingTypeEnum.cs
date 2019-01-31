using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Business
{
    public enum MappingTypeEnum : int
    {
        [DescriptionAttribute("Existente")]
        EXISTING = 1,
        [DescriptionAttribute("Nuevo")]
        NEW = 2,
    }
}
