using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.UserFields
{
    [SAPObject("ORDR")]
    public class SaleOrder : SAPObject
    {
        //Requiere vinculación con TBL_PE_TipoP
        [Field(Description = "Tipo de permiso", Size = 50)]
        public string PE_TipoPermiso { get; set; }

        [Field(Description = "Solicitud UGRS", Size = 2)]
        public string PE_SolicitudUnion { get; set; }

        [Field(Description = "Folio UGRS", Type = BoFieldTypes.db_Numeric)]
        public int PE_FolioUnion { get; set; }

        [Field(Description = "Cargar a", Size = 50)]
        public string PE_CargarA { get; set; }

        [Field(Description = "Solicita", Size = 50)]
        public string PE_Solicita { get; set; }

        [Field(Description = "Aduana 1", Size = 50)]
        public string PE_Aduana1 { get; set; }

        [Field(Description = "Aduana 2", Size = 50)]
        public string PE_Aduana2 { get; set; }

        [Field(Description = "Representante", Size = 50)]
        public string PE_Representante { get; set; }

        //Requiere vinculación con TBL_PE_Asociaciones
        [Field(Description = "Asociacion", Size = 50)]
        public string PE_Asociacion { get; set; }

        [Field(Description = "Origen", Size = 50)]
        public string PE_Origen { get; set; }

        //Requiere vinculación con TBL_PE_Ubicaciones
        [Field(Description = "Ubicación", Size = 50)]
        public string PE_Ubicacion { get; set; }

        [Field(Description = "Telefono", Size = 50)]
        public string PE_Telefono { get; set; }

        [Field(Description = "Destino", Size = 50)]
        public string PE_Destino { get; set; }

        [Field(Description = "Entrada", Size = 50)]
        public string PE_Entrada { get; set; }

        [Field(Description = "Salida", Size = 50)]
        public string PE_Salida { get; set; }

        [Field(Description = "Transporte", Size = 50)]
        public string PE_Transporte { get; set; }

        [Field(Description = "Vencimiento", Type = BoFieldTypes.db_Date)]
        public DateTime PE_Vencimiento { get; set; }

        [Field(Description = "PuntoVerificacion", Size = 30)]
        public string PE_PuntoVerificacion { get; set; }

        [Field(Description = "EdadGanado", Size = 30)]
        public string PE_EdadGanado { get; set; }

        [Field(Description = "PuntoVerficacionNorteSur", Size = 30)]
        public string PE_PuntoVerficacionNorteSur { get; set; }
    }
}
