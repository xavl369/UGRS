using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using UGRS.Core.Application.Forms.Base;
using UGRS.Core.Application.UC.Buttons;
using UGRS.Core.Application.UC.Configuration;
using UGRS.Core.Application.Utility;
using UGRS.Core.Extension.Xml;
using UGRS.Core.Utility;

namespace UGRS.Application.ServiceManager.Services
{
    public class AuctionsService : IServiceConfiguration
    {
        #region Attributes

        //Documento
        private string mStrConfigurationPath;
        private XDocument mObjXDocument;

        //XAML
        private BaseForm mObjBase;
        private StackPanel mObjStackPanelOne;
        private StackPanel mObjStackPanelTwo;
        private UCTimeConfiguration mObjTimeConfiguration;
        private UCWarehouseConfiguration mObjWarehouseConfiguration;
        private UCSeriesConfiguration mObjSeriesConfiguration;
        private UCFoodConfiguration mObjFoodConfiguration;
        private UCComissionConfiguration mObjComissionConfiguration;
        private UCFinancialsConfiguration mObjFinancialsConfiguration;
        private UCSapBuisnessOneConfiguration mObjSapBuisnessOneConfiguration;
        private UCDataBaseConfiguration mObjDataBaseConfiguration;
        private UCEntityFrameworkConfiguration mObjEntityFrameworkConfiguration;
        private UCLogConfiguration mObjLogConfiguration;
        private UCSaveOrCancelButtons mObjButtons;

        #endregion

        #region Constructor

        public AuctionsService(string pStrConfigurationPath, Window pFrmWindows)
        {
            mStrConfigurationPath = pStrConfigurationPath;
            mObjXDocument = XmlUtility.GetXDocument(pStrConfigurationPath);

            InitializeForm(pFrmWindows);
            InitializeControls();
            ConfigureForm();
            LoadConfigurations();

            mObjButtons.btnSave.Click += btnSave_Click;
            mObjButtons.btnCancel.Click += btnCancel_Click;
        }

        #endregion

        #region Events

        private void btnSave_Click(object pObjSender, EventArgs pObjEventArgs)
        {
            SaveConfiguration();
        }

        private void btnCancel_Click(object pObjSender, EventArgs pObjEventArgs)
        {
            Window lObjParent = Window.GetWindow(mObjButtons);
            lObjParent.Close();
        }

        #endregion

        public bool? ShowConfiguration(Window pFrmWindows)
        {
            mObjBase.Owner = pFrmWindows;
            return mObjBase.ShowDialog();
        }

        private void InitializeForm(Window pFrmWindows)
        {
            mObjBase = new BaseForm(pFrmWindows);
            mObjStackPanelOne = new StackPanel();
            mObjStackPanelTwo = new StackPanel();

            //Column definitions
            ColumnDefinition lObjColumnOne = new ColumnDefinition();
            ColumnDefinition lObjColumnTwo = new ColumnDefinition();
            ColumnDefinition lObjColumnThree = new ColumnDefinition();

            //Row definitions
            RowDefinition lObjRowOne = new RowDefinition();
            RowDefinition lObjRowTwo = new RowDefinition();

            //Auto width
            lObjColumnOne.Width = new GridLength(1, GridUnitType.Star);
            lObjColumnTwo.Width = new GridLength(16);
            lObjColumnThree.Width = new GridLength(1, GridUnitType.Star);

            //Auto height
            lObjRowOne.Height = new GridLength(1, GridUnitType.Star);
            lObjRowTwo.Height = new GridLength(46);

            //Add definitions
            mObjBase.grdContainer.ColumnDefinitions.Add(lObjColumnOne);
            mObjBase.grdContainer.ColumnDefinitions.Add(lObjColumnTwo);
            mObjBase.grdContainer.ColumnDefinitions.Add(lObjColumnThree);
            mObjBase.grdContainer.RowDefinitions.Add(lObjRowOne);
            mObjBase.grdContainer.RowDefinitions.Add(lObjRowTwo);
        }

