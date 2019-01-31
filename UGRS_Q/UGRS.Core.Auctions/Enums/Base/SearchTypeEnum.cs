using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Base
{
    public enum SearchTypeEnum : int
    {
        [DescriptionAttribute("Formulario")]
        FORM = 1,
        [DescriptionAttribute("Seleccion")]
        CHOOSE = 2
    }
}
