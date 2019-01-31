using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Security
{
    public enum SpecialFunctionsEnum : int
    {
        [DescriptionAttribute("Cambiar precio")]
        CHANGE_PRICE = 1,
        [DescriptionAttribute("Cambiar peso")]
        CHANGE_WEIGHT = 2,
        [DescriptionAttribute("Cambiar comprador")]
        CHANGE_BUYER = 3,
        [DescriptionAttribute("Cambiar Vendedor")]
        CHANGE_SELLER = 4,
        [DescriptionAttribute("Cambiar Cantidad")]
        CHANGE_QUANTITY = 5,
        [DescriptionAttribute("Cambiar Reprogramado")]
        CHANGE_REPROGRAMMED = 6,
        [DescriptionAttribute("Cambiar No vendido")]
        CHANGE_NOTSOLD = 7

    }
}
