
namespace UGRS.Object.Auctions.Services
{
    public class SetupService
    {
        #region Attributes

        UGRS.Core.SDK.DI.Auctions.Services.SetupService mObjSapSetupService;

        #endregion

        #region Properties

        public UGRS.Core.SDK.DI.Auctions.Services.SetupService SapSetupService
        {
            get { return mObjSapSetupService; }
            set { mObjSapSetupService = value; }
        }

        #endregion

        #region Contructor

        public SetupService()
        {
            SapSetupService = new UGRS.Core.SDK.DI.Auctions.Services.SetupService();
        }

        #endregion

        #region Methods

        public void InitializeTablesAndFields()
        {
            SapSetupService.InitializeTablesAndFields();
        }

        #endregion
    }
}
