namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Trade_BaseEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("AUCTIONS.Trades", "Exported");
            DropColumn("AUCTIONS.Trades", "ExportDate");
        }
        
        public override void Down()
        {
            AddColumn("AUCTIONS.Trades", "ExportDate", c => c.DateTime());
            AddColumn("AUCTIONS.Trades", "Exported", c => c.Boolean(nullable: false));
        }
    }
}
