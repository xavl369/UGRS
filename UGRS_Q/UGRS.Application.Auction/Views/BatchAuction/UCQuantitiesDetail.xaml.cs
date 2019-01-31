using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Application.Auctions.Extensions;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCCustomerStockDetail.xaml
    /// </summary>
    public partial class UCQuantitiesDetail : UserControl
    {
        InventoryServicesFactory mObjInventoryServices;
        AuctionsServicesFactory mObjAuctionsServices;
        Thread mObjInternalWorker;

        public long AuctionId { get; set; }
        public long CustomerId { get; set; }

        public UCQuantitiesDetail()
        {
            InitializeComponent();
            mObjAuctionsServices = new AuctionsServicesFactory();
            mObjInventoryServices = new InventoryServicesFactory();
        }

        public void LoadQuantities(long pLonCustomerId)
        {
            CustomerId = pLonCustomerId;

            if (!grdCustomerStockDatail.IsBlocked())
            {
                mObjInternalWorker = new Thread(() => InternalLoadQuantities(pLonCustomerId));
                mObjInternalWorker.Start();
            }
        }

        public void ResetCustomerStock()
        {
            grdCustomerStockDatail.ClearControl();
        }

        private void InternalLoadQuantities(long pLonCustomerId)
        {
            grdCustomerStockDatail.BlockUI();
            Thread.Sleep(300);

            try
            {
                InternalSetQuantities
                (
                    mObjAuctionsServices.GetBatchAuctionService().GetAvailableQuantityBySeller(AuctionId, pLonCustomerId),
                    mObjAuctionsServices.GetBatchAuctionService().GetSoldQuantityBySeller(AuctionId, pLonCustomerId),
                    mObjAuctionsServices.GetBatchAuctionService().GetPurchasedQuantityByBuyer(AuctionId, pLonCustomerId),
                    mObjAuctionsServices.GetBatchAuctionService().GetSoldQuantityByAuction(AuctionId)
                );
            }
            catch (Exception lObjException)
            {
                grdCustomerStockDatail.UnblockUI();
                InternalShowMessageError(lObjException);
            }
            finally
            {
                grdCustomerStockDatail.UnblockUI();
            }
        }

        private void InternalSetQuantities(int pIntAvailables, int pIntSold, int pIntPurchased, int pIntTotal)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                txtAvailables.Text = pIntAvailables.ToString();
                txtSold.Text = pIntSold.ToString();
                txtPurchased.Text = pIntPurchased.ToString();
                txtTotal.Text = pIntTotal.ToString();
            });
        }

        private void InternalShowMessageError(Exception lObjException)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            });
        }

        private void btnDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowCustomerStockDetails(AuctionId, CustomerId);
        }
    }
}
