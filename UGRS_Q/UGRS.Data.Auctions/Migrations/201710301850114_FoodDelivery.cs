namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodDelivery : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE FINANCIALS.FoodCharges DROP CONSTRAINT DF__FoodCharg__Total__1F98B2C1");
            RenameTable(name: "FINANCIALS.DeliveriesFood", newName: "FoodDeliveries");
            AddColumn("FINANCIALS.FoodDeliveries", "TaxCode", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("FINANCIALS.FoodCharges", "TotalQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("FINANCIALS.FoodCharges", "TotalQuantity", c => c.Single(nullable: false));
            DropColumn("FINANCIALS.FoodDeliveries", "TaxCode");
            RenameTable(name: "FINANCIALS.FoodDeliveries", newName: "DeliveriesFood");
        }
    }
}
