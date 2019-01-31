using System;

namespace UGRS.Core.Auctions.Entities.Base
{
    public class DocumentEntity : ExportEntity
    {
        public bool Opened { get; set; }

        public bool Canceled { get; set; }

        public bool Processed { get; set; }

        public DateTime? ProcessedDate { get; set; }
    }
}
