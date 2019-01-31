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
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Application.Utility;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCBatchHistory.xaml
    /// </summary>
    public partial class UCBatchesHistory : UserControl
    {
        private AuctionsServicesFactory mObjAuctionsFactory;
        private List<Batch> mObjLstAuctionFilter = null;
        public UCBatchesHistory()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjAuctionsFactory = new AuctionsServicesFactory();
            dpDateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpDateTo.SelectedDate = DateTime.Now;
            LoadDatagrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            LoadDatagrid();
        }

        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgDataGrid.ItemsSource = null; });
                List<Batch> lLstObjBatches = mObjAuctionsFactory.GetBatchService().GetList().OrderByDescending(x => x.Number).ToList();
                mObjLstAuctionFilter = lLstObjBatches;
                if (txtSearchAuction.Text != string.Empty)
                {
                    mObjLstAuctionFilter = mObjLstAuctionFilter.Where(x => x.Auction.Folio.Equals(txtSearchAuction.Text)).ToList();
                }
                if ((dpDateFrom.SelectedDate.Value != null || dpDateTo.SelectedDate.Value != null) && txtSearchAuction.Text == string.Empty)
                {
                    if (dpDateFrom.SelectedDate <= dpDateTo.SelectedDate)
                    {
                        mObjLstAuctionFilter = mObjLstAuctionFilter.Where(x => x.CreationDate >= dpDateFrom.SelectedDate && x.CreationDate <= dpDateTo.SelectedDate).ToList();
                    }
                }
                this.Dispatcher.Invoke(() => { dgDataGrid.ItemsSource = mObjLstAuctionFilter; });
            }
            catch (Exception lObjException)
            {
                FormDefult();
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
             
            }
            finally
            {
                FormDefult();
            }
         
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                GrdContent.BlockUI();

            });
        }
        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                GrdContent.UnblockUI();
            });
        }

        private void txtAuctions_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Auction lObjAuction = null;
                if ((e.Key == Key.Enter & ((sender as TextBox).AcceptsReturn == false) && (sender as TextBox).Focus()) || e.Key == Key.Tab)
                {
                    IList<Auction> lLstAuction = SearchAuctions((sender as TextBox).Text);
                    if (lLstAuction.Count == 1)
                    {
                        lObjAuction = lLstAuction[0];
                    }
                    else
                    {
                        (sender as TextBox).Focusable = false;
                        UserControl lUCAuction = new UCSearchAuction(txtSearchAuction.Text, lLstAuction.ToList(), FilterEnum.ACTIVE, AuctionSearchModeEnum.AUCTION); // new UCAuction(true, lLstAuction.ToList(), (sender as TextBox).Text);
                        lObjAuction = FunctionsUI.ShowWindowDialog(lUCAuction, Window.GetWindow(this)) as Auction;
                        (sender as TextBox).Focusable = true;
                    }
                    (sender as TextBox).Focus();
                    if (lObjAuction != null)
                    {
                        SetControls(lObjAuction);
                        LoadDatagrid();
                    }
                    else
                    {
                        (sender as TextBox).Focus();
                    }
                }

            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Subasta", lObjException.Message, Window.GetWindow(this));
            }
        }

        private void SetControls(Auction lObjAuction)
        {
            txtSearchAuction.Text = lObjAuction.Folio;
        }

        private IList<Auction> SearchAuctions(string pStrAuction)
        {
            return mObjAuctionsFactory.GetAuctionService().SearchAuctions(pStrAuction, FilterEnum.NONE);
        }

        private void txt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtSearchAuction.Text = "";
           

            dpDateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpDateTo.SelectedDate = DateTime.Now;

            this.Dispatcher.Invoke(() => { dgDataGrid.ItemsSource = null; });

        }
    }
}
