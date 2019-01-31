using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Inventory
{
    public enum ItemTypeGenderEnum : int
    {
        [DescriptionAttribute("Hembra")]
        Hembra = 0,
        [DescriptionAttribute("Macho")]
        Macho = 1,
        [DescriptionAttribute("Otro")]
        Otro = 2
    }
}
