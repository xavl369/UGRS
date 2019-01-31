// file:	Tables\ProductRequest.cs
// summary:	Implements the product request class

using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.Tables
{
    /// <summary> A product request. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    [Table(Name = "UG_PE_WS_PRRE", Description = "PE WS Solicitud productos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class ProductRequest : Table
    {
        /// <summary> Gets or sets the identifier of the request. </summary>
        /// <value> The identifier of the request. </value>

        [Field(Description = "Request id", Size = 64)]
        public string RequestId { get; set; }

        /// <summary> Gets or sets the identifier of the product. </summary>
        /// <value> The identifier of the product. </value>

        [Field(Description = "Product id", Type = BoFieldTypes.db_Numeric)]
        public int ProductId { get; set; }

        /// <summary> Gets or sets the identifier of the parent product. </summary>
        /// <value> The identifier of the parent product. </value>

        [Field(Description = "Parent product id", Type = BoFieldTypes.db_Numeric)]
        public int ParentProductId { get; set; }

        /// <summary> Gets or sets the product. </summary>
        /// <value> The product. </value>

        [Field(Description = "Product", Size = 100)]
        public string Product { get; set; }

        /// <summary> Gets or sets the quantity. </summary>
        /// <value> The quantity. </value>

        [Field(Description = "Quantity", Type = BoFieldTypes.db_Numeric)]
        public int Quantity { get; set; }

        /// <summary> Gets or sets the authorized quantity. </summary>
        /// <value> The authorized quantity. </value>

        [Field(Description = "Authorized quantity", Type = BoFieldTypes.db_Numeric)]
        public int AuthorizedQuantity { get; set; }

        /// <summary> Gets or sets the identifier of the mobilization goal. </summary>
        /// <value> The identifier of the mobilization goal. </value>

        [Field(Description = "Mobilization goal id", Type = BoFieldTypes.db_Numeric)]
        public int MobilizationGoalId { get; set; }

        /// <summary> Gets or sets the mobilization goal. </summary>
        /// <value> The mobilization goal. </value>

        [Field(Description = "Mobilization goal", Size = 50)]
        public string MobilizationGoal { get; set; }

        /// <summary> Gets or sets the printable mobilization goal. </summary>
        /// <value> The printable mobilization goal. </value>

        [Field(Description = "Printable mobilization goal", Type = BoFieldTypes.db_Numeric)]
        public int PrintableMobilizationGoal { get; set; }

        /// <summary> Gets or sets the description. </summary>
        /// <value> The description. </value>

        [Field(Description = "Description", Size = 250)]
        public string Description { get; set; }

        /// <summary> Gets or sets the state destination. </summary>
        /// <value> The state destination. </value>

        [Field(Description = "State destination", Type = BoFieldTypes.db_Numeric)]
        public int StateDestination { get; set; }

        /// <summary> Gets or sets the city destination. </summary>
        /// <value> The city destination. </value>

        [Field(Description = "City destination", Type = BoFieldTypes.db_Numeric)]
        public int CityDestination { get; set; }

        /// <summary> Gets or sets the location. </summary>
        /// <value> The location. </value>

        [Field(Description = "Location", Size = 100)]
        public string Location { get; set; }
    }
}
