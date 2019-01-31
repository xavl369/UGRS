using SAPbouiCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.SDK;
using UGRS.Core.SDK.DI.Configuration.Services;

namespace UGRS.Form.Configuration
{
    class ConfigurationForm
    {
        private SAPbouiCOM.Application SBO_Application;
        private SAPbouiCOM.Form oForm;


        public ConfigurationForm()
        {
            //*************************************************************
            // set SBO_Application with an initialized application object
            //*************************************************************

            SetApplication();

            CreateForm();

            // events handled by SBO_Application_ItemEvent
            SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void SetApplication()
        {
            SAPbouiCOM.SboGuiApi SboGuiApi = null;
            string sConnectionString = null;

            SboGuiApi = new SAPbouiCOM.SboGuiApi();

            sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));


            SboGuiApi.Connect(sConnectionString);

            SBO_Application = SboGuiApi.GetApplication(-1);

        }

        private void CreateForm()
        {

            SAPbouiCOM.FormCreationParams oCP = null;
            SAPbouiCOM.Item oItem = null;
            SAPbouiCOM.StaticText oStatic = null;
            SAPbouiCOM.Button oButton = null;
            SAPbouiCOM.EditText oEdit = null;

            //  Setting the form creation params
            oCP = ((SAPbouiCOM.FormCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams)));
            oCP.UniqueID = "CFL3";
            oCP.FormType = "CFL3";
            oCP.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Sizable;


            //  Adding the form
            oForm = SBO_Application.Forms.AddEx(oCP);
            oForm.Title = "Configuración";

            oItem = oForm.Items.Add("StaticTxt", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oItem.Left = 10;
            oItem.Top = 20;
            oItem.LinkTo = "EditTxt";
            oItem.Width = 200;
            oStatic = ((SAPbouiCOM.StaticText)(oItem.Specific));
            oStatic.Caption = "Carpeta de archivos";
           

            oItem = oForm.Items.Add("StaticTxt2", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oItem.Left = 10;
            oItem.Top = 50;
            oItem.LinkTo = "EditTxt";
            oItem.Width = 200;
            oStatic = ((SAPbouiCOM.StaticText)(oItem.Specific));
            oStatic.Caption = "Kilometros recorridos";

            oItem = oForm.Items.Add("txtKM", SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oItem.Left = 120;
            oItem.Top = 50;
            oItem.LinkTo = "StaticTxt2";
            oEdit = ((SAPbouiCOM.EditText)(oItem.Specific));


            oItem = oForm.Items.Add("StaticTxt3", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oItem.Left = 10;
            oItem.Top = 80;
            oItem.LinkTo = "EditTxt";
            oStatic = ((SAPbouiCOM.StaticText)(oItem.Specific));
            oStatic.Caption = "Horas de motor";

            oItem = oForm.Items.Add("txtHoras", SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oItem.Left = 120;
            oItem.Top = 80;
            oItem.LinkTo = "StaticTxt3";
            oEdit = ((SAPbouiCOM.EditText)(oItem.Specific));

            //  Adding a CFL button
            oItem = oForm.Items.Add("btnGuardar", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            oItem.Left = 120;
            oItem.Top = 110;
            oButton = ((SAPbouiCOM.Button)(oItem.Specific));
            oButton.Type = SAPbouiCOM.BoButtonTypes.bt_Caption;
            oButton.Caption = "Guardar";
            oItem.Width = 100;
            oItem.Height = 20;
            

            oForm.Width = 300;
            oForm.Height = 300;

            oForm.Visible = true;

        }

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.ItemUID == "btnGuardar")
            {
               UGRS.Core.SDK.DI.Configuration.Tables.Config lObjConfig = new Core.SDK.DI.Configuration.Tables.Config();
               List<UGRS.Core.SDK.DI.Configuration.Tables.Config> lLstConfig = new List<UGRS.Core.SDK.DI.Configuration.Tables.Config>();
               UGRS.Core.SDK.DI.Configuration.Tables.Config lObjConfigKM = new UGRS.Core.SDK.DI.Configuration.Tables.Config();
               lObjConfigKM.Name = "Path KM";
               lObjConfigKM.Value = @"C:\Users\amartinez\Desktop\csv\KM";

               UGRS.Core.SDK.DI.Configuration.Tables.Config lObjConfigHoras = new UGRS.Core.SDK.DI.Configuration.Tables.Config();
               lObjConfigHoras.Name = "Path HorasMotor";
               lObjConfigHoras.Value = @"C:\Users\amartinez\Desktop\csv\KM";

               lLstConfig.Add(lObjConfigKM);
               lLstConfig.Add(lObjConfigHoras);

               AddConfig(lLstConfig);

            }

            if ((FormUID == "CFL1") & (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
            {
                System.Windows.Forms.Application.Exit();
            }


        }
        private void AddConfig(List<UGRS.Core.SDK.DI.Configuration.Tables.Config> pLstConfig)
        {
            ConfigurationService lObjConfigurationService = new ConfigurationService();
            lObjConfigurationService.Initialize();

            foreach (UGRS.Core.SDK.DI.Configuration.Tables.Config lConfig in pLstConfig)
            {
                if (lObjConfigurationService.Exist(lConfig.Name))
                {
                    lObjConfigurationService.Update(lConfig);
                }
                else
                {
                    lObjConfigurationService.Add(lConfig);
                }
            }
        }
       
    }
}
