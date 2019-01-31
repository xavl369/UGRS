using System;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI.ProgressBar;
using UGRS.AddOn.FoodProduction.UI;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using SAPbouiCOM;
using UGRS.AddOn.FoodProduction.DTO;
using UGRS.AddOn.FoodProduction.Services;
    


namespace UGRS.AddOn.FoodProduction.Forms
{
    [FormAttribute("UGRS.PlantaAlimentos.Forms.ReceptionForm", "Forms/ReceptionForm.b1f")]
    class ReceptionForm : UserFormBase
    {
        #region Attributes
        private SAPbobsCOM.Company mObjCompany = null;
        private SAPbobsCOM.StockTransfer mObjStockTransfer = null;
        private SAPbobsCOM.StockTransfer mObjTransferRequest = null;
        ReceptionTransferDAO mObjReceptionTransferDAO = new ReceptionTransferDAO();   
        ReceptionTransferService mObjReceptionTransferService = new ReceptionTransferService();
        private ProgressBarManager mObjProgressBar = null;
        #endregion

        #region variables
        string mIntId = "";
        string mStrWhsCode = "";        
        #endregion

        #region Constructor
        public ReceptionForm()        
        {
            mObjCompany = DIApplication.Company;
            if (mObjCompany.Connected)
            {
                //SAPbobsCOM.StockTransfer mObjoInventoryTransferRequest = (SAPbobsCOM.StockTransfer)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);
            }
        }
        #endregion

