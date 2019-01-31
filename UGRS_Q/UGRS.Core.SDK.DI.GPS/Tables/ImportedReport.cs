// file:	Tables\ImportedReport.cs
// summary:	Implements the imported report class

using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.GPS.Tables
{
    /// <summary> An imported report. </summary>
    /// <remarks> Ranaya, 08/05/2017. </remarks>

    [Table(Name = "UG_IRPT", Description = "Imported reported", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class ImportedReport : Table
    {
        /// <summary> Gets or sets the filename of the file. </summary>
        /// <value> The name of the file. </value>

        [Field(Description = "File Name", Type = BoFieldTypes.db_Memo, Size = 300)]
        public string FileName { get; set; }

        /// <summary> Gets or sets the Date/Time of the date. </summary>
        /// <value> The date. </value>

        [Field(Description = "Import date", Type = BoFieldTypes.db_Date, Size = 10)]
        public DateTime Date { get; set; }
    }
}
