using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class InvoiceLineService
    {
        private IBaseDAO<InvoiceLine> mObjInvoiceLineDAO;

        public InvoiceLineService(IBaseDAO<InvoiceLine> pObjInvoiceLineDAO)
        {
            mObjInvoiceLineDAO = pObjInvoiceLineDAO;
        }

        public IQueryable<InvoiceLine> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<InvoiceLine> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(InvoiceLine pObjInvoiceLine)
        {
            mObjInvoiceLineDAO.SaveOrUpdateEntity(pObjInvoiceLine);
        }

        public void Remove(long pLonId)
        {
            mObjInvoiceLineDAO.RemoveEntity(pLonId);
        }

        private IQueryable<InvoiceLine> GetSortedList()
        {
            return mObjInvoiceLineDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }
    }
}
