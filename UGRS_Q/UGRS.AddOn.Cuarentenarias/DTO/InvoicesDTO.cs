using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.Models
{
    public class InvoicesDTO
    {
        public string Name { get; set; }
        public int IdInspection { get; set; }
        public DateTime ExpDate { get; set; }
        public string CardCode { get; set; }
        public string Corral { get; set; }
        public string CorralCode { get; set; }
        public int DocEntryGI { get; set; }
        public string Cancel { get; set; }
        public double TotalWeight { get; set; }
        public int TQuantity { get; set; }
        public int NP { get; set; }
        public int Rejected { get; set; }
        public string Payment { get; set; }
        public string PaymentCustom { get; set; }
        public string Article { get; set; }
        public int Serial { get; set; }
        public int DocEntryGR { get; set; }
        public string DocEntryIU { get; set; }
        public string DocEntryIM { get; set; }
        public double AverageWeight { get; set; }
        public string SpecialInsp { get; set; }

        public string SerialName { get; set; }
        public string ThreePercent { get; set; }
        public double RealWeight { get; set; }
        public int RowCode { get; set; }
        public string BatchNumber { get; set; }
        public string RBatchNumber { get; set; }
        public string Reference { get; set; }

    }
}
