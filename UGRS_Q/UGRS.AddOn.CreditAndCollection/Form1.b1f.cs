using System;
using System.Collections.Generic;
using System.Xml;
using SAPbouiCOM.Framework;
using UGRS.Core.Utility;
using UGRS.Core.Extension.Enum;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.UI;
using SAPbobsCOM;
using UGRS.AddOn.CreditAndCollection.DTO;
using UGRS.AddOn.CreditAndCollection.DAO;
using UGRS.AddOn.CreditAndCollection.Services;
using UGRS.AddOn.CreditAndCollection.Tables;

namespace UGRS.AddOn.CreditAndCollection
{
    [FormAttribute("UGRS.AddOn.CreditAndCollection.Form1", "Form1.b1f")]
    class Form1 : UserFormBase
    {
        private SAPbouiCOM.EditText mObjTxtDate;
        private SAPbouiCOM.StaticText mObjLblDate;
        private SAPbouiCOM.StaticText mObjLblAuct;
        private SAPbouiCOM.Button mObjBtnSearch;
        private SAPbouiCOM.Matrix mObjSMatrix;
        private SAPbouiCOM.Button mObjBtnSave;
        private SAPbouiCOM.Button mObjBtnPayR;
        private SAPbouiCOM.ComboBox mObjCbAuct;

        private SAPbouiCOM.Form mObjPayRForm;
        private SAPbouiCOM.Form mObjPayRFormUF;
        private SAPbouiCOM.EditText mObjtxtCardCode;
        private SAPbouiCOM.ComboBox mObjcbAuxType;
        private SAPbouiCOM.EditText mObjtxtAuctNum;
        private SAPbouiCOM.OptionBtn mObjOptB;

        

        private List<string> lStrLstAuctions = new List<string>();
        private List<CreditCollectionDTO> mObjLstCreditCollection = null;

        CreditCollectionDAO mObjCreditCollectionDAO = new CreditCollectionDAO();
        CreditCollectionDTO mObjCreditCollectionDTO = new CreditCollectionDTO();


        public Form1()
        {
            LoadEvents();
            LoadComboBox();

        }

        private void LoadComboBox()
        {

            lStrLstAuctions = (List<string>)mObjCreditCollectionDAO.GetComboList();

            var lVarAuctionsG = new HashSet<string>(lStrLstAuctions);

            foreach (var item in lVarAuctionsG)
            {
                mObjCbAuct.ValidValues.Add(item, item.Substring(6, 6));
            }

        }

