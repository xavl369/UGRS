using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shell;
using UGRS.Core.Application.Utility;
using UGRS.Core.DTO.Log;

namespace UGRS.Core.Application.Forms.Log
{
    /// <summary>
    /// Interaction logic for LogViewerForm.xaml
    /// </summary>
    public partial class LogViewerForm : Window
    {
        public LogViewerForm()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsHelper.WindowProc));
        }

        public void SetLog(IList<LogDTO> lLstObjLog)
        {
            try
            {
                dgLog.ItemsSource = lLstObjLog;
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
