namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceLine_DeliveryLines : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.InvoiceLines", "DocLine", c => c.Int(nullable: false));
            AddColumn("FINANCIALS.InvoiceLines", "DocType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.InvoiceLines", "DocType");
            DropColumn("FINANCIALS.InvoiceLines", "DocLine");
        }
    }
}
