namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceCreditPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.Invoices", "CreditPayment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.Invoices", "CreditPayment");
        }
    }
}
