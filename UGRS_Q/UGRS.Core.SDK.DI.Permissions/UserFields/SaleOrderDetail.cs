using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.UserFields
{
    [SAPObject("RDR1")]
    public class SaleOrderDetail : SAPObject
    {
        [Field(Description = "Certificado", Type = BoFieldTypes.db_Numeric)]
        public int PE_Certificado { get; set; }

        //Referencia CF_GLO_5
        [Field(Description = "Estado", Size = 15)]
        public string PE_Estado { get; set; }

        //Referencia CF_GLO_6
        [Field(Description = "Ciudad", Size = 15)]
        public string PE_Ciudad { get; set; }
    }
}
