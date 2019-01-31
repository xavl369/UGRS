using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;

namespace UGRS.AddOn.AccountingAccounts
{
    class Menu
    {
        Utils.Tools objTools = new Utils.Tools();
        public void AddMenuItems()
        {
           
            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "UGRS.AddOn.AccountingAccounts";
            oCreationPackage.String = "Nomina";
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
                oMenuItem = Application.SBO_Application.Menus.Item("UGRS.AddOn.AccountingAccounts");
                oMenus = oMenuItem.SubMenus;
                
                    // Create s sub menu
                    //oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    //oCreationPackage.UniqueID = "UGRS.AddOn.AccountingAccounts.frmSetup";
                    //oCreationPackage.String = "Configuraciones";
                    //oMenus.AddEx(oCreationPackage);
                                
                    // Create s sub menu
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "UGRS.AddOn.AccountingAccounts.frmImport";
                    oCreationPackage.String = "Importar";
                    oMenus.AddEx(oCreationPackage);

                 
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "UGRS.AddOn.AccountingAccounts.frmConfiguracion";
                    oCreationPackage.String = "Configuración";
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
                //if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.AccountingAccounts.frmSetup" )
                //{
                    
                //    if (!objTools.FormExists("frmSetup"))
                //    {
                //        frmSetup activeForm = new frmSetup();
                //        activeForm.Show();                                            
                //    }
                //}
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.AccountingAccounts.frmImport")
                {
                    if (!objTools.FormExists("frmImport"))
                    {
                        frmImport activeForm = new frmImport();
                        activeForm.Show();
                    }
                }
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.AccountingAccounts.frmConfiguracion")
                {
                    if (!objTools.FormExists("frmImport"))
                    {
                        frmConfiguracion activeForm = new frmConfiguracion();
                        activeForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }

    }
}
