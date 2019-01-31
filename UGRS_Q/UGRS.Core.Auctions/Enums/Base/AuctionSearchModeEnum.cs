using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Base
{
    public enum AuctionSearchModeEnum : int
    {
        [DescriptionAttribute("Todas")]
        ALL = 0,
        [DescriptionAttribute("Subastas")]
        AUCTION = 1,
        [DescriptionAttribute("Trato directos")]
        DIRECT_TRADE = 2,
    }
}
