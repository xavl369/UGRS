using System.ComponentModel;

namespace UGRS.Core.Application.Enum.Service
{
    public enum AccessEnum : int
    {
        [DescriptionAttribute("BoardsService")]
        BOARDS = 1,
        [DescriptionAttribute("WeighingMachineService")]
        WEIGHING_MACHINE = 2,
    }
}
