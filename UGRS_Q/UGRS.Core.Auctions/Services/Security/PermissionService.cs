using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.System;
using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Services.Security
{
    public class PermissionService
    {
        private IBaseDAO<Module> mObjModuleDAO;
        private IBaseDAO<Section> mObjSectionDAO;
        private IBaseDAO<Permission> mObjPermissionDAO;
        private IBaseDAO<User> mObjUserDAO;

        public void SaveOrUpdateSystemPermissions(PermissionTypeEnum pEnmTypePermission, IList<Permission> pLstObjPermissions)
        {
            if (pLstObjPermissions != null && pLstObjPermissions.Count > 0)
            {
                //Get permissions to update
                IList<Permission> lLstObjPermissionsToSaveOrUpdate = (from Current in mObjPermissionDAO.GetEntitiesList().Where(p => p.PermissionType == pEnmTypePermission &&
                                                                      (p.AccessType == AccessTypeEnum.MODULE || p.AccessType == AccessTypeEnum.SECTION)).ToList()
                                                                      join New in pLstObjPermissions on Current.Id equals New.Id
                                                                      where Current.AllowAccess != New.AllowAccess
                                                                      select Current).Select(x => { x.AllowAccess = !x.AllowAccess; return x; }).ToList();

                //Get permissions to add
                lLstObjPermissionsToSaveOrUpdate = lLstObjPermissionsToSaveOrUpdate.Concat(pLstObjPermissions.Where(x => x.Id == 0)).ToList();

                //Save changes
                mObjPermissionDAO.SaveOrUpdateEntitiesList(lLstObjPermissionsToSaveOrUpdate);
            }
        }

        public void SaveOrUpdateSpecialPermissions(PermissionTypeEnum pEnmTypePermission, IList<Permission> pLstObjPermissions)
        {
            if(pLstObjPermissions != null && pLstObjPermissions.Count > 0)
            {
                //Get permissions to update
                IList<Permission> lLstObjPermissionsToSaveOrUpdate = (from Current in mObjPermissionDAO.GetEntitiesList().Where(p => p.PermissionType == pEnmTypePermission &&
                                                                      p.AccessType == AccessTypeEnum.SPECIAL_FUNCTION).ToList()
                                                                      join New in pLstObjPermissions on Current.Id equals New.Id
                                                                      where Current.AllowAccess != New.AllowAccess
                                                                      select Current).Select(x => { x.AllowAccess = !x.AllowAccess; return x; }).ToList();

                //Get permissions to add
                lLstObjPermissionsToSaveOrUpdate = lLstObjPermissionsToSaveOrUpdate.Concat(pLstObjPermissions.Where(x => x.Id == 0)).ToList();

                //Save changes
                mObjPermissionDAO.SaveOrUpdateEntitiesList(lLstObjPermissionsToSaveOrUpdate);
            }
        }

        public PermissionService(IBaseDAO<Module> pObjModuleDAO, IBaseDAO<Section> pObjSectionDAO, IBaseDAO<Permission> pObjPermissionDAO, IBaseDAO<User> pObjUserDAO)
        {
            mObjModuleDAO = pObjModuleDAO;
            mObjSectionDAO = pObjSectionDAO;
            mObjPermissionDAO = pObjPermissionDAO;
            mObjUserDAO = pObjUserDAO;
        }

        public IList<MenuDTO> GetTestSystemMenu()
        {
            IList<MenuDTO> lLstObjResult = new List<MenuDTO>();
            IList<Module> lLstObjModules = GetSortedModulesList();
            IList<Section> lLstObjSections = GetSortedSectionsList();

            foreach (Module lObjModule in lLstObjModules)
            {
                MenuDTO lObjMenu = new MenuDTO()
                {
                    Name = lObjModule.Name,
                    Path = lObjModule.Path,
                    Children = new List<MenuDTO>()
                };

                foreach (Section lObjSection in lLstObjSections.Where(x => x.ModuleId == lObjModule.Id))
                {
                    lObjMenu.Children.Add(new MenuDTO()
                    {
                        Name = lObjSection.Name,
                        Path = lObjSection.Path,
                        Children = new List<MenuDTO>()
                    });
                }

                lLstObjResult.Add(lObjMenu);
            }

            return lLstObjResult;
        }

        public IList<MenuDTO> GetSystemMenu(long pLonUserId)
        {
            IList<MenuDTO> lLstObjResult = new List<MenuDTO>();
            IList<Permission> lLstObjUserPemissions = GetUserPermissions(pLonUserId);
            IList<Permission> lLstObjUserTypePemissions = GetUserTypePermissions(GetUserTypeId(pLonUserId));
            IList<Module> lLstObjModules = GetSortedModulesList();
            IList<Section> lLstObjSections = GetSortedSectionsList();

            foreach (Module lObjModule in lLstObjModules)
            {
                if (HasModulePermission(lObjModule.Id, lLstObjUserPemissions, lLstObjUserTypePemissions) ||
                   (HasPartialModulePermission(lObjModule.Id, lLstObjUserPemissions, lLstObjUserTypePemissions)))
                {
                    MenuDTO lObjMenu = new MenuDTO()
                    {
                        Name = lObjModule.Name,
                        Path = lObjModule.Path,
                        Children = new List<MenuDTO>()
                    };

                    foreach (Section lObjSection in lLstObjSections.Where(x => x.ModuleId == lObjModule.Id))
                    {
                        if (HasSectionPermission(lObjSection.Id, lLstObjUserPemissions, lLstObjUserTypePemissions))
                        {
                            lObjMenu.Children.Add(new MenuDTO()
                            {
                                Name = lObjSection.Name,
                                Path = lObjSection.Path,
                                Children = new List<MenuDTO>()
                            });
                        }
                    }

                    lLstObjResult.Add(lObjMenu);
                }
            }

            return lLstObjResult;
        }

        private bool HasModulePermission(long pLonModule, IList<Permission> pLstObjUserPemissions, IList<Permission> pLstObjUserTypePemissions)
        {
            return pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == pLonModule && x.AllowAccess).Count() > 0 ? true :
                   pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == pLonModule && x.AllowAccess == false).Count() == 0 &&
                   pLstObjUserTypePemissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == pLonModule && x.AllowAccess).Count() > 0 ? true : false;
        }

        private bool HasPartialModulePermission(long pLonModule, IList<Permission> pLstObjUserPemissions, IList<Permission> pLstObjUserTypePemissions)
        {
            IList<long> lLstLonSectionsId = mObjSectionDAO.GetEntitiesList().Where(x => x.ModuleId == pLonModule).Select(x => x.Id).ToList();

            return pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && lLstLonSectionsId.Contains(x.AccessId) && x.AllowAccess).Count() > 0 ? true :
                   pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && lLstLonSectionsId.Contains(x.AccessId) && x.AllowAccess == false).Count() == 0 &&
                   pLstObjUserTypePemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && lLstLonSectionsId.Contains(x.AccessId) && x.AllowAccess).Count() > 0 ? true : false;
        }

        private bool HasSectionPermission(long pLonSection, IList<Permission> pLstObjUserPemissions, IList<Permission> pLstObjUserTypePemissions)
        {

            return pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && x.AccessId == pLonSection && x.AllowAccess).Count() > 0 ? true :
                   pLstObjUserPemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && x.AccessId == pLonSection && x.AllowAccess == false).Count() == 0 &&
                   pLstObjUserTypePemissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && x.AccessId == pLonSection && x.AllowAccess).Count() > 0 ? true : false;
        }

        private long GetUserTypeId(long pLonUserId)
        {
            return mObjUserDAO.GetEntitiesList().Where(x => x.Id == pLonUserId).Select(x => x.UserTypeId).FirstOrDefault();
        }

        public IList<Permission> GetUserPermissions(long pLonUserId)
        {
            return mObjPermissionDAO.GetEntitiesList()
                .Where(x => x.PermissionType == PermissionTypeEnum.USER && x.PermissionId == pLonUserId).ToList();
        }

        public IList<Permission> GetUserTypePermissions(long pLonUserTypeId)
        {
            return mObjPermissionDAO.GetEntitiesList()
                .Where(x => x.PermissionType == PermissionTypeEnum.USER_TYPE && x.PermissionId == pLonUserTypeId).ToList();
        }

        private IList<Module> GetSortedModulesList()
        {
            return mObjModuleDAO.GetEntitiesList().OrderBy(a => a.Position).ThenBy(b => b.Name).ToList();
        }

        private IList<Section> GetSortedSectionsList()
        {
            return mObjSectionDAO.GetEntitiesList().OrderBy(a => a.Position).ThenBy(b => b.Name).ToList();
        }
    }
}
