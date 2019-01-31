using System.Collections.Generic;
using System.Linq;
using UGRS.Core.Auctions.DAO.Base;
using UGRS.Core.Auctions.DTO.Reports.Inventory;
using UGRS.Core.Auctions.Entities.Inventory;

namespace UGRS.Core.Auctions.Services.Reports
{
    public static class InventoryReportServiceExtension
    {
        public static IQueryable<GoodsReceipt> FilterTemporary(this IQueryable<GoodsReceipt> pLstObjGoodsReceipt)
        {
            return pLstObjGoodsReceipt.Where(x => !x.Exported);
        }

        public static IList<GoodsReceiptDTO> ToDTO(this IQueryable<GoodsReceipt> pLstObjGoodsReceipt)
        {
            return pLstObjGoodsReceipt.Select(b => new GoodsReceiptDTO()
            {
                GoodsReceiptId = b.Id,
                Folio = b.Folio,
                Quantity = b.Quantity,
                Exported = b.Exported,
                Remarks = b.Remarks,
                CustomerId = b.CustomerId,
                Customer = b.CustomerId > 0 ? b.Customer.Name : string.Empty,
                ItemId = b.ItemId,
                Item = b.ItemId > 0 ? b.Item.Name : string.Empty,
            })
            .ToList();
        }
    }

    public class InventoryReportService
    {
        private IBaseDAO<GoodsReceipt> mObjGoodsReceiptDAO;

        public InventoryReportService(IBaseDAO<GoodsReceipt> pObjGoodsReceiptDAO)
        {
            mObjGoodsReceiptDAO = pObjGoodsReceiptDAO;
        }

        private IQueryable<GoodsReceipt> GetGoodsReceiptsList()
        {
            return mObjGoodsReceiptDAO.GetEntitiesList();
        }

        public IList<GoodsReceiptDTO> GetTemporaryGoodsReceipts()
        {
            return GetGoodsReceiptsList()
                   .FilterTemporary()
                   .ToDTO();
        }
    }
}
