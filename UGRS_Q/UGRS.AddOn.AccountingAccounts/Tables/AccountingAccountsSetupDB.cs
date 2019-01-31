using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.AccountingAccounts.Tables
{
    [Table(Name = "UG_AA_DB", Description = "AA Setup Database", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class AccountingAccountsSetupDB : Table
    {
        [Field(Description = "Code_UG_AA_LOGIN", Size = 8, LinkedTable = "UG_AA_LOGIN")]
        public string Code_UG_AA_LOGIN { get; set; }

        [Field(Description = "NameDB", Size = 50)]
        public string NameDB { get; set; }

        [Field(Description = "Descripcion", Size = 50)]
        public string Descripcion { get; set; }

        [Field(Description = "Status", Size = 1)]
        public string Status { get; set; }
    }
}
