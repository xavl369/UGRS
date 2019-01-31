namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auctions_PreclosureTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Auctions", "ReOpened", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Auctions", "ReOpened");
        }
    }
}
