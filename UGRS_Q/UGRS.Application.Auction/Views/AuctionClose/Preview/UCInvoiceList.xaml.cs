using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Financials;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Data.Auctions.Factories;
using UGRS.Core.Application.Extension.Controls;

namespace UGRS.Application.Auctions.AuctionClose
{
    /// <summary>
    /// Interaction logic for UCInvoiceList.xaml
    /// </summary>
    public partial class UCInvoiceList : UserControl
    {
        IList<Invoice> mLstInvoice;
        InventoryServicesFactory mObjInventoryServices = new InventoryServicesFactory();
        BusinessServicesFactory mObjBussinesPartner = new BusinessServicesFactory();
        SystemServicesFactory mObjSystemFactory = new SystemServicesFactory();
        public UCInvoiceList(IList<Invoice> pLstInvoice)
        {
            InitializeComponent();
            mLstInvoice = pLstInvoice;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<InvoiceDTO> lLstInvoiceDTO;
            lLstInvoiceDTO = InvoiceToDTO(mLstInvoice);
            dgBatch.ItemsSource = lLstInvoiceDTO.ToList().Where(x=> x.Import > 0);

        }

        private void btnExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button lObjExpandCollapse = (Button)sender;
                //checa el objeto boton si es null o no
                if (lObjExpandCollapse != null)
                {
                    // Return the Contains which specified element
                    DataGridRow DgrSelectedRowObj = DataGridRow.GetRowContainingElement(lObjExpandCollapse);

                    // Check the DataGridRow Object is Null or Not
                    if (DgrSelectedRowObj != null)
                    {
                        // si el boton ="+" expande los detalles
                        if (lObjExpandCollapse != null && lObjExpandCollapse.Content.ToString() == "+")
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Visible;
                            lObjExpandCollapse.Content = "-";
                        }
                        // else contrae los detalles
                        else
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Collapsed;
                            lObjExpandCollapse.Content = "+";
                        }
                    }
                }
            }
            catch
            {
                //CustomMessageBox.Show("Error", "No se pudieron consultar los detalles", this.GetParent());
            }
        }

        private void dgBatch_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            DataGrid lObjInnerDataGrid = e.DetailsElement.FindChild<DataGrid>();
            lObjInnerDataGrid.ItemsSource =  (dgBatch.SelectedItem as InvoiceDTO).Lines;
        }

        private List<InvoiceDTO> InvoiceToDTO(IList<Invoice> pLstInvoice)
        {
            return pLstInvoice.Select(b => new InvoiceDTO()
            {
                NumAtCard = b.NumAtCard,
                CardCode = b.CardCode,
                CardName = mObjBussinesPartner.GetPartnerService().GetList().Where(x => x.Code == b.CardCode).Select(y => y.Name).First(),
                Import = b.Lines.Where(x => !x.Removed && x.Price > 0).Select(y => y.Quantity * y.Price).Sum(),
                Lines = InvoiceLinesToDTO(b.Lines.Where(x=> x.Price > 0).ToList()),

            }).AsEnumerable().ToList();
        }

        private List<InvoiceLineDTO> InvoiceLinesToDTO(IList<InvoiceLine> pLstInvoiceLine)
        {
            return pLstInvoiceLine.Select(b => new InvoiceLineDTO()
            {
                ItemCode = b.ItemCode,
                ItemName = b.ItemCode == GetConfiguration(ConfigurationKeyEnum.COMISSION_ITEM_CODE) ? "Comision subasta" : 
                            b.ItemCode == GetConfiguration(ConfigurationKeyEnum.FOOD_ITEM_CODE) ? "Alimento" : "",
                Quantity =  b.Quantity,
                Price = b.Price,
                Import = b.Quantity * b.Price
            }
            ).AsEnumerable().ToList();
        }

        private string GetConfiguration(ConfigurationKeyEnum pEnmKey)
        {
            return mObjSystemFactory.GetConfigurationService().GetByKey(pEnmKey);
        }

    }
}
