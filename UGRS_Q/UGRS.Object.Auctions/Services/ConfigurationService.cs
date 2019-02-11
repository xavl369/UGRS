using System;
using System.Linq;
using System.Xml.Linq;
using UGRS.Core.Auctions.Entities.System;
using UGRS.Core.Auctions.Enums.System;
using UGRS.Core.Extension.Xml;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Data.Auctions.Factories;

namespace UGRS.Object.Auctions.Services
{
    public class ConfigurationService
    {
        #region Attributes

        private QueryManager mObjQueryManager;
        private SystemServicesFactory mObjSystemFactory;
        private FinancialsService mObjSapFinancialsService;
        private XDocument mObjXDocument;

        #endregion

        #region Properties

        private QueryManager QueryManager
        {
            get { return mObjQueryManager; }
            set { mObjQueryManager = value; }
        }

        private SystemServicesFactory SystemFactory
        {
            get { return mObjSystemFactory; }
            set { mObjSystemFactory = value; }
        }

        private FinancialsService SapFinancialsService
        {
            get { return mObjSapFinancialsService; }
            set { mObjSapFinancialsService = value; }
        }

        private XDocument Document
        {
            get { return mObjXDocument; }
            set { mObjXDocument = value; }
        }

        #endregion

        #region Contructor

        public ConfigurationService()
        {
            QueryManager = new QueryManager();
            SystemFactory = new SystemServicesFactory();
            SapFinancialsService = new FinancialsService();
            Document = XmlUtility.GetXDocument(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        #endregion

        #region Methods

        public void ProcessConfigurations()
        {
            try
            {
                GetSapConfigurations();

                //AUCTIONS
                UpdateConfiguration(ConfigurationKeyEnum.AUCTIONS_WAREHOUSE, GetAuctionWarehouse());
                UpdateConfiguration(ConfigurationKeyEnum.FOOD_WAREHOUSE, GetFoodWarehouse());
                UpdateConfiguration(ConfigurationKeyEnum.CORRALS_WAREHOUSE, GetCorralsWarehouse());
                UpdateConfiguration(ConfigurationKeyEnum.REJECTION_WAREHOUSE, GetRejectionWarehouse());
                UpdateConfiguration(ConfigurationKeyEnum.DOCUMENTS_SERIES, GetDocumentSeries());
                UpdateConfiguration(ConfigurationKeyEnum.BUSINESS_PARTNER_SERIES, GetBusinessPartnerSeries());

                //FOOD
                UpdateConfiguration(ConfigurationKeyEnum.FOOD_ITEM_CODE, GetFoodItemCode());
                UpdateConfiguration(ConfigurationKeyEnum.FOOD_ITEM_PRICE, GetFoodItemPrice());
                UpdateConfiguration(ConfigurationKeyEnum.FOOD_TAX_CODE, GetFoodTaxCode());
                UpdateConfiguration(ConfigurationKeyEnum.THREE_PERCENT_PAYMENT, GetThreePercentItem());


                //COMISSION
                UpdateConfiguration(ConfigurationKeyEnum.COMISSION_ITEM_CODE, GetComissionItemCode());
                UpdateConfiguration(ConfigurationKeyEnum.COMISSION_TAX_CODE, GetComissionTaxCode());

                //FINANCIALS
                UpdateConfiguration(ConfigurationKeyEnum.AUCTION_COSTING_CODE, GetCostCenter());
                UpdateConfiguration(ConfigurationKeyEnum.DEBTORS_ACCOUNT, GetDebtorsAccount());
                UpdateConfiguration(ConfigurationKeyEnum.CREDITORS_ACCOUNT, GetCreditorsAccount());
                UpdateConfiguration(ConfigurationKeyEnum.GUIDES_ACCOUNT, GetGuidesAccount());
                UpdateConfiguration(ConfigurationKeyEnum.NO_PAYMENT_GUIDES, GetNoPaymmentGuidesAccount());

                //CONFIGURATION
                UpdateConfiguration(ConfigurationKeyEnum.APP_VERSION, GetAppVersion());
                UpdateConfiguration(ConfigurationKeyEnum.SERV_VERSION, GetServiceVersion());

                
            }
            catch (Exception lObjException)
            {
                LogUtility.WriteException(lObjException);
            }
        }

 

        private void GetSapConfigurations()
        {
            if (!GetFoodItemPrice().Equals(GetPrice(GetAuctionWarehouse(), GetFoodItemCode())))
            {
                Document.SetSetting("FoodItemPrice", GetPrice(GetFoodWarehouse(), GetFoodItemCode()));
            }

            if (!GetFoodTaxCode().Equals(GetTaxCode(GetFoodItemCode())))
            {
                Document.SetSetting("FoodTaxCode", GetTaxCode(GetFoodItemCode()));
            }

            if (!GetComissionTaxCode().Equals(GetTaxCode(GetComissionItemCode())))
            {
                Document.SetSetting("ComissionTaxCode", GetTaxCode(GetComissionItemCode()));
            }

            Document.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        private string GetAuctionWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("AuctionsWarehouse");
        }

        private string GetFoodWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("FoodWarehouse");
        }

        private string GetCorralsWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("CorralsWarehouse");
        }