        #region Initialize
        public override void OnInitializeComponent()
        {
            UIAPIRawForm.Freeze(true);
            this.mtxHeader = ((SAPbouiCOM.Matrix)(this.GetItem("mtxHeader").Specific));
            this.mtxDetail = ((SAPbouiCOM.Matrix)(this.GetItem("mtxDetail").Specific));
            this.cmdAccept = ((SAPbouiCOM.Button)(this.GetItem("cmdAccept").Specific));
            this.cmdAccept.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.cmdAccept_ClickBefore);
            this.cmdReturn = ((SAPbouiCOM.Button)(this.GetItem("cmdReturn").Specific));
            this.cmdReturn.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.cmdReturn_ClickBefore);
            this.lblComents = ((SAPbouiCOM.StaticText)(this.GetItem("lblComents").Specific));
            this.txtComents = ((SAPbouiCOM.EditText)(this.GetItem("txtComents").Specific));
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("btnCancel").Specific));
            this.btnCancel.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCancel_ClickBefore);
            this.LoadApplicationEvents();

            this.OnCustomInitialize();
            UIAPIRawForm.Freeze(false);
        }        

        public override void OnInitializeFormEvents()
        {
        }      

        private void OnCustomInitialize()
        {
            mStrWhsCode = this.GetWhsCode();  // <--only for once            
            bool lBooResultRow = this.LoadMtxHeader(mStrWhsCode);            
            if (lBooResultRow)
            {
                mIntId = ((SAPbouiCOM.EditText)mtxHeader.GetCellSpecific(1, 1)).Value;
                mtxHeader.SelectRow(1, true, false);
                this.LoadMtxDetail(mIntId);
                UIControlsEnabled(true);
            }
            else
            {
                UIControlsEnabled(false);
            }            
        }     
        #endregion

        #region Application
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
                            case BoEventTypes.et_FORM_CLOSE:
                                UnloadApplicationEvents();
                                break;

                            case BoEventTypes.et_CLICK:
                                if (pVal.ItemUID == "mtxHeader" && mtxHeader.RowCount > 0 && pVal.Row > 0)
                                {
                                    UIAPIRawForm.Freeze(true);
                                    mtxHeader.SelectRow(pVal.Row, true, false);
                                    mIntId = ((SAPbouiCOM.EditText)mtxHeader.GetCellSpecific(1, pVal.Row)).Value;
                                    this.LoadMtxDetail(mIntId);
                                    UIAPIRawForm.Freeze(false);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
            }
        }
        #endregion

        #region Eventos
        private void cmdAccept_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;   
            if (this.cmdAccept.Item.Enabled)
            {         
                CreateStockTransfer();   //Crear Documento de Transferencia            
            }            
        }
        
        private void cmdReturn_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.cmdReturn.Item.Enabled)
            {
                CreateTransferRequest(); //Crear Documento de Solicitud de Traslado         
            }            
        }
        
        private void btnCancel_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            UIAPIRawForm.Close();
        }
        
        public void LoadApplicationEvents()
        {
            UIApplication.GetApplication().ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        
        public void UnloadApplicationEvents()
        {
            UIApplication.GetApplication().ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        #region Metodos
        private void UIControlsEnabled(bool pBooEnabled)
        {
            this.txtComents.Active = true;            
            this.cmdAccept.Item.Enabled = pBooEnabled;
            this.cmdReturn.Item.Enabled = pBooEnabled;
        }

        private void ClearTxt()
        { 
            this.txtComents.Value = "";
        }

        private void CreateTransferRequest()
        {
            UIAPIRawForm.Freeze(true);
            int result = -1;
            try
            {
                mObjTransferRequest = (SAPbobsCOM.StockTransfer)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);                
                foreach (TransferHeader_DTO lObjTransferHeader_DTO in mObjReceptionTransferService.GetTransferHeader(GetWhsCode(), mIntId))
                {
                    mObjTransferRequest = PopulateTransferRequest(lObjTransferHeader_DTO);
                    int i = 0;
                    foreach (TransferDetail_DTO lObjTransferDetail_DTO in mObjReceptionTransferService.GetTransferDetail(mIntId))
                    {
                        mObjTransferRequest = PopulateTransferRequestDetail(lObjTransferDetail_DTO, i);
                        mObjTransferRequest.Lines.Add();
                        ++i;
                    }
                    result = mObjTransferRequest.Add();                    
                    if (result == 0)
                    {
                        //Se creo el documento solicitud de transferencia
                        //TicketLogService.WriteSuccess("[CrearTransferencia] DocNum:" + lObjStockTrasnfer.DocNum);
                        UIAPIRawForm.Freeze(false);
                        UIApplication.ShowMessageBox(string.Format("Solicitud de Transferencia realizada correctamente {0}", mIntId));
                        UIAPIRawForm.Freeze(true);
                        bool lBooResultRow = this.LoadMtxHeader(mStrWhsCode);
                        if (lBooResultRow)
                        {
                            mIntId = ((SAPbouiCOM.EditText)mtxHeader.GetCellSpecific(1, 1)).Value;
                            mtxHeader.SelectRow(1, true, false);
                            this.LoadMtxDetail(mIntId);
                            UIControlsEnabled(true);                            
                        }
                        else
                        {
                            UIControlsEnabled(false);
                        }
                        ClearTxt();
                                                
                    }
                    if (result != 0)
                    {
                        UIAPIRawForm.Freeze(false);
                        UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                        //TicketLogService.WriteError("[CrearTransferencia]" + DIApplication.Company.GetLastErrorDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(string.Format(ex.Message, ex.Message));
            }
            finally
            {
                UIAPIRawForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjStockTransfer);                
            }          
        }

        private void CreateStockTransfer()
        {
            UIAPIRawForm.Freeze(true);
            int result = -1;
            try
            {
                mObjStockTransfer = (SAPbobsCOM.StockTransfer)mObjCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                foreach (TransferHeader_DTO lObjTransferHeader_DTO in mObjReceptionTransferService.GetTransferHeader(GetWhsCode(), mIntId))
                {                    
                    mObjStockTransfer = PopulateStockTransfer(lObjTransferHeader_DTO);
                    int i = 0;
                    foreach (TransferDetail_DTO lObjTransferDetail_DTO in mObjReceptionTransferService.GetTransferDetail(mIntId))
                    {                        
                        mObjStockTransfer = PopulateStockTransferDetail(lObjTransferDetail_DTO,i);
                        mObjStockTransfer.Lines.Add();
                        ++i;
                    }
                    result = mObjStockTransfer.Add();                    
                        if (result == 0)
                        {
                            //TicketLogService.WriteSuccess("[CrearTransferencia] DocNum:" + lObjStockTrasnfer.DocNum); 
                            UIAPIRawForm.Freeze(false);
                            UIApplication.ShowMessageBox(string.Format("Transferencia realizada correctamente"));
                            UIAPIRawForm.Freeze(true);
                            bool lBooResultRow = this.LoadMtxHeader(mStrWhsCode);
                            if (lBooResultRow)
                            {
                                mIntId = ((SAPbouiCOM.EditText)mtxHeader.GetCellSpecific(1, 1)).Value;
                                mtxHeader.SelectRow(1, true, false);
                                this.LoadMtxDetail(mIntId);
                                UIControlsEnabled(true);
                            }
                            else
                            {
                                UIControlsEnabled(true);
                            }
                            ClearTxt();
                        }                            
                        if (result != 0)
                        {
                            UIAPIRawForm.Freeze(false);
                            UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                            //TicketLogService.WriteError("[CrearTransferencia]" + DIApplication.Company.GetLastErrorDescription());
                        }
                }
            }
            catch (Exception ex)
            {
                UIAPIRawForm.Freeze(false);
                UIApplication.ShowError(string.Format(ex.Message, ex.Message));
            }
            finally
            {
                UIAPIRawForm.Freeze(false);
                MemoryUtility.ReleaseComObject(mObjStockTransfer);
            }            
        }
        
        private SAPbobsCOM.StockTransfer PopulateTransferRequest(TransferHeader_DTO pObjTransferHeader_DTO)
        {                   
            mObjTransferRequest.Series = this.GetSeriesCode("1250000001");                                                      //Series
            mObjTransferRequest.DocDate = DateTime.Today;                                                                       //fechasubasta
            mObjTransferRequest.DueDate = DateTime.Today;
            mObjTransferRequest.TaxDate = DateTime.Today;                                                                       //fecha de documento            
            mObjTransferRequest.FromWarehouse = pObjTransferHeader_DTO.ToWhsCode; // .Filler;                                                  //Almacen Destino            
            //mObjTransferRequest.ToWarehouse = "PLHE";                                                                           //De almacen
            mObjTransferRequest.JournalMemo = pObjTransferHeader_DTO.JrnlMemo;                                                  //Comentarios
            mObjTransferRequest.Comments = "Se rechazó la transferencia con folio " + pObjTransferHeader_DTO.DocNum + ".  " + this.txtComents.Value.ToString().TrimEnd();                                          //Comentarios            
            mObjTransferRequest.UserFields.Fields.Item("U_MQ_OrigenFol").Value = pObjTransferHeader_DTO.DocNum;                 //Campos definidos por el usuario
            mObjTransferRequest.UserFields.Fields.Item("U_PL_WhsReq").Value = mStrWhsCode;                                      //Campos definidos por el usuario
            return mObjTransferRequest;
        }
        
        private SAPbobsCOM.StockTransfer PopulateTransferRequestDetail(TransferDetail_DTO pObjTransferDetail_DTO, int pIntLineNum)
        {            
            mObjTransferRequest.Lines.ItemCode = pObjTransferDetail_DTO.ItemCode;                                                   //
            mObjTransferRequest.Lines.Quantity = pObjTransferDetail_DTO.Quantity;                                                   //   
            mObjTransferRequest.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pObjTransferDetail_DTO.U_GLO_BagsBales;     //Campos definidos por el usuario
            mObjTransferRequest.Lines.FromWarehouseCode = pObjTransferDetail_DTO.WhsCode;
            mObjTransferRequest.Lines.WarehouseCode = pObjTransferDetail_DTO.FromWhsCode;
            return mObjTransferRequest;
        }
        
        private SAPbobsCOM.StockTransfer PopulateStockTransfer(TransferHeader_DTO pObjTransferHeader_DTO)
        {
            mObjStockTransfer.Series = this.GetSeriesCode("67");                                                    //Series
            mObjStockTransfer.DocDate = DateTime.Today;                                                             //fechasubasta
            mObjStockTransfer.TaxDate = DateTime.Today;                                                             //fecha de documento
            mObjStockTransfer.FromWarehouse = pObjTransferHeader_DTO.ToWhsCode;                                     //Almacen Destino            
            mObjStockTransfer.ToWarehouse = mStrWhsCode;                                                            //De almacen
            mObjStockTransfer.JournalMemo = pObjTransferHeader_DTO.JrnlMemo;                                        //Comentarios
            mObjStockTransfer.Comments = "Se acepto la transferencia con folio " + pObjTransferHeader_DTO.DocNum + ".  " + this.txtComents.Value.ToString().TrimEnd();                                //Comentarios            
            mObjStockTransfer.UserFields.Fields.Item("U_MQ_OrigenFol").Value = pObjTransferHeader_DTO.DocNum;       //Campos definidos por el usuario
            return mObjStockTransfer;
        }        
        
        private SAPbobsCOM.StockTransfer PopulateStockTransferDetail(TransferDetail_DTO pObjTransferDetail_DTO, int pIntLineNum)
        {
            mObjStockTransfer.Lines.ItemCode = pObjTransferDetail_DTO.ItemCode;                                                 //
            mObjStockTransfer.Lines.Quantity = pObjTransferDetail_DTO.Quantity;                                                 //   
            mObjStockTransfer.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = pObjTransferDetail_DTO.U_GLO_BagsBales;   //Campos definidos por el usuario
            mObjStockTransfer.Lines.FromWarehouseCode = pObjTransferDetail_DTO.WhsCode;
            mObjStockTransfer.Lines.WarehouseCode = mStrWhsCode;
            return mObjStockTransfer;
        }
        
        private int GetSeriesCode(string pStrObjectType)
        {
            string lStrUserSignature = DIApplication.Company.UserSignature.ToString();
            int lStrResult = mObjReceptionTransferDAO.getSeries(lStrUserSignature, pStrObjectType);
            return lStrResult;
        }

        private string GetWhsCode()
        {
            string lStrUserSignature = DIApplication.Company.UserSignature.ToString();
            string lStrResult = mObjReceptionTransferDAO.getWareHouse(lStrUserSignature);
            return lStrResult;
        }

        private bool LoadMtxHeader(string pStrWhsRequesting)
        {
            bool lBolVerify = false;
            try
            {
                string lStrQuery = mObjReceptionTransferDAO.SearchWhsTransit(pStrWhsRequesting);
                if (lStrQuery != "")
                {
                    this.UIAPIRawForm.DataSources.DataTables.Item("DT_Header").ExecuteQuery(lStrQuery);                    
                    if (this.UIAPIRawForm.DataSources.DataTables.Item("DT_Header").IsEmpty)
                    {                        
                        mtxHeader.Clear();                        
                        UIApplication.ShowSuccess(string.Format("No hay trasferencias pendientes para {0}", mStrWhsCode));
                        lBolVerify = false;
                    }
                    else
                    {
                        mtxHeader.Columns.Item("Col_0").DataBind.Bind("DT_Header", "DocEntry");
                        mtxHeader.Columns.Item("Col_1").DataBind.Bind("DT_Header", "DocNum");
                        mtxHeader.Columns.Item("Col_2").DataBind.Bind("DT_Header", "DocDate");
                        mtxHeader.Columns.Item("Col_3").DataBind.Bind("DT_Header", "JrnlMemo");
                        mtxHeader.Columns.Item("Col_4").DataBind.Bind("DT_Header", "U_PL_WhsReq");  //'almacen solicita'
                        mtxHeader.Columns.Item("Col_5").DataBind.Bind("DT_Header", "Filler");       //'de alamcen'
                        mtxHeader.Columns.Item("Col_6").DataBind.Bind("DT_Header", "ToWhsCode");    //'almacen destino'
                        mtxHeader.LoadFromDataSource();
                        lBolVerify = true;
                    }                    
                }                
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
            }
            finally
            {
                //MemoryUtility.ReleaseComObject(lObjRecordSet);
                //this.UIAPIRawForm.Freeze(false);
            }
            return lBolVerify;
        }

        private void LoadMtxDetail(string pStrDocEntry)
        {
            try
            {
                string lStrQuery = mObjReceptionTransferDAO.SearchWhsTransitDetail(pStrDocEntry);
                if (lStrQuery != "")
                {
                    this.UIAPIRawForm.DataSources.DataTables.Item("DT_Detail").ExecuteQuery(lStrQuery);
                    if (this.UIAPIRawForm.DataSources.DataTables.Item("DT_Detail").IsEmpty)
                    {
                        mtxDetail.Clear();
                        UIApplication.ShowWarning(string.Format("No se encontró el detalle de la trasferencia {0}", pStrDocEntry));
                    }                        
                    else
                    {
                        mtxDetail.Columns.Item("Col_0").DataBind.Bind("DT_Detail", "DocEntry");
                        mtxDetail.Columns.Item("Col_1").DataBind.Bind("DT_Detail", "ItemCode");
                        mtxDetail.Columns.Item("Col_2").DataBind.Bind("DT_Detail", "Dscription");
                        mtxDetail.Columns.Item("Col_3").DataBind.Bind("DT_Detail", "Quantity");
                        mtxDetail.Columns.Item("Col_4").DataBind.Bind("DT_Detail", "U_GLO_BagsBales");
                        mtxDetail.Columns.Item("Col_5").DataBind.Bind("DT_Detail", "FromWhsCod");   //'de alamacen'
                        mtxDetail.Columns.Item("Col_6").DataBind.Bind("DT_Detail", "WhsCode");      //'almacen destino'
                        mtxDetail.LoadFromDataSource();
                    }                    
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowError(string.Format("ItemEventException: {0}", ex.Message));
            }
            finally
            {
            }    
        }
        
        #endregion

        #region Controls
        private SAPbouiCOM.Matrix mtxHeader; // = new SAPbouiCOM.Matrix();
        private SAPbouiCOM.Matrix mtxDetail;
        private SAPbouiCOM.Button cmdAccept;
        private SAPbouiCOM.Button cmdReturn;
        private SAPbouiCOM.StaticText lblComents;
        private SAPbouiCOM.EditText txtComents;
        private SAPbouiCOM.Button btnCancel;
        #endregion
    }
        
}
