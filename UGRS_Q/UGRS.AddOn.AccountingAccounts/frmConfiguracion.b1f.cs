using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.AddOn.AccountingAccounts.DTO;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.AddOn.AccountingAccounts.DAO;
using UGRS.AddOn.AccountingAccounts.Services;
using UGRS.AddOn.AccountingAccounts.Tables;
using System.Net;

namespace UGRS.AddOn.AccountingAccounts
{
    [FormAttribute("UGRS.AddOn.AccountingAccounts.frmConfiguracion", "frmConfiguracion.b1f")]
    class frmConfiguracion : UserFormBase
    {
        #region Attributes
        private LoginDAO mObjLoginDAO = null;
        private IList<LoginDTO> mListObjLogin = null;
        private LoginDTO gObjNomina = null;
        private LoginDTO gObjAsal = null;
        private ProgressBarManager mObjProgressBar = null;
        private SAPbouiCOM.ChooseFromList mObjChooseFromList;
        private SAPbouiCOM.EditText txtCuentaCNom;
        //SAPbouiCOM.ChooseFromListCollection oCFLs;
        SAPbouiCOM.Conditions oCons;
        SAPbouiCOM.Condition oCon;
        SAPbouiCOM.Form oForm;
        private int gIntCodeNom = 0;
        private int gCodeAsal = 0;
        private bool gBolUpdateNomina = false;
        private bool lBolUpdateAsal = false;
        #endregion

