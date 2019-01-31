using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.Exceptions;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.FoodProduction.Tables;
using UGRS.Core.Utility;
using UGRS.Core.Extension;

namespace UGRS.Core.SDK.DI.FoodProduction.Services
{
    public class TicketDetailService
    {
        private TableDAO<TicketDetail> mObjTicketDetailDAO;

        public TicketDetailService()
        {
            mObjTicketDetailDAO = new TableDAO<TicketDetail>();
        }

        public int Add(TicketDetail pObjTicketDetail)
        {
            return mObjTicketDetailDAO.Add(pObjTicketDetail);
        }

        public int Update(TicketDetail pObjTicketDetail)
        {
            return mObjTicketDetailDAO.Update(pObjTicketDetail);
        }

        public int Remove(TicketDetail pObjTicketDetail)
        {
            return mObjTicketDetailDAO.Remove(pObjTicketDetail.RowCode);
        }

        ///<summary>    Gets configuration code. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<param name="pStrConfigName">    Name of the string configuration. </param>
        ///<returns>    The configuration code. </returns>

        public string GetTicketCode(string pStrField, string pStrConfigName, int pIntLine)
        {
            string lObjCode = "";
            Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //lLstStrParameters.Add("Field", pStrField);
                lLstStrParameters.Add("U_folio", pStrConfigName);
                lLstStrParameters.Add("U_Line", pIntLine.ToString());

                lObjRecordset.DoQuery("SELECT Code FROM [@UG_PL_TCKD] WHERE U_Folio like '{U_folio}' and U_Line like '{U_Line}'".Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    return lObjRecordset.Fields.Item("Code").Value.ToString();
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
            return lObjCode;
        }
    }
}
