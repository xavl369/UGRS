using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UGRS.AddOn.PurchaseInvoice.Models;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.PurchaseInvoice
{
    public class ModalFormXml
    {

        private SAPbouiCOM.Form lObjFormXml = null;
        public SAPbouiCOM.ChooseFromList pObjChooseFromList = null;
        //private SAPbobsCOM.Company lObjCompany = null;


        #region SAP Obj
        SAPbouiCOM.EditText txtNumSocioNegocio = null;
        SAPbouiCOM.EditText txtNombre = null;
        SAPbouiCOM.EditText txtRFC = null;
        SAPbouiCOM.EditText txtSubTotal = null;
        SAPbouiCOM.EditText txtImpuestos = null;
        SAPbouiCOM.EditText txtTotal = null;
        SAPbouiCOM.EditText ltxtMoneda = null;
        SAPbouiCOM.Matrix oMatrix = null;
        SAPbouiCOM.ComboBox lObjComboBox = null;
        #endregion
        #region ChooseFromList
        SAPbouiCOM.ChooseFromListCollection oCFLs = null;
        SAPbouiCOM.Conditions oCons = null;
        SAPbouiCOM.Condition oCon = null;
        SAPbouiCOM.Column oColumn = null;
        #endregion

        List<Concepts> lListxmlConceptLines = new List<Concepts>();
        public List<Concepts> pListxmlConceptLines2 = new List<Concepts>();

        SAPbouiCOM.DataTable oDTx = null;

        public mPurchaseInvoice lObjPurchaseInvoice = new mPurchaseInvoice();

        private string lStrDocType = "";
        private string lStrFormType = "";

        public ModalFormXml(SAPbouiCOM.Form pObjFormXml, mPurchaseInvoice pObjPurchaseInvoice, string pStrDocType, string pStrFormType)
        {
            this.lObjFormXml = pObjFormXml;
            this.lObjPurchaseInvoice = pObjPurchaseInvoice;
            //this.lObjCompany = (SAPbobsCOM.Company)pObjCompany;
            this.lStrDocType = pStrDocType;
            this.lStrFormType = pStrFormType;
            switch (pStrDocType)
            {
                case "Servicios":
                    LoadFormXml("FormXml.xml", "FormXmlService");
                    setItems();
                    LoadFormEvents();
                    break;
                case "Articulos":
                    LoadFormXml("FormXmlArticles.xml", "FormXmlArticle");
                    setItems();
                    LoadFormEvents();
                    break;
            }



        }

        private void LoadFormEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(comboBoxAfterEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        public void UnloadFormEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(comboBoxAfterEvent);
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        private void LoadFormXml(string FileName, string FormName)
        {
            System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
            string sPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]).ToString();

            oXmlDoc.Load(sPath + "\\" + FileName);

            SAPbouiCOM.FormCreationParams creationPackage = (SAPbouiCOM.FormCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                (SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            creationPackage.XmlData = oXmlDoc.InnerXml;

            if (FormName.Equals("FormXmlArticle") || FormName.Equals("FormXmlService"))
            {

                creationPackage.UniqueID = FormName;
                creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
                if (FormName.Equals("FormXmlArticle"))
                {
                    creationPackage.FormType = "FormXmlArticle";
                }
                else if (FormName.Equals("FormXmlService"))
                {
                    creationPackage.FormType = "FormXmlService";
                }
                //creationPackage.

                lObjFormXml = SAPbouiCOM.Framework.Application.SBO_Application.Forms.AddEx(creationPackage);
                lObjFormXml.Title = "Importación XML";
                lObjFormXml.Left = 400;
                lObjFormXml.Top = 100;
                lObjFormXml.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                lObjFormXml.Visible = true;
                initFormXml();
            }

        }

        // Initialize Items

        private void initFormXml()
        {
            lObjFormXml.Freeze(true);
            initItems();
            lObjFormXml.Freeze(false);
        }

        private void initItems()
        {
            txtNumSocioNegocio = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_0").Specific);
            txtNombre = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_1").Specific);
            txtRFC = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_2").Specific);
            txtSubTotal = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_7").Specific);
            txtImpuestos = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_8").Specific);
            txtTotal = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_9").Specific);
            ltxtMoneda = ((SAPbouiCOM.EditText)lObjFormXml.Items.Item("Item_17").Specific);
            oMatrix = ((SAPbouiCOM.Matrix)lObjFormXml.Items.Item("Item_6").Specific);
            lObjComboBox = ((SAPbouiCOM.ComboBox)lObjFormXml.Items.Item("Item_15").Specific);
            initMatrixFormXml();

        }

        private void initMatrixFormXml()
        {
            lObjFormXml.DataSources.DataTables.Add("XmlResult");

            oDTx = lObjFormXml.DataSources.DataTables.Item("XmlResult");

            oDTx.Columns.Add("Col_0", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTx.Columns.Add("Col_1", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            oDTx.Columns.Add("Col_2", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            if (lStrDocType.Equals("Articulos"))
            {
                oDTx.Columns.Add("Col_3", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            }

        }

        //Set Items
        private void setComboBox()
        {
            lObjFormXml.DataSources.UserDataSources.Add("UDCBX", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 30);
            lObjComboBox.DataBind.SetBound(true, "", "UDCBX");

            lObjComboBox.ValidValues.Add("MXP", "Pesos Mexicanos");
            lObjComboBox.ValidValues.Add("USD", "Dolares");
            lObjComboBox.ValidValues.Add("EUR", "Euros");

            //AddChooseFromList();


        }

        private void setItems()
        {
            txtNumSocioNegocio.Value = lObjPurchaseInvoice.CardCode;
            txtNombre.Value = lObjPurchaseInvoice.NombreSocioNegocio;
            txtRFC.Value = lObjPurchaseInvoice.RFCProveedor;
            txtSubTotal.Value = lObjPurchaseInvoice.SubTotal;
            txtImpuestos.Value = lObjPurchaseInvoice.Impuestos;
            txtTotal.Value = lObjPurchaseInvoice.Total;
            ltxtMoneda.Value = lObjPurchaseInvoice.MonedaXml;
            setComboBox();
        }

        private void AddChooseFromList()//aqui break point
        {

            lObjFormXml.DataSources.UserDataSources.Add("UDCFL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            string Cbx = lObjComboBox.Value.Trim();

            try
            {
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                SAPbouiCOM.Conditions oCons = null;
                SAPbouiCOM.Condition oCon = null;
                SAPbouiCOM.Column oColumn = null;

                oCFLs = lObjFormXml.ChooseFromLists;

                SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
                lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                    (SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                //  Adding 2 CFL, one for the button and one for the edit text.
                //string strCFLID = oCFLCreationParams.UniqueID
                lObjCFLCreationParams.MultiSelection = false;
                if (lStrDocType.Equals("Servicios"))
                {
                    lObjCFLCreationParams.ObjectType = "1";
                }
                else
                {
                    lObjCFLCreationParams.ObjectType = "4";
                }
                lObjCFLCreationParams.UniqueID = "CFLACT";

                pObjChooseFromList = oCFLs.Add(lObjCFLCreationParams);

                //  Adding Conditions to CFL1
                oCons = pObjChooseFromList.GetConditions();

                //Onlu Postable Accounts
                if (lStrDocType.Equals("Servicios") && lStrFormType != "65301")
                {
                    if (Cbx == "MXP")
                    {
                        #region MXP
                        oCon = oCons.Add();
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "EUR";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "USD";
                        #endregion
                    }
                    else if (Cbx == "USD")
                    {
                        #region USD
                        oCon = oCons.Add();
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "EUR";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "MXP";
                        #endregion
                    }
                    else if (Cbx == "EUR")
                    {
                        #region EUR
                        oCon = oCons.Add();
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "USD";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "MXP";
                        #endregion
                    }
                }
                else if (lStrDocType.Equals("Servicios") && lStrFormType == "65301")
                {
                    #region advances to suppliers
                    oCon = oCons.Add();
                    oCon.Alias = "AcctCode";
                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "11410000";
                    #endregion
                }
                else
                {
                    oCon = oCons.Add();
                    oCon.Alias = "validFor";
                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "Y";
                }
                pObjChooseFromList.SetConditions(oCons);


                if(oMatrix.RowCount> 0)
                {
                    for (int i = oMatrix.RowCount; i >= 1; i--)
                    {
                        if (oMatrix.RowCount == 1)
                        {
                            oMatrix.ClearRowData(i);
                            oMatrix.DeleteRow(i);
                        }
                        else
                        {
                            oMatrix.DeleteRow(i);
                        }
                    }
                }

                oColumn = oMatrix.Columns.Item("Col_0");

                try { 
                    oColumn.DataBind.SetBound(true, "", "UDCFL");
                }
                catch(Exception)
                {
                    
                }
                oColumn.ChooseFromListUID = pObjChooseFromList.UniqueID;
                if (lStrDocType.Equals("Servicios"))
                {
                    oColumn.ChooseFromListAlias = "AcctCode";
                }
                else
                {
                    oColumn.ChooseFromListAlias = "ItemCode";
                }


            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
            }

        }

        private void ChangeChooseFromList()
        {
            string Cbx = lObjComboBox.Value.Trim();
            try
            {

                oCFLs = lObjFormXml.ChooseFromLists;

                pObjChooseFromList = (SAPbouiCOM.ChooseFromList)oCFLs.Item("CFLACT");


                //  Adding Conditions to CFL1
                oCons = pObjChooseFromList.GetConditions();

                //Condicion para filtrar solo cuentas afectables
                if (lStrDocType.Equals("Servicios") && lStrFormType != "65301")
                {
                    if (Cbx == "MXP")
                    {
                        #region Moneda nacional
                        oCon = oCons.Item(0);
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(1);
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(2);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "EUR";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(3);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "USD";
                        #endregion
                    }
                    else if (Cbx == "USD")
                    {
                        #region Dolares
                        oCon = oCons.Item(0);
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(1);
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(2);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "EUR";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(3);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "MXP";
                        #endregion
                    }
                    else if (Cbx == "EUR")
                    {
                        #region Euros
                        oCon = oCons.Item(0);
                        oCon.Alias = "Postable";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(1);
                        oCon.Alias = "LocManTran";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(2);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "USD";

                        oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        oCon = oCons.Item(3);
                        oCon.Alias = "ActCurr";
                        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                        oCon.CondVal = "MXP";
                        #endregion
                    }
                }

                pObjChooseFromList.SetConditions(oCons);

                if (oMatrix.RowCount > 0)
                {
                    for (int i = oMatrix.RowCount; i >= 1; i--)
                    {
                        if (oMatrix.RowCount == 1)
                        {
                            oMatrix.ClearRowData(i);
                            oMatrix.DeleteRow(i);
                        }
                        else
                        {
                            oMatrix.DeleteRow(i);
                        }
                    }
                }

                oColumn = oMatrix.Columns.Item("Col_0");
                oColumn.DataBind.SetBound(true, "", "UDCFL");
                oColumn.ChooseFromListUID = pObjChooseFromList.UniqueID;
                if (lStrDocType.Equals("Servicios"))
                {
                    oColumn.ChooseFromListAlias = "AcctCode";
                }
                else
                {
                    oColumn.ChooseFromListAlias = "ItemCode";
                }

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + err.StackTrace);
            }
        }

        //Fill FormXml Matrix
        private void fillMatrix()
        {
            if (lStrDocType.Equals("Servicios"))
            {
                for (int i = 0; i < lObjPurchaseInvoice.ConceptLines.Count; i++)
                {
                    oDTx.Rows.Add();

                    oDTx.Columns.Item("Col_1").Cells.Item(i).Value = lObjPurchaseInvoice.ConceptLines[i].Descripcion;
                    oDTx.Columns.Item("Col_2").Cells.Item(i).Value = lObjPurchaseInvoice.ConceptLines[i].ValorUnitario;

                    oMatrix.Columns.Item("Col_1").DataBind.Bind("XmlResult", "Col_1");
                    oMatrix.Columns.Item("Col_2").DataBind.Bind("XmlResult", "Col_2");
                }
            }
            else if (lStrDocType.Equals("Articulos"))
            {
                for (int i = 0; i < lObjPurchaseInvoice.ConceptLines.Count; i++)
                {
                    oDTx.Rows.Add();

                    oDTx.Columns.Item("Col_1").Cells.Item(i).Value = lObjPurchaseInvoice.ConceptLines[i].Descripcion;
                    oDTx.Columns.Item("Col_2").Cells.Item(i).Value = lObjPurchaseInvoice.ConceptLines[i].Cantidad;
                    oDTx.Columns.Item("Col_3").Cells.Item(i).Value = lObjPurchaseInvoice.ConceptLines[i].ValorUnitario;

                    oMatrix.Columns.Item("Col_1").DataBind.Bind("XmlResult", "Col_1");
                    oMatrix.Columns.Item("Col_2").DataBind.Bind("XmlResult", "Col_2");
                    oMatrix.Columns.Item("Col_3").DataBind.Bind("XmlResult", "Col_3");
                }
            }

            //AddChooseFromList();

            oMatrix.LoadFromDataSource();
        }

        //Events

        private void chooseFromListAfterEvent(string FormUID, ref SAPbouiCOM.ItemEvent pValEvent, out bool BubbleEvent)
        {
            BubbleEvent = true;



            if (pValEvent.EventType != SAPbouiCOM.BoEventTypes.et_FORM_CLOSE || pValEvent.EventType != SAPbouiCOM.BoEventTypes.et_FORM_DATA_DELETE)
            {
                if (pValEvent.ColUID == "Col_0" && pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {

                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;

                    SAPbouiCOM.ChooseFromList oCFL = null;
                    
                    if (oCFLEvento.SelectedObjects == null)
                        return;

                    try { 
                        oCFL = lObjFormXml.ChooseFromLists.Item(sCFL_ID);
                    }
                    catch(Exception)
                    {
                        return;
                    }

                    SAPbouiCOM.DataTable oDataTable = null;
                    oDataTable = oCFLEvento.SelectedObjects;

                    var Descrpition = "";
                    var Num = "";

                    try
                    {
                        string val = null;
                        val = System.Convert.ToString(oDataTable.GetValue(0, 0));

                        if (val != null)
                        {
                            if (pValEvent.ItemUID == "Item_6")
                            {

                                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("Col_0").Cells.Item(pValEvent.Row).Specific).Value == "")
                                {
                                    oMatrix.Columns.Item("Col_0").Cells.Item(pValEvent.Row).Click();
                                    oMatrix.Item.Click();
                                }

                                if (pListxmlConceptLines2.Count == 0)
                                {
                                    Concepts lObjConcepto = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1];
                                    if (lStrDocType.Equals("Servicios"))
                                    {
                                        lObjConcepto.NumeroCuenta = val;
                                    }
                                    else
                                    {
                                        lObjConcepto.Articulo = val;
                                    }
                                    pListxmlConceptLines2.Add(lObjConcepto);
                                }
                                else
                                {
                                    ///Validate the account change

                                    if (lStrDocType.Equals("Servicios"))
                                    {
                                        Descrpition = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].Descripcion;
                                        Num = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].NumeroCuenta;

                                        if (pListxmlConceptLines2.Find(x => x.NumeroCuenta == Num && x.Descripcion == Descrpition) != null)
                                        {
                                            pListxmlConceptLines2.Where(x => x.NumeroCuenta == Num && x.Descripcion == Descrpition).Select(x => x.NumeroCuenta = val).ToList();
                                        }
                                        else
                                        {
                                            Concepts lObjConcepto = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1];
                                            lObjConcepto.NumeroCuenta = val;
                                            pListxmlConceptLines2.Add(lObjConcepto);
                                        }

                                    }
                                    else
                                    {
                                        Descrpition = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].Descripcion;
                                        Num = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].Articulo;

                                        if (pListxmlConceptLines2.Find(x => x.Articulo == Num && x.Descripcion == Descrpition) != null)
                                        {
                                            pListxmlConceptLines2.Where(x => x.Articulo == Num && x.Descripcion == Descrpition).Select(x => x.Articulo = val).ToList();
                                        }
                                        else
                                        {
                                            Concepts lObjConcepto = lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1];
                                            lObjConcepto.Articulo = val;
                                            pListxmlConceptLines2.Add(lObjConcepto);
                                        }
                                    }
                                }
                                if (lObjPurchaseInvoice.Version == "3.3")
                                {
                                    if ((lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].ClaveItmProd != "" && lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].ClaveItmProd != null) && !lStrDocType.Equals("Servicios"))
                                    {
                                        SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        lObjRecordSet.DoQuery("select ItemSAT.NcmCode as Code from OITM Items inner join ONCM ItemSAT on Items.NCMCode = ItemSAT.AbsEntry where Items.ItemCode = '" + val + "'");
                                        if (lObjRecordSet.RecordCount == 1)
                                        {
                                            string lStrCode = lObjRecordSet.Fields.Item(0).Value.ToString();
                                            if (!(lStrCode == lObjPurchaseInvoice.ConceptLines[pValEvent.Row - 1].ClaveItmProd))
                                                throw new Exception("Los codigos de articulos con coinciden.");
                                        }
                                        else
                                        {
                                            throw new Exception("El artitulo no se encuentra mapeado.");
                                        }
                                        
                                    }
                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("Col_0").Cells.Item(pValEvent.Row).Specific).Value = val;
                                }
                                else
                                {
                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("Col_0").Cells.Item(pValEvent.Row).Specific).Value = val;
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                        }


                    }
                }
            }
        }

        private void comboBoxAfterEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_DEACTIVATE && pVal.EventType != SAPbouiCOM.BoEventTypes.et_VALIDATE && pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_CLOSE)
            {

                try
                {
                    if (pVal.ItemUID == "Item_15" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT)
                    {

                        if (pObjChooseFromList == null && lObjComboBox.Value != "")
                        {
                            AddChooseFromList();
                        }
                        else if (lObjComboBox.Value != "" && lStrFormType != "65301" && lStrDocType.Equals("Servicios"))
                        {
                            ChangeChooseFromList();
                        }
                        int cn = oMatrix.Columns.Item("Col_1").Cells.Count;
                        if (oMatrix.Columns.Item("Col_1").Cells.Count <= 1 && lObjComboBox.Value != "")
                        {
                            fillMatrix();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        //Fill original form matrix
        public mPurchaseInvoice FillObject()
        {
            lObjPurchaseInvoice.ConceptLines = pListxmlConceptLines2;
            lObjPurchaseInvoice.MonedaDocumento = lObjComboBox.Value;
            return lObjPurchaseInvoice;
        }

        public bool ValidateCompleteLines()
        {
            bool val;
            if (lObjPurchaseInvoice.ConceptLines.Count == pListxmlConceptLines2.Count) { val = true; }
            else { val = false; }

            return val;
        }

        //Check the correct currency with the BP
        public bool ValidateCorrectCurrency()
        {
            string lStrCurrencyByBP = getCurrencyByBP();
            string lStrCBCurrency = lObjComboBox.Value.Trim();

            if (lStrCurrencyByBP == "##" && lStrCBCurrency != "")
            {
                return true;
            }
            else if (lStrCurrencyByBP == lStrCBCurrency && lStrCBCurrency != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getCurrencyByBP()
        {
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string lStrCurrencyByBP = string.Empty;

            try
            {
                lObjRecordSet.DoQuery("SELECT Currency FROM OCRD WHERE LicTradNum='" + lObjPurchaseInvoice.RFCProveedor + "' ");

                if (lObjRecordSet.RecordCount != 0)
                {
                    lStrCurrencyByBP = lObjRecordSet.Fields.Item(0).Value.ToString();
                }
                else
                {
                    //return lStrCurrencyByBP;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }
            
            return lStrCurrencyByBP;
        }

        //Check every line if the currency is correct with the account
        public bool ValidLines(int[] array)
        {
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            int lIntLines = 0;
            int lIntNum = 0;
            bool value = false;

            try
            {
                foreach (var item in pListxmlConceptLines2)
                {
                    lObjRecordSet.DoQuery("SELECT ActCurr FROM OACT WHERE AcctCode='" + item.NumeroCuenta + "' ");
                    if (lObjRecordSet.Fields.Item("ActCurr").Value.ToString() == lObjComboBox.Value.Trim() || lObjRecordSet.Fields.Item("ActCurr").Value.ToString() == "##")
                    {
                        lIntNum++;
                    }
                    else
                    {
                        array[lIntLines] = lIntLines;

                    }
                    lIntLines++;
                }
                if (lIntNum == pListxmlConceptLines2.Count)
                {
                    value = true;
                }
                else
                {
                    value = false;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }

            return value;
        }

        //Set every line if the currency is incorrect with the account
        public bool Lines()
        {
            int[] lArrIntLines = new int[pListxmlConceptLines2.Count];
            if (ValidLines(lArrIntLines))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < lArrIntLines.Length; i++)
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("Col_0").Cells.Item(lArrIntLines[i] + 1).Specific).Value = "";
                }
                return false;
            }
        }

        public void StopObjects()
        {

            pObjChooseFromList = null;
            pListxmlConceptLines2.Clear();

        }


    }
}
