using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
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
using UGRS.Core.Utility;

namespace UGRS.AddOn.Cuarentenarias
{
    /// <summary>
    /// Class MFormInspectionDetails
    /// </summary>
    /// <remarks>@author RCordova-2017/08/11</remarks>
    public class MFormInspectionDetails
    {
        #region SAP Items
        SAPbouiCOM.Form mObjModalForm;
        SAPbouiCOM.DataTable mDtInspD;
        SAPbouiCOM.Matrix mObjMtxInspDetails;
        SAPbouiCOM.EditText txtRE;
        SAPbouiCOM.EditText txtNP;
        SAPbouiCOM.ComboBox cmbType;
        SAPbouiCOM.EditText txtQuantity;
        SAPbouiCOM.EditText txtComments;
        public SAPbouiCOM.Button btnAdd;
        public SAPbouiCOM.Button btnOk;
        public SAPbouiCOM.Button btnMod;
        public SAPbouiCOM.Button btnDel;
        public SAPbouiCOM.Button btnCancel;
        SAPbouiCOM.DataTable mDtInspDetails;
        //SAPbobsCOM.Company mObjCompany;
        #endregion




        #region Objects
        InspectionDAO mObjinspectionDAO = new InspectionDAO();
        List<Inspeccion> mlstInspection = new List<Inspeccion>();
        InspectionDetailsService mObjInspDetailsService = new InspectionDetailsService();
        InspectionCheckListDAO mObjInspectionCheckListDAO = new InspectionCheckListDAO();
        utils Utils = new utils();
        #endregion

        int lIntHeadRejCounter = 0;
        int lIntHeadNPCounter = 0;

        DTO.InspectDetailDTO lObjInspDetDTO = new DTO.InspectDetailDTO();

        List<DTO.InspectDetailDTO> llstInspDetDTO = null;

        List<string> lLstRowCode = new List<string>();

        bool lBoolHasRecords = false;
        /// <summary>
        /// Contructor MFormInspectonDetails
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        public MFormInspectionDetails(String FileName, String FormName, List<Inspeccion> LstInspeccion)
        {
            mlstInspection = LstInspeccion;
            LoadFromXml(FileName, FormName);
        }

        /// <summary>
        /// Metodo LoadFromXml
        /// Metodo para cargar un Form a partir de un archivo XML
        /// <remarks>@author RCordova-2017/08/15</remarks>
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

            if (FormName.Equals("frmInspDet"))
            {

                if (!Utils.FormExists(FormName))
                {
                    creationPackage.UniqueID = FormName;
                    creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                    creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
                    creationPackage.FormType = "frmInspDet";
                    //creationPackage.

                    mObjModalForm = Application.SBO_Application.Forms.AddEx(creationPackage);
                    mObjModalForm.Title = "Detalles de Inspección";
                    mObjModalForm.Left = 400;
                    mObjModalForm.Top = 100;
                    mObjModalForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    mObjModalForm.Visible = true;
                    InitFormXml();

                }
                else
                {
                    mObjModalForm.Select();
                }
            }

        }

        /// <summary>
        /// Method initFormXml
        /// Initialize modal form 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/15</remarks>
        private void InitFormXml()
        {
            mObjModalForm.Freeze(true);
            SetItems();
            ConfigForm();
            SetValues();
            mObjModalForm.Freeze(false);
        }

