using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.Core.Utility;

namespace Corrales
{
    public class modalCharges
    {
        private SAPbouiCOM.Form lObjFormChargesXml = null;

        SAPbouiCOM.Matrix lObjMatrix = null;
        SAPbouiCOM.Matrix mtxItems = null;
        SAPbouiCOM.DataTable lObjDTFloorC = null;
        SAPbobsCOM.Documents lObjInvoice = null;
        SAPbouiCOM.CheckBox lObjCheckboxInv = null;
        private ProgressBarManager mObjProgressBar = null;


        DAO.ChargesDAO lObjChargesDAO = new DAO.ChargesDAO();
        IList<FloorChargesDTO> lObjLstCorrals = null;
        List<FloorChargesDTO> lListToInvoice = new List<FloorChargesDTO>();
        List<FloorChargesDTO> lObjLstToInvoiceT = new List<FloorChargesDTO>();

        Recordset lObjRecordSet = null;

        DateTime lObjDatetime;

        string[] scs = { "yyyyMMdd" };

        #region variables
        string lStrCardCode = null;
        string lStrDocDate = null;
        string lStrMainWhs = "";
        double lDbPrice = 0;
        string lStrArticletoCost = "";
        int lIntUserSign = DIApplication.Company.UserSignature;

        List<string> lStrlstCorral = null;
        List<string> lStrError = null;


        #endregion

        public modalCharges(SAPbouiCOM.Form pObjFormXml, string pStrCardCode, string pStrDocDate, List<string> pStrLstCorral)
        {
            this.lObjFormChargesXml = pObjFormXml;
            this.lStrCardCode = pStrCardCode;
            this.lStrDocDate = pStrDocDate;
            this.lStrlstCorral = pStrLstCorral;
            this.lStrMainWhs = lObjChargesDAO.GetWhs(lIntUserSign);
            DateTime.TryParseExact(lObjChargesDAO.GetServerDate(), scs, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
             , out lObjDatetime);

            lObjLstCorrals = new List<FloorChargesDTO>();
            LoadXmlForm("ChargesModalXML.xml", "FloorCharges");
            loadMatrixFormXML();
        }

