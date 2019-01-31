using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCSkipBatch.xaml
    /// </summary>
    public partial class UCSkipBatch : UserControl
    {
        #region Attribute

        private AuctionsServicesFactory mObjAuctionsFactory;
        private InventoryServicesFactory mObjInventoryFactory;
        private Thread mObjInternalWorker;
        private long mLonAuctionId;

        #endregion

        #region Constructor

        public UCSkipBatch(long pLonAuctionId)
        {
            InitializeComponent();
            mObjAuctionsFactory = new AuctionsServicesFactory();
            mObjInventoryFactory = new InventoryServicesFactory();
            mLonAuctionId = pLonAuctionId;
        }

        #endregion

        #region Events

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            if (txtBatch.ValidRequired())
            {
                tblError.Visibility = System.Windows.Visibility.Collapsed;

                if (!grdSkipBatch.IsBlocked())
                {
                    mObjInternalWorker = new Thread(SkipBatches);
                    mObjInternalWorker.Start();
                }
            }
            else
            {
                tblError.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void txtBatch_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtBatch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSkip_Click(sender, e);
            }
        }

        #endregion

        #region Methods

        private void SkipBatches()
        {
            grdSkipBatch.BlockUI();

            try
            {
                int lIntSkipBatch = GetSkipBatch();
                int lIntNextBatch = GetNextBatch();
                bool lBolDialogResult = false;

                if (lIntSkipBatch > lIntNextBatch)
                {
                    for (int lIntBatchNumber = lIntNextBatch; lIntBatchNumber < lIntSkipBatch; lIntBatchNumber++)
                    {
                        var x = GetBatchObject(mLonAuctionId, lIntBatchNumber);
                        mObjAuctionsFactory.GetBatchAuctionService().SaveOrUpdateBatch(GetBatchObject(mLonAuctionId, lIntBatchNumber));
                    }
                    lBolDialogResult = true;
                }
                CloseDialog(lBolDialogResult);
            }
            catch (Exception lObjException)
            {
                grdSkipBatch.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdSkipBatch.UnblockUI();
            }
        }

        private Batch GetBatchObject(long pLonAuctionId, int pIntBatchNumber)
        {
            return new Batch()
            {
                Number = pIntBatchNumber,
                Quantity = 0,
                Weight = 0,
                AverageWeight = 0,
                Price = 0,
                Amount = 0,
                Reprogrammed = false,
                Unsold = false,
                AuctionId = pLonAuctionId,
            };
        }

        private int GetSkipBatch()
        {
            return (int)this.Dispatcher.Invoke(new Func<int>(() =>
            {
                int lIntNumber = 0;
                return Int32.TryParse(txtBatch.Text, out lIntNumber) ? lIntNumber : 0;

            }));
        }

        private int GetNextBatch()
        {
            return mObjAuctionsFactory.GetBatchService().GetList().Count(x => x.AuctionId == mLonAuctionId) > 0 ?
                mObjAuctionsFactory.GetBatchService().GetList().Where(x => x.AuctionId == mLonAuctionId).Max(y => y.Number) + 1 : 1;
        }

        //private long GetDefaultItemTypeId()
        //{
        //    return mObjInventoryFactory.GetItemTypeService().GetList().Where(x => x.Active && x.Level == 1 && x.PerPrice == false).Select(y => y.Id).FirstOrDefault();
        //}

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke(() =>
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, this.GetParent());
            });
        }

        private void CloseDialog(bool pBolResult)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.GetParent().DialogResult = pBolResult;
            });
        }

        #endregion
    }
}
