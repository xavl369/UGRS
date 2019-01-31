namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchLineBatchNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.BatchLines", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("AUCTIONS.BatchLines", "BatchDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.BatchLines", "BatchDate");
            DropColumn("AUCTIONS.BatchLines", "BatchNumber");
        }
    }
}
