namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeLineCorrection : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.FoodChargeLines", "LineEntryDate", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineSaleDate", c => c.DateTime(nullable: false));
            DropColumn("FINANCIALS.FoodChargeLines", "LineDateEntry");
            DropColumn("FINANCIALS.FoodChargeLines", "LineDateSale");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.FoodChargeLines", "LineDateSale", c => c.DateTime(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "LineDateEntry", c => c.DateTime(nullable: false));
            DropColumn("FINANCIALS.FoodChargeLines", "LineSaleDate");
            DropColumn("FINANCIALS.FoodChargeLines", "LineEntryDate");
        }
    }
}
