// file:	Services\ParameterRequestService.cs
// summary:	Implements the parameter request service class

using System;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.Services;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    /// <summary> A parameter request service. </summary>
    /// <remarks> Ranaya, 23/05/2017. </remarks>

    public class ParameterRequestService
    {
        /// <summary> The object parameter request dao. </summary>
        private TableDAO<ParameterRequest> mObjParameterRequestDAO;

        private PermissionsDAO mObjPermissionsDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public ParameterRequestService()
        {
            mObjParameterRequestDAO = new TableDAO<ParameterRequest>();
            mObjPermissionsDAO = new PermissionsDAO();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>
        public int Add(ParameterRequest pObjRecord)
        {
            int lIntResult = 0;
            //CREAR
            if (!mObjPermissionsDAO.ExistsSaleOrder(pObjRecord.RequestId))
            {
                lIntResult = mObjParameterRequestDAO.Add(pObjRecord);
                if (lIntResult == 0 && mObjPermissionsDAO.IsRequestPreparedToCreateSaleOrder(pObjRecord.RequestId))
                {
                    lIntResult = mObjPermissionsDAO.CreateSaleOrder(pObjRecord.RequestId);
                }
            }
                /*
            //EDITAR
            else
            {
                try
                {
                    pObjRecord.RowCode = mObjPermissionsDAO.GetRowCode("[@UG_PE_WS_PARE]", pObjRecord.RequestId);
                    lIntResult = mObjParameterRequestDAO.Update(pObjRecord);

                    if (lIntResult == 0)
                    {
                        LogService.WriteSuccess("[ParameterRequest UPDATE]");
                        lIntResult = mObjPermissionsDAO.UpdateSaleOrder(pObjRecord.RequestId);
                    }
                    else
                    {
                        LogService.WriteError("ERROR:[ParameterRequest UPDATE]");
                    }
                }
                catch (Exception ex)
                {
                    LogService.WriteError("ERROR:[ParameterRequest UPDATE] - " + ex.Message);
                }

            }
            */
            return lIntResult;

        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(ParameterRequest pObjRecord)
        {
            int lIntResult = 0;
            lIntResult = mObjParameterRequestDAO.Update(pObjRecord);

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
            return mObjParameterRequestDAO.Remove(pStrCode);
        }
    }
}
