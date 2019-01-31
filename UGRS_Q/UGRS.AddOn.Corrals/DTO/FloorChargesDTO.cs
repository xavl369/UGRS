using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrales
{
    public class FloorChargesDTO
    {
        public string Checked { get; set; }
        public string Corral { get; set; }
        public double Total { get; set; }
        public string ItemCode { get; set; }

        public int Quantity { get; set; }
        public int BaseType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime InvDate { get; set; }
        public int Direction { get; set; }
        public string MovType { get; set; }
        public string DistNumber { get; set; }
        public int DaysxCorral { get; set; }

        public bool Show { get; set; }
    }
}
