using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Models;
using UGRS.Core.SDK.Attributes;
using SAPbobsCOM;


namespace UGRS.AddOn.Cuarentenarias.Tables
{
    /// <summary>
    /// InspeccionT
    /// <remarks></remarks>
    /// </summary>

    [Table(Name = "UG_CU_OINS", Description = "CU Inspeccion", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class InspeccionT : Table
    {

        [Field(Description = "Name", Size = 10)]
        public string Name { get; set; }

        [Field(Description = "ID Inspeccion", Type = BoFieldTypes.db_Numeric)]
        public long IDInsp { get; set; }

        [Field(Description = "Fecha de Inspeccion", Type = BoFieldTypes.db_Date)]
        public DateTime DateInsp { get; set; }

        [Field(Description = "ID Usuario", Type = BoFieldTypes.db_Numeric)]
        public long User { get; set; }

        [Field(Description = "Fecha del sistema", Type = BoFieldTypes.db_Date)]
        public DateTime DateSys { get; set; }

        [Field(Description = "CardCode Cliente", Size = 15)]
        public string CardCode { get; set; }

        [Field(Description = "WhsCode Almacen", Size = 15)]
        public string WhsCode { get; set; }

        [Field(Description = "ID de Salida", Type = BoFieldTypes.db_Numeric)]
        public long DocEntryGI { get; set; }

        [Field(Description = "ID de Entrada", Type = BoFieldTypes.db_Numeric)]
        public long DocEntryGR { get; set; }

        [Field(Description = "Factura Mexico", Size = 11)]
        public string DocEntryIM { get; set; }

        [Field(Description = "Factura USD", Size = 11)]
        public string DocEntryIU { get; set; }

        [Field(Description = "Cancelado", Size = 10)]
        public string Cancel { get; set; }

        [Field(Description = "Total Kilogramos", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float TotKls { get; set; }

        [Field(Description = "Peso Promedio", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float AverageW { get; set; }

        [Field(Description = "Cantidad de Cabezas", Type = BoFieldTypes.db_Numeric)]
        public long Quantity{ get; set; }

        [Field(Description = "Ganado No Presentado", Type = BoFieldTypes.db_Numeric)]
        public long QuantityNP { get; set; }

        [Field(Description = "Ganado Rechazado", Type = BoFieldTypes.db_Numeric)]
        public long QuantityReject { get; set; }

        [Field(Description = "Cobro 3%", Size = 10)]
        public string Payment { get; set; }

        [Field(Description = "Aduana UGRS", Size = 10)]
        public string PaymentCustom { get; set; }

        [Field(Description = "Tipo de Ganado", Size = 50)]
        public string Classification { get; set; }

        [Field(Description = "Series", Type = BoFieldTypes.db_Numeric)]
        public long Series { get; set; }

        [Field(Description = "Inspeccion Especial", Size = 10)]
        public string CheckInsp { get; set; }

        [Field(Description = "Hora Inspeccion", Size = 10)]
        public string Time { get; set; }
    }
}
