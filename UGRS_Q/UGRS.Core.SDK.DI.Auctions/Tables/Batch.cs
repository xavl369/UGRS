// file:	tables\batch.cs
// summary:	Implements the batch class

using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Auctions.Tables
{
    /// <summary> The batches table. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    [Table(Name = "UG_SU_BAHS", Description = "SU Lotes", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class Batch : Table
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>

        [Field(Description = "Id", Type = BoFieldTypes.db_Numeric)]
        public long Id { get; set; }

        /// <summary> Gets or sets the identifier of the auction. </summary>
        /// <value> The identifier of the auction. </value>

        [Field(Description = "Auction id", Type = BoFieldTypes.db_Numeric)]
        public long AuctionId { get; set; }

        /// <summary> Gets or sets the number of. </summary>
        /// <value> The number. </value>

        [Field(Description = "Number of batch", Type = BoFieldTypes.db_Numeric)]
        public int Number { get; set; }

        /// <summary> Gets or sets the identifier of the seller. </summary>
        /// <value> The identifier of the seller. </value>

        [Field(Description = "Seller id", Type = BoFieldTypes.db_Numeric)]
        public long SellerId { get; set; }

        /// <summary> Gets or sets the seller code. </summary>
        /// <value> The seller. </value>

        [Field(Description = "Seller card code", Size = 50)]
        public string SellerCode { get; set; }

        /// <summary> Gets or sets the seller. </summary>
        /// <value> The seller. </value>

        [Field(Description = "Seller card name", Size = 200)]
        public string Seller { get; set; }

        /// <summary> Gets or sets the identifier of the buyer. </summary>
        /// <value> The identifier of the buyer. </value>

        [Field(Description = "Buyer id", Type = BoFieldTypes.db_Numeric)]
        public long BuyerId { get; set; }

        /// <summary> Gets or sets the buyer code. </summary>
        /// <value> The buyer. </value>

        [Field(Description = "Buyer card code", Size = 200)]
        public string BuyerCode { get; set; }

        /// <summary> Gets or sets the buyer. </summary>
        /// <value> The buyer. </value>

        [Field(Description = "Buyer card name", Size = 200)]
        public string Buyer { get; set; }

        /// <summary> Gets or sets the code of the item type. </summary>
        /// <value> The identifier of the item type. </value>

        [Field(Description = "Item type id", Type = BoFieldTypes.db_Numeric)]
        public long ItemTypeId { get; set; }

        /// <summary> Gets or sets the type of the item. </summary>
        /// <value> The type of the item. </value>

        [Field(Description = "Item type name", Size = 200)]
        public string ItemType { get; set; }

        /// <summary> Gets or sets the quantity. </summary>
        /// <value> The quantity. </value>

        [Field(Description = "Quantity", Type = BoFieldTypes.db_Numeric)]
        public int Quantity { get; set; }

        /// <summary> Gets or sets the returned. </summary>
        /// <value> The returned. </value>

        [Field(Description = "Returned quantity", Type = BoFieldTypes.db_Numeric)]
        public int Returned { get; set; }

        /// <summary> Gets or sets the weight. </summary>
        /// <value> The weight. </value>

        [Field(Description = "Weight ", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Measurement)]
        public float Weight { get; set; }

        /// <summary> Gets or sets the average weight. </summary>
        /// <value> The average weight. </value>

        [Field(Description = "AverageWeight ", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity)]
        public float AverageWeight { get; set; }

        /// <summary> Gets or sets the price. </summary>
        /// <value> The price. </value>

        [Field(Description = "Price ", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public decimal Price { get; set; }

        /// <summary> Gets or sets the amount. </summary>
        /// <value> The amount. </value>

        [Field(Description = "Amount ", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Sum)]
        public decimal Amount { get; set; }

        /// <summary> Gets or sets the unsold. </summary>
        /// <value> The unsold. </value>

        [Field(Description = "Unsold ", Size = 1)]
        public bool Unsold { get; set; }

        /// <summary> Gets or sets the identifier of the unsold motive. </summary>
        /// <value> The identifier of the unsold motive. </value>

        [Field(Description = "Unsold motive id", Type = BoFieldTypes.db_Numeric)]
        public int UnsoldMotiveId { get; set; }

        /// <summary> Gets or sets the gender. </summary>
        /// <value> The identifier of the unsold motive. </value>

        [Field(Description = "Gender", Size = 20)]
        public string Gender { get; set; }


        /// <summary> Gets or sets the type of the item. </summary>
        /// <value> The type of the item. </value>

        [Field(Description = "Unsold motive name", Size = 200)]
        public string UnsoldMotive { get; set; }

        /// <summary> Gets or sets if item is reprogrammed. </summary>
        /// <value> The type of the item. </value>

        [Field(Description = "Reprogrammed", Size = 1)]
        public bool Reprogrammed { get; set; }

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
