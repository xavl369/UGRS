﻿using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;

namespace UGRS.AddOn.Permissions
{
    class Menu
    {
         public string lStrTypeEx = "";
         public int lIntTypeCount = 0;

         public Menu()
         {

         }

        public void AddMenuItems()
        {

            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "UGRS.AddOn.Permissions";
            oCreationPackage.String = "Configuraciones de permisos";
            oCreationPackage.Enabled = true;
            oCreationPackage.Position = -1;

            oMenus = oMenuItem.SubMenus;

            try
            {
                //  If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception e)
            {

            }

            try
            {
                // Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item("UGRS.AddOn.Permissions");
                oMenus = oMenuItem.SubMenus;

                // Create s sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Permissions.Prefix";
                oCreationPackage.String = "Prefijo de aretes";
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception er)
            { //  Menu already exists
                Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Permissions.Prefix")
                {
                    if (!FormExists(lStrTypeEx, lIntTypeCount))
                    {
                        Prefixes activeForm = new Prefixes();
                        activeForm.Show();
                        lStrTypeEx=activeForm.UIAPIRawForm.TypeEx.ToString();
                        lIntTypeCount = activeForm.UIAPIRawForm.TypeCount;

                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }

        private bool FormExists(string lStrTypex, int lIntTypeCount)
        {
            bool exist = false;
            try
            {
                var a = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetForm(lStrTypex, lIntTypeCount);
                exist = true;
            }
            catch (Exception e)
            {
                exist = false;
            }

            return exist;
        }

    }
}
