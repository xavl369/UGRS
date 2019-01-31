using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;
using System.IO;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Cuarentenarias
{
    class Menu
    {
        Utils.utils lObjUtils = new Utils.utils();

        #region variables
        public string lStrTypeEx = "";
        public int lIntTypeCount = 0;

        public string[] pArrTypeEx = new string[4];
        public int[] pArrTypeCount = new int[4];
        #endregion

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
            string sPath = PathUtilities.GetCurrent("Icon\\Doctor.bmp");

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "UGRS.AddOn.Cuarentenarias";
            oCreationPackage.String = "Cuarentenarias";
            oCreationPackage.Enabled = true;
            oCreationPackage.Image = sPath;
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
                oMenuItem = Application.SBO_Application.Menus.Item("UGRS.AddOn.Cuarentenarias");
                oMenus = oMenuItem.SubMenus;

                // Create s sub menu Listado de Inspección
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Cuarentenarias.frmChkIns";
                oCreationPackage.String = "Listado de Inspección";
                oMenus.AddEx(oCreationPackage);

                // Create s sub menu Inspeccion de Ganado
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Cuarentenarias.frmIns";
                oCreationPackage.String = "Inspección de Ganado";
                oMenus.AddEx(oCreationPackage);

                // Create s sub menu Inspeccion de Ganado
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Cuarentenarias.frmFacIns";
                oCreationPackage.String = "Facturación  Cuarentenarias";
                oMenus.AddEx(oCreationPackage);

                // Create s sub menu Inspeccion de Ganado
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "UGRS.AddOn.Cuarentenarias.frmRejectO";
                oCreationPackage.String = "Salidas por Rechazo";
                oMenus.AddEx(oCreationPackage);

            }
            catch (Exception er)
            { //  Menu already exists
                //Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {

                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Cuarentenarias.frmChkIns")
                {
                    if (!lObjUtils.FormExists(pArrTypeEx[0], pArrTypeCount[0]))
                    {
                        frmChkIns activeForm = new frmChkIns();
                        activeForm.Show();
                        pArrTypeEx[0] = activeForm.UIAPIRawForm.TypeEx.ToString();
                        pArrTypeCount[0] = activeForm.UIAPIRawForm.TypeCount;
                    }
                }

                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Cuarentenarias.frmIns")
                {
                    if (!lObjUtils.FormExists(pArrTypeEx[1], pArrTypeCount[1]))
                    {
                        frmIns activeForm = new frmIns();
                        activeForm.Show();
                        pArrTypeEx[1] = activeForm.UIAPIRawForm.TypeEx.ToString();
                        pArrTypeCount[1] = activeForm.UIAPIRawForm.TypeCount;
                    }
                }
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Cuarentenarias.frmFacIns")
                {
                    if(!lObjUtils.FormExists(pArrTypeEx[2],pArrTypeCount[2]))
                    {
                        Forms.frmInvoices activeForm = new Forms.frmInvoices();
                        activeForm.Show();
                        pArrTypeEx[2] = activeForm.UIAPIRawForm.TypeEx.ToString();
                        pArrTypeCount[2] = activeForm.UIAPIRawForm.TypeCount;
                    }
                }
                if (pVal.BeforeAction && pVal.MenuUID == "UGRS.AddOn.Cuarentenarias.frmRejectO")
                {
                    if (!lObjUtils.FormExists(pArrTypeEx[3], pArrTypeCount[3]))
                    {
                        Forms.frmRejectO activeForm = new Forms.frmRejectO();
                        activeForm.Show();
                        pArrTypeEx[3] = activeForm.UIAPIRawForm.TypeEx.ToString();
                        pArrTypeCount[3] = activeForm.UIAPIRawForm.TypeCount;
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
