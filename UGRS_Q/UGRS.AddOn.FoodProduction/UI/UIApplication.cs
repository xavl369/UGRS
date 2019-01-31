using SAPbouiCOM;

namespace UGRS.AddOn.FoodProduction.UI
{
    public class UIApplication
    {
        private static bool mBolConnected;

        private static SAPbouiCOM.Company mObjCompany;

        private static SAPbouiCOM.Company mObjApplication;

        public static bool Connected
        {
            get { return mBolConnected; }
            private set { mBolConnected = value; }
        }

        public static SAPbouiCOM.Company Company
        {
            get { return mObjCompany; }
            private set { mObjCompany = value; }
        }

        //public static void UIConnect(SAPbouiCOM.Application pObjApplication)
        //{
        //    Company = (SAPbobsCOM.Company)pObjApplication.Company.GetDICompany();
        //    Connected = true;
        //}


        public static bool IsActiveForm(string uniqueId)
        {
            return SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm.UniqueID == uniqueId ? true : false;
        }

        public static Application GetApplication()
        {
            return SAPbouiCOM.Framework.Application.SBO_Application;
        }

        public static SAPbouiCOM.Company GetCompany()
        {
            if (mObjCompany == null)
            {
                mObjCompany = (SAPbouiCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company;
            }
            return mObjCompany;
        }

        public static SAPbouiCOM.MenuCreationParams GetMenuCreationParams()
        {
            return ((SAPbouiCOM.MenuCreationParams)(GetApplication().CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
        }

        public static SAPbouiCOM.Menus GetMenus()
        {
            return GetApplication().Menus;
        }

        public static void ShowMessageBox(string message)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(message, 1, "Ok", "", "");
        }

        public static void ShowMessage(string message)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_None);
        }

        public static void ShowWarning(string message)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
        }

        public static void ShowSuccess(string message)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
        }

        public static void ShowError(string message)
        {
            SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);
        }
    }
}
