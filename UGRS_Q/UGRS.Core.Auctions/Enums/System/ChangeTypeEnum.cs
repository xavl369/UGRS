using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.Enums.System
{
    public enum ChangeTypeEnum : int
    {
        [DescriptionAttribute("Registrar")]
        INSERT = 1,
        [DescriptionAttribute("Modificar")]
        UPDATE = 2,
        [DescriptionAttribute("Eliminar")]
        DELETE = 3
    }
}
