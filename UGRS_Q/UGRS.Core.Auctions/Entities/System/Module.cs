using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;

namespace UGRS.Core.Auctions.Entities.System
{
    [Table("Modules", Schema = "SYSTEM")]
    public class Module : CatalogEntity
    {
        #region Entity properties

        public int Position { get; set; }

        [Column(TypeName = "varchar"), StringLength(50)]
        public string Icon { get; set; }

        [Column(TypeName = "varchar"), StringLength(100)]
        public string Path { get; set; }

        #endregion

        #region External properties

        public virtual IList<Section> Sections { get; set; }

        #endregion
    }
}
