using System;
using System.Windows;
using System.Windows.Controls;

namespace UGRS.Core.Application.UC.Message
{
    /// <summary>
    /// Interaction logic for UCMessage.xaml
    /// </summary>
    public partial class UCMessage : UserControl
    {
        public UCMessage()
        {
            InitializeComponent();
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
