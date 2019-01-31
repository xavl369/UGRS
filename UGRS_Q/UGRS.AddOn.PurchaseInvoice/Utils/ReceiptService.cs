using ContabilidadElectronicaAddOn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UGRS.AddOn.PurchaseInvoice.RecepcionCfdiService;
using UGRS.AddOn.PurchaseInvoice.RecepcionService;
using UGRS.Core.SDK.DI;

namespace UGRS.AddOn.PurchaseInvoice.Utils
{

    public class ReceiptService
    {
        public static RecepcionCfdiClient mObjRevisionCFDI;
        public static RecepcionClient mObjRevision32;


        public static bool CheckCFDI33(string pStrUuid)
        {
            mObjRevisionCFDI = new RecepcionCfdiClient();
            string lStrContrato = ConfigurationManager.ObtenerConfiguracion("ContratoProd");
            string lStrUsuario = ConfigurationManager.ObtenerConfiguracion("UsuarioProd");
            string lStrPass = ConfigurationManager.ObtenerConfiguracion("PasswProd");
            var lStrRespuesta = mObjRevisionCFDI.cfdiPorUUIDv33(lStrContrato, lStrUsuario, lStrPass, pStrUuid);
            return true;
        }

        public static bool CheckXML33(string pStrXml)
        {
            bool lBolSuccess = false;
            try
            {
                mObjRevisionCFDI = new RecepcionCfdiClient();

                string[] lObjConfigVal = GetConfigValues();
                string lStrContrato = lObjConfigVal[0]; //ConfigurationManager.ObtenerConfiguracion("ContratoProd");
                string lStrUsuario = lObjConfigVal[1]; //ConfigurationManager.ObtenerConfiguracion("UsuarioProd");
                string lStrPass = lObjConfigVal[2]; //ConfigurationManager.ObtenerConfiguracion("PasswProd");
                var lStrRespuesta = mObjRevisionCFDI.verificacionPorXMLv33(lStrContrato, lStrUsuario, lStrPass, pStrXml);
                XmlDocument lObjXmlDoc = new XmlDocument();
                lObjXmlDoc.Load(lStrRespuesta);
                string lStrConsultaOK = string.Empty;
                string lStrCodigo = string.Empty;
                string lStrCodigoEstatus = string.Empty;
                string lStrEstado = string.Empty;
                foreach (var lObjX in lObjXmlDoc.ChildNodes)
                {
                    Type lObjType = lObjX.GetType();

                    if (lObjType.Equals(typeof(XmlElement)))
                    {
                        XmlElement lObjNode = (XmlElement)lObjX;
                        foreach (XmlElement item in lObjNode.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "consultaOk":
                                    lStrConsultaOK = item.InnerText;
                                    break;
                                case "codigo":
                                    lStrCodigo = item.InnerText;
                                    break;
                                case "codigoEstatus":
                                    lStrCodigoEstatus = item.InnerText;
                                    break;
                                case "estado":
                                    lStrEstado = item.InnerText;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (lStrCodigoEstatus == "S - Comprobante obtenido satisfactoriamente." || lStrCodigo == "91")
                        {
                            lBolSuccess = true;
                        }
                        else
                            lBolSuccess = false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }


        public static bool CheckXML32(string pStrxml)
        {
            bool lBolSuccess = false;
            try
            {
                mObjRevision32 = new RecepcionClient();

                string[] lObjConfigVal = GetConfigValues();
                string lStrContrato = lObjConfigVal[0]; //ConfigurationManager.ObtenerConfiguracion("ContratoProd");
                string lStrUsuario = lObjConfigVal[1]; //ConfigurationManager.ObtenerConfiguracion("UsuarioProd");
                string lStrPass = lObjConfigVal[2]; //ConfigurationManager.ObtenerConfiguracion("PasswProd");
                var lStrRespuesta = mObjRevision32.verificacionPorXML(lStrContrato, lStrUsuario, lStrPass, pStrxml);
                XmlDocument lObjXmlDoc = new XmlDocument();
                lObjXmlDoc.LoadXml(lStrRespuesta);
                string lStrConsultaOK = string.Empty;
                string lStrCodigo = string.Empty;
                string lStrCodigoEstatus = string.Empty;
                string lStrEstado = string.Empty;
                
                foreach (var lObjX in lObjXmlDoc.ChildNodes)
                {
                    Type lObjType = lObjX.GetType();

                    if (lObjType.Equals(typeof(XmlElement)))
                    {
                        XmlElement lObjNode = (XmlElement)lObjX;
                        foreach (XmlElement item in lObjNode.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "consultaOk":
                                    lStrConsultaOK = item.InnerText;
                                    break;
                                case "codigo":
                                    lStrCodigo = item.InnerText;
                                    break;
                                case "codigoEstatus":
                                    lStrCodigoEstatus = item.InnerText;
                                    break;
                                case "estado":
                                    lStrEstado = item.InnerText;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (lStrCodigoEstatus == "S - Comprobante obtenido satisfactoriamente." || lStrCodigo == "91")
                        {
                            lBolSuccess = true;
                        }
                        else
                            lBolSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lBolSuccess;
        }

        private static string[] GetConfigValues()
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string[] lArrConfig;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                lObjRecordSet.DoQuery("SELECT STUFF((SELECT ', ' + U_Value FROM [@UG_CONFIG] where Name='CO_PROD_CONTRACT' or Name='CP_PROD_USER' or Name='CO_PROD_PASSW' FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') as Value"); //SELECT CO_DIFFACT FROM [@UG_CONFIG]
                if (lObjRecordSet.RecordCount == 1)
                {

                    lArrConfig = lObjRecordSet.Fields.Item(0).Value.ToString().Split(',');
                    if (lArrConfig.Length != 3)
                        throw new Exception("No se encontro la configuracion para hacer la validacion con SAT");
                }
                else
                    throw new Exception("No se encontro la configuracion para hacer la validacion con SAT");

                return lArrConfig;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Marshal.ReleaseComObject(lObjRecordSet);
            }
        }
    }
}
