namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeLine : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FINANCIALS.FoodChargeLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Factor = c.Single(nullable: false),
                        Days = c.Single(nullable: false),
                        Weight = c.Single(nullable: false),
                        WeightFood = c.Single(nullable: false),
                        FoodChargeId = c.Long(nullable: false),
                        Exported = c.Boolean(nullable: false),
                        ExportDate = c.DateTime(),
                        Protected = c.Boolean(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("FINANCIALS.FoodCharges", t => t.FoodChargeId, cascadeDelete: true)
                .Index(t => t.FoodChargeId);
            
            AddColumn("FINANCIALS.FoodCharges", "TotalQuantity", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "TotalWeightFood", c => c.Single(nullable: false));
            DropColumn("FINANCIALS.FoodCharges", "Quantity");
            DropColumn("FINANCIALS.FoodCharges", "Factor");
            DropColumn("FINANCIALS.FoodCharges", "Days");
            DropColumn("FINANCIALS.FoodCharges", "WeightFood");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.FoodCharges", "WeightFood", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Days", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Factor", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Quantity", c => c.Int(nullable: false));
            DropForeignKey("FINANCIALS.FoodChargeLines", "FoodChargeId", "FINANCIALS.FoodCharges");
            DropIndex("FINANCIALS.FoodChargeLines", new[] { "FoodChargeId" });
            DropColumn("FINANCIALS.FoodCharges", "TotalWeightFood");
            DropColumn("FINANCIALS.FoodCharges", "TotalQuantity");
            DropTable("FINANCIALS.FoodChargeLines");
        }
    }
}
