// file:	Models\IField.cs
// summary:	Declares the IField interface

using SAPbobsCOM;
using UGRS.Core.SDK.Attributes;

namespace UGRS.Core.SDK.DI.Models
{
    /// <summary> Interface for field. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public interface IField
    {
        /// <summary> Gets the attributes. </summary>
        /// <returns> The attributes. </returns>

        FieldAttribute GetAttributes();

        /// <summary> Gets user field. </summary>
        /// <returns> The user field. </returns>

        UserFieldsMD GetUserField();
    }
}
