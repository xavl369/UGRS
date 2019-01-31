using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Auctions.Enums.Security;

namespace UGRS.Core.Auctions.Entities.Security
{
    [Table("Authorizations", Schema = "SECURITY")]
    public class Authorization : BaseEntity
    {
        #region Entity properties

        public SpecialFunctionsEnum Function { get; set; }

        [Column(TypeName = "varchar"), StringLength(500)]
        public string Comment { get; set; }

        #endregion

        #region External properties

        public long BatchId { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        #endregion
    }
}
