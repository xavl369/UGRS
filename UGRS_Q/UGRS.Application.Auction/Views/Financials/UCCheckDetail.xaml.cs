using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCCheckListDetail.xaml
    /// </summary>
    public partial class UCCheckDetail : UserControl
    {
        private ListCollectionView mLcvListData = null;

        public static readonly DependencyProperty SellerIdProperty = DependencyProperty.Register
        (
                "SellerId",
                typeof(long),
                typeof(UCCheckDetail),
                new PropertyMetadata((long)0)
        );

        public static readonly DependencyProperty CheckListProperty = DependencyProperty.Register
        (
                "CheckList",
                typeof(List<FoodChargeCheckLineDTO>),
                typeof(UCCheckDetail),
                new PropertyMetadata(new List<FoodChargeCheckLineDTO>())
        );

        public long SellerId
        {
            get { return (long)GetValue(SellerIdProperty); }
            set {SetValue(SellerIdProperty, value);}
        }

        public List<FoodChargeCheckLineDTO> CheckList
        {
            get { return (List<FoodChargeCheckLineDTO>)GetValue(CheckListProperty); }
            set 
            {
                SetValue(CheckListProperty, value);
                LoadDataGrid();
            }
        }

        public UCCheckDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            if (CheckList != null)
            {
                mLcvListData = new ListCollectionView(CheckList);
                dgDetailList.ItemsSource = null;
                dgDetailList.ItemsSource = mLcvListData;
            }
        }

        private void ToggleButton_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            //Get control
            ToggleButton lObjCheck = (pObjSender as ToggleButton);

            //Get data
            FoodChargeCheckLineDTO lObjDetail = lObjCheck.DataContext as FoodChargeCheckLineDTO;

            //Update data
            lObjDetail.ApplyFoodCharge = (bool)lObjCheck.IsChecked;
            CheckList[CheckList.FindIndex(x => x.BatchNumber == lObjDetail.BatchNumber)] = lObjDetail;

            //Undate parent list
            UpdateCheckList();
        }

        private void UpdateCheckList()
        {
            UCCheckList lObjParent = this.FindParent<UCCheckList>();
            lObjParent.UpdateCheckList(SellerId, this.CheckList);
        }
    }
}
