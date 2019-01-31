using UGRS.AddOn.AccountingAccounts.Tables;
using UGRS.Core.SDK.DI.DAO;


namespace UGRS.AddOn.AccountingAccounts.Services
{
    public class AccountingAccountsLoggService
    {
        private TableDAO<AccountingAccountsLogg> mObjLoggDAO;

        public AccountingAccountsLoggService()
        {
            mObjLoggDAO = new TableDAO<AccountingAccountsLogg>();            
        }

        public int Add(AccountingAccountsLogg pObj)
        {
            return mObjLoggDAO.Add(pObj);
        }
        public int Update(AccountingAccountsLogg pObj)
        {
            return mObjLoggDAO.Update(pObj);
        }
        public int Remove(string pStrCode)
        {
            return mObjLoggDAO.Remove(pStrCode);
        }
        public void CreateTable()
        {
            mObjLoggDAO.Initialize();
        }
    }
}
