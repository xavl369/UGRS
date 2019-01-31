namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Batches_GenderField_changed : DbMigration
    {
        public override void Up()
        {
            DropColumn("AUCTIONS.Batches", "Gender");
        }
        
        public override void Down()
        {
            AddColumn("AUCTIONS.Batches", "Gender", c => c.Int(nullable: false));
        }
    }
}
