// file:	Tables\PermissionRequest.cs
// summary:	Implements the permission request class

using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Permissions.Tables
{
    /// <summary> A permission request. </summary>
    /// <remarks> Ranaya, 19/05/2017. </remarks>

    [Table(Name = "UG_PE_WS_PERE", Description = "PE WS Solicitud permisos", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class PermissionRequest : Table
    {
        /// <summary> Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>

        [Field(Description = "Permission request id", Size = 64)]
        public string RequestId { get; set; }

        /// <summary> Gets or sets the printed folio. </summary>
        /// <value> The printed folio. </value>

        [Field(Description = "Printed folio", Size = 50)]
        public string PrintedFolio { get; set; }

        /// <summary> Gets or sets the identifier of the mobilization type. </summary>
        /// <value> The identifier of the mobilization type. </value>

        [Field(Description = "Mobilization type id", Type = BoFieldTypes.db_Numeric)]
        public int MobilizationTypeId { get; set; }

        /// <summary> Gets or sets the type of the mobilization. </summary>
        /// <value> The type of the mobilization. </value>

        [Field(Description = "Mobilization type", Size = 100)]
        public string MobilizationType { get; set; }

        /// <summary> Gets or sets the Date/Time of the date. </summary>
        /// <value> The date. </value>

        [Field(Description = "Date", Type = BoFieldTypes.db_Date)]
        public DateTime Date { get; set; }

        /// <summary> Gets or sets the duration days. </summary>
        /// <value> The duration days. </value>

        [Field(Description = "Duration days", Type = BoFieldTypes.db_Numeric)]
        public int DurationDays { get; set; }

        /// <summary> Gets or sets the validation start date. </summary>
        /// <value> The validation start date. </value>

        [Field(Description = "Validation start date", Type = BoFieldTypes.db_Date)]
        public DateTime ValidationStartDate { get; set; }

        /// <summary> Gets or sets the validation end date. </summary>
        /// <value> The validation end date. </value>

        [Field(Description = "Validation end date", Type = BoFieldTypes.db_Date)]
        public DateTime ValidationEndDate { get; set; }

        /// <summary> Gets or sets the destinations modality. </summary>
        /// <value> The destinations modality. </value>

        [Field(Description = "Destinations modality", Type = BoFieldTypes.db_Numeric)]
        public int DestinationsModality { get; set; }

        /// <summary> Gets or sets the identifier of the productive group. </summary>
        /// <value> The identifier of the productive group. </value>

        [Field(Description = "Productive group id", Size = 32)]
        public string ProductiveGroupId { get; set; }

        /// <summary> Gets or sets the group the productive belongs to. </summary>
        /// <value> The productive group. </value>

        [Field(Description = "Productive group", Size = 100)]
        public string ProductiveGroup { get; set; }

        /// <summary> Gets or sets the identifier of the producer. </summary>
        /// <value> The identifier of the producer. </value>

        [Field(Description = "Producer id", Type = BoFieldTypes.db_Numeric)]
        public int ProducerId { get; set; }

        /// <summary> Gets or sets the producer. </summary>
        /// <value> The producer. </value>

        [Field(Description = "Producer", Size = 100)]
        public string Producer { get; set; }

        /// <summary> Gets or sets the identifier of the administrative phase user. </summary>
        /// <value> The identifier of the administrative phase user. </value>

        [Field(Description = "Administrative phase user id", Type = BoFieldTypes.db_Numeric)]
        public int AdministrativePhaseUserId { get; set; }

        /// <summary> Gets or sets the identifier of the user signature. </summary>
        /// <value> The identifier of the user signature. </value>

        [Field(Description = "User signature id", Type = BoFieldTypes.db_Numeric)]
        public int UserSignatureId { get; set; }

        /// <summary> Gets or sets the producer address. </summary>
        /// <value> The producer address. </value>

        [Field(Description = "Producer address", Size = 250)]
        public string ProducerAddress { get; set; }

        /// <summary> Gets or sets the producer telephone. </summary>
        /// <value> The producer telephone. </value>

        [Field(Description = "Producer telephone", Size = 25)]
        public string ProducerTelephone { get; set; }

        /// <summary> Gets or sets the producer email address. </summary>
        /// <value> The producer email address. </value>

        [Field(Description = "Producer email address", Size = 100)]
        public string ProducerEmailAddress { get; set; }

        /// <summary> Gets or sets the state of the producer. </summary>
        /// <value> The producer state. </value>

        [Field(Description = "Producer state", Size = 100)]
        public string ProducerState { get; set; }

        /// <summary> Gets or sets the producer city. </summary>
        /// <value> The producer city. </value>

        [Field(Description = "Producer city", Size = 100)]
        public string ProducerCity { get; set; }

        /// <summary> Gets or sets the identifier of the origin state. </summary>
        /// <value> The identifier of the origin state. </value>

        [Field(Description = "Origin state id", Type = BoFieldTypes.db_Numeric)]
        public int OriginStateId { get; set; }

        /// <summary> Gets or sets the state of the origin. </summary>
        /// <value> The origin state. </value>

        [Field(Description = "Origin state", Size = 100)]
        public string OriginState { get; set; }

        /// <summary> Gets or sets the identifier of the origin city. </summary>
        /// <value> The identifier of the origin city. </value>

        [Field(Description = "Origin city id", Type = BoFieldTypes.db_Numeric)]
        public int OriginCityId { get; set; }

        /// <summary> Gets or sets the origin city. </summary>
        /// <value> The origin city. </value>

        [Field(Description = "Origin city", Size = 100)]
        public string OriginCity { get; set; }

        /// <summary> Gets or sets the location 1. </summary>
        /// <value> The location 1. </value>

        [Field(Description = "Location1", Size = 50)]
        public string Location1 { get; set; }

        /// <summary> Gets or sets the location 2. </summary>
        /// <value> The location 2. </value>

        [Field(Description = "Location2", Size = 50)]
        public string Location2 { get; set; }

        /// <summary> Gets or sets the entry port. </summary>
        /// <value> The entry port. </value>

        [Field(Description = "Entry port", Type = BoFieldTypes.db_Numeric)]
        public int EntryPort { get; set; }

        /// <summary> Gets or sets the departure port. </summary>
        /// <value> The departure port. </value>

        [Field(Description = "Departure port", Type = BoFieldTypes.db_Numeric)]
        public int DeparturePort { get; set; }

        /// <summary> Gets or sets the transport. </summary>
        /// <value> The transport. </value>

        [Field(Description = "Transport", Type = BoFieldTypes.db_Numeric)]
        public int Transport { get; set; }

        /// <summary> Gets or sets the representative. </summary>
        /// <value> The representative. </value>

        [Field(Description = "Representative", Size = 100)]
        public string Representative { get; set; }

        /// <summary> Gets or sets the sagarpa status. </summary>
        /// <value> The sagarpa status. </value>

        [Field(Description = "Sagarpa Status", Type = BoFieldTypes.db_Numeric)]
        public int SagarpaStatus { get; set; }

        /// <summary> Gets or sets the health status. </summary>
        /// <value> The health status. </value>

        [Field(Description = "Health Status", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Rate)]
        public decimal HealthStatus { get; set; }

        /// <summary> Gets or sets the web service request status. </summary>
        /// <value> The web service request status. </value>

        [Field(Description = "Web service request status", Type = BoFieldTypes.db_Numeric)]
        public int WebServiceRequestStatus { get; set; }

        /// <summary> Gets or sets the deleted. </summary>
        /// <value> The deleted. </value>

        [Field(Description = "Deleted", Type = BoFieldTypes.db_Numeric)]
        public int Deleted { get; set; }

        /// <summary> Gets or sets the identifier of the health user signature. </summary>
        /// <value> The identifier of the health user signature. </value>

        [Field(Description = "Health user signature id", Type = BoFieldTypes.db_Numeric)]
        public int HealthUserSignatureId { get; set; }

        /// <summary> Gets or sets the signature iqr. </summary>
        /// <value> The signature iqr. </value>

        [Field(Description = "Signature iqr", Size = 200)]
        public string SignatureIqr { get; set; }

        /// <summary> Gets or sets the signature. </summary>
        /// <value> The signature. </value>

        [Field(Description = "Signature", Size = 150)]
        public string Signature { get; set; }

        /// <summary> Gets or sets the real signature. </summary>
        /// <value> The real signature. </value>

        [Field(Description = "Real signature", Size = 100)]
        public string RealSignature { get; set; }

        /// <summary> Gets or sets the health signature date. </summary>
        /// <value> The health signature date. </value>

        [Field(Description = "Health signature date", Type = BoFieldTypes.db_Date)]
        public DateTime HealthSignatureDate { get; set; }

        /// <summary> Gets or sets the approval rejection reason. </summary>
        /// <value> The approval rejection reason. </value>

        [Field(Description = "Approval rejection reason", Type = BoFieldTypes.db_Memo, Size = 300)]
        public string ApprovalRejectionReason { get; set; }

        /// <summary> Gets or sets the remarks. </summary>
        /// <value> The remarks. </value>

        [Field(Description = "Remarks", Size = 250)]
        public string Remarks { get; set; }

        /// <summary> Gets or sets the customs. </summary>
        /// <value> The customs. </value>

        [Field(Description = "Customs", Size = 250)]
        public string Customs { get; set; }

        /// <summary> Gets or sets the Destination for the. </summary>
        /// <value> The destination. </value>

        [Field(Description = "Destination", Size = 250)]
        public string Destination { get; set; }

        /// <summary> Gets or sets the customer code. </summary>
        /// <value> The customer code. </value>

        [Field(Description = "Customer code", Size = 250)]
        public string CustomerCode {get; set;}

        /// <summary> Gets or sets the crossing date. </summary>
        /// <value> The crossing date. </value>

        [Field(Description = "Crossing date", Type = BoFieldTypes.db_Date)]
        public DateTime CrossingDate { get; set; }

        /// <summary> Gets or sets the ugrs folio. </summary>
        /// <value> The ugrs folio. </value>

        [Field(Description = "UGRS folio", Type = BoFieldTypes.db_Numeric)]
        public int UgrsFolio { get; set; }

        /// <summary> Gets or sets the ugrs request. </summary>
        /// <value> The ugrs request. </value>

        [Field(Description = "UGRS request", Size = 10)]
        public string UgrsRequest { get; set; }
    }
}
