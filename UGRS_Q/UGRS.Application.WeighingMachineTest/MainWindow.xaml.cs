using System;
using System.Runtime.Remoting;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using UGRS.Core.Application.Utility;
using UGRS.Object.WeighingMachine;

namespace UGRS.Application.WeighingMachineTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WeighingMachineServerObject mObjWeighingMachine;
        private WrapperObject mObjWrapperObject;
        private Guid mObjConnection;
        private bool mBolConnected;

        public MainWindow()
        {
            InitializeComponent();
            lstHistory.Items.Clear();

            if (ConnectRemoteAccess())
            {
                mBolConnected = GetRemoteObject();
            }
        }

        #region Methods

        private bool ConnectRemoteAccess()
        {
            try
            {
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
                return true;
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
                return false;
            }
        }

        public bool GetRemoteObject()
        {
            try
            {
                //Objects
                mObjWeighingMachine = (WeighingMachineServerObject)Activator.GetObject(typeof(WeighingMachineServerObject), "http://localhost:8810/WeighingMachine");
                mObjWrapperObject = new WrapperObject();

                //Events
                mObjWeighingMachine.DataReceived += new WeighingMachineEventHandler(mObjWrapperObject.WrapperOnDataReceived);
                mObjWrapperObject.WrapperDataReceived += new WeighingMachineEventHandler(OnDataReceived);

                //Connection
                mObjConnection = mObjWeighingMachine.Connect();
                return true;
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
                return false;
            }
        }

        #endregion

        #region Events

        private void OnDataReceived(string pStrValue)
        {
            lstHistory.Dispatcher.Invoke((Action)delegate
            {
                lstHistory.Items.Add(pStrValue);
                lstHistory.SelectedIndex = lstHistory.Items.Count - 1;
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                DragMove();
            }
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (mBolConnected)
                {
                    mObjWeighingMachine.Disconnect(mObjConnection);
                }
            }
            catch(Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsHelper.WindowProc));
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lstHistory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            lstHistory.ScrollIntoView(lstHistory.SelectedItem);
        }

        #endregion

        
    }
}
