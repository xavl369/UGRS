namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auction_ReopenTime : DbMigration
    {
        public override void Up()
        {
            DropColumn("AUCTIONS.Auctions", "ReOpenedTime");
        }
        
        public override void Down()
        {
            AddColumn("AUCTIONS.Auctions", "ReOpenedTime", c => c.DateTime());
        }
    }
}
