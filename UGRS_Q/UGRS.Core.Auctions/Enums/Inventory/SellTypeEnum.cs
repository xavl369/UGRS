using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.Enums.Inventory
{
    public enum SellTypeEnum : int
    {
    
            [DescriptionAttribute("Por Kilo")]
            PerWeight = 0,
            [DescriptionAttribute("Por Precio")]
            PerPrice = 1,
            [DescriptionAttribute("Ambos")]
            Both = 2
        
    }
}
