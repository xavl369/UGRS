using System.Collections.Generic;
using System.Windows.Controls;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;
using System.Linq;


namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCBatchesList.xaml
    /// </summary>
    public partial class UCBatchesList : UserControl
    {
        private AuctionsServicesFactory mObjAuctionServiceFactory = new AuctionsServicesFactory();
        private Auction mObjAuction;
        public UCBatchesList(Auction pObjAuction)
        {
            mObjAuction = pObjAuction;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadDatagrid(SearchBatches());
        }

        // <summary>
        //Relaliza la busqueda de lotes
        // </summary>
        private List<Batch> SearchBatches()
        {
            return mObjAuctionServiceFactory.GetBatchService().GetList().Where(x => x.Active == true && x.Removed == false && x.AuctionId == mObjAuction.Id && x.SellerId != null).ToList();
        }

          
        private void LoadDatagrid(List<Batch> pLstbatch)
        {
            //GetListSellersToCharge(pLstbatch);
            ////   int x = pLstbatch.Where(x=> x.AuctionId == mLonAuctionId).Sum
            //mLcvListData = new ListCollectionView(pLstbatch);
            dgBatch.ItemsSource = pLstbatch;
        }
    }
}
