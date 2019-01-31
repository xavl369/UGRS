using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UGRS.Core.Auctions.DTO.Reports.Auctions;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Auctions;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Extension.Enum;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Reports.Auctions.Reports.Auctions;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Reports.Auctions.Reports.Business;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCInternalBatchesBySellerFilter.xaml
    /// </summary>
    public partial class UCInternalBatchesBySellerFilter : UserControl
    {
        #region Attributes

        private ReportsServiceFactory mObjReportFactory;
        private AuctionsServicesFactory mObjAuctionsFactory;
        private BusinessServicesFactory mObjPartnerFactory;
        private Thread mObjWorker;

        #endregion

        #region Constructor

        public UCInternalBatchesBySellerFilter()
        {
            InitializeComponent();

            mObjReportFactory = new ReportsServiceFactory();
            mObjAuctionsFactory = new AuctionsServicesFactory();
            mObjPartnerFactory = new BusinessServicesFactory();

            cboStatus.ItemsSource = ComboUtility.ParseEnumToCombo<BatchStatusFilterEnum>();
            cboStatus.DisplayMemberPath = "Text";
            cboStatus.SelectedValuePath = "Value";
            cboStatus.SelectedIndex = 0;
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentOrDefaultAuction();
            btnGenerate.Focus();
        }

        private void txtAuctions_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            Auction lObjAuction = null;
            if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                string lStrText = (pObjSender as TextBox).Text;
                List<Auction> lLstObjAuctions = mObjAuctionsFactory.GetAuctionService().SearchAuctions(lStrText, FilterEnum.NONE, AuctionSearchModeEnum.AUCTION);

                if (lLstObjAuctions.Count == 1)
                {
                    lObjAuction = lLstObjAuctions[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCAuction = new UCSearchAuction(lStrText, lLstObjAuctions, FilterEnum.NONE, AuctionSearchModeEnum.AUCTION);
                    lObjAuction = FunctionsUI.ShowWindowDialog(lUCAuction, Window.GetWindow(this)) as Auction;
                    (pObjSender as TextBox).Focusable = true;
                }
                (pObjSender as TextBox).Focus();
                if (lObjAuction != null)
                {
                    (pObjSender as TextBox).Text = lObjAuction.Folio;
                }
            }
        }

        private void txtSeller_KeyDown(object pObjSender, KeyEventArgs pObjArgs)
        {
            Partner lObjPartner = null;
            if (pObjArgs.Key == Key.Enter & ((pObjSender as TextBox).AcceptsReturn == false) && (pObjSender as TextBox).Focus())
            {
                string lStrText = (pObjSender as TextBox).Text;
                List<Partner> lLstObjPartners = mObjPartnerFactory.GetPartnerService().SearchPartner(lStrText, FilterEnum.NONE);

                if (lLstObjPartners.Count == 1)
                {
                    lObjPartner = lLstObjPartners[0];
                }
                else
                {
                    (pObjSender as TextBox).Focusable = false;
                    UserControl lUCPartner = new UCSearchBusinessPartner(lStrText, lLstObjPartners, FilterEnum.NONE);
                    lObjPartner = FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner;
                    (pObjSender as TextBox).Focusable = true;
                }
                (pObjSender as TextBox).Focus();
                if (lObjPartner != null)
                {
                    (pObjSender as TextBox).Text = lObjPartner.Code;
                }
            }
        }

        private void btnGenerate_Click(object pObjSender, RoutedEventArgs pObjArgs)
        {
            mObjWorker = new Thread(() => ShowReportViewer());
            mObjWorker.Start();
        }

        #endregion

        #region Methods

        private ReportDocument GetReport(IList<ReportBatchDTO> pLstObjBatches)
        {
            ReportDocument lObjReportDocument = null;
            lObjReportDocument = new rptInternalBatchesBySeller();
            lObjReportDocument.SetDataSource(pLstObjBatches);

            lObjReportDocument.SetParameterValue("AuctionsFolio", !string.IsNullOrEmpty(txtAuctions.Text) ? txtAuctions.Text : "Todas");
            lObjReportDocument.SetParameterValue("Seller", !string.IsNullOrEmpty(txtSeller.Text) ? txtSeller.Text : "Todos");
            lObjReportDocument.SetParameterValue("Status", GetStatusFilter().GetDescription());
            lObjReportDocument.SetParameterValue("FromDate", dpStartDate.SelectedDate != null ? ((DateTime)dpStartDate.SelectedDate).ToString("dd/MMM/yyyy").Replace(".", "") : "Ninguna");
            lObjReportDocument.SetParameterValue("ToDate", dpEndDate.SelectedDate != null ? ((DateTime)dpEndDate.SelectedDate).ToString("dd/MMM/yyyy").Replace(".", "") : "Ninguna");
            lObjReportDocument.SetParameterValue("Location", GetLocation());

            return lObjReportDocument;
        }

        private UCReportViewer GetReportViewer(IList<ReportBatchDTO> pLstObjBatches)
        {
            UCReportViewer lObjReportViewer = new UCReportViewer();
            lObjReportViewer.LoadReport(GetReport(pLstObjBatches));

            return lObjReportViewer;
        }

        private WindowDialog GetWindowsDialog(IList<ReportBatchDTO> pLstObjBatches)
        {
            WindowDialog lObjWindowDialog = new WindowDialog();
            lObjWindowDialog.Owner = Window.GetWindow(this);
            lObjWindowDialog.WindowState = WindowState.Maximized;
            lObjWindowDialog.grContent.Children.Add(GetReportViewer(pLstObjBatches));
            FormDefult();

            return lObjWindowDialog;
        }

        public DateTime? GetStartDate()
        {
            return this.Dispatcher.Invoke(new Func<DateTime?>(() =>
            {
                return dpStartDate.SelectedDate;
            }));
        }

        public DateTime? GetEndDate()
        {
            return this.Dispatcher.Invoke(new Func<DateTime?>(() =>
            {
                return dpEndDate.SelectedDate;
            }));
        }

        public string GetAuction()
        {
            return this.Dispatcher.Invoke(new Func<string>(() =>
            {
                return txtAuctions.Text;
            }));
        }

        public string GetSeller()
        {
            return this.Dispatcher.Invoke(new Func<string>(() =>
            {
                return txtSeller.Text;
            }));
        }

        public BatchStatusFilterEnum GetStatusFilter()
        {
            return this.Dispatcher.Invoke(new Func<BatchStatusFilterEnum>(() =>
            {
                return (BatchStatusFilterEnum)cboStatus.SelectedValue;
            }));
        }

        private bool? ShowReportViewer()
        {
            bool? lBolResult = false;
            FormLoading();

            try
            {
                IList<ReportBatchDTO> lLstObjBatches = mObjReportFactory.GetAuctionsReportsService().GetInternalBatchesBySeller(GetStartDate(), GetEndDate(), GetAuction(), GetSeller(), GetStatusFilter());

                this.Dispatcher.Invoke((Action)delegate
                {
                    if (lLstObjBatches.Count > 0)
                    {
                        WindowDialog lObjWindowDialog = GetWindowsDialog(lLstObjBatches);
                        lBolResult = lObjWindowDialog.ShowDialog();
                    }
                    else
                    {
                        lBolResult = false;
                        ShowMessage("Busqueda de lotes", "No hay lotes para esta subasta");
                        FormDefult();
                    }
                });

            }
            catch (Exception lObjException)
            {
                FormDefult();
                ShowMessage("Error", lObjException.Message);
            }

            return lBolResult;
        }

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdContent.BlockUI();
            });
        }

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdContent.UnblockUI();
            });
        }

        private string GetLocation()
        {
            return ConfigurationUtility.GetValue<string>("Location");
        }

        private void SetCurrentOrDefaultAuction()
        {
            Auction lObjAuction = mObjAuctionsFactory.GetAuctionService().GetCurrentOrLast(AuctionCategoryEnum.AUCTION);

            if (lObjAuction != null)
            {
                txtAuctions.Text = lObjAuction.Folio;
                dpStartDate.SelectedDate = lObjAuction.Date;
                dpEndDate.SelectedDate = lObjAuction.Date;
            }
            else
            {
                txtAuctions.Text = "";
                dpStartDate.SelectedDate = DateTime.Now;
                dpEndDate.SelectedDate = DateTime.Now;
            }
        }

        #endregion
    }
}
