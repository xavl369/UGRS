using System;
using System.Windows;
using System.Windows.Controls;

namespace UGRS.Core.Application.UC.Message
{
    /// <summary>
    /// Interaction logic for UCOption.xaml
    /// </summary>
    public partial class UCOption : UserControl
    {
        public UCOption()
        {
            InitializeComponent();
        }

        private void btnOption1_Click(object sender, RoutedEventArgs e)
        {
            Window lObjParent = Window.GetWindow(this);
            lObjParent.DialogResult = true;
            CloseDialog();
            
        }

        private void btnOption2_Click(object sender, RoutedEventArgs e)
        {
            Window lObjParent = Window.GetWindow(this);
            lObjParent.DialogResult = false;
            CloseDialog();
        }

        private void btnOption3_Click(object sender, RoutedEventArgs e)
        {
            Window lObjParent = Window.GetWindow(this);
            lObjParent.DialogResult = null;
            CloseDialog();
        }

        private void CloseDialog()
        {
            Window lObjParent = Window.GetWindow(this);
            lObjParent.Dispatcher.Invoke((Action)delegate
            {
                lObjParent.Close();
            });
        }
    }
}
