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
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCBatchesHistory.xaml
    /// </summary>
    public partial class UCAuctionsHistoryDetail : UserControl
    {   
        private AuctionsServicesFactory mObjAuctionsFactory;
        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(long), typeof(UCAuctionsHistoryDetail));//obtiene parametro 

        public long SelectedItem
        {
            get { return (long)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


    
       
        public UCAuctionsHistoryDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjAuctionsFactory = new AuctionsServicesFactory();
            List<Batch> lLstObjBatches = mObjAuctionsFactory.GetBatchService().SearchBatches(" ", SelectedItem);
            dgDataGrid.ItemsSource = lLstObjBatches;
           // UserControl lUCSearchBatch = new UCSearchBatch("", lLstObjBatches, SelectedItem);
           // GrdContent.Children.Add(lUCSearchBatch);
        }


    }
}
