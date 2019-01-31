using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Auctions.DTO.Business;
using UGRS.Core.Auctions.Enums.Base;

namespace UGRS.Application.Auctions
{
    public partial class UCPartnerClassification : UserControl
    {
        #region Attributes

        private BusinessServicesFactory mObjBusinessFactory = new BusinessServicesFactory();
        private ListCollectionView mLcvListData = null;

        private long mLonId = 0;
        private long mLonPartnerId = 0;
        private Thread mObjWorker;

        private int mIntPageNumber = 1;
        private int mIntRowsPerPage = 100;
        private IPagedList<PartnerClassification> mLstObjRows;
        private IQueryable<PartnerClassification> mLstObjQuery;

        #endregion

        #region Constructor

        public UCPartnerClassification()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region UserControl

        private void UserControl_Loaded(object pObjSender, RoutedEventArgs pObjArgs)
        {
            ClearControls();
            LoadPagedList();
        }

        #endregion

        #region TextBox

        private void txtSearch_TextChanged(object pObjSender, TextChangedEventArgs pObjArgs)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                SearchPagedList(txtSearch.Text);
            }
            else
            {
                ReloadPagedList();
            }

            if (mLcvListData == null || mLcvListData.Count == 0)
            {
                lblNotFound.Visibility = Visibility.Visible;
                dgPartnerClassifications.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNotFound.Visibility = Visibility.Collapsed;
                dgPartnerClassifications.Visibility = Visibility.Visible;
            }
        }

        private void txtCustomer_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            try
            {
                if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
                {
                    string lStrText = (pObjSender as TextBox).Text;
                    List<Partner> lLstObjSellers = mObjBusinessFactory.GetPartnerService().SearchPartner(lStrText, FilterEnum.ACTIVE);

                    if (lLstObjSellers.Count == 1)
                    {
                        SetCustomerObject(lLstObjSellers[0]);
                    }
                    else
                    {
                        (pObjSender as TextBox).Focusable = false;
                        UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjSellers, FilterEnum.ACTIVE);
                        SetCustomerObject(FunctionsUI.ShowWindowDialog(lUCPartner, this.GetParent()) as Partner);
                        (pObjSender as TextBox).Focusable = true;
                    }
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        #endregion

        #region DataGrid

        private void dgPartnerClassifications_MouseDoubleClick(object pObjSender, MouseButtonEventArgs pObjArgs)
        {
            PartnerClassificationDTO lObjClassification = dgPartnerClassifications.SelectedItem as PartnerClassificationDTO;
            LoadControls(lObjClassification);

            mLonId = lObjClassification.Id;
            mLonPartnerId = lObjClassification.CustomerId;

            btnSave.Content = "Modificar";
            lblTitle.Content = "Modificar registro";
            btnNew.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
        }

        private void dgPartnerClassifications_SizeChanged(object pObjSender, SizeChangedEventArgs pObjArgs)
        {
            LoadPagedList();
        }

        #endregion

        #region Buttons

        private void btnSave_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            if (ValidateControls())
            {
                mObjWorker = new Thread(() => SavePartner());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Clasificación de clientes", "Favor de completar los campos.", Window.GetWindow(this));
            }
        }

        private void btnNew_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            ClearControls();
        }

        private void btnDelete_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
                {
                    this.mObjBusinessFactory.GetPartnerClassificationService().Remove(mLonId);
                    this.SearchPagedList(txtSearch.Text);
                    this.ClearControls();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void btnPrevious_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            GoToPreviousPage();
        }

        private void btnNext_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            GoToNextPage();
        }

        #endregion

        #endregion

        #region Methods

        #region DataGridView

        private void LoadPagedList()
        {
            mIntRowsPerPage = ((int)dgPartnerClassifications.ActualHeight - 50) / 38;
            UpdateCurrentPage(mIntPageNumber, mIntRowsPerPage);
        }

        private void ReloadPagedList()
        {
            mLstObjQuery = mObjBusinessFactory.GetPartnerClassificationService().GetList().OrderBy(x => x.Name);
            UpdateCurrentPage(1, mIntRowsPerPage);
        }

        private void SearchPagedList(string pStrSearchText)
        {
            mLstObjQuery = mObjBusinessFactory.GetPartnerClassificationService().GetList();

            if (!string.IsNullOrEmpty(pStrSearchText))
            {
                mLstObjQuery = mLstObjQuery.Where(x =>
                                (
                                    x.Number.ToString().Contains(pStrSearchText) ||
                                    x.Name.ToUpper().Contains(pStrSearchText.ToUpper()) ||
                                    x.Customer.Code.ToUpper().Contains(pStrSearchText.ToUpper()) ||
                                    x.Customer.Name.ToUpper().Contains(pStrSearchText.ToUpper()))
                                );
            }

            mLstObjQuery = mLstObjQuery.OrderBy(x => x.Name);
            UpdateCurrentPage(1, mIntRowsPerPage);
        }

        private void GoToPreviousPage()
        {
            if (mLstObjRows.HasPreviousPage)
            {
                UpdateCurrentPage(--mIntPageNumber, mIntRowsPerPage);
            }
        }

        private void GoToNextPage()
        {
            if (mLstObjRows.HasNextPage)
            {
                UpdateCurrentPage(++mIntPageNumber, mIntRowsPerPage);
            }
        }

        private async void UpdateCurrentPage(int pIntPageNumber = 1, int pIntPageSize = 10)
        {
            try
            {
                //Get paged list
                mLstObjRows = await GetPagedListAsync(pIntPageNumber, pIntPageSize);

                //Enable or disable buttons
                btnPrevious.IsEnabled = mLstObjRows.HasPreviousPage;
                btnNext.IsEnabled = mLstObjRows.HasNextPage;

                //Load list on grid
                mLcvListData = new ListCollectionView(mObjBusinessFactory.GetPartnerClassificationService().ParseToDto(mLstObjRows.ToList()));
                dgPartnerClassifications.ItemsSource = null;
                dgPartnerClassifications.ItemsSource = mLcvListData;

                //Update pagination
                mIntPageNumber = pIntPageNumber;
                lblPageNumber.Text = string.Format("Página {0}/{1}", mIntPageNumber, mLstObjRows.PageCount);
            }
            catch (Exception lObjException)
            {

                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }

        }

        private async Task<IPagedList<PartnerClassification>> GetPagedListAsync(int pIntPageNumber = 1, int pIntPageSize = 10)
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                {
                    if (mLstObjQuery == null)
                    {
                        mLstObjQuery = mObjBusinessFactory.GetPartnerClassificationService().GetList().OrderBy(x => x.Name);
                    }

                    return mLstObjQuery.ToPagedList(pIntPageNumber, pIntPageSize);
                });
            }
            catch (Exception lObjException)
            {
                //CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
                return null;
            }

        }

        #endregion

        #region Customer

        private void SetCustomerObject(Partner pObjCustomer)
        {
            try
            {
                if (pObjCustomer != null)
                {
                    mLonPartnerId = pObjCustomer.Id;
                    txtCustomerCode.Text = pObjCustomer.Code;
                    txtCustomerName.Text = pObjCustomer.Name;
                }
                else
                {
                    ResetCustomer();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void ResetCustomer()
        {
            txtCustomerCode.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
        }

        #endregion

        #region Form

        private void SavePartner()
        {
            FormLoading();
            try
            {
                this.mObjBusinessFactory.GetPartnerClassificationService().SaveOrUpdate(GetPartnerClassification());
                this.Dispatcher.Invoke(() => ClearControls());
                this.Dispatcher.Invoke(() => SearchPagedList(txtSearch.Text));
            }
            catch (Exception lObjException)
            {
                FormDefault();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                FormDefault();
            }
        }

        private PartnerClassification GetPartnerClassification()
        {
            return (PartnerClassification)this.Dispatcher.Invoke(new Func<PartnerClassification>(() =>
            {
                return new PartnerClassification()
                {
                    Id = mLonId,
                    Number = Convert.ToInt32(txtNumber.Text),
                    Name = txtName.Text,
                    Description = string.Empty,
                    CustomerId = mLonPartnerId
                };
            }));
        }

        private void LoadControls(PartnerClassificationDTO pObjClassification)
        {
            txtNumber.Text = pObjClassification.Number.ToString();
            txtName.Text = pObjClassification.Name;
            txtCustomerCode.Text = pObjClassification.CustomerCode;
            txtCustomerName.Text = pObjClassification.CustomerName;
        }

        private void ClearControls()
        {
            this.Dispatcher.Invoke(() =>
            {
                mLonId = 0;
                mLonPartnerId = 0;
                txtNumber.Text = mObjBusinessFactory.GetPartnerClassificationService().GetNextNumber().ToString();
                txtName.Text = string.Empty;
                txtCustomerCode.Text = string.Empty;
                txtCustomerName.Text = string.Empty;

                lblTitle.Content = "Nuevo registro";
                btnDelete.Visibility = Visibility.Collapsed;
                btnNew.Visibility = Visibility.Collapsed;
                lblMessage.Visibility = Visibility.Collapsed;
                lblValidateCustomer.Visibility = Visibility.Collapsed;

                btnSave.Content = "Guardar";
                List<Control> lLstControls = new List<Control>();
                lLstControls.Add(txtNumber);
                lLstControls.Add(txtName);
                lLstControls.Add(txtCustomerCode);
                lLstControls.Add(txtCustomerName);
                ModifyControlColorBlack(lLstControls);
                dgPartnerClassifications.ItemsSource = null;
            });
        }

        private bool ValidateControls()
        {
            List<Control> lLstControls = new List<Control>();
            bool lBoolOk = true;

            if (string.IsNullOrEmpty(txtNumber.Text))
            {
                lLstControls.Add(txtNumber);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                lLstControls.Add(txtName);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtCustomerCode.Text))
            {
                lLstControls.Add(txtCustomerCode);
                lBoolOk = false;
            }

            ModifyControlColorRed(lLstControls);
            return lBoolOk;
        }

        private void ModifyControlColorRed(List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Red;
            }
        }

        private void ModifyControlColorBlack(List<Control> pLstcontrol)
        {
            foreach (Control lObjControl in pLstcontrol)
            {
                lObjControl.BorderBrush = Brushes.Black;
            }
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdParnerClassification.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdParnerClassification.UnblockUI();
            });
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        #endregion

        #endregion
    }
}
