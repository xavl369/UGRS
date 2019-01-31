using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.Utility;

namespace Corrales
{
    public class FloorCharges
    {
        SAPbouiCOM.Item lObjBtn = null;
        SAPbouiCOM.Form lObjForm = null;
        SAPbouiCOM.Form lObjFormUserFields = null;

        SAPbouiCOM.Matrix lObjSysMatrix = null;


        SAPbouiCOM.EditText lObjEditTxtCardCode = null;
        SAPbouiCOM.EditText lObjEditTxtDocDate = null;
        SAPbouiCOM.EditText lObjEditTxtCorral = null;

        modalCharges pObjModalCharges = null;
        DAO.ChargesDAO mObjChargesDAO = new DAO.ChargesDAO();

        #region variables
        string lStrCorral = string.Empty;
        string lStrCardCode = string.Empty;
        string lStrDocDate = string.Empty;

        List<string> lStrListCorralCodes = null;
        #endregion


        public FloorCharges()
        {
            if (DIApplication.Company.Connected)
            {
                LoadEvents();

            }
        }

        private void initItemsFrm1()
        {
            lObjEditTxtCardCode = (SAPbouiCOM.EditText)lObjForm.Items.Item("4").Specific;
            lObjEditTxtDocDate = (SAPbouiCOM.EditText)lObjForm.Items.Item("10").Specific;
        }
        private void initItemsFrm2()
        {
            lObjEditTxtCorral = (SAPbouiCOM.EditText)lObjFormUserFields.Items.Item("U_GLO_Corral").Specific;
        }

        private void initMatrix()
        {
            lObjSysMatrix = (SAPbouiCOM.Matrix)lObjForm.Items.Item("38").Specific;
        }

        private void LoadEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.FormType.Equals(133))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                lObjForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

                                
                                lObjBtn = lObjForm.Items.Add("btnCharges", SAPbouiCOM.BoFormItemTypes.it_BUTTON);

                                (lObjBtn.Specific as Button).Caption = "Cargo Piso";
                                lObjBtn.Top = 120;
                                lObjBtn.Left = 480;
                                break;

                            case SAPbouiCOM.BoEventTypes.et_CLICK:

                                if (pVal.ItemUID.Equals("btnCharges"))
                                {
                                    initItemsFrm1();

                                    if (ValidBP())
                                    {
                                        lStrListCorralCodes = new List<string>();

                                        

                                        lStrCardCode = (string)lObjEditTxtCardCode.Value;
                                        lStrDocDate = lObjEditTxtDocDate.Value.ToString();

                                        for (int i = 1; i < lObjSysMatrix.RowCount; i++)
                                        {
                                            lStrCorral = mObjChargesDAO.GetLineCorral(((SAPbouiCOM.EditText)(lObjSysMatrix.Columns.Item("44").Cells.Item(i).Specific)).Value);
                                            if (lStrCorral != "")
                                            {
                                                lStrListCorralCodes.Add(lStrCorral);
                                            }
                                        }
                                            pObjModalCharges = new modalCharges(lObjForm, lStrCardCode, lStrDocDate, lStrListCorralCodes);
                                    }
                                    else
                                    {
                                        SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Seleccione Socio de Negocios"
                            , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    }
                                }


                                break;

                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                if (lObjFormUserFields != null)
                                {
                                    MemoryUtility.ReleaseComObject(lObjFormUserFields);
                                }

                                break;

                        }
                    }
                }
                else if (pVal.FormUID.Equals("FloorCharges"))
                {
                    if (!pVal.BeforeAction)
                    {

                        switch (pVal.EventType)
                        {

                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("Item_1"))
                                {
                                    if (pObjModalCharges.AnyLineToInvoice())
                                    {

                                        pObjModalCharges.Invoice();

                                        SAPbouiCOM.Framework.Application.SBO_Application.Forms.Item(pVal.FormUID).Close();

                                    }
                                    else
                                    {
                                        SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText("Seleccione al menos un Corral"
                            , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    }
                                }
                                break;

                            case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                                if (pVal.ColUID.Equals("colChck"))
                                {
                                    pObjModalCharges.ForbiddenUncheck(pVal.Row);
                                }
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

        private bool ValidBP()
        {
            initMatrix();

            return lObjEditTxtCardCode.Value != string.Empty;


        }
    }
}
