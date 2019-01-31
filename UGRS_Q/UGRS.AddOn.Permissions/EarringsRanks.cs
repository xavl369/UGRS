using SAPbouiCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.Permissions
{
    public class EarringsRanks
    {

        SAPbouiCOM.Form lObjEarringsForm = null;
        SAPbouiCOM.Item lObjAddRank = null;
        SAPbouiCOM.EditText lObjETxtBaseEntry = null;

        mFormEarringRanks pObjMFrmEarringR = null;
        
        DAO.EarringRanksDAO lObjEarringRanksDAO = new DAO.EarringRanksDAO();

        Menu pObjMenu = new Menu();

        public EarringsRanks()
        {

            if (DIApplication.Company.Connected)
            {
                LoadEvents();

            }

        }

        private void SetEditTxtBE()
        {
            lObjETxtBaseEntry = ((SAPbouiCOM.EditText)lObjEarringsForm.Items.Item("8").Specific);
        }

        #region Load & Unload Events
        private void LoadEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void UnLoadEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent -= new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }
        #endregion

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (pVal.FormType.Equals(139))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                lObjEarringsForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
                                SetEditTxtBE();
                                lObjAddRank = lObjEarringsForm.Items.Add("btnRank", SAPbouiCOM.BoFormItemTypes.it_BUTTON);

                                (lObjAddRank.Specific as Button).Caption = "Rangos";
                                lObjAddRank.Top = 120;
                                lObjAddRank.Left = 480;
                                break;


                            case SAPbouiCOM.BoEventTypes.et_CLICK:


                                if (pVal.ItemUID.Equals("btnRank"))
                                {

                                    if (lObjEarringRanksDAO.CheckBaseEntry(lObjETxtBaseEntry.Value))
                                    {
                                      
                                        pObjMFrmEarringR = new mFormEarringRanks(lObjETxtBaseEntry.Value);
                                    }

                                }


                                break;


                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                //UnLoadEvents();
                                pObjMenu.lStrTypeEx = "";
                                pObjMenu.lIntTypeCount = 0;

                                break;

                        }
                    }
                }
                else if (FormUID.Equals("mFrmEarringRanks"))
                {
                    if (!pVal.BeforeAction)
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_CLICK:
                                if (pVal.ItemUID.Equals("BtnAdd"))
                                {
                                    
                                    pObjMFrmEarringR.AddRow();
                                }
                                else if (pVal.ItemUID.Equals("BtnOk"))
                                {
                                    pObjMFrmEarringR.SaveRanks();
                                    SAPbouiCOM.Framework.Application.SBO_Application.Forms.Item("mFrmEarringRanks").Close();
                                }
                                else if (pVal.ItemUID.Equals("btnDel"))
                                {
                                    pObjMFrmEarringR.CancelRow();
                                }
                                break;
                            case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                                if (pVal.ItemUID.Equals("TxtFrom") || pVal.ItemUID.Equals("TxtTo"))
                                {
                                    pObjMFrmEarringR.ValidateOnlyNumbers(pVal.CharPressed, pVal.ItemUID);
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

        

    }
}
