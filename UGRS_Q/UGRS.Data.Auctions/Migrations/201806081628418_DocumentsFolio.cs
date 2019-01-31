namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentsFolio : DbMigration
    {
        public override void Up()
        {
            AlterColumn("INVENTORY.GoodsIssues", "Folio", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("INVENTORY.GoodsReturns", "Folio", c => c.String(maxLength: 10, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("INVENTORY.GoodsReturns", "Folio", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("INVENTORY.GoodsIssues", "Folio", c => c.String(maxLength: 50, unicode: false));
        }
    }
}
