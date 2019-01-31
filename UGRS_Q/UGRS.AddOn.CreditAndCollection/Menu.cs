using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;


namespace UGRS.AddOn.CreditAndCollection
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

            //oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            //oCreationPackage.UniqueID = "UGRS.AddOn.CreditAndCollection";
            //oCreationPackage.String = "UGRS.AddOn.CreditAndCollection";
            //oCreationPackage.Enabled = true;
            //oCreationPackage.Position = -1;

            oMenus = oMenuItem.SubMenus;

            try
            {
                //  If the manu already exists this code will fail
                oMenus2 = oMenus.Item(6).SubMenus;
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.CreditAndCollection";
                oCreationPackage.String = "Credito y Cobranza";
                oCreationPackage.Position = oMenus2.Count + 1;
                oMenus2.AddEx(oCreationPackage);
                Application.SBO_Application.SetStatusBarMessage("Addon Credito y Cobranza Listo", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                //oMenus.AddEx(oCreationPackage);
            }
            catch (Exception e)
            {

            }

            //try
            //{
            //    // Get the menu collection of the newly added pop-up item
            //    oMenuItem = Application.SBO_Application.Menus.Item("UGRS.AddOn.CreditAndCollection");
            //    oMenus = oMenuItem.SubMenus;

            //    // Create s sub menu
            //    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
            //    oCreationPackage.UniqueID = "UGRS.AddOn.CreditAndCollection.Form1";
            //    oCreationPackage.String = "Form1";
            //    oMenus.AddEx(oCreationPackage);
            //}
            //catch (Exception er)
            //{ //  Menu already exists
            //    Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            //}
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.CreditAndCollection")
                {
                    Form1 activeForm = new Form1();
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
