// file:	Services\SetupService.cs
// summary:	Implements the setup service class

using UGRS.Core.SDK.DI.Auctions.Tables;
using UGRS.Core.SDK.DI.DAO;

namespace UGRS.Core.SDK.DI.Auctions.Services
{
    public class SetupService
    {
        /// <summary> The object action dao. </summary>
        private TableDAO<Auction> mObjAuctionDAO;

        /// <summary> The object batch dao. </summary>
        private TableDAO<Batch> mObjBatchDAO;

        /// <summary> The object batch dao. </summary>
        private TableDAO<BatchLine> mObjBatchLineDAO;

        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public SetupService()
        {
            mObjAuctionDAO = new TableDAO<Auction>();
            mObjBatchDAO = new TableDAO<Batch>();
            mObjBatchLineDAO = new TableDAO<BatchLine>();
        }

        /// <summary> Initializes the tables. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public void InitializeTablesAndFields()
        {
            mObjAuctionDAO.Initialize();
            mObjBatchDAO.Initialize();
            mObjBatchLineDAO.Initialize();
        }
    }
}
