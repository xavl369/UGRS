// file:	Tables\Auction.cs
// summary:	Implements the auction class

using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Auctions.Tables
{
    /// <summary> The auctions table. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    [Table(Name = "UG_SU_AUTN", Description = "SU Subastas", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class Auction : Table
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>

        [Field(Description = "Id", Type = BoFieldTypes.db_Numeric)]
        public long Id { get; set; }

        /// <summary> Gets or sets the identifier of the location. </summary>
        /// <value> The identifier of the location. </value>

        [Field(Description = "Location id", Type = BoFieldTypes.db_Numeric)]
        public long LocationId { get; set; }

        /// <summary> Gets or sets the location. </summary>
        /// <value> The location. </value>

        [Field(Description = "Location name", Size = 100)]
        public string Location { get; set; }

        /// <summary> Gets or sets the folio. </summary>
        /// <value> The folio. </value>

        [Field(Description = "Folio", Size = 20)]
        public string Folio { get; set; }

        /// <summary> Gets or sets the identifier of the type. </summary>
        /// <value> The identifier of the type. </value>

        [Field(Description = "Type id", Type = BoFieldTypes.db_Numeric)]
        public long TypeId { get; set; }

        /// <summary> Gets or sets the type. </summary>
        /// <value> The type. </value>

        [Field(Description = "Type name", Size = 100)]
        public string Type { get; set; }

        /// <summary> Gets or sets the identifier of the category. </summary>
        /// <value> The identifier of the category. </value>

        [Field(Description = "Category id", Type = BoFieldTypes.db_Numeric)]
        public long CategoryId { get; set; }

        /// <summary> Gets or sets the category. </summary>
        /// <value> The category. </value>

        [Field(Description = "Category name", Size = 100)]
        public string Category { get; set; }

        /// <summary> Gets or sets the commission. </summary>
        /// <value> The commission. </value>

        [Field(Description = "Commission", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Rate )]
        public float Commission { get; set; }

        /// <summary> Gets or sets the Date/Time of the date. </summary>
        /// <value> The date. </value>

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime Date { get; set; }

        [Field(Description = "Opened", Size = 1)]
        public bool Opened { get; set; }

        [Field(Description = "Autorizado Corrales", Size = 1)]
        public bool AutCorral { get; set; }

        [Field(Description = "Autorizado Transporte", Size = 1)]
        public bool AutTransp { get; set; }

        [Field(Description = "Autorizado Subasta", Size = 1)]
        public bool AutAuction { get; set; }

        [Field(Description = "AutorizaCyC", Size = 1)]
        public bool AutCyC { get; set; }

        [Field(Description = "Creación de Cheques", Size = 1)]
        public bool AutFz { get; set; }

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
