// file:	Tables\ParameterRequest.cs
// summary:	Implements the parameter request class

using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.Tables
{
    /// <summary> A parameter request. </summary>
    /// <remarks> Ranaya, 19/05/2017. </remarks>

    [Table(Name = "UG_PE_WS_PARE", Description = "PE WS Solicitud parámetros", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class ParameterRequest : Table
    {
        /// <summary> Gets or sets the identifier of the request. </summary>
        /// <value> The identifier of the request. </value>

        [Field(Description = "Request id", Size = 64)]
        public string RequestId { get; set; }

        /// <summary> Gets or sets the identifier of the product. </summary>
        /// <value> The identifier of the product. </value>

        [Field(Description = "Product id", Type = BoFieldTypes.db_Numeric)]
        public int ProductId { get; set; }

        /// <summary> Gets or sets the identifier of the product request. </summary>
        /// <value> The identifier of the product request. </value>

        [Field(Description = "Product request id", Type = BoFieldTypes.db_Numeric)]
        public int ProductRequestId { get; set; }

        /// <summary> Gets or sets the identifier of the parameter. </summary>
        /// <value> The identifier of the parameter. </value>

        [Field(Description = "Parameter id", Type = BoFieldTypes.db_Numeric)]
        public int ParameterId { get; set; }

        /// <summary> Gets or sets the parameter. </summary>
        /// <value> The parameter. </value>

        [Field(Description = "Parameter", Size = 50)]
        public string Parameter { get; set; }

        /// <summary> Gets or sets the type of the parameter. </summary>
        /// <value> The type of the parameter. </value>

        [Field(Description = "Parameter type", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Rate)]
        public decimal ParameterType { get; set; }

        /// <summary> Gets or sets the printable. </summary>
        /// <value> The printable. </value>

        [Field(Description = "Printable", Type = BoFieldTypes.db_Numeric)]
        public int Printable { get; set; }

        /// <summary> Gets or sets the price. </summary>
        /// <value> The price. </value>

        [Field(Description = "Price", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public decimal Price { get; set; }

        /// <summary> Gets or sets the cost permission. </summary>
        /// <value> The cost permission. </value>

        [Field(Description = "Cost per permission", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Price)]
        public decimal CostPermission { get; set; }

        /// <summary> Gets or sets the identifier of the value. </summary>
        /// <value> The identifier of the value. </value>

        [Field(Description = "Value id", Type = BoFieldTypes.db_Numeric)]
        public int ValueId { get; set; }

        /// <summary> Gets or sets the value. </summary>
        /// <value> The value. </value>

        [Field(Description = "Value", Size = 50)]
        public string Value { get; set; }

        /// <summary> Gets or sets the identifier of the sub value. </summary>
        /// <value> The identifier of the sub value. </value>

        [Field(Description = "SubValue id", Type = BoFieldTypes.db_Numeric)]
        public int SubValueId { get; set; }

        /// <summary> Gets or sets the sub value. </summary>
        /// <value> The sub value. </value>

        [Field(Description = "SubValue", Size = 50)]
        public string SubValue { get; set; }
    }
}
