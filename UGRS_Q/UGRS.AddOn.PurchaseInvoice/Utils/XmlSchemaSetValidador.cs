using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using UGRS.AddOn.PurchaseInvoice.Interfaz;

namespace UGRS.AddOn.PurchaseInvoice.Utils
{
    class XmlSchemaSetValidador : IXmlSchemaSetValidator
    {
        private string ErrorPrincipal = "";
        private string Error = "";
        private string Advertencias = "";
        private Int32 TotErrores = 0, TotAdvertencias = 0;

        public bool CompruebaXMLvsXSD(string targetNamespace, string UriXSD, string UriXML)
        {
            try
            {


                XmlReaderSettings booksSettings = new XmlReaderSettings();
                if (UriXSD == "cfdv33.xsd")
                {
                    booksSettings.Schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/catalogos", "catCFDI.xsd");
                    //booksSettings.Schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/tipoDatos/tdCFDI", "tdCFDI.xsd");
                }
                booksSettings.Schemas.Add(targetNamespace, UriXSD);
                booksSettings.ValidationType = ValidationType.Schema;
                booksSettings.ValidationEventHandler += new ValidationEventHandler(booksSettingsValidationEventHandler);
                StreamReader reader = new StreamReader(UriXML, System.Text.Encoding.UTF8);
                XmlReader Lec = XmlReader.Create(reader, booksSettings);
                while (Lec.Read()) { }

                int lIntErrores = TotalErrores();
                int lIntAdvertencias = TotalAdvertencias();
                
                if (lIntErrores < 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorPrincipal = "Error en CompruebaXMLvsXSD: " + ex.Message;
                return false;
            }
        }

        private void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            long Linea = e.Exception.LineNumber;
            long Posicion = e.Exception.LinePosition;
            if (e.Severity == XmlSeverityType.Warning)
            {
                Advertencias += "Advertencia: " + e.Message + "--> Linea : " + Linea + ", Posición : " + Posicion + "\n";
                TotAdvertencias += 1;
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                if (!e.Message.Contains("'http://www.sat.gob.mx/TimbreFiscalDigital:TimbreFiscalDigital'") 
                    && !e.Message.Contains("'http://www.buzonfiscal.com/ns/addenda/bf/2:AddendaBuzonFiscal'")
                    && !e.Message.Contains("'http://www.sat.gob.mx/terceros:PorCuentadeTerceros'")
                    && !e.Message.Contains("'http://www.sat.gob.mx/implocal:ImpuestosLocales'"))
                {
                    Error += "Error: " + e.Message + "--> Linea : " + Linea + ", Posición : " + Posicion + "\n";
                    TotErrores += 1;
                }
            }
        }

        public Int32 TotalErrores() { return TotErrores; }
        public Int32 TotalAdvertencias() { return TotAdvertencias; }
        public string GetErrores() { return Error; }
        public string GetAdvertencias() { return Advertencias; }
        public string GetErrorPrincipal() { return ErrorPrincipal; }

    }
}