        private void InitializeControls()
        {
            mObjTimeConfiguration = new UCTimeConfiguration();
            mObjWarehouseConfiguration = new UCWarehouseConfiguration();
            mObjSeriesConfiguration = new UCSeriesConfiguration();
            mObjFoodConfiguration = new UCFoodConfiguration();
            mObjComissionConfiguration = new UCComissionConfiguration();
            mObjFinancialsConfiguration = new UCFinancialsConfiguration();
            mObjSapBuisnessOneConfiguration = new UCSapBuisnessOneConfiguration();
            mObjDataBaseConfiguration = new UCDataBaseConfiguration();
            mObjEntityFrameworkConfiguration = new UCEntityFrameworkConfiguration();
            mObjLogConfiguration = new UCLogConfiguration();
            mObjButtons = new UCSaveOrCancelButtons();

            mObjStackPanelOne.Children.Add(mObjTimeConfiguration);
            mObjStackPanelOne.Children.Add(mObjWarehouseConfiguration);
            mObjStackPanelOne.Children.Add(mObjFoodConfiguration);
            mObjStackPanelOne.Children.Add(mObjFinancialsConfiguration);
            mObjStackPanelOne.Children.Add(mObjSapBuisnessOneConfiguration);
            
            
            mObjStackPanelTwo.Children.Add(mObjLogConfiguration);
            mObjStackPanelTwo.Children.Add(mObjSeriesConfiguration);
            mObjStackPanelTwo.Children.Add(mObjComissionConfiguration);
            mObjStackPanelTwo.Children.Add(mObjEntityFrameworkConfiguration);
            mObjStackPanelTwo.Children.Add(mObjDataBaseConfiguration);
        }

        private void ConfigureForm()
        {
            mObjBase.tblTitle.Text = "Configuración";
            mObjBase.Width = 800;
            mObjBase.MinWidth = 800;
            mObjBase.MinHeight = 600;
            mObjBase.SizeToContent = SizeToContent.Height;

            Grid.SetColumn(mObjStackPanelOne, 0);
            Grid.SetColumn(mObjStackPanelTwo, 2);
            Grid.SetRow(mObjStackPanelOne, 0);
            Grid.SetRow(mObjStackPanelTwo, 0);
            Grid.SetRow(mObjButtons, 1);
            Grid.SetColumnSpan(mObjButtons, 3);

            mObjBase.grdContainer.Children.Add(mObjStackPanelOne);
            mObjBase.grdContainer.Children.Add(mObjStackPanelTwo);
            mObjBase.grdContainer.Children.Add(mObjButtons);
        }

        private void LoadConnectionString()
        {
            XElement lObjConnectionString = mObjXDocument.GetConnectionString();
            string lStrConnectionName = lObjConnectionString.Attribute("name").Value;
            string lStrConnectionString = lObjConnectionString.Attribute("connectionString").Value;
            SqlConnectionStringBuilder lObjConnectionStringBuilder = new SqlConnectionStringBuilder(lStrConnectionString);

            mObjEntityFrameworkConfiguration.txtEFConnectionName.Text = lStrConnectionName;
            mObjEntityFrameworkConfiguration.txtEFServer.Text = lObjConnectionStringBuilder["Data Source"] as string;
            mObjEntityFrameworkConfiguration.txtEFDataBase.Text = lObjConnectionStringBuilder["Initial Catalog"] as string;
            mObjEntityFrameworkConfiguration.txtEFUser.Text = lObjConnectionStringBuilder["User ID"] as string;
            mObjEntityFrameworkConfiguration.txtEFPassword.Password = lObjConnectionStringBuilder["Password"] as string;
        }

