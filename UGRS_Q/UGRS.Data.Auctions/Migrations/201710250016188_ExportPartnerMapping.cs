namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExportPartnerMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("BUSINESS.PartnerMappings", "Exported", c => c.Boolean(nullable: false));
            AddColumn("BUSINESS.PartnerMappings", "ExportDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("BUSINESS.PartnerMappings", "ExportDate");
            DropColumn("BUSINESS.PartnerMappings", "Exported");
        }
    }
}
