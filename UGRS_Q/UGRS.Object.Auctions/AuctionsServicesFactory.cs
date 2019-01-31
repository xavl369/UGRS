using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Services;
using UGRS.Core.Utility;
using UGRS.Object.Auctions.Services;

namespace UGRS.Object.Auctions
{
    public class AuctionsServicesFactory
    {
        #region Constructor
        QueryManager mObjQueryManager;

        public AuctionsServicesFactory()
        {
            LogService.WriteInfo("Connecting");
            Reconnection();
        }

        #endregion

        #region Methods
        public void Reconnection()
        {

            if (DIApplication.Connected)
            {
                try
                {
                    mObjQueryManager = new QueryManager();
                    //test connection
                    string lStrValue = mObjQueryManager.GetValue("U_Value", "Name", "SU_HE_SERIE", "[@UG_CONFIG]");
                }
                catch (System.Exception)
                {
                    DIApplication.DIReconnect();
                    //Check if reconnected
                    LogService.WriteInfo("Intentando reconectar");
                    Reconnection();
                    return;
                }
            }
            else
            {
                DIApplication.DIReconnect();
            }
        }

        public AuctionService GetAuctionService()
        {
            return new AuctionService();
        }

        public BatchService GetBatchService()
        {
            return new BatchService();
        }

        public BatchLineService GetBatchLineService()
        {
            return new BatchLineService();
        }

        public BusinessPartnerService GetBusinessPartnerService()
        {
            return new BusinessPartnerService();
        }

        public FinancialsService GetFinancialsService()
        {
            return new FinancialsService();
        }

        public ItemService GetItemService()
        {
            return new ItemService();
        }

        public StockService GetStockService()
        {
            return new StockService();
        }

        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public OperationsService GetOperationsService()
        {
            LogService.WriteInfo("Starting OperationService");
            return new OperationsService();
        }

        public ConfigurationService GetConfigurationService()
        {
            return new ConfigurationService();
        }

        public FoodDeliveryService GetFoodDeliveryService()
        {
            return new FoodDeliveryService();
        }

        public FoodChargesService GetFoodChargeService()
        {
            return new FoodChargesService();
        }

        #endregion
    }
}
