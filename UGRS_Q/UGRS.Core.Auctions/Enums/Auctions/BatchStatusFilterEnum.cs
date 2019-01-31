using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Auctions
{
    public enum BatchStatusFilterEnum : int
    {
        [DescriptionAttribute("Todo")]
        ALL = 1,
        [DescriptionAttribute("Vendido")]
        SOLD = 2,
        [DescriptionAttribute("No vendido")]
        UNSOLD = 3,
    }
}
