using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.Entities.Base
{
    public class TemporaryEntity : BaseEntity
    {
        public bool Temporary { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
