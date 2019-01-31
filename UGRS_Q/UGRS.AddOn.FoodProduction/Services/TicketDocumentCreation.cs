using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.SDK.UI;
using UGRS.Core.Utility;
using UGRS.Core.Extension;
using UGRS.Core.Services;

namespace UGRS.AddOn.FoodProduction.Services
{
   public class TicketDocumentCreation
    {
        QueryManager mObjQueryManager = new QueryManager();
        TicketDAO mObjTicketDAO = new TicketDAO();
        TicketServices mObjTicketServices = new TicketServices();


        public bool CrearDocumento(List<Ticket> pLstTicket, SAPbobsCOM.BoObjectTypes pObjType, string pStrTableBase, int pIntBaseType, string pStrTableDetail)
        {
            bool lBolIsSuccess = false;
            try
            {
                string lStrDocEntry = string.Empty;
                string lStrCostCenter = GetCostCenter();
                List<TicketDetail> lLstTicketDetail = new List<TicketDetail>();
                SAPbobsCOM.Documents lObjDocument = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(pObjType); //SAPbobsCOM.BoObjectTypes.oInvoices);
                foreach (Ticket lObjTicket in pLstTicket)
                {
                    if (pStrTableBase == "ORDR")
                    {
                        lObjDocument.DocObjectCodeEx = "13";
                    }

                    lStrDocEntry = mObjQueryManager.GetValue("DocEntry", "DocNum", lObjTicket.Number.ToString(), pStrTableBase);
                    lObjDocument.CardCode = lObjTicket.BPCode;

                    if (lObjTicket.CapType == 0)
                    {
                        lObjDocument.DocObjectCode = BoObjectTypes.oInvoices;
                    }
                    lLstTicketDetail = mObjTicketDAO.GetListTicketDetail(lObjTicket.Folio) as List<TicketDetail>;
                    if (pStrTableBase == "OINV")
                    {
                        // lLstTicketDetail = AdjustmentTicket(pLstTicket, true, false);
                    }
                    if (pStrTableBase == "OPCH")
                    {
                        //lLstTicketDetail = AdjustmentTicket(pLstTicket, false, false);
                    }

                    for (int i = 0; i < lLstTicketDetail.Count; i++)
                    {
                        if (lObjTicket.Number != 0 && VerifyDocItem(lStrDocEntry, lLstTicketDetail[i].Item, pStrTableDetail))
                        {
                            lObjDocument.Lines.BaseEntry = int.Parse(lStrDocEntry);
                            lObjDocument.Lines.BaseLine = lLstTicketDetail[i].BaseLine;
                            lObjDocument.Lines.BaseType = pIntBaseType;
                        }
                        // lObjDocument.Lines.AccountCode = "2180010000000";
                        if (lLstTicketDetail[i].netWeight < 0)
                        {
                            lLstTicketDetail[i].netWeight *= -1;
                        }

                        lObjDocument.Lines.ItemCode = lLstTicketDetail[i].Item;

                        lObjDocument.Lines.UnitsOfMeasurment = 0;
                        lObjDocument.Lines.UnitPrice = lLstTicketDetail[i].Price;
                        lObjDocument.Lines.COGSCostingCode = lStrCostCenter;

                        if (lObjTicket.CapType == 4)
                        {
                            lObjDocument.Lines.Quantity = 1;
                        }
                        else
                        {
                            lObjDocument.Lines.Quantity = lLstTicketDetail[i].netWeight;
                        }
                        lObjDocument.Lines.WarehouseCode = lLstTicketDetail[i].WhsCode;
                        // lObjDocument.Lines.ProjectCode = lObjTicket.Project;
                        lObjDocument.Lines.UserFields.Fields.Item("U_GLO_BagsBales").Value = lLstTicketDetail[i].BagsBales;
                        lObjDocument.Lines.UserFields.Fields.Item("U_PL_Ticket").Value = lLstTicketDetail[i].Folio;
                        lObjDocument.Lines.Add();
                    }
                }
                if (lObjDocument.Add() != 0)
                {
                    UIApplication.ShowMessageBox(string.Format("Exception: {0}", DIApplication.Company.GetLastErrorDescription()));
                    LogService.WriteError("[ERROR]" + DIApplication.Company.GetLastErrorDescription());
                }
                else
                {
                    lBolIsSuccess = true;
                    LogService.WriteSuccess("[CrearDocumento] DocNum:" + lObjDocument.DocNum);
                    MemoryUtility.ReleaseComObject(lObjDocument);
                    UIApplication.ShowMessageBox(string.Format("Documento realizado correctamente"));

                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Exception: {0}", ex.Message));
                LogService.WriteError("[CrearDocumento]" + ex.Message);
                LogService.WriteError(ex);
            }
            return lBolIsSuccess;
        }

        private string GetCostCenter()
        {
            string lStrCostCenter = "";
            try
            {
                lStrCostCenter = mObjQueryManager.GetValue("U_GLO_CostCenter", "UserID", DIApplication.Company.UserSignature.ToString(), "OUSR");
            }
            catch (Exception lObjException)
            {
                LogService.WriteError("[GetCostCenter]" + lObjException.Message);
                LogService.WriteError(lObjException);
                UIApplication.ShowError(string.Format("CostCenter: {0}", lObjException.Message));
            }
            return lStrCostCenter;
        }

        private bool VerifyDocItem(string pStrCode, string pStrItem, string pStrTable)
        {
            bool lBolVerify = false;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "select DocEntry from {Table} where ItemCode = '{ItemCode}' and DocEntry = '{DocEntry}'";

            SAPbobsCOM.Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                List<string> lStrResult = new List<string>();
                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItem);
                lLstStrParameters.Add("DocEntry", pStrCode);
                lLstStrParameters.Add("Table", pStrTable);
                lStrQuery = lStrQuery.Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);
                if (lObjRecordSet.RecordCount > 0)
                {
                    lBolVerify = true;
                    //lStrResult.Add(lObjRecordSet.Fields.Item(0).Value.ToString());
                }
            }
            catch (Exception ex)
            {
                UIApplication.ShowMessageBox(string.Format("Error de consulta", ex.Message));
                LogService.WriteError("[VerifyDocItem]: " + ex.Message);
                LogService.WriteError(ex);

            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lBolVerify;
            // return lStrResult;
        }


    }
}
