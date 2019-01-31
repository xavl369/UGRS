namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignName : DbMigration
    {
        public override void Up()
        {
            AddColumn("BUSINESS.Partners", "ForeignName", c => c.String(nullable: false, maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("BUSINESS.Partners", "ForeignName");
        }
    }
}
