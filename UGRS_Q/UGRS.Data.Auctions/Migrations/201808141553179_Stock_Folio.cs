namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stock_Folio : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.Stocks", "EntryFolio", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.Stocks", "EntryFolio");
        }
    }
}
