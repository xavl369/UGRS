using System;
using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Auctions
{
    public enum AuctionCategoryEnum : int
    {
        [DescriptionAttribute("Subasta")]
        AUCTION = 1,
        [DescriptionAttribute("Trato directo")]
        DIRECT_TRADE = 2,
        //[DescriptionAttribute("Borregos")]
        //SHEEP = 3,
    }
}
