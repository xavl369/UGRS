using System;
using System.Runtime.Remoting;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using UGRS.Core.Application.Utility;
using UGRS.Core.Services;
using UGRS.Object.Boards;

namespace UGRS.Application.BoardsTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes

        private BoardsServerObject mObjBoards;
        private Guid mObjConnection;
        private bool mBolConnected;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            if (ConnectRemoteAccess())
            {
                mBolConnected = GetRemoteObject();
            }
        }

        #endregion

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
                mObjBoards = (BoardsServerObject)Activator.GetObject(typeof(BoardsServerObject), "http://localhost:8820/Boards");

                //Connection
                mObjConnection = mObjBoards.Connect();
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

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsHelper.WindowProc));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (mBolConnected)
                {
                    mObjBoards.Disconnect(mObjConnection);
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }
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

        private void btnSendDisplay1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogService.WriteInfo(txtHeadsNum_Display1.Text + " " + txtTotalWeight_Display1.Text + " " + txtAverageWeight_Display1.Text);
                mObjBoards.WriteDisplayOne(
                    txtHeadsNum_Display1.Text,
                    txtTotalWeight_Display1.Text,
                    txtAverageWeight_Display1.Text);
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }
        }

        private void btnSendDisplay2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mObjBoards.WriteDisplayTwo(
                    txtBatchNum_Display2.Text,
                    txtHeadsNum_Display2.Text,
                    txtTotalWeight_Display2.Text,
                    txtAverageWeight_Display2.Text,
                    txtBuyerNum_Display2.Text,
                    txtPrice_Display2.Text);
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
