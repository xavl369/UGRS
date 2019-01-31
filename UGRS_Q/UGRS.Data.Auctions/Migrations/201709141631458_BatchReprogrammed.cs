namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchReprogrammed : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Batches", "Reprogrammed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Batches", "Reprogrammed");
        }
    }
}
