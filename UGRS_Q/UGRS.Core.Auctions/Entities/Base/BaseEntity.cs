using System;
using System.ComponentModel.DataAnnotations;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Entities.Base
{
    public class BaseEntity
    {
        #region Entity properties

        [Key]
        public long Id { get; set; }

        public bool Protected { get; set; }

        public bool Removed { get; set; }

        public bool Active { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        #endregion
    }
}
