using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Entities.System
{
    [Table("Changes", Schema = "SYSTEM")]
    public class Change : BaseEntity
    {
        #region Entity properties

        [Required]
        public ChangeTypeEnum ChangeType { get; set; }

        [Column(TypeName = "text"), Required]
        public string ObjectType { get; set; }

        public long ObjectId { get; set; }

        [Column(TypeName = "text"), Required]
        public string Object { get; set; }

        public DateTime Date { get; set; }

        #endregion

        #region External properties

        public long UserId { get; set; }

        #endregion
    }
}
