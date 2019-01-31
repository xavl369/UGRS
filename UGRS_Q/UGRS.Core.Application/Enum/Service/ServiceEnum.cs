using System.ComponentModel;

namespace UGRS.Core.Application.Enum.Service
{
    public enum ServiceEnum : int
    {
        [DescriptionAttribute("AuctionsService")]
        AUCTIONS = 1,
        [DescriptionAttribute("BoardsService")]
        BOARDS = 2,
        [DescriptionAttribute("WeighingMachineService")]
        WEIGHING_MACHINE = 3,
    }
}
