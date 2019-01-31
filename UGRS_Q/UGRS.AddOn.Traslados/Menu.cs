using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;

namespace UGRS.AddOn.Traslados
{
    class Menu
    {
        public void AddMenuItems()
        {
            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;
            
            SAPbouiCOM.Menus oMenus2 = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'
            oMenus = oMenuItem.SubMenus;
            
            try
            {
                oMenus2 = oMenus.Item(7).SubMenus.Item(4).SubMenus;
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Traslados.Form1";
                oCreationPackage.String = "Traslados de Corrales a Subastas";
                oCreationPackage.Position = oMenus2.Count+1;
                oMenus2.AddEx(oCreationPackage);
                Application.SBO_Application.SetStatusBarMessage("Addon Corrales a Subastas Listo", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            }
            catch (Exception e)
            {
                //Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Traslados.Form1")
                {
                    UGRS.AddOn.Traslados.Form1 activeForm = new UGRS.AddOn.Traslados.Form1();
                    activeForm.Show();
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }

    }
}
