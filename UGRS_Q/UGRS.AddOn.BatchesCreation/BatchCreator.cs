using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.AddOn.BatchesCreation
{
    public class BatchCreator
    {
        #region variables
        public string mStrCardCode = "";
        public string mStrDocDate = "";
        public string mStrChekIn = "";
        public string txtValue = "";

        public string mStrFoodType = string.Empty;
        public string mStrCharged = string.Empty;
        public int mIntCharged = -1;
        public int mIntFoodType = -1;

        private int lIntUserSign = DIApplication.Company.UserSignature;
        private string lStrSeriesName;
        #endregion

        #region SapObjects
        SAPbouiCOM.Matrix mMtxBatch;
        SAPbouiCOM.Form mFrmBatches;
        SAPbouiCOM.EditText txtTest;
        SAPbouiCOM.ComboBox mObjCbFoodType;
        SAPbouiCOM.ComboBox mObjCbCharged;
        SAPbouiCOM.ComboBox mObjCbFT;
        SAPbouiCOM.ComboBox mObjCbChg;
        SAPbouiCOM.Form oUDFFrm;
        SAPbouiCOM.Form mObjFormUDF;
        public SAPbouiCOM.Form mFrmGR;
        #endregion

        private DAO.BatchCreatorDAO mObjBatchCreatorDAO = new DAO.BatchCreatorDAO();

        public BatchCreator()
        {
            if (DIApplication.Company.Connected)
            {
                LogService.WriteSuccess("AddOn Connected");
                LoadEvents();
                lStrSeriesName = mObjBatchCreatorDAO.GetSerie(lIntUserSign);
                //initCombos();
            }
        }

        private void initCombo(SAPbouiCOM.Form oUDFFrm)
        {
            mObjCbFoodType = ((SAPbouiCOM.ComboBox)oUDFFrm.Items.Item("U_SU_FoodType").Specific);
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

            try
            {
                #region FormGR
                if (pVal.FormType.Equals(721))
                {


                    if (!pVal.BeforeAction)
                    {


                        switch (pVal.EventType)
                        {

                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                mFrmGR = Application.SBO_Application.Forms.ActiveForm;
                                break;



                            case SAPbouiCOM.BoEventTypes.et_FORM_DEACTIVATE:
                                mFrmGR = Application.SBO_Application.Forms.ActiveForm;
                                string typeFormD = mFrmGR.UDFFormUID;
                                if (mFrmGR.Type == 721)
                                {
                                    if (typeFormD != "")
                                    {
                                        SAPbouiCOM.Form oUDFFrm = Application.SBO_Application.Forms.Item(typeFormD);
                                        txtValue = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_PE_ChargeTo").Specific).Value;
                                        mStrCardCode = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_BusinessPartner").Specific).Value;
                                        mStrChekIn = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_CheckIn").Specific).Value;
                                        mStrDocDate = ((SAPbouiCOM.EditText)mFrmGR.Items.Item("9").Specific).Value;
                                        //mStrFoodType = mObjCbFoodType != null ? mObjCbFoodType.Value : string.Empty;
                                       mStrFoodType = ((SAPbouiCOM.ComboBox)oUDFFrm.Items.Item("U_SU_FoodType").Specific).Value;
                                    }
                                }
                                break;
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                mObjFormUDF = null;
                                break;



                        }
                    }
                    else
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                                if (pVal.CharPressed == 13)
                                {
                                    mFrmGR = Application.SBO_Application.Forms.ActiveForm;
                                    string typeFormE = mFrmGR.UDFFormUID;
                                    if (mFrmGR.Type == 721)
                                    {
                                        if (typeFormE != "")
                                        {
                                            SAPbouiCOM.Form oUDFFrm = Application.SBO_Application.Forms.Item(typeFormE);
                                            txtValue = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_PE_ChargeTo").Specific).Value;
                                            mStrCardCode = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_BusinessPartner").Specific).Value;
                                            mStrChekIn = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_CheckIn").Specific).Value;
                                            mStrDocDate = ((SAPbouiCOM.EditText)mFrmGR.Items.Item("9").Specific).Value;
                                            mStrFoodType = mObjCbFoodType.Value;
                                        }
                                    }

                                }
                                break;


                        }
                    }
                }
                if (pVal.FormType.Equals(-721))
                {
                    if (!pVal.BeforeAction)
                    {

                        switch (pVal.EventType)
                        {

                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                if (mObjFormUDF == null)
                                {
                                    mObjFormUDF = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                    initCombo(mObjFormUDF);
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE:
                                if (mObjFormUDF == null)
                                {
                                    mObjFormUDF = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                                    initCombo(mObjFormUDF);
                                }
                                break;
                        }
                    }
                }

                #endregion

                #region FormBatch
                if (pVal.FormType.Equals(41))
                {
                    if (!pVal.BeforeAction)
                    {

                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                mFrmBatches = Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                AddButton();

                                break;

                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID == "btnBatch")
                                {
                                    SAPbouiCOM.Matrix lObjMtxBatch = (SAPbouiCOM.Matrix)mFrmBatches.Items.Item("3").Specific;
                                    SAPbouiCOM.ComboBox lObjCbFood = (SAPbouiCOM.ComboBox)lObjMtxBatch.Columns.Item("U_GLO_Food3").Cells.Item(1).Specific;
                                    SAPbouiCOM.ComboBox lObjCbPaymnt = (SAPbouiCOM.ComboBox)lObjMtxBatch.Columns.Item("U_SU_Payment").Cells.Item(1).Specific;

                                    if (ValidateGetBatchName())
                                    {
                                        for (int i = 1; i < lObjMtxBatch.Columns.Item(1).Cells.Count + 1; i++)
                                        {
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("2").Cells.Item(i).Specific).Value = GetBatchName();
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("7").Cells.Item(i).Specific).Value = mStrCardCode;
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("U_GLO_Time").Cells.Item(i).Specific).Value = mStrChekIn;


                                            lObjCbFood.Select(mIntFoodType, SAPbouiCOM.BoSearchKey.psk_Index);
                                            lObjCbPaymnt.Select(mIntCharged, SAPbouiCOM.BoSearchKey.psk_Index);



                                            lObjMtxBatch.Columns.Item("10").Cells.Item(i).Click();
                                        }

                                    }

                                }

                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                string typeForm = mFrmGR.UDFFormUID;
                                if (mFrmGR.Type == 41)
                                {
                                    if (typeForm != "")
                                    {
                                        SAPbouiCOM.Form oUDFFrm = Application.SBO_Application.Forms.Item(typeForm);
                                        txtValue = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_PE_ChargeTo").Specific).Value;
                                        mStrCardCode = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_BusinessPartner").Specific).Value;
                                        mStrChekIn = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_CheckIn").Specific).Value;
                                        mStrDocDate = ((SAPbouiCOM.EditText)mFrmGR.Items.Item("9").Specific).Value;
                                    }
                                }
                                break;


                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
            }

        }



        private string GetBatchName()
        {
            string lStrResult = "";



            var seconds = DateTime.Now.Second.ToString();

            if (seconds.Length == 1)
                seconds = "0" + seconds;

            var formatedDate = mStrDocDate.Length == 8 ? mStrDocDate.Substring(2, 6) : mStrDocDate;

            lStrResult = formatedDate + mStrChekIn + seconds;


            return lStrResult;



        }

        /// <summary>
        /// 
        /// </summary>
        private bool ValidateGetBatchName()
        {
            bool lBoolFlag = true;
            if (lStrSeriesName != "")
            {
                if (lStrSeriesName.Substring(0, 2).Equals("SU"))
                {
                    mIntCharged = 1;
                }
                else
                {
                    mIntCharged = 0;
                }
            }
            else
            {
                mIntCharged = 0;
            }

            if (mStrFoodType.Contains("Y") || mStrFoodType.Contains("S"))
            {
                mIntFoodType = 1;
            }
            else
            {
                mIntFoodType = 0;
            }

            if (mStrCardCode == "" || mStrCardCode == null)
            {
                Application.SBO_Application.StatusBar.SetText("Seleccionar Socio de Negocio", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                lBoolFlag = false;
            }
            else if (mStrChekIn == "" || mStrChekIn == null)
            {
                Application.SBO_Application.StatusBar.SetText("Seleccionar Hora de entrada", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                lBoolFlag = false;
            }

            return lBoolFlag;


        }


        /// <summary>
        /// 
        /// </summary>
        private void InitItems()
        {
            mMtxBatch = (SAPbouiCOM.Matrix)mFrmGR.Items.Item("3").Specific;
        }


        /// <summary>
        /// 
        /// </summary>
        private void AddButton()
        {
            SAPbouiCOM.Item lItmBtnBatches;
            lItmBtnBatches = mFrmBatches.Items.Add("btnBatch", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            lItmBtnBatches.Top = mFrmBatches.Items.Item("2").Top;
            lItmBtnBatches.Left = mFrmBatches.Items.Item("2").Left + 70;
            lItmBtnBatches.Height = mFrmBatches.Items.Item("2").Height;
            lItmBtnBatches.Width = mFrmBatches.Items.Item("2").Width;
            (lItmBtnBatches.Specific as SAPbouiCOM.Button).Caption = "Generar Lote";
        }

    }
}
