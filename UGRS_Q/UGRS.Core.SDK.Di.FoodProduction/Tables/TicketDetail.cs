using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;


namespace UGRS.Core.SDK.DI.FoodProduction.Tables
{
    [Table(Name = "UG_PL_TCKD", Description = "PL Ticket detalle", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class TicketDetail : Table
    {
        [Field(Description = "Folio", Size = 64)]
        public string Folio { get; set; }

        [Field(Description = "Line", Type = BoFieldTypes.db_Numeric)]
        public int Line { get; set; }

        [Field(Description = "Artículo", Size = 64)]
        public string Item { get; set; }

        [Field(Description = "Precio", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Price { get; set; }

        [Field(Description = "Primer peso", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float FirstWT { get; set; }

        [Field(Description = "Segundo entrada", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float SecondWT { get; set; }

        [Field(Description = "Peso neto", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float netWeight { get; set; }

        [Field(Description = "Importe", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public float Amount { get; set; }

        [Field(Description = "Sacos - Pacas", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float BagsBales { get; set; }  
        
        [Field(Description = "Fecha entrada", Type = BoFieldTypes.db_Date)]
        public DateTime EntryDate { get; set; }

        [Field(Description = "Hora entrada", Type = BoFieldTypes.db_Numeric)]
        public int EntryTime { get; set; }

        [Field(Description = "Fecha salida", Type = BoFieldTypes.db_Date)]
        public DateTime OutputDate { get; set; }

        [Field(Description = "Hora salida", Type = BoFieldTypes.db_Numeric)]
        public int OutputTime { get; set; }

        [Field(Description = "Almacén", Size = 64)]
        public string WhsCode { get; set; }

        [Field(Description = "Bascula", Type = BoFieldTypes.db_Numeric)]
        public int WeighingM { get; set; }

        [Field(Description = "BaseLine", Type = BoFieldTypes.db_Numeric)]
        public int BaseLine { get; set; }

        
        
    }
}
