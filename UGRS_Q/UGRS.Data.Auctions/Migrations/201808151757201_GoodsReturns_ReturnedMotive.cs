namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodsReturns_ReturnedMotive : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsReturns", "ReturnMotive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsReturns", "ReturnMotive");
        }
    }
}
