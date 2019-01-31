using System;
using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Auctions
{
    public enum AuctionTypeEnum : int
    {
        [DescriptionAttribute("Bovino")]
        BOVINE = 1,
        [DescriptionAttribute("Equino")]
        EQUINE = 2,
        [DescriptionAttribute("Caprino")]
        CAPRINE = 3,
        [DescriptionAttribute("Ovino")]
        OVINE = 4
    }
}
