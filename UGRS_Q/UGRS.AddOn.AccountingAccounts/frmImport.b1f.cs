using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UGRS.AddOn.AccountingAccounts.DAO;
using UGRS.AddOn.AccountingAccounts.DTO;
using UGRS.AddOn.AccountingAccounts.Entities;
using UGRS.AddOn.AccountingAccounts.Utils;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.Utility;

namespace UGRS.AddOn.AccountingAccounts
{
    [FormAttribute("UGRS.AddOn.AccountingAccounts.frmImport", "frmImport.b1f")]
    public class frmImport : UserFormBase
    {
        #region Attributes
        //private SAPbobsCOM.Company mObjCompany = null;
        private LoginDAO mObjLoginDAO = null;
        private IList<LoginDTO> mListObjLogin = null;
        private ProgressBarManager mObjProgressBar = null;
        private DBDAO mObjDBDAO = null;
        private IList<DBDTO> mListObjDB = null;
        private SAPbobsCOM.JournalEntries mObjJournalEntries = null;
        private double mDoubleImpoDedu = 0.0, mDoubleImpoPerc = 0.0;
        private string mStrCostingCode = "";
        string mStrAccount = string.Empty;
        string lStrValueCmb = string.Empty;
        List<Nomina> gLstNomina = null;

        private EmployeesDAO mObjEmployeesDAO = new EmployeesDAO();
        #endregion

        #region variables
        int lIntUserSign = DIApplication.Company.UserSignature;
        int lIntSerie = 0;

        #endregion

