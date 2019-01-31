// file:	Services\DestinationRequestService.cs
// summary:	Implements the destination request service class

using System;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    /// <summary> A destination request service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class DestinationRequestService
    {
        /// <summary> The object destination request dao. </summary>
        private TableDAO<DestinationRequest> mObjDestinationRequestDAO;

        private PermissionsDAO mObjPermissionsDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public DestinationRequestService()
        {
            mObjDestinationRequestDAO = new TableDAO<DestinationRequest>();
            mObjPermissionsDAO = new PermissionsDAO();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(DestinationRequest pObjRecord)
        {
            int lIntResult = 0;
            //CREAR
            if (!mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                lIntResult = mObjDestinationRequestDAO.Add(pObjRecord);
                if (lIntResult == 0 && mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pObjRecord.RequestId))
                {
                    LogService.WriteSuccess("[DestinationRequest CREATED]");
                    lIntResult = mObjPermissionsDAO.CreateSaleOrder(pObjRecord.RequestId);
                    if(lIntResult == 0)
                    {
                        LogService.WriteSuccess("[DestinationRequest SaleOrder CREATED]");
                    }
                }
            }
            //EDITAR
            else
            {
                try
                {
                    pObjRecord.RowCode = mObjPermissionsDAO.GetRowCode("[@UG_PE_WS_DERE]", pObjRecord.RequestId);
                    lIntResult = mObjDestinationRequestDAO.Update(pObjRecord);

                    if (lIntResult == 0)
                    {
                        LogService.WriteSuccess("[DestinationRequest UPDATE]");
                        lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
                    }
                    else
                    {
                        LogService.WriteError("ERROR:[DestinationRequest UPDATE]");
                    }
                }
                catch(Exception ex)
                {
                    LogService.WriteError("ERROR:[DestinationRequest UPDATE] - "+ ex.Message);
                }

            }

            return lIntResult;
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>
        public int Update(DestinationRequest pObjRecord)
        {
            int lIntResult = 0;
            lIntResult = mObjDestinationRequestDAO.Update(pObjRecord);

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
            return mObjDestinationRequestDAO.Remove(pStrCode);
        }
    }
}
