namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchLine : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("AUCTIONS.Batches", "ItemId", "INVENTORY.Items");
            DropIndex("AUCTIONS.Batches", new[] { "ItemId" });
            RenameColumn(table: "AUCTIONS.Batches", name: "ItemId", newName: "Item_Id");
            CreateTable(
                "AUCTIONS.BatchLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        ItemId = c.Long(nullable: false),
                        BatchId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Batches", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("INVENTORY.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.BatchId);
            
            AlterColumn("AUCTIONS.Batches", "Item_Id", c => c.Long());
            CreateIndex("AUCTIONS.Batches", "Item_Id");
            AddForeignKey("AUCTIONS.Batches", "Item_Id", "INVENTORY.Items", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AUCTIONS.Batches", "Item_Id", "INVENTORY.Items");
            DropForeignKey("AUCTIONS.BatchLines", "ItemId", "INVENTORY.Items");
            DropForeignKey("AUCTIONS.BatchLines", "BatchId", "AUCTIONS.Batches");
            DropIndex("AUCTIONS.BatchLines", new[] { "BatchId" });
            DropIndex("AUCTIONS.BatchLines", new[] { "ItemId" });
            DropIndex("AUCTIONS.Batches", new[] { "Item_Id" });
            AlterColumn("AUCTIONS.Batches", "Item_Id", c => c.Long(nullable: false));
            DropTable("AUCTIONS.BatchLines");
            RenameColumn(table: "AUCTIONS.Batches", name: "Item_Id", newName: "ItemId");
            CreateIndex("AUCTIONS.Batches", "ItemId");
            AddForeignKey("AUCTIONS.Batches", "ItemId", "INVENTORY.Items", "Id", cascadeDelete: true);
        }
    }
}
