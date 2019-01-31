using System;
using System.Collections.Generic;
using System.Xml;
using SAPbouiCOM.Framework;
using UGRS.Core.Utility;
using UGRS.AddOn.Cuarentenarias.Services;
using UGRS.AddOn.Cuarentenarias.Tables;
using UGRS.Core.SDK.DI;
using UGRS.AddOn.Cuarentenarias.DAO;



namespace UGRS.AddOn.Cuarentenarias
{
    [FormAttribute("UGRS.AddOn.Cuarentenarias.Form1", "Forms/frmChkIns.b1f")]
    class frmChkIns : UserFormBase
    {
        #region SAP Obj
        private SAPbouiCOM.Button lObjButtonOk;
        private SAPbouiCOM.EditText lObjEdTxtDate;
        private SAPbouiCOM.StaticText lObjStTxtOK;
        private SAPbouiCOM.Matrix lObjMatrix;
        private SAPbouiCOM.Button lObjButtonFind;
        #endregion

        SAPbobsCOM.Company lObjCompany = null;
        SAPbobsCOM.CompanyService lObjCompanyService = null;
        SAPbobsCOM.BatchNumberDetailsService lObjBatchNumbersService;
        SAPbobsCOM.BatchNumberDetailParams lObjBatchNumberDetailParams = null;
        SAPbobsCOM.BatchNumberDetail lObjBatchNumberDetail = null;
        
        private SAPbouiCOM.StaticText StaticText0;

        int SeriesNumber = 0;
        string SeriesName = "";
        string lStrPrincipalWhs = "";
        

        int lIntUserSignature = DIApplication.Company.UserSignature;

        Menu pObjMenu = new Menu();


        InspectionCheckListDAO mObjInspCheckList = new InspectionCheckListDAO();

        string InspDate = "";

