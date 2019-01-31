namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceFields_Payments : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.Invoices", "PaymentCondition", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PayMethod", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.Invoices", "MainUsage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.Invoices", "MainUsage");
            DropColumn("FINANCIALS.Invoices", "PayMethod");
            DropColumn("FINANCIALS.Invoices", "PaymentCondition");
        }
    }
}
