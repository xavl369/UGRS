using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Enums.Inventory;

namespace UGRS.Core.Auctions.Entities.Inventory
{
    [Table("Documents", Schema = "INVENTORY")]
    public class Document : BaseEntity
    {
        public int Number { get; set; }

        [Column(TypeName = "varchar"), StringLength(10)]
        public string Folio { get; set; }

        public DocumentTypeEnum Type { get; set; }

        public long PartnerId { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }
    }
}
