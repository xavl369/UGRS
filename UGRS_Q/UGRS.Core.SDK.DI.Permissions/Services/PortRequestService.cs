// file:	Services\PortRequestService.cs
// summary:	Implements the port request service class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    /// <summary> A port request service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class PortRequestService
    {
        /// <summary> The object port request dao. </summary>
        private TableDAO<PortRequest> mObjPortRequestDAO;

        private PermissionsDAO mObjPermissionsDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public PortRequestService()
        {
            mObjPortRequestDAO = new TableDAO<PortRequest>();
            mObjPermissionsDAO = new PermissionsDAO();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(PortRequest pObjRecord)
        {
            int lIntResult = 0;
            //CREAR

            if (!mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                lIntResult = mObjPortRequestDAO.Add(pObjRecord);
                if (lIntResult == 0 && mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pObjRecord.RequestId))
                {
                    LogService.WriteSuccess("[PortRequest CREATED]");
                    lIntResult = mObjPermissionsDAO.CreateSaleOrder(pObjRecord.RequestId);
                }
            }
            //EDITAR
            else
            {
                // New portRequest line
                if (mObjPermissionsDAO.IsPortExist(pObjRecord.RequestId, pObjRecord.PortId, pObjRecord.PortType) == "0")
                {
                    lIntResult = mObjPortRequestDAO.Add(pObjRecord);
                    if(lIntResult == 0)
                    {
                        LogService.WriteSuccess("[PortRequest UPDATE ADDPORT]");
                    }
                }
                // Update portRequest line
                else
                {
                    pObjRecord.RowCode = mObjPermissionsDAO.GetRowCodeByPort(pObjRecord.RequestId, pObjRecord.PortId, pObjRecord.PortType);
                    lIntResult = mObjPortRequestDAO.Update(pObjRecord);

                    if (lIntResult == 0)
                    {
                        LogService.WriteSuccess("[PortRequest UPDATE]");
                        lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
                        if (lIntResult == 0)
                        {
                            LogService.WriteSuccess("[PortRequest SaleOrder UPDATE]");
                        }

                    }

                }

            }

            return lIntResult;
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(PortRequest pObjRecord)
        {
            int lIntResult = 0;
            lIntResult = mObjPortRequestDAO.Update(pObjRecord);

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
            return mObjPortRequestDAO.Remove(pStrCode);
        }
    }
}
