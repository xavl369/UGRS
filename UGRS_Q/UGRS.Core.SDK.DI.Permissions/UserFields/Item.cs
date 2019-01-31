using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.UserFields
{
    [SAPObject("OITM")]
    public class Item : SAPObject
    {
        [Field(Description = "Tipo de permiso", Size = 15)]
        public string PE_TipoPermiso { get; set; }

        [Field(Description = "Id Sagarpa", Size = 30)]
        public string PE_IdSagarpa { get; set; }
    }
}
