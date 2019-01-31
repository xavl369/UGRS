// file:	Services\ProductRequestService.cs
// summary:	Implements the product request service class

using System;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    /// <summary> A product request service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class ProductRequestService
    {
        /// <summary> The object product request dao. </summary>
        private TableDAO<ProductRequest> mObjProductRequestDAO;

        private PermissionsDAO mObjPermissionsDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public ProductRequestService()
        {
            mObjProductRequestDAO = new TableDAO<ProductRequest>();
            mObjPermissionsDAO = new PermissionsDAO();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>
        public int Add(ProductRequest pObjRecord)
        {
            int lIntResult = 0;

            //CREAR
            if (!mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                lIntResult = mObjProductRequestDAO.Add(pObjRecord);
                if (lIntResult == 0 && mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pObjRecord.RequestId))
                {
                    LogService.WriteSuccess("[ProductRequest CREATED]");
                    lIntResult = mObjPermissionsDAO.CreateSaleOrder(pObjRecord.RequestId);
                }
            }
            //EDITAR
            else
            {
                pObjRecord.RowCode = mObjPermissionsDAO.GetRowCodeByProduct("[@UG_PE_WS_PRRE]", pObjRecord.RequestId, pObjRecord.ProductId);
                lIntResult = mObjProductRequestDAO.Update(pObjRecord);

                if (lIntResult == 0)
                {
                    LogService.WriteSuccess("[ProductRequest UPDATE]");
                    lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
                    if(lIntResult == 0)
                    {
                        LogService.WriteSuccess("[ProductRequest SaleOrder UPDATE]");
                    }
                }
            }

            return lIntResult;
        }


        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>
        public int Update(ProductRequest pObjRecord)
        {
            int lIntResult = 0;
            lIntResult = mObjProductRequestDAO.Update(pObjRecord);

            if (lIntResult == 0
                && mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
            }

            return lIntResult;
        }
        
        /// <summary> Removes this object. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pStrCode"> The String Document entry to remove. </param>
        public int Remove(string pStrCode)
        {
            return mObjProductRequestDAO.Remove(pStrCode);
        }
    }
}
