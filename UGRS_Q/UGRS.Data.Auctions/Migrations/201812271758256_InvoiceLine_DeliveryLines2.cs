namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceLine_DeliveryLines2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.InvoiceLines", "DocNum", c => c.Int(nullable: false));
            DropColumn("FINANCIALS.InvoiceLines", "DocLine");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.InvoiceLines", "DocLine", c => c.Int(nullable: false));
            DropColumn("FINANCIALS.InvoiceLines", "DocNum");
        }
    }
}
