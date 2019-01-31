using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;


namespace UGRS.AddOn.AccountingAccounts.Tables
{
    [Table(Name = "UG_AA_DTLL", Description = "AA Detalle del Registro", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class AccountingAccountsLoggDetail : Table
    {
        [Field(Description = "Code_AA_LOGG", Size = 8, LinkedTable = "UG_AA_LOGG")]
        public string Code_NM_LOGG { get; set; }

        [Field(Description = "ErrorDescription", Type = BoFieldTypes.db_Memo)]
        public string ErrorDescription { get; set; }
    }
}
