using System;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    /// <summary>
    /// Interaction logic for UCPrintBatch.xaml
    /// </summary>
    public partial class UCPrintBatch : UserControl
    {
        AuctionsServicesFactory mObjAuctionsFactory;
        BusinessServicesFactory mObjBusinessFactory;
        InventoryServicesFactory mObjInventoryFactory;  

        public UCPrintBatch(Batch pObjBatch)
        {

            mObjAuctionsFactory = new AuctionsServicesFactory();
            mObjBusinessFactory = new BusinessServicesFactory();
            mObjInventoryFactory = new InventoryServicesFactory();

            InitializeComponent();
            LoadData(pObjBatch);
            CreateDocument();
        }


        /// <summary>
        /// Carga los datos agregando texto a los ya existentes
        /// </summary>
        private void LoadData(Batch pObjBatch)
        {
            lblAuction.Text += " " + GetAuctionFolio(pObjBatch.AuctionId);
            lblDate.Text += " " + pObjBatch.CreationDate.ToString();
            lblBatch.Text += " " + pObjBatch.Number.ToString();
            lblRR.Text += " " + (pObjBatch.Reprogrammed ? "SI" : "NO");
            lblSold.Text += " " + (!pObjBatch.Unsold ? "SI" : "NO");
            lblItem.Text += " " + GetItemTypeCode(pObjBatch.ItemTypeId).ToUpper();
            lblQuantity.Text += " " + pObjBatch.Quantity.ToString();
            lblWeight.Text += " " + pObjBatch.Weight.ToString();
            lblAverageWeight.Text += " " + pObjBatch.AverageWeight.ToString();
            lblSellerName.Text += " \n" + GetCustomerName(pObjBatch.SellerId ?? 0).ToUpper();
            lblPrice.Text += " " + pObjBatch.Price.ToString("C");
            lblTotal.Text += " " + (float.Parse(pObjBatch.Price.ToString()) * pObjBatch.Weight).ToString("C");
            lblBuyercode.Text += " " + GetCustomerClassificationCode((int?)pObjBatch.BuyerClassificationId ?? 0).ToUpper();
            lblBuyerName.Text += " \n" + GetCustomerName(pObjBatch.BuyerId ?? 0).ToUpper();
        }

        private string GetAuctionFolio(long pLonAuctionId)
        {
            return mObjAuctionsFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Id == pLonAuctionId).Count() > 0 ?
                   mObjAuctionsFactory.GetAuctionService().GetListFilteredByCC().Where(x => x.Id == pLonAuctionId).Select(y => y.Folio).FirstOrDefault() : string.Empty;
        }

        private string GetCustomerClassificationCode(long pLonCustomerId)
        {
            return mObjBusinessFactory.GetPartnerClassificationService().GetList().Where(x => x.Id == pLonCustomerId).Count() > 0 ?
                   mObjBusinessFactory.GetPartnerClassificationService().GetList().Where(x => x.Id == pLonCustomerId).Select(y => y.Number).FirstOrDefault().ToString() : string.Empty;
        }

        private string GetCustomerName(long pLonCustomerId)
        {
            return mObjBusinessFactory.GetPartnerService().GetList().Where(x => x.Id == pLonCustomerId).Count() > 0 ?
                   mObjBusinessFactory.GetPartnerService().GetList().Where(x => x.Id == pLonCustomerId).Select(y => y.Name).FirstOrDefault() : string.Empty;
        }

        private string GetItemTypeCode(long? pLonItemTypeId)
        {
            return mObjInventoryFactory.GetItemTypeService().GetList().Where(x => x.Id == pLonItemTypeId).Count() > 0 ?
                   mObjInventoryFactory.GetItemTypeService().GetList().Where(x => x.Id == pLonItemTypeId).Select(y => y.Code).FirstOrDefault() : string.Empty;
        }

        private string GetItemTypeName(long? pLonItemTypeId)
        {
            return mObjInventoryFactory.GetItemTypeService().GetList().Where(x => x.Id == pLonItemTypeId).Count() > 0 ?
                   mObjInventoryFactory.GetItemTypeService().GetList().Where(x => x.Id == pLonItemTypeId).Select(y => y.Name).FirstOrDefault() : string.Empty;
        }

        /// <summary>
        /// Crea Página
        /// </summary>
        public void CreateDocument()
        {
            FixedDocument lFdDocument = new FixedDocument();
            PrinterSettings lObjPrinterSettings = new PrinterSettings();
            int lIntWidth = lObjPrinterSettings.DefaultPageSettings.PaperSize.Width;
            int lIntHeight = lObjPrinterSettings.DefaultPageSettings.PaperSize.Height;
            lFdDocument.DocumentPaginator.PageSize = new Size(lIntWidth, lIntHeight);//new Size(96 * 8.5, 96 * 11); //Tamaño carta A4

            FixedPage lFpPage = new FixedPage();
            UIElement lUIContenido = skpContent;
            skpMain.Children.Remove(skpContent);
            lFpPage.Children.Add(lUIContenido);

            PageContent lPcPageContent = new PageContent();
            ((IAddChild)lPcPageContent).AddChild(lFpPage);
            lFdDocument.Pages.Add(lPcPageContent);
            Print(lFdDocument, lObjPrinterSettings.PrinterName);
            dvPrint.Document = lFdDocument;
            // SaveCurrentDocument(doc, "prueba");
            // PdfSharp.Xps.XpsConverter.Convert("path", "path", 0);
        }

        /// <summary>
        /// Manda a imprimir a la impresora seleccionada en los ajustes
        /// </summary>
        private void Print(FixedDocument pFDPage, string pStrPrintName)
        {
            LocalPrintServer lObjPrintServer = new LocalPrintServer();
            PrinterSettings lObjPrinterSettings = new PrinterSettings();
            PrintQueue lObjPrintQueue = new PrintQueue(lObjPrintServer, Auctions.Properties.Settings.Default.gStrBatchPrinter);
           // var x = lObjPrinterSettings.DefaultPageSettings.PaperSize;
            // x.PaperSize;
            if (lObjPrintQueue == null) return;
            XpsDocumentWriter lObjXpswriter = PrintQueue.CreateXpsDocumentWriter(lObjPrintQueue);
            try
            {
                lObjXpswriter.Write(pFDPage);
            }
            catch (Exception lObjException)
            {
                CustomMessageBox.Show("Error", lObjException.Message, this.GetParent());
            } 
        }
    }
}
