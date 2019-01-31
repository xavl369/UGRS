using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;
using UGRS.AddOn.Cuarentenarias.Models;
using SAPbobsCOM;
using UGRS.Core.SDK.DI;
using UGRS.Core.Exceptions;
using UGRS.Core.Utility;

namespace UGRS.AddOn.Cuarentenarias.DAO
{
    public class InspectionDAO
    {
        QueryManager mObjQueryManager;

        public InspectionDAO()
        {
            mObjQueryManager = new QueryManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStrExpDate"></param>
        /// <param name="pIntUserSign"></param>
        /// <returns></returns>
        public string GetInspectionList(string pStrExpDate, long pIntUserSign)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("DateInsp", pStrExpDate);
            lLstStrParameters.Add("Series", pIntUserSign.ToString());
            //var a = "dadasd {Nombre}".InjectSingleValue("Name", "Raul");

            return this.GetSQL("GetInspectionList").Inject(lLstStrParameters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStrExpDate"></param>
        /// <param name="pIntUserSign"></param>
        /// <returns></returns>
        public string GetBatchLines(string pStrCardCode, string pStrWhsCode)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("CardCode", pStrCardCode);
            lLstStrParameters.Add("WhsCode", pStrWhsCode);

            return this.GetSQL("GetBatchLines").Inject(lLstStrParameters);
        }


        public string GetBatchLine(string pStrCardCode, string pStrWhsCode)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("CardCode", pStrCardCode);
            lLstStrParameters.Add("WhsCode", pStrWhsCode);

            string x = this.GetSQL("GetBatchWGoodIssues").Inject(lLstStrParameters);


            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetBatchWGoodIssues").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(1).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pStrExpDate"></param>
        /// <param name="pIntUserSign"></param>
        /// <returns></returns>
        public string GetWhsCodeGR(int pIntUserId)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("UserId", pIntUserId.ToString());

            return this.GetSQL("GetWhsCodeGR").Inject(lLstStrParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIntCertificate"></param>
        /// <returns></returns>
        public string GetCertificateRequest(int pIntCertificate)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("Certificate", pIntCertificate.ToString());

            string d = this.GetSQL("GetCertificateRequest").Inject(lLstStrParameters);

            return this.GetSQL("GetCertificateRequest").Inject(lLstStrParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pIdInspection"></param>
        /// <returns></returns>
        public string GetInspectionByID(int pIdInspection)
        {
            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("ID", pIdInspection.ToString());

            return this.GetSQL("GetInspectionDetailsList").Inject(lLstStrParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMovTypeList()
        {

            return this.GetSQL("GetMovTypeList");
        }

        public IList<CertificateDTO> ValidateCertificate(int pIntNCertificate/*string pStrType, string pStrCardCode*/)
        {
            Recordset lObjRecordSet = null;
            IList<CertificateDTO> lLstObjResult = new List<CertificateDTO>();
            
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("NCertificate", pIntNCertificate.ToString());
                //lLstStrParameters.Add("ItemCode", pStrType);
                //lLstStrParameters.Add("CardCode", pStrCardCode);


                string lStrQuery = this.GetSQL("ValidateCertificate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                            lLstObjResult.Add(new CertificateDTO()
                            {
                                UsedDate = lObjRecordSet.Fields.Item(0).Value.ToString(),
                                NCertificate = Convert.ToInt32(lObjRecordSet.Fields.Item(1).Value.ToString()),
                                CardCode = lObjRecordSet.Fields.Item(2).Value.ToString(),
                                Quantity = Convert.ToInt32(lObjRecordSet.Fields.Item(3).Value.ToString()),
                                TypeG = lObjRecordSet.Fields.Item(5).Value.ToString(),
                                Status = lObjRecordSet.Fields.Item(6).Value.ToString(),
                                //QuantityUsed = Convert.ToInt32(lObjRecordSet.Fields.Item(4).Value.ToString())
                            });
                            lObjRecordSet.MoveNext();
                     
                    }
                }
            }
            catch (Exception lObjException)
            {

                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }

        public IList<CertificateDTO> SingleValidateCertificate(int pIntNCertificate)
        {
            Recordset lObjRecordSet = null;
            IList<CertificateDTO> lLstObjResult = new List<CertificateDTO>();

            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("NCertificate", pIntNCertificate.ToString());

                string lStrQuery = this.GetSQL("SingleValidateCertificate").Inject(lLstStrParameters);
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        lLstObjResult.Add(new CertificateDTO()
                        {
                            UsedDate = lObjRecordSet.Fields.Item(0).Value.ToString(),
                            NCertificate = Convert.ToInt32(lObjRecordSet.Fields.Item(1).Value.ToString()),
                            CardCode = lObjRecordSet.Fields.Item(2).Value.ToString(),
                            Quantity = Convert.ToInt32(lObjRecordSet.Fields.Item(3).Value.ToString()),
                            TypeG = lObjRecordSet.Fields.Item(4).Value.ToString(),
                            
                        });
                        lObjRecordSet.MoveNext();

                    }
                }
            }
            catch (Exception lObjException)
            {

                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
            return lLstObjResult;
        }


        internal string GetServerTime()
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);


