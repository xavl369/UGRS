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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UGRS.Subastas.Pages
{
    /// <summary>
    /// Interaction logic for FormDetalleSubasta.xaml
    /// </summary>
    public partial class FormDetalleSubasta : Window
    {
        public FormDetalleSubasta()
        {
            InitializeComponent();
        }

        private void dgTest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            this.DragMove();
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvasTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvasTop_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
