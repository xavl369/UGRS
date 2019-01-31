using UGRS.AddOn.AccountingAccounts.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.AddOn.AccountingAccounts.Services
{    
    public class AccountingAccountsSetupDBService
    {
        private TableDAO<AccountingAccountsSetupDB> mObjSetupDBDAO;
        private QueryManager mObjQueryManager;

        public AccountingAccountsSetupDBService()
        {
            mObjSetupDBDAO = new TableDAO<AccountingAccountsSetupDB>();
            mObjQueryManager = new QueryManager();
        }

        public int Add(AccountingAccountsSetupDB pObj)
        {
            return mObjSetupDBDAO.Add(pObj);
        }

        public int Update(AccountingAccountsSetupDB pObj)
        {
            return mObjSetupDBDAO.Update(pObj);
        }

        public int Remove(string pStrCode)
        {
            return mObjSetupDBDAO.Remove(pStrCode);
        }

        public void CreateTable()
        {
            mObjSetupDBDAO.Initialize();
        }

        public AccountingAccountsSetupDB GetTableByCode( string pStrCode)
        {
            return mObjQueryManager.GetTableObject<AccountingAccountsSetupDB>("Code", pStrCode, "[@UG_AA_DB]");
        }
    }
}
