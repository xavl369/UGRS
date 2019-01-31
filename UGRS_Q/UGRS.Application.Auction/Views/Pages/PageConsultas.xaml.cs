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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UGRS.Subastas.Pages;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for PageConsultas.xaml
    /// </summary>
    public partial class PageConsultas : Page
    {
        public PageConsultas()
        {
            InitializeComponent();

            var data = new Test { Test1 = "Test1", Test2 = "Test2" };
            List<Test> lLstTest = new List<Test>();
            lLstTest.Add(data);
            dgTest.ItemsSource = lLstTest;
        }

        private void dgenrut_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FormDetalleSubasta dialog = new FormDetalleSubasta();
            dialog.Show();
        }
    }

    public class Test
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
    }
}
