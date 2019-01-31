using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.FoodProduction.Enums
{
    public class TicketEnum
    {
        public enum TicketStatus : int
        {
            [Description("Cerrado")]
            Close = 0,
            [Description("Abierto")]
            Open = 1,
            [Description("Pendiente de facturar")]
            Pending= 2,
        }
    }
}
