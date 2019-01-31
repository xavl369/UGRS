using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using UGRS.AddOn.PurchaseInvoice.Models;
using System.Runtime.InteropServices;
using System.IO;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.PurchaseInvoice.Utils;
using UGRS.Core.SDK.UI;

namespace UGRS.AddOn.PurchaseInvoice
{
    public class PurchaseInvoices
    {

        //    public SAPbobsCOM.Company lObjCompany { get; set; }
        //    public SAPbouiCOM.Company lObjCompanyUI { get; set; }

        private SAPbouiCOM.Form lObjForm = null;
        private SAPbouiCOM.EditText ltxtProveedor = null;
        private SAPbouiCOM.EditText lObjTxtDocDate = null;
        private SAPbouiCOM.ComboBox lObjComboBoxCurrency = null;
        private SAPbouiCOM.ComboBox lObjComboBoxCurrSource = null;
        private SAPbouiCOM.ComboBox lObjComboBoxDocType = null;
        private ProgressBarManager mObjProgressBar = null;
        private SAPbouiCOM.Form form;

        private string lStrSelectedPath;
        private string lStrFolio;
        private string lStrFolioFiscal;
        private string mStrRfc;
        bool lBolIsSuccessValidate = true;
        bool mBolImportXML = false;
        static List<string> mLstStrError = null;
        private string mStrFecha = string.Empty;
        string mStrFechaDoc = string.Empty;
        string lStrErrorMessage = string.Empty;

        List<Concepts> xmlConceptLines = new List<Concepts>();


        public SAPbouiCOM.EditText ltxtAccountNumber { get; set; }
        public SAPbouiCOM.EditText ltxtDescription { get; set; }
        public SAPbouiCOM.EditText ltxtPrice { get; set; }
        public SAPbouiCOM.EditText ltxtDetalle { get; set; }
        private SAPbouiCOM.EditText lObjEDocNum { get; set; }
        private SAPbouiCOM.Folder lObjFolder { get; set; }


        //Matrix
        private SAPbouiCOM.Matrix lObjMatrix;


        mPurchaseInvoice oPurchaseInvoice;

        private ModalFormXml lObjModalForm = null;

