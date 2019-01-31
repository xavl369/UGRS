using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UGRS.AddOn.Permissions.DTO;
using UGRS.AddOn.Permissions.Tables;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;


namespace UGRS.AddOn.Permissions
{
    public class mFormEarringRanks
    {
        #region SAP objects
        SAPbouiCOM.Form lObjRanksForm = null;
        SAPbouiCOM.Grid lObjRanksGrid = null;
        SAPbouiCOM.EditText lObjETxtFrom = null;
        SAPbouiCOM.EditText lObjETxtTo = null;
        SAPbouiCOM.EditText lObjETActivePrefix = null;
        SAPbouiCOM.DataTable lObjDtRanks = null;
        #endregion

        Services.EarringRanksService lObjEarRankService = new Services.EarringRanksService();
        DAO.EarringRanksDAO lObjEaRanksDAO = new DAO.EarringRanksDAO();

        #region lists
        List<EarringRanksT> lLstEarRnksT = null;
        EarringRanksT lObjEarringRanksT = null;
        List<int> lLstRowCodes = null;
        List<EarringRanksT> lLstDeletedRows = null;
        #endregion

        #region variables
        string lStrBaseEntry = "";
        string lStrActivePrefix = "";
        string lStrValue = "";
        int lIntCertHeadsCounter = 0;
        int lIntDocEntry = 0;
        int lIntHeadsInCertificate = 0;
        #endregion

        public mFormEarringRanks(string pStrBaseEntry)
        {
            this.lStrBaseEntry = pStrBaseEntry;
            this.lIntDocEntry = lObjEaRanksDAO.GetDocEntry(lStrBaseEntry);
            this.lIntHeadsInCertificate = lObjEaRanksDAO.GetTotalCertHeads(lIntDocEntry);
            LoadFromXml("mFrmEarringRanks.xml", "mFrmEarringRanks");
            SetPrefix();

        }

        #region Initialize Modal Form
        private void SetPrefix()
        {
            lStrActivePrefix = lObjEaRanksDAO.GetPrevPrefix();

            lObjETActivePrefix.Value = "SON" + lStrActivePrefix;

        }

        private void LoadFromXml(string FileName, string FormName)
        {
            System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
            //string sPath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]).ToString();
            string sPath = PathUtilities.GetCurrent("XmlForms");

            oXmlDoc.Load(sPath + "\\" + FileName);

            SAPbouiCOM.FormCreationParams creationPackage = (SAPbouiCOM.FormCreationParams)Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            creationPackage.XmlData = oXmlDoc.InnerXml;

            if (FormName.Equals("mFrmEarringRanks"))
            {
                //UGRS.AddOn.Cuarentenarias.Utils.utils.FormExists(FormName);

                creationPackage.UniqueID = FormName;
                creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
                creationPackage.FormType = "mFrmEarringRanks";
                //creationPackage.

                lObjRanksForm = Application.SBO_Application.Forms.AddEx(creationPackage);
                lObjRanksForm.Title = "Rangos de aretes";
                lObjRanksForm.Left = 400;
                lObjRanksForm.Top = 100;
                lObjRanksForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                lObjRanksForm.Visible = true;

                initFormXml();

            }
            else
            {
                //lObjModalForm.Select();
            }
        }

        private void initFormXml()
        {

            lObjRanksGrid = ((SAPbouiCOM.Grid)lObjRanksForm.Items.Item("GrdRanks").Specific);
            lObjETxtFrom = ((SAPbouiCOM.EditText)lObjRanksForm.Items.Item("TxtFrom").Specific);
            lObjETxtTo = ((SAPbouiCOM.EditText)lObjRanksForm.Items.Item("TxtTo").Specific);
            lObjETActivePrefix = ((SAPbouiCOM.EditText)lObjRanksForm.Items.Item("TxtPrefix").Specific);
            lLstEarRnksT = new List<EarringRanksT>();
            lLstDeletedRows = new List<EarringRanksT>();
            lLstRowCodes = new List<int>();

            
            if (!HasLines())
            {
               
                //int lIntTotalCertHeads = lObjEaRanksDAO.GetTotalCertHeads(lStrDocEntry);
                lIntCertHeadsCounter = lObjEaRanksDAO.GetTotalCertHeads(lIntDocEntry);
            }
            else
            {
                ////initGrid();
                LoadCounter();
            }

           
        }

