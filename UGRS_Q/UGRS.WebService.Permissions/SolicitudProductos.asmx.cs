// file:	solicitudproductos.asmx.cs
// summary:	Implements the solicitudproductos.asmx class

using System;
using System.Web.Services;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.SDK.DI.Permissions.Tables;

namespace UGRS.WebService.Permissions
{
    /// <summary>
    /// Summary description for SolicitudProductos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SolicitudProductos : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private PermissionsServicesFactory mObjPermissionServices = new PermissionsServicesFactory();

        #region WebMethod CREAR
        [WebMethod]
        public string Crear(
            string id_solicitud,
            int id_producto,
            int id_producto_padre,
            string producto,
            int cantidad,
            int cantidad_autorizada,
            int id_fin_movilizacion,
            string fin_movilizacion,
            int fin_movilizacion_imprimible,
            string descripcion,
            int estado_destino,
            int ciudad_destino,
            string ubicacion)
        {
            try
            {
                int lIntResultCode = mObjPermissionServices.GetProductRequestService().Add(new ProductRequest()
                {
                    RowCode = "",
                    Quantity = cantidad,
                    AuthorizedQuantity = cantidad_autorizada,
                    Description = descripcion,
                    MobilizationGoal = fin_movilizacion,
                    PrintableMobilizationGoal = fin_movilizacion_imprimible,
                    CityDestination = ciudad_destino,
                    StateDestination = estado_destino,
                    MobilizationGoalId = id_fin_movilizacion,
                    ProductId = id_producto,
                    ParentProductId = id_producto_padre,
                    RequestId = id_solicitud,
                    Product = producto,
                    Location = ubicacion,
                });

                if (lIntResultCode == 0)
                {
                    LogService.WriteSuccess("[ProductRequest SUCCESSFUL]");
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
            int id, 
            string id_solicitud, 
            int id_producto, 
            int id_producto_padre, 
            string producto, 
            int cantidad, 
            int cantidad_autorizada, 
            int id_fin_movilizacion, 
            string fin_movilizacion, 
            int fin_movilizacion_imprimible, 
            string descripcion, 
            int estado_destino, 
            int ciudad_destino, 
            string ubicacion)
        {
            LogService.WriteSuccess("[WS Productos EDITAR]");
            LogService.WriteSuccess("ID:" +id.ToString());
            try
            {
                int lIntResultCode = mObjPermissionServices.GetProductRequestService().Update(new ProductRequest()
                {
                    RowCode = id.ToString(),
                    Quantity = cantidad,
                    AuthorizedQuantity = cantidad_autorizada,
                    Description = descripcion,
                    MobilizationGoal = fin_movilizacion,
                    PrintableMobilizationGoal = fin_movilizacion_imprimible,
                    CityDestination = ciudad_destino,
                    StateDestination = estado_destino,
                    MobilizationGoalId = id_fin_movilizacion,
                    ProductId = id_producto,
                    ParentProductId = id_producto_padre,
                    RequestId = id_solicitud,
                    Product = producto,
                    Location = ubicacion,
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
                int lIntResultCode = mObjPermissionServices.GetProductRequestService().Remove(id.ToString());
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
