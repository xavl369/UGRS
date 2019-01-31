using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.Cuarentenarias.Enums
{
   public enum InvoiceConceptsUSDEnum : int
    {
       [Description("Cuota")]
       A00000441 = 1,
       [Description("Coral")]
       A00000045 = 2,
       [Description("Mantenimiento Cuarentenaria")]
       A00000442 = 2

    }

    public enum InvoiceCustomConcepts : int
    {
        [Description("SERVICIO TRAMITE ADUANA (HONORARIOS) ")]
        A00000047,
        [Description("SERVICIO TRAMITE ADUANA (PREVALIDACION PERMISO) UGRS")]//
        A00000048,
        [Description("SERVICIO TRAMITE ADUANA (PREVALIDACION PEDIMENTO)")]///
        A00000049,
        [Description("SERVICIO TRAMITE ADUANA (VALIDACION AGENCIA ADUANAL SHCP) IVA")]///
        A00000050
    }

    public enum InvoiceExtraConcepts : int
    {
        //[Description("KILOS DE ALIMENTOS")]
        //A00000439, // charge
         [Description("SERVICIO DE RECHAZO")]
        A00000075,
        // [Description("COBRO ADUANA UGRS")]
        //A00000440
        

    }
}