        private void LoadCounter()
        {
            for (int i = 0; i < lObjRanksGrid.DataTable.Rows.Count; i++)
            {

                string lStrDesde = lObjRanksGrid.DataTable.Columns.Item("Desde").Cells.Item(i).Value.ToString().Substring(4);
                string lStrHasta = lObjRanksGrid.DataTable.Columns.Item("Hasta").Cells.Item(i).Value.ToString().Substring(4);

                int lIntHeadsPerLine = (Convert.ToInt32(lStrHasta) - Convert.ToInt32(lStrDesde)) + 1;

                lIntCertHeadsCounter += lIntHeadsPerLine;
            }
            if(lIntCertHeadsCounter == lIntHeadsInCertificate)
            {
                lIntCertHeadsCounter = 0;
            }
            else
            {
                lIntCertHeadsCounter = lIntHeadsInCertificate - lIntCertHeadsCounter;
            }
        }

        private void initGrid()
        {
            lLstEarRnksT = new List<EarringRanksT>();

            lObjRanksGrid.DataTable = lObjRanksForm.DataSources.DataTables.Item("DtRanks");
            lObjRanksGrid.Columns.Item("Code").Visible = false;
            lObjRanksGrid.Columns.Item("Desde").Editable = false;
            lObjRanksGrid.Columns.Item("Hasta").Editable = false;
            lObjRanksGrid.AutoResizeColumns();

        }
        #endregion

        #region principal functions
        public void AddRow()
        {
            string lStrFrom = lObjETActivePrefix.Value + lObjETxtFrom.Value;
            string lStrTo = lObjETActivePrefix.Value + lObjETxtTo.Value;
            int lastrow = 0;
            if (ValidFields())
            {
                lObjEarringRanksT = new EarringRanksT();

                lObjDtRanks.Rows.Add();
                lastrow = lObjDtRanks.Rows.Count - 1;
                lObjDtRanks.Columns.Item("Desde").Cells.Item(lastrow).Value = lStrFrom;
                lObjDtRanks.Columns.Item("Hasta").Cells.Item(lastrow).Value = lStrTo;

                lObjEarringRanksT.EarringFrom = lObjETxtFrom.Value;
                lObjEarringRanksT.EarringTo = lObjETxtTo.Value;
                lObjEarringRanksT.Prefix = lObjETActivePrefix.Value;

                lLstEarRnksT.Add(lObjEarringRanksT);

                ClearFields();
            }
        }



