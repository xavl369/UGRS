using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.Permissions.Tables
{
    [Table(Name = "UG_PE_PRFX", Description = "PE Prefijos para Aretes", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    class PrefixesT : Table
    {
           
  
         [Field(Description = "Prefijo", Size = 50)]
         public string Prefix { get; set; }

         [Field(Description = "Activo", Size = 10)]
         public string Active { get; set; }


    }
}
