using System;
using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.Auctions
{
    public enum LocationEnum : int
    {
        [DescriptionAttribute("Hermosillo")]
        HERMOSILLO = 1,
        [DescriptionAttribute("Sonora sur")]
        SONORA_SUR = 2
    }
}
