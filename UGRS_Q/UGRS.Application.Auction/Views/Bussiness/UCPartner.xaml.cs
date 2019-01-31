using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Business;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCPartner : UserControl
    {
        #region Attributes

        private BusinessServicesFactory mObjPartnerFactory = new BusinessServicesFactory();
        private ListCollectionView mLcvListData = null;

        private long mLonId = 0;
        private Thread mObjWorker;

        private int mIntPageNumber = 1;
        private int mIntRowsPerPage = 100;
        private IPagedList<Partner> mLstObjRows;
        private IQueryable<Partner> mLstObjQuery;

        #endregion

        #region Constructor

        public UCPartner()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region UserControl

        /// <summary>
        /// Evento al terminar de cargar la pantalla.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();
            LoadPagedList();
        }

        #endregion

        #region TextBox

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
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
                dgPartner.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNotFound.Visibility = Visibility.Collapsed;
                dgPartner.Visibility = Visibility.Visible;
            }
        }

        private void txtRFC_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRFC.Text) || txtRFC.Text.Length > 13 || txtRFC.Text.Length < 12)
            {
                LblValidateRFC.Visibility = Visibility.Visible;
                txtRFC.BorderBrush = Brushes.Red;
            }
            else
            {
                LblValidateRFC.Visibility = Visibility.Collapsed;
                txtRFC.BorderBrush = Brushes.Black;
            }
        }

        #endregion

        #region DataGrid

        private void dgPartner_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Partner lObjPartner = dgPartner.SelectedItem as Partner;
            LoadControls(lObjPartner);
            mLonId = lObjPartner.Id;
            btnSave.Content = "Modificar";
            lbltitle.Content = "Modificar registro";
            btnNew.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
        }

        private void dgPartner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //LoadPagedList();
        }

        #endregion

        #region Button

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateControls())
            {
                mObjWorker = new Thread(() => SavePartner());
                mObjWorker.Start();
            }
            else
            {
                CustomMessageBox.Show("Socio de negocios", "Favor de completar los campos.", Window.GetWindow(this));
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomMessageBox.ShowOption("Eliminar", "¿Desea eliminar el registro?", "Si", "No", "", Window.GetWindow(this)) == true)
                {
                    mObjPartnerFactory.GetPartnerService().Remove(mLonId);
                    ClearControls();
                }
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GoToNextPage();
        }

        #endregion

        #endregion

        #region Methods

        private void LoadComboBox()
        {
            cbPartnerStatus.ItemsSource = Enum.GetValues(typeof(PartnerStatusEnum)).Cast<PartnerStatusEnum>();
        }

        private void SavePartner()
        {
            FormLoading();
            try
            {
                this.mObjPartnerFactory.GetPartnerService().SaveOrUpdate(GetPartnerObject());
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

        private Partner GetPartnerObject()
        {
            return (Partner)this.Dispatcher.Invoke(new Func<Partner>(() =>
            {
                return new Partner()
                {
                    Id = mLonId,
                    Code = txtCode.Text,
                    Name = txtName.Text,
                    ForeignName = txtForeignName.Text,
                    TaxCode = txtRFC.Text,
                    PartnerStatus = (PartnerStatusEnum)Enum.Parse(typeof(PartnerStatusEnum), cbPartnerStatus.Text),
                    Temporary = true
                };
            }));
        }

        private void ClearControls()
        {
            txtCode.Text = string.Empty;
            txtName.Text = string.Empty;
            txtRFC.Text = string.Empty;
            txtForeignName.Text = string.Empty;
            cbPartnerStatus.SelectedItem = null;
            btnSave.Content = "Guardar";
            lbltitle.Content = "Nuevo registro";
            mLonId = 0;
            btnDelete.Visibility = Visibility.Collapsed;
            btnNew.Visibility = Visibility.Collapsed;
            lblMessage.Visibility = Visibility.Collapsed;
            LblValidateRFC.Visibility = Visibility.Collapsed;

            List<Control> lLstControls = new List<Control>();
            lLstControls.Add(txtName);
            lLstControls.Add(txtForeignName);
            lLstControls.Add(txtCode);
            lLstControls.Add(txtRFC);
            lLstControls.Add(cbPartnerStatus);
            ModifyControlColorBlack(lLstControls);

            LoadComboBox();
            dgPartner.ItemsSource = null;

        }

        private void LoadControls(Partner pObjPartner)
        {
            txtCode.Text = pObjPartner.Code;
            txtName.Text = pObjPartner.Name;
            txtForeignName.Text = pObjPartner.ForeignName;
            txtRFC.Text = pObjPartner.TaxCode;
            cbPartnerStatus.SelectedItem = pObjPartner.PartnerStatus;
        }

        private bool ValidateControls()
        {
            List<Control> lLstControls = new List<Control>();
            bool lBoolOk = true;

            if (string.IsNullOrEmpty(txtName.Text))
            {
                lLstControls.Add(txtName);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtCode.Text))
            {
                lLstControls.Add(txtCode);
                lBoolOk = false;
            }

            if (string.IsNullOrEmpty(txtRFC.Text))
            {
                lLstControls.Add(txtRFC);
                lBoolOk = false;
            }
            if (cbPartnerStatus.SelectedValue == null)
            {
                lLstControls.Add(cbPartnerStatus);
                lBoolOk = false;
            }
            if (LblValidateRFC.Visibility == Visibility.Visible)
            {
                lBoolOk = false;
            }
            //lblMessage.Visibility = Visibility.Visible;
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

        #region DataGridView

        private void LoadPagedList()
        {
            mIntRowsPerPage = ((int)dgPartner.ActualHeight - 50) / 38;
            UpdateCurrentPage(mIntPageNumber, mIntRowsPerPage);
        }

        private void ReloadPagedList()
        {
            mLstObjQuery = mObjPartnerFactory.GetPartnerService().GetList().OrderBy(x => x.Name);

            UpdateCurrentPage(1, mIntRowsPerPage);
        }

        private void SearchPagedList(string pStrSearchText)
        {
            mLstObjQuery = mObjPartnerFactory.GetPartnerService().GetList();

            if (!string.IsNullOrEmpty(pStrSearchText))
            {
                mLstObjQuery = mLstObjQuery.Where(x =>
                                (
                                    x.Name.ToUpper().Contains(pStrSearchText.ToUpper()) ||
                                    x.Code.ToUpper().Contains(pStrSearchText.ToUpper()))
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
            //Get paged list
            mLstObjRows = await GetPagedListAsync(pIntPageNumber, pIntPageSize);

            //Enable or disable buttons
            btnPrevious.IsEnabled = mLstObjRows.HasPreviousPage;
            btnNext.IsEnabled = mLstObjRows.HasNextPage;

            //Load list on grid
            mLcvListData = new ListCollectionView(mLstObjRows.ToList());
            dgPartner.ItemsSource = null;
            dgPartner.ItemsSource = mLcvListData;

            //Update pagination
            mIntPageNumber = pIntPageNumber;
            lblPageNumber.Text = string.Format("Página {0}/{1}", mIntPageNumber, mLstObjRows.PageCount);
        }

        private async Task<IPagedList<Partner>> GetPagedListAsync(int pIntPageNumber = 1, int pIntPageSize = 10)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (mLstObjQuery == null)
                {
                    mLstObjQuery = mObjPartnerFactory.GetPartnerService().GetList().OrderBy(x => x.Name);
                }

                return mLstObjQuery.ToPagedList(pIntPageNumber, pIntPageSize);
            });

        }

        #endregion

        #region Form

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdPartner.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdPartner.UnblockUI();
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
