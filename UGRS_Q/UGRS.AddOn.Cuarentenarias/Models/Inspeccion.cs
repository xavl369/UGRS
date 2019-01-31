using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.Models
{
    /// <summary>
    /// Representacion de un modelo de Inspección
    /// </summary>
    public class Inspeccion
    {
        public long RowCode { get; set; }
        public long IdInspection { get; set; }
        public string Date { get; set; }
        public string CardCode { get; set; }
        public string Client { get; set; }
        public string WhsCode { get; set; }
        public string Corral { get; set; }
        public string Type { get; set; }
        public long Heads { get; set; }
        public float TotalKg { get; set; }
        public long NP { get; set; }
        public long RE { get; set; }
        public double Average { get; set; }
        public string Status { get; set; }
        public long Series { get; set; }
        public string SeriesName { get; set; }
        public long User { get; set; }
        public string PaymentCustom { get; set; }
        public string SpecialInspection { get; set; }

        public string BatchNumber { get; set; }
        public string RBatchNumber { get; set; }
        public string Item { get; set; }
    }
}
