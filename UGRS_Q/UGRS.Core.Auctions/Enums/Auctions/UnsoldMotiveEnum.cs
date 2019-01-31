using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Auctions
{
    public enum UnsoldMotiveEnum : int
    {
        [DescriptionAttribute("Malanco")]
        DEFECTIVE = 1,
        [DescriptionAttribute("No Arete")]
        NO_EARRING = 2,
        [DescriptionAttribute("No Marca")]
        NO_BRAND = 3,
        [DescriptionAttribute("Precio")]
        PRICE = 4
    }
}