        private void LoadXmlForm(string FileName, string FormName)
        {
            System.Xml.XmlDocument lObjXmlDoc = new System.Xml.XmlDocument();
            //string lStrPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]).ToString();
            string lStrPath = PathUtilities.GetCurrent("XMLforms\\ChargesModalXML.xml");

            lObjXmlDoc.Load(lStrPath);

            SAPbouiCOM.FormCreationParams lObjCreationPackage = (SAPbouiCOM.FormCreationParams)SAPbouiCOM.Framework.Application.SBO_Application.CreateObject
                  (SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            lObjCreationPackage.XmlData = lObjXmlDoc.InnerXml;

            lObjCreationPackage.UniqueID = FormName;
            lObjCreationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
            lObjCreationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
            lObjCreationPackage.FormType = FormName;

            lObjFormChargesXml = SAPbouiCOM.Framework.Application.SBO_Application.Forms.AddEx(lObjCreationPackage);
            lObjFormChargesXml.Title = "Cargos de piso";
            lObjFormChargesXml.Left = 400;
            lObjFormChargesXml.Top = 100;
            lObjFormChargesXml.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
            lObjFormChargesXml.Visible = true;
            initFormXml();
        }

        private void initFormXml()
        {
            lObjFormChargesXml.Freeze(true);
            initItems();
            lObjFormChargesXml.Freeze(false);
        }

        private void initItems()
        {

            lObjMatrix = ((SAPbouiCOM.Matrix)lObjFormChargesXml.Items.Item("Item_0").Specific);
            lObjMatrix.AutoResizeColumns();
            lStrArticletoCost = lObjChargesDAO.GetArticleToInvoice();
            lDbPrice = lObjChargesDAO.GetPrice(lStrArticletoCost, lObjChargesDAO.GetWhs(lIntUserSign));

            initSysMatrix();
            ListCorrals();

        }

        private void initSysMatrix()
        {
            UIApplication.GetApplication().ActivateMenuItem("2053");

            SAPbouiCOM.Form lObjForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(133, -1);
            mtxItems = (SAPbouiCOM.Matrix)lObjForm.Items.Item("38").Specific;
        }

        private void ListCorrals()
        {
            lObjLstCorrals = lObjChargesDAO.GetEntriesList(lStrCardCode, lStrlstCorral, lStrMainWhs);
        }


        private void loadMatrixFormXML()
        {
            lObjFormChargesXml.DataSources.DataTables.Add("XmlResult");

            lObjDTFloorC = lObjFormChargesXml.DataSources.DataTables.Item("XmlResult");

            lObjDTFloorC.Columns.Add("colChck", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            lObjDTFloorC.Columns.Add("colWhs", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            lObjDTFloorC.Columns.Add("colTotal", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            lObjDTFloorC.Columns.Add("colHeads", SAPbouiCOM.BoFieldsType.ft_Integer);
            fillMatrix();
        }

        private void fillMatrix()
        {

            GetCostsVariables();


            for (int i = 0; i < lObjLstToInvoiceT.Count; i++)
            {

                lObjDTFloorC.Rows.Add();

                lObjDTFloorC.Columns.Item("colChck").Cells.Item(i).Value = lObjLstToInvoiceT[i].Checked;
                lObjDTFloorC.Columns.Item("colWhs").Cells.Item(i).Value = lObjLstToInvoiceT[i].Corral;
                lObjDTFloorC.Columns.Item("colTotal").Cells.Item(i).Value = lObjLstToInvoiceT[i].Total;
                lObjDTFloorC.Columns.Item("colHeads").Cells.Item(i).Value = lObjLstToInvoiceT[i].Quantity;

                lObjMatrix.Columns.Item("colChck").DataBind.Bind("XmlResult", "colChck");
                lObjMatrix.Columns.Item("colWhs").DataBind.Bind("XmlResult", "colWhs");
                lObjMatrix.Columns.Item("colTotal").DataBind.Bind("XmlResult", "colTotal");
                lObjMatrix.Columns.Item("colHeads").DataBind.Bind("XmlResult", "colHeads");
            }

            lObjMatrix.LoadFromDataSource();
        }

        private void GetCostsVariables()
        {
            string lStrPrevDstNmb = "";
            DateTime lObjDtmDocDate = lObjDatetime;
            DateTime lObjDtCharge = lObjDatetime;
            int lIntPrevQtyHeads = 0;
            bool lBoolFreeDay = false;

            //DateTime lObjDTGoodIssue = lObjDatetime;

            var lVarGroupedList = from l in lObjLstCorrals group l by new { l.Corral, l.DistNumber };

            foreach (var lVarGroup in lVarGroupedList)
            {
                FloorChargesDTO lObjFloorChargesDTO = new FloorChargesDTO();

                lObjFloorChargesDTO.Corral = lVarGroup.Key.Corral;
                lObjFloorChargesDTO.DocDate = lObjDatetime;

                lObjFloorChargesDTO.DistNumber = lVarGroup.Key.DistNumber;
                foreach (var item in lVarGroup)
                {
                    lObjDtCharge = lObjDatetime;

                    if (item.DocDate <= lObjFloorChargesDTO.DocDate && item.Direction == 0 && item.BaseType == 59 || item.BaseType == 67 && item.Direction == 0)
                    {
                        if (item.InvDate == DateTime.Parse("1900-01-01"))
                        {
                            lObjFloorChargesDTO.DocDate = item.DocDate;
                        }
                        else
                        {
                            lObjFloorChargesDTO.DocDate = item.InvDate;
                            lBoolFreeDay = true;
                        }
                        if (item.BaseType == 67)
                        {
                            lBoolFreeDay = true;
                        }

                        lObjFloorChargesDTO.Quantity += item.Quantity;
                    }
                    else if (item.Direction == 1)
                    {
                        if ((item.BaseType == 60 || item.BaseType == 67) && item.MovType != "6")
                        {
                            if (item.BaseType == 67)
                            {
                                lStrPrevDstNmb = item.DistNumber;
                                lObjDtCharge = item.DocDate;
                            }

                            lIntPrevQtyHeads = lObjFloorChargesDTO.Quantity;
                            lObjFloorChargesDTO.Quantity -= item.Quantity;

                        }
                        else
                        {
                            if (item.Quantity < lObjFloorChargesDTO.Quantity)
                            {

                                lObjFloorChargesDTO.Quantity -= item.Quantity;
                            }

                            lObjFloorChargesDTO.MovType = item.MovType;
                        }
                    }

                }

                if (lObjFloorChargesDTO.Quantity > 0)
                {
                    lIntPrevQtyHeads = lObjFloorChargesDTO.Quantity;
                }
                lObjFloorChargesDTO.DaysxCorral = GetDays(lObjFloorChargesDTO.DocDate, lObjDtCharge, lBoolFreeDay);  // Add of Days to the concept 

                lObjFloorChargesDTO.Total = setCosts(lObjFloorChargesDTO.DaysxCorral, lObjFloorChargesDTO.MovType, lObjFloorChargesDTO.Quantity, lIntPrevQtyHeads); //SetCosts

                lObjFloorChargesDTO.DaysxCorral = lObjFloorChargesDTO.DaysxCorral * lIntPrevQtyHeads; // Add of heads to the concept

                lBoolFreeDay = false;

                if (lObjFloorChargesDTO.MovType == "6" && lObjFloorChargesDTO.Quantity == 0)
                {

                }
                else if (lObjFloorChargesDTO.Quantity >= 0)
                {
                    lObjLstToInvoiceT.Add(lObjFloorChargesDTO);
                }

            }

           
            if (lStrlstCorral.Count > 0)
            {
                foreach (var lVarCorral in lStrlstCorral)
                {
                    foreach (var item in lObjLstToInvoiceT)
                    {
                        if (!AddedAlready(item.Corral))
                        {
                            if (item.Corral == lVarCorral)
                            {
                                item.Checked = "Y";
                            }
                            else
                            {
                                item.Checked = "N";
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in lObjLstToInvoiceT)
                {
                    item.Checked = "N";
                }
            }

            


        }

        private bool AddedAlready(string pStrCorral)
        {
            bool lBoolChecked = false;
            for (int i = 0; i < mtxItems.RowCount; i++)
            {
                string lStrCorralFlag = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("163").Cells.Item(i+1).Specific).Value;
                string lStrTotal = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("21").Cells.Item(i+1).Specific).Value;
                string lStrDaysxCorral = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("11").Cells.Item(i+1).Specific).Value;

                if (lStrCorralFlag == pStrCorral && DifferentBatch2(lStrCorralFlag, lStrTotal, lStrDaysxCorral))
                {
                    lBoolChecked = true;
                }
            }
            return lBoolChecked;
        }

        private bool DifferentBatch2(string lStrCorral, string lStrTotal, string lStrDaysxC)
        {
            bool lBoolValid = false;
            double lDbTotal = double.Parse(lStrTotal.Substring(0, lStrTotal.Length - 3));

            if (lObjLstToInvoiceT.Count > 0)
            {
                foreach (var item in lObjLstToInvoiceT.Where(x => x.Corral == lStrCorral && x.Total == lDbTotal && x.DaysxCorral == double.Parse(lStrDaysxC)))
                {
                    lBoolValid = true;
                }
            }

            return lBoolValid;
        }

        private double setCosts(int lIntDays, string lStrMvType, int lIntTotHeads, int lIntQuantity)
        {
            double lDbFormula = 0;

            if (lIntTotHeads == 0 && lStrMvType == "6")
            {
                lDbFormula = (lIntQuantity * lDbPrice) * 1;
            }
            else
            {
                lDbFormula = (lIntQuantity * lDbPrice) * lIntDays;
            }


            return lDbFormula;
        }

        private int GetDays(DateTime lObjDtDocDate, DateTime lObjDtCharge, bool lBoolFreeDay)
        {
            TimeSpan lTimeSpanCharges = new TimeSpan();
            int lIntDays = 0;

            lTimeSpanCharges = lObjDtCharge - lObjDtDocDate;
            if (!lBoolFreeDay)
            {
                lIntDays = lTimeSpanCharges.Days + 1;
            }
            else
            {
                lIntDays = lTimeSpanCharges.Days;
            }


            return lIntDays;
        }

        private void initProgressBar(int lIntSize)
        {
            mObjProgressBar = new ProgressBarManager(SAPbouiCOM.Framework.Application.SBO_Application, "Cargando lineas del Xml", lIntSize);
        }

        public bool AnyLineToInvoice()
        {
            int lIntChecked = 0;
            for (int i = 1; i <= lObjMatrix.RowCount; i++)
            {
                if (((dynamic)lObjMatrix.Columns.Item(0).Cells.Item(i).Specific).Checked)
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
                return false;
            }
        }

        public void Invoice()
        {
            ListToInvoice();
            lStrError = new List<string>();
            try
            {
                lObjFormChargesXml.Freeze(true);
                initProgressBar(lListToInvoice.Count);

                int lIntMtxRows = mtxItems.RowCount;

                for (int i = 0; i < lListToInvoice.Count; i++)
                {
                    if (lListToInvoice[i].Checked == "Y")
                    {
                        if (!checkSystemMatrix(lListToInvoice[i].Corral))
                        {
                            ((SAPbouiCOM.EditText)mtxItems.Columns.Item(3).Cells.Item(i + lIntMtxRows).Specific).Value = lObjChargesDAO.GetArticleToInvoice();
                            //((SAPbouiCOM.EditText)mtxItems.Columns.Item(20).Cells.Item(i + lIntMtxRows).Specific).Value = lListToInvoice[i].Total.ToString();
                            ((SAPbouiCOM.EditText)mtxItems.Columns.Item("163").Cells.Item(i + lIntMtxRows).Specific).Value = lListToInvoice[i].Corral.ToString();
                            ((SAPbouiCOM.EditText)mtxItems.Columns.Item("11").Cells.Item(i + lIntMtxRows).Specific).Value = lListToInvoice[i].DaysxCorral.ToString();
                            ((SAPbouiCOM.EditText)mtxItems.Columns.Item("14").Cells.Item(i + lIntMtxRows).Specific).Value = lDbPrice.ToString();


                        }
                        else
                        {
                            lStrError.Add("Ya existe una linea para facturar para corral " + lListToInvoice[i].Corral);
                            lIntMtxRows = lIntMtxRows - 1;
                        }
                    }
                    mObjProgressBar.NextPosition();
                }
            }
            catch (Exception lObjException)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(lObjException.ToString()
                                , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                lObjFormChargesXml.Freeze(false);
                mObjProgressBar.Dispose();
                ShowErrors();
            }

        }

        private void ShowErrors()
        {
            if (lStrError.Count > 0)
            {
                foreach (var item in lStrError)
                {
                    SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(item
                             , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }

        }

        private bool checkSystemMatrix(string pStrCorral)
        {
            bool lBoolChecked = false;

            for (int i = 1; i < mtxItems.RowCount; i++)
            {
                string lStrCorralFlag = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("163").Cells.Item(i).Specific).Value;
                string lStrTotal = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("21").Cells.Item(i).Specific).Value;
                string lStrDaysxCorral = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("11").Cells.Item(i).Specific).Value;

                if (lStrCorralFlag == pStrCorral && DifferentBatch(i, lStrCorralFlag, lStrTotal, lStrDaysxCorral))
                {
                    lBoolChecked = true;
                }
            }
            return lBoolChecked;
        }

        private bool DifferentBatch(int lIntRow, string lStrCorral, string lStrTotal, string lStrDaysxC)
        {
            bool lBoolValid = false;
            double lDbTotal = double.Parse(lStrTotal.Substring(0, lStrTotal.Length - 3));

            if (lListToInvoice.Count > 0)
            {
                foreach (var item in lListToInvoice.Where(x => x.Corral == lStrCorral))
                {
                 
                        if (item.Total == lDbTotal)
                        {
                            lBoolValid = true;
                        }
                        else
                        {
                            return false;
                        }
                    
                }
            }

            return lBoolValid;
        }

        //private bool checkIfConceptExists(string pStrCorral)
        //{
        //    bool lBoolChecked = false;

        //    for (int i = 1; i < mtxItems.RowCount; i++)
        //    {
        //        string lStrCorralFlag = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("163").Cells.Item(i).Specific).Value;
        //        string lStrUnitPrice = ((SAPbouiCOM.EditText)mtxItems.Columns.Item("14").Cells.Item(i).Specific).Value;

        //        if (lStrCorralFlag == pStrCorral && DifferentBatch(i, lStrCorralFlag, lStrUnitPrice))
        //        {
        //            lBoolChecked = true;
        //        }
        //    }
        //    return lBoolChecked;
        //}

        private void ListToInvoice()
        {
            lListToInvoice = new List<FloorChargesDTO>();

            for (int i = 1; i <= lObjMatrix.RowCount; i++)
            {
                if (((dynamic)lObjMatrix.Columns.Item(0).Cells.Item(i).Specific).Checked)
                {
                    lObjLstToInvoiceT[i - 1].Checked = "Y";
                }
            }

            ((List<FloorChargesDTO>)lListToInvoice).AddRange(lObjLstToInvoiceT.Where(x => x.Checked == "Y"));
        }

        internal void ForbiddenUncheck(int lIntActiveRow)
        {
            lObjCheckboxInv = ValidCheckinRow(lIntActiveRow);

            if (lObjCheckboxInv != null)
            {
                lObjFormChargesXml.Freeze(true);

                string lStrCorral = Convert.ToString(((SAPbouiCOM.EditText)lObjMatrix.Columns.Item("colWhs").Cells.Item(lIntActiveRow).Specific).Value);
                if (!AddedAlready(lStrCorral))
                {
                    foreach (var item in lStrlstCorral)
                    {

                        if (lStrCorral == item)
                        {
                            lObjCheckboxInv.Caption = "Y";
                        }
                    }
                }
                else
                {
                    lObjCheckboxInv.Caption = "N";
                }

                lObjFormChargesXml.Freeze(false);
            }

        }

        private SAPbouiCOM.CheckBox ValidCheckinRow(int lIntActiveRow)
        {
            SAPbouiCOM.CheckBox lObjCheckBoxT = null;
            try
            {
                lObjCheckBoxT = (SAPbouiCOM.CheckBox)lObjMatrix.Columns.Item("colChck").Cells.Item(lIntActiveRow).Specific;
                return lObjCheckBoxT;
            }
            catch (Exception err)
            {
                return lObjCheckBoxT = null;
            }
        }
    }
}
