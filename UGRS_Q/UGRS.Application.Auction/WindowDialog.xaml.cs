using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for WindowDialog.xaml
    /// </summary>
    public partial class WindowDialog : Window
    {
        public WindowDialog()
        {
            InitializeComponent();
            grContent.Focus();
        }

        public object gObject
        {
            get { return mObject; }
            set { mObject = value; }
        }
        object mObject;

        private void canvasTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr lObjWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(lObjWindowHandle).AddHook(new HwndSourceHook(WindowsHelper.WindowProc));
        }
    }
}
