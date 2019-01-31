using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;
using SAPbobsCOM;

namespace UGRS.Core.SDK.DI.Configuration.Tables
{
    [Table(Name = "UG_Config", Description = "Configuracion",  Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
     public class Config : Table
    {
        ///<summary>    Gets or sets the name. </summary>
        ///<value>  The name. </value>

        [Field(Description = "Name", Type = BoFieldTypes.db_Memo, Size = 100)]
        public string Name { get; set; }

        ///<summary>    Gets or sets the Date/Time of the value. </summary>
        ///<value>  The value. </value>

        [Field(Description = "Value", Type = BoFieldTypes.db_Memo, Size = 300)]
        public string Value { get; set; }
    }
}