                string lStrQuery = this.GetSQL("GetTimeFromServer");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }
        internal int GetCertRowCode(string pStrIdInsp)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCRowCode").InjectSingleValue("IdInsp", pStrIdInsp);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (int)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        //public IList<CertificateDTO> CertificateList(int pIdInsp)
        //{
        //    Recordset lObjRecordSet = null;
        //    IList<CertificateDTO> lLstObjResult = new List<CertificateDTO>();

        //    try
        //    {

        //        lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

        //        string lStrQuery = this.GetSQL("GetCertRowCode").InjectSingleValue("IdInsp", pIdInsp);
        //        lObjRecordSet.DoQuery(lStrQuery);

        //        if (lObjRecordSet.RecordCount > 0)
        //        {
        //            for (int i = 0; i < lObjRecordSet.RecordCount; i++)
        //            {
        //                int x = (int)lObjRecordSet.Fields.Item(0).Value;
        //                string h = lObjRecordSet.Fields.Item(1).Value.ToString();
        //                int cs = (int)lObjRecordSet.Fields.Item(2).Value;
        //                int ces = Convert.ToInt32(lObjRecordSet.Fields.Item(3).Value);
        //                int asce = (int)lObjRecordSet.Fields.Item(4).Value;


        //                lLstObjResult.Add(new CertificateDTO()
        //                {
        //                    RowCode = (int)lObjRecordSet.Fields.Item(0).Value,
        //                    Name = lObjRecordSet.Fields.Item(1).Value.ToString(),
        //                    IdInspection = (int)lObjRecordSet.Fields.Item(2).Value,
        //                    Quantity = Convert.ToInt32(lObjRecordSet.Fields.Item(3).Value.ToString()),
        //                    Serie = (int)lObjRecordSet.Fields.Item(4).Value,

        //                });
        //                lObjRecordSet.MoveNext();

        //            }
        //        }
        //    }
        //    catch (Exception lObjException)
        //    {

        //        throw new DAOException(lObjException.Message, lObjException);
        //    }
        //    finally
        //    {
        //        MemoryUtility.ReleaseComObject(lObjRecordSet);
        //    }
        //    return lLstObjResult;
        //}


        internal string GetDraftOrInvoice(string pStrIdInsp)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetDrftInvoiceKeys").InjectSingleValue("IdInsp", pStrIdInsp);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        internal double GetQuantityRejected(string pStrBatchNumber)
        {
            Recordset lObjRecordSet = null;
            try
            {

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetQuantityInRejected").InjectSingleValue("DistNumber", pStrBatchNumber);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (double)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        internal string GetServerDate()
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);


                string lStrQuery = this.GetSQL("GetServerDate");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public string GetMainWhs(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetMainWhs").InjectSingleValue("UsrId", pIntUserSign);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(0).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

        }

        public string GetBatchToCancel(string pStrExpDate, string pStrCardCode, string pStrMainWhs, string pStrIdInsp)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("ExpDate", pStrExpDate);
            lLstStrParameters.Add("CardCode", pStrCardCode);
            lLstStrParameters.Add("MainWhs", pStrMainWhs);
            lLstStrParameters.Add("IdInsp", pStrIdInsp);


            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetBatchToCancel").Inject(lLstStrParameters);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    return (string)lObjRecordSet.Fields.Item(2).Value;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }
        }

        public Tables.Certificate GetCertificate(long pLonCertificateNumber)
        {
            Recordset lObjRecordSet = null;

            Tables.Certificate lObjCertificate = new Tables.Certificate();
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCertificate").InjectSingleValue("CertificateNumber", pLonCertificateNumber);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    lObjCertificate.RowCode = lObjRecordSet.Fields.Item(0).Value.ToString();
                    lObjCertificate.RowName = lObjRecordSet.Fields.Item(1).Value.ToString();
                    lObjCertificate.IDInsp = Convert.ToInt64(lObjRecordSet.Fields.Item(2).Value);
                    lObjCertificate.ItemCode = lObjRecordSet.Fields.Item(3).Value.ToString();
                    lObjCertificate.Quantity = Convert.ToInt64(lObjRecordSet.Fields.Item(4).Value);
                    lObjCertificate.Serie = Convert.ToInt64(lObjRecordSet.Fields.Item(5).Value);
                    lObjCertificate.UsedDate = DateTime.Parse(lObjRecordSet.Fields.Item(6).Value.ToString());
                }
                else 
                {
                    lObjCertificate = null;
                }
    
            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lObjCertificate;
        }

        public Tables.SICertificates GetCertificate2(long pCert)
        {
            Recordset lObjRecordSet = null;

            Tables.SICertificates lObjCertificate = new Tables.SICertificates();
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetSICertificate").InjectSingleValue("NCert", pCert);

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    lObjCertificate.RowCode = lObjRecordSet.Fields.Item(0).Value.ToString();
                    lObjCertificate.RowName = lObjRecordSet.Fields.Item(1).Value.ToString();
                    lObjCertificate.Quantity = Convert.ToInt64(lObjRecordSet.Fields.Item(2).Value);
                    lObjCertificate.ItemCode = lObjRecordSet.Fields.Item(3).Value.ToString();

                }

            }
            catch (Exception lObjException)
            {
                throw new DAOException(lObjException.Message, lObjException);
            }
            finally
            {
                MemoryUtility.ReleaseComObject(lObjRecordSet);
            }

            return lObjCertificate;
        }
    }

}
