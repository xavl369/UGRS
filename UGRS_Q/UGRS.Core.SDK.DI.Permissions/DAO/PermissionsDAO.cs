using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Exceptions;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.SDK.DI.Permissions.DTO;
using UGRS.Core.Services;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Permissions.DAO
{
    public class PermissionsDAO
    {
        QueryManager mObjQueryManager;

        public PermissionsDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        public bool IsRequestPreparedToCreateSaleOrder(string pStrRequestId)
        {
            return IsPermissionRequestPrepared(pStrRequestId)
                && IsProductRequestPrepared(pStrRequestId)
                && IsDestinationRequestPrepared(pStrRequestId)
                && IsPortRequestPrepared(pStrRequestId)
                && IsParameterRequestPrepared(pStrRequestId) ? true : false;
        }

        public bool ExistsSaleOrder(string pStrRequestId)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            bool lBolResult = false;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);

                lStrQuery = this.GetSQL("CountSaleOrderByRequestId").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    lBolResult = int.Parse(lObjRecordSet.Fields.Item(0).Value.ToString()) > 0;
                }
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lBolResult;
        }

        public int CreateSaleOrder(string pStrRequestId)
        {
            Documents lObjSaleOrder = null;

            try
            {
                lObjSaleOrder = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                lObjSaleOrder = PopulateSaleOrder(lObjSaleOrder, pStrRequestId);

                int lIntResult = lObjSaleOrder.Add();
                if (lIntResult == 0)
                {
                    LogService.WriteSuccess("[SaleOrder CREATED]");
                }
                else
                {
                    LogService.WriteError("CODE ERROR" + lIntResult.ToString());
                }

                return lIntResult;
            }
            catch (Exception lObjException)
            {
                LogService.WriteError(lObjException.Message);
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjSaleOrder);
            }
        }

        public int UpdateSaleOrder(string pStrRequestId)
        {
            Documents lObjSaleOrder = null;

            try
            {
                lObjSaleOrder = GetSaleOrderByRequestId(pStrRequestId);
                LogService.WriteInfo("[SaleOrderToUpdate:"+lObjSaleOrder.DocNum+"]");
                lObjSaleOrder = PopulateSaleOrder(lObjSaleOrder, pStrRequestId);
                LogService.WriteInfo("[SaleOrdeUpdate:" + lObjSaleOrder.DocNum + "]");
                return lObjSaleOrder.Update();
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjSaleOrder);
            }
        }

        public int CancelSaleOrder(string pStrRequestId)
        {
            Documents lObjSaleOrder = null;

            try
            {
                lObjSaleOrder = GetSaleOrderByRequestId(pStrRequestId);
                return lObjSaleOrder.Cancel();
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjSaleOrder);
            }
        }

        public int CloseSaleOrder(string pStrRequestId)
        {
            Documents lObjSaleOrder = null;

            try
            {
                lObjSaleOrder = GetSaleOrderByRequestId(pStrRequestId);
                return lObjSaleOrder.Close();
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjSaleOrder);
            }

        }

        public int GetNextUgrsFolio(string pStrPrefix)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            Dictionary<string, string> lLstStrParameters = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Prefix", pStrPrefix);

                lStrQuery = this.GetSQL("GetNextUgrsFolio").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                return lObjRecordSet.RecordCount > 0 ? (int)lObjRecordSet.Fields.Item("UgrsFolio").Value : 0;
            }
            catch (Exception lObjException)
            {
                throw new QueryException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetRequestIdByPermissionRequestCode(string pStrCode)
        {
            return mObjQueryManager.GetValue("U_RequestId", "Code", pStrCode, "[@UG_PE_WS_PERE]");
        }

        private bool IsPermissionRequestPrepared(string pStrRequestId)
        {
            return mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PERE]") > 0 ? true : false;
        }

        private bool IsProductRequestPrepared(string pStrRequestId)
        {
            return mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PRRE]") > 0 ? true : false;
        }

        private bool IsDestinationRequestPrepared(string pStrRequestId)
        {
            return mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_DERE]") > 0 ? true : false;
        }

        private bool IsPortRequestPrepared(string pStrRequestId)
        {
            string lStrResult = mObjQueryManager.GetValue("U_MobilizationTypeId", "U_RequestId", pStrRequestId, "[@UG_PE_WS_PERE]");
            bool lBoolFlag = false;
            //LogService.WriteInfo("RESULTQUERY" + lStrResult);
            switch (lStrResult)
            {
                case "1":
                    lBoolFlag = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PORE]") == 1 ? true : false;
                    break;
                case "2":
                    lBoolFlag = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PORE]") == 1 ? true : false;
                    break;
                case "3":
                    lBoolFlag = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PORE]") == 2 ? true : false;
                    break;
                case "4":
                    lBoolFlag = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PORE]") == 0 ? true : false;
                    break;
                case "5":
                    lBoolFlag = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PORE]") == 1 ? true : false;
                    break;
            }

            return lBoolFlag;

        }

        private bool IsParameterRequestPrepared(string pStrRequestId)
        {
            string lStrResult = mObjQueryManager.GetValue("U_ParentProductId", "U_RequestId", pStrRequestId, "[@UG_PE_WS_PRRE]");
            bool lBoolReturn = false;
            int lIntResult = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PRRE]");
            // Equino - FaunaSilvestre	
            if(lStrResult == "105" || lStrResult == "118")
            {
                lBoolReturn = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PARE]") == (3 * lIntResult) ? true : false;
            }
            // Bovino - Caprino - Ovino
            if (lStrResult == "1" || lStrResult == "94" || lStrResult == "99")
            {
                lBoolReturn = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PARE]") == (4 * lIntResult) ? true : false;
            }
            // Conejos - Lacteos
            if (lStrResult == "169")
            {
                lBoolReturn = mObjQueryManager.Count("U_RequestId", pStrRequestId, "[@UG_PE_WS_PARE]") == (1 * lIntResult) ? true : false;
            }

            return lBoolReturn;
            
        }

        private int GetSaleOrderKeyByRequestId(string pStrRequestId)
        {
            Recordset lObjRecordset = null;
            int lIntResult = 0;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);

                string lStrQuery = this.GetSQL("GetSaleOrderByRequestId").Inject(lLstStrParameters);
                lObjRecordset.DoQuery(lStrQuery);

                if (lObjRecordset.RecordCount > 0)
                {
                    lIntResult = Convert.ToInt32(lObjRecordset.Fields.Item("DocEntry").Value.ToString());
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordset);
            }

            return lIntResult;
        }

        private Documents GetSaleOrderByRequestId(string pStrRequestId)
        {
            Documents lObjSaleOrder = null;

            lObjSaleOrder = (SAPbobsCOM.Documents)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            int x = GetSaleOrderKeyByRequestId(pStrRequestId);

            lObjSaleOrder.GetByKey(x);

            return lObjSaleOrder;
        }

        private Documents PopulateSaleOrder(Documents pObjSaleOrder, string pStrRequestId)
        {
            PermissionRequestDTO lObjPermissionRequest = GetPermissionRequest(pStrRequestId);
            LogService.WriteInfo("[SaleOrder Begin]");
            //Header
            pObjSaleOrder.CardCode = lObjPermissionRequest.CardCode;
            pObjSaleOrder.DocDate = lObjPermissionRequest.Date;
            pObjSaleOrder.DocDueDate = lObjPermissionRequest.CrossingDate;
            pObjSaleOrder.Series = 147;
         

            //Custom user fields
            pObjSaleOrder.UserFields.Fields.Item("U_PE_IdPermitType").Value = lObjPermissionRequest.MobilizationTypeId.ToString(); //Campo enlazado al catalogo de tipos de movilizaciones
            pObjSaleOrder.UserFields.Fields.Item("U_PE_RequestCodeUGRS").Value = lObjPermissionRequest.UgrsRequest;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_FolioUGRS").Value = lObjPermissionRequest.UgrsFolio;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_ChargeTo").Value = lObjPermissionRequest.Producer;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Requests").Value = lObjPermissionRequest.Producer;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Representative").Value = lObjPermissionRequest.Producer;
            //pObjSaleOrder.UserFields.Fields.Item("U_PE_Asociacion").Value = null; (No se recibe por el WS)
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Phone").Value = lObjPermissionRequest.ProducerTelephone;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Entry").Value = lObjPermissionRequest.Entry;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Departure").Value = lObjPermissionRequest.Departure;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Transport").Value = lObjPermissionRequest.TransportId.ToString(); //Campo enlazado al catalogo de transportes
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Origin").Value = lObjPermissionRequest.Origin;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Customs1").Value = lObjPermissionRequest.Customs1 != 0 ? lObjPermissionRequest.Customs1.ToString() : ""; //Campo enlazado al catalogo de aduanas
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Customs2").Value = lObjPermissionRequest.Customs2 != 0 ? lObjPermissionRequest.Customs2.ToString() : ""; //Campo enlazado al catalogo de aduanas
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Destination").Value = lObjPermissionRequest.Destination;
            pObjSaleOrder.UserFields.Fields.Item("U_PE_Location").Value = lObjPermissionRequest.CustomerLocation;


            pObjSaleOrder = GetLines(pObjSaleOrder, pStrRequestId, lObjPermissionRequest.MobilizationTypeId);

            return pObjSaleOrder;
        }
         

        private Documents GetLines(Documents pObjSaleOrder, string pStrRequestId, int pIntMobilizationTypeId)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrInsurenceItemCode = "";
            string lStrQuery = "";
            string lStrResult = mObjQueryManager.GetValue("U_ParentProductId", "U_RequestId", pStrRequestId, "[@UG_PE_WS_PRRE]");
            LogService.WriteInfo("[SaleOrder: TipoGanado" + lStrResult + "]");
            if (lStrResult == "105" && pIntMobilizationTypeId == 2)
            {
                lStrInsurenceItemCode = mObjQueryManager.GetValue("U_Value","Name","PE_SEG_EQUINO","[@UG_CONFIG]"); ;
            }
            if (lStrResult == "1" && pIntMobilizationTypeId == 2)
            {
                lStrInsurenceItemCode = mObjQueryManager.GetValue("U_Value", "Name", "PE_SEG_BOVINO", "[@UG_CONFIG]"); ;
            }
            LogService.WriteInfo("[SaleOrder: ItemCode" + lStrInsurenceItemCode.ToString() + "]");
            double lDouInsurencePrice = GetInsurencePrice(lStrInsurenceItemCode.ToString()) ;
            LogService.WriteInfo("[SaleOrder: Precio" + lDouInsurencePrice + "]");

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);
                lLstStrParameters.Add("MobilizationTypeId", pIntMobilizationTypeId.ToString());
                lStrQuery = this.GetSQL("GetItemAndPriceByProductRequests").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        pObjSaleOrder.Lines.SetCurrentLine(i);
                        pObjSaleOrder.Lines.WarehouseCode = "OFGE";
                        pObjSaleOrder.Lines.ItemCode = lObjRecordSet.Fields.Item("U_ItemCode").Value.ToString();
                        pObjSaleOrder.Lines.Quantity = Convert.ToDouble(lObjRecordSet.Fields.Item("U_Quantity").Value.ToString());
                        pObjSaleOrder.Lines.Price = Convert.ToDouble(lObjRecordSet.Fields.Item("Price").Value.ToString());
                        pObjSaleOrder.Lines.CostingCode = "OG_PERMI";
                        //pObjSaleOrder.Update();
                        pObjSaleOrder.Lines.Add();

                        if (pIntMobilizationTypeId == 2)
                        {
                            pObjSaleOrder.Lines.WarehouseCode = "OFGE";
                            pObjSaleOrder.Lines.ItemCode = lStrInsurenceItemCode;
                            pObjSaleOrder.Lines.Quantity = Convert.ToDouble(lObjRecordSet.Fields.Item("U_Quantity").Value.ToString());
                            pObjSaleOrder.Lines.Price = lDouInsurencePrice;
                            pObjSaleOrder.Lines.CostingCode = "OG_PERMI";
  

                            pObjSaleOrder.Lines.Add();
                        }

                        lObjRecordSet.MoveNext();
                    }
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return pObjSaleOrder;
        }

        public PermissionRequestDTO GetPermissionRequest(string pStrRequestId)
        {
            PermissionRequestDTO lObjResult = new PermissionRequestDTO();
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);

                lStrQuery = this.GetSQL("GetPermissionRequestById").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    foreach (PropertyInfo lObjProperty in lObjResult.GetType().GetProperties().Where(x => x.GetMethod.IsPublic && !x.GetMethod.IsVirtual))
                    {
                        try
                        {
                            lObjProperty.SetValue
                            (
                                lObjResult,
                                Convert.ChangeType
                                (
                                    lObjRecordSet.Fields.Item(lObjProperty.Name).Value.ToString(),
                                    lObjProperty.PropertyType
                                )
                            );
                        }
                        catch
                        {
                            //Ignore ;)
                        }
                    }
                }
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lObjResult;
        }

        /// <summary>
        /// GetRowCode Method
        /// Get RowCode by RequestId
        /// </summary>
        /// <param name="pStrTable"></param>
        /// <param name="pStrRequestId"></param>
        /// <returns></returns>
        public  string GetRowCode(string pStrTable ,string pStrRequestId )
        {
            string lStrResult = mObjQueryManager.GetValue("Code", "U_RequestId", pStrRequestId, pStrTable);

            return lStrResult;
        }

        public string GetRowCodeByProduct(string pStrTable, string pStrRequestId, int pIntProductId)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrResult="";
            string lStrQuery;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);
                lLstStrParameters.Add("ProductId", pIntProductId.ToString());

                lStrQuery = this.GetSQL("GetRowCodeByProduct").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                lStrResult = lObjRecordSet.Fields.Item(0).Value.ToString();
            }
            catch
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            finally
            {

            }
           

            return lStrResult;
        }

        public string IsPortExist(string pStrRequestId , int pIntPortId, int pIntPortType)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrResult = "";
            string lStrQuery;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);
                lLstStrParameters.Add("PortId", pIntPortId.ToString());
                lLstStrParameters.Add("PortType", pIntPortType.ToString());

                lStrQuery = this.GetSQL("CountPortRequestByPortId").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                lStrResult = lObjRecordSet.Fields.Item(0).Value.ToString();
            }
            catch
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            finally
            {

            }


            return lStrResult;
        }

        public string GetRowCodeByPort(string pStrRequestId, int pIntPortId, int pIntPortType)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            string lStrResult = "";
            string lStrQuery;
            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("RequestId", pStrRequestId);
                lLstStrParameters.Add("PortId", pIntPortId.ToString());
                lLstStrParameters.Add("PortType", pIntPortType.ToString());

                lStrQuery = this.GetSQL("GetRowCodeByPort").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                lStrResult = lObjRecordSet.Fields.Item(0).Value.ToString();
            }
            catch
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            finally
            {

            }


            return lStrResult;
        }

        public int GetUgrsFolio(string pStrRequestId)
        {
            int lStrResult = int.Parse(mObjQueryManager.GetValue("U_UgrsFolio", "U_RequestId", pStrRequestId, "[@UG_PE_WS_PERE]"));

            return lStrResult;
        }

        public double GetInsurencePrice(string pStrItemCode)
        {
            SAPbobsCOM.Recordset lObjRecordSet = null;
            double lDouResult = 0;
            string lStrQuery = "";

            try
            {
                lObjRecordSet = (SAPbobsCOM.Recordset)DIApplication.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("ItemCode", pStrItemCode);
                lStrQuery = this.GetSQL("GetInsurencePrice").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                lDouResult = double.Parse(lObjRecordSet.Fields.Item(0).Value.ToString());
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lDouResult;
        }
    }
}
