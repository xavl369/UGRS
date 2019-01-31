
namespace UGRS.Object.Boards.Services
{
    public interface ISerialPortService
    {
        string GetName();

        void Open();

        void Close();

        void Write(string pStrValue);

        bool IsOpen();
    }
}
