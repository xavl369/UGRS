namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auction_Field_Reprocessed : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Auctions", "ReProcessed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Auctions", "ReProcessed");
        }
    }
}
