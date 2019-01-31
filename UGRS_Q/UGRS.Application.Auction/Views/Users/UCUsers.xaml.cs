using System;
using System.Collections.Generic;
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
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using System.Threading;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Lógica de interacción para UCUser.xaml
    /// </summary>
    public partial class UCUsers : UserControl
    {
        UsersServicesFactory mObjUserFactory = new UsersServicesFactory();
        ListCollectionView mLcvListData = null;
        long mLonId = 0; //bandera si se requiere modificar/eliminar
        private Thread mObjWorker;

        public UCUsers()
        {
            InitializeComponent();
        }

        #region Controls

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDatagrid());
            mObjWorker.Start();

            ClearControls();
            LoadCombobox();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (grdUser.Valid())
            {
                mObjWorker = new Thread(() => SaveOrUpdate());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Usuarios", "Favor de completar los campos.", this.GetParent());
            }
        }

        private void LoadCombobox()
        {
            List<UserType> lObjItemTypes = mObjUserFactory.GetUserTypeService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
            cbUSerType.ItemsSource = lObjItemTypes;
            cbUSerType.DisplayMemberPath = "Name";
            cbUSerType.SelectedValuePath = "Id";
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", this.GetParent()) == true)
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
                mLcvListData.Filter = new Predicate<object>(o => ((User)o).UserName.ToUpper().Contains(txtSearch.Text.ToUpper()));
            }
            dgUser.ItemsSource = mLcvListData;
        }

        private void dgUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            User lObjUser = dgUser.SelectedItem as User;

            if (lObjUser != null)
            {
                txtFirstName.Text = lObjUser.FirstName;
                txtLastName.Text = lObjUser.LastName;
                txtEmailAddress.Text = lObjUser.EmailAddress;
                cbUSerType.SelectedValue = lObjUser.UserTypeId;
                txtUserName.Text = lObjUser.UserName;
                
                btnSave.Content = "Modificar";
                lbltitle.Content = "Modificar registro";
                lblMessage.Visibility = Visibility.Collapsed;
                btnNew.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                mLonId = lObjUser.Id;
            }

        }

        private void txtRepeatPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == txtRepeatPassword.Password)
            {
                txtPassword.BorderBrush = Brushes.Black;
                txtRepeatPassword.BorderBrush = Brushes.Black;
                lblPasswordMsg.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPassword.BorderBrush = Brushes.Red;
                txtRepeatPassword.BorderBrush = Brushes.Red;
                lblPasswordMsg.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        private void LoadDatagrid()
        {
            FormLoading();
            try
            {
                this.Dispatcher.Invoke(() => { dgUser.ItemsSource = null; });
                List<User> lObjUser = mObjUserFactory.GetUserService().GetList().Where(x => x.Active == true && x.Removed == false).ToList();
                mLcvListData = new ListCollectionView(lObjUser);
                this.Dispatcher.Invoke(() => { dgUser.ItemsSource = mLcvListData; });
            }
            catch (Exception lObjException)
            {
                FormDefult();
                ShowMessage("Error", lObjException.Message);
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
                    grdUsersForms.BlockUI();
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
                    grdUsersForms.UnblockUI();
                }
            });
        }

        private void ClearControls()
        {
            ClearControls(grdUser);
            cbUSerType.SelectedValue = null;
            txtPassword.Password = string.Empty;
            txtRepeatPassword.Password = string.Empty;
            lblMessage.Visibility = Visibility.Collapsed;
            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Limpia los controles.
        /// </summary>
        private void ClearControls(Grid pControl)
        {
            UIElementCollection lObjelement = pControl.Children;
            List<FrameworkElement> lLstElement = lObjelement.Cast<FrameworkElement>().ToList();
            var lLstControl = lLstElement.OfType<TextBox>();
            foreach (TextBox lObjcontrol in lLstControl)
            {
                lObjcontrol.Text = string.Empty;
                lObjcontrol.BorderBrush = Brushes.Black;
            }
        }

        /// <summary>
        ///  Valida los controles que se encuentran en blanco.
        /// </summary>
        private bool ValidateControls(Grid pObjBorder)
        {
            bool lBolValidate = true;
            UIElementCollection lObjelement = pObjBorder.Children;
            List<FrameworkElement> lLstElement = lObjelement.Cast<FrameworkElement>().ToList();
            var lLstControl = lLstElement.OfType<TextBox>();
            foreach (TextBox lObjcontrol in lLstControl)
            {
                if (string.IsNullOrEmpty(lObjcontrol.Text))
                {
                    lObjcontrol.BorderBrush = Brushes.Red;
                    lBolValidate = false;
                }
            }
            return lBolValidate;
        }

        private User GetUserObject()
        {
            User lObjUser = null;
            this.Dispatcher.Invoke(() =>
            {
                lObjUser = new User()
                {
                    Id = mLonId,
                    UserName = txtUserName.Text,
                    Password = txtPassword.Password,
                    EmailAddress = txtEmailAddress.Text,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    UserTypeId = Convert.ToInt32(cbUSerType.SelectedValue)
                };
            });

            return lObjUser;
        }

        private void SaveOrUpdate()
        {
            FormLoading(true);
            try
            {
                mObjUserFactory.GetUserService().SaveOrUpdate(GetUserObject());

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
                ShowMessage("Error", lObjException.Message);
            }
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private void Remove()
        {
            try
            {
                mObjUserFactory.GetUserService().Remove(mLonId);
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