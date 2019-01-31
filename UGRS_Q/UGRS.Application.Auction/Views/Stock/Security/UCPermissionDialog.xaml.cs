using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Security;
using UGRS.Core.Auctions.Entities.Security;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension.Enum;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions.Securty
{
    public partial class UCPermissionDialog : UserControl
    {
        #region Attributes

        SecurityServicesFactory mObjSecurityServiceFactory;
        SystemServicesFactory mObjSystemServiceFactory;

        List<ModuleDTO> mLstObjModulesPermissions { get; set; }
        List<SpecialFunctionDTO> mLstObjSpecialPermissions { get; set; }

        PermissionTypeEnum mEnmPermissionType;
        string mStrPermissionType;
        long mLonPermissionId;
        Thread mObjWorker;

        #endregion

        #region Constructor

        public UCPermissionDialog(PermissionTypeEnum pEnmPermissionType, string pStrPermission, long pLonPermissionId)
        {
            mObjSecurityServiceFactory = new SecurityServicesFactory();
            mObjSystemServiceFactory = new SystemServicesFactory();

            mEnmPermissionType = pEnmPermissionType;
            mStrPermissionType = pEnmPermissionType.GetDescription();
            mLonPermissionId = pLonPermissionId;

            InitializeComponent();
            lblPermission.Content = pStrPermission;
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.mLstObjModulesPermissions = GetMenuPermissions(mEnmPermissionType, mLonPermissionId);
                this.mLstObjSpecialPermissions = GetSpecialPermissions(mEnmPermissionType, mLonPermissionId);
                this.tvwPermissionsMenu.DataContext = mLstObjModulesPermissions;
                this.tvwPermissionsSpecial.DataContext = mLstObjSpecialPermissions;
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void MenuCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuCheckBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SpecialCheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SpecialCheckBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public List<ModuleDTO> GetMenuPermissions(PermissionTypeEnum pEnmPermissionType, long pLonPermissionId)
        {
            IList<Module> lLstObjModules = mObjSystemServiceFactory.GetModuleService().GetList().Where(x => x.Active == true).ToList();

            IList<Permission> lLstObjPermissions = pEnmPermissionType == PermissionTypeEnum.USER_TYPE ? 
            mObjSecurityServiceFactory.GetPermissionService().GetUserTypePermissions(pLonPermissionId) :
            mObjSecurityServiceFactory.GetPermissionService().GetUserPermissions(pLonPermissionId);

            foreach (Module lObjModule in lLstObjModules)
            {
                //Is active if exists on permissions list and allow it access
                lObjModule.Active = lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == lObjModule.Id && x.AllowAccess).Count() > 0;

                //For each section in the module
                foreach (UGRS.Core.Auctions.Entities.System.Section lObjSection in lObjModule.Sections)
                {
                    //Is active if exists on permissions list and allow it access
                    lObjSection.Active = lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == lObjModule.Id && x.AllowAccess).Count() > 0;
                }
            }

            return lLstObjModules.Select(x => new ModuleDTO(x)).ToList();
        }

        public List<SpecialFunctionDTO> GetSpecialPermissions(PermissionTypeEnum pEnmPermissionType, long pLonPermissionId)
        {
            List<SpecialFunctionDTO> lLstObjList = new List<SpecialFunctionDTO>();
            List<Permission> lLstObjPermissions = pEnmPermissionType == PermissionTypeEnum.USER_TYPE ?
            mObjSecurityServiceFactory.GetPermissionService().GetUserTypePermissions(pLonPermissionId).ToList() :
            mObjSecurityServiceFactory.GetPermissionService().GetUserPermissions(pLonPermissionId).ToList();

            foreach (EnumItem lObjItem in EnumExtension.GetEnumItemList<SpecialFunctionsEnum>())
            {
                lLstObjList.Add(new SpecialFunctionDTO(lObjItem)
                {
                    Active = lLstObjPermissions.Where(x=> x.AccessType == AccessTypeEnum.SPECIAL_FUNCTION && (int)x.AccessId == lObjItem.Value).Count() > 0
                });
            }

            return lLstObjList;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (mLonPermissionId > 0)
            {
                mObjWorker = new Thread(() => SaveChanges(mEnmPermissionType, mLonPermissionId));
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Permisos", "Favor de seleccionar un usuario/tipo de usuario.");
            }
        }

        private void chkSelecctAll_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            foreach (ModuleDTO lObjModule in tvwPermissionsMenu.Items)
            {
                try
                {
                    foreach (SectionDTO lObjSection in lObjModule.Sections)
                    {
                        lObjSection.Active = (pObjSender as CheckBox).IsChecked ?? false;
                    }
                    lObjModule.Active = (pObjSender as CheckBox).IsChecked ?? false;
                }
                catch
                {
                    //Ignore
                }
            }

            foreach (SpecialFunctionDTO lObjFunction in tvwPermissionsSpecial.Items)
            {
                try
                {
                    lObjFunction.Active = (pObjSender as CheckBox).IsChecked ?? false;
                }
                catch
                {
                    //Ignore
                }
            }
        }

        private void chkExpandAll_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            foreach (ModuleDTO lObjModule in tvwPermissionsMenu.Items)
            {
                try
                {
                    lObjModule.Expanded = (pObjSender as CheckBox).IsChecked ?? false;
                }
                catch
                {
                    //Ignore
                }
            }
        }

        #endregion

        #region Methods

        private IList<Permission> GetMenuPermissionsList(PermissionTypeEnum pEnmPermissionType, long pLonPermissionId)
        {
            List<Permission> lLstObjPermissions = (
                pEnmPermissionType == PermissionTypeEnum.USER_TYPE ?
                mObjSecurityServiceFactory.GetPermissionService().GetUserTypePermissions(pLonPermissionId) :
                mObjSecurityServiceFactory.GetPermissionService().GetUserPermissions(pLonPermissionId)
            ).Where(x=> x.AccessType == AccessTypeEnum.MODULE && x.AccessType == AccessTypeEnum.SECTION).ToList();

            foreach (ModuleDTO lObjModule in mLstObjModulesPermissions)
            {
                //Update if exists in the permissions list
                if (lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == lObjModule.Id).Count() > 0)
                {
                    foreach (Permission lObjPermission in lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.MODULE && x.AccessId == lObjModule.Id))
                    {
                        lObjPermission.AllowAccess = lObjModule.Active;
                    }
                }
                //Add if is new and allow access
                else
                {
                    lLstObjPermissions.Add(new Permission()
                    {
                        PermissionType = pEnmPermissionType,
                        AccessType = AccessTypeEnum.MODULE,
                        AllowAccess = lObjModule.Active,
                        PermissionId = mLonPermissionId, 
                        AccessId = lObjModule.Id,
                    });
                }

                //For each section in the module
                foreach (SectionDTO lObjSection in lObjModule.Sections)
                {
                    //Update if exists in the permissions list
                    if (lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && x.AccessId == lObjSection.Id).Count() > 0)
                    {
                        foreach (Permission lObjPermission in lLstObjPermissions.Where(x => x.AccessType == AccessTypeEnum.SECTION && x.AccessId == lObjSection.Id))
                        {
                            lObjPermission.AllowAccess = lObjSection.Active;
                        }
                    }
                    //Add if is new and allow access
                    else
                    {
                        lLstObjPermissions.Add(new Permission()
                        {
                            PermissionType = pEnmPermissionType,
                            AccessType = AccessTypeEnum.SECTION,
                            AllowAccess = lObjSection.Active,
                            PermissionId = mLonPermissionId,
                            AccessId = lObjSection.Id,
                        });
                    }
                }
            }

            return lLstObjPermissions;
        }

        private IList<Permission> GetSpecialPermissionsList(PermissionTypeEnum pEnmPermissionType, long pLonPermissionId)
        {
            List<Permission> lLstObjPermissions = (
                pEnmPermissionType == PermissionTypeEnum.USER_TYPE ?
                mObjSecurityServiceFactory.GetPermissionService().GetUserTypePermissions(pLonPermissionId) :
                mObjSecurityServiceFactory.GetPermissionService().GetUserPermissions(pLonPermissionId)
            ).Where(x => x.AccessType == AccessTypeEnum.SPECIAL_FUNCTION).ToList();

            foreach (SpecialFunctionDTO lObjFunction in mLstObjSpecialPermissions)
            {
                //Update if exists in the permissions list
                if (lLstObjPermissions.Where(x => x.AccessId == lObjFunction.Id).Count() > 0)
                {
                    foreach (Permission lObjPermission in lLstObjPermissions.Where(x => x.AccessId == lObjFunction.Id))
                    {
                        lObjPermission.AllowAccess = lObjFunction.Active;
                    }
                }
                //Add if is new and allow access
                else
                {
                    lLstObjPermissions.Add(new Permission()
                    {
                        PermissionType = pEnmPermissionType,
                        AccessType = AccessTypeEnum.SPECIAL_FUNCTION,
                        AllowAccess = lObjFunction.Active,
                        PermissionId = mLonPermissionId,
                        AccessId = lObjFunction.Id,
                    });
                }
            }

            return lLstObjPermissions;
        }

        private void SaveChanges(PermissionTypeEnum pEnmPermissionType, long pLonPermissionId)
        {
            FormLoading();
            try
            {
                this.mObjSecurityServiceFactory.GetPermissionService().SaveOrUpdateSystemPermissions(pEnmPermissionType, GetMenuPermissionsList(pEnmPermissionType, pLonPermissionId));
                this.mObjSecurityServiceFactory.GetPermissionService().SaveOrUpdateSpecialPermissions(pEnmPermissionType, GetSpecialPermissionsList(pEnmPermissionType, pLonPermissionId));
                this.Dispatcher.Invoke(() => CustomMessageBox.Show("Permisos", "Los cambios se han guardado correctamente", this.GetParent()));
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke(() => CustomMessageBox.Show("Error", lObjException.Message, this.GetParent()));
            }
            finally
            {
                FormDefault();
            }
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdPermission.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdPermission.UnblockUI();
            });
        }

        #endregion
    }
}
