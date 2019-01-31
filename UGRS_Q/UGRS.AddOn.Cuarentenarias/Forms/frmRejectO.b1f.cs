using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.Core.Utility;
using SAPbobsCOM;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.Cuarentenarias.DAO;
using UGRS.AddOn.Cuarentenarias.Enums;
using System.Text.RegularExpressions;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.UI.ProgressBar;
using System.Threading.Tasks;
using System.Threading;

namespace UGRS.AddOn.Cuarentenarias.Forms
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.Forms.frmRejectionOutflows", "Forms/frmRejectO.b1f")]
    class frmRejectO : UserFormBase
    {
        #region Items
        private SAPbouiCOM.CheckBox lObjChckBox;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.Matrix lObjMtxRejectOut;
        private SAPbouiCOM.Button lObjBtnSearch;
        private SAPbouiCOM.Button lObjBtnExit;
        private SAPbouiCOM.Button lObjBtnInvoice;
        private SAPbouiCOM.EditText lObjTxtClient;
        private SAPbobsCOM.Documents lObjDraftInvoice = null;
        #endregion

        //SAPbobsCOM.Documents lObjInvoice = null;
        SAPbobsCOM.Documents lObjGoodsIssues = null;
        private ProgressBarManager mObjProgressBar = null;
        SAPbouiCOM.EditText lObjEdTxtDocDate = null;
        private SAPbouiCOM.EditText lObjSpDate;
        private SAPbouiCOM.StaticText lObjLbl;

        Menu pObjMenu = new Menu();


        #region variables
        string lStrUserName = DIApplication.Company.UserName;//get user name
        int lIntUserSign = DIApplication.Company.UserSignature;//get user sign
        string lStrMainRWhs;
        string lStrMainWhs;
        string lStrOption = "";
        string lStrReference = "";

        DateTime lObjDatetime;
        string[] scs = { "yyyyMMdd" };


        #endregion

        RejectedDAO mObjRejectedDAO = new RejectedDAO();
        List<DTO.RejectedToInvoiceDTO> lListRejectedToInvoice = new List<DTO.RejectedToInvoiceDTO>();
        List<DTO.ErrorListDTO> lObjlstErrorDTO = new List<DTO.ErrorListDTO>();
        DTO.ErrorListDTO lObjErrorListDTO = new DTO.ErrorListDTO();

        public frmRejectO()
        {
            LoadEvents();
            lStrMainRWhs = mObjRejectedDAO.GetMainRWhs(lIntUserSign);
            lStrMainWhs = mObjRejectedDAO.GetMainWhs(lIntUserSign);
            LoadMatrix(lStrMainRWhs);
            lObjMtxRejectOut.AutoResizeColumns();
            AddChooseFromList();
            initCheckBox();
            ConfigForm();
            //SetSpecialDate(); --------------------------- Extra** if validation is needed


        }

        private void SetSpecialDate()
        {
            DateTime dt = DateTime.Today;

            DayOfWeek CurrentDay = dt.DayOfWeek;

            string lStrDay = CurrentDay.ToString();

            if (lStrDay != "Friday")
            {
                lObjEdTxtDocDate.Item.Enabled = false;
            }
        }

        private void initCheckBox()
        {
            this.UIAPIRawForm.DataSources.UserDataSources.Add("UDCL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);

            lObjChckBox.DataBind.SetBound(true, "", "UDCL");

        }
        private void ConfigForm()
        {
            this.UIAPIRawForm.Left = 400;
            this.UIAPIRawForm.Top = 100;

            lObjEdTxtDocDate.Value = mObjRejectedDAO.GetServerDate();////////////
        }

        #region Load & Unload Events
        private void LoadEvents()
        {
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }
        private void UnLoadEvents()
        {
            Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(chooseFromListAfterEvent);
        }
        #endregion

        private void initProgressBar(int lIntSize)
        {
            mObjProgressBar = new ProgressBarManager(Application.SBO_Application, "En proceso, Espere...", lIntSize);
        }
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
                                if (pVal.ItemUID.Equals("btnSearch"))
                                {
                                    if (lObjTxtClient.Value != string.Empty)
                                    {
                                        FilterData(lStrMainRWhs, lObjTxtClient.Value);
                                    }
                                    else
                                    {
                                        LoadMatrix(lStrMainRWhs);
                                    }
                                }
                                if (pVal.ItemUID.Equals("btnOutput"))
                                {
                                    ListRejects("Output");
                                }
                                if (pVal.ItemUID.Equals("btnInvo"))
                                {
                                    ListRejects("Invoice");
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnLoadEvents();
                                pObjMenu.pArrTypeCount[3] = 0;
                                pObjMenu.pArrTypeEx[3] = "";
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

        private void FilterData(string lStrWhsCode, string lStrCardCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO")
                    .ExecuteQuery(mObjRejectedDAO.GetFilteredRejected(lStrWhsCode, lStrCardCode));

                lObjMtxRejectOut.Columns.Item("Check").DataBind.Bind("DtRejO", "Chk");
                lObjMtxRejectOut.Columns.Item("Insp").DataBind.Bind("DtRejO", "LotNumber");
                lObjMtxRejectOut.Columns.Item("Client").DataBind.Bind("DtRejO", "CardName");
                lObjMtxRejectOut.Columns.Item("HeadType").DataBind.Bind("DtRejO", "ItemName");
                lObjMtxRejectOut.Columns.Item("InspD").DataBind.Bind("DtRejO", "InDate");
                lObjMtxRejectOut.Columns.Item("Stock").DataBind.Bind("DtRejO", "Cantidad");
                lObjMtxRejectOut.Columns.Item("Quantity").DataBind.Bind("DtRejO", "Cantidad");

                lObjMtxRejectOut.LoadFromDataSource();

            }
            catch (Exception er)
            {

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        private void chooseFromListAfterEvent(string FormUID, ref SAPbouiCOM.ItemEvent pValEvent, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pValEvent.FormType == 0 && pValEvent.Action_Success == true && pValEvent.Before_Action == false
                    && pValEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
            {


                SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pValEvent;

                string sCFL_ID = null;
                sCFL_ID = oCFLEvento.ChooseFromListUID;

                SAPbouiCOM.ChooseFromList oCFL = null;
                oCFL = this.UIAPIRawForm.ChooseFromLists.Item(sCFL_ID);

                SAPbouiCOM.DataTable oDataTable = null;
                oDataTable = oCFLEvento.SelectedObjects;

                string val = null;
                try
                {
                    val = System.Convert.ToString(oDataTable.GetValue(0, 0));
                    lObjTxtClient.Value = val;
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Form - Bad Value") && !ex.Message.Contains("Can't set value on item because the item can't get focus."))
                    {
                        Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    }
                }
                finally
                {

                }


            }


        }

        private void ListRejects(string lStrOpt)
        {

            lListRejectedToInvoice = new List<DTO.RejectedToInvoiceDTO>();
            lObjlstErrorDTO = new List<DTO.ErrorListDTO>();
            for (int i = 1; i <= lObjMtxRejectOut.RowCount; i++)
            {
                if (((dynamic)lObjMtxRejectOut.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {
                    switch (lStrOpt)
                    {
                        case "Output":
                            List(i, lStrOpt);
                            break;
                        case "Invoice":
                            List(i, lStrOpt);
                            break;
                    }
                }
            }
            if (lListRejectedToInvoice.Count > 0)
            {

                try
                {
                    initProgressBar(lListRejectedToInvoice.Count);
                    this.UIAPIRawForm.Freeze(true);

                    for (int i = 0; i < lListRejectedToInvoice.Count; i++)
                    {


                        if (!CheckDate(i))
                        {
                            return;
                        }
                    }

                    switch (lListRejectedToInvoice[0].Option)
                    {
                        case "Output":

                            Outputs();
                            break;

                        case "Invoice":
                            InvoiceRDraft();
                            break;

                    }
                    if (lObjTxtClient.Value != string.Empty)
                    {
                        FilterData(lStrMainRWhs, lObjTxtClient.Value);
                    }
                    else
                    {
                        LoadMatrix(lStrMainRWhs);
                    }
                }
                catch (Exception ex)
                {
                    Application.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                finally
                {
                    mObjProgressBar.Stop();
                    this.UIAPIRawForm.Freeze(false);
                    mObjProgressBar.Dispose();
                    ShowErrors();
                }



            }

        }

        private bool CheckDate(int lIntRow)
        {
            DateTime.TryParseExact(lObjEdTxtDocDate.Value, scs, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                   , out lObjDatetime);
            if (lObjDatetime <= DateTime.Today)
            {
                if (lObjDatetime >= lListRejectedToInvoice[lIntRow].InspDate)
                {
                    return true;
                }
                else
                {

                    ////                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea mayor a la de inspección"
                    ////, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    lObjErrorListDTO.Option = "DateBeyond";
                    lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntRow].Inspection;
                    lObjlstErrorDTO.Add(lObjErrorListDTO);

                    return false;
                }
            }
            else
            {
                ////                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea menor al día de hoy"
                ////, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                lObjErrorListDTO.Option = "DateBefore";
                lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntRow].Inspection;
                lObjlstErrorDTO.Add(lObjErrorListDTO);

                return false;
            }
        }

        private void ShowErrors()
        {
            string lStrOntime = "";
            string lStrNeedInvoice = "";
            string lStrHasInvoice = "";
            string lStrCorrectOutput = "";
            string lStrCorrect = "";
            string lStrHasDraft = "";
            string lStrOutOfTime = "";
            string lStrDate = "";
            string lStrDateBeyond = "";
            string lStrDateBefore = "";
            for (int i = 0; i < lObjlstErrorDTO.Count; i++)
            {
                switch (lObjlstErrorDTO[i].Option)
                {
                    case "OnTime":
                        lStrOntime += "Id:" + lObjlstErrorDTO[i].Inspection + " ";

                        break;

                    case "NeedInvoice":
                        lStrNeedInvoice += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;

                    case "WithInvoice":
                        lStrHasInvoice += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "CorrectOutput":
                        lStrCorrectOutput += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;

                    case "Correct":
                        lStrCorrect += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "HasDraft":
                        lStrHasDraft += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "OutOfTime":
                        lStrOutOfTime += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "InvalidDate":
                        lStrDate += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "DateBeyond":
                        lStrDateBeyond += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;
                    case "DateBefore":
                        lStrDateBefore += "Id:" + lObjlstErrorDTO[i].Inspection + " ";
                        break;

                }

            }

            if (lStrOntime != "")
            {
                Application.SBO_Application.StatusBar.SetText("Esta en tiempo. Puede dar salidas en inspección " + lStrOntime
           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                Task.Delay(10);
            }
            if (lStrHasInvoice != "")
            {
                Application.SBO_Application.StatusBar.SetText("Ya se cuenta con una factura en inspección " + lStrHasInvoice
                    + ", puede dar salida", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                Task.Delay(10);
            }
            if (lStrNeedInvoice != "")
            {
                Application.SBO_Application.StatusBar.SetText("Se excedio el tiempo. Favor de facturar en inspección " + lStrNeedInvoice
    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                Task.Delay(10);
            }
            if (lStrCorrectOutput != "")
            {
                Application.SBO_Application.StatusBar.SetText("Salidas realizadas correctamente en " + lStrCorrectOutput
    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Task.Delay(10);
            }
            if (lStrCorrect != "")
            {
                Application.SBO_Application.StatusBar.SetText("Preliminar realizada correctamente en" + lStrCorrect
, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Task.Delay(10);
            }
            if (lStrHasDraft != "")
            {
                Application.SBO_Application.StatusBar.SetText("Este lote ya cuenta con una preliminar en " + lStrHasDraft
, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                Task.Delay(10);
            }
            if (lStrOutOfTime != "")
            {
                Application.SBO_Application.StatusBar.SetText("Se excedio el tiempo. Favor de facturar en inspección " + lStrOutOfTime
          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            if (lStrDate != "")
            {
                Application.SBO_Application.StatusBar.SetText("Fecha Invalida "
          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            if (lStrDateBeyond != "")
            {
                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea mayor a la de inspección"
 , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            if (lStrDateBefore != "")
            {
                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea menor al día de hoy"
, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }


        }

        private double SetCosts(string lStrInspection, int lItnRowlist)
        {
            string lStrArticletoCost = mObjRejectedDAO.GetArticleToInvoice();
            TimeSpan lTimeSpanCharges = new TimeSpan();
            double lDbPrice = mObjRejectedDAO.GetPrice(lStrArticletoCost, mObjRejectedDAO.GetWhs(lIntUserSign));
            double lDbHeads = lListRejectedToInvoice[lItnRowlist].Stock;
            int lIntDays = 0;
            DateTime.TryParseExact(lObjEdTxtDocDate.Value, scs, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                   , out lObjDatetime);

            string lStrDt = Convert.ToDateTime(lObjDatetime).ToString("yyyy-MM-dd");

            if (lObjEdTxtDocDate.Value != Convert.ToDateTime(lObjDatetime).ToString("yyyyMMdd"))
            {

                if (lListRejectedToInvoice[lItnRowlist].LastInvoice.Year >= DateTime.Now.Year)
                {
                    lTimeSpanCharges = Convert.ToDateTime(lStrDt) - lListRejectedToInvoice[lItnRowlist].LastInvoice;
                    lIntDays = lTimeSpanCharges.Days;
                }
                else
                {
                    lTimeSpanCharges = Convert.ToDateTime(lStrDt) - lListRejectedToInvoice[lItnRowlist].InspDate;
                    lIntDays = lTimeSpanCharges.Days - 1;
                }


            }
            else
            {
                if (lListRejectedToInvoice[lItnRowlist].LastInvoice.Year >= DateTime.Now.Year)
                {
                    lTimeSpanCharges = Convert.ToDateTime(lStrDt) - lListRejectedToInvoice[lItnRowlist].LastInvoice;
                    lIntDays = lTimeSpanCharges.Days;
                }
                else
                {
                    lTimeSpanCharges = Convert.ToDateTime(lStrDt) - lListRejectedToInvoice[lItnRowlist].InspDate;
                    lIntDays = lTimeSpanCharges.Days - 1;
                }
            }

            lListRejectedToInvoice[lItnRowlist].QuantityConcept = (lIntDays * lDbHeads);

            return lListRejectedToInvoice[lItnRowlist].Price = (lDbPrice * lDbHeads) * lIntDays;


        }

        private void Listing(int row, string opt)
        {
            if (CheckThreeDays(row))
            {
                List(row, opt);

            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Se excedio el tiempo. Favor de facturar"
       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private bool CheckThreeDays(int lIntRow)
        {
            TimeSpan lTimeSpan = new TimeSpan();
            int lIntDays = 0;

            DateTime.TryParseExact(lObjEdTxtDocDate.Value, scs, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                    , out lObjDatetime);
            lTimeSpan = lObjDatetime - lListRejectedToInvoice[lIntRow].InspDate;
            if (Convert.ToInt32(mObjRejectedDAO.GetDefaultDays()) > 0)
            {
                lIntDays = Convert.ToInt32(mObjRejectedDAO.GetDefaultDays());

                if ((lTimeSpan.Days + 1) < lIntDays)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Actualizar el campo de dias permitidos en CU Rechazo"
, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                return false;
            }
            //                }
            //                else
            //                {
            //                    Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea mayor a la de inspección"
            //    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            //                    return false;
            //                }
            //            }
            //            else
            //            {
            //                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea menor al día de hoy"
            //, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            //                return false;
            //            }
        }

        private void LoadMatrix(string lStrWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO")
                    .ExecuteQuery(mObjRejectedDAO.GetRejectedQuery(lStrWhsCode));

                lObjMtxRejectOut.Columns.Item("Check").DataBind.Bind("DtRejO", "Chk");
                lObjMtxRejectOut.Columns.Item("Insp").DataBind.Bind("DtRejO", "LotNumber");
                lObjMtxRejectOut.Columns.Item("Client").DataBind.Bind("DtRejO", "CardName");
                lObjMtxRejectOut.Columns.Item("HeadType").DataBind.Bind("DtRejO", "ItemName");
                lObjMtxRejectOut.Columns.Item("InspD").DataBind.Bind("DtRejO", "InDate");
                lObjMtxRejectOut.Columns.Item("Stock").DataBind.Bind("DtRejO", "Cantidad");
                lObjMtxRejectOut.Columns.Item("Quantity").DataBind.Bind("DtRejO", "Cantidad");
                lObjMtxRejectOut.Columns.Item("DateInv").DataBind.Bind("DtRejO", "Fecha");

                lObjMtxRejectOut.LoadFromDataSource();

            }
            catch (Exception er)
            {

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        private void List(int lIntRow, string lStrOpt)
        {
            DTO.RejectedToInvoiceDTO lObjRejectedToInvoiceDTO = new DTO.RejectedToInvoiceDTO();

            lObjRejectedToInvoiceDTO.Inspection = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("LotNumber", lIntRow - 1);
            lObjRejectedToInvoiceDTO.Client = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("CardCode", lIntRow - 1);
            lObjRejectedToInvoiceDTO.HeadType = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("ItemCode", lIntRow - 1);
            lObjRejectedToInvoiceDTO.InspDate = (DateTime)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("InDate", lIntRow - 1);
            lObjRejectedToInvoiceDTO.Stock = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("Cantidad", lIntRow - 1);
            lObjRejectedToInvoiceDTO.DistNumb = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DtRejO").GetValue("DistNumber", lIntRow - 1);
            if (lStrOpt.Equals("Output"))
            {
                var lVarCantidad = (Convert.ToString(((SAPbouiCOM.EditText)lObjMtxRejectOut.Columns.Item(6).Cells.Item(lIntRow).Specific).Value));
                if (Regex.IsMatch(lVarCantidad, "^-?\\d*(\\.\\d+)?$"))
                {
                    if (lObjRejectedToInvoiceDTO.Stock >= Convert.ToDouble(lVarCantidad) && Convert.ToDouble(lVarCantidad) > 0)
                    {

                        lObjRejectedToInvoiceDTO.Quantity = Math.Truncate(Convert.ToDouble(lVarCantidad));

                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("Agregar una cantidad posible"
             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        return;
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Solo números"
     , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }
            }
            lObjRejectedToInvoiceDTO.Reference = lStrMainRWhs + "_" + lObjRejectedToInvoiceDTO.Inspection;
            lObjRejectedToInvoiceDTO.Option = lStrOpt;

            lListRejectedToInvoice.Add(lObjRejectedToInvoiceDTO);

        }

        private void Outputs()
        {
            lObjErrorListDTO = new DTO.ErrorListDTO();
            int lIntRetCode = 0;
            int lIntSeries = mObjRejectedDAO.GetSerieForOutputs(lIntUserSign);
            bool lBoolValid = false;

            lObjGoodsIssues = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
            try
            {

                for (int i = 0; i < lListRejectedToInvoice.Count; i++)
                {
                    lObjErrorListDTO = new DTO.ErrorListDTO();
                    lStrReference = lStrMainRWhs + "_" + lListRejectedToInvoice[i].Inspection;

                    if (CheckThreeDays(i) || HasInvoice(lStrReference, lListRejectedToInvoice[i].DistNumb))
                    {

                        lObjGoodsIssues.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                        lObjGoodsIssues.Series = lIntSeries;
                        lObjGoodsIssues.UserFields.Fields.Item("U_MQ_OrigenFol").Value = lListRejectedToInvoice[i].Inspection;
                        lObjGoodsIssues.UserFields.Fields.Item("U_GLO_InMo").Value = "S-GAN";
                        lObjGoodsIssues.DocDate = DateTime.Today;
                        lObjGoodsIssues.DocDueDate = DateTime.Today;


                        lObjGoodsIssues.Lines.ItemCode = lListRejectedToInvoice[i].HeadType;
                        lObjGoodsIssues.Lines.WarehouseCode = lStrMainRWhs;
                        lObjGoodsIssues.Lines.Quantity = lListRejectedToInvoice[i].Quantity;

                        lObjGoodsIssues.Lines.BatchNumbers.BatchNumber = lListRejectedToInvoice[i].DistNumb;
                        lObjGoodsIssues.Lines.BatchNumbers.Quantity = lListRejectedToInvoice[i].Quantity;

                        lObjGoodsIssues.Lines.BatchNumbers.Add();

                        lObjGoodsIssues.Lines.Add();

                        lIntRetCode = lObjGoodsIssues.Add();

                        if (lIntRetCode != 0)
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



                        if (lBoolValid == true)
                        {
                            lObjErrorListDTO.Option = "CorrectOutput";
                            lObjErrorListDTO.Inspection = lListRejectedToInvoice[i].Inspection;
                            lObjlstErrorDTO.Add(lObjErrorListDTO);

                        }

                    }
                    else
                    {

                        lObjErrorListDTO.Option = "OutOfTime";
                        lObjErrorListDTO.Inspection = lListRejectedToInvoice[i].Inspection.ToString();

                        lObjlstErrorDTO.Add(lObjErrorListDTO);

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

        }

        private void InvoiceRDraft()
        {
            SAPbouiCOM.Form lObjFormDraft = null;

            int lIntRetCode = 0;
            int lIntKeyD = 0;
            int lIntCounter = 0;
            int lIntPrevKey = 0;
            bool lBoolDraft = false;

            string lStrArticle = mObjRejectedDAO.GetArticleToInvoice();

            lObjDraftInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);



            var lVarRejectedToInv = from p in lListRejectedToInvoice group p by p.Client into gr select gr;
            try
            {
                foreach (var lVarClient in lVarRejectedToInv)
                {
                    foreach (var item in lVarClient)
                    {
                        lObjErrorListDTO = new DTO.ErrorListDTO();
                        lStrReference = lStrMainRWhs + "_" + item.Inspection;

                        if (!CheckThreeDays(lIntCounter))
                        {
                            if (!HasInvoice(lStrReference, item.DistNumb))
                            {

                                if (SetCosts(item.Inspection, lIntCounter) != -1)
                                {

                                    lIntKeyD = mObjRejectedDAO.GetDraftKey(lStrReference, lStrArticle);

                                    if (lIntKeyD > 0 && lIntKeyD != lIntPrevKey)
                                    {
                                        lObjDraftInvoice.GetByKey(lIntKeyD);

                                        lObjDraftInvoice.Remove();
                                        lIntKeyD = 0;
                                    }
                                    if (lIntKeyD == 0)
                                    {

                                        lObjDraftInvoice.DocObjectCodeEx = "13";
                                        lObjDraftInvoice.CardCode = item.Client;
                                        lObjDraftInvoice.Lines.FreeText = lStrReference;
                                        lObjDraftInvoice.Lines.ItemCode = lStrArticle;
                                        lObjDraftInvoice.Lines.TaxCode = mObjRejectedDAO.GetTaxCode(lStrArticle);
                                        lObjDraftInvoice.Lines.Quantity = item.QuantityConcept;
                                        lObjDraftInvoice.Lines.LineTotal = item.Price;

                                        lObjDraftInvoice.Lines.Add();

                                        lBoolDraft = true;

                                        lObjErrorListDTO.Option = "Correct";
                                        lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntCounter].Inspection;
                                        lObjlstErrorDTO.Add(lObjErrorListDTO);
                                    }
                                    //else
                                    //{
                                    //    lObjErrorListDTO.Option = "HasDraft";
                                    //    lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntCounter].Inspection;
                                    //    lObjlstErrorDTO.Add(lObjErrorListDTO);

                                    //    if (lObjChckBox.Checked)
                                    //    {

                                    //        lIntKeyD = mObjRejectedDAO.GetDraftKey(lStrReference, lStrArticle);

                                    //        if (lIntKeyD > 0 && lIntKeyD != lIntPrevKey)
                                    //        {
                                    //            lObjDraftInvoice.GetByKey(lIntKeyD);
                                    //            if (lObjDraftInvoice.DocDate < lObjDatetime)
                                    //            {
                                    //                lObjDraftInvoice.Remove();
                                    //            }
                                    //            else
                                    //            {

                                    //                lObjFormDraft = Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lIntKeyD.ToString());
                                    //                lIntPrevKey = lIntKeyD;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    lObjErrorListDTO.Option = "InvalidDate";
                                    lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntCounter].Inspection.ToString();

                                    lObjlstErrorDTO.Add(lObjErrorListDTO);
                                }



                            }
                            else
                            {
                                lObjErrorListDTO.Option = "WithInvoice";
                                lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntCounter].Inspection.ToString();

                                lObjlstErrorDTO.Add(lObjErrorListDTO);
                            }
                        }
                        else
                        {

                            lObjErrorListDTO.Option = "OnTime";
                            lObjErrorListDTO.Inspection = lListRejectedToInvoice[lIntCounter].Inspection.ToString();

                            lObjlstErrorDTO.Add(lObjErrorListDTO);
                        }

                        lIntCounter++;
                    }

                    if (lBoolDraft)
                    {
                        lIntRetCode = lObjDraftInvoice.Add();

                        if (lIntRetCode != 0)
                        {
                            string lStrError = DIApplication.Company.GetLastErrorDescription();
                            Application.SBO_Application.StatusBar.SetText(lStrError
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                        }

                        lIntKeyD = mObjRejectedDAO.GetDraftKey(lStrReference, lStrArticle);

                        if (lObjChckBox.Checked)
                        {
                            if (lIntKeyD > 0 && lIntKeyD != lIntPrevKey)
                            {
                                lObjFormDraft = Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lIntKeyD.ToString());
                                lIntPrevKey = lIntKeyD;
                            }
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
                MemoryUtility.ReleaseComObject(lObjDraftInvoice);
            }

        }

        private void AddChooseFromList()
        {
            this.UIAPIRawForm.DataSources.UserDataSources.Add("UDCFL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);

            try
            {
                SAPbouiCOM.EditText lObjEditTxt = null;
                SAPbouiCOM.ChooseFromList lObjCFL = null;

                SAPbouiCOM.ChooseFromListCollection lLstCFLs = null;
                lLstCFLs = this.UIAPIRawForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromListCreationParams lObjCFLCreationParams = null;
                lObjCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);


                lObjCFLCreationParams.MultiSelection = false;
                lObjCFLCreationParams.ObjectType = "2";
                lObjCFLCreationParams.UniqueID = "CFLBP";
                lObjCFL = lLstCFLs.Add(lObjCFLCreationParams);

                //lObjEditTxt = lObjTxtClient;
                lObjTxtClient.DataBind.SetBound(true, "", "UDCFL");
                lObjTxtClient.ChooseFromListUID = lObjCFL.UniqueID;
                lObjTxtClient.ChooseFromListAlias = "CardCode";


            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("InitCustomerChooseFromListException: {0}", ex.Message));
            }




        }

        #region conditions
        private bool HasInvoice(string lStrReference, string lStrDistNumb)
        {
            string lStrDocDate = mObjRejectedDAO.SearchExistentInvoices(lStrReference);
            bool lBoolInvoices = false;
            if (lStrDocDate != string.Empty)
            {
                lBoolInvoices = CheckInvoiceDays(lStrDocDate, lStrDistNumb);
            }
            else
            {
                lBoolInvoices = false;
            }

            return lBoolInvoices;
        }

        private bool CheckInvoiceDays(string lStrDocDate, string lStrDistNumb)
        {
            //TimeSpan lTspDateDiff = (DateTime.Today - Convert.ToDateTime(lStrDocDate));
            DateTime.TryParseExact(lObjEdTxtDocDate.Value, scs, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                    , out lObjDatetime);

            //if (lObjDatetime <= DateTime.Today)
            //{
            //    DateTime lDatetimeInspDate = Convert.ToDateTime(lListRejectedToInvoice.Where(x => x.DistNumb == lStrDistNumb).Select(y => y.InspDate).ToList());
            //    if (lObjDatetime >= lDatetimeInspDate)
            //    {
            TimeSpan lTspDateDiff = (lObjDatetime - Convert.ToDateTime(lStrDocDate));
            lListRejectedToInvoice.Where(x => x.DistNumb == lStrDistNumb).Select(y => y.LastInvoice = Convert.ToDateTime(lStrDocDate)).ToList();
            if (lTspDateDiff.Days == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            //                }
            //                else
            //                {
            //                    Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea mayor a la de inspección"
            //    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            //                    return false;
            //                }
            //            }
            //            else
            //            {
            //                Application.SBO_Application.StatusBar.SetText("Verifique que la fecha sea menor al día de hoy"
            //, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            //                return false;
            //            }
        }

        private bool SameClient()
        {
            //Validate Same client to make an invoid
            if (lListRejectedToInvoice.Count > 1)
            {
                var quer = lListRejectedToInvoice.GroupBy(x => x.Client).Where(x => x.Count() > 1).SelectMany(g => g);
                if (quer.Count() == lListRejectedToInvoice.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }


        #endregion

        #region Default
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lObjTxtClient = ((SAPbouiCOM.EditText)(this.GetItem("txtClient").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.lObjMtxRejectOut = ((SAPbouiCOM.Matrix)(this.GetItem("mtxReject").Specific));
            this.lObjBtnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.lObjBtnExit = ((SAPbouiCOM.Button)(this.GetItem("btnOutput").Specific));
            this.lObjBtnInvoice = ((SAPbouiCOM.Button)(this.GetItem("btnInvo").Specific));
            // this.lObjChckBox = ((SAPbouiCOM.CheckBox)(this.GetItem("ChckP").Specific));
            this.lObjChckBox = ((SAPbouiCOM.CheckBox)(this.UIAPIRawForm.Items.Item("ChckP").Specific));
            this.lObjEdTxtDocDate = ((SAPbouiCOM.EditText)(this.GetItem("txtSpD").Specific));
            this.lObjLbl = ((SAPbouiCOM.StaticText)(this.GetItem("lblSpd").Specific));
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






    }
}
