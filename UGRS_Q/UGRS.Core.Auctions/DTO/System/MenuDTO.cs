using System.Collections.Generic;

namespace UGRS.Core.Auctions.DTO.System
{
    public class MenuDTO
    {
        public MenuDTO()
        {
            Children = new List<MenuDTO>();
        }

        public string Name { get; set; }
        public string Path { get; set; }

        public IList<MenuDTO> Children { get; set; }
    }
}
