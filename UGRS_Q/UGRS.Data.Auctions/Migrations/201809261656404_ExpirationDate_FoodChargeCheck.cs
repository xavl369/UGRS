namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpirationDate_FoodChargeCheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.FoodChargeChecks", "ExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.FoodChargeChecks", "ExpirationDate");
        }
    }
}
