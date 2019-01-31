// file:	Services\ConfigurationService.cs
//
// summary:	Implements the configuration service class

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using UGRS.Core.Exceptions;
using UGRS.Core.Extension;
using UGRS.Core.SDK.DI.Configuration.Tables;
using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.Utility;

namespace UGRS.Core.SDK.DI.Configuration.Services
{
    public class ConfigurationService
    {
        ///<summary>    The object configuration dao. </summary>
        private TableDAO<Config> mObjConfigurationDAO;

        ///<summary>    Default constructor. </summary>
        ///<remarks>    Amartinez, 31/05/2017. </remarks>

        public ConfigurationService()
        {
            mObjConfigurationDAO = new TableDAO<Config>();
        }

        ///<summary>    Adds pObjConfig. </summary>
        ///<remarks>    Amartinez, 26/05/2017. </remarks>
        ///<param name="pObjConfig">    The Object Configuration to add. </param>
        ///<returns>    An int. </returns>

        public int Add(Config pObjConfig)
        {
            return mObjConfigurationDAO.Add(pObjConfig);
        }

        ///<summary>    Updates the given pObjConfig. </summary>
        ///<remarks>    Amartinez, 26/05/2017. </remarks>
        ///<param name="pObjConfig">    The Object Configuration to add. </param>
        ///<returns>    An int. </returns>

        public int Update(Config pObjConfig)
        {
            return mObjConfigurationDAO.Update(pObjConfig);
        }

        ///<summary>    Removes the given pStrName. </summary>
        ///<remarks>    Amartinez, 26/05/2017. </remarks>
        ///<param name="pStrName">  The String name to remove. </param>
        ///<returns>    An int. </returns>

        public int Remove(string pStrName)
        {
            return mObjConfigurationDAO.Remove(pStrName);
        }

        ///<summary>    Exists. </summary>
        ///<remarks>    Amartinez, 31/05/2017. </remarks>
        ///<param name="pStrFileName">  Filename of the string file. </param>
        ///<returns>    True if it succeeds, false if it fails. </returns>

        public bool Exist(string pStrFileName)
        {
            return new QueryManager().Exists("UG_CONFIG", "U_Name", pStrFileName);
        }

        ///<summary>    Gets configuration code. </summary>
        ///<remarks>    Amartinez, 01/06/2017. </remarks>
        ///<param name="pStrConfigName">    Name of the string configuration. </param>
        ///<returns>    The configuration code. </returns>

        public string GetConfigCode(string pStrField, string pStrConfigName)
        {
            string lObjCode = "";
            Recordset lObjRecordset = null;

            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //lLstStrParameters.Add("Field", pStrField);
                lLstStrParameters.Add("ConfigName", pStrConfigName);

                lObjRecordset.DoQuery("SELECT Code FROM [@UG_CONFIG] WHERE U_Name like '{ConfigName}'".Inject(lLstStrParameters));
                
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

        public string GetConfigValue(string pStrConfigName)
        {
            string lObjCode = "";
            Recordset lObjRecordset = null;
            try
            {
                lObjRecordset = (Recordset)DIApplication.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                Dictionary<string, string> lLstStrParameters = new Dictionary<string, string>();
                //lLstStrParameters.Add("Field", pStrField);
                lLstStrParameters.Add("ConfigName", pStrConfigName);

                lObjRecordset.DoQuery("SELECT U_Value FROM [@UG_CONFIG] WHERE U_Name like '{ConfigName}'".Inject(lLstStrParameters));

                if (lObjRecordset.RecordCount > 0)
                {
                    return lObjRecordset.Fields.Item("U_Value").Value.ToString();
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
