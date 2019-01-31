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

    /// 
    /// </summary>
    [Table(Name = "UG_CU_SICERT", Description = "CU Certificados de SI", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class SICertificates : Table
    {


        [Field(Description = "Tipo de Ganado", Size = 50)]
        public string ItemCode { get; set; }

        [Field(Description = "Cantidad de Cabezas", Type = BoFieldTypes.db_Numeric)]
        public long Quantity { get; set; }
    }


}
