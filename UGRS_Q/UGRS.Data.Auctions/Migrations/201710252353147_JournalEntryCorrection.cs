namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JournalEntryCorrection : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.JournalEntryLines", "CostingCode", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "DebitAccount", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "CreditAccount", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "Auxiliary", c => c.String(maxLength: 50, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "AuxiliaryType", c => c.Int(nullable: false));
            DropColumn("FINANCIALS.JournalEntryLines", "AccountCode");
            DropColumn("FINANCIALS.JournalEntryLines", "AccountName");
        }
        
        public override void Down()
        {
            AddColumn("FINANCIALS.JournalEntryLines", "AccountName", c => c.String(maxLength: 100, unicode: false));
            AddColumn("FINANCIALS.JournalEntryLines", "AccountCode", c => c.String(maxLength: 50, unicode: false));
            DropColumn("FINANCIALS.JournalEntryLines", "AuxiliaryType");
            DropColumn("FINANCIALS.JournalEntryLines", "Auxiliary");
            DropColumn("FINANCIALS.JournalEntryLines", "CreditAccount");
            DropColumn("FINANCIALS.JournalEntryLines", "DebitAccount");
            DropColumn("FINANCIALS.JournalEntryLines", "CostingCode");
        }
    }
}
