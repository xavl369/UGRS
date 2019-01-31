using System.ComponentModel.DataAnnotations.Schema;
using UGRS.Core.Auctions.Entities.Base;
using UGRS.Core.Auctions.Enums.System;

namespace UGRS.Core.Auctions.Entities.System
{
    [Table("Configurations", Schema = "SYSTEM")]
    public class Configuration : BaseEntity
    {
        public ConfigurationKeyEnum Key { get; set; }

        [Column(TypeName = "text")]
        public string Value { get; set; }
    }
}