        /// <summary>
        /// 
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void SetItems()
        {
            //mObjCompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();
            txtRE = ((SAPbouiCOM.EditText)mObjModalForm.Items.Item("txtREDet").Specific);
            txtNP = ((SAPbouiCOM.EditText)mObjModalForm.Items.Item("txtNPDet").Specific);
            cmbType = ((SAPbouiCOM.ComboBox)mObjModalForm.Items.Item("cmbType").Specific);
            txtQuantity = ((SAPbouiCOM.EditText)mObjModalForm.Items.Item("txtQuaD").Specific);
            txtComments = ((SAPbouiCOM.EditText)mObjModalForm.Items.Item("txtComD").Specific);
            btnAdd = ((SAPbouiCOM.Button)mObjModalForm.Items.Item("btnAddID").Specific);
            btnOk = ((SAPbouiCOM.Button)mObjModalForm.Items.Item("btnOkID").Specific);
            btnMod = ((SAPbouiCOM.Button)mObjModalForm.Items.Item("btnModD").Specific);
            //btnDel = ((SAPbouiCOM.Button)mObjModalForm.Items.Item("btnDel").Specific);
            btnCancel = ((SAPbouiCOM.Button)mObjModalForm.Items.Item("btnNoID").Specific);
            mObjMtxInspDetails = ((SAPbouiCOM.Matrix)mObjModalForm.Items.Item("MtxInspD").Specific);
            mObjModalForm.Items.Item("txtQuaD").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
            FillCmbType();
            InitMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FillCmbType()
        {

            //SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery(mObjinspectionDAO.GetMovTypeList());

            for (int i = 0; i < lObjRecordSet.RecordCount; i++)
            {
                cmbType.ValidValues.Add(lObjRecordSet.Fields.Item("Tipo").Value.ToString() + "-" + lObjRecordSet.Fields.Item("Name").Value.ToString(), lObjRecordSet.Fields.Item("Tipo").Value.ToString() + "-" + lObjRecordSet.Fields.Item("Name").Value.ToString());

                lObjRecordSet.MoveNext();
            }
            //cmbType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            cmbType.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            //cmbType.
            //cmbType.Selected.Description.ToString();

            MemoryUtility.ReleaseComObject(lObjRecordSet);


        }

        /// <summary>
        /// 
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void InitMatrix()
        {
            mObjModalForm.DataSources.DataTables.Add("DtInspectionD");
            mDtInspD = mObjModalForm.DataSources.DataTables.Item("DtInspectionD");

            mDtInspD.Columns.Add("ColType", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            mDtInspD.Columns.Add("ColQuan", SAPbouiCOM.BoFieldsType.ft_ShortNumber);
            mDtInspD.Columns.Add("ColComm", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);
            mDtInspD.Columns.Add("ColMvT", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric);

            mDtInspD.Rows.Add();
            FillDataSource();

        }

        /// <summary>
        /// Method FillDataSource
        /// 
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void FillDataSource()
        {
            string lStrQuery = mObjinspectionDAO.GetInspectionByID(Convert.ToInt16(mlstInspection[0].IdInspection));
            //SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery(lStrQuery);
            llstInspDetDTO = new List<DTO.InspectDetailDTO>();

            if (lObjRecordSet.RecordCount != 0)
            {

                //mObjModalForm.DataSources.DataTables.Item("DtInspectionD")
                mObjModalForm.DataSources.DataTables.Item("DtInspectionD")
               .ExecuteQuery(lStrQuery);
                for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                {
                    DetailsListWithDt(i);
                }
                UpdateConfig();
                LoadMatrix();
                lBoolHasRecords = true;

            }
            else
            {
                //btnMod.Item.Enabled = false;
                //mObjModalForm.DataSources.DataTables.Add("DtInspectionT");
                mDtInspDetails = mObjModalForm.DataSources.DataTables.Item("DtInspectionD");

                mObjMtxInspDetails.Columns.Item("ColType").DataBind.Bind("DtInspectionD", "ColType");
                mObjMtxInspDetails.Columns.Item("ColQuan").DataBind.Bind("DtInspectionD", "ColQuan");
                mObjMtxInspDetails.Columns.Item("ColComm").DataBind.Bind("DtInspectionD", "ColComm");
                mObjMtxInspDetails.Columns.Item("ColTypeM").DataBind.Bind("DtInspectionD", "ColMvT");

                mObjMtxInspDetails.LoadFromDataSource();


                lIntHeadRejCounter = (int)mlstInspection.Sum(x => x.RE);
                lIntHeadNPCounter = (int)mlstInspection.Sum(x=>x.NP);
            }

        }

        /// <summary>
        /// Method LoadMatrix
        /// 
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void LoadMatrix()
        {
            mObjMtxInspDetails.Columns.Item("ColType").DataBind.Bind("DtInspectionD", "Name");
            mObjMtxInspDetails.Columns.Item("ColQuan").DataBind.Bind("DtInspectionD", "Quantity");
            mObjMtxInspDetails.Columns.Item("ColComm").DataBind.Bind("DtInspectionD", "Comments");
            mObjMtxInspDetails.Columns.Item(4).DataBind.Bind("DtInspectionD", "U_Tipo");

            mObjMtxInspDetails.Columns.Item("ColTypeM").Visible = false;
            mObjMtxInspDetails.LoadFromDataSource();

        }

        /// <summary>
        /// Method SetValues
        /// Set TextBox values in inspection list
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void SetValues()
        {
            txtNP.Value = mlstInspection.Sum(x => x.NP).ToString();
            txtRE.Value = mlstInspection.Sum(x => x.RE).ToString();

            ValidateLines();
        }

        /// <summary>
        /// Method ConfigForm
        /// Configuration form control's
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// </summary>
        private void ConfigForm()
        {
            txtNP.Item.Enabled = false;
            txtRE.Item.Enabled = false;
            if (mObjMtxInspDetails.RowCount <= 0)
            {
                btnMod.Item.Enabled = false;
            }
            mObjMtxInspDetails.Columns.Item("ColTypeM").Visible = false;
            //mObjMtxInspDetails.Columns.Item(0).Visible = false;
        }

        /// <summary>
        /// Method AddRow
        /// Add row in matrix 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/15</remarks>
        public void AddRow()
        {
            if (txtQuantity.Value != string.Empty && cmbType.Value != string.Empty)
            {
                OnlyNumbers();
                if (ValidQuantity())
                {
                    mObjMtxInspDetails.AddRow();

                    ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item("ColType").Cells.Item(mObjMtxInspDetails.RowCount).Specific).Value = cmbType.Value.Substring(2);
                    ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item("ColQuan").Cells.Item(mObjMtxInspDetails.RowCount).Specific).Value = txtQuantity.Value;
                    ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item("ColComm").Cells.Item(mObjMtxInspDetails.RowCount).Specific).Value = txtComments.Value.ToString();
                    ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item("ColTypeM").Cells.Item(mObjMtxInspDetails.RowCount).Specific).Value = cmbType.Value.Substring(0, 1);

                    DetailsList(mObjMtxInspDetails.RowCount);

                    mObjMtxInspDetails.SetCellFocus(mObjMtxInspDetails.RowCount, 0);

                    mObjMtxInspDetails.SelectRow(mObjMtxInspDetails.RowCount, true, false);

                    Application.SBO_Application.StatusBar.SetText("Linea agregada correctamente"
                                , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    ClearFields();
                    if (mObjMtxInspDetails.RowCount > 0)
                    {
                        btnMod.Item.Enabled = true;
                        btnCancel.Item.Enabled = false;
                    }
                }
                else
                {

                }
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Agregue Tipo y Cantidad para continuar"
                               , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private void ValidateLines()
        {
            //bool valid = false;
            int differenceNP = 0;
            int differenceRJ = 0;

            string lStrType = "";

            string lStrMvtType = "";

            if (lBoolHasRecords)
            {

                for (int i = 1; i <= mObjMtxInspDetails.RowCount; i++)
                {
                    lStrType = ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item(1).Cells.Item(i).Specific).Value.ToString();
                    //lStrMvtType = mObjInspectionCheckListDAO.GetTypeCharByNameComplete(lStrType);
                    lStrMvtType = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColTypeM").Cells.Item(i).Specific)).Value;

                    if (lStrMvtType == "N")
                    {

                        double x = double.Parse(((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item(2).Cells.Item(i).Specific).Value);
                        differenceNP += (int)x;

                    }
                    else
                    {
                        double x = double.Parse(((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item(2).Cells.Item(i).Specific).Value);
                        differenceRJ += (int)x;
                    }
                }
                if (Convert.ToInt32(txtNP.Value) != differenceNP)
                {
                    differenceNP = Convert.ToInt32(txtNP.Value) - differenceNP;

                    lIntHeadNPCounter += differenceNP;

                }
                if (Convert.ToInt32(txtRE.Value) != differenceRJ)
                {
                    differenceRJ = Convert.ToInt32(txtRE.Value) - differenceRJ;

                    
                    lIntHeadRejCounter += differenceRJ;

                }

            }
            if (lIntHeadNPCounter > 0 || lIntHeadRejCounter > 0)
            {
                btnAdd.Item.Enabled = true;
                btnOk.Item.Enabled = true;
                btnOk.Item.Enabled = false;
                
            }
            //valid = ValidQuantity();



        }

        private void OnlyNumbers()
        {
            SAPbouiCOM.EditText lObjEditTxtOnlyNumbers = null;

            lObjEditTxtOnlyNumbers = (SAPbouiCOM.EditText)txtQuantity;

            lObjEditTxtOnlyNumbers.Value = Regex.Replace(lObjEditTxtOnlyNumbers.Value, @"[^0-9]", "");
        }

        private bool ValidQuantity()
        {
            bool valid = false;
            int TotalNp = Convert.ToInt32((txtNP.Value));
            int TotalRj = Convert.ToInt32((txtRE.Value));
            //string lStrMvtType = mObjInspectionCheckListDAO.GetTypeCharByName(cmbType.Value);
            string lStrMvtType = cmbType.Value.Substring(0,1);

            if (lStrMvtType == "N")
            {
                if (TotalNp >= Convert.ToInt32((txtQuantity.Value)) && Convert.ToInt64(txtQuantity.Value) > 0)
                {
                    
                    if (lIntHeadNPCounter >= Convert.ToInt64(txtQuantity.Value))
                    {
                        lIntHeadNPCounter -= Convert.ToInt32(txtQuantity.Value);
                        if (lIntHeadNPCounter >= 0)
                        {
                            valid = true;
                        }

                    }
                    else
                    {
                        valid = false;
                        Application.SBO_Application.StatusBar.SetText("Las cantidades agregadas superan a los No presentados"
                               , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                }
                else
                {
                    valid = false;
                    Application.SBO_Application.StatusBar.SetText("La cantidad no puede ser mayor a los No presentados"
                           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }


            }
            else if (TotalRj >= Convert.ToInt64(txtQuantity.Value) && Convert.ToInt64(txtQuantity.Value)>0)
            {
                if (lIntHeadRejCounter >= Convert.ToInt64(txtQuantity.Value))
                {
                    lIntHeadRejCounter -= Convert.ToInt32(txtQuantity.Value);

                    if (lIntHeadRejCounter >= 0)
                    {
                        valid = true;
                    }
                }
                else
                {
                    valid = false;
                    Application.SBO_Application.StatusBar.SetText("Las cantidades agregadas superan los Rechazos"
                           , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            else
            {
                valid = false;
                Application.SBO_Application.StatusBar.SetText("La cantidad no puede ser mayor a la cantidad de Rechazados"
                              , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

            if (lIntHeadNPCounter == 0 && lIntHeadRejCounter == 0)
            {
                btnAdd.Item.Enabled = false;
                btnMod.Item.Enabled = true;
                btnOk.Item.Enabled = true;
                btnCancel.Item.Enabled = false;
            }









            return valid;
        }



        /// <summary>
        /// Method ClearFields
        /// 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/15</remarks>
        private void ClearFields()
        {
            txtComments.Value = "";
            //cmbType.Value = "";
            txtQuantity.Value = "";
        }

        /// <summary>
        /// Method SaveInspectionDetails
        /// 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/15</remarks>
        /// <returns>true/false</returns>
        public bool SaveInspectionDetails()
        {
            double lDbQuantity = 0;
            bool lBoolResult = true;

            int lIntResult = 0;

            DeleteRowList();

            //DetailsList();

            for (int i = 1; i < mObjMtxInspDetails.RowCount + 1; i++)
            {

                if (llstInspDetDTO[i - 1].RowCode == null)
                {
            
                    InspectionDetails inspD = new InspectionDetails();
                    inspD.CodeInsp = mlstInspection[0].IdInspection;
                    inspD.ItemCode = mlstInspection[0].Type;
                    //inspD.MvtoType = Convert.ToInt64(((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColType").Cells.Item(i).Specific)).Value);
                    inspD.MvtoType = Convert.ToInt64(mObjInspectionCheckListDAO.GetTypeByType(((SAPbouiCOM.EditText)mObjMtxInspDetails
                        .Columns.Item(4).Cells.Item(i).Specific).Value, ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColType").Cells.Item(i).Specific)).Value));
                    lDbQuantity = double.Parse(((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColQuan").Cells.Item(i).Specific)).Value);
                    inspD.Quantity = (long)lDbQuantity;
                    inspD.Commentary = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColComm").Cells.Item(i).Specific)).Value;

                    lIntResult = mObjInspDetailsService.SaveInspectionDetails(inspD);
                    if (lIntResult != 0)
                    {
                        Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
                        lBoolResult = false;
                    }
                }

            }
            lBoolResult = true;



            return lBoolResult;


        }

        private void DetailsListWithDt(int lIntRow)
        {

            lObjInspDetDTO = new DTO.InspectDetailDTO();

            lObjInspDetDTO.RowCode = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(1, lIntRow).ToString());
            lObjInspDetDTO.Type = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(0, lIntRow).ToString());
            lObjInspDetDTO.Quantity = Convert.ToInt32(mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(6, lIntRow));
            lObjInspDetDTO.Commentary = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(5, lIntRow).ToString());
            lObjInspDetDTO.TypeMov = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(7, lIntRow).ToString());

            llstInspDetDTO.Add(lObjInspDetDTO);

        }

        private void DetailsList(int lIntRow)
        {

            lObjInspDetDTO = new DTO.InspectDetailDTO();


            lObjInspDetDTO.Type = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColType").Cells.Item(lIntRow).Specific)).Value.ToString();
            lObjInspDetDTO.Quantity = Convert.ToInt32(double.Parse(((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColQuan").Cells.Item(lIntRow).Specific)).Value.ToString()));
            lObjInspDetDTO.Commentary = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColComm").Cells.Item(lIntRow).Specific)).Value.ToString();
            lObjInspDetDTO.TypeMov = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColTypeM").Cells.Item(lIntRow).Specific)).Value.Substring(0,1);

            llstInspDetDTO.Add(lObjInspDetDTO);

        }
        private void DeleteRowList()
        {
            try
            {
                foreach (var item in lLstRowCode)
                {
                    var TempList = llstInspDetDTO.Single(x => x.RowCode == item.ToString());
                    llstInspDetDTO.Remove(TempList);
                    mObjInspDetailsService.DeleteInspectionDetails(item);
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
            }
        }

        /// <summary>
        /// Method ValidateQuantity
        /// Validate quantities (RE , NP, Total )
        /// </summary>
        /// <remarks>@author RCordova-2017/08/16</remarks>
        /// <returns>false/true</returns>
        private bool ValidateQuantity()
        {
            long TotalRe = Convert.ToInt64((txtRE.Value));
            long TotalNP = Convert.ToInt64((txtNP.Value));
            long SumTotal = 0;

            for (int i = 1; i < mObjMtxInspDetails.RowCount + 1; i++)
            {
                //string x = (((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColQuan").Cells.Item(i).Specific)).Value);
                //x = string.Concat(x.TakeWhile(c => c != '.'));
                double lDbQuantity = Convert.ToDouble(((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColQuan").Cells.Item(i).Specific)).Value);

                SumTotal += (long)lDbQuantity;
            }

            if (SumTotal == TotalRe)
            {
                return true;
            }
            else { return false; }

        }

        /// <summary>
        /// Method UpdateConfig
        /// 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/16</remarks>
        private void UpdateConfig()
        {
            btnMod.Item.Enabled = true;
            btnAdd.Item.Enabled = false;
            btnOk.Item.Enabled = false;
            mObjMtxInspDetails.Columns.Item(4).Visible = false;

        }

        /// <summary>
        /// Method DeleteInspectionDetails
        /// 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/17</remarks>
        /// <returns></returns>
        public bool DeleteInspectionDetails()
        {
            bool lBoolResult = true;
            try
            {
                for (int i = 0; i < mObjMtxInspDetails.RowCount; i++)
                {
                    string lStrRowCode = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue("Code", i).ToString());

                    mObjInspDetailsService.DeleteInspectionDetails(lStrRowCode);
                }
            }
            catch
            {
                InsertConfig();
            }

            return lBoolResult;
        }

        /// <summary>
        /// Method InsertConfig
        /// 
        /// </summary>
        /// <remarks>@author RCordova-2017/08/18</remarks>
        public void InsertConfig()
        {
            mDtInspD.Clear();
            mObjMtxInspDetails.Clear();
            btnAdd.Item.Enabled = true;
            //btnMod.Item.Enabled = false;
            btnOk.Item.Enabled = true;
            lIntHeadRejCounter = Convert.ToInt32(mlstInspection.Sum(x=>x.RE) + mlstInspection.Sum(x=>x.NP));

        }


        internal void DeleteRow()
        {
            int lIntSelectedRow = mObjMtxInspDetails.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);


            if (lIntSelectedRow > 0)
            {
                string lStrType = ((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item(1).Cells.Item(lIntSelectedRow).Specific).Value.ToString();
                double lDbQuantity = double.Parse(((SAPbouiCOM.EditText)mObjMtxInspDetails.Columns.Item(2).Cells.Item(lIntSelectedRow).Specific).Value);
                int lIntQntity = (int)lDbQuantity;

                string lStrQuery = mObjinspectionDAO.GetInspectionByID(Convert.ToInt16(mlstInspection[0].IdInspection));

                string lStrRowCode = GetRowCodeFromDatasource(lIntSelectedRow);
                //string lIntMvmType = mObjInspectionCheckListDAO.GetTypeCharByNameComplete(lStrType);
                string lIntMvmType = ((SAPbouiCOM.EditText)(mObjMtxInspDetails.Columns.Item("ColTypeM").Cells.Item(lIntSelectedRow).Specific)).Value;

                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {


                    if (lStrRowCode != string.Empty)
                    {

                        if (lIntMvmType == "N")
                        {
                            lIntHeadNPCounter += lIntQntity;
                        }
                        else
                        {
                            lIntHeadRejCounter += lIntQntity;
                        }

                        //mObjInspDetailsService.DeleteInspectionDetails(lStrRowCode);
                        lLstRowCode.Add((lStrRowCode));

                    }

                    mObjMtxInspDetails.DeleteRow(lIntSelectedRow);


                }
                else
                {

                    if (lIntMvmType == "N")
                    {
                        lIntHeadNPCounter += lIntQntity;
                    }
                    else
                    {
                        lIntHeadRejCounter += lIntQntity;
                    }

                    var TempList = llstInspDetDTO.Single(x => x.RowCode == null && x.Type == lStrType && x.Quantity == lIntQntity);
                    llstInspDetDTO.Remove(TempList);

                    mObjMtxInspDetails.DeleteRow(lIntSelectedRow);

                }

                MemoryUtility.ReleaseComObject(lObjRecordSet);

                if (btnAdd.Item.Enabled == false)
                {
                    btnAdd.Item.Enabled = true;
                    btnOk.Item.Enabled = false;

   
                }

                if (mObjMtxInspDetails.RowCount == 0)
                {
                    btnCancel.Item.Enabled = true;
                }
                //if (btnOk.Item.Enabled == false)
                //{
                //    btnOk.Item.Enabled = true;
                //}
                
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("Seleccionar una línea"
                   , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }


        }

        private string GetRowCodeFromDatasource(int lIntSelectedRow)
        {
            string lStrRowCode = "";

            try
            {
                lStrRowCode = (mObjModalForm.DataSources.DataTables.Item("DtInspectionD").GetValue(1, lIntSelectedRow - 1).ToString());
            }
            catch (Exception)
            {
                lStrRowCode = string.Empty;
            }

            return lStrRowCode;
        }


    }
}
