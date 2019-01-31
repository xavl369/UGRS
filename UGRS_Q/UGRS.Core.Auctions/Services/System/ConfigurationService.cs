using System;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Services.System
{
    public class ConfigurationService
    {
        private IBaseDAO<Configuration> mObjConfigurationDAO;

        public ConfigurationService(IBaseDAO<Configuration> pObjConfigurationDAO)
        {
            mObjConfigurationDAO = pObjConfigurationDAO;
        }

        public string GetByKey(ConfigurationKeyEnum pEnmKey)
        {
            return mObjConfigurationDAO.GetEntitiesList().Where(x => x.Key == pEnmKey).Count() > 0 ?
                   mObjConfigurationDAO.GetEntitiesList().FirstOrDefault(x => x.Key == pEnmKey).Value : string.Empty;
        }

        public IQueryable<Configuration> GetList()
        {
            return mObjConfigurationDAO.GetEntitiesList();
        }

        public void SaveOrUpdate(Configuration pObjConfiguration)
        {
            if (!Exists(pObjConfiguration))
            {
                mObjConfigurationDAO.SaveOrUpdateEntity(pObjConfiguration);
            }
            else
            {
                throw new Exception("Registro existente.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjConfigurationDAO.RemoveEntity(pLonId);
        }

        private bool Exists(Configuration pObjConfiguration)
        {
            return mObjConfigurationDAO.GetEntitiesList().Where(x => x.Key == pObjConfiguration.Key && x.Id != pObjConfiguration.Id).Count() > 0 ? true : false;
        }
    }
}
