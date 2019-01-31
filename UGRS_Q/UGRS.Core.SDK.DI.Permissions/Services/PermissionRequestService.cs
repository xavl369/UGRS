// file:	Services\PermissionRequestService.cs
// summary:	Implements the permission request service class

using System;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    /// <summary> A permission request service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class PermissionRequestService
    {
        /// <summary> The object parameter request dao. </summary>
        private TableDAO<PermissionRequest> mObjPermissionRequestDAO;

        private PermissionsDAO mObjPermissionsDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public PermissionRequestService()
        {
            mObjPermissionRequestDAO = new TableDAO<PermissionRequest>();
            mObjPermissionsDAO = new PermissionsDAO();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(PermissionRequest pObjRecord)
        {
            int lIntResult = 0;
            string lStrPrefix = "";

            //CREAR
            if (!mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                if (pObjRecord.MobilizationTypeId == 2)
                {
                    lStrPrefix = "SM";
                }
                else
                {
                    lStrPrefix = "SX";
                }

                pObjRecord.UgrsRequest = lStrPrefix;
                LogService.WriteError("[Prefix ="+lStrPrefix+"]");
                if (mObjPermissionsDAO.GetNextUgrsFolio(lStrPrefix) == 0)
                {
                    LogService.WriteError("[ERROR:NextUGRSFolio = 0]");
                }
                else
                {
                    pObjRecord.UgrsFolio = mObjPermissionsDAO.GetNextUgrsFolio(lStrPrefix);
                    LogService.WriteError("[UGRSFolio =" + pObjRecord.UgrsFolio + "]");
                    lIntResult = mObjPermissionRequestDAO.Add(pObjRecord);
                    LogService.WriteSuccess("[PermissionRequest CREATE]");
                    if (lIntResult == 0 && mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pObjRecord.RequestId))
                    {
                        lIntResult = mObjPermissionsDAO.CreateSaleOrder(pObjRecord.RequestId);
                    }
                }
                
            }
            //EDITAR
            else
            {
                try
                {
                    pObjRecord.RowCode = mObjPermissionsDAO.GetRowCode("[@UG_PE_WS_PERE]", pObjRecord.RequestId);
                    pObjRecord.UgrsFolio = mObjPermissionsDAO.GetUgrsFolio(pObjRecord.RequestId);
                    lIntResult = mObjPermissionRequestDAO.Update(pObjRecord);

                    if (lIntResult == 0)
                    {
                        LogService.WriteSuccess("[PermissionRequest UPDATE]");
                        lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
                        if(lIntResult == 0)
                        {
                            LogService.WriteSuccess("[PermissionRequest SaleOrder UPDATE]");
                        }
                    }
                    else
                    {
                        LogService.WriteError("ERROR:[PermissionRequest UPDATE]");
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteError("ERROR:[PermissionRequest UPDATE] - " + ex.Message);
                }

            }

            return lIntResult;





        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(PermissionRequest pObjRecord)
        {
            int lIntResult = 0;
            lIntResult = mObjPermissionRequestDAO.Update(pObjRecord);

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
            int lIntResult = 0;
            string lStrRequestId = "";

            lStrRequestId = mObjPermissionsDAO.GetRequestIdByPermissionRequestCode(pStrCode);

            if (mObjPermissionsDAO.ExistsSaleOrder(lStrRequestId))
            {
                lIntResult = mObjPermissionRequestDAO.Remove(pStrCode);
                if (lIntResult == 0)
                {
                    lIntResult = mObjPermissionsDAO.CancelSaleOrder(lStrRequestId);
                }
            }
            else
            {
                // La orden de venta no existe
                lIntResult = 1;
            }

            return lIntResult;
        }

        public int CreateSaleOrder(string pStrRequestId)
        {
            int lIntResult = -1;

            if (mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pStrRequestId))
            {
                lIntResult = mObjPermissionsDAO.CreateSaleOrder(pStrRequestId);
            }

            return lIntResult;
        }

        public int TestCreateSalerOrder(string pStrRequestId)
        {
            int lIntResult = -1;

            if (mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pStrRequestId)
                && !mObjPermissionsDAO.ExistsSaleOrder(pStrRequestId))
            {
                lIntResult = mObjPermissionsDAO.CreateSaleOrder(pStrRequestId);
            }

            return lIntResult;
        }
    }
}
