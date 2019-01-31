using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.AccountingAccounts.Tables
{
    [Table(Name = "UG_AA_LOGIN", Description = "AA Setup Login", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class AccountingAccountsSetupLogin : Table
    {
    [Field(Description = "NameServer", Size = 100)]
    public string NameServer { get; set; }

    [Field(Description = "Code", Type = BoFieldTypes.db_Numeric)]
    public int Code { get; set; }
    
    [Field(Description = "IdBD", Type = BoFieldTypes.db_Numeric)]
    public int IdBD { get; set; }

    /*[Field(Description = "Name", Size = 100)]
    public string Name { get; set; }*/

    [Field(Description = "Login", Size = 20)]
    public string Login { get; set; }

    [Field(Description = "Password", Size = 20)]
    public string Password { get; set; }

    [Field(Description = "AccountingAccount", Size = 16)]
    public string AccountingAccount { get; set; }

    [Field(Description = "Activo", Size = 1)]
    public string Activo { get; set; }

    [Field(Description = "NameDB", Size = 50)]
    public string NameDB { get; set; }

    [Field(Description = "Descripcion", Size = 50)]
    public string Descripcion { get; set; }
    }
}
