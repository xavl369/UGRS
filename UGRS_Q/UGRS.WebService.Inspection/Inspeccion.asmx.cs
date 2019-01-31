// file:	Inspeccion.asmx.cs
// summary:	Implements the inspeccion.asmx class

using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Xml;
using UGRS.Core.SDK.DI.Auctions;
using UGRS.WebService.Inspection;

namespace UGRS.WebService.Inspection
{
    /// <summary>
    /// Web service for
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "http://microsoft.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class Inspeccion : System.Web.Services.WebService
    {
        /// <summary> The object factory services. </summary>
        private AuctionsServicesFactory mObjAuctionsServices = new AuctionsServicesFactory();

        [WebMethod]
        public XmlDataDocument Buscar(string comprador, string vendedor, string rfc_vendedor, string fecha)
        {
            XmlDataDocument lObjXmlDocument = null;

            try
            {
                //DataTable lDtbSearch = mObjAuctionsServices.GetAuctionBatchService().GetBatchesByFilters(comprador, vendedor, rfc_vendedor, fecha);
                DataTable lDtbSearch = mObjAuctionsServices.GetAuctionBatchService().GetBatchesByFilters(comprador, vendedor, rfc_vendedor, fecha);
                lDtbSearch.TableName = "Resultado";

                StringBuilder lObjStringBuilder = new StringBuilder();
                StringWriter lObjStringWriter = new StringWriter(lObjStringBuilder);

                lObjXmlDocument = new XmlDataDocument();
                lDtbSearch.WriteXml(lObjStringWriter);
                lObjXmlDocument.LoadXml(lObjStringWriter.ToString());
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException);
            }

            return lObjXmlDocument;
        }
    }
}
