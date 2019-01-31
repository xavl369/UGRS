using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using UGRS.Core.Application.Utility;

namespace UGRS.Core.Application.Forms.Base
{
    public partial class BaseForm : Window
    {
        private object mObjResultObject;

        public object ResultObject
        {
            get { return mObjResultObject; }
            set { mObjResultObject = value; }
        }

        public BaseForm()
        {
            InitializeComponent();
             this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public BaseForm(Window pObjOwner)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            this.Owner = pObjOwner;
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                DragMove();
            }
            if (e.ClickCount == 2 && this.ResizeMode != ResizeMode.NoResize)
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

        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsHelper.WindowProc));
        }
    }
}