        #region Constructor
        public frmConfiguracion()
        {
            oForm = Application.SBO_Application.Forms.Item(this.UIAPIRawForm.UniqueID);
            LoadFormEvents();
            oForm.Resize(658, 394);

            AddChooseFromListNomina();
            AddChooseFromListAsal();

            FillFieldsBD();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.txtBDName = ((SAPbouiCOM.EditText)(this.GetItem("txtBDName").Specific));
            this.txtDesc = ((SAPbouiCOM.EditText)(this.GetItem("txtDesc").Specific));
            this.StaticText35 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_65").Specific));
            this.StaticText36 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_66").Specific));
            this.txtLogin = ((SAPbouiCOM.EditText)(this.GetItem("txtLogin").Specific));
            this.txtPwd = ((SAPbouiCOM.EditText)(this.GetItem("txtPwd").Specific));
            this.chkActivo = ((SAPbouiCOM.CheckBox)(this.GetItem("chkActivo").Specific));
            this.txtServer = ((SAPbouiCOM.EditText)(this.GetItem("txtServer").Specific));
            this.btnGuardarAsal = ((SAPbouiCOM.Button)(this.GetItem("Item_87").Specific));
            this.Folder1 = ((SAPbouiCOM.Folder)(this.GetItem("Item_74").Specific));
            this.Folder2 = ((SAPbouiCOM.Folder)(this.GetItem("Item_75").Specific));
            this.btnGdrNom = ((SAPbouiCOM.Button)(this.GetItem("btn").Specific));
            this.StaticText42 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_76").Specific));
            this.StaticText43 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_77").Specific));
            this.StaticText44 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_78").Specific));
            this.txtBDNameAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_79").Specific));
            this.txtDescAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_80").Specific));
            this.StaticText45 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_81").Specific));
            this.StaticText46 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_82").Specific));
            this.txtLoginAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_83").Specific));
            this.txtPwdAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_84").Specific));
            this.chkActivoAsal = ((SAPbouiCOM.CheckBox)(this.GetItem("Item_85").Specific));
            this.txtServAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_86").Specific));
            this.StaticText47 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_88").Specific));
            this.txtCuentaCNom = ((SAPbouiCOM.EditText)(this.GetItem("Item_90").Specific));
            this.txtServer.Item.Click();
            this.Folder1.Item.Click();
            this.InitChkBox();
            this.LoadApplicationEvents();
            this.StaticText49 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_93").Specific));
            this.txtCuentaCAsal = ((SAPbouiCOM.EditText)(this.GetItem("Item_94").Specific));

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {

        }

        private void InitChkBox()
        {
            
        }

        public void LoadApplicationEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void LoadFormEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        public void UnloadFormEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        private void AddChooseFromListNomina()
        {
            oForm.DataSources.UserDataSources.Add("CFL_0", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            SAPbouiCOM.ChooseFromListCollection oCFLs = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
            lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                (SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            //  Adding 2 CFL, one for the button and one for the edit text.
            //string strCFLID = oCFLCreationParams.UniqueID
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "1";
            lObjCFLCreationParams.UniqueID = "OACT1";
            mObjChooseFromList = oCFLs.Add(lObjCFLCreationParams);
            oCons = mObjChooseFromList.GetConditions();

            /*oCon = oCons.Add();
            oCon.Alias = "AcctCode";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "C";

            mObjChooseFromList.SetConditions(oCons);*/

            txtCuentaCNom.DataBind.SetBound(true, "", "CFL_0");
            txtCuentaCNom.ChooseFromListUID = mObjChooseFromList.UniqueID;
            txtCuentaCNom.ChooseFromListAlias = "AcctCode";
        }

        private void AddChooseFromListAsal()
        {
            oForm.DataSources.UserDataSources.Add("CFL_1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            SAPbouiCOM.ChooseFromListCollection oCFLs = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
            lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                (SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            //  Adding 2 CFL, one for the button and one for the edit text.
            //string strCFLID = oCFLCreationParams.UniqueID
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "1";
            lObjCFLCreationParams.UniqueID = "OACT2";
            mObjChooseFromList = oCFLs.Add(lObjCFLCreationParams);
            oCons = mObjChooseFromList.GetConditions();

            mObjChooseFromList.SetConditions(oCons);

            txtCuentaCAsal.DataBind.SetBound(true, "", "CFL_1");
            txtCuentaCAsal.ChooseFromListUID = mObjChooseFromList.UniqueID;
            txtCuentaCAsal.ChooseFromListAlias = "AcctCode";
        }
        #endregion

        #region Eventos
        /// <summary>
        /// SBO_Application_ItemEvent
        /// Metodo para controlar los eventos dentro de la pantalla de Configuración
        /// @Author RomanCordova
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        /// 
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("btn"))
                                {
                                    SaveConfigNomina();
                                }
                                if (pVal.ItemUID.Equals("Item_87"))
                                {
                                    SaveConfigAsalariados();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
            }

        }

        private void chooseFromListAfterEvent(string FormUID, ref SAPbouiCOM.ItemEvent pValEvent, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pValEvent.EventType != SAPbouiCOM.BoEventTypes.et_FORM_CLOSE)
            {

                if (pValEvent.ItemUID == "Item_90" && pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    try
                    {
                        SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                        oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                        string sCFL_ID = null;
                        sCFL_ID = oCFLEvento.ChooseFromListUID;

                        SAPbouiCOM.ChooseFromList oCFL = null;
                        oCFL = oForm.ChooseFromLists.Item(sCFL_ID);

                        SAPbouiCOM.DataTable oDataTable = null;
                        oDataTable = oCFLEvento.SelectedObjects;
                        string lStrVal = null;
                        lStrVal = Convert.ToString(oDataTable.GetValue(0, 0));
                        txtCuentaCNom.Value = lStrVal;

                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }

                        if (ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            //LoadMatrixPeticiones(val);
                        }
                    }
                }

                if (pValEvent.ItemUID == "Item_94" && pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    try
                    {
                        SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                        oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                        string sCFL_ID = null;
                        sCFL_ID = oCFLEvento.ChooseFromListUID;

                        SAPbouiCOM.ChooseFromList oCFL = null;
                        oCFL = oForm.ChooseFromLists.Item(sCFL_ID);

                        SAPbouiCOM.DataTable oDataTable = null;
                        oDataTable = oCFLEvento.SelectedObjects;
                        string lStrVal = null;

                        lStrVal = System.Convert.ToString(oDataTable.GetValue(0, 0));
                        txtCuentaCAsal.Value = lStrVal;

                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }

                        if (ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            //LoadMatrixPeticiones();
                        }
                    }
                }
            }
        }
        #endregion

