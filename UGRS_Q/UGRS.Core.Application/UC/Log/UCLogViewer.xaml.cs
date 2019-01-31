using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.DTO.Log;

namespace UGRS.Core.Application.UC.Log
{
    /// <summary>
    /// Interaction logic for UCLogViewer.xaml
    /// </summary>
    public partial class UCLogViewer : UserControl
    {
        public UCLogViewer()
        {
            InitializeComponent();
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
            Window lObjParent = Window.GetWindow(this);
            lObjParent.Dispatcher.Invoke((Action)delegate
            {
                lObjParent.Close();
            });
        }
    }
}