        #region Constructor
        public frmImport()
        {
            lIntSerie = mObjLoginDAO.GetUserSerie(lIntUserSign);       
            try
            {
                this.UIAPIRawForm.Freeze(true);
                //mObjCompany = (SAPbobsCOM.Company)(Application.SBO_Application.Company.GetDICompany());
                if (DIApplication.Company.Connected)
                {
                    Application.SBO_Application.StatusBar.SetText("Add-on Conectado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    Application.SBO_Application.StatusBar.SetText("Add-on disponible.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblYear = ((SAPbouiCOM.StaticText)(this.GetItem("lblYear").Specific));
            this.lblPeriod = ((SAPbouiCOM.StaticText)(this.GetItem("lblPeriod").Specific));
            this.lblNo = ((SAPbouiCOM.StaticText)(this.GetItem("lblNo").Specific));
            this.txtYear = ((SAPbouiCOM.EditText)(this.GetItem("txtYear").Specific));
            this.txtYear.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.txtYear_LostFocusAfter);
            this.txtNo = ((SAPbouiCOM.EditText)(this.GetItem("txtNo").Specific));
            this.txtNo.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.txtNo_LostFocusAfter);
            this.btnCarga = ((SAPbouiCOM.Button)(this.GetItem("btnCarga").Specific));
            this.btnCarga.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCarga_ClickBefore);
            this.cmbPeriod = ((SAPbouiCOM.ComboBox)(this.GetItem("cmbPeriod").Specific));
            this.btnExtrae = ((SAPbouiCOM.Button)(this.GetItem("btnExtrae").Specific));
            this.btnExtrae.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnExtrae_ClickBefore);
            this.lblTipo = ((SAPbouiCOM.StaticText)(this.GetItem("lblTipo").Specific));
            this.cmbTipo = ((SAPbouiCOM.ComboBox)(this.GetItem("cmbTipo").Specific));
            this.cmbTipo.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cmbTipo_ComboSelectAfter);
            this.btnSalir = ((SAPbouiCOM.Button)(this.GetItem("btnSalir").Specific));
            this.btnSalir.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSalir_ClickBefore);
            this.mObjMtx = ((SAPbouiCOM.Matrix)(this.GetItem("Mtx").Specific));
            this.mTxtDate = ((SAPbouiCOM.EditText)(this.GetItem("Item_0").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            // this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private void OnCustomInitialize()
        {
            mTxtDate.Value = DateTime.Now.ToString("yyyyMMdd");
            txtYear.Value = DateTime.Today.Year.ToString();
            cmbPeriod.Select("SELECCIONE");
            FillCmbTipo();
            cmbTipo.Select("SELECCIONE");
            mObjMtx.AutoResizeColumns();

            gLstNomina = new List<Nomina>();
        }

        private SAPbouiCOM.Button btnSalir;
        private SAPbouiCOM.StaticText lblYear;
        private SAPbouiCOM.StaticText lblPeriod;
        private SAPbouiCOM.StaticText lblNo;
        private SAPbouiCOM.EditText txtYear;
        private SAPbouiCOM.EditText txtNo;
        private SAPbouiCOM.Button btnCarga;
        private SAPbouiCOM.ComboBox cmbPeriod;
        private SAPbouiCOM.Button btnExtrae;
        private SAPbouiCOM.StaticText lblTipo;
        private SAPbouiCOM.ComboBox cmbTipo;
        private SAPbouiCOM.DataTable mObjDT;
        //private SAPbouiCOM.Grid Grid0;
        #endregion

        #region Eventos
        private void btnSalir_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Close();
        }

        private void txtYear_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (txtYear.Value != "")
                {
                    if ((txtYear.Value != DateTime.Today.Year.ToString()))
                    {
                        int lIntValidInt = Convert.ToInt32(txtYear.Value.ToString());
                        Check_txtYear(lIntValidInt);
                    }
                }
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case -2146233066:
                    case -2146233033:
                        Application.SBO_Application.StatusBar.SetText("Año Invalido. " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        txtYear.Value = "";
                        break;
                    default:
                        Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        break;
                }
            }
        }

        private void txtNo_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (txtNo.Value != "")
                {
                    int lIntValidInt = Convert.ToInt32(txtNo.Value.ToString());
                    Check_txtNo(lIntValidInt);
                }
            }
            catch (Exception ex)
            {

                switch (ex.HResult)
                {
                    case -2146233066:
                    case -2146233033:
                        Application.SBO_Application.StatusBar.SetText("No. de Quincena Invalida. " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        txtNo.Value = "";
                        break;
                    default:
                        Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        break;
                }
            }
        }

        private void btnCarga_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            mListObjLogin = new List<LoginDTO>();
            AccountsDAO lObjAccounts = null;
            string lStrCtaPuente = "";

            mListObjLogin = GetSetupLogin();
            LoginDTO lObjLoginDTO = mListObjLogin.FirstOrDefault(x => x.Code == int.Parse(lStrValueCmb));

            lStrCtaPuente = lObjLoginDTO.AccountingAccount;
            mStrAccount = lObjLoginDTO.AccountingAccount;

            //LoadList();

            bool lBolResult = Check_txts();
            if (lBolResult && mObjMtx.RowCount > 0)
            {
                //Tipo de Cambio USD
                if (!CheckCurrencyRate())
                {
                    Application.SBO_Application.StatusBar.SetText("Proceso Cancelado. Actualice el tipo de cambio USD.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }
                try
                {
                    this.UIAPIRawForm.Freeze(true);
                    mObjJournalEntries = (SAPbobsCOM.JournalEntries)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

 

                    if (lObjLoginDTO != null)
                    {
                        //Existe Cuentas 
                        string lStrResult = string.Empty;
                        string lStrYear = txtYear.Value;
                        string lStrPeriod = cmbPeriod.Value;
                        string lStrNo = txtNo.Value;

                        Application.SBO_Application.StatusBar.SetText("Procesando cuentas contables... Porfavor espere", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        lObjAccounts = new AccountsDAO();

                        //var lVarGroupAcc = from p in gLstNomina group p by p.CUENTA into grouped select grouped;

                        //lStrResult = lObjAccounts.CheckAccounts(lVarGroupAcc);

                        //if (lStrResult != string.Empty)
                        //{
                        //    Application.SBO_Application.StatusBar.SetText("Proceso Cancelado. Actualice las cuentas." + lStrResult, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        //    return;
                        //}

                        //Application.SBO_Application.StatusBar.SetText("Procesando centros de costos... Porfavor espere", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        ////Existe Centro de Costos  
                        //lStrResult = string.Empty;
                        //lObjAccounts = new AccountsDAO();

                        //var lVarGpCC = from p in gLstNomina group p by p.CUENTA2 into grouped select grouped;

                        ////lStrResult = lObjAccounts.CheckCostingCode(lVarGpCC);

                        //if (lStrResult != string.Empty)
                        //{
                        //    Application.SBO_Application.StatusBar.SetText("Proceso Cancelado. Actualice los Centros de Costo." + lStrResult, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        //    return;
                        //}

                        //mDoubleImpoDedu = 0.0;
                        //mDoubleImpoPerc = 0.0;

                        ////Obtener Clave de SAP en empleados
                        //List<EmployeesDTO> lLstEmployeesDTO = mObjEmployeesDAO.GetEmployeeID();

                        //var lObjQuery = from nom in gLstNomina
                        //                join emp in lLstEmployeesDTO
                        //                    on nom.ATRAB equals emp.IdEmpSAP
                        //                select new { nom, emp };

                        //List<string> lLstMissingEmp = new List<string>();
                        //foreach (var lObjQItem in lObjQuery)
                        //{
                        //    if (lObjQItem.emp.IdEmpNomina == null)
                        //    {
                        //        lLstMissingEmp.Add(lObjQItem.emp.FullName);
                        //    }


                        //    lObjQItem.nom.ATRAB = (int?)lObjQItem.emp.IdEmpNomina;
                        //}

                        //if (lLstMissingEmp.Count > 0)
                        //{
                        //    string lStrMissingEmp = string.Empty;
                        //    foreach (var lObjEmpItem in lLstMissingEmp.Distinct().ToList())
                        //    {
                        //        lStrMissingEmp += lObjEmpItem + ", ";
                        //    }

                        //    Application.SBO_Application.MessageBox("Para continuar, registre en SAP los siguientes empleados: " + lStrMissingEmp);
                        //    NewSearch();

                        //    return;
                        //}

                        mObjJournalEntries = PopulateJournalEntries(); //encabezado
                        mObjJournalEntries = PopulateDetails(mObjJournalEntries, lStrCtaPuente); //lineas

                        int lIntRespJournal = mObjJournalEntries.Add();
                        if (lIntRespJournal == 0)
                        {
                            NewSearch();
                            Application.SBO_Application.MessageBox("Proceso terminado. Se cargo con exito el asiento contable");
                            Application.SBO_Application.StatusBar.SetText("Proceso terminado. Se cargo con exito el asiento contable", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            int lIntError = 0;
                            string lStrError = string.Empty;
                            string xdxd = DIApplication.Company.GetLastErrorDescription();
                            DIApplication.Company.GetLastError(out lIntError, out lStrError);

                            Application.SBO_Application.StatusBar.SetText("Proceso Cancelado. Error al cargar el asiento contable " + lStrError, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("No se puedo establecer la conexión con el Servidor de Nómina", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    }
                }
                catch (Exception ex)
                {
                    Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                finally
                {
                    this.UIAPIRawForm.Freeze(false);
                    MemoryUtility.ReleaseComObject(mObjJournalEntries);

                    //btnCarga.Item.Enabled = false;
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("No hay datos a importar. Favor de primero Buscar Nómina", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        private string GetAccount(string pStrAccount)
        {
            string lStrAccount = pStrAccount;

            if(string.IsNullOrEmpty(lStrAccount))
            {
                lStrAccount = mStrAccount;
            }
            else
            {
                if(lStrAccount.Length < 13)
                {
                   lStrAccount = lStrAccount.PadRight(13, '0');
                }
            }

            return lStrAccount;
        }

        private SAPbobsCOM.JournalEntries PopulateJournalEntries()
        {

            DateTime lObjDtDate = DateTime.ParseExact(mTxtDate.Value, "yyyyMMdd",
                CultureInfo.InvariantCulture);

            ////encabezado
            mObjJournalEntries.Series = 19;
            mObjJournalEntries.ReferenceDate = lObjDtDate;
            mObjJournalEntries.DueDate = lObjDtDate;
            mObjJournalEntries.TaxDate = lObjDtDate;
            mObjJournalEntries.Memo = cmbPeriod.Selected.Description.ToString() + "  " + txtNo.Value + "/" + DateTime.Today.Year.ToString(); //"CAT 1/17";
            mObjJournalEntries.TransactionCode = "NOM";
            mObjJournalEntries.Reference = string.Format("{0}-{1}-{2}", cmbPeriod.Value, txtNo.Value, txtYear.Value);
            mObjJournalEntries.AutoVAT = SAPbobsCOM.BoYesNoEnum.tNO;
            return mObjJournalEntries;
        }

        private SAPbobsCOM.JournalEntries PopulateDetails(JournalEntries pObjJournalEntries,string pStrBridgeAcc)
        {
            double lDbTotDebit = 0;
            double lDbTotCredit = 0;
            try
            {
                mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando", mObjDT.Rows.Count);

              for (int i = 0; i < mObjDT.Rows.Count; i++)
                {
                    int lIntACONC = Convert.ToInt32(mObjDT.Columns.Item("ACONC").Cells.Item(i).Value.ToString());
                    double lDbAIMPO = double.Parse(mObjDT.Columns.Item("AIMPO").Cells.Item(i).Value.ToString());
                    string lStrAcct = GetAccount(mObjDT.Columns.Item("CUE").Cells.Item(i).Value.ToString());


                    pObjJournalEntries.Lines.SetCurrentLine(i);
                    pObjJournalEntries.Lines.AccountCode = Regex.Replace(lStrAcct, @"[$' ']", "");

                    if (lIntACONC >= 1 && lIntACONC <= 100)
                    {
                        pObjJournalEntries.Lines.Debit = lDbAIMPO;

                        lDbTotDebit += lDbAIMPO;

                        mStrCostingCode = mObjDT.Columns.Item("CC").Cells.Item(i).Value.ToString();
                    }
                    if (lIntACONC >= 101 && lIntACONC <= 200)
                    {
                        pObjJournalEntries.Lines.Credit = lDbAIMPO;
                        lDbTotCredit += lDbAIMPO;
                    }

                    pObjJournalEntries.Lines.CostingCode = mStrCostingCode; // centro de costo
                    pObjJournalEntries.Lines.UserFields.Fields.Item("U_FolioFiscal").Value = mObjDT.Columns.Item("UUID").Cells.Item(i).Value.ToString().Trim();

                    pObjJournalEntries.Lines.UserFields.Fields.Item("U_GLO_AuxName").Value = mObjDT.Columns.Item("NNOM").Cells.Item(i).Value.ToString();
                    pObjJournalEntries.Lines.Add();

                    mObjProgressBar.NextPosition();
                }

              pObjJournalEntries.Lines.SetCurrentLine(mObjDT.Rows.Count);
              pObjJournalEntries.Lines.AccountCode = pStrBridgeAcc; //cuenta
              pObjJournalEntries.Lines.Credit = (lDbTotDebit - lDbTotCredit); //abono//Deduccion
              pObjJournalEntries.Lines.LineMemo = "Deposito a Banco Nómina"; //comentario
              pObjJournalEntries.Lines.Add();

              mObjProgressBar.NextPosition();
            }
            catch (Exception er)
            {
                throw er;
            }
            finally
            {
                mObjProgressBar.Dispose();
            }
            return pObjJournalEntries;
        }

        private void btnExtrae_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            mListObjLogin = new List<LoginDTO>();

            if (btnExtrae.Caption == "Nueva Captura")
            {
                NewSearch();
                return;
            }

            if (btnExtrae.Caption == "Buscar Nómina")
            {
                bool Result = Check_txts();

                if (Result)
                {
                    Enabled_txt(false, "btnCarga");
                    Enabled_btn(true, "btnCarga");

                    string lStrYear = txtYear.Value;
                    string lStrPeriod = cmbPeriod.Value;
                    string lStrNo = txtNo.Value;

                    try
                    {
                        this.UIAPIRawForm.Freeze(true);

                        LoginDTO lObjLogin = GetSetupLogin().FirstOrDefault();

                        if (lObjLogin != null)
                        {
                            Application.SBO_Application.StatusBar.SetText("Buscando información... Porfavor espere", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                            List<int> lLstEmpId = new List<int>();
                            EmployeesDAO lObjEmp = new EmployeesDAO();
                            btnExtrae.Caption = "Nueva Captura";

                            //List<Nomina> lLstNomina = new List<Nomina>();
                            AccountsDAO lObjAccount = new AccountsDAO();
                            gLstNomina = new List<Nomina>();

                            LoadMatrix(lStrYear, lStrPeriod, lStrNo, lObjLogin.NameServer, lObjLogin.NameDB, lObjLogin.Login, lObjLogin.Password);


                            //gLstNomina = lObjAccount.GetAccounts(lStrYear, lStrPeriod, lStrNo, lObjLogin.NameServer, lObjLogin.NameDB, lObjLogin.Login, lObjLogin.Password); //lObjEmpItem


                            //if(gLstNomina.Count > 0) {
                            //    lLstNomina.AddRange(gLstNomina);
                            //}
                            //else {
                            //    Application.SBO_Application.StatusBar.SetText("Sin registros para mostrar", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            //}

                            //ConvertListIntoDataTable(lLstNomina);

                            //Grid0.DataTable = this.UIAPIRawForm.DataSources.DataTables.Item("DT_0");
                            //Grid0.AutoResizeColumns();

                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("No se puedo establecer la conexión con el Servidor de Nómina", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    finally
                    {
                        this.UIAPIRawForm.Freeze(false);
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("No hay datos capturados para extraer. Favor de primero capturar datos", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private void LoadMatrix(string lStrYear, string lStrPeriod, string lStrNo, string NameServer, string NameDB, string Login, string Password)
        {
            AccountsDAO lObjAccount = new AccountsDAO();

            this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix")
             .ExecuteQuery(lObjAccount.GetAccts(lStrYear, lStrPeriod, lStrNo, NameServer, NameDB, Login, Password));


            mObjMtx.Columns.Item("clICD").DataBind.Bind("DTMatrix", "ICDNOM");
            mObjMtx.Columns.Item("cAIMPO").DataBind.Bind("DTMatrix", "AIMPO");
            mObjMtx.Columns.Item("cNIVEL").DataBind.Bind("DTMatrix", "NIVEL");
            mObjMtx.Columns.Item("cCUE").DataBind.Bind("DTMatrix", "CUE");
            //mObjMtx.Columns.Item("cCUE").DataBind.Bind("DTMatrix", "ACONC");
            //mObjMtx.Columns.Item("cCUE").DataBind.Bind("DTMatrix", "ATRAB");
            mObjMtx.Columns.Item("cCC").DataBind.Bind("DTMatrix", "CC");
            mObjMtx.Columns.Item("cPROY").DataBind.Bind("DTMatrix", "PROY");
            mObjMtx.Columns.Item("cAcc1").DataBind.Bind("DTMatrix", "CUENTA1");
            mObjMtx.Columns.Item("cNNOM").DataBind.Bind("DTMatrix", "NNOM");
            mObjMtx.Columns.Item("cNRFC").DataBind.Bind("DTMatrix", "NRFC");


            mObjDT = this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix");

            mObjMtx.LoadFromDataSource();
            mObjMtx.AutoResizeColumns();
        }



        private void cmbTipo_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (cmbTipo.Value == "SELECCIONE")
            {
            }
            else
            {
                lStrValueCmb = cmbTipo.Value;
            }
        }

        #endregion

        #region Metodos
        private bool CheckCurrencyRate()
        {
            Tools lObjTools = new Tools();
            bool lboolCurrencyRate = false;
            return lboolCurrencyRate = lObjTools.CheckCurrencyRate(DIApplication.Company);
        }

        private string CheckAccounts(SqlConnection pObjSqlCon, SqlCommand pObjSqlCmd, SAPbobsCOM.Company pObjCompany, string pStrYear, string pStrPeriodo, string pStrNo)
        {
            Tools lObjTools = new Tools();
            string lStrReturn = "";

            string lStrResp = lObjTools.CheckAccounts(pObjSqlCon, pObjSqlCmd, pObjCompany, pStrYear, pStrPeriodo, pStrNo);
            if (lStrResp == "")
                lStrReturn = "";
            else
                lStrReturn = lStrResp;
            return lStrReturn;
        }

        private string CheckCostingCode(SqlConnection pObjSqlCon, SqlCommand pObjSqlCmd, SAPbobsCOM.Company pObjCompany, string pStrYear, string pStrPeriodo, string pStrNo)
        {
            Tools lObjTools = new Tools();
            string lStrReturn = "";

            string lStrResp = lObjTools.CheckCostingCode(pObjSqlCon, pObjSqlCmd, pObjCompany, pStrYear, pStrPeriodo, pStrNo);
            if (lStrResp == "")
                lStrReturn = "";
            else
                lStrReturn = lStrResp;
            return lStrReturn;
        }

        private void Check_txtYear(int pIntYear)
        {
            int lIntValidInt = pIntYear;
            if ((lIntValidInt > 0) && (lIntValidInt <= 9999))
            {
                switch (txtYear.Value.Length)
                {
                    case 4:
                        //año menor o igual al año actual
                        if (lIntValidInt <= DateTime.Today.Year)
                        {
                            //es correcto
                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Año Invalido.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            txtYear.Value = "";
                        }
                        break;
                    default:
                        Application.SBO_Application.StatusBar.SetText("Año Invalido.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        txtYear.Value = "";
                        break;
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Año Invalido.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                txtYear.Value = "";
            }
        }

        private void NewSearch()
        {
            gLstNomina.Clear();
            Enabled_txt(true, "btnExtrae");
            Enabled_btn(true, "btnExtrae");
            Clear_txts();
            Clear_Grid();
            btnExtrae.Caption = "Buscar Nómina";
        }

        private void Check_txtNo(int pInt_txtNo)
        {
            int lIntValid_txtNo = pInt_txtNo;
            if ((lIntValid_txtNo > 0) && (lIntValid_txtNo <= 200))
            {
                //No de quincena permitida
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("No. de Quincena Invalida.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                txtNo.Value = "";
            }
        }

        private void Enabled_btn(bool pBooEnabled, string pStrDefButton)
        {
            btnExtrae.Item.Enabled = pBooEnabled; //!pBooEnabled;  //deshabilita
            btnCarga.Item.Enabled = pBooEnabled;    //habilita            
            UIAPIRawForm.DefButton = pStrDefButton; //default
        }

        private void Enabled_txt(bool pBooEnabled, string pStrDefButton)
        {
            txtYear.Item.Enabled = pBooEnabled;
            txtYear.Active = true;
            cmbPeriod.Item.Enabled = pBooEnabled;
            txtNo.Item.Enabled = pBooEnabled;
            cmbTipo.Item.Enabled = pBooEnabled;
            txtYear.Active = true;
        }

        private void Clear_Grid()
        {
            mObjMtx.Clear();
            //if(!this.UIAPIRawForm.DataSources.DataTables.Item("DT_0").IsEmpty)
            //    this.UIAPIRawForm.DataSources.DataTables.Item("DT_0").Rows.Clear();
        }

        private void Clear_txts()
        {
            txtYear.Value = "";
            cmbPeriod.Select("SELECCIONE");
            txtNo.Value = "";
            cmbTipo.Select("SELECCIONE");
        }

        private bool Check_txts()
        {
            bool pCancel = true;
            if ((txtYear.Value == null) || (txtYear.Value == ""))
            {
                pCancel = false;
            }
            if ((cmbPeriod.Value == null) || (cmbPeriod.Value == "") || (cmbPeriod.Value == "SELECCIONE"))
            {
                pCancel = false;
            }
            if ((txtNo.Value == null) || (txtNo.Value == ""))
            {
                pCancel = false;
            }
            if ((cmbTipo.Value == null) || (cmbTipo.Value == "") || (cmbTipo.Value == "SELECCIONE"))
            {
                pCancel = false;
            }
            if (!pCancel)
            {
                Application.SBO_Application.StatusBar.SetText("Captura Incompleta", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            return pCancel;
        }

        private IList<LoginDTO> GetSetupLogin()
        {
            mObjLoginDAO = new LoginDAO();
            mListObjLogin = new List<LoginDTO>();
            //mListObjLogin = mObjLoginDAO.GetLoginServer();
            mListObjLogin = mObjLoginDAO.GetLoginServer();
            return mListObjLogin;
        }

        private void ConvertListIntoDataTable(List<Nomina> pLstNomina)
        {


            // var timer = Stopwatch.StartNew();
            ProgressBarManager lObjProgressBar = null;
            SAPbouiCOM.DataTable lObjDataTable = this.UIAPIRawForm.DataSources.DataTables.Item("DT_0");
            try
            {

                Parallel.For(0, pLstNomina.Count, _ =>
                {
                    lObjDataTable.Rows.Add();
                });

                lObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando", pLstNomina.Count);


                Parallel.ForEach(Partitioner.Create(0, pLstNomina.Count), (range, state) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        Parallel.ForEach(pLstNomina[0].GetType().GetProperties(), property =>
                        {
                            if (property.Name != "NNOM" && property.Name != "NRFC")
                                lObjDataTable.SetValue(property.Name, i, pLstNomina[i].GetType().GetProperty(property.Name).GetValue(pLstNomina[i], null));
                        });
                    }
                    lObjProgressBar.NextPosition();
                });

            }
            catch (Exception lObjException)
            {
                throw lObjException;
            }
            finally
            {
                if (lObjProgressBar != null)
                {
                    lObjProgressBar.Stop();
                    lObjProgressBar.Dispose();
                }
            }

        }

        private void FillCmbTipo()
        {
            //OnCustomInitialize
            try
            {
                string lStrCode = "", lStrValue = "";
                IList<LoginDTO> lListObjDB = new List<LoginDTO>();
                lListObjDB = GetDBOfComboBox();

                this.cmbTipo.ValidValues.Add("SELECCIONE", "SELECCIONE");
                foreach (LoginDTO lObjDBDTO in lListObjDB)
                {
                    lStrCode = lObjDBDTO.Code.ToString();
                    lStrValue = lObjDBDTO.Descripcion;
                    this.cmbTipo.ValidValues.Add(lStrCode, lStrValue);
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private IList<LoginDTO> GetDBOfComboBox()
        {
            //FillcmbTipo
            mObjLoginDAO = new LoginDAO();
            mListObjLogin = new List<LoginDTO>();

            IList<LoginDTO> lListObjDB = new List<LoginDTO>();
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
        #endregion

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            mObjMtx.AutoResizeColumns();
        }

        private SAPbouiCOM.Matrix mObjMtx;
        private SAPbouiCOM.EditText mTxtDate;
        private SAPbouiCOM.StaticText StaticText0;
    }
}
