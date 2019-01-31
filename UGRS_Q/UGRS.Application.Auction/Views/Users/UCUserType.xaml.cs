using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCUserType.xaml
    /// </summary>
    public partial class UCUserType : UserControl
    {
        UsersServicesFactory mObjUserTypeFactory = new UsersServicesFactory();
        ListCollectionView mLcvListData = null;
        long mLonId = 0; //bandera si se requiere modificar/eliminar
        private Thread mObjWorker;

        public UCUserType()
        {
            InitializeComponent();
        }

        #region Controls

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDatagrid());
            mObjWorker.Start();            
            ClearControls();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Valid())
            {
                mObjWorker = new Thread(() => SaveOrUpdate());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Tipo de usuarios", "Favor de completar los campos.", this.GetParent());
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
            {
                Remove();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                mLcvListData.Filter = null;
            }
            else
            {
                mLcvListData.Filter = new Predicate<object>(o => ((UserType)o).Name.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgUserType.ItemsSource = mLcvListData;
        }

        private void dgUserType_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserType lObjAuctionType = dgUserType.SelectedItem as UserType;

            if (lObjAuctionType != null)
            {
                txtName.Text = lObjAuctionType.Name;
                txtDescription.Text = lObjAuctionType.Description;
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                lblMessage.Visibility = Visibility.Collapsed;
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                txtName.BorderBrush = Brushes.Black;
                txtDescription.BorderBrush = Brushes.Black;
                mLonId = lObjAuctionType.Id;
            }

        }

        #endregion

        #region Methods

        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgUserType.ItemsSource = null; });                
                List<UserType> lObjuserTypes = mObjUserTypeFactory.GetUserTypeService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lObjuserTypes);
                this.Dispatcher.Invoke(() => { dgUserType.ItemsSource = mLcvListData; });
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

        private void FormLoading(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdList.BlockUI();
                    txtSearch.IsEnabled = false;   
                }
                else
                {
                    grdUserTypeForms.BlockUI();
                }
            });
        }

        private void FormDefult(bool pBolForSave = false)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (!pBolForSave)
                {
                    grdList.UnblockUI();
                    txtSearch.IsEnabled = true;
                }
                else
                {
                    grdUserTypeForms.UnblockUI();
                }
            });
        }

        private void ClearControls()
        {
            mLonId = 0;
            spnUserType.ClearControl();
            txtDescription.Text = string.Empty;
            txtName.Text = string.Empty;
            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            lblMessage.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
        }

        private bool Valid()
        {
            return txtName.ValidRequired();
        }

        private UserType GetUserTypeObject()
        {
            UserType lObjUser = null;
            this.Dispatcher.Invoke(() =>
            {
                lObjUser = new UserType()
                {
                    Id = mLonId,
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                };
            });

            return lObjUser;
        }

        private void SaveOrUpdate()
        {
            FormLoading(true);
            try
            {
                mObjUserTypeFactory.GetUserTypeService().SaveOrUpdate(GetUserTypeObject());

                FormDefult(true);

                mObjWorker = new Thread(() => LoadDatagrid());
                mObjWorker.Start();

                this.Dispatcher.Invoke((Action)delegate
                {
                    ClearControls();
                });
            }
            catch (Exception lObjException)
            {
                FormDefult(true);
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void Remove()
        {
            try
            {
                mObjUserTypeFactory.GetUserTypeService().Remove(mLonId);
                LoadDatagrid();
                ClearControls();
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        #endregion
    }
}
