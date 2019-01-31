using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.Core.Auctions.Entities.Base
{
    public class ExportEntity : BaseEntity
    {
        public bool Exported { get; set; }

        public DateTime? ExportDate { get; set; }
    }
}
