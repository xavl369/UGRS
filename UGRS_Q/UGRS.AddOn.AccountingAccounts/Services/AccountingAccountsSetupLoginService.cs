using UGRS.AddOn.AccountingAccounts.Tables;
using UGRS.Core.SDK.DI.DAO;


namespace UGRS.AddOn.AccountingAccounts.Services
{
    public class AccountingAccountsSetupLoginService
    {
        private TableDAO<AccountingAccountsSetupLogin> mObjAccountingAccountsSetupLoginDAO;
        private QueryManager mObjQueryManager;

        public AccountingAccountsSetupLoginService()
        {
            mObjAccountingAccountsSetupLoginDAO = new TableDAO<AccountingAccountsSetupLogin>();
            mObjQueryManager = new QueryManager();
        }

        public int Add(AccountingAccountsSetupLogin pObj)
        {
            return mObjAccountingAccountsSetupLoginDAO.Add(pObj);
        }

        public int Update(AccountingAccountsSetupLogin pObj)
        {
            return mObjAccountingAccountsSetupLoginDAO.Update(pObj);
        }

        public int Remove(string pStrCode)
        {
            return mObjAccountingAccountsSetupLoginDAO.Remove(pStrCode);
        }

        public void CreateTable()
        {
            mObjAccountingAccountsSetupLoginDAO.Initialize();
        }

        public AccountingAccountsSetupLogin GetTableByCode(string pStrCode)
        {
            return mObjQueryManager.GetTableObject<AccountingAccountsSetupLogin>("Code", pStrCode, "[@UG_AA_LOGIN]");            
        }
    }
}
