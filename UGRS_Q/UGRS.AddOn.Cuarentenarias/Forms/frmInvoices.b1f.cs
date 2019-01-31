using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.AddOn.Cuarentenarias.Models;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.AddOn.Cuarentenarias.DTO;
using UGRS.AddOn.Cuarentenarias.Enums;
using UGRS.Core.Extension.Enum;
using System.Text.RegularExpressions;
using UGRS.AddOn.Cuarentenarias.Services;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.AddOn.Cuarentenarias.DAO;
using UGRS.Core.Exceptions;
using System.Xml.Linq;

namespace UGRS.AddOn.Cuarentenarias.Forms
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.Forms.frmInvoices", "Forms/frmInvoices.b1f")]
    class frmInvoices : UserFormBase
    {
        #region Items
        private SAPbouiCOM.Button lObjButtonSearch;
        private SAPbouiCOM.Button lObjButtonAccept;
        private SAPbouiCOM.Button lObjButtonCancel;
        private SAPbouiCOM.Matrix lObjMatrixInv;
        private SAPbouiCOM.EditText lObjEditTxtDate;
        private SAPbouiCOM.StaticText lObjLabelDate;
        private SAPbouiCOM.CheckBox lObjCheck = null;
        #endregion

        RejectedDAO mObjRejectedDAO = new RejectedDAO();
        InvoicesDTO lObjInvoices = new InvoicesDTO();
        List<InvoicesDTO> lListInvoices = new List<InvoicesDTO>();
        List<InvoicesDTO> lListInvoicesG = new List<InvoicesDTO>();
        DAO.InvoicesDAO mObjInvoicesDAO = new DAO.InvoicesDAO();
        IList<ConceptsToInvoiceDTO> lIlistConcepts = null;
        InspeccionService lObjInspeccionS = new InspeccionService();
        List<ConceptsToInvoiceDTO> lListConcepts = new List<ConceptsToInvoiceDTO>();
        IList<ConceptsDTO> lListConceptsUSD = null;
        IList<ConceptsDTO> lListConceptsCustoms = null;
        InspeccionT lOjInspT = null;


        Menu pObjMenu = new Menu();

        private SAPbobsCOM.Documents lObjDraftInvoice = null;
        SAPbobsCOM.Recordset lObjRecordSet = null;

        SAPbouiCOM.EditText lObjEditTxt { get; set; }

        #region variables
        double lDoubleWeightPerHead = 0;
        int lIntInspections = 0;

        string lStrUserName = DIApplication.Company.UserName;//get user name
        int lIntUserSign = DIApplication.Company.UserSignature;//get user sign
        string lStrMainRejectedWhs = "";
        string lStrPrincipalWhs = "";
        bool lBoolNoWeight = false;
        bool lBoolNoWeightInsp = false;
        bool lBoolSameInspections = false;
        //int lIntRow = 1;
        #endregion

        #region public variables
        //public string lStrTypeEx = "";
        //public int lIntTypeCount = 0;
        #endregion

        public frmInvoices()
        {
            LoadEvents();
            lStrMainRejectedWhs = mObjRejectedDAO.GetMainRWhs(lIntUserSign);
            lStrPrincipalWhs = mObjInvoicesDAO.GetPrincipalWhs(lIntUserSign);
            lObjEditTxtDate.Value = mObjInvoicesDAO.GetServerDate();
            ConfigForm();
            LogUtility.WriteInfo("Modulo Facturación Cuarentenarias Inicializado");

        }
        private void ConfigForm()
        {
            this.UIAPIRawForm.Left = 400;
            this.UIAPIRawForm.Top = 100;
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

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //lObjForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
            try
            {
                if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("BtnSearch"))
                                {
                                    LoadMatrix();
                                }
                                if (pVal.ItemUID.Equals("BtnAccept"))
                                {

                                    InvoiceList();

                                    lDoubleWeightPerHead = 0;
                                    lIntInspections = 0;
                                    lBoolNoWeight = false;
                                    //lIntRow = 1;
                                }

                                if (pVal.ItemUID.Equals("BtnCan"))
                                {
                                    CanceledList();
                                    lIntInspections = 0;
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnLoadEvents();
                                //pObjMenu.lIntTypeCount = 0;
                                //pObjMenu.lStrTypeEx = "";

                                pObjMenu.pArrTypeCount[2] = 0;
                                pObjMenu.pArrTypeEx[2] = "";
                                break;


                            case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                                if (pVal.ColUID.Equals("Col_Chck"))
                                {
                                    checkSameInspections(pVal.Row);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
                LogUtility.WriteError(ex.Message);
            }

        }

        private void checkSameInspections(int lIntActiveRow)
        {
            lObjCheck = ValidateRow(lIntActiveRow);

            if (lObjCheck != null)
            {
                string lStrIdIspection = Convert.ToString(((SAPbouiCOM.EditText)lObjMatrixInv.Columns.Item(1).Cells.Item(lIntActiveRow).Specific).Value);

                if (lObjCheck.Checked)
                {

                    FindSameInspection(lStrIdIspection, "Y");
                }
                else
                {

                    FindSameInspection(lStrIdIspection, "N");
                }
            }
        }

        private SAPbouiCOM.CheckBox ValidateRow(int lIntRow)
        {
            SAPbouiCOM.CheckBox lObjCheckBoxT = null;
            try
            {

                lObjCheckBoxT = (SAPbouiCOM.CheckBox)lObjMatrixInv.Columns.Item(0).Cells.Item(lIntRow).Specific;
                return lObjCheckBoxT;
            }
            catch (Exception ex)
            {
                return lObjCheckBoxT = null;
            }
        }

        private void FindSameInspection(string lStrInspection, string lStrAction)
        {
            for (int i = 1; i <= lObjMatrixInv.RowCount; i++)
            {
                if (Convert.ToString(((SAPbouiCOM.EditText)lObjMatrixInv.Columns.Item(1).Cells.Item(i).Specific).Value) == lStrInspection)
                {
                    lObjCheck = (SAPbouiCOM.CheckBox)lObjMatrixInv.Columns.Item(0).Cells.Item(i).Specific;
                    lObjCheck.Caption = lStrAction;



                }

            }
        }

        private void CanceledList()
        {
            int lIntPrevInsp = 0;
            lListInvoices = new List<InvoicesDTO>();
            for (int i = 1; i <= lObjMatrixInv.RowCount; i++)
            {
                if (((dynamic)lObjMatrixInv.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {

                    FillList(i);
                }
            }
            if (SameClient())
            {
                bool lBoolDifInsp = false;
                for (int i = 0; i < lListInvoices.Count; i++)
                {
                    int lIntInspections = AllInspections(lListInvoices[i].IdInspection);
                    if (AllInspections(lListInvoices[i].IdInspection) != lListInvoices.Count)//validates if all the inspections where selected to invoid
                    {
                        lBoolDifInsp = true;
                    }
                }
                if (lListInvoices.Count > 0)
                {
                    bool lBoolDeleted = false;
                    foreach (var item in lListInvoices)
                    {
                        if (lIntPrevInsp != item.IdInspection && lBoolDifInsp == false)
                        {
                            lBoolDeleted = DeleteDraft(item.Reference, item.CorralCode);
                            lIntPrevInsp = item.IdInspection;
                        }
                        else
                        {
                            if (lBoolDeleted == false)
                            {
                                lBoolDeleted = DeleteDraft(item.Reference, item.CorralCode);
                                lIntPrevInsp = item.IdInspection;
                            }
                        }


                    }
                    if (lBoolDeleted)
                    {
                        foreach (var item in lListInvoices)
                        {
                            UpdateInspectionDeleted(item.RowCode);
                        }

                    }
                    LoadMatrix();
                }
            }
            else
            {
                string lStrMessage = "Clientes diferentes";
                Application.SBO_Application.StatusBar.SetText(lStrMessage
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteWarning(lStrMessage);
            }

        }

        private void UpdateInspectionDeleted(int lintcode)
        {
            if (lObjInspeccionS.UpdateInspeccion(GetInspToUpdateCancel(lintcode)) != 0)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                LogUtility.WriteError(DIApplication.Company.GetLastErrorDescription());
            }
            else
            {

                Application.SBO_Application.StatusBar.SetText("Preliminar eliminada correctamente"
                    , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                LogUtility.WriteSuccess("Preliminar eliminada correctamente");
            }
        }


        private bool DeleteDraft(string lStrRef, string lStrCorral)
        {
            bool lBoolValid = false;

            if (!mObjInvoicesDAO.SearchInvoices(lStrRef, lStrCorral))
            {
                lObjDraftInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);


                int lIntKeyMX = mObjInvoicesDAO.GetDraftKey(lStrRef + "MX");
                if (lIntKeyMX > 0)
                {
                    lObjDraftInvoice.GetByKey(lIntKeyMX);
                    lObjDraftInvoice.Remove();

                }

                int lIntKeyUSD = mObjInvoicesDAO.GetDraftKey(lStrRef + "USD");
                if (lIntKeyUSD > 0)
                {
                    lObjDraftInvoice.GetByKey(lIntKeyUSD);
                    lObjDraftInvoice.Remove();
                }


                if (lIntKeyMX == 0 || lIntKeyUSD == 0)
                {
                    string lStrMsg = lIntKeyMX == 0 && lIntKeyUSD == 0 ? "No existen preliminares de esta inspeccion"
                        : lIntKeyMX != 0 && lIntKeyUSD == 0 ? "No existe preliminar en dolares" : "No existe preliminar en Pesos";

                    Application.SBO_Application.StatusBar.SetText(lStrMsg
             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteWarning(lStrMsg);
                    if (lIntKeyMX == 0 && lIntKeyUSD == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {

                    Application.SBO_Application.StatusBar.SetText("Las preliminares se eliminaron correctamente"
             , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    LogUtility.WriteSuccess("Las preliminares se eliminaron correctamente");
                    lBoolValid = true;
                }
            }
            else
            {

                Application.SBO_Application.StatusBar.SetText("La inspección ya tiene una factura no se puede eliminar"
         , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteWarning("La inspección ya tiene una factura no se puede eliminar");
                lBoolValid = false;
            }
            return lBoolValid;
        }

        private void FillList(int lIntRow)
        {
            string lStrExpDate = Convert.ToDateTime(this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("InspectionDate", lIntRow - 1)).ToString("yyyy-MM-dd");
            string lStrCardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("CardCode", lIntRow - 1);
            string lStrIdInsp = Convert.ToString(this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", lIntRow - 1));
            this.UIAPIRawForm.DataSources.DataTables.Item("DT_Can")
                   .ExecuteQuery(mObjInvoicesDAO.GetBatchToCancel(lStrExpDate, lStrCardCode, lStrMainRejectedWhs, lStrIdInsp));


            InvoicesDTO lObjInvoices = new InvoicesDTO();

            //lObjInvoices.IdInspection = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", lIntRow - 1);
            //lObjInvoices.ExpDate = (DateTime)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("InspectionDate", lIntRow - 1);
            //lObjInvoices.CorralCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Corralcode", lIntRow - 1);
            //lObjInvoices.Cancel = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Cancel", lIntRow - 1);
            //lObjInvoices.TQuantity = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Quantity", lIntRow - 1);
            //lObjInvoices.NP = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("NP", lIntRow - 1);
            //lObjInvoices.Rejected = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Rejected", lIntRow - 1);
            //lObjInvoices.Payment = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Payment", lIntRow - 1);
            //lObjInvoices.PaymentCustom = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("PaymentCustom", lIntRow - 1);
            //lObjInvoices.Article = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Article", lIntRow - 1);
            //lObjInvoices.Serial = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Serial", lIntRow - 1);
            //lObjInvoices.SerialName = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("SeriesName", lIntRow - 1);
            //lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("CardCode", lIntRow - 1);
            //lObjInvoices.AverageWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("AvWeight", lIntRow - 1);
            //lObjInvoices.RowCode = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("RowCode", lIntRow - 1);
            //lObjInvoices.RBatchNumber = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_Can").GetValue("DistNumber", 0);

            lObjInvoices.Name = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Name", lIntRow - 1);
            lObjInvoices.IdInspection = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", lIntRow - 1);
            lObjInvoices.ExpDate = (DateTime)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("InspectionDate", lIntRow - 1);
            lObjInvoices.CorralCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Corralcode", lIntRow - 1);
            lObjInvoices.Corral = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Corral", lIntRow - 1);
            lObjInvoices.DocEntryGI = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryGI", lIntRow - 1);
            lObjInvoices.Cancel = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Cancel", lIntRow - 1);
            lObjInvoices.SpecialInsp = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("SpecialInsp", lIntRow - 1);
            lObjInvoices.TQuantity = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Quantity", lIntRow - 1);
            lObjInvoices.NP = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("NP", lIntRow - 1);
            lObjInvoices.Rejected = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Rejected", lIntRow - 1);
            lObjInvoices.Payment = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Payment", lIntRow - 1);
            lObjInvoices.PaymentCustom = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("PaymentCustom", lIntRow - 1);
            lObjInvoices.Article = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Article", lIntRow - 1);
            lObjInvoices.Serial = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Serial", lIntRow - 1);
            lObjInvoices.SerialName = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("SeriesName", lIntRow - 1);
            lObjInvoices.DocEntryGR = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryGR", lIntRow - 1);
            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryIU", lIntRow - 1);
            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryIM", lIntRow - 1);
            lObjInvoices.TotalWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("TotKg", lIntRow - 1);
            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("CardCode", lIntRow - 1);
            lObjInvoices.AverageWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("AvWeight", lIntRow - 1);
            lObjInvoices.RealWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("RealWeigth", lIntRow - 1);
            lObjInvoices.RowCode = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("RowCode", lIntRow - 1);
            lObjInvoices.Reference = lObjInvoices.SerialName + "_" + lObjInvoices.IdInspection + "_";
            lObjInvoices.ThreePercent = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Payment", lIntRow - 1);

            lObjInvoices.BatchNumber = mObjInvoicesDAO.GetBatch(mObjInvoicesDAO.GetDocEntry((lObjInvoices.SerialName + lObjInvoices.IdInspection)), lObjInvoices.CorralCode);

            lListInvoices.Add(lObjInvoices);


        }

        private void InvoiceList()
        {
            lListInvoices = new List<InvoicesDTO>();
            lListInvoicesG = new List<InvoicesDTO>();
            lListConcepts = new List<ConceptsToInvoiceDTO>();
            lListConceptsCustoms = new List<ConceptsDTO>();
            lListConceptsUSD = new List<ConceptsDTO>();
            lBoolSameInspections = SameInspections();

            for (int i = 1; i <= lObjMatrixInv.RowCount; i++)
            {
                if (((dynamic)lObjMatrixInv.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {
                    if (((dynamic)lObjMatrixInv.Columns.Item(8).Cells.Item(i).Specific).Checked)
                    {
                        List(i, "Y");
                    }
                    else
                    {
                        List(i, "N");
                    }
                }
            }
            if (!lBoolNoWeight)
            {
                if (SameClient())
                {
                    if (lListInvoices.Count > 0)
                    {

                        ListCharges();
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Clientes diferentes"
                        , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteWarning("Clientes diferentes");
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Agregue todos los pesos"
                       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteWarning("Agregue todos los pesos");
            }
        }

        private void ListCharges()
        {

            string lStrXmLString = XmlString();

            lIlistConcepts = mObjInvoicesDAO.GetChargesToInvoce(lStrXmLString);
            ((List<ConceptsToInvoiceDTO>)lListConcepts).AddRange(lIlistConcepts);
            if (lIlistConcepts.Count > 0)
            {
                if (lListConcepts[0].DocEntry >= 0)
                {
                    this.UIAPIRawForm.Freeze(true);
                    Invoice();
                    this.UIAPIRawForm.Freeze(false);
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText(lListConcepts[0].ObjType
                   , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteError(lListConcepts[0].ObjType);
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Error en el retorno de datos"
                     , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteError("Error en el retorno de datos (Sin Datos)");
            }
        }

        private string XmlString()//gropuing the list by id
        {

            var lVarInspections = from p in lListInvoices group p by p.IdInspection into grupo select grupo;

            foreach (var group in lVarInspections)
            {
                lObjInvoices = new InvoicesDTO();
                string x = group.Key.ToString();
                foreach (var item in group)
                {
                    lObjInvoices.IdInspection = item.IdInspection;
                    lObjInvoices.TotalWeight += item.TotalWeight;

                    if (item.ThreePercent == "Y")
                    {
                        lObjInvoices.ThreePercent = "Y";
                    }

                    lObjInvoices.Serial = item.Serial;
                }

                if (lObjInvoices.ThreePercent == null)
                {
                    lObjInvoices.ThreePercent = "N";
                }
                lListInvoicesG.Add(lObjInvoices);
            }

            var xEle = new XElement("InspeccionXML",
                 from insp in lListInvoicesG
                 select new XElement("Inspeccion",
                     new XElement("IdInsp", insp.IdInspection),
                            new XElement("TotWeight", insp.TotalWeight),
                                new XElement("Food", insp.ThreePercent),
                                new XElement("Series", insp.Serial)
                            ));

            return xEle.ToString();



        }

        private void Invoice()
        {
            int IntRetCode = 0;
            int counter = 1;
            string lStrRefIU = "";
            string lStrRefIM = "";
            string lStrReference = "";
            string lStrCardCode = "";
            string lStrCostingCode = mObjInvoicesDAO.GetCostingCode(lStrUserName);
            string lStrSerialName = lListInvoices[0].SerialName;
            int lIntKeyD = 0;
            bool lBoolHasDrafts = false;
            bool lBoolHasInvoices = false;
            bool lBoolInvoice = false;
            SAPbouiCOM.Form lObjFormDraft = null;

            var lVarInspections = from p in lListInvoices group p by p.IdInspection into grupo select grupo;


            lStrCardCode = lListInvoices[0].CardCode;

            foreach (var item in lVarInspections)
            {

                if (lVarInspections.Count() > 1)
                {
                    if (counter == lVarInspections.Count())
                    {
                        lStrReference += lStrSerialName + "_" + item.Key;

                    }
                    else
                    {
                        lStrReference += lStrSerialName + "_" + item.Key + ",";
                    }
                }
                else
                {
                    lStrReference += lStrSerialName + "_" + item.Key;
                }
                counter++;
            }
            foreach (var item in lListInvoices)
            {
                if (!lBoolHasDrafts)
                {
                    lBoolHasDrafts = mObjInvoicesDAO.SearchDrafts(lStrReference, item.CorralCode);
                }
                if (!lBoolHasInvoices)
                {
                    lBoolHasInvoices = mObjInvoicesDAO.SearchInvoices(lStrReference, item.CorralCode);
                }
            }





            if (lBoolHasDrafts == false)
            {
                if (lBoolHasInvoices == false)
                {
                    if (ValidateCurrency(lListInvoices[0].CardCode))
                    {

                        #region USD
                        if (lListConcepts.Where(x => x.InvoiceType == "1").Count() >= 1)
                        {
                            lObjDraftInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);

                            lObjDraftInvoice.DocObjectCodeEx = "13";
                            lObjDraftInvoice.CardCode = lStrCardCode;
                            lObjDraftInvoice.NumAtCard = lStrReference + "_USD"; // Ref number
                            lObjDraftInvoice.DocCurrency = "USD";
                            foreach (var item in lListConcepts.Where(x => x.InvoiceType == "1"))
                            {
                                lObjDraftInvoice.Lines.ItemCode = item.ItemCode;
                                lObjDraftInvoice.Lines.Quantity = item.Quantity;
                                lObjDraftInvoice.Lines.COGSCostingCode = lStrCostingCode;
                                //lObjDraftInvoice.Lines.FreeText = item.IdInspection;
                                lObjDraftInvoice.Lines.FreeText = lStrPrincipalWhs + "_" + item.IdInspection + "_USD";

                                lObjDraftInvoice.Lines.UnitPrice = item.Price;

                                lObjDraftInvoice.Lines.Add();
                                lBoolInvoice = true;

                            }
                            if (lBoolInvoice)
                            {
                                IntRetCode = lObjDraftInvoice.Add();
                                if (IntRetCode != 0)
                                {
                                    string ddd = string.Empty;
                                    ddd = DIApplication.Company.GetLastErrorDescription().ToString();

                                    Application.SBO_Application.StatusBar.SetText(ddd
                                            , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return;
                                }
                                else
                                {
                                    lStrRefIU = lStrReference + "_USD";

                                    lIntKeyD = mObjInvoicesDAO.GetDraftKey(lStrRefIU);
                                    if (lIntKeyD != 0)
                                    {
                                        lObjFormDraft = Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lIntKeyD.ToString());
                                    }
                                }
                            }
                        }
                        #endregion
                        #region MX
                        if (lListConcepts.Where(x => x.InvoiceType == "0").Count() >= 1)
                        {
                            lObjDraftInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                            //head
                            lObjDraftInvoice.DocObjectCodeEx = "13";
                            lObjDraftInvoice.CardCode = lStrCardCode;
                            lObjDraftInvoice.NumAtCard = lStrReference + "_MX"; // Ref number
                            //                   /// charges
                            foreach (var lVar in lListConcepts.Where(x => x.InvoiceType == "0"))
                            {
                                lObjDraftInvoice.Lines.ItemCode = lVar.ItemCode;
                                lObjDraftInvoice.Lines.Quantity = lVar.Quantity;
                                if (lVar.DocEntry > 0)
                                {
                                    lObjDraftInvoice.Lines.BaseEntry = lVar.DocEntry;
                                    lObjDraftInvoice.Lines.BaseLine = lVar.LineNum;
                                    lObjDraftInvoice.Lines.BaseType = Convert.ToInt32(lVar.ObjType);
                                }
                                lObjDraftInvoice.Lines.UnitPrice = lVar.Price;
                                lObjDraftInvoice.Lines.TaxCode = lVar.TaxCode;
                                lObjDraftInvoice.Lines.COGSCostingCode = lStrCostingCode;
                                //lObjDraftInvoice.Lines.FreeText = lVar.IdInspection;
                                lObjDraftInvoice.Lines.FreeText = lStrPrincipalWhs + "_" + lVar.IdInspection + "_MX";

                                lObjDraftInvoice.Lines.Add();
                            }
                            IntRetCode = lObjDraftInvoice.Add();
                            if (IntRetCode != 0)
                            {
                                string error = DIApplication.Company.GetLastErrorDescription();
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(error);
                                return;
                            }
                            else
                            {
                                lStrRefIM = lStrReference + "_MX";

                                lIntKeyD = mObjInvoicesDAO.GetDraftKey(lStrRefIM);
                                if (lIntKeyD != 0)
                                {
                                    lObjFormDraft = Application.SBO_Application.OpenForm((SAPbouiCOM.BoFormObjectEnum)112, "", lIntKeyD.ToString());
                                }


                                foreach (var item in lListInvoices)
                                {
                                    if (lObjInspeccionS.UpdateInspeccion(GetInspToUpdate(item.RowCode, lStrRefIU, lStrRefIM)) != 0)
                                    {
                                        Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                                    }
                                    else
                                    {
                                        LoadMatrix();
                                        Application.SBO_Application.StatusBar.SetText("Preliminar realizada exitosamente"
                                            , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("El cliente no es multimoneda"
 , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        LogUtility.WriteInfo("El cliente no es multimoneda");
                        return;
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Esta inspección ya cuenta con una factura"
          , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteInfo("Esta inspección ya cuenta con una factura");
                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Esta inspección ya tiene una preliminar"
           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                LogUtility.WriteInfo("Esta inspección ya tiene una preliminar");
                ///abrir la ventana de preliminares
            }

        }

        private void USDInvoice(string lStrOption, string lStrCardCode, string lStrCorralCode, int lIntTotalExportHeads, string lStrReference, string lStrCostingCode)
        {
            lObjDraftInvoice = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            lListConceptsUSD = mObjInvoicesDAO.GetConceptsDlls();
            switch (lStrOption)
            {
                case "Single":

                    lObjDraftInvoice.DocObjectCodeEx = "13";
                    lObjDraftInvoice.CardCode = lStrCardCode;
                    lObjDraftInvoice.NumAtCard = lStrReference + "USD";
                    lObjDraftInvoice.Lines.COGSCostingCode = lStrCostingCode;
                    lObjDraftInvoice.DocCurrency = "USD";


                    foreach (var item in lListConceptsUSD)
                    {
                        lObjDraftInvoice.Lines.ItemCode = item.Value;
                        lObjDraftInvoice.Lines.Quantity = lIntTotalExportHeads;
                        lObjDraftInvoice.Lines.COGSCostingCode = lStrCostingCode;

                        lObjDraftInvoice.Lines.Price = Convert.ToDouble(mObjInvoicesDAO.GetPriceByWhs(item.Value, lStrPrincipalWhs));

                        lObjDraftInvoice.Lines.Add();
                    }


                    break;
            }
        }


        private Tables.InspeccionT GetInspToUpdate(int lIntCode, string lStrRefIU, string lStrRefIM)
        {
            InspeccionT lOjInspT = new InspeccionT();
            foreach (var item in lListInvoices.Where(x => x.RowCode == lIntCode))
            {
                lOjInspT.RowCode = lIntCode.ToString();
                lOjInspT.Name = item.Name;
                lOjInspT.IDInsp = item.IdInspection;
                lOjInspT.DateInsp = item.ExpDate;
                lOjInspT.DateSys = item.ExpDate;
                lOjInspT.User = lIntUserSign;
                lOjInspT.CardCode = item.CardCode;
                lOjInspT.WhsCode = item.CorralCode;
                lOjInspT.DocEntryGI = item.DocEntryGI;
                lOjInspT.Cancel = item.Cancel;
                lOjInspT.CheckInsp = item.SpecialInsp;
                if (!lBoolNoWeightInsp)
                {
                    lOjInspT.TotKls = (float)item.TotalWeight;
                    if (item.AverageWeight > 0)
                    {
                        lOjInspT.AverageW = (float)item.AverageWeight;
                    }
                    else
                    {
                        lOjInspT.AverageW = (float)(item.TotalWeight / item.TQuantity);
                    }
                }

                lOjInspT.Quantity = item.TQuantity;
                lOjInspT.QuantityNP = item.NP;
                lOjInspT.QuantityReject = item.Rejected;
                lOjInspT.Payment = item.ThreePercent;
                lOjInspT.PaymentCustom = item.PaymentCustom;
                lOjInspT.Classification = item.Article;
                lOjInspT.Series = item.Serial;
                lOjInspT.DocEntryGR = item.DocEntryGR;
                lOjInspT.DocEntryIU = lStrRefIU; //------------
                lOjInspT.DocEntryIM = lStrRefIM;//----------

                return lOjInspT;
            }

            return lOjInspT;
        }

        private Tables.InspeccionT GetInspToUpdateCancel(int lIntRowCode)
        {
            InspeccionT lOjInspT = new InspeccionT();
            foreach (var item in lListInvoices.Where(x => x.RowCode == lIntRowCode))
            {

                lOjInspT.RowCode = item.RowCode.ToString();
                lOjInspT.Name = item.Name;
                lOjInspT.IDInsp = item.IdInspection;
                lOjInspT.DateInsp = item.ExpDate;
                lOjInspT.DateSys = item.ExpDate;
                lOjInspT.User = lIntUserSign;
                lOjInspT.CardCode = item.CardCode;
                lOjInspT.WhsCode = item.CorralCode;
                lOjInspT.DocEntryGI = item.DocEntryGI;
                lOjInspT.Cancel = item.Cancel;
                lOjInspT.CheckInsp = item.SpecialInsp;
                if (!lBoolNoWeightInsp)
                {
                    lOjInspT.TotKls = (float)item.TotalWeight;
                    if (item.AverageWeight > 0)
                    {
                        lOjInspT.AverageW = (float)item.AverageWeight;
                    }
                    else
                    {
                        lOjInspT.AverageW = (float)(item.TotalWeight / item.TQuantity);
                    }
                }

                lOjInspT.Quantity = item.TQuantity;
                lOjInspT.QuantityNP = item.NP;
                lOjInspT.QuantityReject = item.Rejected;
                lOjInspT.Payment = item.ThreePercent;
                lOjInspT.PaymentCustom = item.PaymentCustom;
                lOjInspT.Classification = item.Article;
                lOjInspT.Series = item.Serial;
                lOjInspT.DocEntryGR = item.DocEntryGR;
                lOjInspT.DocEntryIU = ""; //------------
                lOjInspT.DocEntryIM = "";//----------


            }

            return lOjInspT;
        }

        #region Conditions


        private int AllInspections(int lIntInsp)
        {
            if (lIntInspections == 0)
            {
                for (int i = 1; i <= lObjMatrixInv.RowCount; i++)
                {
                    if (((dynamic)lObjMatrixInv.Columns.Item(0).Cells.Item(i).Specific).Checked)
                    {
                        int x = (int)(this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", i - 1));
                        if (x == lIntInsp)
                        {
                            lIntInspections++;
                        }
                    }
                }

            }
            return lIntInspections;
        }

        private bool SameClient()
        {
            //Validate Same client to make an invoid
            if (lListInvoices.Count > 1)
            {
                var quer = lListInvoices.GroupBy(x => x.Name).Where(x => x.Count() > 1).SelectMany(g => g);
                if (quer.Count() == lListInvoices.Count)
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
        private int SameCorral()
        {
            var l = (from p in lListInvoices group p by new { p.CorralCode }).Count();
            //var query = l.GroupBy(y => y.Key.CorralCode).Where(y => y.Count() > 1).SelectMany(g => g);

            return l;


        }


        private bool ValidateCurrency(string lStrCardCode)
        {
            bool lBoolValid = false;
            string lStrCurrencyByBP = mObjInvoicesDAO.GetCurrencyByBP(lStrCardCode);
            if (lStrCurrencyByBP != string.Empty)
            {
                if (lStrCurrencyByBP == "##" || lStrCurrencyByBP == "USD")
                {
                    lBoolValid = true;
                }
                else
                {
                    lBoolValid = false;
                }
            }

            return lBoolValid;
        }

        #endregion

        private void List(int lIntRow, string lStrPercent)
        {


            InvoicesDTO lObjInvoices = new InvoicesDTO();

            lObjInvoices.Name = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Name", lIntRow - 1);
            lObjInvoices.IdInspection = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", lIntRow - 1);
            lObjInvoices.ExpDate = (DateTime)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("InspectionDate", lIntRow - 1);
            lObjInvoices.CorralCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Corralcode", lIntRow - 1);
            lObjInvoices.Corral = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Corral", lIntRow - 1);
            lObjInvoices.DocEntryGI = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryGI", lIntRow - 1);
            lObjInvoices.Cancel = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Cancel", lIntRow - 1);
            lObjInvoices.SpecialInsp = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("SpecialInsp", lIntRow - 1);

            if ((double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("TotKg", lIntRow - 1) > 0 || lBoolSameInspections)
            {
                lObjInvoices.TotalWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("TotKg", lIntRow - 1);
                lBoolNoWeightInsp = false;
            }
            else
            {

                var lVarValue = (((SAPbouiCOM.EditText)lObjMatrixInv.Columns.Item(6).Cells.Item(lIntRow).Specific).Value).ToString();
                if (Regex.IsMatch(lVarValue.ToString(), "^-?\\d*(\\.\\d+)?$"))
                {
                    if (Convert.ToDouble(((SAPbouiCOM.EditText)lObjMatrixInv.Columns.Item(6).Cells.Item(lIntRow).Specific).Value) > 0)
                    {

                        lObjInvoices.TotalWeight = Convert.ToDouble(((SAPbouiCOM.EditText)lObjMatrixInv.Columns.Item(6).Cells.Item(lIntRow).Specific).Value);
                        lBoolNoWeight = false;
                    }
                    else
                    {
                        lBoolNoWeight = true;
                        lBoolNoWeightInsp = true;
                        return;
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Solo números positivos"
                           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    LogUtility.WriteWarning("Números negativos");
                    return;
                }
            }
            lObjInvoices.TQuantity = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Quantity", lIntRow - 1);
            lObjInvoices.NP = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("NP", lIntRow - 1);
            lObjInvoices.Rejected = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Rejected", lIntRow - 1);
            lObjInvoices.Payment = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Payment", lIntRow - 1);
            lObjInvoices.PaymentCustom = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("PaymentCustom", lIntRow - 1);
            lObjInvoices.Article = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Article", lIntRow - 1);
            lObjInvoices.Serial = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Serial", lIntRow - 1);
            lObjInvoices.SerialName = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("SeriesName", lIntRow - 1);
            lObjInvoices.DocEntryGR = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryGR", lIntRow - 1);
            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryIU", lIntRow - 1);
            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("DocEntryIM", lIntRow - 1);

            lObjInvoices.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("CardCode", lIntRow - 1);
            lObjInvoices.AverageWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("AvWeight", lIntRow - 1);
            lObjInvoices.RealWeight = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("RealWeigth", lIntRow - 1);
            lObjInvoices.ThreePercent = lStrPercent;
            lObjInvoices.RowCode = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("RowCode", lIntRow - 1);

            lListInvoices.Add(lObjInvoices);
        }

        private bool SameInspections()
        {
            bool lBoolValid = false;
            int lIntPrevInsp = 0;
            for (int i = 1; i < lObjMatrixInv.RowCount; i++)
            {
                if (((dynamic)lObjMatrixInv.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {
                    int lIntInspection = (int)this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv").GetValue("Inspection", i - 1);
                    if (lIntInspection == lIntPrevInsp || lIntPrevInsp == 0)
                    {
                        lBoolValid = true;
                        lIntPrevInsp = lIntInspection;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return lBoolValid;
        }

        private void LoadMatrix()
        {
            try
            {

                this.UIAPIRawForm.DataSources.DataTables.Item("DT_MtxInv")
                    .ExecuteQuery(mObjInvoicesDAO.GetInspectionsQuery(lObjEditTxtDate.Value, lIntUserSign));

                lObjMatrixInv.Columns.Item("Col_Chck").DataBind.Bind("DT_MtxInv", "Chk");
                lObjMatrixInv.Columns.Item("Col_IdInsp").DataBind.Bind("DT_MtxInv", "Inspection");
                lObjMatrixInv.Columns.Item("Col_Name").DataBind.Bind("DT_MtxInv", "Name");
                lObjMatrixInv.Columns.Item("Col_Corral").DataBind.Bind("DT_MtxInv", "Corral");
                lObjMatrixInv.Columns.Item("Col_Heads").DataBind.Bind("DT_MtxInv", "Quantity");
                lObjMatrixInv.Columns.Item("Col_3pc").DataBind.Bind("DT_MtxInv", "Chk3pc");
                lObjMatrixInv.Columns.Item("Col_Kilos").DataBind.Bind("DT_MtxInv", "TotKg");///
                lObjMatrixInv.Columns.Item("Col_Art").DataBind.Bind("DT_MtxInv", "Item");
                lObjMatrixInv.Columns.Item("NewW").DataBind.Bind("DT_MtxInv", "NewWeight");
                lObjMatrixInv.Columns.Item("Col_Stat").DataBind.Bind("DT_MtxInv", "Stat");

                lObjMatrixInv.LoadFromDataSource();
                lObjMatrixInv.AutoResizeColumns();

            }
            catch (Exception er)
            {

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }


        #region default
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lObjButtonSearch = ((SAPbouiCOM.Button)(this.GetItem("BtnSearch").Specific));
            this.lObjButtonAccept = ((SAPbouiCOM.Button)(this.GetItem("BtnAccept").Specific));
            this.lObjMatrixInv = ((SAPbouiCOM.Matrix)(this.GetItem("MatrixInv").Specific));
            this.lObjEditTxtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.lObjLabelDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.lObjButtonCancel = ((SAPbouiCOM.Button)(this.GetItem("BtnCan").Specific));
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
