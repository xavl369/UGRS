using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.System;

namespace UGRS.Core.Auctions.Services.System
{
    public class ModuleService
    {
        private IBaseDAO<Module> mObjModuleDAO;

        public ModuleService(IBaseDAO<Module> pObjModuleDAO)
        {
            mObjModuleDAO = pObjModuleDAO;
        }

        public IQueryable<Module> GetList()
        {
            return mObjModuleDAO.GetEntitiesList().OrderBy(x=> x.Position);
        }

        public void SaveOrUpdate(Module pObjModule)
        {
            if (!Exists(pObjModule))
            {
                if (pObjModule.Id == 0)
                {
                    pObjModule.Position = GetNextPosition();
                    mObjModuleDAO.AddEntity(pObjModule);
                }
                else
                {
                    pObjModule.Position = GetCurrentPosition(pObjModule.Position);
                    mObjModuleDAO.UpdateEntity(pObjModule, pObjModule.Id);
                }
            }
            else
            {
                throw new Exception("Registro existente.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjModuleDAO.RemoveEntity(pLonId);
        }

        public int GetNextPosition()
        {
            return mObjModuleDAO.GetEntitiesList().Count() > 0 ? mObjModuleDAO.GetEntitiesList().Max(a => a.Position) + 1 : 1;
        }

        public void PositionUp(long pLonId)
        {
            IList<Module> lLstObjModules = mObjModuleDAO.GetEntitiesList()
                .OrderBy(a => a.Position)
                .ThenBy(b => b.Name)
                .AsEnumerable()
                .Select((c, i) => { c.Position = i + 1; return c; })
                .ToList();

            Module lObjModule = lLstObjModules.FirstOrDefault(x => x.Id == pLonId);
            int lIntIndex = lLstObjModules.IndexOf(lObjModule);

            if (lIntIndex > 0)
            {
                int LIntIndexTemp = lLstObjModules[lIntIndex - 1].Position;
                lLstObjModules[lIntIndex - 1].Position = lLstObjModules[lIntIndex].Position;
                lLstObjModules[lIntIndex].Position = LIntIndexTemp;

                mObjModuleDAO.SaveOrUpdateEntity(lLstObjModules[lIntIndex - 1]);
                mObjModuleDAO.SaveOrUpdateEntity(lLstObjModules[lIntIndex]);
            }
            else
            {
                throw new Exception("El registro ya tiene la primera posición.");
            }
        }

        public void PositionDown(long pLonId)
        {
            IList<Module> lLstObjModules = mObjModuleDAO.GetEntitiesList()
                .OrderBy(a => a.Position)
                .ThenBy(b => b.Name)
                .AsEnumerable()
                .Select((c, i) => { c.Position = i + 1; return c; })
                .ToList();

            Module lObjModule = lLstObjModules.FirstOrDefault(x => x.Id == pLonId);
            int lIntIndex = lLstObjModules.IndexOf(lObjModule);

            if (lIntIndex < lLstObjModules.Count - 1)
            {
                int LIntIndexTemp = lLstObjModules[lIntIndex + 1].Position;
                lLstObjModules[lIntIndex + 1].Position = lLstObjModules[lIntIndex].Position;
                lLstObjModules[lIntIndex].Position = LIntIndexTemp;

                mObjModuleDAO.SaveOrUpdateEntity(lLstObjModules[lIntIndex + 1]);
                mObjModuleDAO.SaveOrUpdateEntity(lLstObjModules[lIntIndex]);
            }
            else
            {
                throw new Exception("El registro ya tiene la última posición.");
            }
        }

        private int GetCurrentPosition(long pLonIdModule)
        {
            return mObjModuleDAO.GetEntitiesList().Where(m => m.Id == pLonIdModule).Count() > 0 ? mObjModuleDAO.GetEntitiesList().Where(m => m.Id == pLonIdModule).Select(m => m.Position).FirstOrDefault() : 0;
        }

        private bool Exists(Module pObjModule)
        {
            return mObjModuleDAO.GetEntitiesList().Where(x => x.Name == pObjModule.Name && x.Id != pObjModule.Id).Count() > 0 ? true : false;
        }
    }
}
