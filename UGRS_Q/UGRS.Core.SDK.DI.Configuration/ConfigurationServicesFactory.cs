using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.Configuration.Services;

namespace UGRS.Core.SDK.DI.Configuration
{
    public class ConfigurationServicesFactory
    {
        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public ConfigurationService GetConfigurationService()
        {
            return new ConfigurationService();
        }
    }
}
