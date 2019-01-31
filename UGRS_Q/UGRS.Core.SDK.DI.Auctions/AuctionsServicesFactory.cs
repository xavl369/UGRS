// file:	auctionsfactoryservices.cs
// summary:	Implements the auctionsfactoryservices class

using UGRS.Core.SDK.DI.Auctions.Services;
using UGRS.Core.SDK.DI.Models;

namespace UGRS.Core.SDK.DI.Auctions
{
    /// <summary> The auctions factory services. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    public class AuctionsServicesFactory
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 24/05/2017. </remarks>

        public AuctionsServicesFactory()
        {
            DIApplication.DIConnect();
        }

        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public AuctionService GetAuctionService()
        {
            return new AuctionService();
        }

        public AuctionBatchService GetAuctionBatchService()
        {
            return new AuctionBatchService();
        }

        public BusinessPartnerSevice GetBusinessPartnerSevice()
        {
            return new BusinessPartnerSevice();
        }

        public ItemService GetItemService()
        {
            return new ItemService();
        }

        public ItemBatchService GetItemBatchService()
        {
            return new ItemBatchService();
        }

        public FinancialsService GetFinanacialsService()
        {
            return new FinancialsService();
        }

        public DeliveryFoodService GetDeliveryFoodService()
        {
            return new DeliveryFoodService();
        }
    }
}
