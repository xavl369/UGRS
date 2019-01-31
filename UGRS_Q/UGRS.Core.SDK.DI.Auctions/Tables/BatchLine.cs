using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Auctions.Tables
{
    [Table(Name = "UG_SU_BALN", Description = "SU Lineas de lote", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class BatchLine : Table
    {
        [Field(Description = "Id", Type = BoFieldTypes.db_Numeric)]
        public long Id { get; set; }

        [Field(Description = "Batch id", Type = BoFieldTypes.db_Numeric)]
        public long BatchId { get; set; }

        [Field(Description = "Item id", Type = BoFieldTypes.db_Numeric)]
        public long ItemId { get; set; }

        [Field(Description = "Item name", Size = 200)]
        public string Item { get; set; }

        [Field(Description = "Quantity", Type = BoFieldTypes.db_Numeric)]
        public int Quantity { get; set; }

        [Field(Description = "Returned quantity", Type = BoFieldTypes.db_Numeric)]
        public int Returned { get; set; }

        #region System attributes

        [Field(Description = "Protected", Size = 1)]
        public bool Protected { get; set; }

        [Field(Description = "Removed", Size = 1)]
        public bool Removed { get; set; }

        [Field(Description = "Active", Size = 1)]
        public bool Active { get; set; }

        [Field(Description = "Creation date", Type = BoFieldTypes.db_Date)]
        public DateTime CreationDate { get; set; }

        [Field(Description = "Creation time", Type = BoFieldTypes.db_Date, SubType = BoFldSubTypes.st_Time)]
        public DateTime CreationTime { get; set; }

        [Field(Description = "Modification date", Type = BoFieldTypes.db_Date)]
        public DateTime ModificationDate { get; set; }

        [Field(Description = "Modification time", Type = BoFieldTypes.db_Date, SubType = BoFldSubTypes.st_Time)]
        public DateTime ModificationTime { get; set; }

        #endregion
    }
}
