using UGRS.Core.SDK.DI.Configuration.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.Configuration.Services
{
    public class SetupService
    {
        ///<summary>    The object configuration DAO. </summary>
        private TableDAO<Config> mObjConfigurationDAO;


        ///<summary>    Default constructor. </summary>
        ///<remarks>    Amartinez, 30/05/2017. </remarks>

        public SetupService()
        {
            mObjConfigurationDAO = new TableDAO<Config>();
        }


        ///<summary>    Initializes the tables. </summary>
        ///<remarks>    Amartinez, 30/05/2017. </remarks>

        public void InitializeTables()
        {
            mObjConfigurationDAO.Initialize();
        }

    }
}