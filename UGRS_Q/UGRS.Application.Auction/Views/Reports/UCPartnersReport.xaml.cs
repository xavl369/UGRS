using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Auctions.DTO.Reports.Business;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Reports.Auctions.Reports.Business;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCPartnersReport.xaml
    /// </summary>
    public partial class UCPartnersReport : UserControl
    {
        #region Attributes

        private ReportsServiceFactory mObjReportFactory;
        private Thread mObjWorker;
        
        #endregion

        #region Constructor

        public UCPartnersReport()
        {
            InitializeComponent();

            mObjReportFactory = new ReportsServiceFactory();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => GetReport());
            mObjWorker.Start();
        }

        #endregion

        #region Methods

        private void GetReport()
        {
            ReportDocument lObjReportDocument = null;

            FormLoading();
            try
            {
                IList<PartnerDTO> lLstObjPartners = mObjReportFactory.GetBusinessReportService().GetTemporaryBusinessPartners();
                lObjReportDocument = new rptPartners();
                lObjReportDocument.SetDataSource(lLstObjPartners);

                lObjReportDocument.SetParameterValue("Location", GetLocation());
                lObjReportDocument.SetParameterValue("ReportName", "Reporte de socios de negocios temporales");

                this.Dispatcher.Invoke((Action)delegate
                {
                    crystalReportsViewer.ViewerCore.ReportSource = lObjReportDocument;
                });
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

        private void ShowMessage(string pStrTitle, string lStrMessage)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(pStrTitle, lStrMessage, this.GetParent()));
        }

        private string GetLocation()
        {
            return ConfigurationUtility.GetValue<string>("Location");
        }

        private void FormLoading()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdContent.BlockUI();
            });
        }

        private void FormDefault()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdContent.UnblockUI();
            });
        }

        #endregion
    }
}
