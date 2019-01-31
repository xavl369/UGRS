using UGRS.Core.SDK.DI.FoodProduction.Services;

namespace UGRS.Core.SDK.DI.FoodProduction
{
    public class FoodProductionSeviceFactory
    {

        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public TicketService GetTicketService()
        {
            return new TicketService();
        }

        public TicketDetailService GetTicketDetailService()
        {
            return new TicketDetailService();
        }
    }
}
