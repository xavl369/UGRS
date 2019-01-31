namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeLine1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.FoodCharges", "TotalFoodWeight", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "BatchWeight", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "BatchQuantity", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineQuantity", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineDateEntry", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineDateSale", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "Proportional", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "FoodWeight", c => c.Single(nullable: false));
            DropColumn("FINANCIALS.FoodCharges", "TotalWeightFood");
            DropColumn("FINANCIALS.FoodChargeLines", "Quantity");
            DropColumn("FINANCIALS.FoodChargeLines", "Weight");
            DropColumn("FINANCIALS.FoodChargeLines", "WeightFood");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.FoodChargeLines", "WeightFood", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "Weight", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "Quantity", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "TotalWeightFood", c => c.Single(nullable: false));
            DropColumn("FINANCIALS.FoodChargeLines", "FoodWeight");
            DropColumn("FINANCIALS.FoodChargeLines", "Proportional");
            DropColumn("FINANCIALS.FoodChargeLines", "LineDateSale");
            DropColumn("FINANCIALS.FoodChargeLines", "LineDateEntry");
            DropColumn("FINANCIALS.FoodChargeLines", "LineQuantity");
            DropColumn("FINANCIALS.FoodChargeLines", "BatchQuantity");
            DropColumn("FINANCIALS.FoodChargeLines", "BatchWeight");
            DropColumn("FINANCIALS.FoodCharges", "TotalFoodWeight");
        }
    }
}
