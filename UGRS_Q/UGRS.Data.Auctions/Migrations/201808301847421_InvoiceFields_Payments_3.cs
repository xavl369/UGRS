namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceFields_Payments_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.Invoices", "Payment", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PaymentCondition", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PayMethod", c => c.String());
            AddColumn("FINANCIALS.Invoices", "MainUsage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.Invoices", "MainUsage");
            DropColumn("FINANCIALS.Invoices", "PayMethod");
            DropColumn("FINANCIALS.Invoices", "PaymentCondition");
            DropColumn("FINANCIALS.Invoices", "Payment");
        }
    }
}
