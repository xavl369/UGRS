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
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Core.Application.Access
{
    public class WeighingMachineAccess : IAccessConfiguration
    {
        #region Attributes

        //Documento
        private string mStrConfigurationPath;
        private XDocument mObjXDocument;

        //XAML
        private BaseForm mObjBase;
        private StackPanel mObjStackPanelOne;
        private StackPanel mObjStackPanelTwo;
        private UCModeConfiguration mObjModeConfiguration;
        private UCLogConfiguration mObjLogConfiguration;
        private UCLocationConfiguration mObjLocationConfiguration;
        private UCWeighingMachineConfiguration mObjWeighingMachineConfiguration;
        private UCRemotingConfiguration mObjRemotingConfiguration;
        private UCSaveOrCancelButtons mObjButtons;

        #endregion

        #region Constructor

        public WeighingMachineAccess(string pStrConfigurationPath, Window pFrmWindows)
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
            mObjModeConfiguration = new UCModeConfiguration();
            mObjLogConfiguration = new UCLogConfiguration();
            mObjRemotingConfiguration = new UCRemotingConfiguration();
            mObjLocationConfiguration = new UCLocationConfiguration();
            mObjWeighingMachineConfiguration = new UCWeighingMachineConfiguration();

            mObjButtons = new UCSaveOrCancelButtons();

            mObjStackPanelOne.Children.Add(mObjModeConfiguration);
            mObjStackPanelOne.Children.Add(mObjLogConfiguration);
            mObjStackPanelOne.Children.Add(mObjRemotingConfiguration);
            mObjStackPanelTwo.Children.Add(mObjLocationConfiguration);
            mObjStackPanelTwo.Children.Add(mObjWeighingMachineConfiguration);
        }

        private void ConfigureForm()
        {
            mObjBase.tblTitle.Text = "Configuración";
            mObjBase.Width = 800;
            mObjBase.MinWidth = 800;
            mObjBase.MinHeight = 400;
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

        private void LoadConfigurations()
        {
            try
            {
                //Log
                mObjLogConfiguration.tbnFullLog.IsChecked = mObjXDocument.GetSetting("FullLog").Equals("True");

                //Mode
                mObjModeConfiguration.tbnVirtualMode.IsChecked = mObjXDocument.GetSetting("VirtualMode").Equals("True");

                //Remoting
                mObjRemotingConfiguration.txtChannel.Text = mObjXDocument.GetRemotingChannel();
                mObjRemotingConfiguration.txtPort.Text = mObjXDocument.GetRemotingPort();

                //Location 
                mObjLocationConfiguration.txtLocation.Text = mObjXDocument.GetSetting("Location");

                //WeighingMachine
                mObjWeighingMachineConfiguration.txtPortName.Text = mObjXDocument.GetSetting("PortName");
                mObjWeighingMachineConfiguration.txtBaudRate.Text = mObjXDocument.GetSetting("BaudRate");
                mObjWeighingMachineConfiguration.txtDataBits.Text = mObjXDocument.GetSetting("DataBits");
                mObjWeighingMachineConfiguration.txtParity.Text = mObjXDocument.GetSetting("Parity");
                mObjWeighingMachineConfiguration.txtStopBits.Text = mObjXDocument.GetSetting("StopBits");
                mObjWeighingMachineConfiguration.txtReadTimeout.Text = mObjXDocument.GetSetting("ReadTimeout");
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, null);
            }
        }

        private void GetNewConfiguration()
        {
            //Log
            mObjXDocument.SetSetting("FullLog", (mObjLogConfiguration.tbnFullLog.IsChecked == true ? "True" : "False"));

            //Mode
            mObjXDocument.SetSetting("VirtualMode", (mObjModeConfiguration.tbnVirtualMode.IsChecked == true ? "True" : "False"));

            //Remoting
            mObjXDocument.SetRemotingChannel(mObjRemotingConfiguration.txtChannel.Text);
            mObjXDocument.SetRemotingPort(mObjRemotingConfiguration.txtPort.Text);
            mObjXDocument.SetSetting("ChannelName", mObjRemotingConfiguration.txtChannel.Text);
            mObjXDocument.SetSetting("ChannelPort", mObjRemotingConfiguration.txtPort.Text);

            //Location 
            mObjXDocument.SetSetting("Location", mObjLocationConfiguration.txtLocation.Text);

            //WeighingMachine
            mObjXDocument.SetSetting("PortName", mObjWeighingMachineConfiguration.txtPortName.Text);
            mObjXDocument.SetSetting("BaudRate", mObjWeighingMachineConfiguration.txtBaudRate.Text);
            mObjXDocument.SetSetting("DataBits", mObjWeighingMachineConfiguration.txtDataBits.Text);
            mObjXDocument.SetSetting("Parity", mObjWeighingMachineConfiguration.txtParity.Text);
            mObjXDocument.SetSetting("StopBits", mObjWeighingMachineConfiguration.txtStopBits.Text);
            mObjXDocument.SetSetting("ReadTimeout", mObjWeighingMachineConfiguration.txtReadTimeout.Text);
        }

        private void SaveConfiguration()
        {
            try
            {
                if (mObjModeConfiguration.Valid() && 
                    mObjLocationConfiguration.Valid() && 
                    mObjWeighingMachineConfiguration.Valid() && 
                    mObjRemotingConfiguration.Valid())
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
