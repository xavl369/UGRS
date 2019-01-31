namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoodChargeLineCorrection2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.FoodCharges", "Batches", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "AuctionBatch", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.FoodChargeLines", "ItemBatch", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.FoodChargeLines", "ItemBatch");
            DropColumn("FINANCIALS.FoodChargeLines", "AuctionBatch");
            DropColumn("FINANCIALS.FoodCharges", "Batches");
        }
    }
}
