using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Services.System;
using UGRS.Data.Auctions.DAO.Base;

namespace UGRS.Data.Auctions.Factories
{
    public class SystemServicesFactory
    {
        public ModuleService GetModuleService()
        {
            return new ModuleService(new BaseDAO<Module>());
        }

        public SectionService GetSectionService()
        {
            return new SectionService(new BaseDAO<Section>());
        }

        public ConfigurationService GetConfigurationService()
        {
            return new ConfigurationService(new BaseDAO<Configuration>());
        }
    }
}
