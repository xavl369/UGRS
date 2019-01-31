namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class weight_to_trade : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Trades", "Weight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Trades", "Weight");
        }
    }
}
