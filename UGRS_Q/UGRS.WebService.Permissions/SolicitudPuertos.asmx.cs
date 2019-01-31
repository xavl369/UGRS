// file:	solicitudpuertos.asmx.cs
// summary:	Implements the solicitudpuertos.asmx class

using System;
using System.Web.Services;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.SDK.DI.Permissions.Tables;

namespace UGRS.WebService.Permissions
{
    /// <summary>
    /// Summary description for SolicitudPuertos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SolicitudPuertos : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private PermissionsServicesFactory mObjPermissionServices = new PermissionsServicesFactory();

        #region CREAR
        [WebMethod]
        public string Crear(
            string id_solicitud, 
            int id_puerto, 
            int id_punto_inspeccion, 
            string punto_inspeccion, 
            int tipo_puerto)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPortRequestService().Add(new PortRequest()
                {
                    RowCode = "",
                    PortId = id_puerto,
                    InspectionPointId = id_punto_inspeccion,
                    RequestId = id_solicitud,
                    InspectionPoint = punto_inspeccion,
                    PortType = tipo_puerto,
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[PortRequest SUCCESSFUL]");
                    return "creado";
                }
                else
                {
                    LogService.WriteError(lIntResultCode);
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            return "no creado";
        }
        #endregion 

        #region EDITAR
        [WebMethod]
        public string Editar(
            int id, 
            string id_solicitud, 
            int id_puerto, 
            int id_punto_inspeccion, 
            string punto_inspeccion, 
            int tipo_puerto)
        {
            LogService.WriteSuccess("[WS Puertos EDITAR]");
            LogService.WriteSuccess("ID:" + id.ToString());
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPortRequestService().Update(new PortRequest()
                {
                    RowCode = id.ToString(),
                    PortId = id_puerto,
                    InspectionPointId = id_punto_inspeccion,
                    RequestId = id_solicitud,
                    InspectionPoint = punto_inspeccion,
                    PortType = tipo_puerto,
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[WS Puertos EDITAR]");
                    return "editado";
                }
                else
                {
                    LogService.WriteError(lIntResultCode);
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            return "no editado";
        }
        #endregion 

        #region Borrar
        [WebMethod]
        public string Borrar(int id)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPortRequestService().Remove(id.ToString());
                if (lIntResultCode == 0)
                {
                    return "borrado";
                }
                else
                {
                    LogService.WriteError(lIntResultCode);
                }
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
            return "no borrado";
        }
        #endregion
    }
}
