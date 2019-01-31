using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;
using UGRS.Core.SDK.DI.Extension;
using UGRS.Core.Extension;

namespace Corrales.DAO
{
    public class ChargesDAO
    {
        QueryManager mObjQueryManager;

        public ChargesDAO()
        {
            mObjQueryManager = new QueryManager();
        }


        public string GetCorralsQuery(string pStrCardCode, string pStrWhsCode)
        {

            Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
            lLstStrParameters.Add("CardCode", pStrCardCode);
            lLstStrParameters.Add("WhsCode", pStrWhsCode);
            //var a = "dadasd {Nombre}".InjectSingleValue("Name", "Raul");

            return this.GetSQL("GetCorrals").Inject(lLstStrParameters);
        }

        public IList<FloorChargesDTO> GetEntriesList(string pStrCardCode, List<string> pStrlistCorrals,string pStrMainWhs)
        {

            Recordset lObjRecordSet = null;
            IList<FloorChargesDTO> lLstObjResult = new List<FloorChargesDTO>();
            try
            {
                DateTime? lObjDtnull = null;

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("CardCode", pStrCardCode);
                lLstStrParameters.Add("MainWhs", pStrMainWhs);

                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetCorrals").Inject(lLstStrParameters);
                string lStrCheck = string.Empty;
                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {
                    for (int i = 0; i < lObjRecordSet.RecordCount; i++)
                    {
                        if (i < pStrlistCorrals.Count)
                        {
                            if (pStrlistCorrals[i] == (string)lObjRecordSet.Fields.Item("WhsCode").Value)
                            {
                                lStrCheck = "Y";
                            }
                        }
                        else
                        {
                            lStrCheck = "N";
                        }



                        lLstObjResult.Add(new FloorChargesDTO()
                        {
                            Checked = lStrCheck,
                            Corral = (string)lObjRecordSet.Fields.Item("whscode").Value,
                            BaseType = (int)lObjRecordSet.Fields.Item("basetype").Value,
                            Direction = (int)lObjRecordSet.Fields.Item("direction").Value,
                            DocDate = (DateTime)lObjRecordSet.Fields.Item("docdate").Value,
                            InvDate = (DateTime)lObjRecordSet.Fields.Item("InvDate").Value,
                            MovType = (string)lObjRecordSet.Fields.Item("movtype").Value,
                            Quantity = Convert.ToInt32(lObjRecordSet.Fields.Item("quantity").Value),
                            DistNumber = (string)lObjRecordSet.Fields.Item("distnumber").Value,
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


        //public IList<FloorChargesDTO> GetCorralMvms(string pStrCardCode, string pStrItemCode, string pStrDocDate)
        //{

        //    Recordset lObjRecordSet = null;
        //    IList<FloorChargesDTO> lLstObjResult = new List<FloorChargesDTO>();
        //    try
        //    {

        //        lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);



        //        string lStrQuery = this.GetSQL("GetCorrals").InjectSingleValue("CardCode", pStrCardCode);
        //        string lStrCheck = string.Empty;
        //        lObjRecordSet.DoQuery(lStrQuery);

        //        if (lObjRecordSet.RecordCount > 0)
        //        {

        //            string x = (string)lObjRecordSet.Fields.Item("WhsCode").Value;
        //            string y = (string)lObjRecordSet.Fields.Item("ItemCode").Value;
        //            int s = Convert.ToInt32(lObjRecordSet.Fields.Item("BaseType").Value);
        //            DateTime ss = (DateTime)lObjRecordSet.Fields.Item("DocDate").Value;
        //            int sde = (int)lObjRecordSet.Fields.Item("Direction").Value;
        //            int ded = (int)lObjRecordSet.Fields.Item("Quantity").Value;

        //            lLstObjResult.Add(new FloorChargesDTO()
        //            {
        //                Corral = (string)lObjRecordSet.Fields.Item("WhsCode").Value,
        //                ItemCode = (string)lObjRecordSet.Fields.Item("ItemCode").Value,
        //                BaseType = (int)lObjRecordSet.Fields.Item("BaseType").Value,
        //                Direction = (int)lObjRecordSet.Fields.Item("Direction").Value,
        //                DocDate = (DateTime)lObjRecordSet.Fields.Item("DocDate").Value,
        //                Quantity = (int)lObjRecordSet.Fields.Item("Quantity").Value,
        //            });
        //            lObjRecordSet.MoveNext();

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


        public string GetLineCorral(string pStrBaseReference)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetLineCorral").InjectSingleValue("BaseRef", pStrBaseReference);

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

        public string GetServerDate()
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

        public double GetPrice(string pStrArticle, string pStrWhsCode)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                lLstStrParameters.Add("Article", pStrArticle);
                lLstStrParameters.Add("WhsCode", pStrWhsCode);


                string lStrQuery = this.GetSQL("GetPrice").Inject(lLstStrParameters);

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

        public string GetWhs(int pIntUserSign)
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

                string lStrQuery = this.GetSQL("GetWhs").InjectSingleValue("UsrId", pIntUserSign);

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


        public string GetArticleToInvoice()
        {
            Recordset lObjRecordSet = null;
            try
            {
                lObjRecordSet = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                string lStrQuery = this.GetSQL("GetArticleToInvoice");

                lObjRecordSet.DoQuery(lStrQuery);

                if (lObjRecordSet.RecordCount > 0)
                {

                    return lObjRecordSet.Fields.Item(0).Value.ToString();
                }
                else
                {
                    return string.Empty;
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
    }
}
