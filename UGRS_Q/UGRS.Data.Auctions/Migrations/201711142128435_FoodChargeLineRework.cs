namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeLineRework : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.FoodChargeLines", "AverageWeight", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "Quantity", c => c.Int(nullable: false));
            DropColumn("FINANCIALS.FoodChargeLines", "BatchWeight");
            DropColumn("FINANCIALS.FoodChargeLines", "BatchQuantity");
            DropColumn("FINANCIALS.FoodChargeLines", "LineQuantity");
            DropColumn("FINANCIALS.FoodChargeLines", "Proportional");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.FoodChargeLines", "Proportional", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineQuantity", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "BatchQuantity", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "BatchWeight", c => c.Single(nullable: false));
            DropColumn("FINANCIALS.FoodChargeLines", "Quantity");
            DropColumn("FINANCIALS.FoodChargeLines", "AverageWeight");
        }
    }
}
