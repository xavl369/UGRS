namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceFields_Payments_2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("FINANCIALS.Invoices", "PaymentCondition");
            DropColumn("FINANCIALS.Invoices", "PayMethod");
            DropColumn("FINANCIALS.Invoices", "MainUsage");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.Invoices", "MainUsage", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PayMethod", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PaymentCondition", c => c.Int(nullable: false));
        }
    }
}
