// file:	PermissionFactoryServices.cs
// summary:	Implements the permission factory services class

using System;
using UGRS.Core.SDK.Connection;
using UGRS.Core.SDK.DI.Models;
using UGRS.Core.SDK.DI.Permissions.Services;
using UGRS.Core.SDK.Models;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Permissions
{
    /// <summary> A permission factory services. </summary>
    /// <remarks> Ranaya, 24/05/2017. </remarks>

    public class PermissionsServicesFactory
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public PermissionsServicesFactory()
        {
            DIApplication.DIConnect();
        }

        /// <summary> Gets setup service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The setup service. </returns>

        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        /// <summary> Gets destination request service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The destination request service. </returns>

        public DestinationRequestService GetDestinationRequestService()
        {
            return new DestinationRequestService();
        }

        /// <summary> Gets parameter request service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The parameter request service. </returns>

        public ParameterRequestService GetParameterRequestService()
        {
            return new ParameterRequestService();
        }

        /// <summary> Gets permission request service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The permission request service. </returns>

        public PermissionRequestService GetPermissionRequestService()
        {
            return new PermissionRequestService();
        }

        /// <summary> Gets port request service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The port request service. </returns>

        public PortRequestService GetPortRequestService()
        {
            return new PortRequestService();
        }

        /// <summary> Gets product request service. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <returns> The product request service. </returns>

        public ProductRequestService GetProductRequestService()
        {
            return new ProductRequestService();
        }

        /*
        public DateTime ConvertToDateTime(string pDateFecha)
        {
            DateTime NewDate = ConvertToDateTime(pDateFecha);
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDouDate"></param>
        /// <returns></returns>
        public DateTime UnixTimeStampToDateTime(string pStrDate)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(pStrDate)).ToLocalTime();
            return dtDateTime;
        }

        public string GetRowCode(string pStrRequestId)
        {
            return "";
        }

    }
}
