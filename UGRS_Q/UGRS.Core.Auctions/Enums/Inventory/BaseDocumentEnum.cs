using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.Enums.Inventory
{
    public enum BaseDocumentEnum : int
    {
        [DescriptionAttribute("SAP")]
        SAP = 1,
        [DescriptionAttribute("Subastas")]
        AUTIONS = 2,
        [DescriptionAttribute("Lote")]
        BATCH = 3,
    }
}
