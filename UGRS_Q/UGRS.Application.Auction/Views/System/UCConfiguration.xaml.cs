using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management;
using System.Printing;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using UGRS.Application.ServiceManager.Services;
using UGRS.Core.Application.Access;
using UGRS.Core.Application.Enum.Service;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Auctions.DTO.Users;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Extension.Xml;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Application.Auctions.Extensions;
using WPF.MDI;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCConfiguration.xaml
    /// </summary>
    public partial class UCConfiguration : UserControl
    {
        #region Attributes

        private ServiceController mObjServiceController;
        private UsersServicesFactory mObjUserFactory;
        private XDocument mObjXDocument;
        private Thread mObjServiceThread;
        private Thread mObjWorker;
        private string mStrConfigurationPath;

        #endregion

        #region Constructor

        public UCConfiguration()
        {
            InitializeComponent();
            mObjUserFactory = new UsersServicesFactory();

            cboPrinterBatches.Items.Clear();
            cboPrinterReports.Items.Clear();
            cboService.Items.Clear();

            mStrConfigurationPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            mObjXDocument = XmlUtility.GetXDocument(mStrConfigurationPath);
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            grdLoading.Visibility = Visibility.Visible;
            mObjWorker = new Thread(LoadConfigurations);
            mObjWorker.Start();

            SetWindowsState(WindowState.Normal);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (grdGeneralSettings.Valid() && grdDataBase.Valid())
            {
                Auctions.Properties.Settings.Default.Save();

                GetNewConnectionString();
                mObjXDocument.Save(mStrConfigurationPath);

                CustomMessageBox.Show("Información", "Los cambios se guardaron correctamente.", Window.GetWindow(this));
                this.CloseInternalForm();
            }
            else
            {
                CustomMessageBox.Show("Error", "Favor de completar los campos.", Window.GetWindow(this));
            }
        }

        #region Services

        private void cboService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mObjServiceController = new ServiceController();
                mObjServiceController.ServiceName = ((ServiceEnum)cboService.SelectedItem).GetDescription();
                UpdateButtons();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void cboPrinterBathes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Auctions.Properties.Settings.Default.gStrBatchPrinter = cboPrinterBatches.SelectedItem as string;
        }

        private void cboPrinterReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Auctions.Properties.Settings.Default.gStrReportPrinter = cboPrinterReports.SelectedItem as string;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            mObjServiceThread = new Thread(StartService);
            mObjServiceThread.Start();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowServiceConfiguration();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            ShowLogViewer();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mObjServiceThread = new Thread(StopService);
            mObjServiceThread.Start();
        }

        #endregion

        #region Remote Access

        private void cboAccessService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IChannel[] lLstObjRegisteredChannels = ChannelServices.RegisteredChannels;

            foreach (IChannel lObjChannel in lLstObjRegisteredChannels)
            {
                if(lObjChannel.ChannelName.Contains(GetServiceName(sender)))
                {
                    btnAccessSettings.IsEnabled = false;
                    btnAccessTest.IsEnabled = false;
                    return;
                }
            }

            btnAccessSettings.IsEnabled = true;
            btnAccessTest.IsEnabled = false;
        }

        private void btnAccessSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowAccessConfiguration();
        }

        private void btnAcessTest_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
        
        #endregion

        #region Methods

        #region Form

        private void SetWindowsState(WindowState pEnmWindowState)
        {
            try
            {
                this.GetInternalParent().WindowState = pEnmWindowState;
            }
            catch
            {
                this.GetParent().WindowState = pEnmWindowState;
            }
        }

        #endregion

        #region Loads

        private void LoadConfigurations()
        {
            try
            {
                LoadPrintersList();
                LoadServicesList();
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
                });
            }
            finally
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    grdLoading.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void LoadPrintersList()
        {
            LocalPrintServer lObjPrintServer = new LocalPrintServer();

            PrintQueueCollection lObjPrintQueuesOnLocalServer = lObjPrintServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            List<string> lLstStrPrinters = new List<string>();

            foreach (PrintQueue lObjPrinter in lObjPrintQueuesOnLocalServer)
            {
                lLstStrPrinters.Add(lObjPrinter.FullName);
            }

            this.Dispatcher.Invoke((Action)delegate
            {
                cboPrinterBatches.ItemsSource = lLstStrPrinters;
                cboPrinterBatches.SelectedItem = Auctions.Properties.Settings.Default.gStrBatchPrinter;

                cboPrinterReports.ItemsSource = lLstStrPrinters;
                cboPrinterReports.SelectedItem = Auctions.Properties.Settings.Default.gStrReportPrinter;

                LoadConnectionString();
            });
        }

        private void LoadServicesList()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (GetActiveSession())
                {
                    cboService.ItemsSource = System.Enum.GetValues(typeof(ServiceEnum)).Cast<ServiceEnum>();
                    cboService.SelectedItem = ServiceEnum.AUCTIONS;
                    cboAccessService.ItemsSource = System.Enum.GetValues(typeof(AccessEnum)).Cast<AccessEnum>();
                    cboAccessService.SelectedItem = AccessEnum.WEIGHING_MACHINE;
                }
                else
                {
                    grdServices.Visibility = Visibility.Collapsed;
                    grdRemotoAccess.Visibility = Visibility.Collapsed;
                }
            });
        }

        private bool GetActiveSession()
        {
            SessionDTO lObjCurrentSession = (SessionDTO)StaticSessionUtility.GetCurrentSession();
            if (lObjCurrentSession != null)
            {
                UserDTO pObjUserDto = mObjUserFactory.GetUserService().GetUserDTO(lObjCurrentSession.Id);

                if (pObjUserDto.UserType.Equals("Administrador"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private void LoadConnectionString()
        {
            XElement lObjConnectionString = mObjXDocument.GetConnectionString();
            string lStrConnectionName = lObjConnectionString.Attribute("name").Value;
            string lStrConnectionString = lObjConnectionString.Attribute("connectionString").Value;
            SqlConnectionStringBuilder lObjConnectionStringBuilder = new SqlConnectionStringBuilder(lStrConnectionString);

            txtConnectionName.Text = lStrConnectionName;
            txtServer.Text = lObjConnectionStringBuilder["Data Source"] as string;
            txtDataBase.Text = lObjConnectionStringBuilder["Initial Catalog"] as string;
            txtUser.Text = lObjConnectionStringBuilder["User ID"] as string;
            txtPassword.Password = lObjConnectionStringBuilder["Password"] as string;
        }

        #endregion

        #region Services

        private void StartService()
        {
            DisableButtons();

            try
            {
                if (mObjServiceController.Status == ServiceControllerStatus.Stopped)
                {
                    SetInfo("Iniciando servicio...");

                    try
                    {
                        mObjServiceController.Start();
                        mObjServiceController.WaitForStatus(ServiceControllerStatus.Running);
                        SetInfo("Listo.");
                    }
                    catch (InvalidOperationException)
                    {

                        if (!IsAdministrator())
                        {
                            SetInfo("Favor de iniciar la aplicación como Administrador.");
                        }
                        else
                        {
                            SetInfo("No se pudo iniciar el servicio.");
                        }
                    }
                }
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
                });
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void ShowServiceConfiguration()
        {
            string lStrConfigurationPath = GetConfigurationPath();
            IServiceConfiguration lObjServiceConfiguration = null;

            switch ((ServiceEnum)cboService.SelectedItem)
            {
                case ServiceEnum.AUCTIONS:
                    lObjServiceConfiguration = new AuctionsService(lStrConfigurationPath, Window.GetWindow(this));
                    break;
                case ServiceEnum.BOARDS:
                    lObjServiceConfiguration = new BoardsService(lStrConfigurationPath, Window.GetWindow(this));
                    break;
                case ServiceEnum.WEIGHING_MACHINE:
                    lObjServiceConfiguration = new WeighingMachineService(lStrConfigurationPath, Window.GetWindow(this));
                    break;
            }

            lObjServiceConfiguration.ShowConfiguration(Window.GetWindow(this));
        }

        private void ShowLogViewer()
        {
            LogViewer.Show("Log", GetLogPath(), Window.GetWindow(this));
        }

        private void StopService()
        {
            DisableButtons();

            try
            {
                if (mObjServiceController.Status == ServiceControllerStatus.Running)
                {
                    SetInfo("Deteniendo servicio...");

                    try
                    {
                        mObjServiceController.Stop();
                        mObjServiceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        SetInfo("Listo.");
                    }
                    catch (InvalidOperationException)
                    {
                        SetInfo("No se pudo detener el servicio.");
                    }
                }
            }
            catch (Exception lObjException)
            {
                this.Dispatcher.Invoke(() =>
                {
                    CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
                });
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void SetInfo(string pStrInformation)
        {
            tblInfo.Dispatcher.Invoke((Action)delegate
            {
                tblInfo.Text = pStrInformation;
            });
        }

        private void UpdateButtons()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                DisableButtons();
                EnableButtons();
            });
        }

        private void EnableButtons()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                switch (mObjServiceController.Status)
                {
                    case ServiceControllerStatus.Running:
                        btnStop.IsEnabled = true;
                        btnLog.IsEnabled = true;
                        break;
                    case ServiceControllerStatus.Stopped:
                        btnStart.IsEnabled = true;
                        btnSettings.IsEnabled = true;
                        btnLog.IsEnabled = true;
                        break;
                }
            });
        }

        private void DisableButtons()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                btnStart.IsEnabled = false;
                btnSettings.IsEnabled = false;
                btnLog.IsEnabled = false;
                btnStop.IsEnabled = false;
            });
        }

        private string GetServicePath()
        {
            string ServiceName = mObjServiceController.ServiceName;

            using (ManagementObject lObjManagementObject = new ManagementObject(string.Format("Win32_Service.Name='{0}'", mObjServiceController.ServiceName)))
            {
                lObjManagementObject.Get();
                return lObjManagementObject["PathName"].ToString().Replace("\"", "");
            }
        }

        private string GetLogPath()
        {
            FileInfo lObjFile = new FileInfo(GetServicePath());
            var a = lObjFile.Directory.ToString();
            return System.IO.Path.Combine(lObjFile.Directory.ToString(), "Service.log");
        }

        private string GetConfigurationPath()
        {
            return string.Format("{0}.config", GetServicePath().Replace("\"", ""));
        }

        private bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        #endregion

        #region Remote Access

        private string GetServiceName(object pObjSender)
        {
            return ((ServiceEnum)(pObjSender as ComboBox).SelectedItem).GetDescription().Replace("Service", "");
        }

        private void ShowAccessConfiguration()
        {
            IAccessConfiguration lObjServiceConfiguration = null;

            switch ((AccessEnum)cboAccessService.SelectedItem)
            {
                case AccessEnum.BOARDS:
                    lObjServiceConfiguration = new BoardsAccess(PathUtilities.GetCurrent("Configurations\\Boards.config"), Window.GetWindow(this));
                    break;
                case AccessEnum.WEIGHING_MACHINE:
                    lObjServiceConfiguration = new WeighingMachineAccess(PathUtilities.GetCurrent("Configurations\\WeighingMachine.config"), Window.GetWindow(this));
                    break;
            }

            lObjServiceConfiguration.ShowConfiguration(Window.GetWindow(this));
        }

        #endregion

        #region Database

        private void GetNewConnectionString()
        {
            SqlConnectionStringBuilder lObjConnectionStringBuilder = new SqlConnectionStringBuilder();
            lObjConnectionStringBuilder["Data Source"] = txtServer.Text;
            lObjConnectionStringBuilder["Initial Catalog"] = txtDataBase.Text;
            lObjConnectionStringBuilder["User ID"] = txtUser.Text;
            lObjConnectionStringBuilder["Password"] = txtPassword.Password;

            mObjXDocument.SetConnectionStringName(txtConnectionName.Text);
            mObjXDocument.SetConnectionString(lObjConnectionStringBuilder.ToString());
        }

        #endregion

        #endregion
    }
}