        private void LoadConfigurations()
        {
            try
            {
                //Time
                mObjTimeConfiguration.txtIntervalTime.Text = mObjXDocument.GetSetting("IntervalTime");

                //Warehouse
                mObjWarehouseConfiguration.txtAuctionsWarehouse.Text = mObjXDocument.GetSetting("AuctionsWarehouse");
                mObjWarehouseConfiguration.txtFoodWarehouse.Text = mObjXDocument.GetSetting("FoodWarehouse");
                mObjWarehouseConfiguration.txtCorralsWarehouse.Text = mObjXDocument.GetSetting("CorralsWarehouse");
                mObjWarehouseConfiguration.txtRejectionWarehouse.Text = mObjXDocument.GetSetting("RejectionWarehouse");

                //Series
                mObjSeriesConfiguration.txtDocumentSeries.Text = mObjXDocument.GetSetting("AuctionsSeriesName");
                mObjSeriesConfiguration.txtBusinessPartnerSeries.Text = mObjXDocument.GetSetting("BusinessPartnerSeriesName");

                //Food
                mObjFoodConfiguration.txtItemCode.Text = mObjXDocument.GetSetting("FoodItemCode");
                mObjFoodConfiguration.txtItemPrice.Text = mObjXDocument.GetSetting("FoodItemPrice");
                mObjFoodConfiguration.txtTaxCode.Text = mObjXDocument.GetSetting("FoodTaxCode");

                //Comission
                mObjComissionConfiguration.txtItemCode.Text = mObjXDocument.GetSetting("ComissionItemCode");
                mObjComissionConfiguration.txtTaxCode.Text = mObjXDocument.GetSetting("ComissionTaxCode");

                //Financials
                mObjFinancialsConfiguration.txtCostCenter.Text = mObjXDocument.GetSetting("CostCenter");
                mObjFinancialsConfiguration.txtDebtorsAccount.Text = mObjXDocument.GetSetting("DebtorsAccount");
                mObjFinancialsConfiguration.txtCreditorsAccount.Text = mObjXDocument.GetSetting("CreditorsAccount");
                mObjFinancialsConfiguration.txtGuidesAccount.Text = mObjXDocument.GetSetting("GuidesAccount");

                //SAP Buisiness One
                mObjSapBuisnessOneConfiguration.txtLicenseServer.Text = mObjXDocument.GetSetting("LicenseServer");
                mObjSapBuisnessOneConfiguration.txtUserName.Text = mObjXDocument.GetSetting("UserName");
                mObjSapBuisnessOneConfiguration.txtPassword.Password = mObjXDocument.GetSetting("Password");
                mObjSapBuisnessOneConfiguration.txtLanguage.Text = mObjXDocument.GetSetting("Language");

                //Data base
                mObjDataBaseConfiguration.txtDbServerType.Text = mObjXDocument.GetSetting("DbServerType");
                mObjDataBaseConfiguration.txtDbServer.Text = mObjXDocument.GetSetting("SQLServer");
                mObjDataBaseConfiguration.txtDbUser.Text = mObjXDocument.GetSetting("SQLUserName");
                mObjDataBaseConfiguration.txtDbPassword.Password = mObjXDocument.GetSetting("SQLPassword");
                mObjDataBaseConfiguration.txtDataBase.Text = mObjXDocument.GetSetting("DataBaseName");

                //Log
                mObjLogConfiguration.tbnFullLog.IsChecked = mObjXDocument.GetSetting("FullLog").Equals("True") || mObjXDocument.GetSetting("FullLog").Equals("true");

                //EntityFramework
                 LoadConnectionString();
            }
            catch(Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, null);
            }
        }

