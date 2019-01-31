// file:	Tables\PortRequest.cs
// summary:	Implements the port request class

using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.Tables
{
    /// <summary> A port request. </summary>
    /// <remarks> Ranaya, 19/05/2017. </remarks>

    [Table(Name = "UG_PE_WS_PORE", Description = "PE WS Solicitud puertos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class PortRequest : Table
    {
        /// <summary> Gets or sets the identifier of the request. </summary>
        /// <value> The identifier of the request. </value>

        [Field(Description = "Request id", Size = 64)]
        public string RequestId { get; set; }

        /// <summary> Gets or sets the identifier of the port. </summary>
        /// <value> The identifier of the port. </value>

        [Field(Description = "Port id", Type = BoFieldTypes.db_Numeric)]
        public int PortId { get; set; }

        /// <summary> Gets or sets the identifier of the inspection point. </summary>
        /// <value> The identifier of the inspection point. </value>

        [Field(Description = "Inspection point id", Type = BoFieldTypes.db_Numeric)]
        public int InspectionPointId { get; set; }

        /// <summary> Gets or sets the inspection point. </summary>
        /// <value> The inspection point. </value>

        [Field(Description = "Inspection point", Size = 250)]
        public string InspectionPoint { get; set; }

        /// <summary> Gets or sets the type of the port. </summary>
        /// <value> The type of the port. </value>

        [Field(Description = "Port type", Type = BoFieldTypes.db_Numeric)]
        public int PortType { get; set; }
    }
}
