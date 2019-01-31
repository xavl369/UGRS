using System.ComponentModel;

namespace UGRS.Object.Boards.Enums
{
    public enum PositionEnum : int
    {
        [DescriptionAttribute("Numero de cabezas")]
        HEADS_NUMBER = 1,
        [DescriptionAttribute("Peso total")]
        TOTAL_WEIGHT = 2,
        [DescriptionAttribute("Peso promedio")]
        AVERAGE_WEIGHT = 3,
        [DescriptionAttribute("Numero de lote")]
        BATCH_NUMBER = 4,
        [DescriptionAttribute("Numero de cabezas")]
        SALE_HEADS_NUMBER = 5,
        [DescriptionAttribute("Peso total")]
        SALE_TOTAL_WEIGHT = 6,
        [DescriptionAttribute("Peso promedio")]
        SALE_AVERAGE_WEIGHT = 7,
        [DescriptionAttribute("Numero de comprador")]
        BUYER_NUMBER = 8,
        [DescriptionAttribute("Precio")]
        PRICE = 9
    }
}
