using UGRS.AddOn.Permissions.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.Permissions.Services
{
    public class SetupService
    {
        private TableDAO<EarringRanksT> mObjEarringRanksDAO;
        private TableDAO<PrefixesT> mObjPrefixesDAO;

        public SetupService()
        {
            mObjPrefixesDAO = new TableDAO<PrefixesT>();
            mObjEarringRanksDAO = new TableDAO<EarringRanksT>();
        }


        public void InitializeTables()
        {
            mObjEarringRanksDAO.Initialize();
            mObjPrefixesDAO.Initialize();
        }
    }
}
