namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JournalEntryCorrection2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.JournalEntryLines", "AccountCode", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "ContraAccount", c => c.String(maxLength: 50, unicode: false));
            DropColumn("FINANCIALS.JournalEntryLines", "DebitAccount");
            DropColumn("FINANCIALS.JournalEntryLines", "CreditAccount");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.JournalEntryLines", "CreditAccount", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "DebitAccount", c => c.String(maxLength: 50, unicode: false));
            DropColumn("FINANCIALS.JournalEntryLines", "ContraAccount");
            DropColumn("FINANCIALS.JournalEntryLines", "AccountCode");
        }
    }
}
