using UGRS.AddOn.AccountingAccounts.Tables;
using UGRS.Core.SDK.DI.DAO;


namespace UGRS.AddOn.AccountingAccounts.Services
{
    public class AccountingAccountsLoggDetailService
    {
        private TableDAO<AccountingAccountsLoggDetail> mObjDetailDAO;

        public AccountingAccountsLoggDetailService()
        {
            mObjDetailDAO = new TableDAO<AccountingAccountsLoggDetail>();            
        }

        public int Add(AccountingAccountsLoggDetail pObj)
        {
            return mObjDetailDAO.Add(pObj);
        }
        public int Update(AccountingAccountsLoggDetail pObj)
        {
            return mObjDetailDAO.Update(pObj);
        }
        public int Remove(string pStrCode)
        {
            return mObjDetailDAO.Remove(pStrCode);
        }
        public void CreateTable()
        {
            mObjDetailDAO.Initialize();
        }
    }
}