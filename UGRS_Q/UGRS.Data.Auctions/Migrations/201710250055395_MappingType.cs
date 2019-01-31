namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingType : DbMigration
    {
        public override void Up()
        {
            AddColumn("BUSINESS.PartnerMappings", "Type", c => c.Int(nullable: false));
            DropColumn("BUSINESS.PartnerMappings", "Exists");
        }
        
        public override void Down()
        {
            AddColumn("BUSINESS.PartnerMappings", "Exists", c => c.Boolean(nullable: false));
            DropColumn("BUSINESS.PartnerMappings", "Type");
        }
    }
}
