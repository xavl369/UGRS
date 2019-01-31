namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovementsNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsIssues", "Number", c => c.Int(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "Number", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsReturns", "Number");
            DropColumn("INVENTORY.GoodsIssues", "Number");
        }
    }
}