        #region Load&Undload Events
        private void LoadEvents()
        {
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void UnLoadEvents()
        {
            Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        #region Default
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.mObjTxtDate = ((SAPbouiCOM.EditText)(this.GetItem("txtDate").Specific));
            this.mObjLblDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblDate").Specific));
            this.mObjLblAuct = ((SAPbouiCOM.StaticText)(this.GetItem("lblAuct").Specific));
            this.mObjBtnSearch = ((SAPbouiCOM.Button)(this.GetItem("btnSearch").Specific));
            this.mObjSMatrix = ((SAPbouiCOM.Matrix)(this.GetItem("mtxSeller").Specific));
            this.mObjBtnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.mObjBtnPayR = ((SAPbouiCOM.Button)(this.GetItem("btnPaymR").Specific));
            this.mObjCbAuct = ((SAPbouiCOM.ComboBox)(this.GetItem("cbAuctN").Specific));
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

        /// <Events>
        /// Eventos de Form1
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
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

                                    LoadMatrix();

                                }
                                if (pVal.ItemUID.Equals("btnPaymR"))
                                {
                                    int lIntSelectedRow = mObjSMatrix.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);

                                    if (lIntSelectedRow > 0)
                                    {
                                        string lStrBP = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").Columns.Item("U_SellerCode").Cells.Item(lIntSelectedRow - 1).Value;
                                        string lStrAuctNum = mObjCbAuct.Value;

                                        OutgoingPayments(lStrBP, lStrAuctNum);
                                    }
                                    else
                                    {
                                        string lStrMessage = Enums.MessagesEnum.WarningMessages.SIN_REGISTRO.GetDescription();
                                        Messages(lStrMessage, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                        LogUtility.WriteInfo("SIN REGISTRO SeELECCIONADO");
                                    }

                                }
                                if (pVal.ItemUID.Equals("btnSave"))
                                {
                                    ListSeletAutorizations();
                                }


                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                UnLoadEvents();
                                break;
                        }
                    }
                }






            }
            catch (Exception lObjException)
            {
                //LogUtility.WriteError("DI API: " + DIApplication.Company.GetLastErrorDescription());
                LogUtility.WriteError("EXCEPCION: " + lObjException.Message);
                Messages(lObjException.Message, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                //Application.SBO_Application.StatusBar.SetText(lObjException.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private void ListSeletAutorizations()
        {
            mObjLstCreditCollection = new List<CreditCollectionDTO>();

            for (int i = 1; i <= mObjSMatrix.RowCount; i++)
            {
                if (((dynamic)mObjSMatrix.Columns.Item(4).Cells.Item(i).Specific).Checked)
                {
                    if (((dynamic)mObjSMatrix.Columns.Item(4).Cells.Item(i).Specific).Checked)
                    {
                        List(i);
                    }
                }
            }

            if(mObjLstCreditCollection.Count > 0)
            {
                SaveAutorization();
            }
        }

        private void SaveAutorization()
        {
            CreditAndCollectionService mObjCreditCollectionService = new CreditAndCollectionService();
            string lStrMessage = string.Empty;

            foreach (var item in mObjLstCreditCollection)
            {
                if (mObjCreditCollectionService.SaveAutorization(GetObjAutorization(item.AuctionNumb, item.Seller, item.AuxType)) != 0)
                {
                    string lStrError = DIApplication.Company.GetLastErrorDescription();
                    Messages(lStrError, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                else
                {
                    Messages(lStrMessage, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
            }
 
        }

        private CreditAndCollectionTable GetObjAutorization(string pStrFolio,string pStrAux, int pIntAuxType)
        {
        CreditAndCollectionTable mObjCreditAndCollectionT = new CreditAndCollectionTable();

        mObjCreditAndCollectionT.FolioSubasta = pStrFolio;
        mObjCreditAndCollectionT.Auxiliar = pStrAux;
        mObjCreditAndCollectionT.TipoAux = pIntAuxType;
        mObjCreditAndCollectionT.Revizado = "Y";

        return mObjCreditAndCollectionT;     
        }

        private void List(int pIntRow)
        {
            mObjCreditCollectionDTO = new DTO.CreditCollectionDTO();

            mObjCreditCollectionDTO.Seller = (string)this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").GetValue("U_SellerCode", pIntRow-1);
            mObjCreditCollectionDTO.TotCredit = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").GetValue("TotSelled", pIntRow - 1);
            mObjCreditCollectionDTO.TotDebit = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").GetValue("TotDebit", pIntRow - 1);
            mObjCreditCollectionDTO.TotInvoice = (double)this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").GetValue("PaidInv", pIntRow - 1);
            mObjCreditCollectionDTO.AuctionNumb = mObjCbAuct.Value;
            mObjCreditCollectionDTO.AuxType = 1;

            mObjLstCreditCollection.Add(mObjCreditCollectionDTO);
        }

        private void LoadMatrix()
        {
            string lStrAccount = "2050030000000";
            try
            {
                this.UIAPIRawForm.Freeze(true);

                this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC")
                    .ExecuteQuery(mObjCreditCollectionDAO.GetMatrixList(mObjTxtDate.Value, mObjCbAuct.Value, lStrAccount));

                mObjSMatrix.Columns.Item("Seller").DataBind.Bind("DM_CC", "CardName");
                mObjSMatrix.Columns.Item("TotCred").DataBind.Bind("DM_CC", "TotSelled");
                mObjSMatrix.Columns.Item("TotDeb").DataBind.Bind("DM_CC", "TotDebit");
                mObjSMatrix.Columns.Item("Chck").DataBind.Bind("DM_CC", "Chk");
                mObjSMatrix.Columns.Item("OvdInv").DataBind.Bind("DM_CC", "PaidInv");

                mObjSMatrix.LoadFromDataSource();
                mObjSMatrix.AutoResizeColumns();

                DisableAutorizedCells();

                this.UIAPIRawForm.Freeze(false);
            }
            catch (Exception er)
            {

            }
            finally
            {
                //MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        private void DisableAutorizedCells()
        {
            SAPbouiCOM.CommonSetting lObjRowCtrl;
            lObjRowCtrl = mObjSMatrix.CommonSetting;

            for (int i = 1; i < mObjSMatrix.RowCount; i++)
            {
                if ((this.UIAPIRawForm.DataSources.DataTables.Item("DM_CC").GetValue("Autorized", i - 1).Equals("Y")))
                {
                    lObjRowCtrl.SetCellEditable(i, 4, false);
                }
                else
                {
                    lObjRowCtrl.SetCellEditable(i, 4, true);
                }
            }
           
        }

        #region Methods

        private void OutgoingPayments(string pStrBP, string pStrAuction)
        {
            initOutgoingPayments();

            mObjPayRForm.Freeze(true);
            mObjOptB.Item.Click();
            mObjtxtCardCode.Value = pStrBP;
            mObjcbAuxType.Select(1, SAPbouiCOM.BoSearchKey.psk_Index);
            mObjtxtAuctNum.Value = pStrAuction;
            mObjPayRForm.Freeze(false);


        }

        private void initOutgoingPayments()
        {

            UIApplication.GetApplication().ActivateMenuItem("2818"); //Open Outgoing Payments Form

            mObjPayRForm = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(426, 1); //Set Form
            mObjPayRFormUF = UIApplication.GetApplication().Forms.GetFormByTypeAndCount(-426, 1); // Set UserFields Form

            //Set Controls
            mObjtxtCardCode = (SAPbouiCOM.EditText)mObjPayRForm.Items.Item("5").Specific;
            mObjOptB = (SAPbouiCOM.OptionBtn)mObjPayRForm.Items.Item("56").Specific;
            mObjcbAuxType = (SAPbouiCOM.ComboBox)mObjPayRFormUF.Items.Item("U_FZ_AuxiliarType").Specific;
            mObjtxtAuctNum = (SAPbouiCOM.EditText)mObjPayRFormUF.Items.Item("U_FZ_FolioAuction").Specific;

        }

        #endregion


        #region Utilities
        /// <Messages>
        /// Mensajes generales
        /// </summary>
        /// <param name="lStrMessage"></param>
        /// <param name="boStatusBarMessageType"></param>
        private void Messages(string lStrMessage, SAPbouiCOM.BoStatusBarMessageType boStatusBarMessageType)
        {
            Application.SBO_Application.StatusBar.SetText(lStrMessage, SAPbouiCOM.BoMessageTime.bmt_Short, boStatusBarMessageType);
        }

        #endregion



    }
}