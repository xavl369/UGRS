namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stock_PaymentField : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.Stocks", "Payment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.Stocks", "Payment");
        }
    }
}
