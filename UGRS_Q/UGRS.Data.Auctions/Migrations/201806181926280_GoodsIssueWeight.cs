namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodsIssueWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsIssues", "Weight", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsIssues", "Weight");
        }
    }
}
