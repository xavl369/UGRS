namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodCharge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AUCTIONS.FoodCharge",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Folio = c.Int(nullable: false),
                        AuctionId = c.Long(nullable: false),
                        SellerId = c.Long(nullable: false),
                        Comision = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeight = c.Single(nullable: false),
                        AditionalWeight = c.Single(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Factor = c.Single(nullable: false),
                        Days = c.Single(nullable: false),
                        WeightFood = c.Single(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("AUCTIONS.FoodCharge", "SellerId", "BUSINESS.Partners");
            DropForeignKey("AUCTIONS.FoodCharge", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("AUCTIONS.FoodCharge", new[] { "SellerId" });
            DropIndex("AUCTIONS.FoodCharge", new[] { "AuctionId" });
            DropTable("AUCTIONS.FoodCharge");
        }
    }
}
