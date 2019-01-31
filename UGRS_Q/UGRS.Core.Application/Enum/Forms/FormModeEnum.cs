using System.ComponentModel;

namespace UGRS.Core.Application.Enum.Forms
{
    public enum FormModeEnum : int
    {
        [DescriptionAttribute("Default")]
        DEFAULT = 0,
        [DescriptionAttribute("Nuevo")]
        NEW = 1,
        [DescriptionAttribute("Modificar")]
        EDIT = 2,
        [DescriptionAttribute("Consulta")]
        READ = 3,
        [DescriptionAttribute("Eliminar")]
        REMOVE = 4,
    }
}
