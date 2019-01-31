using System;
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

namespace UGRS.Application.ServiceManager.Services
{
    public class BoardsService : IServiceConfiguration
    {
        #region Attributes

        //Documento
        private string mStrConfigurationPath;
        private XDocument mObjXDocument;

        //XAML
        private BaseForm mObjBase;
        private StackPanel mObjStackPanelOne;
        private StackPanel mObjStackPanelTwo;
        private UCLocationConfiguration mObjLocationConfiguration;
        private UCLogConfiguration mObjLogConfiguration;
        private UCRemotingConfiguration mObjRemotingConfiguration;
        private UCBoardsConfiguration mObjBoardsConfiguration;
        private UCSaveOrCancelButtons mObjButtons;

        #endregion

        #region Constructor

        public BoardsService(string pStrConfigurationPath, Window pFrmWindows)
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
            mObjLocationConfiguration = new UCLocationConfiguration();
            mObjLogConfiguration = new UCLogConfiguration();
            mObjRemotingConfiguration = new UCRemotingConfiguration();
            mObjBoardsConfiguration = new UCBoardsConfiguration();
            mObjButtons = new UCSaveOrCancelButtons();

            mObjStackPanelOne.Children.Add(mObjLocationConfiguration);
            mObjStackPanelOne.Children.Add(mObjLogConfiguration);
            mObjStackPanelOne.Children.Add(mObjRemotingConfiguration);
            mObjStackPanelTwo.Children.Add(mObjBoardsConfiguration);
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

        private void LoadConfigurations()
        {
            try
            {
                //Location
                mObjLocationConfiguration.txtLocation.Text = mObjXDocument.GetSetting("Location");

                //Log
                mObjLogConfiguration.tbnFullLog.IsChecked = mObjXDocument.GetSetting("FullLog").Equals("True");

                //Remoting
                mObjRemotingConfiguration.txtChannel.Text = mObjXDocument.GetRemotingChannel();
                mObjRemotingConfiguration.txtPort.Text = mObjXDocument.GetRemotingPort();

                //Boards
                if (mObjXDocument.GetSetting("Location").Equals("HERMOSILLO"))
                {
                    mObjBoardsConfiguration.txtDisplay1PortName.Text = mObjXDocument.GetSetting("PortName");
                    mObjBoardsConfiguration.txtDisplay1BaudRate.Text = mObjXDocument.GetSetting("BaudRate");
                    mObjBoardsConfiguration.txtDisplay1DataBits.Text = mObjXDocument.GetSetting("DataBits");
                    mObjBoardsConfiguration.txtDisplay1Parity.Text = mObjXDocument.GetSetting("Parity");
                    mObjBoardsConfiguration.txtDisplay1StopBits.Text = mObjXDocument.GetSetting("StopBits");
                    mObjBoardsConfiguration.txtDisplay1ReadTimeout.Text = mObjXDocument.GetSetting("ReadTimeout");

                    mObjBoardsConfiguration.txtDisplay2PortName.Text = mObjXDocument.GetSetting("PortName");
                    mObjBoardsConfiguration.txtDisplay2BaudRate.Text = mObjXDocument.GetSetting("BaudRate");
                    mObjBoardsConfiguration.txtDisplay2DataBits.Text = mObjXDocument.GetSetting("DataBits");
                    mObjBoardsConfiguration.txtDisplay2Parity.Text = mObjXDocument.GetSetting("Parity");
                    mObjBoardsConfiguration.txtDisplay2StopBits.Text = mObjXDocument.GetSetting("StopBits");
                    mObjBoardsConfiguration.txtDisplay2ReadTimeout.Text = mObjXDocument.GetSetting("ReadTimeout");
                }
                else
                {
                    mObjBoardsConfiguration.txtDisplay1PortName.Text = mObjXDocument.GetSetting("Display1_PortName");
                    mObjBoardsConfiguration.txtDisplay1BaudRate.Text = mObjXDocument.GetSetting("Display1_BaudRate");
                    mObjBoardsConfiguration.txtDisplay1DataBits.Text = mObjXDocument.GetSetting("Display1_DataBits");
                    mObjBoardsConfiguration.txtDisplay1Parity.Text = mObjXDocument.GetSetting("Display1_Parity");
                    mObjBoardsConfiguration.txtDisplay1StopBits.Text = mObjXDocument.GetSetting("Display1_StopBits");
                    mObjBoardsConfiguration.txtDisplay1ReadTimeout.Text = mObjXDocument.GetSetting("Display1_ReadTimeout");
                    mObjBoardsConfiguration.txtDisplay2PortName.Text = mObjXDocument.GetSetting("Display2_PortName");
                    mObjBoardsConfiguration.txtDisplay2BaudRate.Text = mObjXDocument.GetSetting("Display2_BaudRate");
                    mObjBoardsConfiguration.txtDisplay2DataBits.Text = mObjXDocument.GetSetting("Display2_DataBits");
                    mObjBoardsConfiguration.txtDisplay2Parity.Text = mObjXDocument.GetSetting("Display2_Parity");
                    mObjBoardsConfiguration.txtDisplay2StopBits.Text = mObjXDocument.GetSetting("Display2_StopBits");
                    mObjBoardsConfiguration.txtDisplay2ReadTimeout.Text = mObjXDocument.GetSetting("Display2_ReadTimeout");
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, null);
            }
        }

