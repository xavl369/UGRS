using CrystalDecisions.CrystalReports.Engine;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UGRS.Core.Auctions.DTO.Reports.Inventory;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;
using UGRS.Reports.Auctions.Reports.Inventory;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using System;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCGoodsReceiptsReports.xaml
    /// </summary>
    public partial class UCGoodsReceiptsReport : UserControl
    {
        #region Attributes

        private ReportsServiceFactory mObjReportFactory;
        private Thread mObjWorker;
        
        #endregion

        #region Constructor

        public UCGoodsReceiptsReport()
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
                IList<GoodsReceiptDTO> lLstObjGoodsReceipts = mObjReportFactory.GetInventoryReportService().GetTemporaryGoodsReceipts();
                lObjReportDocument = new rptGoodsReceipts();
                lObjReportDocument.SetDataSource(lLstObjGoodsReceipts);

                lObjReportDocument.SetParameterValue("Location", GetLocation());
                lObjReportDocument.SetParameterValue("ReportName", "Reporte de entradas temporales");

                this.Dispatcher.Invoke((Action)delegate
                {
                    crystalReportsViewer.ViewerCore.ReportSource = lObjReportDocument;
                });
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

        private void FormDefult()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                grdContent.UnblockUI();
            });
        }

        #endregion
    }
}
