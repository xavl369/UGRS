using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSearchBatch.xaml
    /// </summary>
    public partial class UCSearchBatches : UserControl
    {
        AuctionsServicesFactory mObjServiceFactory = new AuctionsServicesFactory();
        ListCollectionView mLcvListData = null; //Filtros
        List<Batch> lLstAuctionBatches = null;
        List<Auction> lObjLstAuctions = null;
        List<Batch> lObjLstFilters = new List<Batch>();
        private bool mBolReadOnly;

        public UCSearchBatches()
        {
            InitializeComponent();
        }

        private void Defaults(object sender, RoutedEventArgs e)
        {
            lLstAuctionBatches = mObjServiceFactory.GetBatchService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            lObjLstAuctions = mObjServiceFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Opened == true && x.Active == true && x.Removed == false).ToList();

            txtSearchAuction.Text = lObjLstAuctions.Where(x => x.Opened == true).Select(y => y.Folio).Last();
    
            if(mBolReadOnly == false)
            {

                txtSearchBatch.Visibility = Visibility.Collapsed;
            }
        }

        public UCSearchBatches(List<Batch> pLstBatch, bool pBolReadOnly)
        {
            InitializeComponent();
            loadDatagrid(pLstBatch);
            lLstAuctionBatches = new List<Batch>();

            mBolReadOnly = pBolReadOnly;
            if (pBolReadOnly)
            {

                txtSearchBuyer.Visibility = Visibility.Collapsed;
                txtSearchSeller.Visibility = Visibility.Collapsed;
                txtSearchAuction.Visibility = Visibility.Collapsed;

                txtSearchBatch.Focus();
                mLcvListData = new ListCollectionView(pLstBatch);
              
            }

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);

            dgBatch.Focus();
            dgBatch.SelectedIndex = 0;

        }

        private void loadDatagrid(List<Batch> pLstbatch)
        {
            mLcvListData = new ListCollectionView(pLstbatch);
            dgBatch.ItemsSource = mLcvListData;
        }

        private void loadDatagrid()
        {
            dgBatch.ItemsSource = null;

            lObjLstFilters = lLstAuctionBatches;

            if(txtSearchAuction.Text != string.Empty)
            {
                lObjLstFilters = lObjLstFilters.Where(x => x.Auction.Folio == txtSearchAuction.Text).ToList();
            }
            if (txtSearchSeller.Text != string.Empty)
            {
                lObjLstFilters = lObjLstFilters.Where(x => x.SellerId != null && x.Seller.Code.Equals(txtSearchSeller.Text)).ToList();
            }
            if (txtSearchBuyer.Text != string.Empty)
            {
                lObjLstFilters = lObjLstFilters.Where(x => x.BuyerId != null && x.Buyer.Code.Equals(txtSearchBuyer.Text)).ToList();
            }

            mLcvListData = new ListCollectionView(lObjLstFilters);

            dgBatch.ItemsSource = mLcvListData;
        }

        private void txtAuctions_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Auction lObjAuction = null;
                if ((e.Key == Key.Enter & ((sender as TextBox).AcceptsReturn == false) && (sender as TextBox).Focus() )|| e.Key==Key.Tab)
                {
                    IList<Auction> lLstAuction = SearchAuctions((sender as TextBox).Text);
                    if (lLstAuction.Count == 1)
                    {
                        lObjAuction = lLstAuction[0];
                    }
                    else
                    {
                        (sender as TextBox).Focusable = false;
                        UserControl lUCAuction = new UCAuction(true, lLstAuction.ToList(), (sender as TextBox).Text);
                        lObjAuction = FunctionsUI.ShowWindowDialog(lUCAuction, Window.GetWindow(this)) as Auction;
                        (sender as TextBox).Focusable = true;
                    }
                    (sender as TextBox).Focus();
                    if (lObjAuction != null)
                    {
                        SetControls(lObjAuction);
                        loadDatagrid();
                    }
                    else
                    {
                        (sender as TextBox).Focus();
                    }
                }
               
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, Window.GetWindow(this));
            }

        }

        private void txtPartner_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Partner lObjPartner = new Partner();
                if ((e.Key == Key.Enter & ((sender as TextBox).AcceptsReturn == false) && (sender as TextBox).Focus())|| e.Key==Key.Tab)
                {
                    List<Partner> lLstPartner = SearchPartner((sender as TextBox).Text);
                    if (lLstPartner.Count == 1)
                    {
                        lObjPartner = lLstPartner[0];
                    }
                    else
                    {
                        (sender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner((sender as TextBox).Text, lLstPartner, FilterEnum.ACTIVE);
                        lObjPartner = FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner;
                        (sender as TextBox).Focusable = true;
                    }
                    (sender as TextBox).Focus();
                    if (lObjPartner != null)
                    {
                        SetControls(lObjPartner,(sender as TextBox).Name);
                        loadDatagrid();
                    }
                    else
                    {
                        (sender as TextBox).Focus();
                    }
              
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error", ex.Message, Window.GetWindow(this));
            }
        }

        private List<Partner> SearchPartner(string pStrPartner)
        {
            BusinessServicesFactory lObjPartnerFactory = new BusinessServicesFactory();
            return lObjPartnerFactory.GetPartnerService().SearchPartner(pStrPartner, FilterEnum.ACTIVE);
        }

        private void SetControls(Partner lObjPartner, string pStrBuyOrSell)
        {
            switch(pStrBuyOrSell)
            {
                case "txtSearchSeller":
                    txtSearchSeller.Text = lObjPartner.Code;
                    break;

                case "txtSearchBuyer":
                    txtSearchBuyer.Text = lObjPartner.Code;
                    break;
            }
  
        }

        private void SetControls(Auction lObjAuction)
        {
            txtSearchAuction.Text = lObjAuction.Folio;

        }

        private IList<Auction> SearchAuctions(string pStrAuction)
        {
            return mObjServiceFactory.GetAuctionService().SearchAuctions(pStrAuction, FilterEnum.OPENED);
        }

        //private void Filters()
        //{

        //    if (txtSearchBatch.Text != string.Empty)
        //    {

        //        if (lObjLstFilters.Count > 0)
        //        {
        //            mLcvListData = new ListCollectionView(lObjLstFilters.Where(x => x.Number.ToString().Contains(txtSearchBatch.Text)).ToList());
        //        }
        //        else
        //        {
        //            mLcvListData.Filter = new Predicate<object>(o => ((Batch)o).Number.ToString().Contains(txtSearchBatch.Text));
        //        }

        //        lObjLstFilters = mLcvListData.SourceCollection as List<Batch>;

        //    }


        //    if (txtSearchAuction.Text != string.Empty)
        //    {
        //        if (lObjLstFilters.Count > 0)
        //        {
        //            mLcvListData = new ListCollectionView(lObjLstFilters.Where(x => x.Auction.Folio.ToUpper().Contains(txtSearchAuction.Text.ToUpper())).ToList());
        //        }
        //        else
        //        {
        //            mLcvListData.Filter = new Predicate<object>(o => ((Batch)o).Auction.Folio.ToUpper().Contains(txtSearchAuction.Text.ToUpper()));
        //        }
        //        lObjLstFilters = mLcvListData.SourceCollection as List<Batch>;

        //    }

        //    if (txtSearchSeller.Text != string.Empty)
        //    {




        //        //mLcvListData = CollectionViewSource.GetDefaultView(lObjLstFilters.Where(x => x.Auction.Folio.ToUpper().Contains(txtSearchAuction.Text.ToUpper())).ToList()) as ListCollectionView;

        //        if (lObjLstFilters.Count > 0)
        //        {
        //            mLcvListData = new ListCollectionView(lObjLstFilters.Where(x => x.SellerId.HasValue && x.Seller.Name.ToUpper().Contains(txtSearchSeller.Text.ToUpper())).ToList());
        //        }
        //        else
        //        {
        //            List<Batch> list = lLstAuctions.Where(x => x.Active == true && x.Removed == false && x.SellerId.HasValue).ToList();

        //            mLcvListData = new ListCollectionView(list.Where(x => x.Seller.Name.ToUpper().Contains(txtSearchSeller.Text.ToUpper())).ToList());



        //        }

        //        lObjLstFilters = mLcvListData.SourceCollection as List<Batch>;
        //    }

        //    if (string.IsNullOrEmpty(txtSearchAuction.Text) && string.IsNullOrEmpty(txtSearchBatch.Text) && string.IsNullOrEmpty(txtSearchSeller.Text))
        //    {
        //        mLcvListData = new ListCollectionView(lLstAuctions);
        //        lObjLstFilters = new List<Batch>();
        //    }


        //    dgBatch.ItemsSource = mLcvListData;
        //    dgBatch.SelectedIndex = 0;

        //}

        private void dgBatch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReturnBatch();
            }
            if (char.IsLetterOrDigit((char)e.Key))
            {
                txtSearchAuction.Focus();
            }
        }

        private void dgBatch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReturnBatch();
        }

        private void ReturnBatch()
        {
            try
            {
                Window lObjWindowParent = Window.GetWindow(this);
                WindowDialog lObjWindowDialog = lObjWindowParent as WindowDialog;
                Batch lobjAuction = dgBatch.SelectedItem as Batch;
                lObjWindowDialog.gObject = lobjAuction as object;
                lObjWindowParent.Close();
            }
            catch
            {

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchBatch.Text != string.Empty)
            {
                mLcvListData.Filter = new Predicate<object>(o => ((Batch)o).Number.ToString().Contains(txtSearchBatch.Text));
                dgBatch.ItemsSource = mLcvListData;
                dgBatch.SelectedIndex = 0;
            }
        }
    }
}
