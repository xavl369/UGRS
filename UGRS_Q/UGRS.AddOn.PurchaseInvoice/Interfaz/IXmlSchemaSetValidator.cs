using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.PurchaseInvoice.Interfaz
{
    public interface IXmlSchemaSetValidator
    {
        bool CompruebaXMLvsXSD(string pStrTargetNamespace, string pStrUriXSD, string pStrUriXML);
        Int32 TotalErrores();
        Int32 TotalAdvertencias();
        string GetErrores();
        string GetAdvertencias();
        string GetErrorPrincipal();
    }
}
