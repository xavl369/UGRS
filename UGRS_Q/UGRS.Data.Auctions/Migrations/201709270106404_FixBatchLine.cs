namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixBatchLine : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("AUCTIONS.Batches", "Item_Id", "INVENTORY.Items");
            DropIndex("AUCTIONS.Batches", new[] { "Item_Id" });
            DropColumn("AUCTIONS.Batches", "Item_Id");
        }
        
        public override void Down()
        {
            AddColumn("AUCTIONS.Batches", "Item_Id", c => c.Long());
            CreateIndex("AUCTIONS.Batches", "Item_Id");
            AddForeignKey("AUCTIONS.Batches", "Item_Id", "INVENTORY.Items", "Id");
        }
    }
}
