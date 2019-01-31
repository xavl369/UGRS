using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using UGRS.Application.Auctions.Securty;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Enums.Security;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Extension.Wpf;
using UGRS.Core.Application.Extension.Controls;
using System.Threading;
using UGRS.Core.Application.Utility;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for Permission.xaml
    /// </summary>
    public partial class UCPermission : UserControl
    {
        UsersServicesFactory mObjUserTypeFactory = new UsersServicesFactory();
        ListCollectionView mLcvListData = null;
        private Thread mObjWorker;

        public UCPermission()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                if (cbType.GetSelectedEnumValue<PermissionTypeEnum>() == PermissionTypeEnum.USER_TYPE)
                {
                    mLcvListData.Filter = new Predicate<object>(o => 
                        ((UserType)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()) || 
                        ((UserType)o).Description.ToUpper().Contains(txtSearch.Text.ToUpper()));
                    dgUserType.ItemsSource = mLcvListData;
                }
                else
                {
                    mLcvListData.Filter = new Predicate<object>(o =>
                        ((User)o).UserName.ToUpper().Contains(txtSearch.Text.ToUpper()) ||
                        ((User)o).FirstName.ToUpper().Contains(txtSearch.Text.ToUpper()) ||
                        ((User)o).LastName.ToUpper().Contains(txtSearch.Text.ToUpper()));
                    dgUser.ItemsSource = mLcvListData;
                }
            }
        }

        private void LoadDatagridUserType()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgUserType.ItemsSource = null; });
                List<UserType> lObjuserTypes = mObjUserTypeFactory.GetUserTypeService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lObjuserTypes);

                this.Dispatcher.Invoke((Action)delegate 
                {
                    dgUserType.ItemsSource = mLcvListData;

                    dgUser.Visibility = Visibility.Collapsed;
                    dgUserType.Visibility = Visibility.Visible;
                });
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
                grdList.BlockUI();
            });
        }

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdList.UnblockUI();
            });
        }

        private void LoadDataGridUser()
        {
            dgUserType.ItemsSource = null;
            List<User> lObjuserTypes = mObjUserTypeFactory.GetUserService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            mLcvListData = new ListCollectionView(lObjuserTypes);
            dgUser.ItemsSource = mLcvListData;

            dgUser.Visibility = Visibility.Visible;
            dgUserType.Visibility = Visibility.Collapsed;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDatagridUserType());
            mObjWorker.Start();
            LoadCombobox();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            UserType lObjUserType = dgUserType.SelectedItem as UserType;
            UserControl lUCPermissionDialog = new UCPermissionDialog(cbType.GetSelectedEnumValue<PermissionTypeEnum>(), lObjUserType.Name, lObjUserType.Id);
            grMenu.Children.Add(lUCPermissionDialog);
            grMenu.Visibility = Visibility.Visible;
        }

        private void btnConfigUser_Click(object sender, RoutedEventArgs e)
        {
            User lObjUser = dgUser.SelectedItem as User;
            UserControl lUCPermissionDialog = new UCPermissionDialog(cbType.GetSelectedEnumValue<PermissionTypeEnum>(), lObjUser.UserName, lObjUser.Id);
            grMenu.Children.Add(lUCPermissionDialog);
            grMenu.Visibility = Visibility.Visible;
        }

        private void LoadCombobox()
        {
            cbType.LoadSource<PermissionTypeEnum>();
            cbType.SelectedIndex = cbType.GetIndex((int)PermissionTypeEnum.USER_TYPE);
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbType.GetSelectedEnumValue<PermissionTypeEnum>() == PermissionTypeEnum.USER_TYPE)
            {
                LoadDatagridUserType();
            }
            else
            {
                LoadDataGridUser();
            }
            grMenu.Visibility = Visibility.Collapsed;
        }

   
    }
}
