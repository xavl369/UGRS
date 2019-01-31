using System;
using System.Collections.Generic;
using System.Xml;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.Permissions
{
    [FormAttribute("UGRS.AddOn.Permissions.Form1", "Prefixes.b1f")]
    class Prefixes : UserFormBase
    {

        private SAPbouiCOM.ComboBox lObjCmbPrefixes;
        private SAPbouiCOM.Button lObjBtnAccept;
        private SAPbouiCOM.DataSource lObjDtPrefixes;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText lObjETActiveP;

        Services.PrefixesService lObjPrefixesService = new Services.PrefixesService();
        DAO.PrefixesDAO lObjPrefixesDAO = new DAO.PrefixesDAO();

        int lIntRowCode = 0;
        string lStrPrevPrefix = "";

        public Prefixes()
        {
            LoadEvents();
            SetPrevPrefix();
            ConfigForm();
        }

        private void ConfigForm()
        {
            this.UIAPIRawForm.Left = 700;
            this.UIAPIRawForm.Top = 300;
        }

        private void SetPrevPrefix()
        {
             lIntRowCode = lObjPrefixesDAO.GetPrevCode();
             lStrPrevPrefix = lObjPrefixesDAO.GetPrevPrefix();

             lObjETActiveP.Value = lStrPrevPrefix;

        }

        //private void setComboBox()
        //{
        //    //Recordset lObjRecordSet = null;


        //    ////this.UIAPIRawForm.DataSources.DBDataSources.Add("DBDS");
        //    ////lObjDtPrefixes = (SAPbouiCOM.DataSource)this.UIAPIRawForm.DataSources.DBDataSources.Item("DBDS");
            
        //    ////lObjCmbPrefixes.DataBind.SetBound(true, "DBDS", "Prefix");

        //    //lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

        //    //lObjRecordSet.DoQuery("select U_Prefix from [@ug_pe_prfx] ");

        //    //while (lObjRecordSet.EoF == false)
        //    //{
        //    //    lObjCmbPrefixes.ValidValues.Add(lObjRecordSet.Fields.Item(0).Value.ToString(),"prefix");
        //    //    lObjRecordSet.MoveNext();
        //    //}



            
            
        //}

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

            if (FormUID.Equals(this.UIAPIRawForm.UniqueID))
            {
                if (!pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {

                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            if (pVal.ItemUID.Equals("BtnOk") && lObjCmbPrefixes.Value != string.Empty)
                            {
                                if (ShowConfirmDialog())
                                {
                                    ChooseActivePrefix();
                                    Application.SBO_Application.StatusBar.SetText("Prefijo actualizado correctamente"
                        , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                }
                            
                            }
                            break;

                        case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                            UnLoadEvents();
                            break;

                    }
                }
            }
        }


        private bool ShowConfirmDialog()
        {
            int result = SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("¿Esta seguro que desea cambiar la configuración de prefijos para permisos?", 1, "Ok", "Cancelar", "");
            if (result == 1)
            {
                return true;
            }
            else { return false; }
        }

        private void ChooseActivePrefix()
        {
          
            InsertActivePrefix();
            if (PrevPrefix())
            {
                UpdatePreviousPrefix(lStrPrevPrefix, lIntRowCode);
            }
            SetPrevPrefix();
        }

        private bool PrevPrefix()
        {
            if(lIntRowCode == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

   
        private void InsertActivePrefix()
        {


            Tables.PrefixesT lObjPrefixesT = new Tables.PrefixesT();

                lObjPrefixesT.Prefix = lObjCmbPrefixes.Value;
                lObjPrefixesT.Active = "Y";

                int lIntResult = lObjPrefixesService.SaveActivePrefix(lObjPrefixesT);
                if (lIntResult != 0)
                {
                    string lStrError = DIApplication.Company.GetLastErrorDescription();
                    Application.SBO_Application.StatusBar.SetText(lStrError
                          , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                }
       
            
        }

        private void UpdatePreviousPrefix(string lStrPrefix, int lIntRowCode)
        {
            Tables.PrefixesT lObjPrefixesT = new Tables.PrefixesT();
            
            lObjPrefixesT.Prefix = lStrPrefix;
            lObjPrefixesT.Active = "N";
            lObjPrefixesT.RowCode = lIntRowCode.ToString();

            int lIntResult = lObjPrefixesService.UpdatePrevPrefix(lObjPrefixesT);
            if (lIntResult != 0)
            {
                string lStrError = DIApplication.Company.GetLastErrorDescription();
                Application.SBO_Application.StatusBar.SetText(lStrError
                      , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            }
        }



        #region default
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lObjCmbPrefixes = ((SAPbouiCOM.ComboBox)(this.GetItem("cbPrefix").Specific));
            this.lObjBtnAccept = ((SAPbouiCOM.Button)(this.GetItem("BtnOk").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.lObjETActiveP = ((SAPbouiCOM.EditText)(this.GetItem("txtActive").Specific));
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

        
    }
}