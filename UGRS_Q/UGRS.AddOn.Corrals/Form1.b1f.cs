using System;
using System.Collections.Generic;
using System.Xml;
using SAPbouiCOM.Framework;
using UGRS.Core.Utility;
using ContabilidadElectronicaAddOn.Utils;
using System.Linq;

namespace UGRS.AddOn.Traslados
{
    [FormAttribute("UGRS.AddOn.Traslados.Form1", "Form1.b1f")]
    class Form1 : UserFormBase
    {

        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Button Button1;
        SAPbobsCOM.Company oCompany;
        SAPbouiCOM.Form oForm;
        private SAPbouiCOM.ChooseFromList mObjChooseFromList;
        SAPbouiCOM.ChooseFromListCollection oCFLs;
        SAPbouiCOM.Conditions oCons;
        SAPbouiCOM.Condition oCon;
        SAPbouiCOM.Column oColumn;
        private SAPbouiCOM.Matrix Matrix1;
        List<object> lLstExtraData;

        public Form1()
        {
            oForm = Application.SBO_Application.Forms.Item(this.UIAPIRawForm.UniqueID);
            oCompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();
            //LoadMatrixPeticiones();
            Matrix1.Columns.Item("Col_7").Visible = false;
            Matrix1.Columns.Item("Col_8").Visible = false;
            LoadFormEvents();
            AddChooseFromListSupplier();
            AddChooseFromListStockTransfer();
        }
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("Item_1").Specific));
            this.EditText0.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText0_KeyDownAfter);
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("Item_3").Specific));
            this.EditText1.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText1_KeyDownAfter);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_5").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_6").Specific));
            this.Matrix1 = ((SAPbouiCOM.Matrix)(this.GetItem("Item_7").Specific));
            this.Matrix1.KeyDownAfter += new SAPbouiCOM._IMatrixEvents_KeyDownAfterEventHandler(this.Matrix1_KeyDownAfter);
            this.OnCustomInitialize();

        }


        private void LoadFormEvents()
        {

            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        public void UnloadFormEvents()
        {

            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }

        private void chooseFromListAfterEvent(string FormUID, ref SAPbouiCOM.ItemEvent pValEvent, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pValEvent.EventType != SAPbouiCOM.BoEventTypes.et_FORM_CLOSE)
            {

                if (pValEvent.ItemUID == "Item_1" && pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {

                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;

                    SAPbouiCOM.ChooseFromList oCFL = null;
                    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);

                    SAPbouiCOM.DataTable oDataTable = null;
                    oDataTable = oCFLEvento.SelectedObjects;
                    string val = null;
                    try
                    {

                        val = System.Convert.ToString(oDataTable.GetValue(0, 0));
                        EditText0.Value = val;

                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }

                        if (ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            LoadMatrixPeticiones(val);
                        }
                    }
                }

                if (pValEvent.ItemUID == "Item_3" && pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {

                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;

                    SAPbouiCOM.ChooseFromList oCFL = null;
                    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);

                    SAPbouiCOM.DataTable oDataTable = null;
                    oDataTable = oCFLEvento.SelectedObjects;
                    string val = null;
                    try
                    {

                        val = System.Convert.ToString(oDataTable.GetValue(1, 0));
                        EditText1.Value = val;

                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }

                        if (ex.Message.Contains("Can't set value on item because the item can't get focus."))
                        {
                            LoadMatrixPeticiones();
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            //  this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.CloseAfter += new CloseAfterHandler(this.Form_CloseAfter);

        }

        private SAPbouiCOM.StaticText StaticText0;

        private void OnCustomInitialize()
        {

        }

        private void AddChooseFromListSupplier()
        {
            oForm.DataSources.UserDataSources.Add("CFL_0", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            SAPbouiCOM.ChooseFromListCollection oCFLs = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
            lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                (SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            //  Adding 2 CFL, one for the button and one for the edit text.
            //string strCFLID = oCFLCreationParams.UniqueID
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "2";
            lObjCFLCreationParams.UniqueID = "CFLACT";
            mObjChooseFromList = oCFLs.Add(lObjCFLCreationParams);
            oCons = mObjChooseFromList.GetConditions();

            oCon = oCons.Add();
            oCon.Alias = "CardType";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "C";

            mObjChooseFromList.SetConditions(oCons);

            EditText0.DataBind.SetBound(true, "", "CFL_0");
            EditText0.ChooseFromListUID = mObjChooseFromList.UniqueID;
            EditText0.ChooseFromListAlias = "CardCode";
        }

        private void AddChooseFromListStockTransfer()
        {
            oForm.DataSources.UserDataSources.Add("CFL_1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            SAPbouiCOM.ChooseFromListCollection oCFLs = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
            lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                (SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

            //  Adding 2 CFL, one for the button and one for the edit text.
            //string strCFLID = oCFLCreationParams.UniqueID
            lObjCFLCreationParams.MultiSelection = false;
            lObjCFLCreationParams.ObjectType = "1250000001";
            lObjCFLCreationParams.UniqueID = "CFLDOC";
            mObjChooseFromList = oCFLs.Add(lObjCFLCreationParams);
            oCons = mObjChooseFromList.GetConditions();

            oCon = oCons.Add();
            oCon.Alias = "DocStatus";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "O";

            oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;


            oCon = oCons.Add();
            oCon.Alias = "ToWhsCode";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "SUHE";  /// dejar configurable ?


            mObjChooseFromList.SetConditions(oCons);

            EditText1.DataBind.SetBound(true, "", "CFL_1");
            EditText1.ChooseFromListUID = mObjChooseFromList.UniqueID;
            EditText1.ChooseFromListAlias = "DocNum";
        }


        private void LoadMatrixPeticiones(string pStrCliente = null)
        {
            Matrix1.Clear();
            string lStrQuery;
            if (EditText1.Value != "" || EditText1.Value != null)
            {
                if (pStrCliente == null || pStrCliente == "")
                    lStrQuery = "SELECT 'N' AS Seleccionar, cast(sum(T2.OpenInvQty) as decimal(18,2)) AS Cantidad, T2.FromWhsCod AS Almacen, T2.U_CR_Cardcode AS ClienteCod, T4.CardName AS Cliente, T2.ItemCode AS ItemCode, T3.ItemName Tipo, cast(cast(sum(T2.OpenInvQty) as decimal(18,2)) as varchar(8)) AS Trans FROM OWTQ T1 INNER JOIN WTQ1 T2 ON T1.DocEntry = T2.DocEntry INNER JOIN OCRD T4 ON T4.cardcode = T2.U_CR_Cardcode INNER JOIN OITM T3 ON T3.ItemCode = T2.ItemCode  WHERE T1.DocNum = '" + EditText1.Value + "' AND T2.InvntSttus = 'O' group by T2.FromWhsCod , T2.U_CR_Cardcode , T4.CardName , T2.ItemCode , T3.ItemName";

                else
                    lStrQuery = "SELECT 'N' AS Seleccionar, cast(sum(T2.OpenInvQty) as decimal(18,2)) AS Cantidad, T2.FromWhsCod AS Almacen, T2.U_CR_Cardcode AS ClienteCod, T4.CardName AS Cliente, T2.ItemCode AS ItemCode, T3.ItemName Tipo, cast(cast(sum(T2.OpenInvQty) as decimal(18,2)) as varchar(8)) AS Trans FROM OWTQ T1 INNER JOIN WTQ1 T2 ON T1.DocEntry = T2.DocEntry INNER JOIN OCRD T4 ON T4.cardcode = T2.U_CR_Cardcode INNER JOIN OITM T3 ON T3.ItemCode = T2.ItemCode  WHERE T1.DocNum = '" + EditText1.Value + "' and T2.U_CR_Cardcode = '" + pStrCliente + "'  AND T2.InvntSttus = 'O' group by T2.FromWhsCod , T2.U_CR_Cardcode , T4.CardName , T2.ItemCode , T3.ItemName";

                oForm.DataSources.DataTables.Item("dtMatrix").ExecuteQuery(lStrQuery);

                if (oForm.DataSources.DataTables.Item("dtMatrix").Rows.Count == 0)
                    Application.SBO_Application.StatusBar.SetText("Sin resultados.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                else
                {
                    Matrix1.Columns.Item("Col_0").DataBind.Bind("dtMatrix", "Seleccionar");
                    Matrix1.Columns.Item("Col_1").DataBind.Bind("dtMatrix", "Cliente");
                    Matrix1.Columns.Item("Col_2").DataBind.Bind("dtMatrix", "Tipo");
                    Matrix1.Columns.Item("Col_3").DataBind.Bind("dtMatrix", "Almacen");
                    Matrix1.Columns.Item("Col_4").DataBind.Bind("dtMatrix", "Cantidad");
                    Matrix1.Columns.Item("Col_5").DataBind.Bind("dtMatrix", "Trans");
                    Matrix1.Columns.Item("Col_6").DataBind.Bind("dtMatrix", "ClienteCod");
                    Matrix1.Columns.Item("Col_6").Visible = false;
                    Matrix1.Columns.Item("Col_7").DataBind.Bind("dtMatrix", "ItemCode");
                    Matrix1.LoadFromDataSource();
                }
                Matrix1.AutoResizeColumns();
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Es necesario seleccionar la solicitud de transferencia", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            
        }


        private void EditText0_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 13)
            {
                LoadMatrixPeticiones(EditText0.Value);
            }

        }


        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
           // createTransfer();
            
            
            
            BubbleEvent = true;
            try
            {
                if (EditText1.Value == "")
                {
                    Application.SBO_Application.MessageBox("Favor de capturar el Folio de la solicitud");
                    return;
                }
                SAPbobsCOM.StockTransfer lObjStockTrasnfer;
                string lStrFolio = EditText1.Value;
                SAPbobsCOM.SBObob lObjSBObob = (SAPbobsCOM.SBObob)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet = lObjSBObob.GetObjectKeyBySingleValue(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest, "DocNum", lStrFolio, SAPbobsCOM.BoQueryConditions.bqc_Equal);
                SAPbobsCOM.StockTransfer lObjInventory = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);
                if (lObjRecordSet.RecordCount == 1)
                {
                    lObjInventory.GetByKey(int.Parse(lObjRecordSet.Fields.Item(0).Value.ToString()));

                }

                Memory.ReleaseComObject(lObjSBObob);
                Memory.ReleaseComObject(lObjRecordSet);

                SAPbobsCOM.Recordset lObjRecordSet2 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet2.DoQuery("select T0.U_CR_Batch, T0.DocEntry,T0.LineNum, T0.OpenQty, T0.WhsCode, T0.FromWhsCod, T0.U_CR_CardCode, T0.ItemCode from WTQ1 T0 inner join OWTQ T1 on T1.DocEntry = T0.DocEntry Inner join OCRD T3 on T3.cardcode = T0.U_CR_CardCode where T1.DocNum = '" + lStrFolio + "' and T0.InvntSttus = 'O'");

                lLstExtraData = new List<object>();

                for (int i = 0; i < lObjRecordSet2.RecordCount; i++)
                {

                    lLstExtraData.Add(new
                    {
                        Lote = lObjRecordSet2.Fields.Item(0).Value.ToString(),
                        DocEntry = lObjRecordSet2.Fields.Item(1).Value.ToString(),
                        LineNum = lObjRecordSet2.Fields.Item(2).Value.ToString(),
                        Quantity = lObjRecordSet2.Fields.Item(3).Value.ToString(),
                        Whscode = lObjRecordSet2.Fields.Item(4).Value.ToString(),
                        FromWhscode = lObjRecordSet2.Fields.Item(5).Value.ToString(),
                        CardCode = lObjRecordSet2.Fields.Item(6).Value.ToString(),
                        ItemCode = lObjRecordSet2.Fields.Item(7).Value.ToString()

                    });
                    lObjRecordSet2.MoveNext();
                }

                Memory.ReleaseComObject(lObjRecordSet2);




                lObjStockTrasnfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

                bool lBolIsSuccesss = false;
                for (int i = 1; i <= Matrix1.RowCount; i++)
                {
                    SAPbouiCOM.CheckBox lObjChk = (SAPbouiCOM.CheckBox)Matrix1.Columns.Item(0).Cells.Item(i).Specific;

                    if (lObjChk.Checked)
                    {
                        lObjStockTrasnfer.FromWarehouse = ((SAPbouiCOM.EditText)Matrix1.Columns.Item(3).Cells.Item(i).Specific).Value;
                        lObjStockTrasnfer.ToWarehouse = ((dynamic)lLstExtraData[i - 1]).Whscode;
                        string lStrCardCode = ((SAPbouiCOM.EditText)Matrix1.Columns.Item("Col_6").Cells.Item(i).Specific).Value;
                        string lStrAlmacen = ((SAPbouiCOM.EditText)Matrix1.Columns.Item("Col_3").Cells.Item(i).Specific).Value;
                        string lStrCantidad = ((SAPbouiCOM.EditText)Matrix1.Columns.Item("Col_4").Cells.Item(i).Specific).Value;
                        string lStrTrans = ((SAPbouiCOM.EditText)Matrix1.Columns.Item("Col_5").Cells.Item(i).Specific).Value;
                        string lStrItemCode = ((SAPbouiCOM.EditText)Matrix1.Columns.Item("Col_7").Cells.Item(i).Specific).Value;
                        var lObjData = new
                        {
                            CardCode = lStrCardCode,
                            Almacen = lStrAlmacen,
                            Cantidad = lStrCantidad,
                            Trans = lStrTrans,
                            ItemCode = lStrItemCode
                        };
                        if (lStrTrans.Trim() != "")
                        {
                            lObjStockTrasnfer = AddLines(lObjStockTrasnfer, lObjInventory, lObjData);
                            lBolIsSuccesss = true;
                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Capturar cantidad", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
                if (lBolIsSuccesss)
                {
                    lObjStockTrasnfer.DocDate = DateTime.Now;
                    
                    if (lObjStockTrasnfer.Add() != 0)
                    {
                        int error = oCompany.GetLastErrorCode();
                        string strError = oCompany.GetLastErrorDescription();
                        Application.SBO_Application.MessageBox(strError);
                    }
                    else
                    {
                        EditText1.Value = "";
                        LoadMatrixPeticiones();
                        Application.SBO_Application.StatusBar.SetText("Traslado correctamente efectuado.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("No se ha seleccionado ningun almacen para el traslado.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private void createTransfer()
        {
            try
            {
                SAPbobsCOM.StockTransfer lObjStockTrasnfer;
                lObjStockTrasnfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

                
                lObjStockTrasnfer.Lines.ItemCode = "A00000440";    
                lObjStockTrasnfer.FromWarehouse = "CRHEC108";
                lObjStockTrasnfer.Lines.WarehouseCode = "SUHE";
                lObjStockTrasnfer.Lines.Quantity = 1;

                string xdd = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest.ToString();
                lObjStockTrasnfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                lObjStockTrasnfer.Lines.BaseEntry = 46;
                lObjStockTrasnfer.Lines.BaseLine = 0;
                lObjStockTrasnfer.Lines.BatchNumbers.BatchNumber = "aaaaa123";
                lObjStockTrasnfer.Lines.BatchNumbers.Quantity = 1;
                lObjStockTrasnfer.Lines.BatchNumbers.Add();

                int x = lObjStockTrasnfer.Add();

                if (x != 0)
                {
                    string y = oCompany.GetLastErrorDescription();
                }
            }catch(Exception er)
            {
                
            }



        }

        private SAPbobsCOM.StockTransfer AddLines(SAPbobsCOM.StockTransfer pObjStockTrasnfer, SAPbobsCOM.StockTransfer pObjRequest, object pObjData)
        {
            float lFltnum = float.Parse(((dynamic)pObjData).Trans);
            int lIntQuanity = int.Parse(lFltnum.ToString());

            try
            {
                var lObjLines = lLstExtraData.FindAll(x => ((dynamic)x).FromWhscode == ((dynamic)pObjData).Almacen && ((dynamic)x).ItemCode == ((dynamic)pObjData).ItemCode && ((dynamic)x).CardCode == ((dynamic)pObjData).CardCode);
                List<object> lLstObjTemp = new List<object>();
                foreach (var lObjLine in lObjLines)
                {
                    List<object> lLstrLotes = GetListBatch(pObjStockTrasnfer.FromWarehouse, ((dynamic)lObjLine).CardCode, ((dynamic)pObjData).ItemCode, lLstObjTemp);

                    if (lIntQuanity > 0)
                    {
                        pObjStockTrasnfer.Lines.ItemCode = ((dynamic)pObjData).ItemCode;
                        //pObjStockTrasnfer.Lines.InventoryQuantity = double.Parse(((dynamic)pObjData).Cantidad);
                        if (int.Parse(((dynamic)lObjLine).Quantity) < lIntQuanity)
                            pObjStockTrasnfer.Lines.Quantity = int.Parse(((dynamic)lObjLine).Quantity);
                        else
                            pObjStockTrasnfer.Lines.Quantity = lIntQuanity;


                        pObjStockTrasnfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                        pObjStockTrasnfer.Lines.BaseEntry = pObjRequest.DocEntry;
                        pObjStockTrasnfer.Lines.BaseLine = int.Parse(((dynamic)lObjLine).LineNum);
                        pObjStockTrasnfer.Lines.FromWarehouseCode = ((dynamic)pObjData).Almacen;
                        pObjStockTrasnfer.Lines.WarehouseCode = ((dynamic)lObjLine).Whscode;
                        int lBatchQty = int.Parse(pObjStockTrasnfer.Lines.Quantity.ToString());
                        while (lBatchQty > 0)
                        {
                            if (((dynamic)lObjLine).Lote != "")
                                lLstrLotes = lLstrLotes.OrderByDescending(x => ((dynamic)x).Batch == ((dynamic)lObjLine).Lote).ToList();

                            foreach (var lotes in lLstrLotes)
                            {
                                if (lBatchQty > 0)
                                {
                                    var lObjUsedBatch = lLstObjTemp.Find(x => ((dynamic)x).Batch == ((dynamic)lotes).Batch);
                                    int lIntBatchQty = ((dynamic)lotes).Quantity;
                                    if (lObjUsedBatch == null || ((dynamic)lObjUsedBatch).Quantity < lIntBatchQty)
                                    {
                                        pObjStockTrasnfer.Lines.BatchNumbers.BatchNumber = ((dynamic)lotes).Batch;
                                        int lIntQty = 0;
                                        if (lObjUsedBatch != null)
                                            lIntQty = lIntBatchQty - ((dynamic)lObjUsedBatch).Quantity;
                                        else
                                        {
                                            if (((dynamic)lotes).Quantity < lBatchQty)
                                                lIntQty = lIntBatchQty;
                                            else
                                                lIntQty = lBatchQty;
                                        }
                                        pObjStockTrasnfer.Lines.BatchNumbers.Quantity = lIntQty;
                                        pObjStockTrasnfer.Lines.BatchNumbers.Add();
                                        lLstObjTemp.Add(new
                                        {
                                            Batch = ((dynamic)lotes).Batch,
                                            Quantity = lIntQty
                                        });
                                        lBatchQty -= lIntQty;
                                    }
                                }
                            }
                            if (lBatchQty > 0)
                                throw new Exception("No se tiene la cantidad de articulos suficiente dentro de los lotes para hacer el traslado");
                        }
                        pObjStockTrasnfer.Lines.Add();
                        lIntQuanity -= int.Parse(((dynamic)lObjLine).Quantity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pObjStockTrasnfer;
        }


        private List<object> GetListBatch(string pStrAlmacen, string oStrCardCode, string pStrItemCode, List<object> pLstUsedBatch)
        {
            SAPbobsCOM.Recordset lObjRecordSet3 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet3.DoQuery("SELECT T0.DistNumber AS BatchNumber, T1.Quantity FROM OBTN AS T0 INNER JOIN dbo.OBTQ AS T1 ON T0.ItemCode = T1.ItemCode AND T0.SysNumber = T1.SysNumber where T1.WhsCode = '" + pStrAlmacen + "' and T1.Quantity>0 and MnfSerial is not null and T0.MnfSerial = '" + oStrCardCode + "' and T0.ItemCode = '" + pStrItemCode + "' order by T0.ExpDate asc");

            List<object> lLstrLotes = new List<object>();
            while (!lObjRecordSet3.EoF)
            {

                var lObjUsedBatch = pLstUsedBatch.Find(x => ((dynamic)x).Batch == lObjRecordSet3.Fields.Item(0).Value.ToString());

                if (lObjUsedBatch == null || ((dynamic)lObjUsedBatch).Quantity < int.Parse(lObjRecordSet3.Fields.Item(1).Value.ToString()))
                {
                    lLstrLotes.Add(new
                    {
                        Batch = lObjRecordSet3.Fields.Item(0).Value.ToString(),
                        Quantity = int.Parse(lObjRecordSet3.Fields.Item(1).Value.ToString())
                    });
                }

                lObjRecordSet3.MoveNext();
            }

            Memory.ReleaseComObject(lObjRecordSet3);

            return lLstrLotes;
        }

        private void Matrix1_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 13 || (pVal.CharPressed > 36 && pVal.CharPressed < 41))
            {

                double lDblCantidad = double.Parse(((SAPbouiCOM.EditText)Matrix1.GetCellSpecific(5, pVal.Row)).Value);
                if (((SAPbouiCOM.EditText)Matrix1.GetCellSpecific(5, pVal.Row)).Value != "")
                {
                    double lDblCant = 0;
                    double.TryParse(((SAPbouiCOM.EditText)Matrix1.GetCellSpecific(6, pVal.Row)).Value, out lDblCant);
                    if (!(lDblCant > 0 && lDblCant <= lDblCantidad))
                        Application.SBO_Application.MessageBox("Favor de capturar un valor valido dentro de los articulos a trasferir");
                }

            }
        }

        private void EditText1_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 13)
                LoadMatrixPeticiones();
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            Matrix1.AutoResizeColumns();
        }

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            UnloadFormEvents();

        }
    }
}