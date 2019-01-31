namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Invoice_PayedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.Invoices", "Payed", c => c.Boolean(nullable: false));
            AddColumn("FINANCIALS.Invoices", "PayedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.Invoices", "PayedDate");
            DropColumn("FINANCIALS.Invoices", "Payed");
        }
    }
}
