namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodsReceiptDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsReceipts", "ExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsReceipts", "ExpirationDate");
        }
    }
}
