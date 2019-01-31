namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Batch_SellType_Field : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Batches", "SellType", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Batches", "SellType");
        }
    }
}
