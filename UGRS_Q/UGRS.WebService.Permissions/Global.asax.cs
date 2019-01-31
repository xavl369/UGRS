// file:	Global.asax.cs
// summary:	Implements the global.asax class

using System;
using UGRS.Core.SDK.DI.Auctions;
using UGRS.Core.SDK.DI.Permissions;
using UGRS.Core.Utility;

namespace UGRS.WebService.Permissions
{
    /// <summary> A global. </summary>
    /// <remarks> Ranaya, 24/05/2017. </remarks>

    public class Global : System.Web.HttpApplication
    {
        #region Methods

        /// <summary> Event handler. Called by Application for start events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Application_Start(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Application start");
            PermissionsServicesFactory lObjPermissionService = null;

            try
            {
                lObjPermissionService = new PermissionsServicesFactory();
                lObjPermissionService.GetSetupService().InitializeTablesAndFields();
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }
        }

        /// <summary> Event handler. Called by Session for start events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Session_Start(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Session start");
        }

        /// <summary> Event handler. Called by Application for begin request events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Application_BeginRequest(object pObjSender, EventArgs pObjEventsArgs)
     {
            LogService.WriteInfo("Application begin request");
        }

        /// <summary> Event handler. Called by Application for authenticate request events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Application_AuthenticateRequest(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Application authenticate request");
        }

        /// <summary> Event handler. Called by Application for error events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Application_Error(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Application error");
        }

        /// <summary> Event handler. Called by Session for end events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Session_End(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Session end");
        }

        /// <summary> Event handler. Called by Application for end events. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>
        /// <param name="pObjSender">     The object sender. </param>
        /// <param name="pObjEventsArgs"> Event information. </param>

        protected void Application_End(object pObjSender, EventArgs pObjEventsArgs)
        {
            LogService.WriteInfo("Application end");
        }

        #endregion
    }
}