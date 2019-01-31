namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeCheckList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FINANCIALS.FoodChargeCheckLists",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BatchNumber = c.String(maxLength: 100, unicode: false),
                        BatchDate = c.DateTime(nullable: false),
                        FoodCharge = c.Boolean(nullable: false),
                        FoodDeliveries = c.Boolean(nullable: false),
                        AlfalfaDeliveries = c.Boolean(nullable: false),
                        ApplyFoodCharge = c.Boolean(nullable: false),
                        SellerId = c.Long(nullable: false),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BUSINESS.Partners", t => t.SellerId, cascadeDelete: true)
                .Index(t => t.SellerId);
            
            AddColumn("FINANCIALS.FoodDeliveries", "BatchNumber", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropForeignKey("FINANCIALS.FoodChargeCheckLists", "SellerId", "BUSINESS.Partners");
            DropIndex("FINANCIALS.FoodChargeCheckLists", new[] { "SellerId" });
            DropColumn("FINANCIALS.FoodDeliveries", "BatchNumber");
            DropTable("FINANCIALS.FoodChargeCheckLists");
        }
    }
}
