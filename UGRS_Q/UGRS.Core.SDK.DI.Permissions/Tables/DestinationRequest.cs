// file:	Tables\DestinationRequest.cs
// summary:	Implements the destination request class

using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.Tables
{
    /// <summary> A destination request. </summary>
    /// <remarks> Ranaya, 19/05/2017. </remarks>

    [Table(Name = "UG_PE_WS_DERE", Description = "PE WS Solicitud destinos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class DestinationRequest : Table
    {
        /// <summary> Gets or sets the identifier of the request. </summary>
        /// <value> The identifier of the request. </value>

        [Field(Description = "Request id", Size = 64)]
        public string RequestId { get; set; }

        /// <summary> Gets or sets the identifier of the state. </summary>
        /// <value> The identifier of the state. </value>

        [Field(Description = "State id", Type = BoFieldTypes.db_Numeric)]
        public int StateId { get; set; }

        /// <summary> Gets or sets the state. </summary>
        /// <value> The state. </value>

        [Field(Description = "State", Size = 100)]
        public string State { get; set; }

        /// <summary> Gets or sets the identifier of the city. </summary>
        /// <value> The identifier of the city. </value>

        [Field(Description = "City id", Type = BoFieldTypes.db_Numeric)]
        public int CityId { get; set; }

        /// <summary> Gets or sets the city. </summary>
        /// <value> The city. </value>

        [Field(Description = "City", Size = 100)]
        public string City { get; set; }

        /// <summary> Gets or sets the location. </summary>
        /// <value> The location. </value>

        [Field(Description = "Location", Size = 100)]
        public string Location { get; set; }

        /// <summary> Gets or sets the identifier of the inspection point. </summary>
        /// <value> The identifier of the inspection point. </value>

        [Field(Description = "Inspection point id", Type = BoFieldTypes.db_Numeric)]
        public int InspectionPointId { get; set; }

        /// <summary> Gets or sets the inspection point. </summary>
        /// <value> The inspection point. </value>

        [Field(Description = "Inspection point", Size = 150)]
        public string InspectionPoint { get; set; }

        /// <summary> Gets or sets the identifier of the product. </summary>
        /// <value> The identifier of the product. </value>

        [Field(Description = "Product id", Type = BoFieldTypes.db_Numeric)]
        public int ProductId { get; set; }
    }
}
