namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeCheckCorrection : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "FINANCIALS.FoodChargeCheckLists", newName: "FoodChargeChecks");
        }
        
        public override void Down()
        {
            RenameTable(name: "FINANCIALS.FoodChargeChecks", newName: "FoodChargeCheckLists");
        }
    }
}
