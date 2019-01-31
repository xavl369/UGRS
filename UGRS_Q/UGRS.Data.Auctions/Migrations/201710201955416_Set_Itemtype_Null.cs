namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Set_Itemtype_Null : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("AUCTIONS.Batches", "ItemTypeId", "INVENTORY.ItemTypes");
            DropIndex("AUCTIONS.Batches", new[] { "ItemTypeId" });
            AlterColumn("AUCTIONS.Batches", "ItemTypeId", c => c.Long());
            CreateIndex("AUCTIONS.Batches", "ItemTypeId");
            AddForeignKey("AUCTIONS.Batches", "ItemTypeId", "INVENTORY.ItemTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AUCTIONS.Batches", "ItemTypeId", "INVENTORY.ItemTypes");
            DropIndex("AUCTIONS.Batches", new[] { "ItemTypeId" });
            AlterColumn("AUCTIONS.Batches", "ItemTypeId", c => c.Long(nullable: false));
            CreateIndex("AUCTIONS.Batches", "ItemTypeId");
            AddForeignKey("AUCTIONS.Batches", "ItemTypeId", "INVENTORY.ItemTypes", "Id", cascadeDelete: true);
        }
    }
}