        #region Metodos
        private void FillFieldsBD()
        {

            try
            {
                IList<LoginDTO> lListObjDB = new List<LoginDTO>();
                lListObjDB = GetLoginInfo();

                if (lListObjDB.Count > 0)
                {
                    gObjNomina = lListObjDB.Where(x => x.Descripcion == "Nomina").FirstOrDefault();
                    gObjAsal = lListObjDB.Where(x => x.Descripcion == "Asalariados").FirstOrDefault();

                    if (gObjNomina != null)
                    {
                        gIntCodeNom = gObjNomina.Code;
                        this.txtServer.Value = gObjNomina.NameServer;
                        this.txtLogin.Value = gObjNomina.Login;
                        this.txtPwd.Value = gObjNomina.Password;
                        this.txtBDName.Value = gObjNomina.NameDB;
                        this.txtCuentaCNom.Value = gObjNomina.AccountingAccount;
                        this.txtDesc.Value = gObjNomina.Descripcion;

                        gBolUpdateNomina = true;
                    }
                    else
                    {
                        gBolUpdateNomina = false;
                    }

                    if (gObjAsal != null)
                    {
                        gCodeAsal = gObjAsal.Code;
                        this.txtServAsal.Value = gObjAsal.NameServer;
                        this.txtLoginAsal.Value = gObjAsal.Login;
                        this.txtPwdAsal.Value = gObjAsal.Password;
                        this.txtBDNameAsal.Value = gObjAsal.NameDB;
                        this.txtCuentaCAsal.Value = gObjAsal.AccountingAccount;
                        this.txtDescAsal.Value = gObjAsal.Descripcion;

                        lBolUpdateAsal = true;
                    }
                    else
                    {
                        lBolUpdateAsal = false;
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }    
    
        }

        /// <summary>
        /// Crea o modifica los datos de conexión de nómina
        /// </summary>
        private void SaveConfigNomina()
        {
            AccountingAccountsSetupLogin setupLogin = null;
            AccountingAccountsSetupLoginService setupLoginService = new AccountingAccountsSetupLoginService();

            try
            {
                if (!CheckEmptyFieldsNomina())
                {
                    Application.SBO_Application.StatusBar.SetText("Complete campos vacíos para continuar", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    return;
                }

                if (!ValidateIPString(txtServer.Value))
                {
                    Application.SBO_Application.StatusBar.SetText("Verifique el formato de la dirección ip de su servidor", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    return;
                }

                if (gBolUpdateNomina)
                {
                    setupLogin = new AccountingAccountsSetupLogin()
                    {
                        Code = gIntCodeNom,
                        NameServer = txtServer.Value,
                        Login = txtLogin.Value,
                        Password = txtPwd.Value,
                        NameDB = txtBDName.Value,
                        AccountingAccount = txtCuentaCNom.Value,
                        //Activo = chkActivo.Checked ? chkActivo.ValOn : chkActivo.ValOff,
                        RowCode = gIntCodeNom.ToString()
                    };

                    setupLoginService.Update(setupLogin);
                }
                else
                {
                    setupLogin = new AccountingAccountsSetupLogin()
                    {
                        NameServer = txtServer.Value,
                        Login = txtLogin.Value,
                        Password = txtPwd.Value,
                        NameDB = txtBDName.Value,
                        Descripcion = "Nomina",
                        AccountingAccount = txtCuentaCNom.Value,
                        //Activo = chkActivo.Checked ? chkActivo.ValOn : chkActivo.ValOff,
                        RowCode = gIntCodeNom.ToString()
                    };

                    setupLoginService.Add(setupLogin);

                    FillFieldsBD();

                    gBolUpdateNomina = true;
                }

                Application.SBO_Application.StatusBar.SetText("Configuración guardada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

        }

        /// <summary>
        /// Crea o modifica los datos de conexión de asalariados
        /// </summary>
        private void SaveConfigAsalariados()
        {
            AccountingAccountsSetupLogin setupLogin = null;
            AccountingAccountsSetupLoginService setupLoginService = new AccountingAccountsSetupLoginService();

            try
            {
                if (!CheckEmptyFieldsAsal())
                {
                    Application.SBO_Application.StatusBar.SetText("Complete campos vacíos para continuar", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    return;
                }

                if (!ValidateIPString(txtServAsal.Value))
                {
                    Application.SBO_Application.StatusBar.SetText("Verifique el formato de la dirección ip de su servidor", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    return;
                }

                if (lBolUpdateAsal)
                {
                    setupLogin = new AccountingAccountsSetupLogin()
                    {
                        Code = gCodeAsal,
                        NameServer = txtServAsal.Value,
                        Login = txtLoginAsal.Value,
                        Password = txtPwdAsal.Value,
                        NameDB = txtBDNameAsal.Value,
                        AccountingAccount = txtCuentaCAsal.Value,
                        //Activo = chkActivoAsal.Checked ? chkActivoAsal.ValOn : chkActivoAsal.ValOff,
                        RowCode = gCodeAsal.ToString()
                    };

                    setupLoginService.Update(setupLogin);
                }
                else
                {
                    setupLogin = new AccountingAccountsSetupLogin()
                    {
                        NameServer = txtServAsal.Value,
                        Login = txtLoginAsal.Value,
                        Password = txtPwdAsal.Value,
                        NameDB = txtBDNameAsal.Value,
                        Descripcion = "Asalariados",
                        AccountingAccount = txtCuentaCAsal.Value,
                        //Activo = chkActivoAsal.Checked ? chkActivoAsal.ValOn : chkActivoAsal.ValOff,
                        RowCode = gCodeAsal.ToString()
                    };

                    setupLoginService.Add(setupLogin);

                    FillFieldsBD();

                    lBolUpdateAsal = true;
                }

                Application.SBO_Application.StatusBar.SetText("Configuración guardada con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

        }

        private IList<LoginDTO> GetLoginInfo()
        {
            //FillcmbTipo
            mObjLoginDAO = new LoginDAO();
            mListObjLogin = new List<LoginDTO>();

            try
            {
                mListObjLogin = mObjLoginDAO.GetLoginServer();
                return mListObjLogin;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return mListObjLogin;
            }
        }

        private bool CheckEmptyFieldsNomina()
        {
            bool isEmpty = true;

            if (string.IsNullOrEmpty(txtServer.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtLogin.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtPwd.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtBDName.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtCuentaCNom.Value))
                isEmpty = false;

            return isEmpty;
        }

        private bool CheckEmptyFieldsAsal()
        {
            bool isEmpty = true;

            if (string.IsNullOrEmpty(txtServer.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtLogin.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtPwd.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtBDName.Value))
                isEmpty = false;

            if (string.IsNullOrEmpty(txtCuentaCAsal.Value))
                isEmpty = false;

            return isEmpty;
        }

        private bool ValidateIPString(string ip)
        {
            var parts = ip.Split('.');

            bool isValid = parts.Length == 4
                           && !parts.Any(
                               x =>
                               {
                                   int y;
                                   return Int32.TryParse(x, out y) && y > 255 || y < 1;
                               });

            return isValid;
        }
        #endregion

        private SAPbouiCOM.EditText txtBDName;
        private SAPbouiCOM.EditText txtDesc;
        private SAPbouiCOM.StaticText StaticText35;
        private SAPbouiCOM.StaticText StaticText36;
        private SAPbouiCOM.EditText txtLogin;
        private SAPbouiCOM.EditText txtPwd;
        private SAPbouiCOM.CheckBox chkActivo;
        private SAPbouiCOM.EditText txtServer;
        private SAPbouiCOM.Folder Folder1;
        private SAPbouiCOM.Folder Folder2;
        private SAPbouiCOM.Button btnGdrNom;
        private SAPbouiCOM.StaticText StaticText42;
        private SAPbouiCOM.StaticText StaticText43;
        private SAPbouiCOM.StaticText StaticText44;
        private SAPbouiCOM.EditText txtBDNameAsal;
        private SAPbouiCOM.EditText txtDescAsal;
        private SAPbouiCOM.StaticText StaticText45;
        private SAPbouiCOM.StaticText StaticText46;
        private SAPbouiCOM.EditText txtLoginAsal;
        private SAPbouiCOM.EditText txtPwdAsal;
        private SAPbouiCOM.CheckBox chkActivoAsal;
        private SAPbouiCOM.EditText txtServAsal;
        private SAPbouiCOM.Button btnGuardarAsal;
        private SAPbouiCOM.StaticText StaticText47;
        private SAPbouiCOM.StaticText StaticText49;
        private SAPbouiCOM.EditText txtCuentaCAsal;

    }
}
