using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.System;

namespace UGRS.Core.Auctions.Services.System
{
    public class SectionService
    {
        private IBaseDAO<Section> mObjSectionDAO;

        public SectionService(IBaseDAO<Section> pObjSectionDAO)
        {
            mObjSectionDAO = pObjSectionDAO;
        }

        public IQueryable<Section> GetList()
        {
            return mObjSectionDAO.GetEntitiesList().OrderBy(x=> x.Module.Position).ThenBy(x=> x.Position);
        }

        public void SaveOrUpdate(Section pObjSection)
        {
           // mObjSectionDAO.SaveOrUpdateEntity(pObjSection);
            if (!Exists(pObjSection))
            {
                if (pObjSection.Id == 0)
                {
                    pObjSection.Position = GetNextPosition(pObjSection.ModuleId);
                    mObjSectionDAO.AddEntity(pObjSection);
                }
                else
                {
                    pObjSection.Position = GetCurrentPosition(pObjSection.Id);
                    mObjSectionDAO.UpdateEntity(pObjSection, pObjSection.Id);
                }
            }
            else
            {
                throw new Exception("Registro existente.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjSectionDAO.RemoveEntity(pLonId);
        }

        public void SaveOrUpdateEntity(Section pObjSection)
        {
            if (!Exists(pObjSection))
            {
                if (pObjSection.Id == 0)
                {
                    pObjSection.Position = GetNextPosition(pObjSection.ModuleId);
                    mObjSectionDAO.AddEntity(pObjSection);
                }
                else
                {
                    pObjSection.Position = GetCurrentPosition(pObjSection.Id);
                    mObjSectionDAO.UpdateEntity(pObjSection, pObjSection.Id);
                }
            }
            else
            {
                throw new Exception("Registro existente.");
            }
        }

        public int GetNextPosition(long pLngModule)
        {
            return mObjSectionDAO.GetEntitiesList().Where(x => x.ModuleId == pLngModule).Count() > 0 ? mObjSectionDAO.GetEntitiesList().Where(x => x.ModuleId == pLngModule).Max(a => a.Position) + 1 : 1;
        }

        public void PositionUp(long pLonIdSection, long pLonModuleId)
        {
            IList<Section> lLstObjSections = mObjSectionDAO.GetEntitiesList()
                .Where(x => x.ModuleId == pLonModuleId)
                .OrderBy(a => a.Position)
                .ThenBy(b => b.Name)
                .AsEnumerable()
                .Select((c, i) => { c.Position = i + 1; return c; })
                .ToList();

            Section lObjSection = lLstObjSections.FirstOrDefault(x => x.Id == pLonIdSection);
            int lIntIndex = lLstObjSections.IndexOf(lObjSection);

            if (lIntIndex > 0)
            {
                int lIntPositionTemp = lLstObjSections[lIntIndex - 1].Position;
                lLstObjSections[lIntIndex - 1].Position = lLstObjSections[lIntIndex].Position;
                lLstObjSections[lIntIndex].Position = lIntPositionTemp;

                mObjSectionDAO.UpdateEntity(lLstObjSections[lIntIndex - 1], lLstObjSections[lIntIndex - 1].Id);
                mObjSectionDAO.UpdateEntity(lLstObjSections[lIntIndex], lLstObjSections[lIntIndex].Id);
            }
            else
            {
                throw new Exception("El registro ya tiene la primera posición.");
            }
        }

        public void PositionDown(long pLonIdSection, long pLonModuleId)
        {
            IList<Section> lLstObjSections = mObjSectionDAO.GetEntitiesList()
                .Where(x => x.ModuleId == pLonModuleId)
                .OrderBy(a => a.Position)
                .ThenBy(b => b.Name)
                .AsEnumerable()
                .Select((c, i) => { c.Position = i + 1; return c; })
                .ToList();

            Section lObjSection = lLstObjSections.FirstOrDefault(x => x.Id == pLonIdSection);
            int lIntIndex = lLstObjSections.IndexOf(lObjSection);

            if (lIntIndex < lLstObjSections.Count - 1)
            {
                int lIntPositionTemp = lLstObjSections[lIntIndex + 1].Position;
                lLstObjSections[lIntIndex + 1].Position = lLstObjSections[lIntIndex].Position;
                lLstObjSections[lIntIndex].Position = lIntPositionTemp;

                mObjSectionDAO.UpdateEntity(lLstObjSections[lIntIndex + 1], lLstObjSections[lIntIndex + 1].Id);
                mObjSectionDAO.UpdateEntity(lLstObjSections[lIntIndex], lLstObjSections[lIntIndex].Id);
            }
            else
            {
                throw new Exception("El registro ya tiene la última posición.");
            }
        }

        private int GetCurrentPosition(long pLonIdSection)
        {
            return mObjSectionDAO.GetEntitiesList().Where(s => s.Id == pLonIdSection).Count() > 0 ? mObjSectionDAO.GetEntitiesList().Where(s => s.Id == pLonIdSection).Select(s => s.Position).FirstOrDefault() : 0;
        }

        private bool Exists(Section pObjSection)
        {
            return mObjSectionDAO.GetEntitiesList().Where(x => x.Name == pObjSection.Name && x.ModuleId == pObjSection.ModuleId && x.Id != pObjSection.Id).Count() > 0 ? true : false;
        }
    }
}
