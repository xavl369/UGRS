namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JournalLines_Comments : DbMigration
    {
        public override void Up()
        {
            AddColumn("FINANCIALS.JournalEntryLines", "Commentaries", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FINANCIALS.JournalEntryLines", "Commentaries");
        }
    }
}
