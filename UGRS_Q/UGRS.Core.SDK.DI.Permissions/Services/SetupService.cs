// file:	Services\DatabaseService.cs
// summary:	Implements the setup service class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Permissions.Tables;
using UGRS.Core.SDK.DI.Permissions.UserFields;

namespace UGRS.Core.SDK.DI.Permissions.Services
{
    public class SetupService
    {
        /// <summary> The object destination request dao. </summary>
        private TableDAO<DestinationRequest> mObjDestinationRequestDAO;

        /// <summary> The object parameter request dao. </summary>
        private TableDAO<ParameterRequest> mObjParameterRequestDAO;

        /// <summary> The object permission request dao. </summary>
        private TableDAO<PermissionRequest> mObjPermissionRequestDAO;

        /// <summary> The object port request dao. </summary>
        private TableDAO<PortRequest> mObjPortRequestDAO;

        /// <summary> The object product request dao. </summary>
        private TableDAO<ProductRequest> mObjProductRequestDAO;

        private SAPObjectDAO<SaleOrder> mobjSaleOrderDAO;

        private SAPObjectDAO<SaleOrderDetail> mobjSaleOrderDetailDAO;

        private SAPObjectDAO<Item> mobjItemDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public SetupService()
        {
            mObjDestinationRequestDAO = new TableDAO<DestinationRequest>();
            mObjParameterRequestDAO = new TableDAO<ParameterRequest>();
            mObjPermissionRequestDAO = new TableDAO<PermissionRequest>();
            mObjPortRequestDAO = new TableDAO<PortRequest>();
            mObjProductRequestDAO = new TableDAO<ProductRequest>();
            mobjSaleOrderDAO = new SAPObjectDAO<SaleOrder>();
            mobjSaleOrderDetailDAO = new SAPObjectDAO<SaleOrderDetail>();
            mobjItemDAO = new SAPObjectDAO<Item>();
        }


        /// <summary> Initializes the tables. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public void InitializeTablesAndFields()
        {
            //Create tables and fields if not exists
            mObjDestinationRequestDAO.Initialize();
            mObjParameterRequestDAO.Initialize();
            mObjPermissionRequestDAO.Initialize();
            mObjPortRequestDAO.Initialize();
            mObjProductRequestDAO.Initialize();

            //Create user fields if not exists in SAP documents
            //mobjSaleOrderDAO.InitializeUserFields();
            //mobjSaleOrderDetailDAO.InitializeUserFields();
            //mobjItemDAO.InitializeUserFields();
        }
    }
}
