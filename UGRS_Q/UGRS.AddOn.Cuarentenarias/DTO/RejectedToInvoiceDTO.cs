using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.DTO
{
    public class RejectedToInvoiceDTO
    {
            public string Inspection { get; set; }
            public string Client { get; set; }
            public string HeadType { get; set; }
            public DateTime InspDate { get; set; }
            public double Stock { get; set; }
            public double Quantity { get; set; }
            public string DistNumb { get; set; }
            public double Price { get; set; }

            public string Option { get; set; }
            public string Reference { get; set; }
            public DateTime LastInvoice { get; set; }
            public double QuantityConcept { get; set; }
    }
}
