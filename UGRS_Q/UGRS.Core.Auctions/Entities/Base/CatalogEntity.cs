using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UGRS.Core.Auctions.Entities.Base
{
    public class CatalogEntity : BaseEntity
    {
        #region Entity properties

        [Column(TypeName = "varchar"), StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        #endregion
    }
}
