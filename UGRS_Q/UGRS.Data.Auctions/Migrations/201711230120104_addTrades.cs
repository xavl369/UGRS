namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTrades : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AUCTIONS.Trades",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SellerId = c.Long(),
                        BuyerId = c.Long(),
                        AuctionId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("AUCTIONS.Auctions", t => t.AuctionId, cascadeDelete: true)
                .ForeignKey("BUSINESS.Partners", t => t.BuyerId)
                .ForeignKey("BUSINESS.Partners", t => t.SellerId)
                .Index(t => t.SellerId)
                .Index(t => t.BuyerId)
                .Index(t => t.AuctionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("AUCTIONS.Trades", "SellerId", "BUSINESS.Partners");
            DropForeignKey("AUCTIONS.Trades", "BuyerId", "BUSINESS.Partners");
            DropForeignKey("AUCTIONS.Trades", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("AUCTIONS.Trades", new[] { "AuctionId" });
            DropIndex("AUCTIONS.Trades", new[] { "BuyerId" });
            DropIndex("AUCTIONS.Trades", new[] { "SellerId" });
            DropTable("AUCTIONS.Trades");
        }
    }
}
