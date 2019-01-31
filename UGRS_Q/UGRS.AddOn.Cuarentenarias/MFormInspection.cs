using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UGRS.AddOn.Cuarentenarias.DAO;
using UGRS.AddOn.Cuarentenarias.Models;
using UGRS.AddOn.Cuarentenarias.Services;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.AddOn.Cuarentenarias.Utils;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Cuarentenarias
{
    /// <summary>
    /// MFormInspeccion Class
    /// </summary>
    public class MFormInspection
    {

        SAPbouiCOM.Form lObjModalForm;

        #region Items
        public SAPbouiCOM.EditText txtNP;
        public SAPbouiCOM.EditText txtRE;
        SAPbouiCOM.EditText txtTotalKg;
        SAPbouiCOM.EditText txtTime;
        public SAPbouiCOM.Button btnAceptar;
        SAPbouiCOM.Button btnCancelar;
        //SAPbouiCOM.Button btnValidar;
        SAPbouiCOM.Matrix lObjMatrixInsp;
        public SAPbouiCOM.Matrix lObjMatrixCert;
        SAPbouiCOM.DataTable oDTInsp;
        SAPbouiCOM.DataTable oDTCert;
        //SAPbobsCOM.Company lObjCompany;
        public SAPbouiCOM.CheckBox chkAduana;
        public SAPbouiCOM.CheckBox chkInspEsp;

        #endregion


        //Lista para guardar los certificados a evaluar
        List<CertificateDTO> lstCertificateMatrix = new List<CertificateDTO>();
        List<CertificateDTO> LstCertificate = new List<CertificateDTO>();

        List<Inspeccion> lstInspeccion = new List<Inspeccion>();

        List<CertificateDTO> lstCertificateT = new List<CertificateDTO>();
        IList<CertificateDTO> IlstCertificateDTO = null;
        InspectionDAO inspectionDAO = new InspectionDAO();

        InspeccionService inspService = new InspeccionService();
        CertificateService certService = new CertificateService();

        public String Clasification = "";

        int lIntNPforSpecialInsp = 0;
        int lIntRjSpecial = 0;
        int lIntHeads = 0;
        int lIntHeadRjCounter = 0;
        int lIntHeadNPCounter = 0;

        bool lBoolHeadQty = false;

        bool lBoolRJ = false;
        bool lBoolNp = false;

        int lIntUserSignature = DIApplication.Company.UserSignature;

        //frmIns lObjFrmInsp = new frmIns();

        utils Utils = new utils();

        public MFormInspection()
        {

            //initFormXml();
            //lObjCompany = (SAPbobsCOM.Company)SAPbouiCOM.Framework.Application.SBO_Application.Company.GetDICompany();
            //LoadMatrixCertificate();
            //initMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        public MFormInspection(String FileName, String FormName, List<Inspeccion> LstInspeccion)
        {
            lstInspeccion = LstInspeccion;
            LoadFromXml(FileName, FormName);
            LoadMatrixCertificate();
            LoadMatrix();
            //LoadEvents();


        }

        //private void LoadEvents()
        //{
        //    this.txtTotalKg.LostFocusAfter += this.LostFocus;
        //}

        //private void LostFocus(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        //{
        //    lObjMatrixInsp.SelectRow(1, true, false);
        //}



        /// <summary>
        /// Metodo LoadFromXml
        /// Metodo para cargar un Form a partir de un archivo XML
        /// @Author RomanCordova
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FormName"></param>
        private void LoadFromXml(string FileName, string FormName)
        {
            System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
            //string sPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]).ToString();
            string sPath = PathUtilities.GetCurrent("XmlForms");

            oXmlDoc.Load(sPath + "\\" + FileName);

            SAPbouiCOM.FormCreationParams creationPackage = (SAPbouiCOM.FormCreationParams)Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            creationPackage.XmlData = oXmlDoc.InnerXml;

            if (FormName.Equals("frmModIns"))
            {

                if (!Utils.FormExists(FormName))
                {
                    creationPackage.UniqueID = FormName;
                    creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                    creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
                    creationPackage.FormType = "frmModIns";
                    //creationPackage.

                    lObjModalForm = Application.SBO_Application.Forms.AddEx(creationPackage);
                    lObjModalForm.Title = "Inspección de Ganado";
                    lObjModalForm.Left = 400;
                    lObjModalForm.Top = 100;
                    lObjModalForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    lObjModalForm.Visible = true;
                    initFormXml();

                }
                else
                {
                    lObjModalForm.Select();
                }
            }

        }

        /// <summary>
        /// Metodo initFormXml
        /// Metodo para inicializar el Formulario Modal
        /// @Author RomanCordova
        /// </summary>
        private void initFormXml()
        {
            lObjModalForm.Freeze(true);
            setItems();
            SetFocusTXT();
            lObjModalForm.Freeze(false);
        }

        /// <summary>
        /// Metodo initItems
        /// Metodo para asignar los items de la pantalla modal
        /// @Author RomanCordova
        /// </summary>
        private void setItems()
        {
            txtTime = ((SAPbouiCOM.EditText)lObjModalForm.Items.Item("txtTime").Specific);
            txtRE = ((SAPbouiCOM.EditText)lObjModalForm.Items.Item("txtRE").Specific);
            txtNP = ((SAPbouiCOM.EditText)lObjModalForm.Items.Item("txtNP").Specific);
            btnAceptar = ((SAPbouiCOM.Button)lObjModalForm.Items.Item("btnModOk").Specific);
            txtTotalKg = ((SAPbouiCOM.EditText)lObjModalForm.Items.Item("txtTotkg").Specific);
            lObjMatrixInsp = ((SAPbouiCOM.Matrix)lObjModalForm.Items.Item("MtxInsp").Specific);
            lObjMatrixCert = ((SAPbouiCOM.Matrix)lObjModalForm.Items.Item("MtxCert").Specific);
            chkAduana = ((SAPbouiCOM.CheckBox)lObjModalForm.Items.Item("chkAdu").Specific);
            chkInspEsp = ((SAPbouiCOM.CheckBox)lObjModalForm.Items.Item("chkInsE").Specific);
            btnCancelar = ((SAPbouiCOM.Button)lObjModalForm.Items.Item("btnModNo").Specific);

            initChkBox();
            FormatForm();
            initMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        private void initChkBox()
        {
            lObjModalForm.DataSources.UserDataSources.Add("UDCL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            lObjModalForm.DataSources.UserDataSources.Add("UDDL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);

            chkAduana.DataBind.SetBound(true, "", "UDCL");
            chkInspEsp.DataBind.SetBound(true, "", "UDDL");
        }

        /// <summary>
        /// FormarForm Method
        /// Metodo para aplicar el formato al formulario
        /// </summary>
        private void FormatForm()
        {
            //lObjModalForm.Freeze(true);
            lObjMatrixInsp.AutoResizeColumns();
            lObjMatrixCert.Columns.Item("Col_1").Editable = false;
            lObjMatrixCert.AutoResizeColumns();
            lObjMatrixInsp.Item.Enabled = false;
            CheckValues();
            SetFocusTXT();
            ////lObjModalForm.Freeze(false);
        }
        /// <summary>
        /// 
        /// </summary>
        private void initMatrix()
        {
            #region Matrix Certificado

            lObjModalForm.DataSources.DataTables.Add("DtCertificado");
            oDTCert = lObjModalForm.DataSources.DataTables.Item("DtCertificado");

            oDTCert.Columns.Add("Col_0", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTCert.Columns.Add("Col_1", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            oDTCert.Rows.Add();

            #endregion

            #region Matrix Inspeccion

            lObjModalForm.DataSources.DataTables.Add("DtInspeccion");

            oDTInsp = lObjModalForm.DataSources.DataTables.Item("DtInspeccion");

            oDTInsp.Columns.Add("Col_0", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTInsp.Columns.Add("Col_1", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTInsp.Columns.Add("Col_2", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            #endregion

        }



        public bool ValidateCertificate(List<Inspeccion> LstInspeccion, string lStrCert)
        {
            lstCertificateT = new List<CertificateDTO>();
            int NCertificate = 0;
            bool Flag = true;
            long TotalHeads = GetTotalHeads();
            int lIntTotalQuantity = 0;

            if (lstCertificateMatrix.Count > 1)
            {
                for (int i = 0; i < lstCertificateMatrix.Count; i++)
                {
                    NCertificate = Convert.ToInt32(lstCertificateMatrix[i].NCertificate);

                    IlstCertificateDTO = inspectionDAO.ValidateCertificate(NCertificate);
                    ((List<CertificateDTO>)lstCertificateT).AddRange(IlstCertificateDTO);
                }

            }
            if (lstCertificateMatrix.Count == 1)
            {
                NCertificate = Convert.ToInt32(lStrCert);
                IlstCertificateDTO = inspectionDAO.ValidateCertificate(NCertificate);
                ((List<CertificateDTO>)lstCertificateT).AddRange(IlstCertificateDTO);
            }

            foreach (var item in lstCertificateT)
            {
                lIntTotalQuantity += (int)item.Quantity;
            }


            //IF ( Comparar Lista de Certificados
            if (Flag && TotalHeads <= lIntTotalQuantity)
            {
                if (lstCertificateT.Count > 0 && lstCertificateMatrix.Count == lstCertificateT.Count)
                {
                    if (lstCertificateT[0].Status == "Y")
                    {
                        List<CertificateDTO> lstCerTemp = new List<CertificateDTO>();

                        for (int i = 0; i < lstCertificateT.Count; i++)
                        {
                            string lstFecha = LstInspeccion[0].Date.Substring(6, 2) + "/" + LstInspeccion[0].Date.Substring(4, 2) + "/" + LstInspeccion[0].Date.Substring(0, 4);
                            DateTime ldtmFecha = DateTime.Parse(lstFecha);
                            DateTime ldtmFechaCertificado = DateTime.Parse(lstCertificateT[i].UsedDate);
                            CertificateDTO lObjCertificate = new CertificateDTO();

                            // IF ( Comparar fechas de certificados )
                            if (ldtmFechaCertificado < ldtmFecha)
                            {
                                Application.SBO_Application.StatusBar.SetText("La fecha del Certificado expiró"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                LogUtility.WriteInfo("Certificado Vencido");
                                lBoolHeadQty = true;
                                Flag = false;
                            }
                            else
                            {
                                //IF (Validación de Tipo de Ganado)
                                if (lstCertificateT[i].TypeG == LstInspeccion[0].Type)
                                {
                                    lObjCertificate.UsedDate = lstCertificateT[i].UsedDate;
                                    lObjCertificate.NCertificate = lstCertificateT[i].NCertificate;
                                    lObjCertificate.TypeG = lstCertificateT[i].TypeG;
                                    lObjCertificate.Quantity = lstCertificateT[i].Quantity;
                                    lObjCertificate.Serie = lstCertificateMatrix[i].Serie;

                                    lstCerTemp.Add(lObjCertificate);
                                }
                                else
                                {
                                    Application.SBO_Application.StatusBar.SetText("El tipo de ganado del certificado no es valido"
                               , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    LogUtility.WriteInfo("El tipo de ganado no coincide en el certificado");
                                    lstCertificateT = new List<CertificateDTO>();
                                    lBoolHeadQty = true;
                                    //Flag = false;
                                    return false;

                                }//FIN Else(Validación de Tipo de Ganado)
                                if (lstCertificateT[i].CardCode != LstInspeccion[0].CardCode)
                                {
                                    Application.SBO_Application.StatusBar.SetText("El certificado no coincide con el socio de negocio"
                             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    LogUtility.WriteInfo("El socio de negocio no coincide con el certificado utilizado");
                                    lstCertificateT = new List<CertificateDTO>();
                                    lBoolHeadQty = true;
                                    //Flag = false;
                                    return false;

                                }
                                // if (Flag && TotalHeads > lstCertificateT[i].Quantity)
                                // {
                                //     Application.SBO_Application.StatusBar.SetText("La cantidad de cabezas de ganado no es valida para este certificado"
                                //, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                //     lstCertificateT = new List<CertificateDTO>();
                                //     Flag = false;
                                // }
                                else
                                {
                                    if (Flag)
                                    {
                                        LstCertificate = lstCerTemp;
                                    }

                                }
                            }
                            //FIN Else ( Validación fechas )
                        }
                        // Fin For para recorrer Lista
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("El Certificado #" + NCertificate + " ya fue utilizado anteriormente"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("El certificado ya fue utilizado: " + NCertificate);
                        lstCertificateT = new List<CertificateDTO>();
                        lBoolHeadQty = true;
                        Flag = false;
                    }

                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("El Certificado #" + NCertificate + " no se encuentra registrado"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteInfo("El certificado no esta registrado: " + NCertificate);
                    lstCertificateT = new List<CertificateDTO>();
                    lBoolHeadQty = true;
                    Flag = false;

                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("La cantidad de cabezas de ganado no es valida para este certificado"
                             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("La cantidad de cabezas no coincide con las del certificado");
                lstCertificateT = new List<CertificateDTO>();
                lBoolHeadQty = false;
                Flag = false;
            }

            if (Flag)
            {
                btnAceptar.Item.Enabled = true;
                lObjModalForm.Items.Item("txtTotkg").Click(SAPbouiCOM.BoCellClickType.ct_Regular);

            }

            return Flag;

        }


        public bool ValidCertificate(List<Inspeccion> lLstInspection)
        {
            lstCertificateT = new List<CertificateDTO>();
            int lIntNCertificate = 0;
            int lIntCertQtty = 0;
            int lIntTotalHeads = (int)GetTotalHeads();
            bool lBoolValid = false;
            string lStrTypeG = lLstInspection.Select(x => x.Type).FirstOrDefault();

            if (lstCertificateMatrix.Count > 1)
            {
                for (int i = 0; i < lstCertificateMatrix.Count; i++)
                {
                    lIntNCertificate = Convert.ToInt32(lstCertificateMatrix[i].NCertificate);

                    IlstCertificateDTO = inspectionDAO.ValidateCertificate(lIntNCertificate).Where(x => x.TypeG == lStrTypeG).ToList();
                    ((List<CertificateDTO>)lstCertificateT).AddRange(IlstCertificateDTO);
                }

            }
            if (lstCertificateMatrix.Count == 1)
            {
                lIntNCertificate = Convert.ToInt32(lstCertificateMatrix[0].NCertificate);
                IlstCertificateDTO = inspectionDAO.ValidateCertificate(lIntNCertificate).Where(x => x.TypeG == lStrTypeG).ToList();
                ((List<CertificateDTO>)lstCertificateT).AddRange(IlstCertificateDTO);
            }

            if (IlstCertificateDTO.Count > 0)
            {
                lIntCertQtty = (int)lstCertificateT.Sum(x => x.Quantity);

                if (lIntTotalHeads <= lIntCertQtty)
                {
                    string d = DateTime.Parse(lstCertificateT.Select(x => x.UsedDate).FirstOrDefault()).ToShortDateString();

                    if (lstCertificateT.Where(x => x.Status == "N" && DateTime.Parse(x.UsedDate).ToShortDateString() != DateTime.Now.ToShortDateString()).ToList().Count == 0)
                    {
                        foreach (var lVarCert in lstCertificateT)
                        {
                            //if (lIntTotalHeads <= lVarCert.Quantity)
                            //{
                            if (lVarCert.TypeG == lStrTypeG)
                            {
                                if (lVarCert.CardCode == lLstInspection.Select(x => x.CardCode).FirstOrDefault())
                                {
                                    CertificateDTO lObjCertificateDTO = new CertificateDTO();

                                    lObjCertificateDTO.UsedDate = lVarCert.UsedDate;
                                    lObjCertificateDTO.NCertificate = lVarCert.NCertificate;
                                    lObjCertificateDTO.TypeG = lVarCert.TypeG;
                                    lObjCertificateDTO.Serie = lVarCert.Serie;
                                    lObjCertificateDTO.Quantity = lVarCert.Quantity >= lIntTotalHeads ? lIntTotalHeads : lVarCert.Quantity;
                                    lObjCertificateDTO.Status = lVarCert.Status;

                                    lIntTotalHeads = (int)lVarCert.Quantity >= lIntTotalHeads ? lIntTotalHeads - lIntTotalHeads : lIntTotalHeads - (int)lVarCert.Quantity;

                                    LstCertificate.Add(lObjCertificateDTO);
                                    lBoolValid = true;
                                }
                                else
                                {
                                    Application.SBO_Application.StatusBar.SetText("El certificado no coincide con el socio de negocio"
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    LogUtility.WriteInfo("El socio de negocio no coincide con el certificado utilizado");
                                    lstCertificateT = new List<CertificateDTO>();
                                    lBoolHeadQty = true;
                                }
                            }
                            else
                            {
                                Application.SBO_Application.StatusBar.SetText("El tipo de ganado del certificado no es valido"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                LogUtility.WriteInfo("El tipo de ganado no coincide en el certificado");
                                lstCertificateT = new List<CertificateDTO>();
                                lBoolHeadQty = true;
                            }
                            //}
                        }
                    }
                    else
                    {
                        List<long> lstUsedCert = lstCertificateT.Where(x => x.Status == "N").Select(x => x.NCertificate).ToList();
                        string lStrMessage = string.Empty;

                        if (lstCertificateT.Count > 1)
                        {
                            lStrMessage = "Los Certificados" + string.Join(", #", lstUsedCert.ToArray()) + " ya han sido utilizados";
                        }
                        else
                        {
                            lStrMessage = "El Certificado: #" + lstUsedCert[0].ToString() + " ya ha sido utilizado anteriormente";
                        }


                        Application.SBO_Application.StatusBar.SetText(lStrMessage
                     , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo(lStrMessage);
                        lstCertificateT = new List<CertificateDTO>();
                        lBoolHeadQty = true;
                    }
                }
                else
                {

                    if (lstCertificateT.Where(x => x.Status == "N" && x.Quantity == 0).ToList().Count > 0)
                    {
                        string lStrCertificates =

                        string.Format("No hay cabezas disponibles para {0}: {1}", lstCertificateT.Where(x => x.Status == "N" && x.Quantity == 0).Count() == 1 ?
                            "el siguiente certificado" : "los siguientes certificados",
                            string.Join(" ", lstCertificateT.Where(x => x.Status == "N" && x.Quantity == 0).Select(x => string.Format("-{0}", x.NCertificate.ToString()))));


                        Application.SBO_Application.StatusBar.SetText(lStrCertificates
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo(lStrCertificates);
                        lstCertificateT = new List<CertificateDTO>();
                        lBoolHeadQty = true;
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("El certificado no cuenta con la cantidad de cabezas suficiente"
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }

                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("No se encontro el certificado, o el tipo de cabezas no es valido"
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("No se encontro el certificado, o el tipo de cabezas no es valido");
                lstCertificateT = new List<CertificateDTO>();
                lBoolHeadQty = true;
            }


            if (lBoolValid)
            {
                btnAceptar.Item.Enabled = true;
                lObjModalForm.Items.Item("txtTotkg").Click(SAPbouiCOM.BoCellClickType.ct_Regular);

            }

            return lBoolValid;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        /// <returns></returns>
        public bool SaveCertificate()
        {
            Certificate certificate = null;
            bool lBoolResult = false;
            int lIntResult = 0;
            try
            {
                if (LstCertificate.Count > 0)
                {
                    for (int i = 0; i < LstCertificate.Count; i++)
                    {

                        if (LstCertificate[i].Status == "N")
                        {
                            certificate = inspectionDAO.GetCertificate(LstCertificate[i].NCertificate);

                            if (certificate != null)
                            {
                                certificate.Quantity += LstCertificate[i].Quantity;

                                lIntResult = certService.UpdateCertificate(certificate);
                            }
                            else
                            {
                                SICertificates lObjSICertificate = null;

                                lObjSICertificate = inspectionDAO.GetCertificate2(LstCertificate[i].NCertificate);

                                lObjSICertificate.Quantity -= LstCertificate[i].Quantity;

                                lIntResult = certService.UpdateSICert(lObjSICertificate);
                            }
                        }
                        else
                        {

                            DateTime lll = DateTime.Parse(LstCertificate[i].UsedDate);

                            certificate = new Certificate();
                            certificate.RowName = Convert.ToString(LstCertificate[i].NCertificate);
                            certificate.ItemCode = LstCertificate[i].TypeG;
                            certificate.IDInsp = GetLastIdInspection() + 1;
                            certificate.Quantity = LstCertificate[i].Quantity;
                            certificate.Serie = LstCertificate[i].Serie;
                            certificate.UsedDate = DateTime.Parse(LstCertificate[i].UsedDate);

                            lIntResult = certService.SaveCertificate(certificate);
                        }

                        if (lIntResult != 0)
                        {
                            lBoolResult = false;
                        }
                        else
                        {
                            lBoolResult = true;
                        }
                    }
                }
                else
                {
                    if (!lBoolRJ)
                    {
                        Application.SBO_Application.StatusBar.SetText("No se agrego ningun certificado"
                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("No se agrego ningun certificado");
                    }
                    else
                    {
                        lBoolResult = true;
                    }

                }
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteError("Salida sin éxito: " + lObjException.Message);
            }
            return lBoolResult;
        }

        /// <summary>
        /// 
        /// <remarks>@author RCordova-2017/08/16</remarks>
        /// </summary>
        public void SetRequestValue(int pIntRow, List<Inspeccion> pLstInspection)
        {
            SAPbobsCOM.Recordset lObjRecordSet;
            SAPbouiCOM.EditText lStrCertificado = ((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(pIntRow).Specific);
            string lStrValor = ((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(pIntRow).Specific).Value;
            SAPbouiCOM.EditText lStrValorDeEnseguida = ((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(2).Cells.Item(pIntRow).Specific);
            //lObjMatrixCert.SelectRow()
            try
            {
                if (((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(pIntRow).Specific).Value != string.Empty)
                {
                    //for (int i = 1; i < lObjMatrixCert.Columns.Item(1).Cells.Count; i++)
                    //{



                    if (!string.IsNullOrEmpty(lStrValor))
                    {
                        //if()
                        if (string.IsNullOrEmpty(lStrValorDeEnseguida.Value))
                        {
                            if (!SameCertificate(lStrValor, pIntRow))
                            {
                                //lObjRecordSet = (SAPbobsCOM.Recordset)lObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                lObjRecordSet.DoQuery(inspectionDAO.GetCertificateRequest(Convert.ToInt32(lStrValor)));
                                lStrValorDeEnseguida.Value = lObjRecordSet.Fields.Item("Solicitud").Value.ToString();
                            }
                            else
                            {
                                lObjMatrixCert.ClearRowData(pIntRow);
                            }
                        }

                        if (lStrValorDeEnseguida.Value == string.Empty)
                        {
                            Application.SBO_Application.StatusBar.SetText("No se encontro el certificado"
                          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            lStrCertificado.Value = "";
                            lStrValorDeEnseguida.Value = "";
                            LogUtility.WriteInfo("No se encontro el certificado");
                        }
                        else
                        {
                            FillCertificateList(pIntRow);
                            if (ValidCertificate(pLstInspection)) //if (ValidateCertificate(pLstInspection, lStrValor))
                            {
                                Application.SBO_Application.StatusBar.SetText("Certificado Válido"
                          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                LogUtility.WriteInfo("Certificado Válido");
                                lObjMatrixCert.Item.Enabled = false;


                            }
                            else
                            {
                                if (lBoolHeadQty)
                                {
                                    lStrCertificado.Value = "";
                                    lStrValorDeEnseguida.Value = "";
                                    lBoolHeadQty = false;
                                }
                            }
                        }
                    }
                    //}
                }
                else
                {
                    if (lStrValorDeEnseguida.Value != string.Empty)
                    {
                        lStrValorDeEnseguida.Value = "";
                        //((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(pIntRow).Specific).Value = "";
                    }
                }


            }
            catch (Exception ex)
            {

            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lStrValor"></param>
        /// <param name="lIntRow"></param>
        /// <returns></returns>
        private bool SameCertificate(string lStrValor, int lIntRow)
        {
            bool lBoolVal = false;
            for (int i = 1; i < lObjMatrixCert.RowCount; i++)
            {
                if (((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(i).Specific).Value != string.Empty)
                {
                    if (lStrValor == ((SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(i).Specific).Value)
                    {
                        if (i != lIntRow)
                        {
                            lBoolVal = true;
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return lBoolVal;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FillCertificateList(int lIntRow)
        {

            //for (int i = 1; i < lObjMatrixCert.RowCount - 1; i++)
            //{

            CertificateDTO certificadoMatrix = new CertificateDTO();
            SAPbouiCOM.EditText txtNCertificate = (SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(1).Cells.Item(lIntRow).Specific;
            SAPbouiCOM.EditText txtNSerie = (SAPbouiCOM.EditText)lObjMatrixCert.Columns.Item(2).Cells.Item(lIntRow).Specific;

            if (lstCertificateMatrix.Count == 0)
            {
                certificadoMatrix.NCertificate = Convert.ToInt64(txtNCertificate.Value.ToString());
                certificadoMatrix.Serie = Convert.ToInt64(txtNSerie.Value.ToString());
                lstCertificateMatrix.Add(certificadoMatrix);
            }
            else
            {
                try
                {
                    if (lstCertificateMatrix[lIntRow - 1] != null)
                    {
                        lstCertificateMatrix[lIntRow - 1].NCertificate = Convert.ToInt64(txtNCertificate.Value.ToString());
                        lstCertificateMatrix[lIntRow - 1].Serie = Convert.ToInt64(txtNSerie.Value.ToString());
                    }
                }
                catch
                {
                    certificadoMatrix.NCertificate = Convert.ToInt64(txtNCertificate.Value.ToString());
                    certificadoMatrix.Serie = Convert.ToInt64(txtNSerie.Value.ToString());
                    lstCertificateMatrix.Add(certificadoMatrix);
                }
            }



        }


        /// <summary>
        /// 
        /// </summary>
        private void LoadMatrix()
        {

            for (int i = 0; i < lstInspeccion.Count; i++)
            {
                oDTInsp.Rows.Add();

                oDTInsp.Columns.Item("Col_0").Cells.Item(i).Value = lstInspeccion[i].Corral;
                oDTInsp.Columns.Item("Col_1").Cells.Item(i).Value = lstInspeccion[i].Item;
                oDTInsp.Columns.Item("Col_2").Cells.Item(i).Value = lstInspeccion[i].Heads.ToString();


                lObjMatrixInsp.Columns.Item("Col_0").DataBind.Bind("DtInspeccion", "Col_0");
                lObjMatrixInsp.Columns.Item("Col_1").DataBind.Bind("DtInspeccion", "Col_1");
                lObjMatrixInsp.Columns.Item("Col_2").DataBind.Bind("DtInspeccion", "Col_2");

            }

            lObjMatrixInsp.LoadFromDataSource();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        private void DisableMatrixCertificate()
        {
            lObjMatrixCert.Item.Enabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        private void LoadMatrixCertificate()
        {
            for (int i = 0; i < 12; i++)
            {
                oDTCert.Rows.Add();

                oDTCert.Columns.Item("Col_0").Cells.Item(i).Value = "";
                oDTCert.Columns.Item("Col_1").Cells.Item(i).Value = "";

                lObjMatrixCert.Columns.Item("Col_0").DataBind.Bind("DtCertificado", "Col_0");
                lObjMatrixCert.Columns.Item("Col_1").DataBind.Bind("DtCertificado", "Col_1");

            }

            lObjMatrixCert.LoadFromDataSource();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        public void SaveInspection()
        {

            try
            {
                //if (chkInspEsp.Checked || CheckNP())

                if (chkInspEsp.Checked || lBoolNp)
                {
                    GetTotalHeads();
                    SetInspectionValues();

                    //lstInspeccion.Select(x=> {x.SpecialInspection = "Y"; return x;});

                    foreach (var item in lstInspeccion)
                    {
                        item.SpecialInspection = "Y";
                    }
                    //lstInspeccion[0].SpecialInspection = "Y";

                    DIApplication.Company.StartTransaction();
                    if (GoodsIssue() && GoodsReceipt() && UpdateInspection())
                    {
                        DIApplication.Company.EndTransaction(BoWfTransOpt.wf_Commit);
                        Application.SBO_Application.MessageBox("La inspeccion fue realizada correctamente");
                        LogUtility.WriteInfo("Inspección realizada correctamente");
                        lstCertificateMatrix = new List<CertificateDTO>();

                        Application.SBO_Application.Forms.Item("frmModIns").Close();
                    }
                    else
                    {
                        DIApplication.Company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                }
                else
                {
                    GetTotalHeads();
                    SetInspectionValues();

                    DIApplication.Company.StartTransaction();
                    if (SaveCertificate() && GoodsIssue() && GoodsReceipt() && UpdateInspection())
                    {
                        DIApplication.Company.EndTransaction(BoWfTransOpt.wf_Commit);
                        Application.SBO_Application.StatusBar.SetText("La inspeccion fue realizada correctamente"
                             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                        Application.SBO_Application.Forms.Item("frmModIns").Close();
                    }
                    else { DIApplication.Company.EndTransaction(BoWfTransOpt.wf_RollBack); }

                }
            }
            catch (Exception lObjException)
            {
                Application.SBO_Application.MessageBox("Error, " + lObjException.Message);
                LogUtility.WriteError("Error " + lObjException.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        private bool GoodsIssue()
        {
            bool lBoolResult = true;
            SAPbobsCOM.Documents lObjDocumentGI = null;
            SAPbobsCOM.Recordset lObjRecordSet = null;
            try
            {
                int lIntResult = 0;
                long lLonHeads = 0;
                lObjDocumentGI = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

                //lObjDocumentGI.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                lObjDocumentGI.Series = Convert.ToInt32(GetGoodIssueSeries());
                lObjDocumentGI.DocDate = DateTime.Today;
                lObjDocumentGI.DocDueDate = DateTime.Today;
                lObjDocumentGI.UserFields.Fields.Item("U_GLO_BusinessPartner").Value = lstInspeccion[0].CardCode;
                lObjDocumentGI.UserFields.Fields.Item("U_MQ_OrigenFol").Value = lstInspeccion[0].IdInspection.ToString();
                lObjDocumentGI.UserFields.Fields.Item("U_GLO_InMo").Value = "S-GAN";
                lObjDocumentGI.Reference2 = lstInspeccion[0].SeriesName + lstInspeccion[0].IdInspection;

                for (int i = 0; i < lstInspeccion.Count; i++)
                {
                    lLonHeads = lstInspeccion[i].Heads;
                    //lObjDocument.Lines.COGSCostingCode
                    lObjDocumentGI.Lines.ItemCode = lstInspeccion[i].Type;
                    //lObjDocumentGI.Lines.ItemDescription = lstInspeccion[i].Type; ;
                    lObjDocumentGI.Lines.WarehouseCode = lstInspeccion[i].WhsCode;
                    lObjDocumentGI.Lines.Quantity = lstInspeccion[i].Heads;

                    //lObjRecordSet = (SAPbobsCOM.Recordset)lObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    lObjRecordSet.DoQuery(inspectionDAO.GetBatchLines(lstInspeccion[i].CardCode, lstInspeccion[i].WhsCode));

                    for (int y = 0; y < lObjRecordSet.RecordCount; y++)
                    {
                        if (lLonHeads > 0)
                        {

                            lObjDocumentGI.Lines.BatchNumbers.Quantity = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                            lObjDocumentGI.Lines.BatchNumbers.BatchNumber = lObjRecordSet.Fields.Item(1).Value.ToString();
                            //lObjDocumentGI.Lines.BatchNumbers.ManufacturerSerialNumber = lObjRecordSet.Fields.Item(3).Value.ToString();

                            lLonHeads = lLonHeads - Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                            lObjDocumentGI.Lines.BatchNumbers.Add();
                        }
                        lObjRecordSet.MoveNext();

                    }


                    lObjDocumentGI.Lines.Add();
                }

                lIntResult = lObjDocumentGI.Add();
                if (lIntResult != 0)
                {
                    Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                    LogUtility.WriteError("Salida sin éxito: " + DIApplication.Company.GetLastErrorDescription());
                    lBoolResult = false;
                }
                /*
                else
                {
                    DocEntryGI = string.Empty;
                    lObjCompany.GetNewObjectCode(out DocEntryGI);
                }
                */
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                LogUtility.WriteError("Salida sin éxito: " + DIApplication.Company.GetLastErrorDescription());
                //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
                MemoryUtility.ReleaseComObject(lObjDocumentGI);
            }

            return lBoolResult;

        }

        /// <summary>
        /// Method GoodsREceipt
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        private bool GoodsReceipt()
        {
            bool lBoolResult = true;

            SAPbobsCOM.Documents lObjDocumentGR = null;
            SAPbobsCOM.Recordset lObjRecordSetW = null;
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrEntryHour = DateTime.Now.ToString("HH:mm");

            try
            {
                if ((Convert.ToInt32(txtNP.Value) + Convert.ToInt32(txtRE.Value)) > 0)
                {

                    lObjDocumentGR = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);
                    int lIntResult = 0;

                    lObjDocumentGR.Series = Convert.ToInt32(lstInspeccion[0].Series);
                    lObjDocumentGR.DocDate = DateTime.Today;
                    lObjDocumentGR.DocDueDate = DateTime.Today;
                    lObjDocumentGR.UserFields.Fields.Item("U_GLO_BusinessPartner").Value = lstInspeccion[0].CardCode;
                    lObjDocumentGR.UserFields.Fields.Item("U_GLO_InMo").Value = "E-GAN";
                    lObjDocumentGR.UserFields.Fields.Item("U_PE_Certificate").Value = "0";
                    lObjDocumentGR.UserFields.Fields.Item("U_GLO_Guide").Value = "0";
                    lObjDocumentGR.UserFields.Fields.Item("U_GLO_CheckIn").Value = lStrEntryHour;
                    lObjDocumentGR.Reference2 = lstInspeccion[0].SeriesName + lstInspeccion[0].IdInspection;

                    //Lines
                    lObjDocumentGR.Lines.ItemCode = lstInspeccion[0].Type;
                    lObjDocumentGR.Lines.ItemDescription = lstInspeccion[0].Type;

                    lObjRecordSetW = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                    lObjRecordSetW.DoQuery(inspectionDAO.GetWhsCodeGR(lIntUserSignature));
                    if (lObjRecordSetW.RecordCount != 0)
                    {
                        lObjDocumentGR.Lines.WarehouseCode = lObjRecordSetW.Fields.Item(0).Value.ToString();
                    }

                    if (chkInspEsp.Checked)
                    {
                        lObjDocumentGR.Lines.Quantity = GetTotalHeads();
                    }
                    else
                    {
                        lObjDocumentGR.Lines.Quantity = Convert.ToInt32(txtNP.Value) + Convert.ToInt32(txtRE.Value);
                    }

                    lObjDocumentGR.Lines.BatchNumbers.Quantity = Convert.ToInt32(txtNP.Value) + Convert.ToInt32(txtRE.Value);

                    lObjDocumentGR.Lines.BatchNumbers.BatchNumber = "R_" + (GetLastIdInspection() + 1) + "_" + DateTime.Today.Date.ToString("yyyyMMddhhmm");

                    lObjDocumentGR.Lines.BatchNumbers.UserFields.Fields.Item("U_GLO_Time").Value = lStrEntryHour;

                    lObjDocumentGR.Lines.BatchNumbers.ManufacturerSerialNumber = lstInspeccion[0].CardCode;
                    lObjDocumentGR.Lines.BatchNumbers.InternalSerialNumber = Convert.ToString(GetLastIdInspection() + 1);

                    lObjDocumentGR.Lines.BatchNumbers.Add();

                    lObjDocumentGR.Lines.Add();

                    lIntResult = lObjDocumentGR.Add();
                    if (lIntResult != 0)
                    {
                        string lStrerror = DIApplication.Company.GetLastErrorDescription();
                        LogUtility.WriteError("Entrada sin éxito" + lStrerror);
                        lBoolResult = false;
                    }

                }
                else
                {
                    lBoolResult = true;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                LogUtility.WriteError("Entrada sin éxito" + DIApplication.Company.GetLastErrorDescription());
                //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjDocumentGR);
                MemoryUtility.ReleaseComObject(lObjRecordSetW);
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lBoolResult;




        }

        /// <summary>
        /// 
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields()
        {
            bool valid = false;
            lIntHeads = ValidateHeadQuantity();


            if (Regex.IsMatch(txtNP.Value, "^-?\\d*(\\.\\d+)?$") && Regex.IsMatch(txtNP.Value, "^-?\\d*(\\.\\d+)?$") && Regex.IsMatch(txtTotalKg.Value, "^-?\\d*(\\.\\d+)?$"))
            {
                if (Convert.ToInt32(txtNP.Value) >= 0 && Convert.ToInt32(txtRE.Value) >= 0 && Convert.ToDouble(txtTotalKg.Value) >= 0)
                {
                    valid = true;
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Solo números positivos"
                           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("Se ingresaron números negativos");
                return false;
            }


            if (chkInspEsp.Checked || CheckNP() || lIntHeads == 0)
            {
                if (Convert.ToInt32(txtNP.Value) <= lIntNPforSpecialInsp)
                {
                    valid = true;
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Las NP superan la cantidad de cabezas"
                          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteInfo("NP mayores al total de cabezas");
                    lBoolHeadQty = true;
                    return false;
                }
            }
            else
            {
                if (CheckRejected())
                {

                    if (Convert.ToInt32(txtRE.Value) <= lIntNPforSpecialInsp)
                    {
                        valid = true;
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("Los Rechazos superan la cantidad de cabezas"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("Rechazos mayores al total de cabezas");
                        lBoolHeadQty = true;
                        return false;
                    }
                }
                else
                {
                    if (Convert.ToDouble(txtTotalKg.Value) > 0)
                    {
                        if (Convert.ToInt32(txtNP.Value) >= 0 && Convert.ToInt32(txtRE.Value) >= 0)
                        {
                            if (ValidQuantityNPRJ())
                            {
                                if (lObjMatrixCert.Item.Enabled == false)
                                {
                                    valid = true;
                                }
                                else
                                {
                                    Application.SBO_Application.StatusBar.SetText("Agregue un certificado válido"
                                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    LogUtility.WriteInfo("Certificado Inválido");
                                    return false;
                                }
                            }
                            else
                            {
                                Application.SBO_Application.StatusBar.SetText("La cantidad de Rechazo y/o No presentados es mayor a las cabezas en Existencia"
      , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                LogUtility.WriteInfo("Rechazos y/o NP superan el total de cabezas");
                                return false;
                            }
                        }
                    }

                    else
                    {


                        Application.SBO_Application.StatusBar.SetText("Agregue el peso"
            , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("Falta agregar peso");
                        return false;


                    }
                }

            }
            return valid;
        }

        private bool CheckRejected()
        {
            bool lboolValid = false;
            lIntRjSpecial = 0;
            if (txtNP.Value == string.Empty)
            {
                txtNP.Value = "0";
            }
            foreach (var item in lstInspeccion)
            {
                lIntRjSpecial += (int)item.Heads;
            }

            if (Convert.ToInt32(txtRE.Value) == lIntRjSpecial)
            {
                if (ValidQuantityNPRJ())
                {
                    lboolValid = true;
                    lBoolRJ = true;
                }
            }
            else
            {
                lboolValid = false;
            }

            return lboolValid;
        }

        private bool ValidQuantityNPRJ()
        {
            bool lboolvalid = false;
            if (lIntHeads >= 0)
            {
                lboolvalid = true;
            }
            else if (lIntHeads < 0)
            {
                lboolvalid = false;
            }
            return lboolvalid;
        }

        private int ValidateHeadQuantity()
        {
            int lIntRejectedNP = 0;
            int lIntTotal = 0;

            for (int i = 0; i < lObjMatrixInsp.RowCount; i++)
            {
                lIntRejectedNP += Convert.ToInt32(((SAPbouiCOM.EditText)lObjMatrixInsp.Columns.Item("Col_2").Cells.Item(i + 1).Specific).Value);
            }

            return lIntTotal = (lIntRejectedNP) - (Convert.ToInt32(txtRE.Value) + Convert.ToInt32(txtNP.Value));

        }

        private void OnlyNumbers(SAPbouiCOM.EditText lObjEditText, string lStrDot)
        {
            SAPbouiCOM.EditText lObjEditTxtOnlyNumbers = null;

            lObjEditTxtOnlyNumbers = (SAPbouiCOM.EditText)lObjEditText;

            //if (lObjEditTxtOnlyNumbers.Value == "" && lStrDot == "Y")
            //{
            //    lObjEditTxtOnlyNumbers.Value = "0.00";
            //}
            //else if (lObjEditTxtOnlyNumbers.Value == "" && lStrDot == "N")
            //{
            //    lObjEditTxtOnlyNumbers.Value = "0";
            //}

            if (lStrDot == "N")
            {
                lObjEditTxtOnlyNumbers.Value = Regex.Replace(lObjEditTxtOnlyNumbers.Value, @"[^0-9]", "");
                if (lObjEditTxtOnlyNumbers.Value == "")
                {
                    lObjEditTxtOnlyNumbers.Value = "0";
                }
            }
            else
            {
                lObjEditTxtOnlyNumbers.Value = new string(lObjEditTxtOnlyNumbers.Value.Where(c => char.IsDigit(c) || char.IsPunctuation(c)).ToArray());

                if (lObjEditTxtOnlyNumbers.Value.Split('.').Length - 1 >= 2 || lObjEditTxtOnlyNumbers.Value == "")
                {
                    lObjEditTxtOnlyNumbers.Value = "0.00";
                    return;
                }

            }


        }


        /// <summary>
        /// 
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        private bool UpdateInspection()
        {
            if (chkInspEsp.Checked || lIntHeads == 0 || lBoolNp) { }
            else { CalculateAverage(); }

            double lIntTtKg = Convert.ToDouble(txtTotalKg.Value);

            //int lIntRej = Convert.ToInt32(txtRE.Value);
            //int lIntNP = Convert.ToInt32(txtNP.Value);

            bool lBoolResult = true;

            try
            {
                for (int i = 0; i < lstInspeccion.Count; i++)
                {
                    InspeccionT insp = new InspeccionT();
                    insp.RowCode = lstInspeccion[i].RowCode.ToString();
                    insp.User = lstInspeccion[i].User;
                    insp.Series = lstInspeccion[i].Series;
                    insp.WhsCode = lstInspeccion[i].WhsCode;
                    insp.CardCode = lstInspeccion[i].CardCode;
                    insp.IDInsp = lstInspeccion[i].IdInspection;
                    insp.Quantity = lstInspeccion[i].Heads;
                    if (lBoolNp)
                    {
                        insp.PaymentCustom = "N";
                    }
                    else
                    {
                        insp.PaymentCustom = lstInspeccion[i].PaymentCustom;
                    }


                    insp.QuantityNP = lstInspeccion[i].NP;
                    insp.QuantityReject = lstInspeccion[i].RE;
                    //if (lIntRej > 0)
                    //{
                    //    lIntRej -= lIntRej;
                    //}
                    //if (lIntNP > 0)
                    //{
                    //    lIntNP -= lIntNP;
                    //}

                    if (!lBoolRJ)
                    {
                        if (chkInspEsp.Checked || lIntHeads == 0 || lBoolNp) { insp.AverageW = 0; }
                        else
                        {

                            insp.AverageW = float.Parse(lstInspeccion[i].Average.ToString("0.0000"));

                        }
                    }
                    else
                    {
                        insp.AverageW = 0;
                    }

                    insp.TotKls = lstInspeccion[i].TotalKg;
                    lIntTtKg -= lIntTtKg;

                    insp.Cancel = "N";
                    //insp.Payment = "N";
                    string Date = lstInspeccion[i].Date.Substring(6, 2) + "/" + lstInspeccion[i].Date.Substring(4, 2) + "/" + lstInspeccion[i].Date.Substring(0, 4);
                    insp.DateInsp = DateTime.Parse(Date);
                    insp.DateSys = DateTime.Parse(Date);
                    if (!lBoolRJ)
                    {
                        insp.CheckInsp = lstInspeccion[i].SpecialInspection;
                    }
                    else
                    {
                        insp.CheckInsp = "N";
                    }
                    insp.Time = txtTime.Value;

                    if (inspService.UpdateInspeccion(insp) != 0)
                    {
                        lBoolResult = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                LogUtility.WriteError("Error al actualizar la inspección: " + DIApplication.Company.GetLastErrorDescription());
            }

            return lBoolResult;
        }


        /// <summary>
        /// Method SetInspectionValues
        /// Set controls values in inspection list 
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        private void SetInspectionValues()
        {
            long IdInspection = GetLastIdInspection() + 1;
            string lStrCustom = "";
            if (chkAduana.Checked) { lStrCustom = "Y"; }
            else { lStrCustom = "N"; }

            if (chkInspEsp.Checked || lBoolNp)
            {
                for (int i = 0; i < lstInspeccion.Count; i++)
                {
                    lstInspeccion[i].IdInspection = IdInspection;
                    lstInspeccion[i].NP = 0;
                    lstInspeccion[i].RE = 0;
                    lstInspeccion[i].TotalKg = 0;
                    lstInspeccion[i].PaymentCustom = "N";


                }
            }
            else
            {
                for (int i = 0; i < lstInspeccion.Count; i++)
                {
                    lstInspeccion[i].IdInspection = IdInspection;
                    lstInspeccion[i].NP = Convert.ToInt64(txtNP.Value.ToString());
                    lstInspeccion[i].RE = Convert.ToInt64(txtRE.Value.ToString());
                    lstInspeccion[i].TotalKg = float.Parse(txtTotalKg.Value.ToString());
                    lstInspeccion[i].PaymentCustom = lStrCustom;
                }
            }

        }

        /// <summary>
        /// Method ValidateInspection
        /// Validate inspection TextBox & TotalHeads 
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        private bool ValidateInspection()
        {
            if (txtNP.Value == "" && txtRE.Value == "")
            {

            }
            long SumNpRe = Convert.ToInt64(txtNP.Value) + Convert.ToInt64(txtRE.Value);
            if (SumNpRe > 1)
            {

            }
            return true;
        }

        /// <summary>
        /// Method GetTotalHeads
        /// Get totalHead (MatrixInspection)
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        /// <returns>TotalHeads</returns>
        private long GetTotalHeads()
        {
            long TotalHeads = 0;
            for (int i = 0; i < lstInspeccion.Count; i++)
            {
                TotalHeads += Convert.ToInt64(lstInspeccion[i].Heads);
            }

            return TotalHeads;
        }

        /// <summary>
        /// Method GetGoodIssueSeries
        /// Return the GI Series
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        /// <returns>SerialNumber</returns>
        private long GetGoodIssueSeries()
        {
            long UserSignature = lIntUserSignature;
            long SerialNumber = 0;
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                lObjRecordSet.DoQuery("SELECT t0.series,SeriesName FROM NNM1 t0 INNER JOIN NNM2 t1 ON t0.Series = t1.Series WHERE t1.UserSign = " + UserSignature + " AND t0.ObjectCode=60");

                if (lObjRecordSet.RecordCount == 1)
                {
                    SerialNumber = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                }
            }
            catch (Exception lObjException)
            {
                Application.SBO_Application.MessageBox(lObjException.Message);
                LogUtility.WriteError("Error " + lObjException.Message);
                LogUtility.WriteError(DIApplication.Company.GetLastErrorDescription());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return SerialNumber;
        }


        /// <summary>
        /// GetUserSerialNumber()
        /// This Method returns the serial number of the connected user in SAP
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        /// <returns>SerialNumber</returns>
        private long GetUserSerialNumber()
        {
            //long UserSignature = lObjCompany.UserSignature;
            long UserSignature = lIntUserSignature;
            long SerialNumber = 0;

            try
            {
                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery("SELECT t0.series,SeriesName FROM NNM1 t0 INNER JOIN NNM2 t1 ON t0.Series = t1.Series WHERE t1.UserSign = " + UserSignature + " AND t0.ObjectCode=59");

                if (lObjRecordSet.RecordCount == 1)
                {
                    SerialNumber = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                }

                MemoryUtility.ReleaseComObject(lObjRecordSet);

            }
            catch (Exception lObjException)
            {
                Application.SBO_Application.MessageBox(lObjException.Message);
                LogUtility.WriteError("Error " + lObjException.Message);
                LogUtility.WriteError(DIApplication.Company.GetLastErrorDescription());
            }

            return SerialNumber;
        }


        /// <summary>
        /// Method GetLastIdInspection
        /// This Method returns the last id Ispection .
        /// <remarks>@Author Rcordova </remarks>
        /// </summary>
        /// <returns></returns>
        private long GetLastIdInspection()
        {
            long LastId = 0;
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                lObjRecordSet.DoQuery("SELECT Top 1 U_IDInsp FROM [@UG_CU_OINS] ORDER BY U_IDInsp DESC");

                LastId = Convert.ToInt64(lObjRecordSet.Fields.Item(0).Value.ToString());
            }
            catch (Exception lObjException)
            {
                Application.SBO_Application.MessageBox(lObjException.Message);
                LogUtility.WriteError("Error " + lObjException.Message);
                LogUtility.WriteError(DIApplication.Company.GetLastErrorDescription());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }


            return LastId;
        }

        /// <summary>
        /// Method CalculateAverage
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        private void CalculateAverage()
        {
            lIntHeadRjCounter = Convert.ToInt32(txtRE.Value);
            lIntHeadNPCounter = Convert.ToInt32(txtNP.Value);
            long TotalKg = Convert.ToInt64(txtTotalKg.Value);
            long TotCabezasAverage = 0;
            long TotCabezas = 0;
            double average = 0;
            float totKilos = 0;
            float SumTotKilos = 0;

            TotCabezas = GetTotalHeads();
            TotCabezasAverage = GetTotalHeads() - (Convert.ToInt32(txtNP.Value.ToString()) + Convert.ToInt32(txtRE.Value.ToString()));
            average = double.Parse(txtTotalKg.Value.ToString()) / TotCabezasAverage;

            for (int i = 0; i < lstInspeccion.Count; i++)
            {
                totKilos = ((float)(SetHeads(i, (int)lstInspeccion[i].Heads)) / TotCabezasAverage) * float.Parse(txtTotalKg.Value.ToString());
                lstInspeccion[i].TotalKg = (i == (lstInspeccion.Count - 1) ? Convert.ToInt64(txtTotalKg.Value.ToString()) - SumTotKilos : totKilos);
                lstInspeccion[i].Average = average;
                SumTotKilos += totKilos;
            }

        }

        private long SetHeads(int lIntLine, int lIntTotalHeaadsLine)
        {

            lstInspeccion[lIntLine].NP = 0;
            lstInspeccion[lIntLine].RE = 0;

            if (lIntHeadRjCounter > 0)
            {
                lIntTotalHeaadsLine -= lIntHeadRjCounter;

                if (lIntTotalHeaadsLine > 0)
                {
                    lstInspeccion[lIntLine].RE = lIntHeadRjCounter;
                    lIntHeadRjCounter = 0;
                }
                else
                {
                    lstInspeccion[lIntLine].RE = lIntHeadRjCounter - Math.Abs(lIntTotalHeaadsLine);
                    lIntHeadRjCounter = Math.Abs(lIntTotalHeaadsLine);
                }

            }
            if (lIntTotalHeaadsLine > 0)
            {
                lIntTotalHeaadsLine -= lIntHeadNPCounter;

                if (lIntTotalHeaadsLine > 0)
                {
                    lstInspeccion[lIntLine].NP = lIntHeadNPCounter;
                    lIntHeadNPCounter = 0;
                }
                else
                {
                    lstInspeccion[lIntLine].NP = lIntHeadNPCounter - Math.Abs(lIntTotalHeaadsLine);
                    lIntHeadNPCounter = Math.Abs(lIntTotalHeaadsLine);
                }
            }


            if (lIntTotalHeaadsLine < 0)
            {
                lIntTotalHeaadsLine = 0;
            }


            return Convert.ToInt64(lIntTotalHeaadsLine);
        }

        /// <summary>
        /// Method ConfigInspectionEsp
        /// 
        /// </summary>
        /// <remarks>@Author Rcordova </remarks>
        public void ConfigInspectionEsp()
        {
            lObjModalForm.Items.Item("txtTotkg").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
            lObjMatrixCert.Item.Enabled = false;


            txtNP.Item.Enabled = false;
            txtRE.Item.Enabled = false;

            txtTotalKg.Value = "0.00";
            txtRE.Value = "0";
            txtNP.Value = "0";

            for (int i = 1; i <= lObjMatrixCert.RowCount; i++)
            {
                lObjMatrixCert.ClearRowData(i);
            }
            lObjMatrixInsp.SetCellFocus(1, 0);

            lObjMatrixInsp.SelectRow(1, true, false);

            txtTotalKg.Item.Enabled = false;
            btnAceptar.Item.Enabled = true;
        }

        public void ConfigSpetialInspAct()
        {
            lObjModalForm.Items.Item("txtTotkg").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
            lObjMatrixCert.Item.Enabled = true;
            txtNP.Item.Enabled = true;
            txtRE.Item.Enabled = true;
            txtTotalKg.Item.Enabled = true;
            txtTotalKg.Item.Click();

        }

        internal void SetFocusTXT()
        {
            txtTotalKg.Active = true;
        }

        private bool CheckNP()
        {
            bool lboolValid = false;
            lIntNPforSpecialInsp = 0;
            if (txtNP.Value == string.Empty)
            {
                txtNP.Value = "0";
            }
            foreach (var item in lstInspeccion)
            {
                lIntNPforSpecialInsp += (int)item.Heads;
            }

            if (Convert.ToInt32(txtNP.Value) == lIntNPforSpecialInsp)
            {
                if (ValidQuantityNPRJ())
                {
                    lboolValid = true;
                    lBoolNp = true;
                }
            }
            else
            {
                lboolValid = false;
            }

            return lboolValid;
        }

        internal bool ValidInspection()
        {
            bool lBoolValid = false;


            CheckValues();
            OnlyNumbers(txtTotalKg, "Y");
            OnlyNumbers(txtRE, "N");
            OnlyNumbers(txtNP, "N");
            if (ValidateFields())
            {
                lBoolValid = true;
            }
            else
            {
                lBoolValid = false;
            }

            return lBoolValid;
        }

        private void CheckValues()
        {
            if (txtTotalKg.Value == "")
            {
                txtTotalKg.Value = "0.00";
            }
            if (txtNP.Value == "")
            {
                txtNP.Value = "0";
            }
            if (txtRE.Value == "")
            {
                txtRE.Value = "0";
            }

            txtTime.Value = inspectionDAO.GetServerTime();

        }


    }
}