        private void GetNewConfiguration()
        {
            try
            {
                //Location
                mObjXDocument.SetSetting("Location", mObjLocationConfiguration.txtLocation.Text);

                //Log
                mObjXDocument.SetSetting("FullLog", (mObjLogConfiguration.tbnFullLog.IsChecked == true ? "True" : "False"));

                //Remoting
                //Remoting
                mObjXDocument.SetRemotingChannel(mObjRemotingConfiguration.txtChannel.Text);
                mObjXDocument.SetRemotingPort(mObjRemotingConfiguration.txtPort.Text);
                mObjXDocument.SetSetting("ChannelName", mObjRemotingConfiguration.txtChannel.Text);
                mObjXDocument.SetSetting("ChannelPort", mObjRemotingConfiguration.txtPort.Text);

                //Boards
                if (mObjXDocument.GetSetting("Location").Equals("HERMOSILLO"))
                {
                    mObjXDocument.SetSetting("PortName", mObjBoardsConfiguration.txtDisplay1PortName.Text);
                    mObjXDocument.SetSetting("BaudRate", mObjBoardsConfiguration.txtDisplay1BaudRate.Text);
                    mObjXDocument.SetSetting("DataBits", mObjBoardsConfiguration.txtDisplay1DataBits.Text);
                    mObjXDocument.SetSetting("Parity", mObjBoardsConfiguration.txtDisplay1Parity.Text);
                    mObjXDocument.SetSetting("StopBits", mObjBoardsConfiguration.txtDisplay1StopBits.Text);
                    mObjXDocument.SetSetting("ReadTimeout", mObjBoardsConfiguration.txtDisplay1ReadTimeout.Text);
                }
                else
                {
                    mObjXDocument.SetSetting("Display1_PortName", mObjBoardsConfiguration.txtDisplay1PortName.Text);
                    mObjXDocument.SetSetting("Display1_BaudRate", mObjBoardsConfiguration.txtDisplay1BaudRate.Text);
                    mObjXDocument.SetSetting("Display1_DataBits", mObjBoardsConfiguration.txtDisplay1DataBits.Text);
                    mObjXDocument.SetSetting("Display1_Parity", mObjBoardsConfiguration.txtDisplay1Parity.Text);
                    mObjXDocument.SetSetting("Display1_StopBits", mObjBoardsConfiguration.txtDisplay1StopBits.Text);
                    mObjXDocument.SetSetting("Display1_ReadTimeout", mObjBoardsConfiguration.txtDisplay1ReadTimeout.Text);
                    mObjXDocument.SetSetting("Display2_PortName", mObjBoardsConfiguration.txtDisplay2PortName.Text);
                    mObjXDocument.SetSetting("Display2_BaudRate", mObjBoardsConfiguration.txtDisplay2BaudRate.Text);
                    mObjXDocument.SetSetting("Display2_DataBits", mObjBoardsConfiguration.txtDisplay2DataBits.Text);
                    mObjXDocument.SetSetting("Display2_Parity", mObjBoardsConfiguration.txtDisplay2Parity.Text);
                    mObjXDocument.SetSetting("Display2_StopBits", mObjBoardsConfiguration.txtDisplay2StopBits.Text);
                    mObjXDocument.SetSetting("Display2_ReadTimeout", mObjBoardsConfiguration.txtDisplay2ReadTimeout.Text);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, null);
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                if (mObjLocationConfiguration.Valid() &&
                    mObjLogConfiguration.Valid() &&
                    mObjRemotingConfiguration.Valid() &&
                    mObjBoardsConfiguration.Valid())
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
