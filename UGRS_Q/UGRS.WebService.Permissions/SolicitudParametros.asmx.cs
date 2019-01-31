// file:	SolicitudParametros.asmx.cs
// summary:	Implements the solicitud parametros.asmx class

using System;
using System.Web.Services;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.SDK.DI.Permissions.Tables;

namespace UGRS.WebService.Permissions
{
    /// <summary>
    /// Summary description for SolicitudParametros
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SolicitudParametros : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private PermissionsServicesFactory mObjPermissionServices = new PermissionsServicesFactory();

        #region CREAR
        [WebMethod]
        public string Crear(
            string id_solicitud,
            int id_producto,
            int id_solicitud_producto,
            int id_parametro,
            string parametro,
            decimal tipo_parametro,
            int imprimible,
            decimal precio,
            int cobro_por_permiso,
            int id_valor,
            string valor,
            int id_subvalor,
            string subvalor)
        {

            try
            {
                int lIntResultCode = mObjPermissionServices.GetParameterRequestService().Add(new ParameterRequest()
                {
                    RowCode = "",
                    CostPermission = cobro_por_permiso,
                    ParameterId = id_parametro,
                    ProductId = id_producto,
                    RequestId = id_solicitud,
                    ProductRequestId = id_solicitud_producto,
                    SubValueId = id_subvalor,
                    ValueId = id_valor,
                    Printable = imprimible,
                    Parameter = parametro,
                    Price = precio,
                    SubValue = subvalor,
                    ParameterType = tipo_parametro,
                    Value = valor
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[ParameterRequest SUCCESSFUL]");
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

        [WebMethod]
        public string Editar(
            int id,
            string id_solicitud,
            int id_producto,
            int id_solicitud_producto,
            int id_parametro,
            string parametro,
            decimal tipo_parametro,
            int imprimible,
            decimal precio,
            int cobro_por_permiso,
            int id_valor,
            string valor,
            int id_subvalor,
            string subvalor)
        {
            LogService.WriteSuccess("[WS Parametros EDITAR]");
            LogService.WriteSuccess("ID:" + id.ToString());
            try
            {
                int lIntResultCode = mObjPermissionServices.GetParameterRequestService().Update(new ParameterRequest()
                {
                    RowCode = id.ToString(),
                    CostPermission = cobro_por_permiso,
                    ParameterId = id_parametro,
                    ProductId = id_producto,
                    RequestId = id_solicitud,
                    ProductRequestId = id_solicitud_producto,
                    SubValueId = id_subvalor,
                    ValueId = id_valor,
                    Printable = imprimible,
                    Parameter = parametro,
                    Price = precio,
                    SubValue = subvalor,
                    ParameterType = tipo_parametro,
                    Value = valor
                });

                if (lIntResultCode == 0)
                {
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

        [WebMethod]
        public string Borrar(int id)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetParameterRequestService().Remove(id.ToString());
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
    }
}
