namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodsReturnWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsReturns", "Weight", c => c.Single(nullable: false));
            DropColumn("INVENTORY.GoodsIssues", "Weight");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.GoodsIssues", "Weight", c => c.Single(nullable: false));
            DropColumn("INVENTORY.GoodsReturns", "Weight");
        }
    }
}
