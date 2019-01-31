using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.DTO.System
{
    public class LogDTO
    {
        public long Id { get; set; }
        public ChangeTypeEnum ChangeType { get; set; }
        public long ObjectId { get; set; }
        public string Object { get; set; }
        public long UserId { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }
}