        public PurchaseInvoices()
        {
            if (DIApplication.Company.Connected)
            {
                Application.SBO_Application.StatusBar.SetText("Add-on Conectado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                Application.SBO_Application.StatusBar.SetText("Add-on disponible.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                //Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(FormDataEvent);
            }

        }


        //protected void FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo busObj, out bool bubbleEvent)
        //{

        //    bubbleEvent = true;

        //    if ((busObj.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD && busObj.BeforeAction))
        //    {
        //        lObjForm = Application.SBO_Application.Forms.Item(busObj.FormUID);
        //        if (oPurchaseInvoice != null)
        //        {
        //            if (busObj.FormTypeEx == "141" || busObj.FormTypeEx == "65301" || busObj.FormTypeEx == "60092" || busObj.FormTypeEx == "181")
        //            {
        //                string lStrImpustosRetencion = (lObjForm.Items.Item("166").Specific as SAPbouiCOM.EditText).Value;
        //                try
        //                {
        //                    bool lBolIsSuccess = ValidarUUID(oPurchaseInvoice.ImpuestosTraslados, oPurchaseInvoice.Total, oPurchaseInvoice.Retenciones, mStrRfc, lStrFolioFiscal);
        //                    if (!lBolIsSuccess)
        //                        bubbleEvent = false;
        //                    else
        //                        oPurchaseInvoice = null;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        //                    bubbleEvent = false;
        //                }
        //            }
        //        }
        //    }
        //    if(!busObj.BeforeAction)
        //    {

        //    }

        //}



        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           // SAPbouiCOM.Item lItmBtnReadXML = null;
            SAPbouiCOM.Item lItmBtnImportar = null;
            try
            {
                #region Button
                if ((pVal.FormType == 141 || pVal.FormType == 65301 || pVal.FormType == 60092 || pVal.FormType == 181) && pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD && pVal.Before_Action)
                {

                    lObjForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);



                    if (pVal.ItemUID.Equals("1") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                    {
                        if (oPurchaseInvoice != null)
                        {
                            string lStrImpustosRetencion = (lObjForm.Items.Item("166").Specific as SAPbouiCOM.EditText).Value;


                            bool lBolIsSuccess = ValidarUUID(oPurchaseInvoice.ImpuestosTraslados, oPurchaseInvoice.Total, oPurchaseInvoice.Retenciones, mStrRfc, lStrFolioFiscal);
                            if (!lBolIsSuccess)
                            {
                                Application.SBO_Application.StatusBar.SetText(lStrErrorMessage, SAPbouiCOM.BoMessageTime.bmt_Medium,
                                    SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                BubbleEvent = false;
                         
                            }
                            //else
                            //{
                            //    if (lObjTxtDocDate.Value != string.Empty)
                            //    {
                            //        oPurchaseInvoice = null;
                            //    }
              
                            //}
                        }
                    }

                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && pVal.Before_Action)
                    {


                        //lItmBtnReadXML = lObjForm.Items.Add("btnReadXML", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                        //lItmBtnReadXML.Top = lObjForm.Items.Item("70").Top + 20;
                        //lItmBtnReadXML.Left = lObjForm.Items.Item("70").Left;
                        //(lItmBtnReadXML.Specific as SAPbouiCOM.Button).Caption = "Importar XML";

                        if (!ItemExist("btnImpo", lObjForm))
                        {
                            SAPbouiCOM.Item txtUUID = lObjForm.Items.Item("480002119");
                            lItmBtnImportar = lObjForm.Items.Add("btnImpo", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                            lItmBtnImportar.FromPane = 10;
                            lItmBtnImportar.ToPane = 10;
                            lItmBtnImportar.Top = txtUUID.Top + 15;
                            lItmBtnImportar.Left = txtUUID.Left;

                            (lItmBtnImportar.Specific as SAPbouiCOM.Button).Caption = "Importar";
                        }
                #endregion
                    }

                    #region OpenFileDialog & LoadXML
                    else if (pVal.ItemUID == "btnReadXML" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_CLICK && pVal.Before_Action)
                    {
                        initItems();

                        if (lObjMatrix.RowCount > 1)
                        {
                            try
                            {
                                initProgressBar(lObjMatrix.RowCount);

                                for (int i = lObjMatrix.RowCount; i >= 1; i--)
                                {
                                    if (lObjMatrix.RowCount == 1)
                                        lObjMatrix.ClearRowData(i);
                                    else
                                        lObjMatrix.DeleteRow(i);
                                }
                            }
                            catch (Exception err)
                            {
                                Application.SBO_Application.StatusBar.SetText(err.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                            }
                            finally
                            {
                                mObjProgressBar.Dispose();
                            }
                        }
                        if (lObjComboBoxDocType.Selected.Value == "S" && lObjMatrix.RowCount == 1 && OpenFileDialog())
                        {
                            if (validUUID(lStrFolioFiscal))
                            {
                                if (validatePurchase())
                                {
                                    if (getPurchaseByRFC())
                                    {
                                        mStrRfc = oPurchaseInvoice.RFCProveedor;
                                        lObjModalForm = new ModalFormXml(lObjForm, oPurchaseInvoice, "Servicios", pVal.FormType.ToString()); //aqui estaba breakpoint
                                    }
                                    else
                                        Application.SBO_Application.StatusBar.SetText("RFC emisor no registrado.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
                                else
                                    Application.SBO_Application.StatusBar.SetText("RFC receptor incorrecto.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                            else
                                Application.SBO_Application.StatusBar.SetText("Folio Fiscal ya registrado", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                        else if (lObjComboBoxDocType.Selected.Value == "I" && lObjMatrix.RowCount == 1 && OpenFileDialog())
                        {
                            if (validUUID(lStrFolioFiscal))
                            {
                                if (validatePurchase())
                                {
                                    if (getPurchaseByRFC())
                                        lObjModalForm = new ModalFormXml(lObjForm, oPurchaseInvoice, "Articulos", pVal.FormType.ToString());
                                    else
                                        Application.SBO_Application.StatusBar.SetText("RFC emisor no registrado", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
                                else
                                    Application.SBO_Application.StatusBar.SetText("RFC receptor incorrecto.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                            else
                                Application.SBO_Application.StatusBar.SetText("Folio Fiscal ya registrado", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                    else if (pVal.EventType.Equals(SAPbouiCOM.BoEventTypes.et_CLICK) && pVal.ItemUID.Equals("btnImpo") && pVal.BeforeAction)
                    {
                        importXML();
                    }
                }


                    #endregion
                if (!pVal.BeforeAction)
                {
                    if (pVal.ItemUID.Equals("1"))
                    {
                        BubbleEvent = false;
                    }
                }


                #region AcceptBtn FormXml
                else if ((pVal.FormUID == "FormXmlService" || pVal.FormUID == "FormXmlArticle") && pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD && pVal.Before_Action)
                {
                    #region Services
                    if (pVal.FormUID == "FormXmlService" && pVal.EventType.Equals(SAPbouiCOM.BoEventTypes.et_CLICK) && pVal.ItemUID.Equals("Item_13") && pVal.BeforeAction)
                    {
                        if (lObjModalForm.ValidateCompleteLines())
                        {
                            if (lObjModalForm.ValidateCorrectCurrency())
                            {
                                if (lObjModalForm.Lines())
                                {
                                    if (lObjModalForm.pObjChooseFromList != null)
                                    {
                                        Marshal.ReleaseComObject(lObjModalForm.pObjChooseFromList);
                                        Marshal.FinalReleaseComObject(lObjModalForm.pObjChooseFromList);
                                    }
                                    oPurchaseInvoice = lObjModalForm.FillObject();
                                    SetCurrency();
                                    FillMatrix();
                                    Application.SBO_Application.Forms.Item(pVal.FormUID).Close();
                                    SetUUID();

                                }
                                else
                                {
                                    Application.SBO_Application.StatusBar.SetText("Algunas cuentas no coinciden con la moneda del documento, favor de verificarlas", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                }
                            }
                            else
                            {
                                Application.SBO_Application.StatusBar.SetText("Verifique la moneda del documento", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            }
                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Faltan cuentas por agregar", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        }
                    }
                    #endregion
                    #region Articles
                    else if (pVal.FormUID == "FormXmlArticle" && pVal.EventType.Equals(SAPbouiCOM.BoEventTypes.et_CLICK) && pVal.ItemUID.Equals("Item_13") && pVal.BeforeAction)
                    {
                        if (lObjModalForm.ValidateCompleteLines())
                        {
                            if (lObjModalForm.ValidateCorrectCurrency())
                            {

                                if (lObjModalForm.pObjChooseFromList != null)
                                {
                                    Marshal.ReleaseComObject(lObjModalForm.pObjChooseFromList);
                                    Marshal.FinalReleaseComObject(lObjModalForm.pObjChooseFromList);
                                }
                                oPurchaseInvoice = lObjModalForm.FillObject();
                                SetCurrency();
                                FillMatrixArticles();
                                Application.SBO_Application.Forms.Item(pVal.FormUID).Close();
                                SetUUID();

                            }
                            else
                            {
                                Application.SBO_Application.StatusBar.SetText("Verifique la moneda del documento", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            }
                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Faltan articulos por agregar", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                        }
                    }
                    #endregion

                    #region CloseForm
                    else if ((pVal.FormUID == "FormXmlService" || pVal.FormUID == "FormXmlArticle") && pVal.EventType.Equals(SAPbouiCOM.BoEventTypes.et_CLICK) && pVal.ItemUID.Equals("Item_14") && pVal.BeforeAction)
                    {
                        if (lObjModalForm.pListxmlConceptLines2 != null)
                        {
                            lObjModalForm.StopObjects();
                            lObjModalForm.UnloadFormEvents();
                        }
                        Application.SBO_Application.Forms.Item(pVal.FormUID).Close();
                    }
                    else if ((pVal.FormUID == "FormXmlService" || pVal.FormUID == "FormXmlArticle") && pVal.EventType.Equals(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE) && pVal.Before_Action)
                    {
                        if (lObjModalForm != null && lObjModalForm.pListxmlConceptLines2 != null && lObjModalForm.pListxmlConceptLines2.Count > 0)
                        {
                            lObjModalForm.StopObjects();
                            lObjModalForm.UnloadFormEvents();
                        }
                    }
                    #endregion

                }
                #endregion
            }
            catch (Exception er)
            {
                LogUtility.WriteError("ItemEvent: " + er.Message);
                LogUtility.WriteException(er);
                try
                {

                    UIApplication.ShowError(er.Message);
                    //Application.SBO_Application.StatusBar.SetText(er.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    lObjModalForm.StopObjects();
                    lObjModalForm.UnloadFormEvents();
                }
                catch (Exception)
                {
                    //Ignore

                }
            }
        }

        private void initItems()
        {
            lObjFolder = (SAPbouiCOM.Folder)lObjForm.Items.Item("350002087").Specific;
            lObjEDocNum = (SAPbouiCOM.EditText)lObjForm.Items.Item("480002119").Specific;
            lObjComboBoxDocType = (SAPbouiCOM.ComboBox)lObjForm.Items.Item("3").Specific;
            ltxtProveedor = (SAPbouiCOM.EditText)lObjForm.Items.Item("4").Specific;
            lObjTxtDocDate = (SAPbouiCOM.EditText)lObjForm.Items.Item("10").Specific;
            lObjComboBoxCurrency = (SAPbouiCOM.ComboBox)lObjForm.Items.Item("63").Specific;
            lObjComboBoxCurrSource = (SAPbouiCOM.ComboBox)lObjForm.Items.Item("70").Specific;
            if (lObjComboBoxDocType.Value == "S")
            {
                lObjMatrix = (SAPbouiCOM.Matrix)lObjForm.Items.Item("39").Specific;
            }
            else
            {
                lObjMatrix = (SAPbouiCOM.Matrix)lObjForm.Items.Item("38").Specific;
            }
        }

        private void initProgressBar(int lIntSize)
        {
            mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "Cargando lineas del Xml", lIntSize);
        }

        private void FillMatrix()
        {
            lObjForm.Freeze(true);
            int lIntCount = 1;
            try
            {
                initProgressBar(oPurchaseInvoice.ConceptLines.Count);

                foreach (var item in oPurchaseInvoice.ConceptLines)
                {

                    ltxtDescription = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("1").Cells.Item(lIntCount).Specific;
                    ltxtDescription.Value = item.Descripcion.ToString();


                    ltxtAccountNumber = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("2").Cells.Item(lIntCount).Specific;
                    ltxtAccountNumber.Value = item.NumeroCuenta.ToString();

                    //Validar si es peso mexicano(columna 12), si no se utiliza la columna 14
                    if (lObjComboBoxCurrency.Value == "MXP")
                    {
                        ltxtPrice = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("12").Cells.Item(lIntCount).Specific;
                        ltxtPrice.Value = item.ValorUnitario;
                    }
                    else
                    {
                        ltxtPrice = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("14").Cells.Item(lIntCount).Specific;
                        ltxtPrice.Value = item.ValorUnitario;
                    }

                    ltxtDetalle = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("256").Cells.Item(lIntCount).Specific;
                    ltxtDetalle.Value = item.Descripcion.ToString();


                    lIntCount++;
                    mObjProgressBar.NextPosition();
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("FillMatrix: " + ex.Message);
                LogUtility.WriteException(ex);
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                lObjForm.Freeze(false);
                mObjProgressBar.Dispose();

            }
        }

        private void FillMatrixArticles()
        {
            lObjForm.Freeze(true);
            int lIntCount = 1;

            try
            {
                initProgressBar(oPurchaseInvoice.ConceptLines.Count);
                foreach (var item in oPurchaseInvoice.ConceptLines)
                {
                    ltxtDescription = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("1").Cells.Item(lIntCount).Specific;
                    ltxtDescription.Value = item.Articulo.ToString();

                    ltxtAccountNumber = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("11").Cells.Item(lIntCount).Specific;
                    ltxtAccountNumber.Value = item.Cantidad;

                    ltxtPrice = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("14").Cells.Item(lIntCount).Specific;
                    ltxtPrice.Value = item.ValorUnitario;

                    ltxtDetalle = (SAPbouiCOM.EditText)lObjMatrix.Columns.Item("256").Cells.Item(lIntCount).Specific;
                    ltxtDetalle.Value = item.Descripcion.ToString();

                    lIntCount++;
                    mObjProgressBar.NextPosition();
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                lObjForm.Freeze(false);
                mObjProgressBar.Dispose();
            }
        }

        private void SetCurrency()
        {


            switch (lObjModalForm.getCurrencyByBP())
            {
                case "##":
                    lObjComboBoxCurrency.Select(2, SAPbouiCOM.BoSearchKey.psk_Index);
                    lObjComboBoxCurrency.Select(oPurchaseInvoice.MonedaDocumento.Trim(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    break;
                case "MXP":
                    lObjComboBoxCurrSource.Select(2, SAPbouiCOM.BoSearchKey.psk_Index);
                    break;
                case "USD":
                    lObjComboBoxCurrSource.Select(2, SAPbouiCOM.BoSearchKey.psk_Index);
                    break;
                case "EUR":
                    lObjComboBoxCurrSource.Select(2, SAPbouiCOM.BoSearchKey.psk_Index);
                    break;

            }
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private bool OpenFileDialog()
        {
            bool lBValid = false;

            var lVarThread = new Thread((ThreadStart)(() =>
            {

                using (System.Windows.Forms.OpenFileDialog ofdImport = new System.Windows.Forms.OpenFileDialog())
                {
                    ofdImport.CheckFileExists = true;
                    ofdImport.DefaultExt = "xml";
                    ofdImport.Multiselect = false;
                    IntPtr sapProc = GetForegroundWindow();
                    WindowWrapper MyWindow = null;
                    MyWindow = new WindowWrapper(sapProc);

                    if (ofdImport.ShowDialog(MyWindow) == System.Windows.Forms.DialogResult.OK)
                    {
                        lStrSelectedPath = ofdImport.FileName;
                        if (Path.GetExtension(lStrSelectedPath) == ".xml")
                        {
                            lBValid = LoadXmlFile();
                        }
                        else
                        {
                            Application.SBO_Application.MessageBox("Formato incorrecto, cargue el archivo adecuado(xml)");
                            lBValid = false;
                        }
                    }
                    else
                    {
                        lBValid = false;
                    }

                    ofdImport.Dispose();
                }
            }));
            lVarThread.SetApartmentState(ApartmentState.STA);
            lVarThread.Start();
            lVarThread.Join();

            if (lVarThread.IsAlive)
            {
                lVarThread.Abort();
            }
            return lBValid;
        }

        private string GetXMLAttributeValue(XmlNode pObjXmlNode, string pStrAttributo)
        {
            if (pObjXmlNode.Attributes[pStrAttributo] != null)
                return pObjXmlNode.Attributes[pStrAttributo].Value;
            else
                return "";
        }

        private bool LoadXmlFile()
        {
            bool lBolSuccessXML = false;

            try
            {


                XmlDocument lObjXmlDoc = new XmlDocument();
                lObjXmlDoc.Load(lStrSelectedPath);

                oPurchaseInvoice = new mPurchaseInvoice();
                var tfdNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                var comNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");
                string lStrVersion = GetXMLAttributeValue(lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Version");

                XmlSchemaSetValidador lObjValidador = new XmlSchemaSetValidador();



                if (lStrVersion == "")
                    lBolSuccessXML = lObjValidador.CompruebaXMLvsXSD("http://www.sat.gob.mx/cfd/3", "cfdv32.xsd", lStrSelectedPath);
                //ValidateXml(lObjXmlDoc, "cfdv32.xsd");
                else
                    lBolSuccessXML = lObjValidador.CompruebaXMLvsXSD("http://www.sat.gob.mx/cfd/3", "cfdv33.xsd", lStrSelectedPath);
                //ValidateXml(lObjXmlDoc, "cfdv33.xsd");
                if (lBolSuccessXML)
                {
                    bool lBolcheckSAT = false;
                    if (lStrVersion == "")
                        lBolcheckSAT = LoadObjectMXL32(lObjXmlDoc);
                    else
                        lBolcheckSAT = LoadObjectXML33(lObjXmlDoc, lStrVersion);

                    if (lBolcheckSAT)
                        oPurchaseInvoice.ConceptLines = xmlConceptLines;
                    else
                        Application.SBO_Application.StatusBar.SetText("El XML tiene algun error o no existe en la base de datos del SAT", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("El Xml no coincide con las reglas del XSD, tiene " + lObjValidador.TotalErrores() + " errores", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("LoadXmlFile: " + ex.Message);
                LogUtility.WriteException(ex);
            }

            return lBolSuccessXML;
        }

        private bool LoadObjectMXL32(XmlDocument pObjDoc)
        {

            var tfdNsM = new XmlNamespaceManager(pObjDoc.NameTable);
            tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
            var comNsM = new XmlNamespaceManager(pObjDoc.NameTable);
            comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");

            int lIntTest = GetTestValue();

            if (lIntTest == 0)
            {
                Application.SBO_Application.StatusBar.SetText("Se revisa el XML ante el SAT", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                if (!ReceiptService.CheckXML32(pObjDoc.InnerXml))
                    return false;
            }

            lStrFolioFiscal = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM), "UUID");
            oPurchaseInvoice.Version = "3.2";
            lStrFolio = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "folio");
            oPurchaseInvoice.SubTotal = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "subTotal");
            oPurchaseInvoice.Total = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "total");
            mStrFecha = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "fecha");
            oPurchaseInvoice.Impuestos = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Impuestos", comNsM), "totalImpuestosTrasladados");
            oPurchaseInvoice.RFCProveedor = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Emisor", comNsM), "rfc");
            oPurchaseInvoice.RFCReceptor = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Receptor", comNsM), "rfc");
            oPurchaseInvoice.Nombre = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Emisor", comNsM), "nombre");
            oPurchaseInvoice.MonedaXml = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Moneda");
            XmlNode lStrConcepto = pObjDoc.SelectSingleNode("//cfdi:Conceptos", comNsM);
            xmlConceptLines = new List<Concepts>();
            foreach (XmlElement lXmlChild in lStrConcepto.ChildNodes)
            {
                if (lXmlChild.HasAttributes)
                {
                    Concepts conceptos = new Concepts();
                    foreach (XmlAttribute lXmlAtribute in lXmlChild.Attributes)
                    {
                        switch (lXmlAtribute.Name)
                        {
                            case "descripcion":
                                conceptos.Descripcion = lXmlAtribute.Value;
                                break;
                            case "valorUnitario":
                                conceptos.ValorUnitario = lXmlAtribute.Value;
                                break;
                            case "importe":
                                conceptos.Importe = lXmlAtribute.Value;
                                break;
                            case "unidad":
                                conceptos.Unidad = lXmlAtribute.Value;
                                break;
                            case "cantidad":
                                conceptos.Cantidad = lXmlAtribute.Value;
                                break;
                        }
                    }
                    xmlConceptLines.Add(conceptos);
                }
            }

            XmlNode lObjXmlNode = pObjDoc.SelectSingleNode("//cfdi:Impuestos", comNsM);

            if (lObjXmlNode.ChildNodes.Count > 1)
            {
                XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                oPurchaseInvoice.ImpuestosTraslados = GetImpuestosTraslados(lObjXmlNodeTraslados, "3.2").ToString();
                XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                oPurchaseInvoice.Retenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, "3.2").ToString();
            }
            else
            {
                if (lObjXmlNode.ChildNodes.Count == 1)
                {
                    switch (lObjXmlNode.ChildNodes[0].Name)
                    {
                        case "cfdi:Traslados":
                            XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                            oPurchaseInvoice.ImpuestosTraslados = GetImpuestosTraslados(lObjXmlNodeTraslados, "3.2").ToString();
                            break;
                        case "cfdi:Retenciones":
                            XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                            oPurchaseInvoice.Retenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, "3.2").ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

        private bool LoadObjectXML33(XmlDocument pObjDoc, string pStrVersion)
        {
            var tfdNsM = new XmlNamespaceManager(pObjDoc.NameTable);
            tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
            var comNsM = new XmlNamespaceManager(pObjDoc.NameTable);
            comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");

            int lIntTest = GetTestValue();

            if (lIntTest == 0)
            {
                if (!ReceiptService.CheckXML32(pObjDoc.InnerXml))
                    return false;
            }

            lStrFolioFiscal = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM), "UUID");
            oPurchaseInvoice.Version = "3.3";
            lStrFolio = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Folio");
            oPurchaseInvoice.SubTotal = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "SubTotal");
            oPurchaseInvoice.Total = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Total");
            oPurchaseInvoice.Impuestos = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Impuestos", comNsM), "TotalImpuestosTrasladados");
            oPurchaseInvoice.RFCProveedor = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Emisor", comNsM), "Rfc");
            oPurchaseInvoice.RFCReceptor = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Receptor", comNsM), "Rfc");
            oPurchaseInvoice.Nombre = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Emisor", comNsM), "Nombre");//Receptor
            oPurchaseInvoice.MonedaXml = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Moneda");
            mStrFecha = GetXMLAttributeValue(pObjDoc.SelectSingleNode("//cfdi:Comprobante", comNsM), "Fecha");


            XmlNode lStrConcepto = pObjDoc.SelectSingleNode("//cfdi:Conceptos", comNsM);

            xmlConceptLines = new List<Concepts>();
            foreach (XmlElement lXmlChild in lStrConcepto.ChildNodes)
            {
                if (lXmlChild.HasAttributes)
                {
                    Concepts conceptos = new Concepts();

                    foreach (XmlAttribute lXmlAtribute in lXmlChild.Attributes)
                    {
                        switch (lXmlAtribute.Name)
                        {
                            case "Descripcion":
                                conceptos.Descripcion = lXmlAtribute.Value;
                                break;
                            case "ValorUnitario":
                                conceptos.ValorUnitario = lXmlAtribute.Value;
                                break;
                            case "Importe":
                                conceptos.Importe = lXmlAtribute.Value;
                                break;
                            case "ClaveUnidad":
                                conceptos.Unidad = lXmlAtribute.Value;
                                break;
                            case "Cantidad":
                                conceptos.Cantidad = lXmlAtribute.Value;
                                break;
                            case "ClaveProdServ":
                                conceptos.ClaveItmProd = lXmlAtribute.Value;
                                break;
                        }

                    }
                    xmlConceptLines.Add(conceptos);
                }
            }

            XmlNode lObjXmlNode = pObjDoc.SelectSingleNode("//cfdi:Impuestos", comNsM);

            if (lObjXmlNode.ChildNodes.Count > 1)
            {
                XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                oPurchaseInvoice.ImpuestosTraslados = GetImpuestosTraslados(lObjXmlNodeTraslados, pStrVersion).ToString();
                XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                oPurchaseInvoice.Retenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, pStrVersion).ToString();
            }
            else
            {
                if (lObjXmlNode.ChildNodes.Count == 1)
                {
                    switch (lObjXmlNode.ChildNodes[0].Name)
                    {
                        case "cfdi:Traslados":
                            XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                            oPurchaseInvoice.ImpuestosTraslados = GetImpuestosTraslados(lObjXmlNodeTraslados, pStrVersion).ToString();
                            break;
                        case "cfdi:Retenciones":
                            XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                            oPurchaseInvoice.Retenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, pStrVersion).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }

        private int GetTestValue()
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            int lIntResult = 0;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery("SELECT U_Value FROM [@UG_CONFIG] where Name= 'CO_TEST'"); //SELECT CO_DIFFACT FROM [@UG_CONFIG]
                if (lObjRecordSet.RecordCount == 1)
                    lIntResult = int.Parse(lObjRecordSet.Fields.Item(0).Value.ToString());

                return lIntResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }
        }

        private bool validatePurchase()
        {
            bool validar = false;
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                lObjRecordSet.DoQuery("SELECT * FROM OADM WHERE TaxIdNum='" + oPurchaseInvoice.RFCReceptor + "'");

                if (lObjRecordSet.RecordCount != 0)
                {
                    validar = true;
                }
                else
                {
                    validar = false;
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("validatePurchase: " + ex.Message);
                LogUtility.WriteException(ex);
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }

            return validar;
        }

        private bool getPurchaseByRFC()
        {
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            bool value = false;

            try
            {
                mStrRfc = oPurchaseInvoice.RFCProveedor;
                lObjRecordSet.DoQuery("SELECT CardCode, CardName FROM OCRD WHERE LicTradNum='" + oPurchaseInvoice.RFCProveedor + "' AND CardType='S' AND validFor='Y' AND frozenFor='N'  ");

                if (lObjRecordSet.RecordCount != 0)
                {
                    oPurchaseInvoice.CardCode = lObjRecordSet.Fields.Item(0).Value.ToString();
                    oPurchaseInvoice.NombreSocioNegocio = lObjRecordSet.Fields.Item(1).Value.ToString();
                    ltxtProveedor.Value = oPurchaseInvoice.CardCode;

                    value = true;
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("El socio de negocio no existe, favor de darlo de alta", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    value = false;
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("getPurchaseByRFC: " + ex.Message);
                LogUtility.WriteException(ex);
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }

            return value;
        }

        private bool ItemExist(string pStrItemName, SAPbouiCOM.Form pObjForm)
        {
            try
            {
                pObjForm.Items.Item(pStrItemName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetUUID()
        {
            lObjFolder.Select();

            lObjEDocNum.Active = true;
            lObjEDocNum.Value = lStrFolioFiscal;
            lObjEDocNum.Active = false;

            DateTime lObjDt = DateTime.Now;
            DateTime.TryParse(mStrFecha, out lObjDt);
            SAPbouiCOM.EditText txtDate = (SAPbouiCOM.EditText)lObjForm.Items.Item("46");
            if (txtDate.Item.Enabled)
            {
                ((SAPbouiCOM.EditText)lObjForm.Items.Item("46").Specific).Active = true;
                ((SAPbouiCOM.EditText)lObjForm.Items.Item("46").Specific).Value = lObjDt.ToString("yyyyMMdd");
                ((SAPbouiCOM.EditText)lObjForm.Items.Item("46").Specific).Active = false;
            }
             
            lObjFolder = (SAPbouiCOM.Folder)lObjForm.Items.Item("112").Specific;
            
            lObjFolder.Select();
        }

        private void ReadXMLInvoice(string pStrUrl)
        {
            try
            {
                string fileName = pStrUrl;
                XmlDocument lObjXmlDoc = new XmlDocument();
                lObjXmlDoc.Load(fileName);
                var tfdNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                var comNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");
                string lStrFolioFiscal = lObjXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["UUID"].Value;
                string lStrVersion = string.Empty;
                if (lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Version"] == null)
                {
                    lStrVersion = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["version"].Value;
                }
                else
                {
                    lStrVersion = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Version"].Value;
                }

                string lStrTotal = string.Empty;
                if (lStrVersion == "3.3")
                    lStrTotal = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Total"].Value;
                else
                    lStrTotal = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["total"].Value;
                string lStrImpuestos = "0";
                string lStrImpuestosRetenciones = "0";
                XmlNode lObjXmlNode = lObjXmlDoc.SelectSingleNode("//cfdi:Impuestos", comNsM);

                if (lObjXmlNode.ChildNodes.Count > 1)
                {
                    XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                    lStrImpuestos = GetImpuestosTraslados(lObjXmlNodeTraslados, lStrVersion).ToString();
                    XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                    lStrImpuestosRetenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, lStrVersion).ToString();
                }
                else
                {
                    if (lObjXmlNode.ChildNodes.Count == 1)
                    {
                        switch (lObjXmlNode.ChildNodes[0].Name)
                        {
                            case "cfdi:Traslados":
                                XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                                lStrImpuestos = GetImpuestosTraslados(lObjXmlNodeTraslados, lStrVersion).ToString();
                                break;
                            case "cfdi:Retenciones":
                                XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                                lStrImpuestosRetenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, lStrVersion).ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
                string lStrRFC = string.Empty;
                if (lStrVersion == "3.3")
                    lStrRFC = lObjXmlDoc.SelectSingleNode("//cfdi:Emisor", comNsM).Attributes["Rfc"].Value;
                else
                    lStrRFC = lObjXmlDoc.SelectSingleNode("//cfdi:Emisor", comNsM).Attributes["rfc"].Value;

                //ValidarXMLFacturaUUID(lStrImpuestos, lStrTotal, lStrImpuestosRetenciones, lStrRFC, lStrFolioFiscal);
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("ReadXMLInvoice: " + ex.Message);
                LogUtility.WriteException(ex);
                throw new Exception("Error interno no identificado. Error: " + ex.ToString());
            }
        }


        private float GetImpuestosTraslados(XmlNode pObjXmlNode, string pStrVersion)
        {
            float lFltImpuestos = 0;
            foreach (XmlElement lObjxmlElement in pObjXmlNode.ChildNodes)
            {
                if (pStrVersion == "3.3")
                {
                    if (lObjxmlElement.Attributes["Impuesto"].Value.ToString() == "002")
                        lFltImpuestos += float.Parse(lObjxmlElement.Attributes["Importe"].Value.ToString());
                }
                else
                {
                    if (lObjxmlElement.Attributes["impuesto"].Value.ToString() == "IVA")
                        lFltImpuestos += float.Parse(lObjxmlElement.Attributes["importe"].Value.ToString());
                }


            }
            return lFltImpuestos;
        }

        private float GetImpuestosRetenciones(XmlNode pObjXmlNode, string pStrVersion)
        {
            float lFltImpuestosRetenciones = 0;
            foreach (XmlElement lObjxmlElement in pObjXmlNode.ChildNodes)
            {
                //if (lObjxmlElement.Attributes["impuesto"].Value.ToString() == "IVA")
                if (pStrVersion == "3.3")
                    lFltImpuestosRetenciones += float.Parse(lObjxmlElement.Attributes["Importe"].Value.ToString());
                else
                    lFltImpuestosRetenciones += float.Parse(lObjxmlElement.Attributes["importe"].Value.ToString());
            }
            return lFltImpuestosRetenciones;
        }

        private void ValidarXMLFacturaUUID(string pStrImpuestos, string pStrTotal, string pStrImpuestosRetenciones, string pStrRFC, string pStrFolioFiscal)
        {
            double lDblDirefenciaImpuesto = 0;
            double lDblDirefenciaTotal = 0;
            double lDblDiferenciaReal = 0;
            pStrImpuestosRetenciones = pStrImpuestosRetenciones == "" ? "0" : pStrImpuestosRetenciones;
            string lStrTotalFactura = (lObjForm.Items.Item("29").Specific as SAPbouiCOM.EditText).Value;
            string lStrImpuestosFactura = (lObjForm.Items.Item("27").Specific as SAPbouiCOM.EditText).Value;
            string lStrImpustosRetencion = (lObjForm.Items.Item("166").Specific as SAPbouiCOM.EditText).Value;
            string lStrCardCode = (lObjForm.Items.Item("4").Specific as SAPbouiCOM.EditText).Value;
            double lDblTotalFactura = double.Parse(lStrTotalFactura.Substring(0, lStrTotalFactura.Length - 4));
            if (lStrImpuestosFactura == "") lStrImpuestosFactura = "0 MXP";
            double lDblImpuestosFactura = double.Parse(lStrImpuestosFactura.Substring(0, lStrImpuestosFactura.Length - 4));
            lDblDirefenciaImpuesto = lDblImpuestosFactura - double.Parse(pStrImpuestos);
            if (lStrImpustosRetencion != "")
                lDblDirefenciaTotal = lDblTotalFactura - (double.Parse(pStrTotal));
            else
                lDblDirefenciaTotal = lDblTotalFactura - (double.Parse(pStrTotal) + double.Parse(pStrImpuestosRetenciones));

            SAPbobsCOM.Recordset lObjRecordSet2 = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet2.DoQuery("SELECT U_Value FROM [@UG_CONFIG] where Name='CO_DIFFACT'"); //SELECT CO_DIFFACT FROM [@UG_CONFIG]
            if (lObjRecordSet2.RecordCount == 1)
                lDblDiferenciaReal = double.Parse(lObjRecordSet2.Fields.Item(0).Value.ToString());

            MemoryUtility.ReleaseComObject(lObjRecordSet2);
            SAPbobsCOM.BusinessPartners lObjBussinessPartners = (SAPbobsCOM.BusinessPartners)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            if (lObjBussinessPartners.GetByKey(lStrCardCode))
                lStrCardCode = lObjBussinessPartners.FederalTaxID;

            if (lStrCardCode == pStrRFC)
            {
                if (!(lDblDirefenciaImpuesto <= lDblDiferenciaReal && lDblDirefenciaImpuesto >= (lDblDiferenciaReal * (-1))))
                    Application.SBO_Application.MessageBox("La diferencia de impuestos del XML y la factura no corresponden a la configuración. Favor de comprobar la información");
                else
                {
                    if (!(lDblDirefenciaTotal <= lDblDiferenciaReal && lDblDirefenciaTotal >= (lDblDiferenciaReal * (-1))))
                        Application.SBO_Application.MessageBox("La diferencia de totales del XML y la factura no corresponden a la configuración. Favor de comprobar la información");
                    else
                    {
                        SAPbobsCOM.SBObob lObjSBObob = (SAPbobsCOM.SBObob)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                        SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices, "EDocNum", pStrFolioFiscal, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                        if (lObjRecordSet.RecordCount == 0)
                        {
                            lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oInvoices, "EDocNum", pStrFolioFiscal, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                            if (lObjRecordSet.RecordCount == 0)
                            {
                                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes, "EDocNum", pStrFolioFiscal, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                                if (lObjRecordSet.RecordCount == 0)
                                {
                                    lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oCreditNotes, "EDocNum", pStrFolioFiscal, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                                    if (lObjRecordSet.RecordCount == 0)
                                        CargarUUIDFactura(pStrFolioFiscal);
                                    else
                                        CargarUUIDFacturaDontExist(lObjRecordSet, pStrFolioFiscal, "NotaCreditoCliente");
                                }
                                else
                                    CargarUUIDFacturaDontExist(lObjRecordSet, pStrFolioFiscal, "NotaCreditoProveedor");
                            }
                            else
                                CargarUUIDFacturaDontExist(lObjRecordSet, pStrFolioFiscal, "FacturaCliente");
                        }
                        else
                            CargarUUIDFacturaDontExist(lObjRecordSet, pStrFolioFiscal, "FacturaProveedor");

                        MemoryUtility.ReleaseComObject(lObjRecordSet);
                        MemoryUtility.ReleaseComObject(lObjSBObob);
                    }
                }
            }
            else
                Application.SBO_Application.MessageBox("El RFC del Proveedor no coincide. Favor de comprobar la información");

            MemoryUtility.ReleaseComObject(lObjBussinessPartners);
        }



        private bool ValidarUUID(string pStrImpuestos, string pStrTotal, string pStrImpuestosRetenciones, string pStrRFC, string pStrFolioFiscal)
        {
            SAPbobsCOM.SBObob lObjSBObob = null;
            SAPbobsCOM.Recordset lObjRecordSet = null;
            SAPbobsCOM.BusinessPartners lObjBussinessPartners = null;
            bool lBolIsSuccess = false;
            try
            {
                double lDblDirefenciaImpuesto = 0;
                double lDblDirefenciaTotal = 0;
                double lDblDiferenciaReal = 0;
                pStrImpuestosRetenciones = (pStrImpuestosRetenciones == "" || pStrImpuestosRetenciones == null) ? "0" : pStrImpuestosRetenciones;
                string lStrTotalFactura = (lObjForm.Items.Item("29").Specific as SAPbouiCOM.EditText).Value;
                string lStrImpuestosFactura = (lObjForm.Items.Item("27").Specific as SAPbouiCOM.EditText).Value;
                string lStrImpustosRetencion = (lObjForm.Items.Item("166").Specific as SAPbouiCOM.EditText).Value;
                string lStrCardCode = (lObjForm.Items.Item("4").Specific as SAPbouiCOM.EditText).Value;
                double lDblTotalFactura = double.Parse(lStrTotalFactura.Substring(0, lStrTotalFactura.Length - 4));
                if (lStrImpuestosFactura == "") lStrImpuestosFactura = "0 MXP";
                double lDblImpuestosFactura = double.Parse(lStrImpuestosFactura.Substring(0, lStrImpuestosFactura.Length - 4));
                lDblDirefenciaImpuesto = lDblImpuestosFactura - double.Parse(pStrImpuestos);
                if (lStrImpustosRetencion != "")
                    lDblDirefenciaTotal = lDblTotalFactura - (double.Parse(pStrTotal));
                else
                    lDblDirefenciaTotal = lDblTotalFactura - (double.Parse(pStrTotal) + double.Parse(pStrImpuestosRetenciones));

                SAPbobsCOM.Recordset lObjRecordSet2 = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet2.DoQuery("SELECT U_Value FROM [@UG_CONFIG] where Name='CO_DIFFACT'"); //SELECT CO_DIFFACT FROM [@UG_CONFIG]
                if (lObjRecordSet2.RecordCount == 1)
                    lDblDiferenciaReal = double.Parse(lObjRecordSet2.Fields.Item(0).Value.ToString());

                lObjBussinessPartners = (SAPbobsCOM.BusinessPartners)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                if (lObjBussinessPartners.GetByKey(lStrCardCode))
                    lStrCardCode = lObjBussinessPartners.FederalTaxID;

                if (lStrCardCode == pStrRFC)
                {
                    if (!(lDblDirefenciaImpuesto <= lDblDiferenciaReal && lDblDirefenciaImpuesto >= (lDblDiferenciaReal * (-1))))
                        //throw new Exception("La diferencia de impuestos del XML y la factura es mayor o menor a 2. Favor de comprobar la información");
                        lStrErrorMessage = "La diferencia de impuestos del XML y la factura no corresponden a la configuración. Favor de comprobar la información";
                    else
                    {
                        if (!(lDblDirefenciaTotal <= 2 && lDblDiferenciaReal >= (lDblDiferenciaReal * (-1))))
                            //throw new Exception("La diferencia de totales del XML y la factura es mayor o menor a 2. Favor de comprobar la información");
                            lStrErrorMessage = "La diferencia de impuestos del XML y la factura no corresponden a la configuración. Favor de comprobar la información";
                        else
                        {
                            lBolIsSuccess = validUUID(pStrFolioFiscal);
                        }
                    }
                }
                else
                    //throw new Exception("El RFC del Proveedor no coincide. Favor de comprobar la información");

                    lStrErrorMessage = "El RFC del Proveedor no coincide. Favor de comprobar la información";
                return lBolIsSuccess;
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("ValidarUUID: " + ex.Message);
                LogUtility.WriteException(ex);
                lStrErrorMessage = ex.ToString();
                throw ex;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
                MemoryUtility.ReleaseComObject(lObjSBObob);
                MemoryUtility.ReleaseComObject(lObjBussinessPartners);
            }

        }

        /// <summary>
        /// Carga los campos con el UUID recibido como parametro
        /// </summary>
        /// <param name="pStrFolioFiscal"></param>
        private void CargarUUIDFactura(string pStrFolioFiscal)
        {
            SAPbouiCOM.EditText txtUUID = (lObjForm.Items.Item("480002119").Specific as SAPbouiCOM.EditText);
            SAPbouiCOM.EditText txtFecha = (lObjForm.Items.Item("46").Specific as SAPbouiCOM.EditText);
            if (!txtUUID.Active)
            {
                txtUUID.Active = true;
            }
            if (!txtFecha.Active)
            {
                txtFecha.Active = true;
            }

            txtUUID.Value = pStrFolioFiscal;
            //DateTime lDtmFecha = DateTime.Parse(mStrFechaDoc);
            //txtFecha.Value = lDtmFecha.ToString("yyyyMMdd");

            txtUUID.Active = false;
            txtFecha.Active = false;
        }

        /// <summary>
        /// Metodo utilizado para llenar el campo cuando existe 1 o mas registros con el mismo UUID y la factura actual esta creada
        /// </summary>
        /// <param name="pObjRecordSet"></param>
        /// <param name="pStrFolioFiscal"></param>
        private void ValidarUUIDFacturaMas2(SAPbobsCOM.Recordset pObjRecordSet, string pStrFolioFiscal)
        {
            bool IsCancelled = false;
            if (pObjRecordSet.RecordCount > 1)
            {
                while (!pObjRecordSet.EoF)
                {
                    int lIntDocEntry = int.Parse(pObjRecordSet.Fields.Item(0).Value.ToString());
                    SAPbobsCOM.Documents oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                    if (oInvoice2.GetByKey(lIntDocEntry))
                        if (oInvoice2.Cancelled == SAPbobsCOM.BoYesNoEnum.tNO)
                        {
                            IsCancelled = false;
                            Application.SBO_Application.MessageBox("Ya existe ese UUID en otra factura de venta con ID: " + oInvoice2.DocNum);
                        }
                        else
                            IsCancelled = true;
                    pObjRecordSet.MoveNext();
                }


            }
            else
            {
                int lIntDocEntry = int.Parse(pObjRecordSet.Fields.Item(0).Value.ToString());
                SAPbobsCOM.Documents oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                if (oInvoice2.GetByKey(lIntDocEntry))
                    if (oInvoice2.Cancelled == SAPbobsCOM.BoYesNoEnum.tNO)
                        Application.SBO_Application.MessageBox("Ya existe ese UUID en otra factura de venta con ID: " + oInvoice2.DocNum);
                    else
                        IsCancelled = true;
            }
            if (IsCancelled)
            {
                SAPbouiCOM.EditText txtUUID = (lObjForm.Items.Item("480002119").Specific as SAPbouiCOM.EditText);
                txtUUID.Value = pStrFolioFiscal;
            }
        }

        /// <summary>
        /// Metodo utilizado para llenar el campo cuando existe 1 o mas registros con el mismo UUID pero la factura actual no esta creada
        /// </summary>
        /// <param name="pObjRecordSet"></param>
        /// <param name="pStrFolioFiscal"></param>
        private void CargarUUIDFacturaDontExist(SAPbobsCOM.Recordset pObjRecordSet, string pStrFolioFiscal, string pStrTipoFactura)
        {
            if (pObjRecordSet.RecordCount > 1)
            {
                bool lBolNotCancelled = false;
                string lStrDocNum = string.Empty;
                while (!pObjRecordSet.EoF)
                {
                    int lIntDocEntry = int.Parse(pObjRecordSet.Fields.Item(0).Value.ToString());

                    SAPbobsCOM.Documents oInvoice2 = null;
                    switch (pStrTipoFactura)
                    {
                        case "FacturaCliente":
                            oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                            break;
                        case "FacturaProveedor":
                            oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                            break;
                        case "NotaCreditoCliente":
                            oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                            break;
                        case "NotaCreditoProveedor":
                            oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                            break;
                        default:
                            break;
                    }

                    if (oInvoice2 != null && oInvoice2.GetByKey(lIntDocEntry))
                        if (oInvoice2.Cancelled == SAPbobsCOM.BoYesNoEnum.tNO)
                        {
                            lBolNotCancelled = false;
                            lStrDocNum = oInvoice2.DocNum.ToString();
                        }
                        else
                            lBolNotCancelled = true;

                    pObjRecordSet.MoveNext();
                    MemoryUtility.ReleaseComObject(oInvoice2);
                }
                if (lBolNotCancelled)
                    CargarUUIDFactura(pStrFolioFiscal);
                else
                    Application.SBO_Application.MessageBox("Ya existe ese UUID en otra factura de venta con ID: " + lStrDocNum);
            }
            else
            {
                int lIntDocEntry = int.Parse(pObjRecordSet.Fields.Item(0).Value.ToString());
                SAPbobsCOM.Documents oInvoice2 = null;
                switch (pStrTipoFactura)
                {
                    case "FacturaCliente":
                        oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                        break;
                    case "FacturaProveedor":
                        oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;
                    case "NotaCreditoCliente":
                        oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                        break;
                    case "NotaCreditoProveedor":
                        oInvoice2 = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                        break;
                    default:
                        break;
                }
                if (oInvoice2.GetByKey(lIntDocEntry))
                {
                    if (oInvoice2.Cancelled == SAPbobsCOM.BoYesNoEnum.tNO)
                        Application.SBO_Application.MessageBox("Ya existe ese UUID en otra factura de venta con ID: " + oInvoice2.DocNum);
                    else
                        CargarUUIDFactura(pStrFolioFiscal);
                }
                MemoryUtility.ReleaseComObject(oInvoice2);
            }
        }


        private void showOpenFileDialog()
        {
            Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new Thread(ShowFolderBrowser);

                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }

                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("showOpenFileDialog " + ex.Message);
                LogUtility.WriteException(ex);
                Application.SBO_Application.MessageBox(ex.Message);
            }
            finally
            {
                ShowFolderBrowserThread.Abort();
            }
        }

        private void ShowFolderBrowser()
        {
            var oProcessess = Process.GetProcessesByName("SAP Business One");

            var oProcess = Process.GetCurrentProcess(); //.GetProcessesByName("SAP Business One").ToList();


            using (System.Windows.Forms.OpenFileDialog oFile = new System.Windows.Forms.OpenFileDialog())
            {
                string fileName = "";

                try
                {
                    IntPtr sapProc = GetForegroundWindow();
                    WindowWrapper MyWindow = null;
                    MyWindow = new WindowWrapper(sapProc);

                    oFile.Multiselect = false;
                    oFile.Filter = "Archivos XML(*.XML)|*.xml";
                    oFile.FilterIndex = 0;
                    oFile.RestoreDirectory = true;
                    var dialogResult = oFile.ShowDialog(MyWindow);
                    if (dialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            fileName = oFile.FileName;


                            XmlDocument lObjXmlDoc = new XmlDocument();
                            lObjXmlDoc.Load(fileName);
                            var tfdNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                            tfdNsM.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                            var comNsM = new XmlNamespaceManager(lObjXmlDoc.NameTable);
                            comNsM.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/3");
                            string lStrFolioFiscal = lObjXmlDoc.SelectSingleNode("//tfd:TimbreFiscalDigital", tfdNsM).Attributes["UUID"].Value;
                            string lStrVersion = string.Empty;

                            if (lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Version"] == null)
                            {
                                lStrVersion = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["version"].Value;
                                mStrFechaDoc = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["fecha"].Value;
                            }
                            else
                            {
                                lStrVersion = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Version"].Value;
                                mStrFechaDoc = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Fecha"].Value;
                            }

                            //bool valid = false;

                            //if (lStrVersion == "3.3")
                            //    valid = Validate33(fileName);
                            //else
                            //    valid = Validate32(fileName);


                            string lStrTotal = string.Empty;
                            if (lStrVersion == "3.3")
                                lStrTotal = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["Total"].Value;
                            else
                                lStrTotal = lObjXmlDoc.SelectSingleNode("//cfdi:Comprobante", comNsM).Attributes["total"].Value;
                            string lStrImpuestos = "0";
                            string lStrImpuestosRetenciones = "0";
                            XmlNode lObjXmlNode = lObjXmlDoc.SelectSingleNode("//cfdi:Impuestos", comNsM);

                            if (lObjXmlNode.ChildNodes.Count > 1)
                            {
                                XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                                lStrImpuestos = GetImpuestosTraslados(lObjXmlNodeTraslados, lStrVersion).ToString();
                                XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                                lStrImpuestosRetenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, lStrVersion).ToString();
                            }
                            else
                            {
                                if (lObjXmlNode.ChildNodes.Count == 1)
                                {
                                    switch (lObjXmlNode.ChildNodes[0].Name)
                                    {
                                        case "cfdi:Traslados":
                                            XmlNode lObjXmlNodeTraslados = lObjXmlNode.SelectSingleNode("//cfdi:Traslados", comNsM);
                                            lStrImpuestos = GetImpuestosTraslados(lObjXmlNodeTraslados, lStrVersion).ToString();
                                            break;
                                        case "cfdi:Retenciones":
                                            XmlNode lObjXmlNodeRetenciones = lObjXmlNode.SelectSingleNode("//cfdi:Retenciones", comNsM);
                                            lStrImpuestosRetenciones = GetImpuestosRetenciones(lObjXmlNodeRetenciones, lStrVersion).ToString();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            string lStrRFC = string.Empty;
                            if (lStrVersion == "3.3")
                                lStrRFC = lObjXmlDoc.SelectSingleNode("//cfdi:Emisor", comNsM).Attributes["Rfc"].Value;
                            else
                                lStrRFC = lObjXmlDoc.SelectSingleNode("//cfdi:Emisor", comNsM).Attributes["rfc"].Value;



                            ValidarXMLFacturaUUID(lStrImpuestos, lStrTotal, lStrImpuestosRetenciones, lStrRFC, lStrFolioFiscal);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error interno no identificado. Error: " + ex.ToString());
                        }
                    }
                    else
                        System.Windows.Forms.Application.ExitThread();
                }
                catch (Exception ex)
                {
                    fileName = "";
                    Application.SBO_Application.MessageBox("Error interno no identificado. Error: " + ex.ToString());
                    System.Windows.Forms.Application.ExitThread();
                }
            }
        }


        //validates purchaseInvoice when opening the file
        private bool validUUID(string UUID)
        {
            bool found = false;
            try
            {
                string docId = getInvoices(UUID);

                if (!docId.Equals(""))
                {
                    found = true;
                    Application.SBO_Application.MessageBox("Ya existe ese UUID en otro documento con ID: " + docId);
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("ValidarUUID: " + ex.Message);
                LogUtility.WriteException(ex);
            }
            return !found;
        }
        private string getInvoices(string UUID)
        {
            string docId = "";
            try
            {
                //FacturaCliente
                docId = checkDocuments(UUID, SAPbobsCOM.BoObjectTypes.oInvoices);

                if (docId.Equals(""))////"FacturaProveedor"
                    docId = checkDocuments(UUID, SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);

                if (docId.Equals(""))//"NotaCreditoCliente"
                    docId = checkDocuments(UUID, SAPbobsCOM.BoObjectTypes.oCreditNotes);

                if (docId.Equals(""))//"NotaCreditoProveedor"
                    docId = checkDocuments(UUID, SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("getInvoices: " + ex.Message);
                LogUtility.WriteException(ex);
                throw;
            }
            return docId;
        }
        private string checkDocuments(string UUID, SAPbobsCOM.BoObjectTypes docType)
        {
            string docId = "";
            try
            {
                SAPbobsCOM.SBObob lObjSBObob = (SAPbobsCOM.SBObob)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                SAPbobsCOM.Recordset lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(docType, "EDocNum", UUID, SAPbobsCOM.BoQueryConditions.bqc_Equal);

                if (lObjRecordSet.RecordCount > 0)//"FacturaCliente"            
                    docId = validUUIDDetail(lObjRecordSet, UUID, docType);

                MemoryUtility.ReleaseComObject(lObjRecordSet);
                MemoryUtility.ReleaseComObject(lObjSBObob);
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("checkDocuments: " + ex.Message);
                LogUtility.WriteException(ex);
                throw;
            }
            return docId;
        }
        private string validUUIDDetail(SAPbobsCOM.Recordset pObjRecordSet, string pStrFolioFiscal, SAPbobsCOM.BoObjectTypes docType)
        {
            string docId = "";

            string lStrDocNum = string.Empty;

            while (!pObjRecordSet.EoF)
            {
                int lIntDocEntry = int.Parse(pObjRecordSet.Fields.Item(0).Value.ToString());

                SAPbobsCOM.Documents oInv = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(docType);

                if (oInv != null && oInv.GetByKey(lIntDocEntry))
                {
                    if (oInv.DocumentStatus == SAPbobsCOM.BoStatus.bost_Open)
                        if (oInv.Cancelled == SAPbobsCOM.BoYesNoEnum.tNO)
                        {

                            docId = oInv.DocNum.ToString();
                        }

                }

                pObjRecordSet.MoveNext();
                MemoryUtility.ReleaseComObject(oInv);
            }


            MemoryUtility.ReleaseComObject(pObjRecordSet);

            return docId;
        }

        private void importXML()
        {
            string lStrDocNum = (lObjForm.Items.Item("8").Specific as SAPbouiCOM.EditText).Value;
            string lStrTotal = (lObjForm.Items.Item("29").Specific as SAPbouiCOM.EditText).Value;

            SAPbobsCOM.SBObob lObjSBObob = (SAPbobsCOM.SBObob)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.Documents oInvoice = null;

            try
            {
                lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices, "DocNum", lStrDocNum, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        int lIntDocEntry = int.Parse(lObjRecordSet.Fields.Item(0).Value.ToString());
                        oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        if (oInvoice.GetByKey(lIntDocEntry))
                        {
                            showOpenFileDialog();
                        }
                    }
                }
                else
                {
                    oInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);

                    if (string.IsNullOrEmpty(lStrTotal) || lStrTotal == "0")
                    {
                        Application.SBO_Application.MessageBox("La información no ha sido capturada");
                    }
                    else
                    {
                        showOpenFileDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteError("importXML: " + ex.Message);
                LogUtility.WriteException(ex);
                throw ex;
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
                MemoryUtility.ReleaseComObject(lObjSBObob);
            }
        }
    }


}