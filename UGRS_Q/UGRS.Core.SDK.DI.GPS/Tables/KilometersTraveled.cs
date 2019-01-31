// file:	Tables\KilometersTraveled.cs
// summary:	Implements the kilometers traveled class

using System;
using UGRS.Core.SDK.Attributes;
using SAPbobsCOM;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.GPS.Tables
{
    /// <summary> The kilometers traveled. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Table(Name="UG_KMTL", Description="Kilometers traveled", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class KilometersTraveled : Table
    {
    

        [Field(Description = "Vehicle name", Size = 100)]
        public string Name { get; set; }

        /// <summary> Gets or sets from date. </summary>
        /// <value> from date. </value>

        [Field(Description = "From date", Type = BoFieldTypes.db_Date, Size = 10)]
        public DateTime FromDate { get; set; }

        /// <summary> Gets or sets to date. </summary>
        /// <value> to date. </value>

        [Field(Description = "To date", Type = BoFieldTypes.db_Date)]
        public DateTime ToDate { get; set; }

        /// <summary> Gets or sets origin address. </summary>
        /// <value> from address. </value>

        [Field(Description = "Origin Address", Size = 200)]
        public string FromAddress { get; set; }

        /// <summary> Gets or sets destination address. </summary>
        /// <value> to address. </value>

        [Field(Description = "Destination Address", Size = 200)]
        public string ToAddress { get; set; }

        /// <summary> Gets or sets the distance. </summary>
        /// <value> The distance. </value>

        [Field(Description = "Distance traveled", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Distance { get; set; }

        /// <summary> Gets or sets the duration. </summary>
        /// <value> The duration. </value>

        [Field(Description = "Duration", Size = 100)]
        public string Duration { get; set; }

        /// <summary> Gets or sets the maximum velocity. </summary>
        /// <value> The maximum velocity. </value>

        [Field(Description = "MaxVelocity", Size = 30)]
        public string MaxVelocity { get; set; }

        /// <summary> Gets or sets the odometer start. </summary>
        /// <value> The odometer start. </value>

        [Field(Description = "OdometerStart", Type=BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float OdometerStart { get; set; }

        /// <summary> Gets or sets the odometer end. </summary>
        /// <value> The odometer end. </value>

        [Field(Description = "OdometerStart", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float OdometerEnd { get; set; }

        /// <summary> Gets or sets the motor hours. </summary>
        /// <value> The motor hours. </value>

        [Field(Description = "Distance traveled", Type = BoFieldTypes.db_Numeric, Size = 10)]
        public int MotorHours { get; set; }
    }
}