        public frmChkIns()
        {
            LoadEvents();
            SetUserSeries();
            lStrPrincipalWhs = mObjInspCheckList.GetPrincipalWhs(lIntUserSignature);
            lObjEdTxtDate.Value = mObjInspCheckList.GetServerDate();
            LogUtility.WriteInfo("Modulo Listado de Inspección Iniciado");
            
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

        #region Default Initialize
        public override void OnInitializeComponent()
        {
            //this.lObjCompany = ((SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany());
            //   this.lObjCompany = ((SAPbobsCOM.Company)(typeof(SAPbouiCOM.Framework.Application).Application.Company.GetDICompany()));
            this.lObjButtonOk = ((SAPbouiCOM.Button)(this.GetItem("btnOkChk").Specific));
            this.lObjEdTxtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDateIns").Specific));
            this.lObjStTxtOK = ((SAPbouiCOM.StaticText)(this.GetItem("lbl").Specific));
            this.lObjMatrix = ((SAPbouiCOM.Matrix)(this.GetItem("MtxChkIns").Specific));
            this.lObjButtonFind = ((SAPbouiCOM.Button)(this.GetItem("btnFindIns").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.lObjMatrix.AutoResizeColumns();
            //   MemoryUtility.ReleaseComObject();
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("Item_0").Specific));
            FormConfig();
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
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>

        #endregion

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
                                if (pVal.ItemUID.Equals("btnFindIns"))
                                {
                                    if (lObjEdTxtDate.Value != string.Empty)
                                    {
                                        LoadMatrix();
                                    }
                                }
                                if (pVal.ItemUID.Equals("btnOkChk"))
                                {
                                    ProgramInspection();
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                pObjMenu.pArrTypeCount[0] = 0;
                                pObjMenu.pArrTypeEx[0] = "";
                                UnLoadEvents();
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
                //LogService.WriteError(ex.Message);
            }

        }

        /// <summary>
        /// /
        /// </summary>
        private void ProgramInspection()
        {
            InspDate = lObjEdTxtDate.Value.ToString();
            try
            {
                for (int i = 1; i <= lObjMatrix.RowCount; i++)
                {

                    if (((dynamic)lObjMatrix.Columns.Item(1).Cells.Item(i).Specific).Checked)
                    {
                        if (CheckStatus(i))
                        {
                            SaveInspeccion(i);
                        }
                        else
                        {
                            return;
                        }
                        //ChangeStatus(i);
                    }
                }

                LoadMatrix();


            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
                //LogService.WriteError(ex.Message);
             
            }



        }

        private bool CheckStatus(int lIntRow)
        {

            if (((SAPbouiCOM.EditText)(lObjMatrix.Columns.Item(6).Cells.Item(lIntRow).Specific)).Value == "Sin Asignar")
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
        /// </summary>
        /// <param name="lIntRow"></param>
        private void SaveInspeccion(int lIntRow)
        {
            InspeccionService inspService = new InspeccionService();

            if (inspService.SaveInspeccion(GetObjInspT(lIntRow)) != 0)
            {
                Application.SBO_Application.MessageBox(DIApplication.Company.GetLastErrorDescription());
            }
            else
            {
                Application.SBO_Application.StatusBar.SetText("La inspeccion fue guardada correctamente"
       , SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lIntRow"></param>
        /// <returns></returns>
        private InspeccionT GetObjInspT(int lIntRow)
        {
            InspeccionT inspT = new InspeccionT();
            inspT.DateInsp = DateTime.Now;
            inspT.DateSys = DateTime.Now;
            inspT.Series = GetUserSerialNumber();
            inspT.Cancel = "N";
            //inspT.User = lObjCompany.UserSignature;
            inspT.User = lIntUserSignature;
            inspT.CardCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("CardCode", lIntRow - 1);
            inspT.WhsCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("WhsCode", lIntRow - 1);
            inspT.Quantity = Convert.ToInt64(this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("Cantidad", lIntRow - 1).ToString());
            inspT.Classification = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("ItemCode", lIntRow - 1);
            inspT.IDInsp = 0;
            inspT.DocEntryIM = "";
            inspT.DocEntryIU = "";
            inspT.CheckInsp = "N";

            return inspT;
        }

        /*
        private void ChangeStatus(int lIntrow)
        {
            //initialize a Batch service to update it
            lObjCompanyService = lObjCompany.GetCompanyService();
            lObjBatchNumbersService = (SAPbobsCOM.BatchNumberDetailsService)lObjCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.BatchNumberDetailsService);
            lObjBatchNumberDetailParams = (SAPbobsCOM.BatchNumberDetailParams)lObjBatchNumbersService.GetDataInterface(SAPbobsCOM.BatchNumberDetailsServiceDataInterfaces.bndsBatchNumberDetailParams);

            try
            {
                //Query to obtain AbsEntry(Key)
                SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)lObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string lstrItemCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("ItmCode", lIntrow - 1);
                string lStrCode = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("BtchNumb", lIntrow - 1);
                string lStrCorral = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix").GetValue("Corralcode", lIntrow - 1);
                string query = (string.Format(@"SELECT DISTINCT
                T0.AbsEntry
                FROM OBTN T0
                INNER JOIN OIBT T1 ON t1.BatchNum = t0.DistNumber and t1.ItemCode = t0.ItemCode
                    
                WHERE T0.ItemCode = '{0}'
                AND t0.DistNumber = '{1}'
                AND T1.WhsCode = '{2}'", lstrItemCode, lStrCode, lStrCorral));

                lObjRecordSet.DoQuery(query);

                int lIntDocentry = (int)lObjRecordSet.Fields.Item("AbsEntry").Value;

                //Setting Parameters
                lObjBatchNumberDetailParams.DocEntry = lIntDocentry;
                //Get the Detail by Key(parameters)
                lObjBatchNumberDetail = lObjBatchNumbersService.Get(lObjBatchNumberDetailParams);

                lObjBatchNumberDetail.UserFields.Item("U_CU_Status").Value = "P";

                lObjBatchNumbersService.Update(lObjBatchNumberDetail);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(lObjBatchNumberDetail);

                lObjMatrix.LoadFromDataSource();


            }
            catch (Exception er)
            {

            }
            finally
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(lObjCompanyService);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(lObjBatchNumbersService);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(lObjBatchNumberDetailParams);

            }



        }
        */

        /// <summary>
        /// 
        /// </summary>
        private void FillDataSource()
        {

            this.UIAPIRawForm.DataSources.DataTables.Item("DTMatrix")
            .ExecuteQuery(mObjInspCheckList.GetInspectionCheckList(lObjEdTxtDate.Value, lStrPrincipalWhs));
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadMatrix()
        {
            FillDataSource();


            lObjMatrix.Columns.Item("Col_Check").DataBind.Bind("DTMatrix", "Chk");
            //lObjMatrix.Columns.Item("Col_Folio").DataBind.Bind("DTMatrix", "Folio");
            lObjMatrix.Columns.Item("Col_Name").DataBind.Bind("DTMatrix", "CardName");
            lObjMatrix.Columns.Item("Col_Type").DataBind.Bind("DTMatrix", "ItemName");
            lObjMatrix.Columns.Item("Col_Corral").DataBind.Bind("DTMatrix", "WhsName");
            lObjMatrix.Columns.Item("Col_Heads").DataBind.Bind("DTMatrix", "Cantidad");
            lObjMatrix.Columns.Item("Col_Status").DataBind.Bind("DTMatrix", "Estatus");

            lObjMatrix.LoadFromDataSource();
            lObjMatrix.AutoResizeColumns();


        }

        /// <summary>
        /// GetUserSerialNumber()
        /// Metodo para obtener el numero de serie del usuario conectado en SAP
        /// </summary>
        /// <returns></returns>
        private int GetUserSerialNumber()
        {
            //int UserSignature = lObjCompany.UserSignature;
            int UserSignature = lIntUserSignature;
            int SerialNumber = 0;


            //SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)lObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery("SELECT t0.series,SeriesName FROM NNM1 t0 INNER JOIN NNM2 t1 ON t0.Series = t1.Series WHERE t1.UserSign = " + UserSignature + " AND t0.ObjectCode=59");

            if (lObjRecordSet.RecordCount == 1)
            {
                SerialNumber = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
            }

            MemoryUtility.ReleaseComObject(lObjRecordSet);
            return SerialNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetUserSeries()
        {
            int UserSignature = lIntUserSignature;
            //int UserSignature = lIntUserSignature;

            //SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)lObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            lObjRecordSet.DoQuery("SELECT t0.series,SeriesName FROM NNM1 t0 INNER JOIN NNM2 t1 ON t0.Series = t1.Series WHERE t1.UserSign = " + UserSignature + " AND t0.ObjectCode=59");

            if (lObjRecordSet.RecordCount == 1)
            {
                SeriesNumber = Convert.ToInt32(lObjRecordSet.Fields.Item(0).Value.ToString());
                SeriesName = lObjRecordSet.Fields.Item(1).Value.ToString();
                EditText0.Value = SeriesName;
            }

            MemoryUtility.ReleaseComObject(lObjRecordSet);
        }

        /// <summary>
        /// 
        /// </summary>
        private void FormConfig()
        {
            //EditText
            EditText0.Item.Enabled = false;

            //Matrix
            //lObjMatrix.Columns.Item("Col_Folio").Editable = false;
            lObjMatrix.Columns.Item("Col_Name").Editable = false;
            lObjMatrix.Columns.Item("Col_Type").Editable = false;
            lObjMatrix.Columns.Item("Col_Corral").Editable = false;
            lObjMatrix.Columns.Item("Col_Heads").Editable = false;
            lObjMatrix.Columns.Item("Col_Status").Editable = false;
        }

        private SAPbouiCOM.EditText EditText0;

    }
}