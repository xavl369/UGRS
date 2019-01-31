// file:	Tables\TimeMotor.cs
// summary:	Implements the time motor class

using SAPbobsCOM;
using System;
using UGRS.Core.SDK.Attributes;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.GPS.Tables
{
    /// <summary> A time motor. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Table(Name = "UG_TMEG", Description = "Time with engine turned on", Type = BoUTBTableType.bott_NoObjectAutoIncrement)]
    public class TimeEngine : Table
    {
      

        /// <summary> Gets or sets the name of the account. </summary>
        /// <value> The name of the account. </value>

        [Field(Description = "Account name", Size = 100)]
        public string AccountName { get; set; }

        /// <summary> Gets or sets the account number. </summary>
        /// <value> The account number. </value>

        [Field(Description = "Account number", Type = BoFieldTypes.db_Numeric, Size = 10)]
        public int AccountNumber { get; set; }

        /// <summary> Gets or sets the Date/Time of the date start. </summary>
        /// <value> The date start. </value>

        [Field(Description = "Date start", Type = BoFieldTypes.db_Date, Size = 10)]
        public DateTime DateStart { get; set; }

        /// <summary> Gets or sets the Date/Time of the date end. </summary>
        /// <value> The date end. </value>

        [Field(Description = "Date end", Type = BoFieldTypes.db_Date, Size = 10)]
        public DateTime DateEnd { get; set; }

        /// <summary> Gets or sets the name of the machine. </summary>
        /// <value> The name of the machine. </value>

        [Field(Description = "Machine name", Size = 50)]
        public string MachineName { get; set; }

        /// <summary> Gets or sets the pin or vin. </summary>
        /// <value> The pin or vin. </value>

        [Field(Description = "Pin or vin", Size = 50)]
        public string PinOrVin { get; set; }

        /// <summary> Gets or sets the identifier of the terminal. </summary>
        /// <value> The identifier of the terminal. </value>

        [Field(Description = "Terminal ID", Size = 50)]
        public string TerminalId { get; set; }

        /// <summary> Gets or sets the brand. </summary>
        /// <value> The brand. </value>

        [Field(Description = "Brand", Size = 30)]
        public string Brand { get; set; }

        /// <summary> Gets or sets the model. </summary>
        /// <value> The model. </value>

        [Field(Description = "Model", Size = 30)]
        public string Model { get; set; }

        /// <summary> Gets or sets the type of the machine. </summary>
        /// <value> The type of the machine. </value>

        [Field(Description = "Machine type", Size = 30)]
        public string MachineType { get; set; }

        /// <summary> Gets or sets the group the machine belongs to. </summary>
        /// <value> The machine group. </value>

        [Field(Description = "Machine group", Size = 30)]
        public string MachineGroup { get; set; }

        /// <summary> Gets or sets the other that owns this item. </summary>
        /// <value> The other owner. </value>

        [Field(Description = "Other owner", Size = 30)]
        public string OtherOwner { get; set; }

        /// <summary> Gets or sets the Date/Time of the last call. </summary>
        /// <value> The last call. </value>

        [Field(Description = "Last call", Type = BoFieldTypes.db_Date, Size = 10)]
        public DateTime LastCall { get; set; }

        /// <summary> Gets or sets the location. </summary>
        /// <value> The location. </value>

        [Field(Description = "Location", Size = 100)]
        public string Location { get; set; }

        /// <summary> Gets or sets the areas control. </summary>
        /// <value> The areas control. </value>

        [Field(Description = "Areas control", Size = 100)]
        public string AreasControl { get; set; }

        /// <summary> Gets or sets the period. </summary>
        /// <value> The period. </value>

        [Field(Description = "Engine hours of current period", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Period { get; set; }

        /// <summary> Gets or sets the last period. </summary>
        /// <value> The last period. </value>

        [Field(Description = "Engine hours of last known period ", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float LastPeriod { get; set; }

        /// <summary> Gets or sets the week 1. </summary>
        /// <value> The week 1. </value>

        [Field(Description = "Engine hours of week 1", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Week1 { get; set; }

        /// <summary> Gets or sets the week 2. </summary>
        /// <value> The week 2. </value>

        [Field(Description = "Engine hours of week 2", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Week2 { get; set; }

        /// <summary> Gets or sets the week 3. </summary>
        /// <value> The week 3. </value>

        [Field(Description = "Engine hours of week 3", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Week3 { get; set; }

        /// <summary> Gets or sets the week 4. </summary>
        /// <value> The week 4. </value>

        [Field(Description = "Engine hours of week 4", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Week4 { get; set; }

        /// <summary> Gets or sets the week 5. </summary>
        /// <value> The week 5. </value>

        [Field(Description = "Engine hours of week 5", Type = BoFieldTypes.db_Float, SubType = BoFldSubTypes.st_Quantity, Size = 10)]
        public float Week5 { get; set; }
    }
}
