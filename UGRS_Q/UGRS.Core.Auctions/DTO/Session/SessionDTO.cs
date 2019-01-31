using System.Collections.Generic;

namespace UGRS.Core.Auctions.DTO.Session
{
    public class SessionDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, object> Objects { get; set; }
    }
}
