using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.Cuarentenarias.Tables
{
    /// <summary>
    /// Class InspectionDetails
    /// <remarks>@Author RCordova- 2017/08/14</remarks>
    /// </summary>
    [Table(Name = "UG_CU_INS1", Description = "CU Inspección Detalle", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class InspectionDetails : Table
    {

        [Field(Description = "Codigo de Inspección", Type = BoFieldTypes.db_Numeric)]
        public long CodeInsp { get; set; }

        [Field(Description = "Articulo", Size = 50)]
        public string ItemCode { get; set; }

        [Field(Description = "Tipo", Type = BoFieldTypes.db_Numeric)]
        public long MvtoType { get; set; }

        [Field(Description = "Series", Size = 3000)]
        public string Commentary { get; set; }

        [Field(Description = "Cantidad de Cabezas", Type = BoFieldTypes.db_Numeric)]
        public long Quantity { get; set; }

    }
}
