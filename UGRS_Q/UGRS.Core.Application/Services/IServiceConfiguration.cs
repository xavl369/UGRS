using System.Windows;

namespace UGRS.Application.ServiceManager.Services
{
    public interface IServiceConfiguration
    {
        bool? ShowConfiguration(Window pFrmWindows);
    }
}
