using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.AddOn.AccountingAccounts.Tables
{
    [Table(Name = "UG_AA_LOGG", Description = "AA Registro de Carga", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class AccountingAccountsLogg : Table
    {
        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime Date { get; set; }

        [Field(Description = "File", Size = 100)]
        public string File { get; set; }

        [Field(Description = "Rows", Type = BoFieldTypes.db_Numeric)]
        public int Rows { get; set; }

        [Field(Description = "Loaded", Type = BoFieldTypes.db_Numeric)]
        public int Loaded { get; set; }

        [Field(Description = "Error", Size = 1)]//Y,N
        public bool Error { get; set; }

        [Field(Description = "User", Size = 25)]
        public string User { get; set; }
    }
}
