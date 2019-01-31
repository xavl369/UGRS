using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using System.Threading;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Utility;
using UGRS.Core.Auctions.Enums.Auctions;


namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCAuctionsHistory.xaml
    /// </summary>
    public partial class UCAuctionsHistory : UserControl
    {
        private AuctionsServicesFactory mObjAuctionServiceFactory;
        private ListCollectionView mLcvListData = null; //Filtros
        
        private Thread mObjWorker;
        private DateTime lObjDtFrom;
        private DateTime lObjDtTo;
        private bool mBolReadOnly;

        public List<Auction> pObjLstAuction = null;
        private List<Auction> lObjLstAuctionFilter = null;
        List<Auction> lLstAuctions = null;

        public UCAuctionsHistory()
        {
            InitializeComponent();
            //GrdContent.Children.Add(new UCAuction(true));
            mObjAuctionServiceFactory = new AuctionsServicesFactory();
            LoadComboBoxLocation();
            LoadComboBoxCategory();

            lLstAuctions = mObjAuctionServiceFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Active == true && x.Removed == false).ToList();
            dpDateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpDateTo.SelectedDate = DateTime.Now;
            LoadDatagrid();
        }



        private void btnExpandCollapse_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Button lObjExpandCollapse = (Button)sender;
                //checa el objeto boton si es null o no
                if (lObjExpandCollapse != null)
                {
                    // Return the Contains which specified element
                    DataGridRow DgrSelectedRowObj = DataGridRow.GetRowContainingElement(lObjExpandCollapse);

                    // Check the DataGridRow Object is Null or Not
                    if (DgrSelectedRowObj != null)
                    {
                        // si el boton ="+" expande los detalles
                        if (lObjExpandCollapse != null && lObjExpandCollapse.Content.ToString() == "+")
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Visible;
                            lObjExpandCollapse.Content = "-";
                           

                        }
                        // else contrae los detalles
                        else
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Collapsed;
                            lObjExpandCollapse.Content = "+";
                        }
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show("Error", "No se pudieron consultar los detalles", this.GetParent());
            }
        }

        /// <summary>
        /// Carga los datos en el DataGrid.
        /// </summary>
        private void LoadDatagrid()
        {
            FormLoading();

            try
            {
                this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = null; });
                lObjLstAuctionFilter = lLstAuctions;

                if(txtSearchAuction.Text != string.Empty)
                {
                    lObjLstAuctionFilter = lObjLstAuctionFilter.Where(x => x.Folio.Equals(txtSearchAuction.Text)).ToList();
                }
                //if(cbCategory.SelectedValue != null)
                //{
                //    lObjLstAuctionFilter = lObjLstAuctionFilter.Where(x => x.CategoryId == Convert.ToInt64(cbCategory.SelectedValue)).ToList();
                //}
                if (cbTypeId.SelectedValue != null)
                {
                    lObjLstAuctionFilter = lObjLstAuctionFilter.Where(x => x.Type == (AuctionTypeEnum)cbTypeId.SelectedValue).ToList();
                }

                if (dpDateFrom.SelectedDate.Value != null || dpDateTo.SelectedDate.Value != null)
                {
                    if (dpDateFrom.SelectedDate <= dpDateTo.SelectedDate)
                    {
                        lObjLstAuctionFilter = lObjLstAuctionFilter.Where(x => x.Date >= dpDateFrom.SelectedDate && x.Date <= dpDateTo.SelectedDate).ToList();
                    }

                }
                mLcvListData = new ListCollectionView(lObjLstAuctionFilter);
                this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = mLcvListData; });

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



            //try
            //{
            //    this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = null; });
            //    List<Auction> lLstAuctions = mObjAuctionServiceFactory.GetAuctionService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            //    mLcvListData = new ListCollectionView(lLstAuctions);

            //    this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = mLcvListData; });
            //}
            //catch (System.Exception lObjException)
            //{
            //    FormDefult();
            //    CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());

            //}
            //finally
            //{
            //    FormDefult();
            //}
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                GrdContent.BlockUI();

            });
        }

        private void LoadComboBoxCategory()
        {
            //cbCategory.ItemsSource = ComboUtility.ParseListObjectToCombo(mObjAuctionServiceFactory.GetCategoryService().GetListByStatus(true).ToList());
            //cbCategory.DisplayMemberPath = "Text";
            //cbCategory.SelectedValuePath = "Value";
            //cbCategory.SelectedValue = 0;
        }

        private void LoadComboBoxLocation()
        { 
            cbTypeId.ItemsSource = UGRS.Core.Extension.Enum.EnumExtension.GetEnumItemList<AuctionTypeEnum>();
            cbTypeId.DisplayMemberPath = "Text";
            cbTypeId.SelectedValuePath = "Value";
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

        private IList<Auction> SearchAuctions(string pStrAuction)
        {
            return mObjAuctionServiceFactory.GetAuctionService().SearchAuctions(pStrAuction, FilterEnum.NONE);
        }

        private void SetControls(Auction lObjAuction)
        {
            txtSearchAuction.Text = lObjAuction.Folio;
        }


        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                LoadDatagrid();
            }
        }


        private void dt_keyDown(object sender, KeyEventArgs e)
        {
            DatePicker lObjDp = sender as DatePicker;

            if (lObjDp.Name.Equals("dpDateFrom") || lObjDp.Name.Equals("dpDateTo"))
            {
                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    LoadDatagrid();
                }
            }

        }

        private void txt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtSearchAuction.Text = "";
            cbTypeId.SelectedIndex = -1;

            dpDateFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpDateTo.SelectedDate = DateTime.Now;

            this.Dispatcher.Invoke(() => { dgAuctions.ItemsSource = null; });

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDatagrid();
        }


        //private void Dte_Chng(object sender, SelectionChangedEventArgs e)
        //{
        //    DatePicker lObjDT = sender as DatePicker;

        //    if (lObjDT.Name.Equals(dpDateFrom))
        //    {
        //        lObjDtFrom = Convert.ToDateTime(lObjDT.SelectedDate);
        //    }
        //    else if (lObjDT.Name.Equals(dpDateTo))
        //    {
        //        lObjDtTo = Convert.ToDateTime(lObjDT.SelectedDate);
        //    }
        //}







    }
}
