using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class JournalEntryLineService
    {
        private IBaseDAO<JournalEntryLine> mObjJournalEntryLineDAO;

        public JournalEntryLineService(IBaseDAO<JournalEntryLine> pObjJournalEntryLineDAO)
        {
            mObjJournalEntryLineDAO = pObjJournalEntryLineDAO;
        }

        public IQueryable<JournalEntryLine> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<JournalEntryLine> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(JournalEntryLine pObjJournalEntryLine)
        {
            mObjJournalEntryLineDAO.SaveOrUpdateEntity(pObjJournalEntryLine);
        }

        public void Remove(long pLonId)
        {
            mObjJournalEntryLineDAO.RemoveEntity(pLonId);
        }

        private IQueryable<JournalEntryLine> GetSortedList()
        {
            return mObjJournalEntryLineDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }
    }
}
