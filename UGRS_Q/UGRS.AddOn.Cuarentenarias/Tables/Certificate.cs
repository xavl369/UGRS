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
    /// 
    /// </summary>
    [Table(Name = "UG_CU_CERT", Description = "CU Certificados", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class Certificate : Table
    {

        [Field(Description = "ID Inspeccion", Type = BoFieldTypes.db_Numeric)]
        public long IDInsp { get; set; }

        [Field(Description = "Tipo de Ganado", Size = 50)]
        public string ItemCode { get; set; }

        [Field(Description = "Cantidad de Cabezas", Type = BoFieldTypes.db_Numeric)]
        public long Quantity { get; set; }

        [Field(Description = "Serie", Type = BoFieldTypes.db_Numeric)]
        public long Serie { get; set; }

        [Field(Description = "Fecha de uso", Type = BoFieldTypes.db_Date)]
        public DateTime UsedDate { get; set; }

        [Field(Description = "Generacion NC", Type = BoFieldTypes.db_Alpha)]
        public bool CreditNote { get; set; }
    }
}
