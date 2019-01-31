// file:	SolicitudPermisos.asmx.cs
// summary:	Implements the solicitud permisos.asmx class

using System;
using System.Web.Services;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.SDK.DI.Permissions.Tables;

namespace UGRS.WebService.Permissions
{
    /// <summary>
    /// Summary description for SolicitudPermisos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SolicitudPermisos : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private PermissionsServicesFactory mObjPermissionServices = new PermissionsServicesFactory();

        #region WebMethod CREAR
        [WebMethod]
        public string Crear(
            string id,
            string folio_impreso,
            int id_tipo_movilizacion,
            string tipo_movilizacion,
            string fecha,
            int duracion_dias,
            // DateTime in UGRS WS
            string fecha_validez_inicio,
            // DateTime in UGRS WS
            string fecha_validez_fin,
            int modalidad_destinos,
            string id_grupo_productivo,
            string grupo_productivo,
            int id_productor,
            string productor,
            int id_usuario_fase_administrativa,
            int id_usuario_firma,
            string domicilio_productor,
            string telefono_productor,
            string correo_productor,
            string estado_productor,
            string ciudad_productor,
            int id_estado_origen,
            string estado_origen,
            int id_ciudad_origen,
            string ciudad_origen,
            string ubicacion,
            int puerto_entrada,
            int puerto_salida,
            int trasporte,
            string representante,
            int status_sagarhpa,
            decimal status_sanidad,
            int status_webservice_solicitud,
            int eliminado,
            int id_usuario_firma_sanidad,
            string firma_iqr,
            string firma,
            string firma_real,
            // DateTime in UGRS WS
            string fecha_firma_sanidad,
            string razon_rechazo_aprobacion,
            string comentarios,
            string aduana,
            string destino,
            string clave_cliente,
            // DateTime in UGRS WS
            string fechaCruce)
        {
            LogService.WriteSuccess("[WS Parameters]");
            LogService.WriteSuccess(id);

            try
            {
                int lIntResultCode = mObjPermissionServices.GetPermissionRequestService().Add(new PermissionRequest()
                {
                    RowCode = "",
                    RequestId = id,
                    OriginCity = ciudad_origen,
                    WebServiceRequestStatus = status_webservice_solicitud,
                    ProducerCity = ciudad_productor,
                    Remarks = comentarios,
                    ProducerEmailAddress = correo_productor,
                    ProducerAddress = domicilio_productor,
                    DurationDays = duracion_dias,
                    Deleted = eliminado,
                    OriginState = estado_origen,
                    ProducerState = estado_productor,
                    Date = mObjPermissionServices.UnixTimeStampToDateTime(fecha),
                    HealthSignatureDate = mObjPermissionServices.UnixTimeStampToDateTime(fecha_firma_sanidad),
                    ValidationEndDate = mObjPermissionServices.UnixTimeStampToDateTime(fecha_validez_fin),
                    ValidationStartDate = mObjPermissionServices.UnixTimeStampToDateTime(fecha_validez_inicio),
                    Signature = firma,
                    SignatureIqr = firma_iqr,
                    RealSignature = firma_real,
                    PrintedFolio = folio_impreso,
                    ProductiveGroup = grupo_productivo,
                    OriginCityId = id_ciudad_origen,
                    OriginStateId = id_estado_origen,
                    ProductiveGroupId = id_grupo_productivo,
                    ProducerId = id_productor,
                    MobilizationTypeId = id_tipo_movilizacion,
                    AdministrativePhaseUserId = id_usuario_fase_administrativa,
                    UserSignatureId = id_usuario_firma,
                    HealthUserSignatureId = id_usuario_firma_sanidad,
                    DestinationsModality = modalidad_destinos,
                    Producer = productor,
                    EntryPort = puerto_entrada,
                    DeparturePort = puerto_salida,
                    Representative = representante,
                    ApprovalRejectionReason = razon_rechazo_aprobacion,
                    HealthStatus = status_sanidad,
                    SagarpaStatus = status_sagarhpa,
                    ProducerTelephone = telefono_productor,
                    Transport = trasporte,
                    MobilizationType = tipo_movilizacion,
                    Location1 = ubicacion,
                    Customs = aduana,
                    CustomerCode = clave_cliente,
                    Destination = destino,
                    CrossingDate = mObjPermissionServices.UnixTimeStampToDateTime(fechaCruce),
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[PermissionsRequest SUCCESSFUL]");
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

        #region WebMethod EDITAR
        [WebMethod]
        public string Editar(
            string id,
            int folio,
            string folio_impreso,
            int id_tipo_movilizacion,
            string tipo_movilizacion,
            DateTime fecha,
            int duracion_dias,
            DateTime fecha_validez_inicio,
            DateTime fecha_validez_fin,
            int modalidad_destinos,
            string id_grupo_productivo,
            string grupo_productivo,
            int id_productor,
            string productor,
            int id_usuario_fase_administrativa,
            int id_usuario_firma,
            string domicilio_productor,
            string telefono_productor,
            string correo_productor,
            string estado_productor,
            string ciudad_productor,
            int id_estado_origen,
            string estado_origen,
            int id_ciudad_origen,
            string ciudad_origen,
            string ubicacion,
            int puerto_entrada,
            int puerto_salida,
            int trasporte,
            string representante,
            int status_sagarhpa,
            decimal status_sanidad,
            int status_webservice_solicitud,
            int eliminado,
            int id_usuario_firma_sanidad,
            string firma_iqr,
            string firma,
            string firma_real,
            DateTime fecha_firma_sanidad,
            string razon_rechazo_aprobacion,
            string comentarios,
            string aduana,
            string destino,
            string clave_cliente,
            DateTime fechaCruce)
        {
            LogService.WriteSuccess("[WS Permisos EDITAR]");
            LogService.WriteSuccess(id);
            LogService.WriteSuccess(folio.ToString());
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPermissionRequestService().Update(new PermissionRequest()
                {
                    RowCode = folio.ToString(),
                    RequestId = id,
                    OriginCity = ciudad_origen,
                    WebServiceRequestStatus = status_webservice_solicitud,
                    ProducerCity = ciudad_productor,
                    Remarks = comentarios,
                    ProducerEmailAddress = correo_productor,
                    ProducerAddress = domicilio_productor,
                    DurationDays = duracion_dias,
                    Deleted = eliminado,
                    OriginState = estado_origen,
                    ProducerState = estado_productor,
                    Date = fecha,
                    HealthSignatureDate = fecha_firma_sanidad,
                    ValidationEndDate = fecha_validez_fin,
                    ValidationStartDate = fecha_validez_inicio,
                    Signature = firma,
                    SignatureIqr = firma_iqr,
                    RealSignature = firma_real,
                    PrintedFolio = folio_impreso,
                    ProductiveGroup = grupo_productivo,
                    OriginCityId = id_ciudad_origen,
                    OriginStateId = id_estado_origen,
                    ProductiveGroupId = id_grupo_productivo,
                    ProducerId = id_productor,
                    MobilizationTypeId = id_tipo_movilizacion,
                    AdministrativePhaseUserId = id_usuario_fase_administrativa,
                    UserSignatureId = id_usuario_firma,
                    HealthUserSignatureId = id_usuario_firma_sanidad,
                    DestinationsModality = modalidad_destinos,
                    Producer = productor,
                    EntryPort = puerto_entrada,
                    DeparturePort = puerto_salida,
                    Representative = representante,
                    ApprovalRejectionReason = razon_rechazo_aprobacion,
                    HealthStatus = status_sanidad,
                    SagarpaStatus = status_sagarhpa,
                    ProducerTelephone = telefono_productor,
                    Transport = trasporte,
                    MobilizationType = tipo_movilizacion,
                    Location1 = ubicacion,
                    Customs = aduana,
                    CustomerCode = clave_cliente,
                    Destination = destino,
                    CrossingDate = fechaCruce,
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
        #endregion

        #region WebMethod BORRAR
        [WebMethod]
        public string Borrar(int id)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPermissionRequestService().Remove(id.ToString());
                if (lIntResultCode == 0)
                {
                    return "borrar";
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
            return "no borrar";
        }
        #endregion

        #region TEST
        [WebMethod]
        public string CrearOrdenVenta(string idSolicitud)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPermissionRequestService().CreateSaleOrder(idSolicitud);
                if (lIntResultCode == 0)
                {
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

        [WebMethod]
        public string TestCreateSalerOrder(string idSolicitud)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetPermissionRequestService().TestCreateSalerOrder(idSolicitud);
                if (lIntResultCode == 0)
                {
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
    }
}
