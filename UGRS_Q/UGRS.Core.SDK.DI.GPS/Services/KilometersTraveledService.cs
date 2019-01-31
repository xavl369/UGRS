// file:	services\kilometerstraveledservice.cs
// summary:	Implements the kilometerstraveledservice class

using UGRS.Core.SDK.DI.DAO;
using UGRS.Core.SDK.DI.GPS.Tables;

namespace UGRS.Core.SDK.DI.GPS.Services
{
    /// <summary> The kilometers traveled service. </summary>
    /// <remarks> Ranaya, 08/05/2017. </remarks>

    public class KilometersTraveledService
    {
        /// <summary> The object kilometers traveled dao. </summary>
        private TableDAO<KilometersTraveled> mObjKilometersTraveledDAO;

        public KilometersTraveledService()
        {
            mObjKilometersTraveledDAO = new TableDAO<KilometersTraveled>();
        }

        /// <summary> Adds pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Add(KilometersTraveled pObjRecord)
        {
            return mObjKilometersTraveledDAO.Add(pObjRecord);
        }

        /// <summary> Updates the given pObjRecord. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pObjRecord"> The Object record to add. </param>
        /// <returns> An int. </returns>

        public int Update(KilometersTraveled pObjRecord)
        {
            return mObjKilometersTraveledDAO.Update(pObjRecord);
        }

        /// <summary> Removes this object. </summary>
        /// <remarks> Ranaya, 08/05/2017. </remarks>
        /// <param name="pStrDocEntry"> The String Document entry to remove. </param>

        public int Remove(string pStrDocEntry)
        {
            return mObjKilometersTraveledDAO.Remove(pStrDocEntry);
        }
    }
}
