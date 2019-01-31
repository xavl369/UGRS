// file:	SolicitudDestinos.asmx.cs
// summary:	Implements the solicitud destinos.asmx class

using System;
using System.Web.Services;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.SDK.DI.Permissions.Tables;

namespace UGRS.WebService.Permissions
{
    /// <summary>
    /// Summary description for SolicitudDestinos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SolicitudDestinos : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private PermissionsServicesFactory mObjPermissionServices = new PermissionsServicesFactory();
        #region WebMethod CREAR
        [WebMethod]
        public string Crear(
            string id_solicitud,
            int id_estado,
            string estado,
            int id_ciudad,
            string ciudad,
            string ubicacion,
            int id_punto_inspeccion,
            string punto_inspeccion,
            int id_producto)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetDestinationRequestService().Add(new DestinationRequest()
                {
                    RowCode = "",
                    City = ciudad,
                    State = estado,
                    CityId = id_ciudad,
                    StateId = id_estado,
                    ProductId = id_producto,
                    InspectionPointId = id_punto_inspeccion,
                    RequestId = id_solicitud,
                    InspectionPoint = punto_inspeccion,
                    Location = ubicacion
                });

                if(lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[DestinationRequest SUCCESSFUL]");
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

        #region WebMethod Editar
        [WebMethod]
        public string Editar(
            int id,
            string id_solicitud,
            int id_estado,
            string estado,
            int id_ciudad,
            string ciudad,
            string ubicacion,
            int id_punto_inspeccion,
            string punto_inspeccion,
            int id_producto)
        {
            LogService.WriteSuccess("[WS Destinos EDITAR]");
            LogService.WriteSuccess("ID:"+id.ToString());
            try
            {
                int lIntResultCode = mObjPermissionServices.GetDestinationRequestService().Update(new DestinationRequest()
                {
                    RowCode = id.ToString(),
                    City = ciudad,
                    State = estado,
                    CityId = id_ciudad,
                    StateId = id_estado,
                    ProductId = id_producto,
                    InspectionPointId = id_punto_inspeccion,
                    RequestId = id_solicitud,
                    InspectionPoint = punto_inspeccion,
                    Location = ubicacion
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[Destinos EDITADO]");
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

        #region WebMethod Borrar
        [WebMethod]
        public string Borrar(int id)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetDestinationRequestService().Remove(id.ToString());
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
