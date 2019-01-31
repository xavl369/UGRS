// file:	Services\ImportedReportService.cs
// summary:	Implements the imported report service class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.GPS.Tables;

namespace UGRS.Core.SDK.DI.GPS.Services
{
    /// <summary> An imported report service. </summary>
    /// <remarks> Ranaya, 08/05/2017. </remarks>

    public class ImportedReportService
    {
        /// <summary> The object kilometers traveled dao. </summary>
        private TableDAO<ImportedReport> mObjImportedReportDAO;

        public ImportedReportService()
        {
            mObjImportedReportDAO = new TableDAO<ImportedReport>();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(ImportedReport pObjRecord)
        {
            return mObjImportedReportDAO.Add(pObjRecord);
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(ImportedReport pObjRecord)
        {
            return mObjImportedReportDAO.Update(pObjRecord);
        }

        /// <summary> Removes this object. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>

        public int Remove(string pStrDocEntry)
        {
            return mObjImportedReportDAO.Remove(pStrDocEntry);
        }

        ///<summary>    Exists. </summary>
        ///<remarks>    Amartinez, 22/05/2017. </remarks>
        ///<param name="pStrFileName">  Filename of the string file. </param>
        ///<returns>    True if it succeeds, false if it fails. </returns>

        public bool Exist(string pStrFileName)
        {
            return new QueryManager().Exists("U_FileName", pStrFileName, "[@UG_IRPT]");
        }
    }
}
