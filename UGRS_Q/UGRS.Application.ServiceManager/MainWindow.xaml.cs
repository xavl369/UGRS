using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Application.ServiceManager.Services;
using UGRS.Core.Application.Enum.Service;
using UGRS.Core.Application.Utility;
using UGRS.Core.Extension.Enum;

namespace UGRS.Application.ServiceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes

        private ServiceController mObjServiceController;
        private Thread mObjThread;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            cboServices.Items.Clear();
            cboServices.ItemsSource = System.Enum.GetValues(typeof(ServiceEnum)).Cast<ServiceEnum>();
            cboServices.SelectedItem = ServiceEnum.AUCTIONS;
        }

        #endregion

        #region Events

        #region Button

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            mObjThread = new Thread(StartService);
            mObjThread.Start();
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
            mObjThread = new Thread(StopService);
            mObjThread.Start();
        }

        private void btnClose_Click(object pObjSender, RoutedEventArgs pObjEventArgs)
        {
            this.Close();
        }

        #endregion

        #region TitleBar

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                DragMove();
            }
        }

        #endregion

        #region ComboBox

        private void cboServices_SelectionChanged(object pObjSender, SelectionChangedEventArgs pObjEventArgs)
        {
            try
            {
                mObjServiceController = new ServiceController();
                mObjServiceController.ServiceName = ((ServiceEnum)cboServices.SelectedItem).GetDescription();
                UpdateButtons();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }
        }

        #endregion

        #endregion

        #region Methods

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
                CustomMessageBox.Show("Error", lObjException.Message, this);
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

            switch ((ServiceEnum)cboServices.SelectedItem)
            {
                case ServiceEnum.AUCTIONS:
                    lObjServiceConfiguration = new AuctionsService(lStrConfigurationPath, this);
                    break;
                case ServiceEnum.BOARDS:
                    lObjServiceConfiguration = new BoardsService(lStrConfigurationPath, this);
                    break;
                case ServiceEnum.WEIGHING_MACHINE:
                    lObjServiceConfiguration = new WeighingMachineService(lStrConfigurationPath, this);
                    break;
            }

            lObjServiceConfiguration.ShowConfiguration(this);
        }

        private void ShowLogViewer()
        {
            LogViewer.Show("Log", GetLogPath(), this);
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
                CustomMessageBox.Show("Error", lObjException.Message, this);
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
                        SetInfo("Iniciado");
                        break;
                    case ServiceControllerStatus.Stopped:
                        btnStart.IsEnabled = true;
                        btnSettings.IsEnabled = true;
                        btnLog.IsEnabled = true;
                        SetInfo("Detenido");
                        break;
                    case ServiceControllerStatus.StartPending:
                        SetInfo("Iniciado...");
                        break;
                    case ServiceControllerStatus.StopPending:
                        SetInfo("Deteniendo...");
                        break;
                    case ServiceControllerStatus.Paused:
                        SetInfo("Pausado");
                        break;
                    case ServiceControllerStatus.PausePending:
                        SetInfo("Pausando...");
                        break;
                    case ServiceControllerStatus.ContinuePending:
                        SetInfo("Continuando...");
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
#if DEBUG
            var lObjDirectory = lObjFile.Directory.ToString();
#endif
            return Path.Combine(lObjFile.Directory.ToString(), "Service.log");
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

    }
}
