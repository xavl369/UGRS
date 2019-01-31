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
    public class TicketService
    {
        private TableDAO<Ticket> mObjTicketDAO;

        public TicketService()
        {
            mObjTicketDAO = new TableDAO<Ticket>();
        }

        public int Add(Ticket pObjTicket)
        {
            return mObjTicketDAO.Add(pObjTicket);
        }

        public int Update(Ticket pObjTicket)
        {
            return mObjTicketDAO.Update(pObjTicket);
        }


        ///<summary>    Gets configuration code. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<param name="pStrConfigName">    Name of the string configuration. </param>
        ///<returns>    The configuration code. </returns>

        public string GetTicketCode(string pStrField, string pStrConfigName)
        {
            string lObjCode = "";
            Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //lLstStrParameters.Add("Field", pStrField);
                lLstStrParameters.Add("U_folio", pStrConfigName);

                lObjRecordset.DoQuery("SELECT Code FROM [@UG_PL_TCKT] WHERE U_Folio like '{U_folio}'".Inject(lLstStrParameters));

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
        ///<summary>    Exists. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="pStrFileName">  Filename of the string file. </param>
        ///<returns>    True if it succeeds, false if it fails. </returns>

        //    public bool Exist(string pStrFileName)
        //    {
        //        return new QueryManager().Exists("UG_IRPT", "U_FileName", pStrFileName);
        //    }
        //}
    }
}
