// file:	Connection\IConnection.cs
// summary:	Declares the IConnection interface

namespace UGRS.Core.SDK.Connection
{
    /// <summary> Interface for connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public interface IConnection
    {
        string GetConnectionString();
    }
}
