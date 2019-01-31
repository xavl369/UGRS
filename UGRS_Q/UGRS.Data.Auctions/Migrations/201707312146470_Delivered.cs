namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delivered : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsReturns", "Delivered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsReturns", "Delivered");
        }
    }
}
