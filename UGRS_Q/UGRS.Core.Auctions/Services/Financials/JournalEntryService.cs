using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class JournalEntryService
    {
        private IBaseDAO<JournalEntry> mObjJournalEntryDAO;
        private IBaseDAO<JournalEntryLine> mObjJournalEntryLineDAO;

        public JournalEntryService(IBaseDAO<JournalEntry> pObjJournalEntryDAO, IBaseDAO<JournalEntryLine> pObjJournalEntryLineDAO)
        {
            mObjJournalEntryDAO = pObjJournalEntryDAO;
            mObjJournalEntryLineDAO = pObjJournalEntryLineDAO;
        }

        public IQueryable<JournalEntry> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<JournalEntry> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(JournalEntry pObjJournalEntry)
        {
            if (!Exists(pObjJournalEntry))
            {
                IList<JournalEntryLine> lLstObjLines = pObjJournalEntry.Lines;
                pObjJournalEntry.Lines = null;

                mObjJournalEntryDAO.SaveOrUpdateEntity(pObjJournalEntry);

                if (lLstObjLines != null && lLstObjLines.Count > 0)
                {
                    mObjJournalEntryLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.JournalEntryId = pObjJournalEntry.Id; return x; }).ToList());
                }
            }
            else
            {
                throw new Exception("La factura capturada ya se encuentra registrada.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjJournalEntryDAO.RemoveEntity(pLonId);
        }

        private IQueryable<JournalEntry> GetSortedList()
        {
            return mObjJournalEntryDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }

        private bool Exists(JournalEntry pObjJournalEntry)
        {
            return mObjJournalEntryDAO
                    .GetEntitiesList()
                    .Where(x => x.AuctionId == pObjJournalEntry.AuctionId
                        && x.Id != pObjJournalEntry.Id)
                    .Count() > 0 ? true : false;
        }
    }
}
