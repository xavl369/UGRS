using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using UGRS.Core.SDK.DI.Configuration;
using UGRS.Core.SDK.DI.Configuration.Services;
using UGRS.Core.SDK.DI.Configuration.Tables;
using System.Windows.Forms;
using UGRS.Core.Utility;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;

namespace UGRS.AddOn.Configuration
{
    [FormAttribute("UGRS.AddOn.Configuration.ConfigurationForm", "ConfigurationForm.b1f")]
    class ConfigurationForm : UserFormBase
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        private ConfigurationServicesFactory mObjConfigurationService = new ConfigurationServicesFactory();

        public ConfigurationForm()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("lblCarpeta").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.btnGuardar = ((SAPbouiCOM.Button)(this.GetItem("btnGuardar").Specific));
            this.btnGuardar.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnGuardar_ClickBefore);
            this.txtKM = ((SAPbouiCOM.EditText)(this.GetItem("txtKM").Specific));
            this.txtHoras = ((SAPbouiCOM.EditText)(this.GetItem("txtHoras").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_0").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_3").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);
            this.OnCustomInitialize();


            //DataTable lDtbCode = mObjConfigurationService.GetConfigurationService().GetConfigCode("Path KM");
           // lDtbCode.TableName = "Resultado";

            UGRS.Core.SDK.DI.Configuration.ConfigurationServicesFactory lObjConfigurationFactoryServices = new UGRS.Core.SDK.DI.Configuration.ConfigurationServicesFactory();
            lObjConfigurationFactoryServices.GetSetupService().InitializeTables();

            txtHoras.Value = mObjConfigurationService.GetConfigurationService().GetConfigValue("Path HorasMotor");
            txtKM.Value = mObjConfigurationService.GetConfigurationService().GetConfigValue("Path KM");
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.StaticText StaticText0;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.Button btnGuardar;
        private SAPbouiCOM.EditText txtKM;
        private SAPbouiCOM.EditText txtHoras;

        private void btnGuardar_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pObjVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (validFields())
            {
                List<Config> lLstConfig = new List<Config>();
                Config lObjConfigKM = new Config();
                lObjConfigKM.Name = "Path KM";
                lObjConfigKM.Value = txtKM.Value;

                Config lObjConfigHoras = new Config();
                lObjConfigHoras.Name = "Path HorasMotor";
                lObjConfigHoras.Value = txtHoras.Value;

                lLstConfig.Add(lObjConfigKM);
                lLstConfig.Add(lObjConfigHoras);

                AddConfig(lLstConfig);

            }
        }

        

        ///<summary>    Adds a configuration. </summary>
        ///<remarks>    Amartinez, 31/05/2017. </remarks>
        ///<param name="pLstConfig">    The list configuration. </param>

        private void AddConfig(List<Config> pLstConfig)
        {
            //Modificar por select
            foreach (Config lObjConfig in pLstConfig)
            {
                if (mObjConfigurationService.GetConfigurationService().Exist(lObjConfig.Name))
                {
                    lObjConfig.RowCode = mObjConfigurationService.GetConfigurationService().GetConfigCode("Code", lObjConfig.Name);
                    mObjConfigurationService.GetConfigurationService().Update(lObjConfig);
                }
                else
                {
                    mObjConfigurationService.GetConfigurationService().Add(lObjConfig);
                }
            }
        }

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            Thread lThreadShowFolderBrowser = new Thread(() => showFolderBrowserDialog(txtKM));
            lThreadShowFolderBrowser.SetApartmentState(ApartmentState.STA);
            lThreadShowFolderBrowser.Start();
        }

        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            Thread lThreadShowFolderBrowser = new Thread(() => showFolderBrowserDialog(txtHoras));
            lThreadShowFolderBrowser.SetApartmentState(ApartmentState.STA);
            lThreadShowFolderBrowser.Start();
        }

        ///<summary>    Shows the folder browser dialog. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<param name="pTxtEditText">  The edit text. </param>

        private void showFolderBrowserDialog(SAPbouiCOM.IEditText pTxtEditText)
        {
            FolderBrowserDialog lObjFolderBrowserDialog = new FolderBrowserDialog();

            IntPtr lPtrForegroundWindow = GetForegroundWindow();
            WindowWrapper lObjWindow = new WindowWrapper(lPtrForegroundWindow);

            Type lTypeFolderBrowser = lObjFolderBrowserDialog.GetType();
            FieldInfo lFieldInfo = lTypeFolderBrowser.GetField("rootFolder", BindingFlags.NonPublic | BindingFlags.Instance);
            lFieldInfo.SetValue(lObjFolderBrowserDialog, 0);

            if (lObjFolderBrowserDialog.ShowDialog(lObjWindow) == DialogResult.OK)
            {
                pTxtEditText.Value = lObjFolderBrowserDialog.SelectedPath;
            }
        }

        ///<summary>    Determines if we can valid fields. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<returns>    True if it succeeds, false if it fails. </returns>

        private bool validFields()
        {
            List<string> lLstErrorFields = new List<string>();

            if (!validPath(txtKM.Value))
            {
                lLstErrorFields.Add("Ubicación de Kilómetros inválida.");
            }
            if (!validPath(txtHoras.Value))
            {
                lLstErrorFields.Add("Ubicación de horas inválida.");
            }

            if (txtKM.Value == txtHoras.Value)
            {
                lLstErrorFields.Add("La ubicación de los directorios deben de ser diferente");
            }

            if (lLstErrorFields.Count > 0)
            {
                string lStrMessage = string.Format("Favor de corregir {0}:\n{1}",
                    (lLstErrorFields.Count == 1 ? "el siguiente error" : "los siguientes errores"),
                    string.Join("\n", lLstErrorFields.Select(x => string.Format("-{0}", x)).ToArray()));

                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(lStrMessage);
            }
            return lLstErrorFields.Count == 0 ? true : false;
        }



        ///<summary>    Valid path. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<param name="path">  Full pathname of the file. </param>
        ///<returns>    True if it succeeds, false if it fails. </returns>

        private bool validPath(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return true;
                }
                else
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                //Ignore exception
            }

            return false;
        }
        private SAPbouiCOM.Button Button1;
        private SAPbouiCOM.Button Button0;


      
    }
}
