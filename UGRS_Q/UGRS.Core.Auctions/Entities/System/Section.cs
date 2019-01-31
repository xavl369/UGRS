using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.System
{
    [Table("Sections", Schema = "SYSTEM")]
    public class Section : CatalogEntity
    {
        #region Entity properties

        public int Position { get; set; }

        [Column(TypeName = "varchar"), StringLength(100)]
        public string Path { get; set; }


        #endregion

        #region External properties

        public long ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        #endregion
    }
}
