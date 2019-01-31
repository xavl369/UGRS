namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Batches_GenderField : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Batches", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AUCTIONS.Batches", "Gender");
        }
    }
}
