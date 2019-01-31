using System.ComponentModel;

namespace UGRS.Core.Application.Enum.Base
{
    public enum ViewStatusEnum : int
    {
        [DescriptionAttribute("Cerrado")]
        CLOSED = 0,
        [DescriptionAttribute("Cargando")]
        LOADING = 1,
        [DescriptionAttribute("Listo")]
        READY = 2
    }
}
