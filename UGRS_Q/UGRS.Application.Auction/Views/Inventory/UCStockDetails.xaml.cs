using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Application.Auctions.Extensions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.Enums.Inventory;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCStockDetails : UserControl
    {
        AuctionsServicesFactory mObjAuctionsFactory;
        Thread mObjInternalWorker;
        long mLonAuction;
        long mLonCustomer;

        public UCStockDetails(long pLonAuction, long pLonCustomer)
        {
            InitializeComponent();
            mObjAuctionsFactory = new AuctionsServicesFactory();
            mLonAuction = pLonAuction;
            mLonCustomer = pLonCustomer;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!grdDetailsForm.IsBlocked())
            {
                mObjInternalWorker = new Thread(new ThreadStart(LoadDetails));
                mObjInternalWorker.Start();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseForm();
        }

        private void LoadDetails()
        {
            grdDetailsForm.BlockUI();
            try
            {
                int lIntFemaleQtyForAuction = GetQuantityForAuctionOnCurrentAuction(ItemTypeGenderEnum.Hembra);
                int lIntMaleQtyForAuction = GetQuantityForAuctionOnCurrentAuction(ItemTypeGenderEnum.Macho);
                int lIntFemaleQtyForReprogram = GetQuantityForReprogramOnCurrentAuction(ItemTypeGenderEnum.Hembra);
                int lIntMaleQtyForReprogram = GetQuantityForReprogramOnCurrentAuction(ItemTypeGenderEnum.Macho);
                int lIntFemaleQtySales = GetQuantitySalesOnCurrentAuction(ItemTypeGenderEnum.Hembra);
                int lIntMaleQtySales = GetQuantitySalesOnCurrentAuction(ItemTypeGenderEnum.Macho);
                int lIntFemaleQtyPurchases = GetQuantityPurchasesOnCurrentAuction(ItemTypeGenderEnum.Hembra);
                int lIntMaleQtyPurchases = GetQuantityPurchasesOnCurrentAuction(ItemTypeGenderEnum.Macho);
                int lIntFemaleQtyForFuture = GetQuantityForFutureAuctions(ItemTypeGenderEnum.Hembra);
                int lIntMaleQtyForFuture = GetQuantityForFutureAuctions(ItemTypeGenderEnum.Macho);

                this.Dispatcher.Invoke(() => 
                {
                    txtToAuctionFemale.Text = lIntFemaleQtyForAuction.ToString();
                    txtToAuctionMale.Text = lIntMaleQtyForAuction.ToString();
                    txtToAuctionTotal.Text = (lIntFemaleQtyForAuction + lIntMaleQtyForAuction).ToString();
                    txtToReprogramFemale.Text = lIntFemaleQtyForReprogram.ToString();
                    txtToReprogramMale.Text = lIntMaleQtyForReprogram.ToString();
                    txtToReprogramTotal.Text = (lIntFemaleQtyForReprogram + lIntMaleQtyForReprogram).ToString();
                    txtSalesFemale.Text = lIntFemaleQtySales.ToString();
                    txtSalesMale.Text = lIntMaleQtySales.ToString();
                    txtSalesTotal.Text = (lIntFemaleQtySales + lIntMaleQtySales).ToString();
                    txtPurchasesFemale.Text = lIntFemaleQtyPurchases.ToString();
                    txtPurchasesMale.Text = lIntMaleQtyPurchases.ToString();
                    txtPurchasesTotal.Text = (lIntFemaleQtyPurchases + lIntMaleQtyPurchases).ToString();
                    txtFutureToAuctionFemale.Text = lIntFemaleQtyForFuture.ToString();
                    txtFutureToAuctionMale.Text = lIntMaleQtyForFuture.ToString();
                    txtFutureToAuctionTotal.Text = (lIntFemaleQtyForFuture + lIntMaleQtyForFuture).ToString();
                });
            }
            catch (Exception lObjException)
            {
                grdDetailsForm.UnblockUI();
                this.ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdDetailsForm.UnblockUI();
            }
        }

        private int GetQuantityForFutureAuctions(ItemTypeGenderEnum pEnmGender)
        {
            return mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForFutureAuctions(mLonAuction, mLonCustomer, pEnmGender);
        }

        private int GetQuantityForAuctionOnCurrentAuction(ItemTypeGenderEnum pEnmGender)
        {
            return mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForAuctionOnCurrentAuction(mLonAuction, mLonCustomer, pEnmGender);
        }

        private int GetQuantityForReprogramOnCurrentAuction(ItemTypeGenderEnum pEnmGender)
        {
            return mObjAuctionsFactory.GetAuctionStockService().GetAvailableQuantityForReprogramOnCurrentAuction(mLonAuction, mLonCustomer, pEnmGender);
        }

        private int GetQuantitySalesOnCurrentAuction(ItemTypeGenderEnum pEnmGender)
        {
            return mObjAuctionsFactory.GetAuctionStockService().GetSalesQuantityOnCurrentAuction(mLonAuction, mLonCustomer, pEnmGender);
        }

        private int GetQuantityPurchasesOnCurrentAuction(ItemTypeGenderEnum pEnmGender)
        {
            return mObjAuctionsFactory.GetAuctionStockService().GetPurchasesQuantityOnCurrentAuction(mLonAuction, mLonCustomer, pEnmGender);
        }
    }
}
