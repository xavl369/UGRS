// file:	services\setupservice.cs
// summary:	Implements the setup service class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.GPS.Tables;

namespace UGRS.Core.SDK.DI.GPS.Services
{
    public class SetupService
    {
        /// <summary> The object imported report DAO. </summary>
        private TableDAO<ImportedReport> mObjImportedReportDAO;

        /// <summary> The object kilometers traveled DAO. </summary>
        private TableDAO<KilometersTraveled> mObjKilometersTraveledDAO;

        /// <summary> The object time engine DAO. </summary>
        private TableDAO<TimeEngine> mObjTimeEngineDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public SetupService()
        {
            mObjImportedReportDAO = new TableDAO<ImportedReport>();
            mObjKilometersTraveledDAO = new TableDAO<KilometersTraveled>();
            mObjTimeEngineDAO = new TableDAO<TimeEngine>();
        }

        /// <summary> Initializes the tables. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public void InitializeTables()
        {
            mObjImportedReportDAO.Initialize();
            mObjKilometersTraveledDAO.Initialize();
            mObjTimeEngineDAO.Initialize();
        }
    }
}
