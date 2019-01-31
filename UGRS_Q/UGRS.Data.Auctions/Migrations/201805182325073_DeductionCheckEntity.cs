namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeductionCheckEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FINANCIALS.DeductionChecks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuctionId = c.Long(nullable: false),
                        SellerId = c.Long(nullable: false),
                        Deduct = c.Boolean(nullable: false),
                        Comments = c.String(),
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
            DropForeignKey("FINANCIALS.DeductionChecks", "SellerId", "BUSINESS.Partners");
            DropForeignKey("FINANCIALS.DeductionChecks", "AuctionId", "AUCTIONS.Auctions");
            DropIndex("FINANCIALS.DeductionChecks", new[] { "SellerId" });
            DropIndex("FINANCIALS.DeductionChecks", new[] { "AuctionId" });
            DropTable("FINANCIALS.DeductionChecks");
        }
    }
}
