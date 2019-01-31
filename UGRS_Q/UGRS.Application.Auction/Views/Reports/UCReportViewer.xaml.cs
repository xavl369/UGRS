using CrystalDecisions.CrystalReports.Engine;
using System.Windows.Controls;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCReportViewer.xaml
    /// </summary>
    public partial class UCReportViewer : UserControl
    {
        public UCReportViewer()
        {
            InitializeComponent();
        }

        public void LoadReport(ReportDocument pObjReportDocument)
        {
            crystalReportsViewer.ViewerCore.ReportSource = pObjReportDocument;
        }
    }
}
