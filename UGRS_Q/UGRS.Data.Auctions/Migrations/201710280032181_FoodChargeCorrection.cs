namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeCorrection : DbMigration
    {
        public override void Up()
        {
            DropColumn("FINANCIALS.FoodCharges", "Comision");
            DropColumn("FINANCIALS.FoodCharges", "AditionalWeight");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.FoodCharges", "AditionalWeight", c => c.Single(nullable: false));
            AddColumn("FINANCIALS.FoodCharges", "Comision", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
