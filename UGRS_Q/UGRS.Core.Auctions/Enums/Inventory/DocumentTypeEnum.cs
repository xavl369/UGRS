using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Inventory
{
    public enum DocumentTypeEnum : int
    {
        [DescriptionAttribute("Entrada")]
        GOODS_RECEIPT = 1,
        [DescriptionAttribute("Salida")]
        GOODS_ISSUE = 2,
        [DescriptionAttribute("Devolución")]
        GOODS_RETURN = 3,
    }
}
