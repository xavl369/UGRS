using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGRS.AddOn.Cuarentenarias.Models;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.AddOn.Cuarentenarias.DAO;
using UGRS.Core.Utility;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI;
using UGRS.Core.Exceptions;
using UGRS.AddOn.Cuarentenarias.Services;

namespace UGRS.AddOn.Cuarentenarias
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.frmIns", "Forms/frmIns.b1f")]
    public class frmIns : UserFormBase
    {
        private SAPbouiCOM.Button btnOk;
        private SAPbouiCOM.Button btnFind;
        private SAPbouiCOM.EditText lObjTxtDate;
        private SAPbouiCOM.StaticText lblDate;
        private SAPbouiCOM.Matrix lObjMatrix;
        private SAPbouiCOM.DataTable lObjDTableInsp;
        private SAPbouiCOM.Button lObjBtnCer;
        private SAPbouiCOM.Button lObjBtnDelInsp;
        private SAPbouiCOM.Button Button1;
        SAPbobsCOM.Documents lObjGoodsIssues = null;
        SAPbobsCOM.Documents lObjDocumentGR = null;
        SAPbouiCOM.CheckBox lObjCheckboxInsp = null;

        public int SerialNumber = 0;
        public string SeriesName = "";
        public string DateInsp = "";

        int lIntActiveRow = -1;

        int lIntUsrSignature = DIApplication.Company.UserSignature;//get user name
        bool lBoolInspectioning = false;


        MFormInspection mFormInsp = null;
        MFormInspectionDetails mFormInspDet = null;
        MFormCertificates mFormCert = null;


        Menu pObjMenu = new Menu();


        InspectionDAO mObjinspectionDAO = new InspectionDAO();
        RejectedDAO mObjRejectedDAO = new RejectedDAO();

        public List<Inspeccion> lstInspeccion = new List<Inspeccion>();
        InspeccionService lObjInspeccionService = new InspeccionService();
        CertificateService lObjCertificateService = new CertificateService();
        List<CertificateDTO> lstCertificate = new List<CertificateDTO>();
        IList<CertificateDTO> lIlstCertificates = null;

        string lStrPrincipalRWhs = "";
        int lIntUserSign = DIApplication.Company.UserSignature;//get user sign
        bool lBoolLine = false;
        bool lBoolitmChange = false;


        public frmIns()
        {
            LoadEvents();
            lObjTxtDate.Value = mObjinspectionDAO.GetServerDate();
            lStrPrincipalRWhs = mObjinspectionDAO.GetMainWhs(lIntUserSign);
            LogUtility.WriteInfo("Modulo Inspección de ganado Inicializado");

        }
        #region Load & Unload Events
        private void LoadEvents()
        {
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        private void UnLoadEvents()
        {
            Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion


        #region default
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {

            this.btnOk = ((SAPbouiCOM.Button)(this.GetItem("btnOkIns").Specific));
            this.btnFind = ((SAPbouiCOM.Button)(this.GetItem("BtnFind").Specific));
            this.lObjTxtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDateIns").Specific));
            this.lblDate = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.lObjMatrix = ((SAPbouiCOM.Matrix)(this.GetItem("mtxInsp").Specific));
            this.ConfigForm();
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btnAddD").Specific));
            this.lObjBtnCer = ((SAPbouiCOM.Button)(this.GetItem("btnCert").Specific));
            this.lObjBtnDelInsp = ((SAPbouiCOM.Button)(this.GetItem("btnDel").Specific));
            this.OnCustomInitialize();

        }
        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {



        }

        private void OnCustomInitialize()
        {

        }

        #endregion

        private void ConfigForm()
        {
            this.UIAPIRawForm.Left = 400;
            this.UIAPIRawForm.Top = 100;
        }

        private void SetValues()
        {
            DateInsp = lObjTxtDate.Value.ToString();

        }

        /// <summary>
        /// SBO_Application_ItemEvent
        /// Metodo para controlar los eventos dentro de la pantalla de Facturas de Proveedor y Modal
        /// @Author RomanCordova
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        /// 
        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            string y = pVal.CharPressed.ToString();
            try
            {
                if (pVal.FormTypeEx.Equals("133"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:

                                int x = pVal.CharPressed;
                                break;

                        }
                    }

                }

                if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("BtnFind"))
                                {
                                    LogUtility.WriteInfo("Busqueda Iniciada");
                                    LoadMatrix();
                                }
                                else if (pVal.ItemUID.Equals("btnOkIns"))
                                {
                                    LogUtility.WriteInfo("Iniciar Modal Inspección");
                                    if (AnyLineChecked()) // Validacion para seleccionar al menos una linea Luis Enriquez
                                    {
                                        SetValues();
                                        ShowMFormInspection();
                                    }

                                }
                                else if (pVal.ItemUID.Equals("btnAddD"))
                                {
                                    LogUtility.WriteInfo("Iniciar Modal Detalles de Inspección");
                                    if (AnyLineChecked())
                                    {
                                        ShowMFormInspectionDetails();
                                    }
                                }
                                else if (pVal.ItemUID.Equals("btnCert"))
                                {
                                    if (AnyLineChecked())
                                    {
                                        ShowMFormCert();
                                    }
                                }
                                else if (pVal.ItemUID.Equals("btnDel"))
                                {
                                    LogUtility.WriteInfo("Eliminando Inspección");
                                    if (AnyLineChecked())
                                    {
                                        Deleted();
                                    }
                                }

                                break;

                            case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                                if (pVal.ColUID.Equals("Col_0"))
                                {
                                    checkSameInspections(pVal.Row);
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnLoadEvents();
                                pObjMenu.pArrTypeCount[1] = 0;
                                pObjMenu.pArrTypeEx[1] = "";
                                LogUtility.WriteInfo("Modulo Inspección de ganado Cerrado");
                                break;

                        }
                    }
                }

                #region ModalForm Inspection
                else if (FormUID.Equals("frmModIns"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:

                                if (pVal.ItemUID.Equals("btnModOk"))
                                {
                                    
                                    if (mFormInsp.btnAceptar.Item.Enabled == true)
                                    {
                                        if (mFormInsp.ValidInspection())

                                            if (ShowConfirmDialog())
                                            {
                                                LogUtility.WriteInfo("Guardando Inspección");
                                                lBoolInspectioning = true;
                                                mFormInsp.SaveInspection();
                                                LoadMatrix();
                                                lBoolInspectioning = false;
                                            }
                                    }

                                }
                                if (pVal.ItemUID.Equals("btnModNo"))
                                {

                                    Application.SBO_Application.Forms.Item("frmModIns").Close();
                                    LogUtility.WriteInfo("Proceso Cancelado");
                                }

                                break;

                            case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                                if (pVal.ItemUID.Equals("chkInsE"))
                                {
                                    if (mFormInsp.chkInspEsp.Checked)
                                    {
                                        mFormInsp.ConfigInspectionEsp();
                                    }
                                    else
                                    {
                                        mFormInsp.ConfigSpetialInspAct();
                                    }


                                }
                                break;


                            case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                                if (pVal.ItemUID.Equals("MtxCert") && pVal.ColUID.Equals("Col_0"))
                                {
                                    mFormInsp.SetRequestValue(pVal.Row, lstInspeccion);
                                }
                                //if (pVal.ItemUID.Equals("txtNP") && mFormInsp != null && lBoolitmChange == true)
                                //{
                                //    mFormInsp.ValidInspection();
                                //lBoolitmChange = false;
                                //}


                                break;

                            case SAPbouiCOM.BoEventTypes.et_GOT_FOCUS:
                                if (pVal.ItemUID.Equals("MtxCert") && pVal.ColUID.Equals("Col_0") && mFormInsp.lObjMatrixCert.Item.Enabled == false)
                                {
                                    mFormInsp.SetFocusTXT();
                                }
                                break;


                        }
                    }
                    //else
                    //{
                    //    switch (pVal.EventType)
                    //    {
                    //        case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                    //            if (pVal.ItemUID.Equals("txtNP") && pVal.ItemChanged && mFormInsp != null)
                    //            {
                    //                if (Convert.ToInt32(mFormInsp.txtNP.Value) > 0)
                    //                {
                    //                    lBoolitmChange = true;
                    //                }
                    //            }
                    //            break;
                    //    }
                    ////}
                }
                #endregion

                #region ModalForm InspectionDetails

                else if (FormUID.Equals("frmInspDet"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("btnAddID") && mFormInspDet.btnAdd.Item.Enabled == true)
                                {
                                    mFormInspDet.AddRow();
                                }

                                if (pVal.ItemUID.Equals("btnOkID") && mFormInspDet.btnOk.Item.Enabled == true)
                                {
                                    if (mFormInspDet.SaveInspectionDetails())
                                    {
                                        Application.SBO_Application.StatusBar.SetText("Detalles guardados Correctamente"
                                           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                        Application.SBO_Application.Forms.Item("frmInspDet").Close();
                                        LoadMatrix();
                                    }
                                }
                                if (pVal.ItemUID.Equals("btnModD") && mFormInspDet.btnMod.Item.Enabled == true)
                                {

                                    mFormInspDet.DeleteRow();

                                }

                                if (pVal.ItemUID.Equals("btnNoID") && mFormInspDet.btnCancel.Item.Enabled == true)
                                {

                                    Application.SBO_Application.Forms.Item("frmInspDet").Close();

                                }


                                break;

                            case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                                string uy = pVal.CharPressed.ToString();
                                break;

                        }
                    }
                    else
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                                if (pVal.CharPressed == 27)
                                {
                                    BubbleEvent = false;
                                }
                                break;
                        }
                    }


                }

                #endregion


            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
                LogUtility.WriteInfo(ex.Message);
            }

        }

        private void Deleted()
        {
            lstInspeccion = new List<Inspeccion>();
            lstCertificate = new List<CertificateDTO>();
            bool lBoolGoodIssues = false;
            bool lBoolGoodRecipts = false;

            for (int i = 1; i <= lObjMatrix.RowCount; i++)
            {
                if (((dynamic)lObjMatrix.Columns.Item("Col_0").Cells.Item(i).Specific).Checked)
                {

                    ListDeleted(i);
                }
            }

            if (ValidateSelectCombo())
            {
                if (ValidateIdInspection())
                {
                    if (lstInspeccion.Count > 0)
                    {

                        
                        if (lstInspeccion[0].IdInspection == 0 || !(DrafOrInvoice(lstInspeccion[0].IdInspection.ToString()) == "FACTURADO"))
                        {
                            if (lstInspeccion[0].RBatchNumber != "")
                            {
                                lBoolGoodIssues = RejectedGoodsIssue();
                                if (lBoolGoodIssues == false)
                                {

                                    Application.SBO_Application.StatusBar.SetText("Esta Inspección ya tiene salidas, no se puede eliminar"
                                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return;
                                }
                            }
                            if (lstInspeccion[0].BatchNumber != "")
                            {
                                lBoolGoodRecipts = NewGoodsRecipts();
                            }
                            if (lBoolGoodRecipts && lBoolGoodIssues || (lBoolGoodRecipts))
                            {
                                foreach (var lvarItm in lstInspeccion)
                                {
                                    if (lstInspeccion[0].IdInspection != 0)
                                    {
                                        DeleteCertificate(mObjinspectionDAO.GetCertRowCode(lvarItm.IdInspection.ToString()));
                                    }
                                    UpdateTable(lvarItm.RowCode);
                                }
                            }
                            else
                            {
                                if (lstInspeccion[0].IdInspection == 0)
                                {
                                    foreach (var lvarItm in lstInspeccion)
                                    {
                                        UpdateTable(lvarItm.RowCode);
                                    }
                                }
                            }



                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Esta inspección no se puede eliminar debido a que ya esta facturada"
                                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                        //}

                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Diferentes Inspecciones"
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Diferentes clientes"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private string DrafOrInvoice(string lStrIdInsp)
        {
            string x = mObjinspectionDAO.GetDraftOrInvoice(lStrIdInsp);
            return mObjinspectionDAO.GetDraftOrInvoice(lStrIdInsp);

        }

        private void DeleteCertificate(int lIntRowCode)
        {
            if (lIntRowCode > 0)
            {
                if (lObjCertificateService.DeleteInspectionDetails(lIntRowCode.ToString()) != 0)
                {
                    Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                }
            }

        }

        private void UpdateTable(long lLongRowCode)
        {

            if (lObjInspeccionService.UpdateInspeccion(GetInspToUpdateCancel((int)lLongRowCode)) != 0)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
            }
            else
            {
                LoadMatrix();
                Application.SBO_Application.StatusBar.SetText("Inspeccion eliminada correctamente"
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private Tables.InspeccionT GetInspToUpdateCancel(int lIntCode)
        {
            InspeccionT lOjInspT = new InspeccionT();
            foreach (var item in lstInspeccion.Where(x => x.RowCode == lIntCode))
            {
                lOjInspT.RowCode = lIntCode.ToString();
                lOjInspT.IDInsp = item.IdInspection;
                lOjInspT.DateInsp = Convert.ToDateTime(item.Date);
                lOjInspT.DateSys = Convert.ToDateTime(item.Date);
                lOjInspT.User = lIntUserSign;
                lOjInspT.CardCode = item.CardCode;
                lOjInspT.WhsCode = item.WhsCode;
                lOjInspT.Cancel = "Y";
                lOjInspT.CheckInsp = "N";
                lOjInspT.TotKls = (float)item.TotalKg;
                lOjInspT.Quantity = item.Heads;
                lOjInspT.QuantityNP = item.NP;
                lOjInspT.QuantityReject = item.RE;
                lOjInspT.PaymentCustom = item.PaymentCustom;
                lOjInspT.Classification = item.Type;
                lOjInspT.Series = item.Series;

            }
            return lOjInspT;
        }



        private bool NewGoodsRecipts(/*long lLongRowCode*/)
        {
            int lIntResult = 0;
            bool lBoolValid = false;
            lObjDocumentGR = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
            try
            {
                lObjDocumentGR.UserFields.Fields.Item("U_GLO_InMo").Value = "E-GAN";
                lObjDocumentGR.UserFields.Fields.Item("U_PE_Certificate").Value = "0";
                lObjDocumentGR.UserFields.Fields.Item("U_GLO_Guide").Value = "0";
                lObjDocumentGR.UserFields.Fields.Item("U_GLO_CheckIn").Value = DateTime.Now.ToString("HH:mm");

                foreach (var item in lstInspeccion)
                {

                    lObjDocumentGR.Series = (int)item.Series;
                    lObjDocumentGR.DocDate = DateTime.Today;
                    lObjDocumentGR.DocDueDate = DateTime.Today;
                    lObjDocumentGR.UserFields.Fields.Item("U_GLO_BusinessPartner").Value = item.CardCode;


                    //Lines
                    lObjDocumentGR.Lines.ItemCode = item.Type;

                    lObjDocumentGR.Lines.WarehouseCode = item.WhsCode;

                    lObjDocumentGR.Lines.Quantity = item.Heads;

                    //Sumatoria NP +RE
                    lObjDocumentGR.Lines.BatchNumbers.Quantity = item.Heads;
                    //Concatenar ALIAS+IDINSP+FECHAINGRESO

                    lObjDocumentGR.Lines.BatchNumbers.BatchNumber = item.BatchNumber;
                    lObjDocumentGR.Lines.BatchNumbers.ManufacturerSerialNumber = item.CardCode;
                    lObjDocumentGR.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = DateTime.Now.ToString("HH:mm");


                    lObjDocumentGR.Lines.BatchNumbers.Add();

                    lObjDocumentGR.Lines.Add();
                }
                lIntResult = lObjDocumentGR.Add();
                if (lIntResult != 0)
                {
                    string error = DIApplication.Company.GetLastErrorDescription();
                    Application.SBO_Application.StatusBar.SetText(error
                                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    lBoolValid = false;
                }
                else
                {
                    lBoolValid = true;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocumentGR);
            }
            return lBoolValid;
        }

        private bool RejectedGoodsIssue()
        {
            int lIntRetCode = 0;
            int lIntSeries = mObjRejectedDAO.GetSerieForOutputs(lIntUserSign);
            bool lBoolValid = false;
            bool lBoolValidQ = false;
            lObjGoodsIssues = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
            var lVarInspections = from p in lstInspeccion group p by p.IdInspection into grupo select grupo;
            //int lIntSumNPRJ = (int)lstInspeccion.Sum(x => x.RE + x.NP);
            int lIntSumNPRJ = 0; 

            try
            {
                foreach (var lVar in lVarInspections)
                {
                    foreach (var item in lVar.Where(x => x.RE > 0 || x.NP >0))
                    {
                        lIntSumNPRJ = (int)(item.NP + item.RE);
                        if (HasGoodIssues(item.RBatchNumber) >= lIntSumNPRJ)
                        {
                            if ((item.NP + item.RE) > 0)
                            {
                                lObjGoodsIssues.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                                lObjGoodsIssues.Series = lIntSeries;
                                lObjGoodsIssues.DocDate = DateTime.Today;
                                lObjGoodsIssues.DocDueDate = DateTime.Today;


                                lObjGoodsIssues.Lines.ItemCode = item.Type;
                                lObjGoodsIssues.Lines.WarehouseCode = lStrPrincipalRWhs;

                                lObjGoodsIssues.Lines.Quantity = item.RE + item.NP;///////////////// las np van a rechazos?.
                                lObjGoodsIssues.Lines.BatchNumbers.Quantity = item.RE + item.NP;


                                lObjGoodsIssues.Lines.BatchNumbers.BatchNumber = item.RBatchNumber;


                                lObjGoodsIssues.Lines.BatchNumbers.Add();

                                lObjGoodsIssues.Lines.Add();

                                lIntRetCode = lObjGoodsIssues.Add();

                                lBoolValidQ = true;
                            }
                        }
                        else
                        {
                            lBoolValidQ = false;

                        }

                    }
                    if (lIntRetCode != 0 && lBoolValidQ == true)
                    {
                        string lStrError = DIApplication.Company.GetLastErrorDescription();
                        Application.SBO_Application.StatusBar.SetText(lStrError
           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        lBoolValid = false;

                    }
                    else
                    {
                        if (lBoolValidQ)
                        {
                            lBoolValid = true;
                        }
                        else
                        {
                            lBoolValid = false;
                        }
                    }

                }


            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjGoodsIssues);
            }
            return lBoolValid;
        }

        private double HasGoodIssues(string lStrBatch)
        {
            double lIntQntty = mObjinspectionDAO.GetQuantityRejected(lStrBatch);

            return lIntQntty;
        }

        private void ListDeleted(int lIntRow)
        {
            Inspeccion lObjInspection = new Inspeccion();



            lObjInspection.IdInspection = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("ID", lIntRow - 1);
            lObjInspection.Date = Convert.ToString(this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Fecha", lIntRow - 1));
            lObjInspection.WhsCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("WhsCode", lIntRow - 1);
            lObjInspection.Heads = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Cantidad", lIntRow - 1);
            lObjInspection.NP = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("NP", lIntRow - 1);
            lObjInspection.RE = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("RE", lIntRow - 1);
            lObjInspection.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("CardCode", lIntRow - 1);
            lObjInspection.Type = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Tipo", lIntRow - 1);
            lObjInspection.RowCode = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Code", lIntRow - 1);
            lObjInspection.Series = Convert.ToInt64(this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Serie", lIntRow - 1));
            lObjInspection.BatchNumber = mObjinspectionDAO.GetBatchLine(lObjInspection.CardCode, lObjInspection.WhsCode);
            string date = Convert.ToDateTime(lObjInspection.Date).ToString("yyyy-MM-dd");
            lObjInspection.RBatchNumber = mObjinspectionDAO.GetBatchToCancel(date, lObjInspection.CardCode, lStrPrincipalRWhs, lObjInspection.IdInspection.ToString());

            lstInspeccion.Add(lObjInspection);



        }



        private void ShowMFormCert()
        {

            if (ValidateIdInspection())
            {
                if (IdInspection())
                {
                    mFormCert = new MFormCertificates("frmModCert.xml", "frmModCert", lstInspeccion);
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Este registro aún no cuenta con un Id de inspección"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }

            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Debe seleccionar el mismo Id Inspección"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ShowConfirmDialog()
        {
            int result = SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Esta seguro que desea guardar la inspección", 1, "Ok", "Cancelar", "");
            if (result == 1)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        private void ShowMFormInspection()
        {
            GetCheckedInspectionList();

            if (ValidateSelectCombo())
            {
                if (ValidateTipoGanado())
                {
                    if (!IdInspection())
                    {
                        mFormInsp = new MFormInspection("frmModIns.xml", "frmModIns", lstInspeccion);
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("Este registro ya cuenta con un Id de Inspección"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("Registro(s) con ID de inspección");
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Seleccione solamente una clasificación de ganado"
                         , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteInfo("Clasificaciones de ganado distintas");
                }

            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Seleccione solamente un cliente"
     , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("Socios de negocio diferentes");
            }
        }

        /// <summary>
        /// Method ShowMFormInspectionDetails()
        /// Show a Modal form Inspection Details
        /// <remarks>@Author RCordova - 2017/08/14</remarks>
        /// </summary>
        private void ShowMFormInspectionDetails()
        {
            GetCheckedInspectionList();


            if (ValidateIdInspection())
            {
                if (IdInspection())
                {
                    if (lstInspeccion[0].NP > 0 || lstInspeccion[0].RE > 0)
                    {
                        mFormInspDet = new MFormInspectionDetails("frmInspDet.xml", "frmInspDet", lstInspeccion);
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("Esta inspección no cuenta con No presentadas ni Rechazos"
                               , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("Inspección sin registros de NP o Rechazos");
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Este registro aún no cuenta con un Id de inspección"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteInfo("El registro no cuenta con ID de Inspección");
                }

            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Debe seleccionar el mismo Id Inspección"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("Diferentes ID de Inspección");
            }


        }

        private bool IdInspection()
        {
            if (lstInspeccion.FindAll(x => x.IdInspection == 0).Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author RCordova </remarks>
        /// <returns></returns>
        private bool ValidateIdInspection()
        {
            if (lstInspeccion.Count == lstInspeccion.FindAll(x => x.IdInspection == lstInspeccion[0].IdInspection).Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        private void GetCheckedInspectionList()
        {
            lstInspeccion = new List<Inspeccion>();

            for (int i = 1; i < lObjMatrix.RowCount + 1; i++)
            {
                Inspeccion lObjInspeccion = new Inspeccion();

                if (((dynamic)lObjMatrix.Columns.Item(1).Cells.Item(i).Specific).Checked)
                {
                    //lObjInspeccion.User = lObjCompany.UserSignature;
                    lObjInspeccion.User = lIntUsrSignature;
                    lObjInspeccion.Series = GetUserSerialNumber();
                    lObjInspeccion.SeriesName = SeriesName;
                    lObjInspeccion.RowCode = Convert.ToInt64(this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Code", i - 1).ToString());
                    lObjInspeccion.Date = ((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(2).Cells.Item(i).Specific).Value;
                    lObjInspeccion.CardCode = this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("CardCode", i - 1).ToString();
                    lObjInspeccion.Client = ((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(4).Cells.Item(i).Specific).Value;
                    lObjInspeccion.IdInspection = Convert.ToInt64(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(5).Cells.Item(i).Specific).Value);
                    lObjInspeccion.WhsCode = this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("WhsCode", i - 1).ToString();
                    lObjInspeccion.Corral = ((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(3).Cells.Item(i).Specific).Value;
                    lObjInspeccion.Item = ((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(6).Cells.Item(i).Specific).Value;
                    lObjInspeccion.Type = this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp").GetValue("Tipo", i - 1).ToString();
                    lObjInspeccion.Heads = Convert.ToInt64(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(7).Cells.Item(i).Specific).Value);
                    lObjInspeccion.NP = Convert.ToInt64(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(10).Cells.Item(i).Specific).Value);
                    lObjInspeccion.RE = Convert.ToInt64(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(9).Cells.Item(i).Specific).Value);
                    lObjInspeccion.Status = ((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(11).Cells.Item(i).Specific).Value;
                    lObjInspeccion.TotalKg = float.Parse(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item(7).Cells.Item(i).Specific).Value);

                    lstInspeccion.Add(lObjInspeccion);
                }
            }
        }

        /// <summary>
        /// ValidateSelectCombo
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        private bool ValidateSelectCombo()
        {
            if (lstInspeccion.Count == lstInspeccion.FindAll(x => x.Client == lstInspeccion[0].Client).Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        /// <returns></returns>
        private bool ValidateTipoGanado()
        {
            if (lstInspeccion.Count == lstInspeccion.FindAll(x => x.Type == lstInspeccion[0].Type).Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        private void FillDataSource()
        {
            long SerialNumber = GetUserSerialNumber();

            this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrixInsp")
             .ExecuteQuery(mObjinspectionDAO.GetInspectionList(lObjTxtDate.Value, SerialNumber));
            LogUtility.WriteInfo("Datos de consulta: " + lObjTxtDate.Value + " " + SerialNumber);
        }

        /// <summary>
        /// LoadMatrix Method
        /// 
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        private void LoadMatrix()
        {
            FillDataSource();

            lObjMatrix.Columns.Item("Col_0").DataBind.Bind("DTMatrixInsp", "Chk");
            lObjMatrix.Columns.Item("ColDate").DataBind.Bind("DTMatrixInsp", "Fecha");
            lObjMatrix.Columns.Item("ColCorral").DataBind.Bind("DTMatrixInsp", "Corral");
            lObjMatrix.Columns.Item("ColClient").DataBind.Bind("DTMatrixInsp", "Cliente");
            lObjMatrix.Columns.Item("ColID").DataBind.Bind("DTMatrixInsp", "ID");
            lObjMatrix.Columns.Item("ColTipo").DataBind.Bind("DTMatrixInsp", "Item");
            lObjMatrix.Columns.Item("ColHeads").DataBind.Bind("DTMatrixInsp", "Cantidad");
            lObjMatrix.Columns.Item("ColKg").DataBind.Bind("DTMatrixInsp", "TotalKg");
            lObjMatrix.Columns.Item("ColRej").DataBind.Bind("DTMatrixInsp", "RE");
            lObjMatrix.Columns.Item("ColNP").DataBind.Bind("DTMatrixInsp", "NP");
            lObjMatrix.Columns.Item("ColStatus").DataBind.Bind("DTMatrixInsp", "Estatus");
            lObjMatrix.Columns.Item("ColDet").DataBind.Bind("DTMatrixInsp", "Detalles");



            lObjMatrix.LoadFromDataSource();

        }


        /// <summary>
        /// GetUserSerialNumber()
        /// Metodo para obtener el numero de serie del usuario conectado en SAP
        /// <remarks>@Author RCordova </remarks>
        /// </summary>
        /// <returns></returns>
        private long GetUserSerialNumber()
        {
            long UserSignature = lIntUsrSignature;

            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery("SELECT t0.series,SeriesName FROM NNM1 t0 INNER JOIN NNM2 t1 ON t0.Series = t1.Series WHERE t1.UserSign = " + UserSignature + " AND t0.ObjectCode=59");

            if (lObjRecordSet.RecordCount == 1)
            {
                SerialNumber = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                SeriesName = lObjRecordSet.Fields.Item(1).Value.ToString();
            }

            MemoryUtility.ReleaseComObject(lObjRecordSet);
            return SerialNumber;
        }

        private bool AnyLineChecked()
        {
            int lIntChecked = 0;
            for (int i = 1; i <= lObjMatrix.RowCount; i++)
            {
                if (((dynamic)lObjMatrix.Columns.Item(1).Cells.Item(i).Specific).Checked)
                {
                    lIntChecked++;
                }
            }

            if (lIntChecked > 0)
            {
                return true;
            }
            else
            {
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Seleccione al menos una línea"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("Ninguna línea seleccionada");
                return false;
            }
        }

        private void checkSameInspections(int lIntActiveRow)
        {
            lObjCheckboxInsp = ValidateRow(lIntActiveRow);

            if (lObjCheckboxInsp != null)
            {
                string lStrIdIspection = Convert.ToString(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item("ColID").Cells.Item(lIntActiveRow).Specific).Value);

                if (lStrIdIspection != "0")
                {
                    if (lObjCheckboxInsp.Checked)
                    {

                        FindSameInspection(lStrIdIspection, "Y");
                    }
                    else
                    {

                        FindSameInspection(lStrIdIspection, "N");
                    }
                }
            }
        }

        private SAPbouiCOM.CheckBox ValidateRow(int lIntRow)
        {
            SAPbouiCOM.CheckBox lObjCheckBoxT = null;
            try
            {

                lObjCheckBoxT = (SAPbouiCOM.CheckBox)lObjMatrix.Columns.Item("Col_0").Cells.Item(lIntRow).Specific;
                return lObjCheckBoxT;
            }
            catch (Exception ex)
            {
                return lObjCheckBoxT = null;
            }
        }

        private void FindSameInspection(string lStrInspection, string lStrAction)
        {

            for (int i = 1; i <= lObjMatrix.RowCount; i++)
            {
                if (Convert.ToString(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item("ColID").Cells.Item(i).Specific).Value) == lStrInspection)
                {
                    lObjCheckboxInsp = (SAPbouiCOM.CheckBox)lObjMatrix.Columns.Item("Col_0").Cells.Item(i).Specific;

                    lObjCheckboxInsp.Caption = lStrAction;
                }

            }
        }

        


    }
}
