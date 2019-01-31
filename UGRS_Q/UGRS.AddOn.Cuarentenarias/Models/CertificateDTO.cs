using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.Models
{
    /// <summary>
    /// Model Certificate
    /// Representacion de un modelo de certificado en Cuarentenarias.
    /// </summary>
    public class CertificateDTO
    {
        public String UsedDate { get; set; }
        public long NCertificate { get; set; }
        public String TypeG { get; set; }
        public long Quantity { get; set; }
        public long QuantityUsed { get; set; }
        public long Serie { get; set; }
        public string CardCode { get; set; }
        public string Status { get; set; }

        public int RowCode { get; set; }
        public string Name { get; set; }
        public int IdInspection { get; set; }
    }
}
