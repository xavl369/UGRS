using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.FoodProduction.DTO
{
    public class TotalsDTO
    {
        public double Amount { get; set; } //lDblAmount
        public double OutputW { get; set; }//lDblOutput
        public double InputW { get; set; } //lDblInput
        public double WeightB { get; set; } //lDblPesoBruto
        public double Tara { get; set; }//lDblTara
        public double WeightTotal { get; set; }//lDblPeso
        public double Bags { get; set; }//lDblSacos
        public double Variation { get; set; }//lDblVariacion
        public double WeightNet { get; set; }//PesoNeto
    }
}
