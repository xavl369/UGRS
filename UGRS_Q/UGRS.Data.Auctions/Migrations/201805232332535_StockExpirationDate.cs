namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StockExpirationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.Stocks", "ExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.Stocks", "ExpirationDate");
        }
    }
}
