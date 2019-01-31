namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanAuctionCatalogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("AUCTIONS.Auctions", "CategoryId", "AUCTIONS.Categories");
            DropForeignKey("AUCTIONS.Auctions", "LocationId", "AUCTIONS.Locations");
            DropForeignKey("AUCTIONS.Auctions", "TypeId", "AUCTIONS.Types");
            DropIndex("AUCTIONS.Auctions", new[] { "LocationId" });
            DropIndex("AUCTIONS.Auctions", new[] { "TypeId" });
            DropIndex("AUCTIONS.Auctions", new[] { "CategoryId" });
            AddColumn("AUCTIONS.Auctions", "Type", c => c.Int(nullable: false));
            AddColumn("AUCTIONS.Auctions", "Category", c => c.Int(nullable: false));
            AddColumn("AUCTIONS.Auctions", "Location", c => c.Int(nullable: false));
            AddColumn("INVENTORY.ItemTypeDefinitions", "AuctionType", c => c.Int(nullable: false));
            DropColumn("AUCTIONS.Auctions", "LocationId");
            DropColumn("AUCTIONS.Auctions", "TypeId");
            DropColumn("AUCTIONS.Auctions", "CategoryId");
            DropColumn("INVENTORY.ItemTypeDefinitions", "AuctionTypeId");
            DropTable("AUCTIONS.Categories");
            DropTable("AUCTIONS.Locations");
            DropTable("AUCTIONS.Types");
        }
        
        public override void Down()
        {
            CreateTable(
                "AUCTIONS.Types",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        Description = c.String(maxLength: 250, unicode: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AUCTIONS.Locations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Abbreviation = c.String(maxLength: 5, unicode: false),
                        Name = c.String(maxLength: 100, unicode: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AUCTIONS.Categories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, unicode: false),
                        Description = c.String(maxLength: 100, unicode: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("INVENTORY.ItemTypeDefinitions", "AuctionTypeId", c => c.Long(nullable: false));
            AddColumn("AUCTIONS.Auctions", "CategoryId", c => c.Long(nullable: false));
            AddColumn("AUCTIONS.Auctions", "TypeId", c => c.Long(nullable: false));
            AddColumn("AUCTIONS.Auctions", "LocationId", c => c.Long(nullable: false));
            DropColumn("INVENTORY.ItemTypeDefinitions", "AuctionType");
            DropColumn("AUCTIONS.Auctions", "Location");
            DropColumn("AUCTIONS.Auctions", "Category");
            DropColumn("AUCTIONS.Auctions", "Type");
            CreateIndex("AUCTIONS.Auctions", "CategoryId");
            CreateIndex("AUCTIONS.Auctions", "TypeId");
            CreateIndex("AUCTIONS.Auctions", "LocationId");
            AddForeignKey("AUCTIONS.Auctions", "TypeId", "AUCTIONS.Types", "Id", cascadeDelete: true);
            AddForeignKey("AUCTIONS.Auctions", "LocationId", "AUCTIONS.Locations", "Id", cascadeDelete: true);
            AddForeignKey("AUCTIONS.Auctions", "CategoryId", "AUCTIONS.Categories", "Id", cascadeDelete: true);
        }
    }
}