        private string GetRejectionWarehouse()
        {
            return ConfigurationUtility.GetValue<string>("RejectionWarehouse");
        }

        private string GetDocumentSeries()
        {
            return ConfigurationUtility.GetValue<string>("AuctionsSeriesName");
        }

        private string GetBusinessPartnerSeries()
        {
            return ConfigurationUtility.GetValue<string>("BusinessPartnerSeriesName");
        }

        private string GetFoodItemCode()
        {
            return ConfigurationUtility.GetValue<string>("FoodItemCode");
        }

        private string GetFoodItemPrice()
        {
            return ConfigurationUtility.GetValue<string>("FoodItemPrice");
        }

        private string GetFoodTaxCode()
        {
            return ConfigurationUtility.GetValue<string>("FoodTaxCode");
        }

        private string GetComissionItemCode()
        {
            return ConfigurationUtility.GetValue<string>("ComissionItemCode");
        }

        private string GetComissionTaxCode()
        {
            return ConfigurationUtility.GetValue<string>("ComissionTaxCode");
        }

        private string GetCostCenter()
        {
            return ConfigurationUtility.GetValue<string>("CostCenter");
        }

        private string GetDebtorsAccount()
        {
            return ConfigurationUtility.GetValue<string>("DebtorsAccount");
        }

        private string GetCreditorsAccount()
        {
            return ConfigurationUtility.GetValue<string>("CreditorsAccount");
        }

        private string GetGuidesAccount()
        {
            return ConfigurationUtility.GetValue<string>("GuidesAccount");
        }

        private string GetThreePercentItem()
        {
            return ConfigurationUtility.GetValue<string>("ThreePercent");
        }

        private string GetNoPaymmentGuidesAccount()
        {
            return ConfigurationUtility.GetValue<string>("NoPaymmentGuidesAccount");
        }

        private string GetTaxCode(string pStrItemCode)
        {
            return mObjQueryManager.GetValue("LnTaxCode", "StrVal1", pStrItemCode, "OTCX");
        }

        private string GetPrice(string pStrWhsCode, string pStrItemCode)
        {
            return SapFinancialsService.GetPrice(pStrWhsCode, pStrItemCode);
        }

        private string GetAppVersion()
        {
            return mObjQueryManager.GetValue("U_Value","Name",ConfigurationUtility.GetValue<string>("AppVersion"),"[@UG_CONFIG]");
        }

        private string GetServiceVersion()
        {
            return mObjQueryManager.GetValue("U_Value", "Name", ConfigurationUtility.GetValue<string>("ServVersion"), "[@UG_CONFIG]");
        }

        private void UpdateConfiguration(ConfigurationKeyEnum pEnmKey, string pStrValue)
        {
            if (ExistsConfiguration(pEnmKey))
            {
                if (HasChanges(pEnmKey, pStrValue))
                {
                    SystemFactory
                        .GetConfigurationService()
                        .SaveOrUpdate(SystemFactory.GetConfigurationService()
                                        .GetList()
                                        .Where(x => x.Key == pEnmKey)
                                        .AsEnumerable()
                                        .Select(y => { y.Value = pStrValue; return y; })
                                        .FirstOrDefault());
                }
            }
            else
            {
                SystemFactory.GetConfigurationService().SaveOrUpdate(new Configuration()
                {
                    Key = pEnmKey,
                    Value = pStrValue
                });
            }
        }

        private bool ExistsConfiguration(ConfigurationKeyEnum pEnmKey)
        {
            return SystemFactory.GetConfigurationService().GetList().Where(x => x.Key == pEnmKey).Count() > 0;
        }

        private bool HasChanges(ConfigurationKeyEnum pEnmKey, string pStrValue)
        {
            return SystemFactory.GetConfigurationService().GetList().AsEnumerable().Where(x => x.Key == pEnmKey && !x.Value.Equals(pStrValue)).Count() > 0;
        }

        #endregion
    }
}
