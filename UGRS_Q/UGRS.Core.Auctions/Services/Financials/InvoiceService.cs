using System;
using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Financials;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Core.Auctions.Services.Financials
{
    public class InvoiceService
    {
        private IBaseDAO<Invoice> mObjInvoiceDAO;
        private IBaseDAO<InvoiceLine> mObjInvoiceLineDAO;

        public InvoiceService(IBaseDAO<Invoice> pObjInvoiceDAO, IBaseDAO<InvoiceLine> pObjInvoiceLineDAO)
        {
            mObjInvoiceDAO = pObjInvoiceDAO;
            mObjInvoiceLineDAO = pObjInvoiceLineDAO;
        }

        public IQueryable<Invoice> GetList()
        {
            return GetSortedList();
        }

        public IQueryable<Invoice> GetListByStatus(bool pBolActive)
        {
            return GetSortedList().Where(x => x.Active == pBolActive);
        }

        public void SaveOrUpdate(Invoice pObjInvoice)
        {
            if (!Exists(pObjInvoice))
            {
                IList<InvoiceLine> lLstObjLines = pObjInvoice.Lines;
                pObjInvoice.Lines = null;

                mObjInvoiceDAO.SaveOrUpdateEntity(pObjInvoice);

                if (lLstObjLines != null && lLstObjLines.Count > 0)
                {
                    mObjInvoiceLineDAO.SaveOrUpdateEntitiesList(lLstObjLines.Select(x => { x.InvoiceId = pObjInvoice.Id; return x; }).ToList());
                }
            }
            else
            {
                throw new Exception("La factura capturada ya se encuentra registrada.");
            }
        }

        public void Remove(long pLonId)
        {
            mObjInvoiceDAO.RemoveEntity(pLonId);
        }

        private IQueryable<Invoice> GetSortedList()
        {
            return mObjInvoiceDAO.GetEntitiesList().OrderBy(a => a.CreationDate);
        }

        private bool Exists(Invoice pObjInvoice)
        {
            return mObjInvoiceDAO
                    .GetEntitiesList()
                    .Where(x => x.AuctionId == pObjInvoice.AuctionId
                        && x.CardCode == pObjInvoice.CardCode
                        && x.Id != pObjInvoice.Id)
                    .Count() > 0 ? true : false;
        }

      
    }
}
