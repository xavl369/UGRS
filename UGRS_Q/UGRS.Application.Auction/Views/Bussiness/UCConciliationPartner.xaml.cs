using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UGRS.Application.Auctions.Utils;
using UGRS.Core.Application.Extension.Controls;
using UGRS.Core.Application.Utility;
using UGRS.Core.Auctions.DTO.Session;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Base;
using UGRS.Core.Auctions.Enums.Business;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Application.Auctions
{
    public partial class UCConciliationPartner : UserControl
    {
        #region Attributes
        private BusinessServicesFactory mObjPartnerFactory = new BusinessServicesFactory();
        private Thread mObjWorker = null;

        #endregion

        #region Constructor

        public UCConciliationPartner()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => LoadDataGrid());
            mObjWorker.Start();
        }

        private void dgPartner_CellDoubleClick(object sender, RoutedEventArgs e)
        {
            var lObjCell = sender as DataGridCell;
            if (lObjCell != null && lObjCell.Content is TextBlock)
            {
                LoadPartnerSAP();
            }
        }

        private void btnConciliate_Click(object sender, RoutedEventArgs e)
        {
            PartnerMappingDTO lObjParnerMap = dgPartner.SelectedItem as PartnerMappingDTO;

            lObjParnerMap.PartnerSAP.Temporary = true;

            LoadPartnerSAP();
        }

        private void chkTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PartnerMappingDTO lObjParnerMap = dgPartner.SelectedItem as PartnerMappingDTO;

            var lObjComboBox = sender as ComboBox;

            if (!lObjComboBox.IsLoaded)
                return;

            string lStrValue = lObjComboBox.SelectedItem.ToString();

            if (lObjComboBox.SelectedValue.ToString() == "System.Windows.Controls.ComboBoxItem: Existente")
            {
                lObjParnerMap.PartnerSAP.Temporary = true;
                dgPartner.Items.Refresh();
                //lObjParnerMap.Type = (MappingTypeEnum)Enum.Parse(typeof(MappingTypeEnum), "EXISTING");
                LoadPartnerSAP();
            }

            if (lObjComboBox.SelectedValue.ToString() == "System.Windows.Controls.ComboBoxItem: Nuevo")
            {
                lObjParnerMap.PartnerSAP = new Partner();
                lObjParnerMap.PartnerSAP.Temporary = false;
                //lObjParnerMap.Type = (MappingTypeEnum)Enum.Parse(typeof(MappingTypeEnum), "NEW");
                dgPartner.Items.Refresh();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            mObjWorker = new Thread(() => SavePartnerMapping());
            mObjWorker.Start();
        }

        #endregion

        #region Methods

        private long GetCurrentUserId()
        {
            return ((SessionDTO)StaticSessionUtility.GetCurrentSession()).Id;
        }

        private void LoadDataGrid()
        {
            grdPartnerConciliation.BlockUI();
            Thread.Sleep(300);

            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    dgPartner.ItemsSource = null;
                });

                List<PartnerMappingDTO> lLstObjMapping = mObjPartnerFactory
                                        .GetPartnerService()
                                        .GetTemporaryAndUnmappedList()
                                        .ToList().Select(x => new PartnerMappingDTO()
                                        {
                                            Partner = x,
                                            //Type = MappingTypeEnum.NEW,
                                            PartnerSAP = new Partner()

                                        }).ToList();

                this.Dispatcher.Invoke(() =>
                {
                    dgPartner.ItemsSource = lLstObjMapping;
                });
            }
            catch (Exception lObjException)
            {
                grdPartnerConciliation.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdPartnerConciliation.UnblockUI();
            }
        }

        private Partner PartnerExist(long pLonPartnerId)
        {
            Partner lObjPartner = mObjPartnerFactory.GetPartnerService().GetEntity(pLonPartnerId);
            return lObjPartner;
        }

        private void LoadPartnerSAP()
        {
            PartnerMappingDTO lObjParnerMap = dgPartner.SelectedItem as PartnerMappingDTO;
            List<Partner> lLstObjSellers = mObjPartnerFactory.GetPartnerService().SearchPartner("", FilterEnum.ACTIVE).Where(x => x.Code != lObjParnerMap.Partner.Code).ToList();
            UserControl lUCPartner = new UCSearchBusinessPartner("", lLstObjSellers, FilterEnum.ACTIVE);
            lObjParnerMap.PartnerSAP = FunctionsUI.ShowWindowDialog(lUCPartner, Window.GetWindow(this)) as Partner;
            if (lObjParnerMap.PartnerSAP != null && lObjParnerMap.PartnerSAP.Code != (dgPartner.SelectedItem as PartnerMappingDTO).Partner.Code)
            {
                lObjParnerMap.PartnerSAP.Temporary = true;
                dgPartner.SelectedItem = lObjParnerMap;
                dgPartner.Items.Refresh();
            }
            else
            {
                lObjParnerMap.PartnerSAP = new Partner();
            }
        }

        private void SavePartnerMapping()
        {
            grdPartnerConciliation.BlockUI();

            try
            {
                List<PartnerMappingDTO> lLstObjMapping = null;

                lLstObjMapping = dgPartner.ItemsSource as List<PartnerMappingDTO>;

                if (lLstObjMapping != null && lLstObjMapping.Count > 0)
                {
                     IList<PartnerMapping> lLstObjPartnerMappingList = null;
                    //IList<PartnerMapping> lLstObjPartnerMappingList = lLstObjMapping.Where(x=> x.PartnerSAP.Temporary).Select(y => new PartnerMapping()
                    if(lLstObjMapping.Select(x=>x.PartnerSAP) != null)
                    {
                    lLstObjPartnerMappingList = lLstObjMapping.Select(y => new PartnerMapping()
                    {
                        //Type = y.Type,
                        Autorized = true,
                        PartnerId = y.Partner.Id,
                        AutorizedByUserId = GetCurrentUserId(),
                        NewPartnerId = (long?)y.PartnerSAP.Id

                    }).ToList();
                    }
                    if (lLstObjPartnerMappingList != null)
                    {
                        mObjPartnerFactory.GetPartnerMappingService().SaveOrUpdateList(lLstObjPartnerMappingList);
                        ShowMessage("Conciliación", "Mapeo guardado correctamente.");
                    }


                }


            }
            catch (Exception lObjException)
            {
                grdPartnerConciliation.UnblockUI();
                ShowMessage("Error", lObjException.Message);
            }
            finally
            {
                grdPartnerConciliation.UnblockUI();
            }
        }

        private void ShowMessage(string pStrTitle, string pStrMessage)
        {
            this.Dispatcher.Invoke(() =>
            {
                CustomMessageBox.Show(pStrTitle, pStrMessage, Window.GetWindow(this));
            });
        }

        #endregion



    }
}
