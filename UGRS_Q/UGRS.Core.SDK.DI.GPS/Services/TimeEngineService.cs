// file:	services\timeengineservice.cs
// summary:	Implements the timeengineservice class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.GPS.Tables;

namespace UGRS.Core.SDK.DI.GPS.Services
{
    /// <summary> A time engine service. </summary>
    /// <remarks> Ranaya, 08/05/2017. </remarks>

    public class TimeEngineService
    {
        /// <summary> The object time engine dao. </summary>
        private TableDAO<TimeEngine> mObjTimeEngineDAO;

        public TimeEngineService()
        {
            mObjTimeEngineDAO = new TableDAO<TimeEngine>();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(TimeEngine pObjRecord)
        {
            return mObjTimeEngineDAO.Add(pObjRecord);
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(TimeEngine pObjRecord)
        {
            return mObjTimeEngineDAO.Update(pObjRecord);
        }

        /// <summary> Removes this object. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>

        public int Remove(string pStrDocEntry)
        {
            return mObjTimeEngineDAO.Remove(pStrDocEntry);
        }
    }
}