        private void GetNewConfiguration()
        {
            //Time
            mObjXDocument.SetSetting("IntervalTime", mObjTimeConfiguration.txtIntervalTime.Text);

            //Warehouse
            mObjXDocument.SetSetting("AuctionsWarehouse", mObjWarehouseConfiguration.txtAuctionsWarehouse.Text);
            mObjXDocument.SetSetting("FoodWarehouse", mObjWarehouseConfiguration.txtFoodWarehouse.Text);
            mObjXDocument.SetSetting("CorralsWarehouse", mObjWarehouseConfiguration.txtCorralsWarehouse.Text);
            mObjXDocument.SetSetting("RejectionWarehouse", mObjWarehouseConfiguration.txtRejectionWarehouse.Text);

            //Series
            mObjXDocument.SetSetting("AuctionsSeriesName", mObjSeriesConfiguration.txtDocumentSeries.Text);
            mObjXDocument.SetSetting("BusinessPartnerSeriesName", mObjSeriesConfiguration.txtBusinessPartnerSeries.Text);

            //Food
            mObjXDocument.SetSetting("FoodItemCode", mObjFoodConfiguration.txtItemCode.Text);
            mObjXDocument.SetSetting("FoodItemPrice", mObjFoodConfiguration.txtItemPrice.Text);
            mObjXDocument.SetSetting("FoodTaxCode", mObjFoodConfiguration.txtTaxCode.Text);

            //Comission
            mObjXDocument.SetSetting("ComissionItemCode", mObjComissionConfiguration.txtItemCode.Text);
            mObjXDocument.SetSetting("ComissionTaxCode", mObjComissionConfiguration.txtTaxCode.Text);

            //Financials
            mObjXDocument.SetSetting("CostCenter", mObjFinancialsConfiguration.txtCostCenter.Text);
            mObjXDocument.SetSetting("DebitAccount", mObjFinancialsConfiguration.txtDebtorsAccount.Text);
            mObjXDocument.SetSetting("CreditAccount", mObjFinancialsConfiguration.txtCreditorsAccount.Text);

            //SAP Buisiness One
            mObjXDocument.SetSetting("LicenseServer", mObjSapBuisnessOneConfiguration.txtLicenseServer.Text);
            mObjXDocument.SetSetting("UserName", mObjSapBuisnessOneConfiguration.txtUserName.Text);
            mObjXDocument.SetSetting("Password", mObjSapBuisnessOneConfiguration.txtPassword.Password);
            mObjXDocument.SetSetting("Language", mObjSapBuisnessOneConfiguration.txtLanguage.Text);

            //Data base
            mObjXDocument.SetSetting("DbServerType", mObjDataBaseConfiguration.txtDbServerType.Text);
            mObjXDocument.SetSetting("SQLServer", mObjDataBaseConfiguration.txtDbServer.Text);
            mObjXDocument.SetSetting("SQLUserName", mObjDataBaseConfiguration.txtDbUser.Text);
            mObjXDocument.SetSetting("SQLPassword", mObjDataBaseConfiguration.txtDbPassword.Password);
            mObjXDocument.SetSetting("DataBaseName", mObjDataBaseConfiguration.txtDataBase.Text);

            //Log
            mObjXDocument.SetSetting("FullLog", (mObjLogConfiguration.tbnFullLog.IsChecked == true ? "True" : "False"));

            //EntityFramework
            GetNewConnectionString();
        }

        private void GetNewConnectionString()
        {
            SqlConnectionStringBuilder lObjConnectionStringBuilder = new SqlConnectionStringBuilder();
            lObjConnectionStringBuilder["Data Source"] = mObjEntityFrameworkConfiguration.txtEFServer.Text;
            lObjConnectionStringBuilder["Initial Catalog"] = mObjEntityFrameworkConfiguration.txtEFDataBase.Text;
            lObjConnectionStringBuilder["User ID"] = mObjEntityFrameworkConfiguration.txtEFUser.Text;
            lObjConnectionStringBuilder["Password"] = mObjEntityFrameworkConfiguration.txtEFPassword.Password;

            mObjXDocument.SetConnectionStringName(mObjEntityFrameworkConfiguration.txtEFConnectionName.Text);
            mObjXDocument.SetConnectionString(lObjConnectionStringBuilder.ToString());
        }

        private void SaveConfiguration()
        {
            try
            {
                if (mObjTimeConfiguration.Valid() && 
                    mObjWarehouseConfiguration.Valid() &&
                    mObjSeriesConfiguration.Valid() &&
                    mObjFoodConfiguration.Valid() && 
                    mObjComissionConfiguration.Valid() && 
                    mObjFinancialsConfiguration.Valid() && 
                    mObjSapBuisnessOneConfiguration.Valid() && 
                    mObjDataBaseConfiguration.Valid() && 
                    mObjEntityFrameworkConfiguration.Valid())
                {
                    GetNewConfiguration();
                    mObjXDocument.Save(mStrConfigurationPath);

                    CustomMessageBox.Show("Información", "Los cambios se guardaron correctamente.", mObjBase);
                    mObjBase.Close();
                }
                else
                {
                    CustomMessageBox.Show("Alerta", "Favor de completar los campos.", mObjBase);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, mObjBase);
            }
        }
    }
}