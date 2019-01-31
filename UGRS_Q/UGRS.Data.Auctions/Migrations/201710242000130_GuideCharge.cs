namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GuideCharge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FINANCIALS.GuideCharges",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        AuctionId = c.Long(nullable: false),
                        SellerId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Auctions", t => t.AuctionId, cascadeDelete: true)
                .ForeignKey("BUSINESS.Partners", t => t.SellerId, cascadeDelete: true)
                .Index(t => t.AuctionId)
                .Index(t => t.SellerId);
            
            CreateTable(
                "INVENTORY.ItemTypeDefinitions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuctionTypeId = c.Long(nullable: false),
                        ItemTypeId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("FINANCIALS.FoodCharges", "Exported", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "ExportDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("FINANCIALS.GuideCharges", "SellerId", "BUSINESS.Partners");
            DropForeignKey("FINANCIALS.GuideCharges", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("FINANCIALS.GuideCharges", new[] { "SellerId" });
            DropIndex("FINANCIALS.GuideCharges", new[] { "AuctionId" });
            DropColumn("FINANCIALS.FoodCharges", "ExportDate");
            DropColumn("FINANCIALS.FoodCharges", "Exported");
            DropTable("INVENTORY.ItemTypeDefinitions");
            DropTable("FINANCIALS.GuideCharges");
        }
    }
}
