using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions.AuctionClose
{
    /// <summary>
    /// Interaction logic for UCDirectDealList.xaml
    /// </summary>
    public partial class UCDirectDealList : UserControl
    {
        private AuctionsServicesFactory mObjAuctionServiceFactory = new AuctionsServicesFactory();
        private Auction mObjAuction;
        public UCDirectDealList(Auction pObjAuction)
        {
            mObjAuction = pObjAuction;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDatagrid(SearchTrade());
        }

        private List<Trade> SearchTrade()
        {
            return mObjAuctionServiceFactory.GetTradeService().GetList().Where(x => x.Active == true && x.Removed == false
                && x.AuctionId == mObjAuction.Id && x.SellerId != null).ToList();
        }

        private void LoadDatagrid(List<Trade> pLstTrade)
        {
            dgTrade.ItemsSource = pLstTrade;
        }

    }
}
