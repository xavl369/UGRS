using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;

namespace Corrales
{
    public class ModGoodsReceipt
    {

        public string mStrCardCode = "";
        public string mStrDocDate = "";
        public string mStrChekIn = "";
        public string txtValue = "";
        SAPbouiCOM.Matrix mMtxBatch;
        SAPbouiCOM.Form mFrmBatches;
        SAPbouiCOM.EditText txtTest;
        public SAPbouiCOM.Form mFrmGR;

      public ModGoodsReceipt()
        {
            if (DIApplication.Company.Connected)
            {
                LoadEvents();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadEvents()
        {
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void UnLoadEvents()
        {
            Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FormUID"></param>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
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
                                
                                //mFrmGR = Application.SBO_Application.Forms.Item("143");
                                /* GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);*/
                                //GetValue();
                                break;

                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                /*
                                string typeForm = mFrmGR.UDFFormUID;
                                if (pVal.ItemUID == "1")
                                {
                                    SAPbouiCOM.Form oUDFFrm = Application.SBO_Application.Forms.Item(typeForm);
                                    txtValue = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_PE_ChargeTo").Specific).Value;
                                    mStrCardCode = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_BusinessPartner").Specific).Value;
                                    mStrChekIn = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_GLO_CheckIn").Specific).Value;
                                    mStrDocDate = ((SAPbouiCOM.EditText)mFrmGR.Items.Item("9").Specific).Value;
                                }

                                */
                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:

                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE:
                                /*
                                mFrmGR = Application.SBO_Application.Forms.ActiveForm;
                                string typeForm = mFrmGR.UDFFormUID;
                                if (typeForm != "")
                                {
                                    SAPbouiCOM.Form oUDFFrm = Application.SBO_Application.Forms.Item(typeForm);
                                    txtValue = ((SAPbouiCOM.EditText)oUDFFrm.Items.Item("U_PE_ChargeTo").Specific).Value;
                                }
                                 * */
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
                                    }
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_KEY_DOWN :
                                if(pVal.CharPressed == 13)
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
                                        }
                                    }

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
                                    if (ValidateGetBatchName())
                                    {
                                        for (int i = 1; i < lObjMtxBatch.Columns.Item(1).Cells.Count + 1; i++)
                                        {
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("2").Cells.Item(i).Specific).Value = GetBatchName();
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("7").Cells.Item(i).Specific).Value = mStrCardCode;
                                            ((SAPbouiCOM.EditText)lObjMtxBatch.Columns.Item("U_GLO_Time").Cells.Item(i).Specific).Value = mStrChekIn;

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
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(string.Format("ItemEventException: {0}", ex.Message));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string GetBatchName()
        {
            string lStrResult = "";
            SAPbobsCOM.Recordset lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {

                lObjRecordSet.DoQuery("SELECT CardFName FROM OCRD WHERE CardCode ='" + mStrCardCode + "' ");

                var seconds = DateTime.Now.Second.ToString();

                if (seconds.Length == 1)
                    seconds = "0" + seconds;

                var formatedDate = mStrDocDate.Length == 8 ? mStrDocDate.Substring(2, 6) : mStrDocDate;

                lStrResult = lObjRecordSet.Fields.Item(0).Value.ToString() + formatedDate + mStrChekIn + seconds;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }



            return lStrResult;



        }

        /// <summary>
        /// 
        /// </summary>
        private bool ValidateGetBatchName()
        {
            bool lBoolFlag = true;

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

        private void SetValue()
        {

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
            (lItmBtnBatches.Specific as SAPbouiCOM.Button).Caption = "Generar Lote";
        }

    }
}
