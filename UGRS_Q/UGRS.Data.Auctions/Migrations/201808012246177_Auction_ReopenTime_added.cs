namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Auction_ReopenTime_added : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Auctions", "ReOpenedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Auctions", "ReOpenedTime");
        }
    }
}
