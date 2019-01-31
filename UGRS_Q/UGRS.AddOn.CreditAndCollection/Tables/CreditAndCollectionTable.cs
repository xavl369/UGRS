using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.CreditAndCollection.Tables
{

     [Table(Name = "UG_CC_AUTN", Description = "CU Credito y Cobranza", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]

   public class CreditAndCollectionTable : Table 
    {

        [Field(Description = "Folio de Subasta", Size = 20)]
        public string FolioSubasta { get; set; }

        [Field(Description = "Tipo de Auxiliar", Type = BoFieldTypes.db_Numeric)]
        public long TipoAux { get; set; }

        [Field(Description = "Cantidad de Cabezas", Size = 15)]
        public string Auxiliar { get; set; }

        [Field(Description = "Revisado", Size = 5)]
        public string Revizado { get; set; }

    }
}