        public void SaveRanks()
        {
            CancelRowList();

            foreach (var item in lLstEarRnksT)
            {


                Tables.EarringRanksT lObjEarringsRanksT = new Tables.EarringRanksT();

                lObjEarringsRanksT.BaseEntry = lStrBaseEntry;
                lObjEarringsRanksT.EarringFrom = item.EarringFrom;
                lObjEarringsRanksT.EarringTo = item.EarringTo;
                lObjEarringsRanksT.Prefix = item.Prefix;
                lObjEarringsRanksT.Cancelled = "N";
                int lIntResult = lObjEarRankService.SaveRanks(lObjEarringsRanksT);
                if (lIntResult != 0)
                {
                    string lStrError = DIApplication.Company.GetLastErrorDescription();
                    Application.SBO_Application.StatusBar.SetText(lStrError
                          , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                }
            }



        }

        private void CancelRowList()
        {
            try
            {
                foreach (var item in lLstDeletedRows)
                {
                    EarringRanksT lObjEarringRanksT = new EarringRanksT();

                    lObjEarringRanksT.RowCode = item.RowCode;
                    lObjEarringRanksT.EarringFrom = item.EarringFrom;
                    lObjEarringRanksT.EarringTo = item.EarringTo;
                    lObjEarringRanksT.Prefix = item.Prefix;
                    lObjEarringRanksT.BaseEntry = item.BaseEntry;
                    lObjEarringRanksT.Cancelled = "Y";

                    int lIntResult = lObjEarRankService.UpdateRanks(lObjEarringRanksT);
                    if (lIntResult != 0)
                    {
                        string lStrError = DIApplication.Company.GetLastErrorDescription();
                        Application.SBO_Application.StatusBar.SetText(lStrError
                              , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                    }
                }
            }
            catch
            {

            }
        }

        internal void CancelRow()
        {
            var lVarSelRows = lObjRanksGrid.Rows.SelectedRows;
            if (lVarSelRows.Count > 0)
            {
                int lIntSelRow = lVarSelRows.Item(0, SAPbouiCOM.BoOrderType.ot_RowOrder);

                string lStrEarringFrom = lObjRanksGrid.DataTable.Columns.Item("Desde").Cells.Item(lIntSelRow).Value.ToString().Substring(4);
                string lStrEarringTo = lObjRanksGrid.DataTable.Columns.Item("Hasta").Cells.Item(lIntSelRow).Value.ToString().Substring(4);
                int lIntRowCode = Convert.ToInt32(lObjRanksGrid.DataTable.Columns.Item("Code").Cells.Item(lIntSelRow).Value);
                if (lIntSelRow >= 0)
                {
                    if (Convert.ToInt32(lObjRanksGrid.DataTable.Columns.Item("Code").Cells.Item(lIntSelRow).Value) != 0)
                    {
                        //lLstRowCodes.Add(lIntRowCode);

                        lObjEarringRanksT = new EarringRanksT();
                        lObjEarringRanksT.RowCode = lIntRowCode.ToString();
                        lObjEarringRanksT.EarringFrom = lStrEarringFrom;
                        lObjEarringRanksT.EarringTo = lStrEarringTo;
                        lObjEarringRanksT.BaseEntry = lStrBaseEntry;
                        lObjEarringRanksT.Prefix = lObjETActivePrefix.Value;

                        lLstDeletedRows.Add(lObjEarringRanksT);

                        lObjRanksGrid.DataTable.Rows.Remove(lIntSelRow);


                        increaseHeadCounter(lStrEarringFrom,lStrEarringTo);
                    }
                    else
                    {
                        var lVarTempLst = lLstEarRnksT.Single(x => x.RowCode == null && x.EarringFrom == lStrEarringFrom && x.EarringTo == lStrEarringTo);
                        lLstEarRnksT.Remove(lVarTempLst);

                        lObjRanksGrid.DataTable.Rows.Remove(lIntSelRow);

                        increaseHeadCounter(lStrEarringFrom, lStrEarringTo);
                    }

                }


            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Seleccionar una línea"
         , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

        }

        private void increaseHeadCounter(string lStrEarringFrom, string lStrEarringTo)
        {
            int lIntHeads = (Convert.ToInt32(lStrEarringTo) - Convert.ToInt32(lStrEarringFrom) )+1;

            lIntCertHeadsCounter += lIntHeads;
        }


        #endregion

        #region Queries
        public bool HasLines()
        {

            bool lBoolValid = false;
            lObjRanksForm.DataSources.DataTables.Add("DtRanks");
            lObjDtRanks = lObjRanksForm.DataSources.DataTables.Item("DtRanks");


            string ss = lObjEaRanksDAO.GetLinesQuery(lStrBaseEntry);
            lObjRanksForm.DataSources.DataTables.Item("DtRanks").ExecuteQuery(lObjEaRanksDAO.GetLinesQuery(lStrBaseEntry));


            if (lObjRanksForm.DataSources.DataTables.Item("DtRanks").Rows.Count >= 1)
            {
                string y = lObjRanksForm.DataSources.DataTables.Item("DtRanks").Columns.Item("Desde").Cells.Item(0).Value.ToString();


                if (y == "")
                {
                    lObjDtRanks.Rows.Remove(0);
                    lBoolValid = false;
                }
                else
                {
                    lBoolValid = true;
                }
            }
            else
            {
                lObjDtRanks.Rows.Remove(0);
                lBoolValid = false;
            }


            initGrid();

            return lBoolValid;

        }




        #endregion

        #region validations
        private void ClearFields()
        {
            lObjETxtTo.Value = string.Empty;
            lObjETxtFrom.Value = string.Empty;
        }

        private bool ValidRanks(string lStrFrom, string lStrTo)
        {
            bool valid = false;

            if (lObjEaRanksDAO.CheckStoredRanks(lStrFrom, lStrTo))
            {
                valid = false;
            }
            else
            {
                valid = true;
            }

            if (checkDeletedList(lStrFrom, lStrTo))
            {
                valid = true;
            }


            return valid;
        }

        private bool checkDeletedList(string lStrFrom, string lStrTo)
        {
            bool lBoolValid = false;
            foreach (var item in lLstDeletedRows)
            {
                if(Convert.ToInt32(item.EarringFrom) < Convert.ToInt32(lStrFrom) || Convert.ToInt32(item.EarringTo) > Convert.ToInt32(lStrFrom))
                {
                    if(Convert.ToInt32(item.EarringFrom) < Convert.ToInt32(lStrTo) || Convert.ToInt32(item.EarringTo) > Convert.ToInt32(lStrTo))
                    {
                        lBoolValid = true;
                    }
                    else
                    {
                        lBoolValid = false;
                        break;
                    }
                }
                else
                {
                    lBoolValid = false;
                    break;
                }
            }
            return lBoolValid;
        }

        internal void ValidateOnlyNumbers(int pCharPressed, string pStrUID)
        {

            SAPbouiCOM.EditText lObjETOnlyNumbers = ((SAPbouiCOM.EditText)lObjRanksForm.Items.Item(pStrUID).Specific);
            string lStrIntToChar = (Convert.ToChar(pCharPressed)).ToString();

            if (lObjETOnlyNumbers.Value.Length <= 5 && pCharPressed != 32)
            {


                if (!Regex.IsMatch(lObjETOnlyNumbers.Value, "^-?\\d*(\\.\\d+)?$") || lStrIntToChar == "-")
                {
                    lObjETOnlyNumbers.Value = new string(lObjETOnlyNumbers.Value.Where(c => char.IsDigit(c)).ToArray());
                }
                else
                {
                    lStrValue = lObjETOnlyNumbers.Value;
                }

            }
            else
            {
                lObjETOnlyNumbers.Value = lStrValue;
            }


        }

        private bool ValidFields()
        {
            bool lBoolValid = false;
            if (lObjETxtFrom.Value != string.Empty && lObjETxtTo.Value != string.Empty)
            {
                int lIntFrom = Convert.ToInt32(lObjETxtFrom.Value);
                int lIntTo = Convert.ToInt32(lObjETxtTo.Value);

                if (lIntTo > lIntFrom)
                {
                    if (CheckGridValues(lIntFrom, lIntTo))
                    {
                        if (ValidRanks(lIntFrom.ToString(), lIntTo.ToString()))
                        {
                            if (CheckHeadQuantity())
                            {
                                lBoolValid = true;
                            }

                        }
                        else
                        {
                            Application.SBO_Application.StatusBar.SetText("Ya se registro un rango similar"
              , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            lBoolValid = false;
                        }
                    }
                    else
                    {
                        Application.SBO_Application.StatusBar.SetText("Los rangos superan la cantidad de cabezas"
                      , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        lBoolValid = false;
                    }
                }
                else
                {
                    Application.SBO_Application.StatusBar.SetText("Agregar rangos válidos"
                    , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    lBoolValid = false;
                }
            }
            return lBoolValid;
        }

        private bool CheckHeadQuantity()
        {
            bool valid = false;
            int lIntRankQuantity = (Convert.ToInt32(lObjETxtTo.Value) - Convert.ToInt32(lObjETxtFrom.Value))+1;

            if (lIntCertHeadsCounter >= (lIntRankQuantity))
            {
                lIntCertHeadsCounter -= lIntRankQuantity;
                valid = true;
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Supera el numero de cabezas por certificado"
                    , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                valid = false;
            }






            return valid;
        }

        private bool CheckGridValues(int lIntFrom, int lIntTo)
        {
            bool Valid = false;
            if (lObjRanksGrid.DataTable.Rows.Count >= 1)
            {
                for (int i = 0; i < lObjRanksGrid.Rows.Count; i++)
                {

                    string lStrDesde = lObjRanksGrid.DataTable.Columns.Item("Desde").Cells.Item(i).Value.ToString().Substring(4);
                    string lStrHasta = lObjRanksGrid.DataTable.Columns.Item("Hasta").Cells.Item(i).Value.ToString().Substring(4);

                    if (Convert.ToInt32(lStrDesde) > lIntFrom
                        || Convert.ToInt32(lStrHasta) < lIntFrom)
                    {
                        if (Convert.ToInt32(lStrDesde) > lIntTo
                            || Convert.ToInt32(lStrHasta) < lIntTo)
                        {
                            Valid = true;
                        }
                        else
                        {
                            Valid = false;
                        }
                    }
                    else
                    {
                        Valid = false;
                        break;
                    }
                }
            }
            else
            {
                Valid = true;
            }
            return Valid;
        }
        #endregion
    }
}
